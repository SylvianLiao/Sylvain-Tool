using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_ActivityPointBonus : NGUIChildGUI  
{
	public UIWidget			WidgetMyPoint			= null;	// 分數集合
	public UILabel			LabelMyPointTitle		= null;	// 分數標題
	public UILabel			LabelMyPoint			= null; // 分數
	public UILabel			LabelRuleNoteTitle		= null; // 規則標題
	public UILabel			LabelRuleNote			= null; // 規則內文
	public UIButton			ButtonClose				= null; //
	public UIGrid			GridPointBonus			= null; //
	public UIPanel			PanelPointBonus			= null; //
	public UIScrollView		ScrollViewPointBonus	= null; //
	//slot
	string slotName									= "Slot_ActivityPointBonus";
	public List<Slot_ActivityPointBonus> slotBouns	= new List<Slot_ActivityPointBonus>();

	//
	[HideInInspector]public ENUM_RANK_UI_TYPE RankType = ENUM_RANK_UI_TYPE.ActivityRank;
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel			panelGuide				= null; //教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //全螢幕按鈕
	public UISprite			spGuideShowScoreReward	= null; //導引介紹積分獎勵
	public UILabel			lbGuideShowScoreReward	= null;
	public UISprite			spGuideLeavePage		= null; //導引離開積分獎勵頁面
	public UILabel			lbGuideLeavePage		= null;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ActivityPointBonus";
	
	//-------------------------------------------------------------------------------------------------
	private UI_ActivityPointBonus() : base(GUI_SMARTOBJECT_NAME)
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
		LabelMyPointTitle.text	= GameDataDB.GetString(2826);	//我的得分：
		LabelRuleNoteTitle.text = GameDataDB.GetString(2827);	//達成獎勵說明
		CreatSlot();
	}

	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_ActivityPointBonus"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_ActivityPointBonus go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_ActivityPointBonus>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_ActivityPointBonus load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}

		Slot_ActivityPointBonus tempGo	= Instantiate(go) as Slot_ActivityPointBonus;
		tempGo.InitialSlot();

		// GuildList
		for(int i=0; i<ARPGApplication.instance.m_ActivityMgrSystem.GetPointRewardCount(); ++i) 
		{
			Slot_ActivityPointBonus newgo = Instantiate(tempGo) as Slot_ActivityPointBonus;

			newgo.transform.parent			= GridPointBonus.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//GOforLocal.localPosition;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slot{0:00}",i);
//			newgo.btnActivity.userData = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityDataByIndex(i);
			
//			newgo.InitialSlot();
			slotBouns.Add(newgo);
		}
		tempGo.gameObject.SetActive(false);
		tempGo.Destroy();
	}

	//-------------------------------------------------------------------------------------------------
	public void SetUI()
	{
		S_Activity data = ARPGApplication.instance.m_ActivityMgrSystem.GetSelectActivityData();

		S_Activity_Tmp 	activityDBF = GameDataDB.ActivityDB.GetData(data.iActivityDBID);
		if(activityDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動表錯誤 活動編號 {0}", data.iActivityDBID));
			return ;
		}
		
		S_ActivityInfo_Tmp infoDBF = GameDataDB.ActivityInfoDB.GetData(activityDBF.iGroup);
		if(infoDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動資料表錯誤 活動群組 {0}", activityDBF.iGroup));
			return ;
		}

		int rankPoint = 0;
		// 分數
		if (RankType == ENUM_RANK_UI_TYPE.ActivityRank)
		{
			S_PlayerRankData rankData = ARPGApplication.instance.m_ActivityMgrSystem.GetPlayerRankData();
			rankPoint = rankData.iPoint;
		}
		else if (RankType == ENUM_RANK_UI_TYPE.GuildBossRank)
		{
			GuildMemberData selfGuildData = ARPGApplication.instance.m_GuildSystem.GetSelfData();
			rankPoint = selfGuildData.GuildWarScore;
		}
		LabelMyPoint.text = rankPoint.ToString();
		// 規則內文
		LabelRuleNote.text = GameDataDB.GetString(infoDBF.iPointRankRewardNote);

		// Slot
		S_RankReward_Tmp reward = ARPGApplication.instance.m_ActivityMgrSystem.GetRankRewardData();
		for(int i=0; i<slotBouns.Count; ++i)
		{
			slotBouns[i].SetBonusSlot(reward.PointReward[i],rankPoint);
		}
	}
}
