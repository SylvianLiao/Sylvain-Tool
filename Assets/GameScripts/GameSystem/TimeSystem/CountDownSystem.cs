using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CdTimer
{
    public string m_CdTimerTicket;
    private DateTime m_AdjustTime;

    public delegate void 	OnTimesUpEvent(string ticket);
	public OnTimesUpEvent	onTimesUp;				    //倒數計時結束後的監聽事件
	    
	public bool 			IsStart;
	public bool 			m_EverSuspended;			//是否曾經因App縮小而中斷過
	public DateTime 		m_LastUpdateTime; 		    //暫存上次更新時間, 以便倒數中斷恢復時重算倒數時間
	public float 			m_CountDownTime;			//倒數中的時間
	public float 			m_CountDownLimit;			//倒數上限時間
	//-----------------------------------------------------------------------------------------------------
	private CdTimer(string ticket, DateTime adjustTime)
    {
        m_CdTimerTicket = ticket;
        m_AdjustTime = adjustTime;
        Init();
	}
	//-----------------------------------------------------------------------------------------------------
	static public CdTimer CreateCdTimer(string ticket, DateTime adjustTime)
	{
        return new CdTimer(ticket, adjustTime);
	}
	//-----------------------------------------------------------------------------------------------------
	public void Init()
	{
        IsStart         = false;
        m_EverSuspended    = false;
        m_CountDownTime    = 0.0f;
        m_CountDownLimit   = 0.0f;
		onTimesUp 		   = null;
	}
	//-----------------------------------------------------------------------------------------------------
	//取得剩餘時間
	public int Getm_CountDownTime(Enum_TimeFormat emType = Enum_TimeFormat.Hour)
	{
		int hours = 0;
		int minute = 0;
		int seconds = 0;
		hours = (int)(m_CountDownTime / 3600);
		minute = (int)(m_CountDownTime / 60);
		seconds = (int)m_CountDownTime;
		int cdTime = 0;

		switch(emType)
		{
		case Enum_TimeFormat.Second:
			cdTime = seconds;
			break;
		case Enum_TimeFormat.Minute:
			cdTime = minute;
			break;
		case Enum_TimeFormat.Hour:
			cdTime = hours;
			break;
		}
		return cdTime;
	}

	//-----------------------------------------------------------------------------------------------------
	//計算上次更新時間到目前經過多少秒
	private float PassTime(DateTime m_LastUpdateTime, DateTime baseTime)
	{
		//經過秒數 = 上次更新時間 - 目前時間
		float timeStamp = (float)(((TimeSpan)baseTime.Subtract(m_LastUpdateTime)).TotalSeconds);
		//校正時間錯誤,若client時間比server還慢,則將更新時間設為同步時的client時間
		if (timeStamp < 0)
		{
			m_LastUpdateTime = baseTime;
			timeStamp = 0.0f;
		}
		return timeStamp;
	}
	//----------------------------------------------------------------------------------------------------- 
	//根據紀錄的上次更新時間同步倒數時間
	public void SyncCdTime(bool isReplace = false)
	{
		SyncCdTime(m_LastUpdateTime, isReplace);
	}
	//-----------------------------------------------------------------------------------------------------
	/// <summary>根據傳入時間重新同步倒數時間(主要用於server重新同步時間用)</summary>
	/// 注意! 若傳入時間與現在時間距離過久計算量會過於龐大, 則此Function不會執行CD到時該執行的內容, 請盡量避免此狀況! 
	public void SyncCdTime(DateTime UpdateTime,bool isReplace = false)
	{
		if (isReplace)
			m_LastUpdateTime = UpdateTime;

		DateTime adjustClientTime = m_AdjustTime;
		float timeStamp = PassTime(UpdateTime,adjustClientTime);
		if (timeStamp > m_CountDownLimit)
		{
			int shouldUpdate = 0;
			shouldUpdate = (int)(timeStamp / m_CountDownLimit);	//該值該更新的次數 = 上次該值更新至現在的秒數 / 該值CD時間 
			if (shouldUpdate > 9999)
			{
				UnityDebugger.Debugger.Log("CountDownSysten: SyncTime need to update ="+shouldUpdate+",lead SyncCdTime() function to interrupted!");
			}
			else
			{
				for(int m = 0 ; m < shouldUpdate ; ++m)
				{
					if (onTimesUp != null)
						onTimesUp(m_CdTimerTicket);
				}
			}
			timeStamp = timeStamp % m_CountDownLimit;
		}
		//倒數時間 = 倒數上限 - 已經過時間
		m_CountDownTime = m_CountDownLimit - timeStamp;
	}
}
/*
※使用注意! 計時器關閉後不會從Map中刪除，會永久留存!
*/
//-----------------------------------------------------------------------------------------------------
public class CountDownSystem : BaseSystem
{
    //計算frame單位時間用
    private DateTime 				m_lastFrameTime;
	//Client與Server時間的誤差值
	public TimeSpan 				m_differTimeSpan;
	//預設計時器數量
	//-----------------------------------------------------------------------------------------------------	
	public Dictionary<string ,CdTimer>  m_CdTimerMap = new Dictionary<string, CdTimer>();//啟動中的倒數計時器編號管理器 <識別用字串, 倒數計時管理器>	
	//-----------------------------------------------------------------------------------------------------
	public CountDownSystem(GameScripts.GameFramework.GameApplication app):base(app) {}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		m_differTimeSpan = TimeSpan.Zero;
	}
    //-----------------------------------------------------------------------------------------------------
    /// <summary>
    /// 開啟倒數計時功能,回傳該計時器(需要跟Server連動的版本)
    /// </summary>
    public CdTimer StartCountDown(string ticket, DateTime m_LastUpdateTime , float cdLimit , CdTimer.OnTimesUpEvent onTimesUp)
	{
        CdTimer cdTimer = (m_CdTimerMap.ContainsKey(ticket)) ? m_CdTimerMap[ticket] : CdTimer.CreateCdTimer(ticket, GetAdjustedClientTime());
        cdTimer.m_CdTimerTicket = ticket;
		cdTimer.IsStart = true;
		cdTimer.m_LastUpdateTime = m_LastUpdateTime;
		cdTimer.m_CountDownLimit = cdLimit;
		cdTimer.onTimesUp = onTimesUp;
		cdTimer.SyncCdTime();

        if (!m_CdTimerMap.ContainsKey(ticket))
            m_CdTimerMap.Add(ticket, cdTimer);
		return cdTimer;
	}
	//-----------------------------------------------------------------------------------------------------
	/// <summary>
	/// 開啟倒數計時功能,回傳該計時器(純Client自己計時的版本)
	/// </summary>
	public CdTimer StartCountDown(string ticket, float cdLimit , CdTimer.OnTimesUpEvent onTimesUp)
	{
		return StartCountDown(ticket, GetAdjustedClientTime(),cdLimit,onTimesUp);
	}
    //-----------------------------------------------------------------------------------------------------
    /// <summary>
    /// 關閉倒數計時功能
    /// </summary>
    public void CloseCountDown(string ticket)
    {
        if (!m_CdTimerMap.ContainsKey(ticket))
        {
            UnityDebugger.Debugger.Log("CdTimer Close Failed , there is no " + ticket + " CdTimer");
            return;
        }
        m_CdTimerMap[ticket].IsStart = false;
    }
    //-----------------------------------------------------------------------------------------------------
    public override void Update()
	{
		if (m_lastFrameTime == DateTime.MinValue)
			m_lastFrameTime = DateTime.UtcNow;

		DateTime now = DateTime.UtcNow;
		TimeSpan detla = now - m_lastFrameTime;
		
		foreach(KeyValuePair<string ,CdTimer> data in m_CdTimerMap)
		{
			//檢查倒數計時開關是否打開
			if (data.Value.IsStart)
			{
				if (data.Value.m_EverSuspended)
				{
					data.Value.SyncCdTime();
					data.Value.m_EverSuspended = false;
				}
                CountDown(data.Value.m_CdTimerTicket,
                        ref data.Value.m_CountDownTime,
                        data.Value.m_CountDownLimit,
                        data.Value.onTimesUp,
                        (float)detla.TotalSeconds,
                        ref data.Value.m_LastUpdateTime);
            }
		}
		
		m_lastFrameTime = now;
	}
	//-----------------------------------------------------------------------------------------------------
	private void CountDown(string ticket, ref float cdTime , float cdLimit , CdTimer.OnTimesUpEvent onTimeup, float deltaTime,ref DateTime m_LastUpdateTime)
	{
		if(cdTime > 0.0f)
		{
			cdTime -= deltaTime;

			//更新角色ap
			if(cdTime <= 0.0f)
			{
				if (onTimeup != null)
					onTimeup(ticket);
				cdTime = cdLimit;
				m_LastUpdateTime = GetAdjustedClientTime();
			}
		}//if
		else 	//以防萬一的寫法
		{
			//重新計時
			cdTime = cdLimit;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//取得計時器
	public CdTimer GetCdTimer(string ticket)
	{
		return (m_CdTimerMap.ContainsKey(ticket))?m_CdTimerMap[ticket]: null;
	}
	//-----------------------------------------------------------------------------------------------------
	private DateTime TimestampToDateTime(float timestamp)
	{
		DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
		dt.AddSeconds((double)timestamp);
		return dt;
	}
	//-----------------------------------------------------------------------------------------------------
	public void GetServerTime(string ticket)
	{
		
	}
	//-----------------------------------------------------------------------------------------------------
	public void SyncServerTime(DateTime serverTime)
	{
		m_differTimeSpan = DateTime.UtcNow.Subtract(serverTime);
	}
	//-----------------------------------------------------------------------------------------------------
	public DateTime GetAdjustedClientTime()
	{
		DateTime adjustClientTime = DateTime.UtcNow.Subtract(m_differTimeSpan);
		return adjustClientTime;
	}
	//-----------------------------------------------------------------------------------------------------
	//紀錄App是否暫提過，App暫停時呼叫
	/*
	public void RecordAppPause()
	{

		for(int i = 0 ; i < m_CdTimeDataList.Count ; ++i)
		{
			if (m_CdTimeDataList[i].isStart == true)
				m_CdTimeDataList[i].m_EverSuspended = true;
		}

	}
	*/
	//-----------------------------------------------------------------------------------------------------
	//紀錄App是否暫提過，App暫停時呼叫
	/*
	public void RecordAppResume()
	{
		m_m_LastUpdateTime = DateTime.UtcNow;
	}
	*/
		
}
