using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_ActivityMenu : NGUIChildGUI
{
	public UIPanel			panelBase				= null; //活動主選單
	//
//	public UIPanel			panelButton 			= null; //左右切換鈕
	public UIButton			btnLeftChoose			= null; //左移
	public UIButton			btnRightChoose			= null; //右移
	//
	public UIPanel			panelActivityList		= null; //活動列表
	public UIGrid			GridActivityMenu		= null; //自動排序活動
//	public UIButton			btnActivityPrefab		= null; //活動Prefab
//	public ActivityInfo[] 	ActivityInfos			= null; //活動項目群
	public int				ActivityInfoCount		= 5;	//活動數目
//	private S_Activity[]	dbActivitys				= null; //活動資訊列
//	public Transform		GOforLocal				= null; //定位用

	//slot
	string slotName									= "Slot_ActivityMenu";
	public List<Slot_ActivityMenu> slotMenu			= new List<Slot_ActivityMenu>();
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel			panelGuide				= null; //教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //防點擊
	public UILabel			lbGuideIntroduce		= null; 
	public UISprite			spGuideEnterExpStage	= null; //導引進入經驗關卡頁面
	public UILabel			lbGuideEnterStageRight	= null;
	public UILabel			lbGuideEnterStageLeft	= null;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_ActivityMenu";

	//-------------------------------------------------------------------------------------------------
	private UI_ActivityMenu() : base(GUI_SMARTOBJECT_NAME)
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
		CreatSlot();
	}

	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_ActivityMenu"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_ActivityMenu go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_ActivityMenu>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_ActivityMenu load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}

		ActivityMgrSystem actSystem = ARPGApplication.instance.m_ActivityMgrSystem;

		// GuildList
		for(int i=0; i<actSystem.GetActivityTotalCount(); ++i) 
		{
			Slot_ActivityMenu newgo	= Instantiate(go) as Slot_ActivityMenu;
				
			newgo.transform.parent			= GridActivityMenu.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//GOforLocal.localPosition;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slot{0:00}",i);
//			newgo.btnActivity.userData = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityDataByIndex(i);
			newgo.btnActivity.userData = i;

			newgo.InitialSlot();
			newgo.gameObject.SetActive(false);
			slotMenu.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot()
	{
		//避免活動數量變更產生錯誤
		if(ARPGApplication.instance.m_ActivityMgrSystem.GetActivityTotalCount() != slotMenu.Count)
		{
			slotMenu.Clear();
			CreatSlot();
		}

		S_Activity data;
		EMUM_ACTIVITY_TYPE type;
		for(int i=0; i<ARPGApplication.instance.m_ActivityMgrSystem.GetActivityTotalCount(); ++i)
		{
//			slotMenu[i].SetSlot(ARPGApplication.instance.m_ActivityMgrSystem.GetActivityDataByIndex(i).iActivityDBID);
			data = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityDataByIndex(i);
			type = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityType(data.iActivityInfoDBID);

			//巔峰競技場不在這顯示
			if(type != EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PeakPVP && type != EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_GuildWar)
			{
				slotMenu[i].SetSlot(data);
			}
			else
			{
				slotMenu[i].gameObject.SetActive(false);
			}

		}
		GridActivityMenu.Reposition();
	}

	//-------------------------------------------------------------------------------------------------
}