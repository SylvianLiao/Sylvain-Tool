using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class MarqueeInfo
{
	public string mqContents = "";
	public ENUM_MESSAGESLOTTYPE mqType = ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_NULL;
}

public class UI_Marquee : NGUIChildGUI 
{
	public UISprite				spMarqueeBG			= null;
	public UILabel				lbContents 			= null;
	public UIButton				btnContents 		= null;
	public Transform			tRightPoint 		= null;
	public Transform			tLeftPoint  		= null;
	
	//------------------跑馬燈用變數-----------------------------------------------------------------
	[HideInInspector]
	public Queue<MarqueeInfo>	m_MqContentsQueue 	= new Queue<MarqueeInfo>();	//廣播訊息管理器
	public int					m_MqQueueMaxCount	= 50;						//廣播訊息管理器容量
	[HideInInspector]
	public bool 				m_MqIsPlaying		= false;					//由server啟動
	private bool				m_MqIsDone			= true;
	[HideInInspector]
	public MarqueeInfo			m_NowMarquee		= null;						//紀錄正在顯示的廣播訊息
	private Vector3 			m_MqPlayingPos		= Vector3.zero;				//廣播播放時的位置
	public float				m_MarqueeSpeed		= 0.0f;						//廣播速度
	public float				m_MqGapTime			= 2.0f;						//廣播間隔時間
	private CdTimer				m_MarqueeCdTimer	= null;
	private BossHighLight		m_BossHightLight	= null;
	[HideInInspector]
	public List<MarqueeInfo>	m_MqMineSlot		= new List<MarqueeInfo>();	//自己的廣播訊息

	//
	private bool				m_bBossCome			= false;

