using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum ENUM_RANK_UI_TYPE
{
	ActivityRank	= 0,
	GuildBossRank,
}

public class UI_ActivityRank : NGUIChildGUI 
{
	//上面
	public UILabel			LabelRankTitle		= null;		//排名標題
	public UIButton			ButtonRule			= null;		//規則按鈕
	public UILabel			LabelRule			= null;		//規則按鈕文字
	public UIButton			ButtonClose			= null;		//關閉按鈕


	//左邊Rank
	public UILabel			LabelRankNote		= null;		//排名注意事項	(暫無)
	public UIGrid			GridRank			= null;		//排名Grid
	public UILabel			LabelRankPage		= null;		//頁				(暫無)
	public UIButton			ButtonLeftArrow		= null;		//左箭頭			(暫無)
	public UIButton			ButtonRightArrow	= null;		//右箭頭			(暫無)
	public UIScrollView		ScrollViewRank		= null;
	public UIWrapContentEX	WrapRank			= null;		//排名Wrap

	//右邊Reward
	public UILabel			LabelRewardTitle	= null;		//獎勵標題
	public UIGrid			GridReward			= null;		//獎勵Grid
	public UIPanel			PanelRankReward		= null;
	public UIScrollView		ScrollViewRankReward = null;

	//slot
	public string	slotNameRank 		= "Slot_ActivityRank_Rank";
	public string	slotNameReward 		= "Slot_ActivityRank_Reward";
	public string	slotGuildBossRank 	= "Slot_GuildBossRank_Rank";
	const int rankSlotCount = 10;	//ActivityRankState也有一個

	public List<Slot_ActivityRank_Rank> 	slotRankList	= new List<Slot_ActivityRank_Rank>();
	public List<Slot_GuildBossRank_Rank> 	slotGuildBossRankList	= new List<Slot_GuildBossRank_Rank>();
	public List<Slot_ActivityRank_Reward>  	slotRewardList	= new List<Slot_ActivityRank_Reward>();
	//
	[HideInInspector]public ENUM_RANK_UI_TYPE RankType = ENUM_RANK_UI_TYPE.ActivityRank;
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel			panelGuide				= null; //教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //全螢幕按鈕
	public UISprite			spGuideShowRankReward	= null; //導引介紹排行榜獎勵
	public UILabel			lbGuideShowRankReward	= null;
	public UISprite			spGuideLeavePage		= null; //導引離開排行榜獎勵頁面
	public UILabel			lbGuideLeavePage		= null;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ActivityRank";
	
	//-------------------------------------------------------------------------------------------------
	private UI_ActivityRank() : base(GUI_SMARTOBJECT_NAME)
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
//		CreateRankSlot();
		if (RankType == ENUM_RANK_UI_TYPE.ActivityRank)
			CreateRankSlot2();
		else
			CreateGuildBossRankSlot();

		CreateRewardSlot();
		WrapRank.enabled = true;
		WrapRank.SortAlphabetically();
		SetUILab();
	}
	//-------------------------------------------------------------------------------------------------
	void CreateRankSlot()
	{
		if(slotNameRank == "")
		{
			slotNameRank = "Slot_ActivityRank_Rank"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_ActivityRank_Rank go = ResourceManager.Instance.GetGUI(slotNameRank).GetComponent<Slot_ActivityRank_Rank>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("CreateRankSlot load prefeb error,path:{0}", "GUI/"+slotNameRank) );
			return;
		}

		// 排行格
