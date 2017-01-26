using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_ActivityDetail : NGUIChildGUI
{
	//活動挑戰型態
//	public UIPanel			panelChallegeType		= null; 			
	//左邊
	[Header("LeftSide")]
	public UITexture		texActivityPic			= null;				// 活動大圖
	public UISprite			SpriteMyRankBG			= null;				// 我的排名底圖
	public UILabel			LabelMyRanktitle		= null;				// 我的排名 標題
	public UILabel			LabelMyRankNum			= null;				// 我的排名 名次
	public Transform		TSRankInfo				= null;				// 我的積分底
	public UILabel			LabelRankInfoMy			= null;				// 我的積分
	public UILabel			LabelRankInfoOther		= null;				// 前一名差距

	//右邊
	[Header("RightSide")]
	public UISprite			SpriteActivityInfoBG	= null;				// 右邊底圖
	public UITable			TabelStageList			= null;				// 右邊ScrollView清單
	public UILabel			LabelActivityName		= null;				// 關卡名稱
	public UIButton			ButtonReach				= null;				// 達成獎勵
	public UILabel			LabelReach				= null;				//
	public UIButton			ButtonRank				= null;				// 排行榜
	public UILabel			LabelRank				= null;				//
	public UIButton			ButtonClose				= null;				// 關閉按鈕
	public UILabel			LabelActivityPeriodTitle= null;				// 活動時間 標題
	public UILabel			LabelActivityPeriod		= null;				// 活動時間
	public UILabel			LabelActivityInfoTitle	= null;				// 活動說明 標題
	public UILabel			LabelActivityInfo		= null;				// 活動說明

	public UIButton			ButtonStore				= null;				// PVP商店
	public UILabel			LabelStore				= null;				// PVP商店按鈕名稱
	//slot
	public Transform		StageList				= null;				// 關卡列表
	public UISprite			SpriteStageListBG		= null;
	public UIGrid			GridStageList			= null;				// 關卡列表

	//底部
	[Header("Bottom")]
	public UIButton			ButtonChangeOpponent	= null;				// 換對手按鈕
	public UILabel 			LabelChangeOpponent		= null;				// 換對手LABEL
	public UILabel			LabelBattleLimit		= null;				// 今日可挑戰次數
	public UILabel			lbResetChallenge		= null;				// 挑戰次數重置時間
	public UIButton			ButtonRetroactive		= null;				// 補登
	public UILabel			LabelButtonRetroactive	= null;				// 補登按鈕Label
	public UILabel			LabelRetroactive		= null;				// 補登可用次數
	public UILabel			LabelRetroactiveText	= null;				// 補登可用次數
	public UIProgressBar	RetroactiveProBar		= null;				// 補登倒數條
	//-------------------------------------------------------------------------------------------------
	private ENUM_APType 			m_CountDownType  		= ENUM_APType.ENUM_APType_Null;
	public CdTimer					m_ActiveCdTimer			= null;			//倒數時間
	//-------------------------------------------------------------------------------------------------
	//SLOT
//	[HideInInspector]
	public string						slotName 		= "Slot_ActivityDetail_Stage";
	public string						slotNamePVP		= "Slot_ValuePVP_Opponent";
	[HideInInspector]
//	public  <Slot_ActivityDetail_Stage	aLimitTimeType	= null;

	public List<Slot_ActivityDetail_Stage> slotStage 	= new List<Slot_ActivityDetail_Stage>();
	public List<Slot_ValuePVP_Opponent>    slotPVP	 	= new List<Slot_ValuePVP_Opponent>();

	//
	List<int> tempDIDList	= new List<int>();

	//-------------------------------------------------------------------------------------------------
	[Header("RetPanel")]
	public UIPanel 			PanelRetroactiveCheck	= null;			//購買補登界面
	public UIButton			ButtonRetrOnce			= null;			//購買一次按鈕
	public UIButton			ButtonRetrFull			= null;			//買到滿按鈕
	public UIButton			ButtonExit				= null;			//離開按鈕
	public UILabel			LabelRetOnce			= null;			//買一次按鈕字串
	public UILabel			LabelRetFull			= null;			//買到滿按鈕字串
	public UILabel			LabelRetText			= null;			//買補登說明字串
	[HideInInspector]
	public ENUM_APType		m_emAPType				= ENUM_APType.ENUM_APType_Null;	//開啟的活動
	private int				m_iBuyOnceCost 			= 0;
	private int				m_iBuyFullCost 			= 0;
	public int				BuyOnceCost				{get{return m_iBuyOnceCost;}}
	public int				BuyFullCost				{get{return m_iBuyFullCost;}}

	//-------------------------------------新手教學用-------------------------------------
	[Header("Guide")]
	public UIPanel			panelGuide				= null; //教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //全螢幕按鈕
	public UISprite			spGuideShowStageReward	= null; //導引介紹關卡獎勵
	public UILabel			lbGuideShowStageReward	= null;
	public UISprite			spGuideShowRank			= null; //導引介紹積分與排行
	public UILabel			lbGuideShowRank			= null;
	public UISprite			spGuideScoreReward		= null; //導引點擊達成獎勵
	public UILabel			lbGuideScoreReward	 	= null;
	public UISprite			spGuideRankReward	 	= null; //導引點擊排行榜
	public UILabel			lbGuideRankReward	 	= null;
	public UISprite			spGuideShowEnemy	 	= null; //導引介紹競技場對手
	public UILabel			lbGuideShowEnemy	 	= null;
	public UILabel			lbGuideFinish	 		= null;	//導引教學結束
	//-------------------------------------------------------------------------------------------------

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_ActivityDetail";
	//-------------------------------------------------------------------------------------------------
	private UI_ActivityDetail() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		CreateSlot();
	}

	//-------------------------------------------------------------------------------------------------
	public void Update()
	{
		if (m_ActiveCdTimer != null && m_ActiveCdTimer.isStart )
		{
			lbResetChallenge.text = m_ActiveCdTimer.ShowTime();
			RetroactiveProBar.value = 1 - (m_ActiveCdTimer.countDownTime / m_ActiveCdTimer.countDownLimit);
		}
	}
	//-------------------------------------------------------------------------------------------------
	void CreateSlot()
	{
		//取資料
		S_ActivityInfo_Tmp infoDBF = ARPGApplication.instance.m_ActivityMgrSystem.GetSelectActivityInfoDBF();
		if(infoDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動資料表錯誤 "));
			return ;
		}

		if(	  infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_Normal 
		   || infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_Rank
		   || infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_WeekCycle
		   || infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_Guide)
		{
			if(slotName == "")
			{
				slotName = "Slot_ActivityDetail_Stage";
			}
			
			Slot_ActivityDetail_Stage go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_ActivityDetail_Stage>();
			if(go == null)
			{
				UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+slotName) );
				return;
			}

			List<int> dIDList = ARPGApplication.instance.m_ActivityMgrSystem.GetDungeonList();
			for(int i=0; i<dIDList.Count; ++i)
			{
				Slot_ActivityDetail_Stage newgo= Instantiate(go) as Slot_ActivityDetail_Stage;
				
				newgo.transform.parent			= GridStageList.transform;
				newgo.transform.localScale		= Vector3.one;
				newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
				newgo.transform.localPosition	= Vector3.zero;
				newgo.gameObject.SetActive(true);
				
				newgo.name = string.Format("slotActivity{0:00}",i);
				newgo.ButtonBattle.userData = dIDList[i];
				newgo.InitialSlot();
				
				slotStage.Add(newgo);
			}
		}
		else if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PVP || 
		        infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_3VS3)
		{
			if(slotNamePVP == "")
			{
				slotNamePVP = "Slot_ValuePVP_Opponent";
			}
			
			Slot_ValuePVP_Opponent go = ResourceManager.Instance.GetGUI(slotNamePVP).GetComponent<Slot_ValuePVP_Opponent>();
			if(go == null)
			{
				UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+slotName) );
				return;
			}

			for(int i=0; i<(int)EMUM_DATAPVP_COMPETITOR_LEVEL.EMUM_COMPETITOR_LEVEL_MAX; ++i)
			{
				//
				Slot_ValuePVP_Opponent newgo= Instantiate(go) as Slot_ValuePVP_Opponent;
				
				newgo.transform.parent			= GridStageList.transform;
				newgo.transform.localScale		= Vector3.one;
				newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
				newgo.transform.localPosition	= Vector3.zero;
				newgo.gameObject.SetActive(true);
				
				newgo.name = string.Format("slotActivity{0:00}",i);
				newgo.ButtonBattle.userData = i;
				newgo.InitialSlot(infoDBF.emActivityType);
				
				slotPVP.Add(newgo);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetUI(int activityID)
	{		
		S_Activity_Tmp 	activityDBF = GameDataDB.ActivityDB.GetData(activityID);
		if(activityDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動表錯誤 活動編號 {0}", activityID));
			return ;
		}
		S_ActivityInfo_Tmp infoDBF = GameDataDB.ActivityInfoDB.GetData(activityDBF.iGroup);
		if(infoDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動資料表錯誤 活動群組 {0}", activityDBF.iGroup));
			return ;
		}

		if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PVP || 
		   infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_3VS3)
		{
			if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDataPVPOpponentCount(infoDBF.emActivityType) <=0)
			{
				JsonSlot_Dungeon.Send_CtoM_GetActivityRank(activityID);
				return ;
			}
		}

		//設定開啟的活動
		m_emAPType = (ENUM_APType)infoDBF.iVariablePos;
		//UI
		// 活動大圖
		Utility.ChangeTexture(texActivityPic, infoDBF.iBackGroundType);
				
		// 關卡名稱
		LabelActivityName.text = GameDataDB.GetString(infoDBF.CName);	
		// 達成獎勵
		LabelReach.text	= GameDataDB.GetString(2816);	//達成獎勵
		// 排行榜			
		LabelRank.text = GameDataDB.GetString(2817);	//排行榜
		// PVP商店
		LabelStore.text = GameDataDB.GetString(2833);	//水晶商店	
	
		//活動時間
		LabelActivityPeriodTitle.text = GameDataDB.GetString(2839);	//活動時間
		//活動說明
		LabelActivityInfoTitle.text = GameDataDB.GetString(2840);	//活動說明

		if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_Normal 
		   ||infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_WeekCycle
		   || infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_Guide)
		{
			ButtonReach.gameObject.SetActive(false);
			ButtonRank.gameObject.SetActive(false);
			SpriteMyRankBG.gameObject.SetActive(false);
			TSRankInfo.gameObject.SetActive(false);
		}
		else
		{
			ButtonReach.gameObject.SetActive(true);
			ButtonRank.gameObject.SetActive(true);
			SpriteMyRankBG.gameObject.SetActive(true);
			TSRankInfo.gameObject.SetActive(true);
			//設定玩家資料
			S_PlayerRankData data = ARPGApplication.instance.m_ActivityMgrSystem.GetPlayerRankData();
			SetUIRank(data);
		}

		// 活動時間
		LabelActivityPeriod.text = GameDataDB.GetString(activityDBF.ActivityPeriod);				
		// 活動說明
		LabelActivityInfo.text = GameDataDB.GetString(infoDBF.ActivityNote);

		//PVP商店按鈕只有PVP活動開啟
		ButtonStore.gameObject.SetActive(false);
		//刷新按鈕只有PVP活動開啟
		ButtonChangeOpponent.gameObject.SetActive(false);
		//補登按鈕有功能才開放
		ButtonRetroactive.gameObject.SetActive(false);
		LabelButtonRetroactive.text = GameDataDB.GetString(576);	//補登

		LabelRetroactiveText.text = GameDataDB.GetString(547);//可補登次數：
		//購買補登介面設定
		PanelRetroactiveCheck.gameObject.SetActive(false);
		LabelRetOnce.text = "";	
		LabelRetFull.text = "";	
		LabelRetText.text = string.Format(GameDataDB.GetString(577),0);	//請選擇補登次數

		// 除值活動(不顯示關卡對手列表..等)		
		if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_Store)
		{
			StageList.gameObject.SetActive(false);
		}
		// PVP活動
		else if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PVP ||
		        infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_3VS3)
		{
			// 刷新
			LabelChangeOpponent.text = string.Format("{0}({1})",GameDataDB.GetString(1567),
			                                         ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetCanUseDataPVPResetCount(infoDBF.emActivityType));	//刷新
			ButtonStore.gameObject.SetActive(true);
			ButtonChangeOpponent.gameObject.SetActive(true);
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetCanUseDataPVPResetCount(infoDBF.emActivityType) <=0)
			{
				ButtonChangeOpponent.isEnabled = false;
			}
			else
			{
				ButtonChangeOpponent.isEnabled = true;
			}

			for(int i=0; i< ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDataPVPOpponentCount(infoDBF.emActivityType); ++i)
			{
				slotPVP[i].SetSlotValue(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDataPVPOpponent(infoDBF.emActivityType,
				                                                                                                  (EMUM_DATAPVP_COMPETITOR_LEVEL)i));
			}
		}
		else
		{
			tempDIDList.Clear();
			tempDIDList = ARPGApplication.instance.m_ActivityMgrSystem.GetDungeonListByGroupID(infoDBF.Dungeon_Group);
			if(tempDIDList == null || tempDIDList.Count == 0)
			{
				UnityDebugger.Debugger.LogError(string.Format("讀取活動副本編號錯誤 活動群組 {0}", activityDBF.iGroup));
				return ;
			}

			StageList.gameObject.SetActive(true);
			//循環活動
			if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_WeekCycle)
			{
				int weekNum = ARPGApplication.instance.m_RoleSystem.GetWeekCycleNum();
				for(int i=0; i<slotStage.Count; ++i)
				{
					if(i < weekNum+1)
					{
						slotStage[i].SetSlot(tempDIDList[i]);
					}
					else
					{
						slotStage[i].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				//SLOT
				for(int i=0; i<slotStage.Count; ++i)
				{
					slotStage[i].SetSlot(tempDIDList[i]);
				}
			}
		}
		
		// 今日可挑戰次數 及 挑戰次數回復時間
		S_TimeRec_Tmp timeTmp = GameDataDB.TimeRecDB.GetData(infoDBF.iVariablePos);
		if (timeTmp == null)
		{
			LabelBattleLimit.text = "";
			LabelBattleLimit.gameObject.SetActive(false);
			lbResetChallenge.gameObject.SetActive(false);

			LabelRetroactive.text = "";
			LabelRetroactive.gameObject.SetActive(false);
		}
		else
		{
			if(timeTmp.iLimitValue > 0)
			{
				LabelBattleLimit.text = string.Format("{2}{0}/{1}", 
				                                      ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue(infoDBF.iVariablePos), 
				                                      timeTmp.iLimitValue,
				                                      GameDataDB.GetString(2818));	//今日可挑戰次數：
				LabelBattleLimit.gameObject.SetActive(true);

				//是否有補登
				bool bUseRet = false;
				if(timeTmp.iRetroactiveTimes > 0)
				{
					LabelRetroactive.text = string.Format("{0}/{1}",ARPGApplication.instance.m_ActivityMgrSystem.GetAPRetTimes(infoDBF.iVariablePos),timeTmp.iRetroactiveTimes); 
					LabelRetroactive.gameObject.SetActive(true);
					//設定補登按鈕狀態
					ButtonRetroactive.gameObject.SetActive(true);
					ButtonRetroactive.isEnabled = (ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue(infoDBF.iVariablePos) < timeTmp.iLimitValue);

					bUseRet = ARPGApplication.instance.m_ActivityMgrSystem.GetAPRetTimes(infoDBF.iVariablePos) < timeTmp.iRetroactiveTimes
						|| ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue(infoDBF.iVariablePos) < timeTmp.iLimitValue;
				}
				else
				{
					LabelRetroactive.gameObject.SetActive(false);
					ButtonRetroactive.gameObject.SetActive(false);

					bUseRet = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue(infoDBF.iVariablePos) < timeTmp.iLimitValue;
				}

				//挑戰次數重置時間
				if (bUseRet)
				{
					CdTimer timer = ARPGApplication.instance.m_CountDownTimerSystem.GetCdTimer((Enum_CdTimerType)ARPGApplication.instance.m_ActivityMgrSystem.GetActivityAPType(activityDBF.iGroup));
					
					if(timer.isStart == true)
					{
						lbResetChallenge.gameObject.SetActive(true);
						RetroactiveProBar.gameObject.SetActive(true);
					}
					else
					{
						lbResetChallenge.gameObject.SetActive(true);
						lbResetChallenge.text = GameDataDB.GetString(2841);	//[00FF00]每日4點重置次數[-]
						RetroactiveProBar.gameObject.SetActive(false);
					}
				}
				else
				{
					lbResetChallenge.gameObject.SetActive(false);
				}

			}
			else
			{
				LabelBattleLimit.text = "";
				LabelBattleLimit.gameObject.SetActive(false);

				LabelRetroactive.text = "";
				LabelRetroactive.gameObject.SetActive(false);
			}		
		}

		//調整版型
		//ScrollViewStage
		switch(infoDBF.emActivityUIType)
		{
		case EMUM_ACTIVITY_UI_TYPE.EMUM_ACTIVITY_UI_TYPE_Normal:
			break;
		case EMUM_ACTIVITY_UI_TYPE.EMUM_ACTIVITY_UI_TYPE_High:
			SpriteStageListBG.height = 416;
			break;
		}
	}

	//-------------------------------------------------------------------------------------------------
	//設定玩家資料
	public void SetUIRank(S_PlayerRankData data)
	{
		// 我的排名 名次
		LabelMyRanktitle.text = GameDataDB.GetString(2838);;	//我的排名
		if(data.rank == 0)
		{
			LabelMyRankNum.text	= GameDataDB.GetString(2819);	//榜外
		}
		else
		{
			LabelMyRankNum.text	= string.Format("{0}", data.rank);
		}

		// 我的積分
		LabelRankInfoMy.text = string.Format("{1} {0}", data.iPoint, GameDataDB.GetString(2820));	//我的積分

		int point = data.PrePlayerPoint -data.iPoint;
		// 前一名差距
		if(point <= 0)
		{
			point = 0;
		}
		LabelRankInfoOther.text	= string.Format("{0} {1}{2}", 
		                                        GameDataDB.GetString(2821), 
		                                        point, GameDataDB.GetString(2822)); //與前一名差距 分 
	}

	//-------------------------------------------------------------------------------------------------
