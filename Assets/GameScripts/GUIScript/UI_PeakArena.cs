using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PeakArena : NGUIChildGUI 
{
	//左邊
	public UIWidget			ContainerPeakArenaInfo		= null;	
	public UITexture		TexturePeakArenaInfoPic		= null;		// 活動大圖
	public Transform		TSRankInfo				= null;			// 我的積分底
	public UILabel			LabelTimeTitle			= null;			// 時間 標題
	public UILabel			LabelTime				= null;			// 時間說明
	public UILabel			LabelInfoTitle			= null;			// 活動說明 標題
	public UILabel			LabelInfo				= null;			// 活動說明
	public TweenPosition	TwPosMyRank				= null;			// 我的排名位移效果
	public UILabel			LabelMyRankTitle		= null;			// 我的排名 標題
	public UILabel			LabelMyRankNum			= null;			// 我的排名 名次

	//右上
	public UIWidget			ContainerButtonClose	= null;				// 關閉
	public UIButton			ButtonClose				= null;				// 關閉按鈕

	//右邊
	public UIWidget			ContainerPeakArena		= null;
	public UILabel			LabelPeakArenaName		= null;				// 關卡名稱

	public UIButton			ButtonRank				= null;				// 獎勵按鈕
	public UILabel			LabelRank				= null;				// 獎勵

	public UIButton			ButtonHistory			= null;				// 歷程按鈕
	public UILabel			LabelHistory			= null;				// 歷程
	public UISprite			SpriteNewLogTip			= null;				// 新歷程提示

	public UISprite			SpriteBattleLimitBG		= null;				// 挑戰次數底版
	public UILabel			LabelBattleLimit		= null;				// 挑戰次數
	public UIButton			ButtonReload			= null;				// 重置次數按鈕
	public UILabel 			LabelReload				= null;				// 重置次數
	public UILabel 			LabelReloadCost			= null;				// 重置次數花費

	public UISprite			SpriteCountdownBG		= null;				// 挑戰次數恢復底版
	public UILabel 			LabelCountdown			= null;				// 挑戰次數恢復時間LABEL
	public UIButton			ButtonResetCD			= null;				// 清除CD時間按鈕
	public UILabel 			LabelResetCD			= null;				// 清除CD時間
	public UILabel 			LabelResetCDCost		= null;				// 清除CD時間花費
	
	public UIButton			ButtonChangeOpponent	= null;				// 換對手按鈕
	public UILabel 			LabelChangeOpponent		= null;				// 換對手LABEL

	public UIPanel			PanelMask				= null;				// Sync用
	
	//slot
	public Transform		StageList				= null;				// 關卡列表
	public UIGrid			GridStageList			= null;				// 關卡列表
	
	//-------------------------------------------------------------------------------------------------
//	private ENUM_APType 			m_CountDownType  		= ENUM_APType.ENUM_APType_Null;
	public CdTimer					m_PeakCdTimer			= null;			//倒數時間
	//-------------------------------------------------------------------------------------------------
	//SLOT
	//	[HideInInspector]
	public string					slotName 		= "Slot_PeakArena";
	public List<Slot_PeakArena>		slotPVP	 		= new List<Slot_PeakArena>();

	//
	List<int> tempDIDList	= new List<int>();

	//-------------------------------------新手教學用-------------------------------------
	public UIPanel			panelGuide				= null; //教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //全螢幕按鈕
	public UISprite			spGuideShowRankInfo		= null; //導引介紹排行資訊
	public UILabel			lbGuideShowRankInfo		= null;
	public UISprite			spGuideRankReward	 	= null; //導引點擊排行榜
	public UILabel			lbGuideRankReward	 	= null;
	public UISprite			spGuideShowEnemy	 	= null; //導引介紹競技場對手
	public UILabel			lbGuideShowEnemy	 	= null;
	public UISprite			spGuidePVPHistory	 	= null; //導引PVP歷程按鈕
	public UILabel			lbGuidePVPHistory	 	= null;
	public UILabel			lbGuideFinish	 		= null;	//導引教學結束

	private const string GUI_SMARTOBJECT_NAME = "UI_PeakArena";
	//-------------------------------------------------------------------------------------------------
	private UI_PeakArena() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		CreateSlot();

		LabelTimeTitle.text 	= GameDataDB.GetString(5302);	//活動時間
		LabelTime.text 			= GameDataDB.GetString(5311);	//每日[00FF00]21:00[-]結算巔峰獎勵。並於[00FF00]22:00[-]重啟。
		LabelInfoTitle.text 	= GameDataDB.GetString(5303);	//活動說明
		LabelInfo.text 			= GameDataDB.GetString(5312);	//擊敗對手並取代名次。登上巔峰，稱霸群雄。
		LabelMyRankTitle.text 	= GameDataDB.GetString(5304);	//我的排名
		LabelPeakArenaName.text = GameDataDB.GetString(5305);	//顛峰競技場
		LabelRank.text 			= GameDataDB.GetString(5306);	//顛峰獎勵
		LabelHistory.text 		= GameDataDB.GetString(5307);	//歷程
		LabelReload.text 		= GameDataDB.GetString(5308);	//重置次數
		LabelResetCD.text 		= GameDataDB.GetString(5309);	//結束準備
		LabelChangeOpponent.text = GameDataDB.GetString(5310);	//刷新對手

		PanelMask.gameObject.SetActive(false);
	}

	//-------------------------------------------------------------------------------------------------
	public void Update()
	{
		if (m_PeakCdTimer != null && m_PeakCdTimer.isStart)
		{
			LabelCountdown.text = string.Format(GameDataDB.GetString(5325),	//挑戰準備中 {0}
												m_PeakCdTimer.ShowTime(ENUM_CDTime_ShowTime.Minute));
			//設定按鈕需要多少寶石
			LabelResetCDCost.text = string.Format(GameDataDB.GetString(5326),	//X{0}
			                                      m_PeakCdTimer.GetCountDownTime(ENUM_CDTime_ShowTime.Minute)+1);
		}
	}

	//-------------------------------------------------------------------------------------------------
	void CreateSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_PeakArena";
		}
		
		Slot_PeakArena go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_PeakArena>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_PeakArena load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		for(int i=0; i<GameDefine.PEAKPVP_OPPONENT_MAX; ++i)
		{
			//
			Slot_PeakArena newgo= Instantiate(go) as Slot_PeakArena;
			
			newgo.transform.parent			= GridStageList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("Slot_PeakArena{0:00}",i);
			newgo.ButtonBattle.userData = i;
			newgo.InitialSlot();
			
			slotPVP.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	//設定玩家資料
	public void SetUIRank(S_PlayerRankData data)
	{
//		if(data.rank == 0)
		if(data.iPoint == 0)
		{
			LabelMyRankNum.text	= GameDataDB.GetString(2819);	//榜外
		}
		else
		{
			LabelMyRankNum.text	= string.Format("{0}", GameDefine.DEF_PEAKPVP_BOTCOMPETITOR_COUNT-data.iPoint+1); //名次
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

		// 活動大圖
		Utility.ChangeTexture(TexturePeakArenaInfoPic, infoDBF.iBackGroundType);
		
		// 關卡名稱
		LabelPeakArenaName.text = GameDataDB.GetString(infoDBF.CName);


		// 今日可挑戰次數 及 挑戰次數回復錢
		S_TimeRec_Tmp timeTmp = GameDataDB.TimeRecDB.GetData(infoDBF.iVariablePos);
		int count = ARPGApplication.instance.m_ActivityMgrSystem.CheckActivityValue(infoDBF.GUID);
		string tempStr;
		if(count == 0)
		{
			tempStr = GameDataDB.GetString(5333);	//可挑戰次數 [FF3C64]{0}/{1}[-]
		}
		else
		{
			tempStr = GameDataDB.GetString(5324);	//可挑戰次數 {0}/{1}
		}
		LabelBattleLimit.text =	string.Format(tempStr,
		                                      count,
		                                      timeTmp.iLimitValue);


		//設定還能不能買次數
		if(ARPGApplication.instance.m_ActivityMgrSystem.CheckPeakPVPBuyReload() > 0)
		{
			ButtonReload.gameObject.SetActive(true);

			//設定恢復次數價錢
			if(count >0)
			{
				ButtonReload.gameObject.SetActive(false);
			}
			else
			{
				ButtonReload.gameObject.SetActive(true);
				
				int prize = ARPGApplication.instance.m_ActivityMgrSystem.CheckPeakPVPBuyReload();
				if(prize > 0)
				{
					LabelReloadCost.text = string.Format(GameDataDB.GetString(5326),	//X{0}
					                                     prize);
				}
			}
		}
		else
		{
			ButtonReload.gameObject.SetActive(false);
			LabelBattleLimit.text = GameDataDB.GetString(5329);		//今日次數已使用完畢
		}

		//新歷程標示
		SpriteNewLogTip.gameObject.SetActive(ARPGApplication.instance.m_ActivityMgrSystem.GetRankChangeFlag());
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot()
	{
		for(int i=0; i<slotPVP.Count; ++i)
		{
			slotPVP[i].SetSlot(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPeakPVPOpponent(i));
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void HideSlot()
	{
		for(int i=0; i<slotPVP.Count; ++i)
		{
			slotPVP[i].SetSlot(null);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlotButtonEnable(bool val)
	{
//		for(int i=0; i<slotPVP.Count; ++i)
//		{
//			slotPVP[i].ButtonBattle.isEnabled = val;
//		}

		for(int i=0; i<slotPVP.Count; ++i)
		{
			slotPVP[i].SetSlotButtonColor(val);
		}

	}

	//-------------------------------------------------------------------------------------------------
	public void SetClearCDBtn()
	{

	}
}