	private const string 	GUI_SMARTOBJECT_NAME = "UI_Marquee";
	//-------------------------------------------------------------------------------------------------
	private UI_Marquee() : base(GUI_SMARTOBJECT_NAME){}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitMarqueeUI();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
		ResetMarqueeUI();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	//於ARPGApplication中呼叫
	public void update()
	{
		string stateName = ARPGApplication.instance.GetCurrentGameState().name;
		//設定要顯示廣播的UI
		if (stateName != GameDefine.LOBBY_STATE 		&&
		    stateName != GameDefine.DAYACTIVE_STATE 	&&
		    stateName != GameDefine.TAPCASH_STATE 		&&
		    stateName != GameDefine.SETTING_STATE 		&&
		    stateName != GameDefine.LOGINREWARD_STATE 	&&
		    stateName != GameDefine.MAILBOX_STATE 		&&
		    //特殊條件顯示
		    stateName != GameDefine.DUNGEON_STATE)
		    //ARPGApplication.instance.GetCurrentGameState().name != GameDefine.VIPNOTE_STATE)
		{
			if (this.gameObject.activeSelf)
				Hide();
			return;
		}
		/*//於戰鬥畫面BOSS登場時不顯示廣播
		else if (stateName == GameDefine.DUNGEON_STATE)
		{
			if (m_BossHightLight == null)
			{
				if (Camera.main != null)
					m_BossHightLight = Camera.main.GetComponent<BossHighLight>();
			}
			else if (m_BossHightLight.isBossComing == true || m_BossHightLight.isBossDying == true)
			{
				if (this.gameObject.activeSelf)
					Hide();
				return;
			}
		}*/
//		//於儲值介面開啟的VIP說明不顯示廣播
//		else if (ARPGApplication.instance.GetCurrentGameState().name == GameDefine.VIPNOTE_STATE)
//		{
//			if(ARPGApplication.instance.CheckCurrentGameStates(GameDefine.DEPOSITDIAMOND_STATE) == true)
//			{
//				if (this.gameObject.activeSelf == true)
//					Hide();
//				return;
//			}
//		}

		if(!m_bBossCome)
		{
			if(m_MqIsDone && !m_MqIsPlaying && (m_MqContentsQueue.Count > 0 || m_MqMineSlot.Count > 0))
				StartMarquee(Enum_CdTimerType.Enum_CdTimerType_Marquee);
			if(m_MqIsPlaying)
			{
				MarqueePlaying();
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetbBossCome(bool bl)
	{
		m_bBossCome = bl;
	}
	//-------------------------------------------------------------------------------------------------
	public void InitMarqueeUI()
	{
		ResetMarqueePos();
		m_MqIsPlaying		= false;
		m_MqIsDone			= true;
	}
	//-------------------------------------------------------------------------------------------------
	public void ResetMarqueeUI()
	{
		ResetMarqueePos();
		m_MqIsPlaying		= false;
		m_MqIsDone			= true;
	}
	//-------------------------------------------------------------------------------------------------
	public void StartMarquee(Enum_CdTimerType cdType)
	{
		//檢查廣播管理器是否超量
		CleanMarqueeQueue();
		//關閉倒數計時
		if (m_MarqueeCdTimer != null)
		{
			m_MarqueeCdTimer.CloseCountDown();
			m_MarqueeCdTimer = null;
		}
		//讀取廣播內容
		if(m_MqMineSlot.Count != 0)
		{
			m_NowMarquee = m_MqMineSlot[0];
			m_MqMineSlot.Remove(m_MqMineSlot[0]);
		}
		else
			m_NowMarquee = m_MqContentsQueue.Dequeue();


		if (m_NowMarquee == null)
			return;
		lbContents.text = m_NowMarquee.mqContents;
		ResetMarqueePos();
		/*
		//檢查廣播內容類型
		switch(m_NowMarquee.mqType)
		{
		case ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_ANNOUNCEMENT:		//公告
			btnContents.userData = ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_ANNOUNCEMENT;
			btnContents.isEnabled = false;
			break;
		case ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_PVPRANK:				//PVP排行榜	
			btnContents.userData = ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_PVPRANK;
			btnContents.isEnabled = true;
			break;	
		case ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_ACITVITYRANK:		//活動排行榜
			btnContents.userData = ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_ACITVITYRANK;
			btnContents.isEnabled = true;
			break;
		case ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_ITEMMALLRANK:		//儲值排行榜
			btnContents.userData = ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_ITEMMALLRANK;
			btnContents.isEnabled = true;
			break;
		default:
			btnContents.userData = ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_NULL;
			btnContents.isEnabled = false;
			break;
		}*/
		//開始顯示廣播
		m_MqIsPlaying = true;
		m_MqIsDone = false;
		Show();
	}
	//-------------------------------------------------------------------------------------------------
	private void MarqueePlaying()
	{
		m_MqPlayingPos = lbContents.transform.localPosition;
		m_MqPlayingPos.x -= Time.deltaTime * m_MarqueeSpeed;
		lbContents.transform.localPosition = m_MqPlayingPos;
		if (lbContents.transform.localPosition.x +(float)lbContents.width < tLeftPoint.localPosition.x)
		{
			MarqueeFinish();
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void MarqueeFinish()
	{
		m_MqIsPlaying = false;
		//檢查是否有其他訊息要繼續廣播
		if (m_MqContentsQueue.Count > 0 || m_MqMineSlot.Count > 0)
		{
			m_MqIsDone = false;
			if (m_MarqueeCdTimer == null)
				RegManyMarquees();
		}
		else
		{
			m_MqIsDone = true;
			if (m_MarqueeCdTimer != null)
				m_MarqueeCdTimer.CloseCountDown();
			Hide();
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void ResetMarqueePos()
	{
		lbContents.transform.localPosition = tRightPoint.localPosition;
	}
	//-------------------------------------------------------------------------------------------------
	private void CleanMarqueeQueue()
	{
		int overCount = 0;
		if(m_MqContentsQueue.Count > m_MqQueueMaxCount)
			overCount = m_MqContentsQueue.Count - m_MqQueueMaxCount;
		if (overCount > 0)
		{
			for(int i=0 ; i < overCount ; ++i)
				m_MqContentsQueue.Dequeue();
			UnityDebugger.Debugger.Log("廣播管理器內容過多, 刪除"+overCount+"封訊息");
		}
	}
	//-------------------------------------------------------------------------------------------------
	//註冊倒數計時系統，用於廣播之間的間隔等待
	public void RegManyMarquees()
	{
		ARPGApplication.instance.m_CountDownTimerSystem.StartCountDown(m_MqGapTime , StartMarquee , Enum_CdTimerType.Enum_CdTimerType_Marquee);
		m_MarqueeCdTimer = ARPGApplication.instance.m_CountDownTimerSystem.GetCdTimer(Enum_CdTimerType.Enum_CdTimerType_Marquee);
		UnityDebugger.Debugger.Log("註冊計時器= 多數公告跑馬燈, 上次更新時間= 無");
	}
}
