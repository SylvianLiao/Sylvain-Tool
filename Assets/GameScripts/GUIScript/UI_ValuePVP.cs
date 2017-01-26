using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class ValuePVPOpponentValue
{
	public int			RoleID 		= 0;		//角色編號
	public string		RoleName 	= "";		//角色名稱
	public int			iLevel;	//對手強度
	public int			Face 		= 0;		//頭像
	public int			PetDBID1	= 0;		//寵物1
	public int			PetDBID2	= 0;		//寵物2
	public int			Power 		= 0;		//戰力
	public int			Lv 			= 0;		//等級
	public int 			Rank 		=0;			//排名
	public int			Sex			= 0;		//姓別 0:女 1:男
	public int			BodyModel	= 0;		//身體
	public int			HeadModel	= 0;		//頭
	public int			WeaponItemDBID	= 0;	//武器
	public int			WingDBID	= 0;		//翅膀
	//新需求
	public EMUM_COMPETITOR_RESULT			Result;		//結果
}

public class ValuePVPRewardValue
{
	public int rRankFrom	= 0;
	public int rRankTo		= 0;
	public int rPoint		= 0;
	public int[] ItemID;
	public int ItemID_1 	= 0;
	public int ItemID_2 	= 0;
	public int ItemID_3 	= 0;
	public int[] ItemCount;
	public int ItemCount_1	= 0;
	public int ItemCount_2	= 0;
	public int ItemCount_3	= 0;

	public ValuePVPRewardValue()
	{
		ItemID = new int[3];
		ItemCount = new int[3];
	}
}

public enum ValuePVP_PageIndex
{
	Battle = 0,
	Rank,
	Reward,
}

public enum ValuePVP_RewardIndex
{
	RewardRank = 0,
	RewardPoint,
}

public class UI_ValuePVP : NGUIChildGUI  
{
	public static int		OpponentCount	= (int)EMUM_DATAPVP_COMPETITOR_LEVEL.EMUM_COMPETITOR_LEVEL_MAX;		// 顯示的對手數量
	public static int		RankCount		= 10;		// 顯示的排名數量

	ValuePVP_PageIndex 		selectPageIndex  = ValuePVP_PageIndex.Battle;		// 選擇的頁簽
	private int 			selectSlotIndex  = -1;		// 選擇的SLOT

	ValuePVP_RewardIndex	selectRewardPageIndex	= ValuePVP_RewardIndex.RewardRank;
	public static int		RewardCount		= 20;		// 顯示的排名數量
	private int 			iRankPoint		= 0;		// 目前積分

	//	玩家資訊
//	public UISprite			SpriteRightBG 				= null;	//右半邊底版
//	public UISprite			SpritePlayerIcon 			= null;	//頭像
//	public UISprite			SpriteRewardIcon 			= null;	//獎勵

//	public UILabel			LabelPlayerLV				= null;	//玩家等級
	public UILabel 			LabelPlayerPowerValue		= null; //戰力
//	public UILabel 			LabelPlayerName				= null;	//名稱
	public UILabel 			LabelRank					= null;	//排名
	public UILabel 			LabelWin					= null;	//勝場
	public UILabel 			LabelLoses					= null;	//敗場
	public UILabel 			LabelWinningStreak			= null;	//連勝
	public UILabel 			LabelPlayerPoint			= null; //積分
	public UILabel			LabelPointGap				= null;	//排名上升還需幾場

//	public UILabel			LabelPlayerLVTitle			= null;	//玩家等級Title
//	public UILabel 			LabelPlayerPowerValueTitle	= null; //戰力Title
//	public UILabel 			LabelPlayerNameTitle		= null;	//名稱Title
//	public UILabel 			LabelRankTitle				= null;	//排名Title
//	public UILabel 			LabelWinTitle				= null;	//勝場Title
//	public UILabel 			LabelLosesTitle				= null;	//敗場Title
//	public UILabel 			LabelWinningStreakTitle		= null;	//連勝Title
//	public UILabel 			LabelPlayerPointTitle		= null; //積分Title

	//battle
	public UISprite			SpriteForBattleBG			= null;	//戰鬥按鈕底板
	public UIButton			ButtonAddChallengeTimes		= null;	//增加對戰次數
	public UIButton			ButtonRefreshOpponent		= null;	//
	public UILabel			LabelRefreshMoney			= null;	//刷新需要寶石數字
	public UILabel			LabelRefresh				= null;	//刷新