/*	private void RegActiveReset(int timeRecID)
	{
		if (timeRecID < 0 || timeRecID > (int)ENUM_APType.ENUM_APType_Max)
			return;

		m_CountDownType = (ENUM_APType)timeRecID;
	
		//找出倒數時間上限
		S_TimeRec_Tmp apTimeTmp = GameDataDB.TimeRecDB.GetData(timeRecID);
		if(apTimeTmp == null)
		{
			return;
		}
		//檢查挑戰次數上次更新時間是否有client自己更新過的紀錄
		DateTime apUpdateTime = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetClientUpdateTime(m_CountDownType);
		//若無則找出Server給予的上次更新時間
		if (apUpdateTime <= DateTime.MinValue)
			apUpdateTime = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetUpdateTime(m_CountDownType);
		//註冊倒數計時器
		ARPGApplication.instance.m_CountDownTimerSystem.StartCountDown(apUpdateTime , apTimeTmp.iTime , ActiveChallengeRecover , (Enum_CdTimerType)timeRecID);
		m_ActiveCdTimer = ARPGApplication.instance.m_CountDownTimerSystem.GetCdTimer((Enum_CdTimerType)timeRecID);
		UnityDebugger.Debugger.Log("註冊計時器= "+m_CountDownType+", 上次更新時間= "+apUpdateTime);
	}
	
	//-------------------------------------------------------------------------------------------------
	private void ActiveChallengeRecover()
	{
		if(ARPGApplication.instance.GetCurrentGameState().name == GameDefine.ACTIVITYDETAIL_STATE)
		{
			if ((int)m_CountDownType < 0)
				return;
			S_TimeRec_Tmp apTimeTmp = GameDataDB.TimeRecDB.GetData((int)m_CountDownType);
			if(apTimeTmp == null)
				return;
			
			ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.AddAP(m_CountDownType , 1);
			ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SetClientUpdateTime(m_CountDownType, DateTime.UtcNow);
			LabelBattleLimit.text = string.Format("{2}{0}/{1}", 
			                                      ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue((int)m_CountDownType),
			                                      apTimeTmp.iLimitValue,
			                                      GameDataDB.GetString(2818));	//今日可挑戰次數：
		}
		else if(ARPGApplication.instance.GetCurrentGameState().name == GameDefine.LOBBY_STATE)
		{
//			....
		}

	}
*/
	//-------------------------------------------------------------------------------------------------
	public void SetUIActivityValue(ENUM_APType type, int value)
	{
		LabelBattleLimit.text = string.Format("{2}{0}/{1}", 
		                                      ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue((int)type),
		                                      value,
		                                      GameDataDB.GetString(2818));	//今日可挑戰次數：
	}
	//-------------------------------------------------------------------------------------------------
	public void SetUIRetroactiveValue(ENUM_APType type, int value)
	{
		LabelRetroactive.text = string.Format("{0}/{1}"
		                                      ,ARPGApplication.instance.m_ActivityMgrSystem.GetAPRetTimes((int)type)
		                                      ,value); 
	}
	//-------------------------------------------------------------------------------------------------
	public void ClearResetChallengeLabel()
	{
		lbResetChallenge.text = "";
		lbResetChallenge.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetRetBuyBottoms()
	{
		int iNowAP = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue((int)m_emAPType);
		int iNowRet = ARPGApplication.instance.m_ActivityMgrSystem.GetAPRetTimes((int)m_emAPType);
		//取表
		S_ShopPrize_Tmp shopTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_BUY_AP_RETROACTIVE);
		if(shopTmp == null)
		{
			UnityDebugger.Debugger.LogError("讀取價格資料表錯誤");
			return;
		}

		S_TimeRec_Tmp timeTmp = GameDataDB.TimeRecDB.GetData((int)m_emAPType);
		if(timeTmp == null)
		{
			UnityDebugger.Debugger.LogError("讀取時間資料表錯誤");
			return;
		}

		int iValue = timeTmp.iLimitValue - iNowAP; //差多少滿AP	
		int iBuyCount = 1;
		int iCost = 0;

		S_APData sApData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.sAPData[(int)m_emAPType];

		//設定說明文
		SetRetPanelText(sApData.iBuyRetCount);

		//先設定買一次
		if(sApData.iBuyRetCount > shopTmp.iShopPrizeOfLastTime)
			iCost = shopTmp.GetPrize(shopTmp.iShopPrizeOfLastTime);
		else
			iCost = shopTmp.GetPrize(sApData.iBuyRetCount);

		m_iBuyOnceCost = iCost;
		LabelRetOnce.text = string.Format(GameDataDB.GetString(575),iBuyCount,'\n',iCost);	//補登{0}次{1}{2}寶石

		//再設定買到滿
		//狀況判斷
		if(timeTmp.iRetroactiveTimes >= iValue)//補登次數充足
		{
			iBuyCount = iValue;
		}
		else
		{
			iBuyCount = iNowRet;
		}

		//算錢
		iCost = 0;
		for(int i = 0;i<iBuyCount;++i)
		{
			if(sApData.iBuyRetCount > shopTmp.iShopPrizeOfLastTime)
				iCost += shopTmp.GetPrize(shopTmp.iShopPrizeOfLastTime);
			else
				iCost += shopTmp.GetPrize(sApData.iBuyRetCount + i);
		}

		m_iBuyFullCost = iCost;
		LabelRetFull.text = string.Format(GameDataDB.GetString(575),iBuyCount,'\n',iCost);//補登{0}次{1}{2}寶石
	}
	//-------------------------------------------------------------------------------------------------
	//重置AP補登資訊
	public void ResetAPRetValue()
	{
		S_TimeRec_Tmp apTimeTmp = GameDataDB.TimeRecDB.GetData((int)m_emAPType);
		if (apTimeTmp == null)
		{
			UnityDebugger.Debugger.LogError("讀取計時資料表錯誤");
			return;
		}	

		SetUIActivityValue(m_emAPType,apTimeTmp.iLimitValue);
		SetUIRetroactiveValue(m_emAPType,apTimeTmp.iRetroactiveTimes);

		int iNowAP = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityValue((int)m_emAPType);
		int iNowRet = ARPGApplication.instance.m_ActivityMgrSystem.GetAPRetTimes((int)m_emAPType);

		//設定補登按鈕狀態
		bool bSwitch = (iNowAP >= apTimeTmp.iLimitValue || iNowRet <= 0);
		//如果AP滿了且補登無次數可用 則關閉按鈕功能
		ButtonRetroactive.isEnabled = !bSwitch;

		//如果AP及補登都滿了就關閉倒數
		if(iNowAP >= apTimeTmp.iLimitValue && iNowRet >= apTimeTmp.iRetroactiveTimes)
		{
			ClearResetChallengeLabel();
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定補登說明資訊
	public void SetRetPanelText(int iCount)
	{
		LabelRetText.text = string.Format(GameDataDB.GetString(577),iCount);	//請選擇補登次數
	}
}