//		for(int i=0; i<ARPGApplication.instance.m_ActivityMgrSystem.GetRankDataList().Count; ++i)
		for(int i=0; i<rankSlotCount; ++i)
		{
			Slot_ActivityRank_Rank newgo= Instantiate(go) as Slot_ActivityRank_Rank;
			
			newgo.transform.parent			= GridRank.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
			
			newgo.name = string.Format("slotRank{0:00}",i);
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(true);
			slotRankList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	void CreateRankSlot2()
	{
		if(slotNameRank == "")
		{
			slotNameRank = "Slot_ActivityRank_Rank"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_ActivityRank_Rank go = ResourceManager.Instance.GetGUI(slotNameRank).GetComponent<Slot_ActivityRank_Rank>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("CreateRankSlot load prefeb error,path:{0}", "GUI/"+slotNameRank) );
			return;
		}
		
		// 排行格
		for(int i=0; i<rankSlotCount; ++i)
		{
			Slot_ActivityRank_Rank newgo= Instantiate(go) as Slot_ActivityRank_Rank;
			
			newgo.transform.parent			= WrapRank.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= new Vector3(0, -i, 0);//Vector3.zero;//itemSlotLocal[i].localPosition;
			
			newgo.name = string.Format("slotRank{0:00}",i);
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(true);
			slotRankList.Add(newgo);
		}

//		WrapRank.SortAlphabetically();
	}
	//-------------------------------------------------------------------------------------------------
	void CreateGuildBossRankSlot()
	{
		if(slotGuildBossRank == "")
		{
			slotGuildBossRank = "Slot_GuildBossRank_Rank"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_GuildBossRank_Rank go = ResourceManager.Instance.GetGUI(slotGuildBossRank).GetComponent<Slot_GuildBossRank_Rank>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("CreateGuildBossRankSlot load prefeb error,path:{0}", "GUI/"+slotGuildBossRank) );
			return;
		}
		
		// 排行格
		for(int i=0; i<rankSlotCount; ++i)
		{
			Slot_GuildBossRank_Rank newgo= Instantiate(go) as Slot_GuildBossRank_Rank;
			
			newgo.transform.parent			= WrapRank.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= new Vector3(0, -i, 0);//Vector3.zero;//itemSlotLocal[i].localPosition;
			
			newgo.name = string.Format("slotRank{0:00}",i);
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(true);
			slotGuildBossRankList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	void CreateRewardSlot()
	{
//		slotNameReward 		= "Slot_ActivityRank_Reward";
		if(slotNameReward == "")
		{
			slotNameReward = "Slot_ActivityRank_Reward"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_ActivityRank_Reward go = ResourceManager.Instance.GetGUI(slotNameReward).GetComponent<Slot_ActivityRank_Reward>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("CreateRankSlot load prefeb error,path:{0}", "GUI/"+slotNameReward) );
			return;
		}

		//取資料
//		ActivityDetailState state = ARPGApplication.instance.GetGameStateByName(GameDefine.ACTIVITYDETAIL_STATE) as ActivityDetailState;
//		S_Activity data = state.GetActivityData();
		S_Activity data = ARPGApplication.instance.m_ActivityMgrSystem.GetSelectActivityData();

		if(data ==null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動資料錯誤 "));
			return ;
		}
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

		S_RankReward_Tmp rewardDBF = GameDataDB.RankRewardDB.GetData(infoDBF.RankReward);
		if(rewardDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動獎勵表錯誤 獎勵編號 {0}", infoDBF.RankReward));
			return ;
		}

		// 排行榜格子
		for(int i=0; i<rewardDBF.RankReward.Length; ++i)
		{
			Slot_ActivityRank_Reward newgo= Instantiate(go) as Slot_ActivityRank_Reward;
			
			newgo.transform.parent			= GridReward.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
			
			newgo.name = string.Format("slotReward{0:00}",i);
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(true);
			slotRewardList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetPageLabel(int page)
	{
		int bottom 	= (int)GameDefine.ACTIVITY_RANK_PAGE * (page-1) + 1;
		int top 	= (int)GameDefine.ACTIVITY_RANK_PAGE * page;

		LabelRankPage.text = string.Format("{0}-{1}", bottom, top);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetRankReward()
	{
		S_RankReward_Tmp data = ARPGApplication.instance.m_ActivityMgrSystem.GetRankRewardData();

		for(int i=0; i<slotRewardList.Count; ++i)
		{
			if(data == null || data.RankReward.Length <= i || data.RankReward[i].iPointRankFrom == -1)
			{
				slotRewardList[i].gameObject.SetActive(false);
			}
			else
			{
				slotRewardList[i].SetSlot(data.RankReward[i],RankType == ENUM_RANK_UI_TYPE.GuildBossRank);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetUILab()
	{
		if (RankType == ENUM_RANK_UI_TYPE.ActivityRank)
			LabelRankTitle.text   = GameDataDB.GetString(2817);	//排行榜
		else if (RankType == ENUM_RANK_UI_TYPE.GuildBossRank)
			LabelRankTitle.text   = GameDataDB.GetString(8628);	//公會排行榜
		LabelRule.text 		  = GameDataDB.GetString(2835); //規則
		LabelRewardTitle.text = GameDataDB.GetString(2836); //排名獎勵
	}
}