	public UIButton			ButtonFight					= null;	//開始戰鬥按鈕
	public UILabel			LabelFight					= null;	//開始戰鬥
	public UITexture		TexturePlayer				= null; //角色大圖

	public UILabel			LabelSpriteStrong			= null; //強
	public UILabel			LabelSpriteMedium			= null; //中
	public UILabel			LabelSpriteWeak				= null; //弱

	public UILabel			LabelWinTitle2				= null; //勝場
	public UILabel			LabelLosesTitle2			= null; //敗場
	public UILabel			LabelWinningStreakTitle2	= null; //連勝

	//rank
	public UISprite			SpriteForRankBG				= null;	//排行榜下方底板
	public UILabel			LabelForRankNote			= null;	//排行榜下方佈告

	//reward
	public UISprite			SpriteForRewardBG			= null;
	public UIButton			ButtonRankReward			= null;
	public UIButton			ButtonPointReward			= null;
	public UIButton			ButtonRule					= null;
	
	// Page
	//public List<UILabel>	LabelPages					= new List<UILabel>();
	//public List<UIButton>	ButtonPages					= new List<UIButton>();
	public UIButton			ButtonBattlePage			= null;
	public UIButton			ButtonRankPage				= null;
	public UIButton			ButtonRewardPage			= null;
	public UILabel			LabelBattlePage				= null;
	public UILabel			LabelRankPage				= null;
	public UILabel			LabelRewardPage				= null;

	//Rule
	public UIPanel			PanelRule					= null;
	public UILabel			LabelRuleTitle				= null;
	public UILabel			labelRuleContect			= null;
	public UIButton			ButtonRuleCheck				= null;

	// slot
	public UIGrid			gridOpponent				= null;
	public Transform		slotlocal					= null;
	public UIGrid			gridReward					= null;
	public Transform		slotrewardlocal				= null;
	public UIGrid			gridReward2					= null;
	public Transform		slotrewardlocal2			= null;
	
	public List<Slot_ValuePVP_Opponent> 	slotOpponents 	= new List<Slot_ValuePVP_Opponent>();
	public List<ValuePVPOpponentValue>		listOpponent 	= new List<ValuePVPOpponentValue>();
	public List<ValuePVPOpponentValue>		listTableOfRank = new List<ValuePVPOpponentValue>();
	//
	public List<Slot_ValuePVP_Reward>		slotRankRewards		= new List<Slot_ValuePVP_Reward>();
	public List<Slot_ValuePVP_Reward>		slotPointRewards	= new List<Slot_ValuePVP_Reward>();
	public List<ValuePVPRewardValue>		listRewardOfRank 	= new List<ValuePVPRewardValue>();
	public List<ValuePVPRewardValue>		listRewardOfPoint 	= new List<ValuePVPRewardValue>();
	//
	public UIScrollView		LeftScrollView				= null;	//控制ScrollView 行為
	//
	string slotName	= "Slot_ValuePVP_Opponent";
	string rewardSlotName = "Slot_ValuePVPRankReward";

	public UILabel			LabelChallengeTimes			= null; //可挑戰次數
	public UILabel			LabelChallengeTimesTitle	= null; //可挑戰次數

	//排名換頁
	public UILabel			LabelBotRankPage			= null;		//頁
	public UIButton			ButtonLeftArrow				= null;		//左箭頭
	public UIButton			ButtonRightArrow			= null;		//右箭頭

	//-----------------------------------------------------------------------
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ValuePVP";
	
	//-------------------------------------------------------------------------------------------------
	private UI_ValuePVP() : base(GUI_SMARTOBJECT_NAME)
	{		
	}

	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
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
		InitialUILabel();
		SetLabelRefresh();
		//獎勵業面設置
		CreatRewardSlot();
		SetRewardOfRankValue();
//		SetUILabel();

