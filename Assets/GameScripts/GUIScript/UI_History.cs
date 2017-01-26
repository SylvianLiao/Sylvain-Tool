using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum ENUM_UI_History_Type
{
	PeakArena	=0,
	Guild,

	Max,
}

public class UI_History : NGUIChildGUI 
{
	public UILabel		LabelTitle				= null;
	public UIButton 	ButtonClose				= null;
	public UISprite		SpriteMsgTitleBG		= null;
	public UILabel		LabelTime				= null;
	public UILabel		LabelEven				= null;

	public UIGrid		GridMsgList				= null;
	public UILabel		LabelNote				= null;
	public UIButton 	ButtonRefresh			= null;
	public UILabel		LabelRefresh			= null;

	public List<Slot_History>		slotList	= new List<Slot_History>();
	public string					slotName 	= "Slot_History";

	public int 			refreshLogCD 			= 5; //秒
	DateTime	openTime;
	TimeSpan	ts;

	ENUM_UI_History_Type m_type = ENUM_UI_History_Type.PeakArena;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_History";
	
	//-------------------------------------------------------------------------------------------------
	private UI_History() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		switch(m_type)
		{
		case ENUM_UI_History_Type.PeakArena:
			LabelTitle.text 		= GameDataDB.GetString(5313);	//巔峰歷程
			LabelNote.text 			= GameDataDB.GetString(5317);	//※歷程只保留最新的20筆，置頂為最新事件
			break;
		case ENUM_UI_History_Type.Guild:
			LabelTitle.text 		= GameDataDB.GetString(8101);	//公會動態
			LabelNote.text 			= GameDataDB.GetString(5317);	//※歷程只保留最新的20筆，置頂為最新事件

			ButtonRefresh.gameObject.SetActive(false);
			break;
		}

		LabelTime.text 			= GameDataDB.GetString(5314);	//日期
		LabelEven.text 			= GameDataDB.GetString(5315);	//事件
		LabelRefresh.text 		= GameDataDB.GetString(5316);	//刷新
			
		CreateSlot();
	}

	//-------------------------------------------------------------------------------------------------
	void CreateSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_History";
		}
		
		Slot_History go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_History>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_History load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		for(int i=0; i<10; ++i)
		{
			Slot_History newgo= Instantiate(go) as Slot_History;
			
			newgo.transform.parent			= GridMsgList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("Slot_History{0:00}",i);
			newgo.InitialSlot();
			
			slotList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		openTime = DateTime.UtcNow;
//		SetUILabel();
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot()
	{
		int logCount;
		List<S_HistoryLog> logDataList;
		ulong serial;		

		switch(m_type)
		{
		case ENUM_UI_History_Type.PeakArena:
			logCount	= ARPGApplication.instance.m_ActivityMgrSystem.GetPeakPVPLogCount();
			logDataList	= ARPGApplication.instance.m_ActivityMgrSystem.GetPeakPVPLogDataList();
			serial		= ARPGApplication.instance.m_ActivityMgrSystem.GetLastLogSerial();

			//存最後一筆LOG序號
			ARPGApplication.instance.m_ActivityMgrSystem.SetLastLogSerial();
			break;
		case ENUM_UI_History_Type.Guild:
			logCount 	= ARPGApplication.instance.m_GuildSystem.GetGuildLogCount();
			logDataList = ARPGApplication.instance.m_GuildSystem.GetGuildLogData();
			serial		= ARPGApplication.instance.m_GuildSystem.GetLastLogSerial();

			//存最後一筆LOG序號
			ARPGApplication.instance.m_GuildSystem.SetLastLogSerial();
			break;
		default:
			logCount 	= 0;
			logDataList = null;
			serial		= 0;

			break;
		}

		for(int i=0; i<slotList.Count; ++i)
		{
			if(i < logCount)
			{
				slotList[i].SetSlot(logDataList[i], serial);
			}
			else
			{
				slotList[i].InitialSlot();
			}
		}
	}

	//-------------------------------------------------------------------------------------------------
	public bool CheckRefreshLogCD()
	{
		ts = DateTime.UtcNow - openTime;
		if(ts.TotalSeconds > refreshLogCD)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void UpdateRefreshLogCD()
	{
		openTime = DateTime.UtcNow;
	}

	//-------------------------------------------------------------------------------------------------
//	public void SetUILabel()
//	{
//		LabelTitle.text 		= GameDataDB.GetString(5313);	//巔峰歷程
//		LabelTime.text 			= GameDataDB.GetString(5314);	//日期
//		LabelEven.text 			= GameDataDB.GetString(5315);	//事件
//		LabelRefresh.text 		= GameDataDB.GetString(5316);	//刷新
//		LabelNote.text 			= GameDataDB.GetString(5317);	//※歷程只保留最新的20筆，置頂為最新事件
//	}

	//-------------------------------------------------------------------------------------------------
	public void SetUIType(ENUM_UI_History_Type type)
	{
		m_type = type;
	}

}