		// 設定自己的大圖
		//texture編號
		int textureIndex = ARPGApplication.instance.m_RoleSystem.GetRoleTexture();
		//設定texture位置
		//S_SpriteCoordinate_Tmp locDBF = GameDataDB.SpriteCoordinate.GetData(textureIndex);
		//TexturePlayer.transform.localPosition = new Vector3(locDBF.LeftSpriteX,locDBF.LeftSpriteY);
		//換圖
		Utility.ChangeTexture(TexturePlayer, textureIndex); 
		TexturePlayer.MakePixelPerfect();
	}

	//-------------------------------------------------------------------------------------------------
	public void start()
	{
	}

	//-------------------------------------------------------------------------------------------------
	// 建立SLOT
	public void CreatSlot()
	{
		Slot_ValuePVP_Opponent go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_ValuePVP_Opponent>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ValuePVP_Opponent load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		for(int i=0; i<RankCount; ++i)
		{
			Slot_ValuePVP_Opponent slot = Instantiate(go) as Slot_ValuePVP_Opponent;
			
			slot.transform.parent = slotlocal.transform.parent;
			slot.transform.localScale = Vector3.one;		
			slot.transform.localRotation = slotlocal.transform.localRotation;
			slot.transform.localPosition = slotlocal.transform.localPosition;
			
			slot.name = string.Format("go{0:00}",i);
//			slot.ButtonSlot.userData = i;
			slot.Clear();
			slotOpponents.Add(slot);
		}

	}
	//-------------------------------------------------------------------------------------------------
	// 建立獎勵SLOT
	public void CreatRewardSlot()
	{
		Slot_ValuePVP_Reward go = ResourceManager.Instance.GetGUI(rewardSlotName).GetComponent<Slot_ValuePVP_Reward>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ValuePVP_Reward load prefeb error,path:{0}", "GUI/"+rewardSlotName) );
			return;
		}
		
		for(int i = 0;i<RewardCount;i++)
		{
			Slot_ValuePVP_Reward slot = Instantiate(go) as Slot_ValuePVP_Reward;
			
			slot.transform.parent = slotrewardlocal.transform.parent;
			slot.transform.localScale = Vector3.one;		
			slot.transform.localRotation = slotrewardlocal.transform.localRotation;
			slot.transform.localPosition = slotrewardlocal.transform.localPosition;
			
			slot.name = string.Format("rw{0:00}",i);
			slot.Clear();
			slotRankRewards.Add(slot);
		}
		for(int i = 0;i<RewardCount;i++)
		{
			Slot_ValuePVP_Reward slot = Instantiate(go) as Slot_ValuePVP_Reward;
			
			slot.transform.parent = slotrewardlocal2.transform.parent;
			slot.transform.localScale = Vector3.one;		
			slot.transform.localRotation = slotrewardlocal2.transform.localRotation;
			slot.transform.localPosition = slotrewardlocal2.transform.localPosition;
			
			slot.name = string.Format("rw{0:00}",i);
			slot.Clear();
			slotPointRewards.Add(slot);
		}
	}
	//-------------------------------------------------------------------------------------------------
	void SetUILabel()
	{

	}
	//-------------------------------------------------------------------------------------------------
	void InitialUILabel()
	{
//		//玩家等級
//		LabelPlayerLV.text 			= "";
		//戰力
		LabelPlayerPowerValue.text  = "";
//		//名稱
//		LabelPlayerName.text 		= "";
		//排名
		LabelRank.text				= "";
		//勝場
		LabelWin.text				= "";
		//半場
		LabelLoses.text				= "";
		//連勝
		LabelWinningStreak.text		= "";
		//積分
		LabelPlayerPoint.text		= "";
		//今日挑戰次數
		LabelChallengeTimes.text 	= "";
		//排名上升還需幾場
		LabelPointGap.text			= "";

		//刷新
		LabelRefresh.text			= string.Format("{0}({1})", GameDataDB.GetString(1567),99); // 刷新 1567
		//刷新需要寶石數字
		//LabelRefreshMoney.text		= GameDataDB.GetString(1568); // 7 1568
		//開始戰鬥
		LabelFight.text				= GameDataDB.GetString(1569); //開始戰鬥 1569
		//暫定顯示資料PVP注意事項
		LabelForRankNote.text		= GameDataDB.GetString(1972); //小提醒：排行榜名次每10分鐘刷新一次！

		LabelSpriteStrong.text			= GameDataDB.GetString(1562); //強 
		LabelSpriteMedium.text			= GameDataDB.GetString(1563); //中
		LabelSpriteWeak.text			= GameDataDB.GetString(1564); //弱
		
		LabelWinTitle2.text				= GameDataDB.GetString(1565); //勝場
		LabelLosesTitle2.text			= GameDataDB.GetString(1566); //敗場
		LabelWinningStreakTitle2.text	= GameDataDB.GetString(1571); //連勝

		labelRuleContect.text			= GameDataDB.GetString(1570); //規則內容
	} 
/*
	//-------------------------------------------------------------------------------------------------
	// 設定玩家資料
	public void SetPlayerInfo(JSONPG_MtoC_PlayerDataPVPRankResult pg)
	{
		//頭像
//		int iIconNumber = ARPGApplication.instance.m_RoleSystem.GetHeadIconNumber();
//		Utility.ChangeAtlasSprite(SpritePlayerIcon, iIconNumber);
		//獎勵
		//暫無
		//玩家等級
//		LabelPlayerLV.text 			= string.Format("{0}", ARPGApplication.instance.m_RoleSystem.iBaseLevel);
		//戰力
		LabelPlayerPowerValue.text 	= string.Format("{0}", pg.iPower);
		//名稱
//		LabelPlayerName.text 		= string.Format("{0}", ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName);
		//排名
		LabelRank.text				= string.Format("{0}", pg.iRank);	
		//勝場
		LabelWin.text				= string.Format("{0}", pg.iWin);
		//半場
		LabelLoses.text				= string.Format("{0}", pg.iLose);
		//連勝
//		LabelWinningStreak.text		= string.Format("{0}", pg);	
		SetDataPVPVictories();
		//積分
		LabelPlayerPoint.text		= string.Format("{0}", pg.iPoint);
	
		iRankPoint					= pg.iPoint;

		//排名上升還需幾場
		SetPointGap(pg.iDiffPoint);

		//設定獎勵Slot
		SetPointRewardSlot(listRewardOfPoint);
		SetRankRewardSlot(listRewardOfRank);
		// 設定PVP次數
		SetChallengeTimes();
	}
*/
	//-------------------------------------------------------------------------------------------------
	// 設定對手資料
/*	public void SetOpponentValue(JSONPG_StoC_RoleSync_DataPVPCompetitor pg)
	{
		listOpponent.Clear();

		for(int i=0; i<OpponentCount; ++i)
		{
			if(listOpponent.Count<3)
			{
				ValuePVPOpponentValue savedata = new ValuePVPOpponentValue();
				savedata.RoleID 			= pg.RoleID[i];
				savedata.RoleName 			= pg.RoleName[i];
				savedata.Face 				= pg.Face[i];
				savedata.PetDBID1 			= pg.PetDBID1[i];
				savedata.PetDBID2 			= pg.PetDBID2[i];
				savedata.Lv 				= pg.Lv[i];
				savedata.Power				= pg.Power[i];
				savedata.Sex				= pg.Sex[i];
				savedata.BodyModel			= pg.BodyModel[i];
				savedata.HeadModel			= pg.HeadModel[i];
				savedata.WeaponItemDBID		= pg.WeaponItemDBID[i];

				listOpponent.Add(savedata);
			}
			else
			{
				UnityDebugger.Debugger.LogError("UI_ValuePVP SetOpponentValue Error");
			}
		}
	}
*/
	//-------------------------------------------------------------------------------------------------
	// 設定對手資料
	public void SetOpponentValue2(JSONPG_StoC_RoleSync_DataPVPCompetitor pg)
	{

		if(listOpponent.Count<3)
		{
			ValuePVPOpponentValue savedata = new ValuePVPOpponentValue();
			savedata.RoleID 			= pg.iRoleID;
			savedata.RoleName 			= pg.strRoleName;
			savedata.iLevel				= pg.iLevel;
			savedata.Face 				= pg.iFace;
			savedata.PetDBID1 			= pg.iPetDBID1;
			savedata.PetDBID2 			= pg.iPetDBID2;
			savedata.Lv 				= pg.iLv;
			savedata.Power				= pg.iPower;
		
				
			listOpponent.Add(savedata);
		}
		else
		{
			UnityDebugger.Debugger.LogError("UI_ValuePVP SetOpponentValue Error");
		}
	}
	/*
	//-------------------------------------------------------------------------------------------------
	// 設定排行榜資料(接收的資料型態要改)
	public void SetTableOfRankingValue(JSONPG_MtoC_DataPVPRank pg)
	{
		ValuePVPOpponentValue savedata = new ValuePVPOpponentValue();

			if(pg.iFace >=0)
			{
				savedata.RoleName 			= pg.strRoleName;
				savedata.Face 				= pg.iFace;
				savedata.Lv 				= pg.iLv;
				savedata.Rank				= pg.iRank;
				savedata.Power				= pg.iPoint;
				
				listTableOfRank.Add(savedata);
			}
			else
			{
				UnityDebugger.Debugger.LogError("UI_ValuePVP SetTableOfRankingValue Error");
			}
	}*/
	//-------------------------------------------------------------------------------------------------
	//設定獎勵資料
	public void SetRewardOfRankValue()
	{

		S_RankReward_Tmp rankreward = GameDataDB.RankRewardDB.GetData(1);
		S_Reward_Tmp dbf = GameDataDB.RewardDB.GetData(-1);
		for(int i = 0;i<RewardCount;i++)
		{
			//排名
			ValuePVPRewardValue savedata = new ValuePVPRewardValue();
			savedata.rRankFrom = rankreward.RankReward[i].iPointRankFrom;
			savedata.rRankTo = rankreward.RankReward[i].iPointRankTo;
			savedata.ItemID_1 = rankreward.RankReward[i].iRewardID[0];
			savedata.ItemID_2 = rankreward.RankReward[i].iRewardID[1];
			savedata.ItemID_3 = rankreward.RankReward[i].iRewardID[2];

			//積分
			ValuePVPRewardValue savedata1 = new ValuePVPRewardValue();
			savedata1.rPoint = rankreward.PointReward[i].iPoint;
			savedata1.ItemID_1 = rankreward.PointReward[i].iRewardID[0];
			savedata1.ItemID_2 = rankreward.PointReward[i].iRewardID[1];
			savedata1.ItemID_3 = rankreward.PointReward[i].iRewardID[2];

			//設定物品及數量
			for (int j = 0; j < 3; ++j) {
				//排名
				savedata.ItemID[j] = rankreward.RankReward[i].iRewardID[j];
				dbf = GameDataDB.RewardDB.GetData(savedata.ItemID[j]);
				savedata.ItemCount[j] = (dbf != null ? dbf.Count : -1);
				//積分
				savedata1.ItemID[j] = rankreward.PointReward[i].iRewardID[j];
				dbf = GameDataDB.RewardDB.GetData(savedata1.ItemID[j]);
				savedata1.ItemCount[j] = (dbf != null ? dbf.Count : -1);
			}

			listRewardOfRank.Add(savedata);
			listRewardOfPoint.Add(savedata1);
		}

	}
	//-------------------------------------------------------------------------------------------------
	//設定PVP次數倒數計時
	public void SetChallengeReciprocal(string time)
	{
		LabelChallengeTimesTitle.text = time;
	}

	//-------------------------------------------------------------------------------------------------
	// 設定刷新次數
	public void SetLabelRefresh()
	{
		//需更改為刷新次數(目前以勝利次數暫時代替)
		int val = 5-ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDataPVPResetCount(EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PVP);
		if(val <=0)
		{
			LabelRefresh.text = string.Format("{0}({1}{2}{3})",GameDataDB.GetString(1567),GameDataDB.GetString(1327),val,GameDataDB.GetString(1329)); // 刷新 1567
			ButtonRefreshOpponent.isEnabled = false;
		}
		else
		{
			LabelRefresh.text = string.Format("{0}({1})",GameDataDB.GetString(1567),val); // 刷新 1567
			ButtonRefreshOpponent.isEnabled = true;
		}
	}
	//-------------------------------------------------------------------------------------------------
	// 讀取對手資料
	public void LoadOpponentValue(S_DataPVPRank data, int index)
	{
		ValuePVPOpponentValue savedata = new ValuePVPOpponentValue();
		/*
		savedata.RoleID 		= data.RoleID;
		savedata.RoleName 		= data.RoleName;
		savedata.emLevel		= data.emLevel;
		savedata.Face 			= data.Face;
		savedata.PetDBID1 		= data.PetDBID1;
		savedata.PetDBID2 		= data.PetDBID2;
		savedata.Lv				= data.Lv;
		savedata.Power			= data.Power;
		savedata.Sex			= data.Sex;
		savedata.BodyModel		= data.BodyModel;
		savedata.HeadModel		= data.HeadModel;
		savedata.WeaponItemDBID	= data.WeaponItemDBID;
*/
		listOpponent.Insert(index,savedata);

		/*listOpponent[index].RoleID 			= data.RoleID;
		listOpponent[index].RoleName 		= data.RoleName;
		listOpponent[index].Face 			= data.Face;
		listOpponent[index].PetDBID1 		= data.PetDBID1;
		listOpponent[index].PetDBID2 		= data.PetDBID2;
		listOpponent[index].Lv				= data.Lv;
		listOpponent[index].Power			= data.Power;
		listOpponent[index].Sex				= data.Sex;
		listOpponent[index].BodyModel		= data.BodyModel;
		listOpponent[index].HeadModel		= data.HeadModel;
		listOpponent[index].WeaponItemDBID	= data.WeaponItemDBID;*/
	}

	//-------------------------------------------------------------------------------------------------
	//取整串對手資料
	public List<ValuePVPOpponentValue> GetOpponentValueList()
	{
		return listOpponent;
	}

	//-------------------------------------------------------------------------------------------------
	//取整串排行榜資料
	public List<ValuePVPOpponentValue> GetRankValueList()
	{
		return listTableOfRank;
	}
	
	//-------------------------------------------------------------------------------------------------
	//取指定對手資料
	public ValuePVPOpponentValue GetOpponentValue(int val)
	{
		return listOpponent[val];
	}

	//-------------------------------------------------------------------------------------------------
	//取得目前積分
	public int GetNowRankPoint()
	{
		return iRankPoint;
	}
	//-------------------------------------------------------------------------------------------------
	// 設定對手SLOT
	public void SetOpponentSlot(List<ValuePVPOpponentValue> list)
	{
/*		for(int i=0; i<slotOpponents.Count; ++i)
		{
			if(i < RankCount && list.Count > i)
			{
				slotOpponents[i].ButtonSlot.GetComponent<UIDragScrollView>().enabled=true;
				slotOpponents[i].SetSlotValue(list[i], selectPageIndex);
				slotOpponents[i].gameObject.SetActive(true);
				//判斷開關ScrollView
				if(list.Count <= OpponentCount)
				{
					slotOpponents[i].ButtonSlot.GetComponent<UIDragScrollView>().enabled=false;
				}
				else
				{
					slotOpponents[i].ButtonSlot.GetComponent<UIDragScrollView>().enabled=true;
				}
			}
			else
			{
				slotOpponents[i].gameObject.SetActive(false);
			}
		}

		gridOpponent.Reposition();
		LeftScrollView.ResetPosition();

		switch(selectPageIndex)
		{
		case ValuePVP_PageIndex.Battle:
			//預設選取中間
			SetSeletMark(1);
			for(int i=list.Count-1; i>=0; --i)
			{
				EMUM_COMPETITOR_RESULT eResult = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDataPVPCompetitor(i);
				//UnityDebugger.Debugger.Log("挑戰目標需求: "+eResult);
				SetOpponentResult(slotOpponents[i],eResult);
				if(eResult != EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_WIN
				   && eResult != EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_LOSE)
				{
					SetSeletMark(i);
				}
			}
			break;
		case ValuePVP_PageIndex.Rank:
			EMUM_COMPETITOR_RESULT rResult = EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_NONE;
			for(int i=0; i<slotOpponents.Count; ++i)
				SetOpponentResult(slotOpponents[i],rResult);
			break;
		}
*/
	}

	//-------------------------------------------------------------------------------------------------
	//設定對手對戰結果
	private void SetOpponentResult(Slot_ValuePVP_Opponent Opponent,EMUM_COMPETITOR_RESULT eResult)
	{
		switch(eResult)
		{
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_NONE:
			Opponent.SpriteCover.gameObject.SetActive(false);
//			Opponent.LabelResult.text = "";
			break;
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_WIN:
			Opponent.SpriteCover.gameObject.SetActive(true);
//			Opponent.LabelResult.text = GameDataDB.GetString(1584);
			break;
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_LOSE:
			Opponent.SpriteCover.gameObject.SetActive(true);
//			Opponent.LabelResult.text = GameDataDB.GetString(1583);
			break;
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_MAX:
		default:
			UnityDebugger.Debugger.LogError("***Opponent's battle Result is empty");
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	// 設定RANK獎勵SLOT
	public void SetRankRewardSlot(List<ValuePVPRewardValue> list)
	{
		for (int  i= 0; i< list.Count; i++)
		{
			slotRankRewards[i].SetSlotValue(list[i],ValuePVP_RewardIndex.RewardRank);
		}
	}
	//-------------------------------------------------------------------------------------------------
	// 設定POINT獎勵SLOT
	public void SetPointRewardSlot(List<ValuePVPRewardValue> list)
	{
		for (int  i= 0; i< list.Count; i++)
		{
			slotPointRewards[i].SetSlotValue(list[i],ValuePVP_RewardIndex.RewardPoint);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//取得頁簽
	public ValuePVP_PageIndex GetSelectPage()
	{
		return selectPageIndex ;
	}

	//-------------------------------------------------------------------------------------------------
	//設定獎勵頁簽
	public void SetRewardPage(ValuePVP_RewardIndex rIndex)
	{
		selectRewardPageIndex = rIndex ;
	}
	
	//-------------------------------------------------------------------------------------------------
	//取得獎勵頁簽
	public ValuePVP_RewardIndex GetRewardPage()
	{
		return selectRewardPageIndex ;
	}
	
	//-------------------------------------------------------------------------------------------------
	//設定頁簽
	public void SetSelectPage(ValuePVP_PageIndex pIndex)
	{
		selectPageIndex = pIndex ;
	}
	
	//-------------------------------------------------------------------------------------------------
	// 取得選取SLOT
	public int GetSelectSlot()
	{
		return 	selectSlotIndex ;
	}

	//-------------------------------------------------------------------------------------------------
	// 設定選取SLOT
	public void SetSelectSlot(int val)
	{
		selectSlotIndex = val;
	}
	//-------------------------------------------------------------------------------------------------
	//清除對手選取框
//	public void ClearSelectMark()
//	{
//		for(int i=0; i<slotOpponents.Count; ++i)
//		{
//			slotOpponents[i].ClearSelectMark();
//		}
//	}

	//-------------------------------------------------------------------------------------------------
	//設定對手選取框
//	public void SetSeletMark(int index)
//	{
//		//selectSlotIndex = index;
//		SetSelectSlot(index);
//		ClearSelectMark();
//		if(index >= 0 )
//		{
//			slotOpponents[selectSlotIndex].SetSeletMark();
//		}
//	}

	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	// 設定與上一名勝場數差
	public void SetPointGap(int point)
	{
		if(point <= 0)
		{
			UnityDebugger.Debugger.Log("***與上一名分數差為負值，出錯啦!!!!");
			LabelPointGap.text = "";
			return;
		}
		int times =  point / GameDefine.PVP_WINPOINT;
		float count = point % GameDefine.PVP_WINPOINT;
		if(count > 0)
			times += 1;
		
		LabelPointGap.text = string.Format("再贏{0}場就能上升名次!!",times);
		
	}
	//-------------------------------------------------------------------------------------------------
	//設定頁數
	public void SetPageLabel(int page)
	{
		int bottom = (int)GameDefine.ACTIVITY_RANK_PAGE * (page-1) + 1;
		int top = (int)GameDefine.ACTIVITY_RANK_PAGE * page;
		LabelBotRankPage.text = string.Format("{0}-{1}", bottom, top);

		ButtonLeftArrow.isEnabled = (page != 1);
		ButtonRightArrow.isEnabled = (page*10 < listTableOfRank.Count);

	}
	//-------------------------------------------------------------------------------------------------
	//排行資訊換頁slot
	public void ChangeRankOpponentPage(List<ValuePVPOpponentValue> list,int page)
	{
/*		int dataIndex;
		EMUM_COMPETITOR_RESULT rResult = EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_NONE;
		for(int i=0; i<slotOpponents.Count; ++i)
		{
			//計算當前頁數的資料編號
			dataIndex = (page-1)* slotOpponents.Count +i;
			if(i < RankCount && list.Count > i && dataIndex < list.Count)
			{
				slotOpponents[i].ButtonSlot.GetComponent<UIDragScrollView>().enabled=true;
				slotOpponents[i].SetSlotValue(list[dataIndex], selectPageIndex);
				slotOpponents[i].gameObject.SetActive(true);
				//關閉戰鬥結果
				SetOpponentResult(slotOpponents[i],rResult);
				//判斷開關ScrollView
				if(list.Count <= OpponentCount)
				{
					slotOpponents[i].ButtonSlot.GetComponent<UIDragScrollView>().enabled=false;
				}
				else
				{
					slotOpponents[i].ButtonSlot.GetComponent<UIDragScrollView>().enabled=true;
				}
			}
			else
			{
				slotOpponents[i].gameObject.SetActive(false);
			}
		}
		
		gridOpponent.Reposition();
		LeftScrollView.ResetPosition();
*/
	}

	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}
