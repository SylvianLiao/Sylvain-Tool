using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;
using System.Collections;
using System.Text;

//-----------------------------------------------------------------------------------------------------
public class DungeonInfo
{
	public UIButton			btnInfo				= null;
	public UISprite			spritePlatform		= null;				//圓盤
	public UISprite			spriteIcon			= null;				//副本圖樣
	public UISprite			spritelock			= null; 			//鎖圖
	public BoxCollider		spriteLockCollider	= null; 			//鎖圖
	public UISprite			spriteRoadLine		= null; 			//路線圖
	public UISprite[]		spriteStar			= new UISprite[4];	//星級評價
	public UILabel			lbLVLimit			= null;				//等級限制
	public bool				bDark;
	public bool				bCloseAutoMission	= false;			//本關卡是否要關閉自動導引任務開關
	//
	[System.NonSerialized]
	public S_Dungeon_Tmp	dbfData				= null;
	//
	private const string 	m_LineName			= "Sprite";
	protected int 			iBossLock			= 129000;
	//-----------------------------------------------------------------------------------------------------
	public void assigneGameObject(Transform mode)
	{
		if(mode == null)
			return;
		
		Transform	t;
		btnInfo				= mode.GetComponent<UIButton>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Platform)");
		spritePlatform		= t.GetComponent<UISprite>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Icon)");
		spriteIcon 			= t.GetComponent<UISprite>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Lock)");
		spritelock 			= t.GetComponent<UISprite>();
		spriteLockCollider  = spritelock.GetComponent<BoxCollider>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Star1)");
		spriteStar[0] 		= t.GetComponent<UISprite>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Star2)");
		spriteStar[1] 		= t.GetComponent<UISprite>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Star3)");
		spriteStar[2] 		= t.GetComponent<UISprite>();
		//
		t					= btnInfo.transform.FindChild("Sprite(Star4)");
		spriteStar[3] 		= t.GetComponent<UISprite>();
		//
		t					= btnInfo.transform.FindChild("Label(LVLimit)");
		lbLVLimit	 		= t.GetComponent<UILabel>();

		Init();
	}
	//------------------------------------bdar-----------------------------------------------------------------
	public void Init()
	{
		for(int i=0; i<spriteStar.Length; ++i)
			spriteStar[i].gameObject.SetActive(false);
		spritelock.gameObject.SetActive(false);
		btnInfo.gameObject.SetActive(false);
		lbLVLimit.gameObject.SetActive(false);
		lbLVLimit.text = "";
		dbfData = null;
		spriteIcon.color = new Color(0.4f,0.4f,0.4f);
	}
	//-----------------------------------------------------------------------------------------------------
	public virtual void SetData(S_Dungeon_Tmp dbf , List<UISprite> lineContainer)
	{
		Init();
		//先移除前一個assign 的事件
		UIEventListener.Get(spritelock.gameObject).onClick -= AlertNotYetOpenDungeon;
		
		if(dbf == null)
		{
			btnInfo.gameObject.SetActive(false);
			return;
		}
		dbfData = dbf;

		btnInfo.gameObject.SetActive(true);
		//預設路線圖
		if (spriteRoadLine != null)
		{
			switch(spriteRoadLine.spriteName)
			{
			case "UI_dgline_01_n":
				spriteRoadLine.spriteName = "UI_dgline_01_g";
				break;
			case "UI_dgline_02_n":
				spriteRoadLine.spriteName = "UI_dgline_02_g";
				break;
			case "UI_dgline_03_n":
				spriteRoadLine.spriteName = "UI_dgline_03_g";
				break;
			case "UI_dgline_04_n":
				spriteRoadLine.spriteName = "UI_dgline_04_g";
				break;
			case "UI_dgline_05_n":
				spriteRoadLine.spriteName = "UI_dgline_05_g";
				break;
			}
		}

		//
		lbLVLimit.text = string.Format(GameDataDB.GetString(1595), dbf.iLevelLimit.ToString());
		//設定鎖圖
		switch(dbf.DungeonIconSize)
		{
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_Small:
			spriteIcon.SetDimensions(100 , 100);
			break;
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_Medium:
			spriteIcon.SetDimensions(120 , 120);
			break;
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_big: //BOSS關
			Utility.ChangeAtlasSprite(spritelock,iBossLock);
			spriteIcon.SetDimensions(190 , 190);
			spritelock.SetDimensions(140 , 140);
			break;
		}
		Utility.ChangeAtlasSprite(spriteIcon,dbf.iDungeonIcon);
		
		bDark = true;
		
		//檢查關卡解鎖條件, 等級不足= 鎖住+不防點擊、任務未解= 鎖住+防點擊
		S_QuestData_Tmp questDBF = null;
		//判斷任務ID是否到達
		if(dbf.iUnlockQuestID >= 0)
		{
			//判斷等級是否足夠
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel() < dbf.iLevelLimit)
			{
				spritelock.gameObject.SetActive(true);
				if (spriteLockCollider)
					spriteLockCollider.enabled = false;
				lbLVLimit.gameObject.SetActive(true);
			}

			questDBF = GameDataDB.QuestDB.GetData(dbf.iUnlockQuestID);
			//判斷任務是否已達解鎖關卡
			if(questDBF!=null && questDBF.iPreFlag>=0)
			{
				if (ARPGApplication.instance.m_RoleSystem.sBaseQuestFlag.Get(questDBF.iPreFlag))
				{
					bDark = false;
					spriteIcon.color = Color.white;
				}
				else
				{
					if (spriteLockCollider)
						spriteLockCollider.enabled = true;
					spritelock.gameObject.SetActive(true);
					UIEventListener.Get(spritelock.gameObject).onClick += AlertNotYetOpenDungeon;
					btnInfo.enabled = false;
				}
			}
		}

		//取得路線圖
		if(lineContainer!=null && lineContainer.Count!=0 && dbf.iGroupID>0)
		{
			if(dbf.iGroupID>lineContainer.Count)
			{
				UnityDebugger.Debugger.LogWarning("請企劃檢查第"+dbf.iGroup+"章的DungeonGUID"+dbf.GUID+"Index"+dbf.iGroupID);
				return;
			}
			UISprite SpriteRoad = lineContainer[dbf.iGroupID-1];
			if (SpriteRoad != null)
				spriteRoadLine = SpriteRoad;
		}
			
		if(bDark == false)
		{
			//設定路線圖
			if (spriteRoadLine != null)
			{
				switch(spriteRoadLine.spriteName)
				{
				case "UI_dgline_01_g":
					spriteRoadLine.spriteName = "UI_dgline_01_n";
					break;
				case "UI_dgline_02_g":
					spriteRoadLine.spriteName = "UI_dgline_02_n";
					break;
				case "UI_dgline_03_g":
					spriteRoadLine.spriteName = "UI_dgline_03_n";
					break;
				case "UI_dgline_04_g":
					spriteRoadLine.spriteName = "UI_dgline_04_n";
					break;
				case "UI_dgline_05_g":
					spriteRoadLine.spriteName = "UI_dgline_05_n";
					break;
				}
			}
			//利用關卡結束時的任務編號檢查此關卡是否已解過，挑戰已解過關卡時會關閉自動任務
			if (dbf.StageType == ENUM_StageType.ENUM_StageType_Normal)
			{
				bCloseAutoMission = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckQuestUnlockOrNot(dbf.iEndQuestID);
			}
			//挑戰特殊關卡一律會關閉自動任務
			else if (dbf.StageType == ENUM_StageType.ENUM_StageType_hard ||
			         dbf.StageType == ENUM_StageType.ENUM_StageType_Money ||
			         dbf.StageType == ENUM_StageType.ENUM_StageType_EXP)
			{
				bCloseAutoMission = true;
			}
			//顯示星級(限主關卡)
			if(dbf.iPart == 0)
			{
				//取得關卡星等
				RoleStageData rsData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbf.GUID);
				//開鎖但尚未進入過
				if(rsData == null)
					return;
				else
				{
					for(int i=0; i<rsData.iStar; ++i)
					{
						spriteStar[i].gameObject.SetActive(true);
					}
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	protected void AlertNotYetOpenDungeon(GameObject gb)
	{
		string str = GameDataDB.GetString(2107);					//"關卡未解鎖"
		ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
	}
	//-----------------------------------------------------------------------------------------------------
	//特別針對需求製作重訂大小
	public void ResetIconSize(bool isBigger)
	{
		if(isBigger == true)
			spriteIcon.SetDimensions(80 , 80);
		else
			spriteIcon.SetDimensions(60 , 60);
	}
}

//-----------------------------------------------------------------------------------------------------
//章_介面_成員
[Serializable]
class ChooseDungeonChapterBase
{
	public UIPanel		m_panelBase				= null;
	public UILabel		m_lbChapterName			= null; //章節名稱
	//左右切換鈕
	public UIWidget		m_wgButton				= null;
	public UIWidget		m_wgChooseChapter		= null; //左右切換相關
	public UIButton		m_btnLeftChoose			= null; //往左切換
	public UIButton		m_btnRightChoose		= null; //往右切換
	//底下各關卡資訊列
	public UIPanel		m_panelBottomStageInfo	= null; //各章節關卡相關
	public GameObject	m_LeftCloud				= null;
	public GameObject	m_RightCloud			= null;
	public GameObject	m_ChapterNameContainer	= null;
	public UIToggle		m_tgNormal				= null;	//切換困難/一般模式使用
	public UIToggle		m_tgHard				= null;	//切換困難/一般模式使用
	public UILabel		m_lbNormal				= null;	//切換困難/一般模式使用
	public UILabel		m_lbHard				= null;	//切換困難/一般模式使用
	public UITexture	m_texHardMask			= null;	//切換困難/一般模式使用
	//獎勵相關
	public UILabel		m_lbRewardTitle			= null;
	public UILabel		m_lbrewardStarCount		= null;
	public UIButton		m_btnReward0			= null;
	public UIButton		m_btnReward1			= null;
	public UIButton		m_btnReward2			= null;
	public UIButton		m_btnReward3			= null;
	public UIButton		m_btnReward4			= null;
	public UIWidget		m_okReward1				= null;
	public UIWidget		m_okReward2				= null;
	public UIWidget		m_okReward3				= null;
	public UIWidget		m_okReward4				= null;
	public UIWidget		m_getReward1			= null;
	public UIWidget		m_getReward2			= null;
	public UIWidget		m_getReward3			= null;
	public UIWidget		m_getReward4			= null;
	public UILabel		m_lbReward1Star			= null;
	public UILabel		m_lbReward2Star			= null;
	public UILabel		m_lbReward3Star			= null;
	public UILabel		m_lbReward4Star			= null;
	public UISlider		m_sliderStars			= null;
	public GameObject	m_BottomZoneContainer	= null;

	//
	public void SetActive(bool bSet)
	{	m_panelBase.gameObject.SetActive(bSet);	}
	//
	public void Init()
	{
		m_lbHard.text				= GameDataDB.GetString(2730);	//"困難"
		m_lbNormal.text				= GameDataDB.GetString(2731);	//"普通"
		m_lbRewardTitle.text		= GameDataDB.GetString(2167);	//"副本星數獎勵"
		m_lbrewardStarCount.text	= "0/0";
		m_sliderStars.value			= 0;
	}
}
//-----------------------------------------------------------------------------------------------------
//節_介面_成員
[Serializable]
class ChooseDungeonStageBase
{
	public UIPanel		m_panelBase					= null;
	public UISprite		m_spriteBG					= null;
	//
	public UILabel		m_lbStageName				= null;	//關卡名稱
	public UIWidget		m_containerNormal			= null; //關卡名稱底圖一般
	public UIWidget		m_containerHard				= null; //關卡名稱底圖困難
	public UILabel		m_lbStageSummary			= null;	//關卡介紹
	public UILabel		m_lbStageSummaryTitle		= null;	//關卡介紹標題
	public UISprite		m_spriteStageIcon			= null;	//關卡圖示
	public UILabel		m_lb2StarInfo				= null;	//二星說明
	public UILabel		m_lb3StarInfo				= null;	//三星說明
	public UILabel		m_lb4StarInfo				= null;	//四星說明
	public UIButton		m_btnStartStage				= null;	//挑戰按鈕
	public UIButton		m_btnMopUp					= null;	//掃蕩按鈕
	public UIButton		m_btnFormation				= null;	//使用戰陣按鈕
	public UIGrid		m_gNeedAP					= null;	//所需AP數
	public UISprite		m_spriteNeedAP				= null;	//所需AP數
	public UILabel		m_lbNeedAP					= null;	//所需AP數
	public UILabel		m_lbChallegeTimes1			= null;	//挑戰次數
	public UILabel		m_lbChallegeTimes2			= null;	//挑戰次數
	public UILabel		m_lbChallegeTimes3			= null;	//挑戰次數
	public UILabel		m_lbTeamInfo				= null;	//我的隊伍
	public UILabel		m_lbTreasureInfo			= null;	//機會獲得
	public UILabel		m_lbPlusInfo				= null;	//關卡加成
	public UILabel		m_lbMopUp					= null;	//掃蕩字
	public UILabel		m_lbStartStage				= null; //開始關卡字樣
	public UISprite		m_spriteFormationBackground = null;	//戰陣背景圖示
	public GameObject	m_gFakeMopUp				= null; //為了按鈕關閉時仍可支援點擊功能而做的假按鈕
	public GameObject	m_gFakeStartStage			= null; //為了按鈕關閉時仍可支援點擊功能而做的假按鈕
	public UILabel		m_lbStarsTitle				= null;	//星等條件標題
	public UISprite[]	m_spriteStarsInfo			= null;	//星等條件說明
	public UIWidget		m_wgBossBoss1				= null;	//魔王字
	public UIWidget		m_wgBossBoss2				= null;	//頭目字
	public UISlider		m_sliderDegree				= null;	//關卡進度
	public UISprite[]	m_spriteDegreePoint			= null;	//關卡進度點圖示
	public UISprite		m_spriteDegreeOne			= null;	//關卡進度點圖示
	public UIWidget[]	m_wgStageTweens				= null;	//Tween效果
	//格 相關
	public GameObject[]		m_gTeamPosArray			= null;	//隊伍圖
	public GameObject[]		m_gTreasuresPosArray	= null;	//關卡獎勵圖
	public GameObject[]		m_gPlusPosArray			= null;	//加成角色圖
	public UIWidget			m_wgEnemyContainer		= null;	//關卡敵方集合
	public UIWidget			m_wgTeamContainer		= null;	//隊伍集合
	public UIWidget			m_wgTreasuresContainer	= null; //關卡獎勵集合
	public UIWidget			m_wgPlusContainer		= null;	//加成角色集合
	public UISprite[]		m_spritePlusStar		= null;	//加成角色星星顯示
	public UISprite[]		m_spriteTeamVocation	= null;	//隊伍中寵物職業
	public UILabel[]		m_lbTeamAttribution		= null;	//隊伍中寵物屬性
	public Slot_RoleIcon	m_slotPlayerFace		= null;	//玩家大頭圖
	public UITexture[]		m_textureBosses			= null;	//Boss全身圖示
	public UISprite			m_spriteBossVocation	= null;	//Boss職業圖示
	public UILabel			m_lbBossName			= null;	//Boss名稱
	//
	[HideInInspector]public Slot_Item[] m_slotTeamArray				= new Slot_Item[2];	//隊伍按鈕
	[HideInInspector]public Slot_Item[]	m_slotTreasuresArray		= new Slot_Item[3];	//獎勵按鈕
	[HideInInspector]public Slot_Item[]	m_slotPlusArray				= new Slot_Item[GameDefine.DUNGEON_BONUSPETGUID_MAX];
	[HideInInspector]public List<DungeonInfo>	m_dungeonInfoList	= new List<DungeonInfo>();	//副本相關資訊陣列
	//
	public void SetActive(bool bSet)
	{	m_panelBase.gameObject.SetActive(bSet);	}

	public bool IsActive()
	{	return m_panelBase.gameObject.activeSelf;}
	//
	public void Init()
	{
		m_lbStageName.text			= "";
		m_lbStageSummary.text		= "";
		m_lbStageSummaryTitle.text	= GameDataDB.GetString(976);	//"故事劇情"
		m_lb2StarInfo.text			= "";
		m_lb3StarInfo.text			= "";
		m_lb4StarInfo.text			= GameDataDB.GetString(939);	//"攜帶指定夥伴，並在時限內通關";
		m_lbNeedAP.text				= "0";
		m_lbChallegeTimes1.text		= GameDataDB.GetString(979);	//"剩餘挑戰"
		m_lbChallegeTimes2.text		= "";
		m_lbChallegeTimes3.text		= GameDataDB.GetString(980);	//"次"
		m_lbTeamInfo.text			= GameDataDB.GetString(975);	//"隊伍資訊";
		m_lbTreasureInfo.text		= GameDataDB.GetString(2101);	//"關卡獎勵";
		m_lbPlusInfo.text			= GameDataDB.GetString(978);	//"額外星數獎勵";
		m_lbMopUp.text				= GameDataDB.GetString(2105);	//"掃蕩"
		m_lbStartStage.text 		= GameDataDB.GetString(2102);	//"挑戰"
		m_lbStarsTitle.text			= GameDataDB.GetString(977);	//"星等評價"
		m_dungeonInfoList.Clear();
		m_lbBossName.text			= "";
		m_sliderDegree.value		= 0;
	}
}
//-----------------------------------------------------------------------------------------------------
//獎勵_介面_成員
[Serializable]
class ChooseDungeonRewardBase
{
	public UIPanel		m_panelBase				= null;		//關卡總星數獎勵介面
	public UIButton		m_btnQuit				= null;		//關閉按鈕
	//
	public UILabel		m_lbCSTitle				= null;
	public GameObject	m_gStarReward1			= null;		//星數獎勵1容器
	public UILabel		m_lbCSStep				= null;		//條件
	public Slot_Item[]	m_CSRewardsArray		= null;		//獎勵道具
	public UILabel		m_lbBtnGet				= null;		//取得按鈕LABEL
	public UISprite		m_spBtnCSGet			= null;
	public UIButton		m_btnCSGet				= null;		//取得按鈕

	//
	public void SetActive(bool bSet)
	{	m_panelBase.gameObject.SetActive(bSet);	}
	//
	public void Init()
	{
		m_lbCSTitle.text= GameDataDB.GetString(2167);	//"副本星數獎勵"
		m_lbCSStep.text	= string.Format(GameDataDB.GetString(2171), "XX");	//"累計 {0} 星獎勵"
		m_lbBtnGet.text	= GameDataDB.GetString(2168);	//"領取"
	}
}
//-----------------------------------------------------------------------------------------------------
class UI_ChooseDungeon : NGUIChildGUI
{
	public UIPanel								panelBase				= null;
	//
	public ChooseDungeonChapterBase				CDChapterBase			= new ChooseDungeonChapterBase();	//章_介面_成員
	public ChooseDungeonStageBase				CDStageBase				= new ChooseDungeonStageBase();		//節_介面_成員
	public ChooseDungeonRewardBase				CDRewardBase			= new ChooseDungeonRewardBase();	//獎勵_介面_成員
	//
	public UITexture							textBottomColldier		= null; //全螢幕Collider
	//
	[HideInInspector]public UI_MopUpBox			uiTopStateView			= null;
	//
	[System.NonSerialized]
	public S_ChapterItem_Tmp					sChapterItem			= null;	//目前章節獎勵資料
	private const string 						GUI_SMARTOBJECT_NAME 	= "UI_ChooseDungeon";

#region 章_地圖關卡
	[HideInInspector]public Slot_DungeonChapter	nowSlotChapter			= null;						//目前章節Slot
	[HideInInspector]public List<DungeonInfo>	dungeonInfoList			= new List<DungeonInfo>();	//副本相關資訊陣列
	[HideInInspector]public Slot_DungeonChapter	changedSlotChapter		= null;						//要切換之目標章節Slot
	[HideInInspector]public int 				iMaxUnlockNormalChapter	= 1;						//紀錄最大已解鎖一般章數
	[HideInInspector]public int					iMaxUnlockHardChapter	= 1;						//紀錄最大已解鎖困難章數
	public UIButton								btnDungeonPrefab		= null;						//副本的Prefab
	public UISprite								spriteHighlight			= null;						//選取關卡指引圈
	public int									iMaxUnlockChapter
	{
		get
		{
			int maxChapter = 0;
			switch(nowGroupType)
			{
			case ENUM_GroupRank_Type.ENUM_Rank_Normal:	maxChapter = iMaxUnlockNormalChapter;	break;
			case ENUM_GroupRank_Type.ENUM_Rank_Hard:	maxChapter = iMaxUnlockHardChapter;		break;
			}
			return maxChapter;
		}
	}
#endregion 章_地圖關卡

#region 紀錄
	public	ENUM_GroupRank_Type					nowGroupType			= ENUM_GroupRank_Type.ENUM_Rank_Max;
	private int									m_nowNormalChapter		= 0;
	private int									m_nowHardChapter		= 0;
	public	int									nowChapter
	{
		set
		{
			switch(nowGroupType)
			{
			case ENUM_GroupRank_Type.ENUM_Rank_Normal:
				m_nowNormalChapter = value;
				break;
			case ENUM_GroupRank_Type.ENUM_Rank_Hard:
				m_nowHardChapter = value;
				break;
			}
		}
		get
		{
			S_Dungeon_Tmp NewestDBF = null;
			int returnChapter = 0;
			switch(nowGroupType)
			{
			case ENUM_GroupRank_Type.ENUM_Rank_Normal:
				//取得最新任務進度關卡
				if(m_nowNormalChapter <= 0)
				{
					NewestDBF = ChooseDungeonState.getNewestDungeonDBF(ARPGApplication.instance.m_RoleSystem.iNowRunQuestID);
					if(NewestDBF != null)
						m_nowNormalChapter = NewestDBF.iGroup;
				}
				returnChapter = m_nowNormalChapter;
				break;
			case ENUM_GroupRank_Type.ENUM_Rank_Hard:
				//取得最新困難關卡進度
				if(m_nowHardChapter <= 0)
				{
					NewestDBF = ChooseDungeonState.getNewestHardDungeonDBF(iMaxUnlockNormalChapter);
					if(NewestDBF != null)
						m_nowHardChapter = NewestDBF.iGroup;
				}
				returnChapter = m_nowHardChapter;
				break;
			}
			return returnChapter;
		}
	}
	private S_Dungeon_Tmp						m_nowNormalStageDBF	= null;
	private S_Dungeon_Tmp						m_nowHardStageDBF	= null;
	public S_Dungeon_Tmp						nowStageDBF
	{
		set
		{
			switch(nowGroupType)
			{
			case ENUM_GroupRank_Type.ENUM_Rank_Normal:
				m_nowNormalStageDBF = value;
				break;
			case ENUM_GroupRank_Type.ENUM_Rank_Hard:
				m_nowHardStageDBF = value;
				break;
			}
		}
		get
		{
			S_Dungeon_Tmp returnDBF = null;
			switch(nowGroupType)
			{
			case ENUM_GroupRank_Type.ENUM_Rank_Normal:
				//取得最新任務進度關卡
				if(m_nowNormalStageDBF == null)
					m_nowNormalStageDBF = ChooseDungeonState.getNewestDungeonDBF(ARPGApplication.instance.m_RoleSystem.iNowRunQuestID);
				returnDBF = m_nowNormalStageDBF;
				break;
			case ENUM_GroupRank_Type.ENUM_Rank_Hard:
				//取得最新困難關卡進度
				if(m_nowHardStageDBF == null)
					m_nowHardStageDBF = ChooseDungeonState.getNewestHardDungeonDBF(iMaxUnlockNormalChapter);
				returnDBF = m_nowHardStageDBF;
				break;
			}
			return returnDBF;
		}
	}
#endregion 紀錄

#region 執行用變數
	public	static float		fEffectTime		= 0.5f;
	[HideInInspector]public	int					iStarRewardIndex		= -1;			//星數獎勵標籤
	//
	private List<S_Dungeon_Tmp> m_dbfDungeonList		= new List<S_Dungeon_Tmp>();	//同章內的DBF副本資料
	private List<S_Dungeon_Tmp>	m_dbfDungeonList2		= new List<S_Dungeon_Tmp>();	//同節內的DBF副本資料
	private ChooseDungeonState	m_CDState				= null;
	private const int			m_iBetween				= 110;
	private const int			m_iNowChapterDepth		= 20;
	private const int			m_iChangedChapterDepth	= 10;
	private const string		m_SlotName				="Slot_Item";
	private const string		m_SlotChpaterName		="Slot_DungeonChapter";
	private StringBuilder 		m_Strb 					= new StringBuilder();
	private int					m_iChapterStars			= 0;		//本章節得到的星數
	private int					m_iRewardMaxStars		= 0;		//本章節最大獎勵星數
#endregion 執行用變數

#region 指引教學相關元件
	public UIPanel 			panelGuide 				= null;	//指引教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //全螢幕按鈕
	public UISprite 		spGuideSelectStage 		= null;
	public UILabel 			lbGuideSelectStage 		= null;
	public UISprite 		spGuideHighLight 		= null;
	public UISprite 		spGuideStartDungeon 	= null;
	public UILabel 			lbGuideStartDungeon 	= null;
	public UISprite 		spGuideChooseChapter 	= null;
	public UILabel 			lbGuideChooseChapter 	= null;
	public UISprite 		spGuideHardMode 		= null;
	public UILabel 			lbGuideHardMode 		= null;
	public UISprite 		spGuideStarRewardBtn	= null;
	public UILabel 			lbGuideStarRewardBtn 	= null;
	public UISprite 		spGuideFirstStarReward	= null;
	public UILabel 			lbGuideFirstStarReward 	= null;
	public UISprite 		spGuideMopUpBtn			= null;
	public UILabel 			lbGuideMopUpBtn 		= null;
	public UISprite 		spGuideStarTime			= null;
	public UILabel 			lbGuideStarTime 		= null;
	public UILabel 			lbGuideFinish 			= null;
#endregion 指引教學相關元件
	
	//-----------------------------------------------------------------------------------------------------
	private UI_ChooseDungeon() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		m_CDState = (ChooseDungeonState)ARPGApplication.instance.GetGameStateByName(GameDefine.CHOOSEDUNGEON_STATE);

		CDChapterBase.Init();
		CDStageBase.Init();
		CDRewardBase.Init();

		CDChapterBase.SetActive(true);
		//預設關閉的UI
		CDChapterBase.m_panelBottomStageInfo.gameObject.SetActive(false);
		CDChapterBase.m_BottomZoneContainer.gameObject.SetActive(false);
		CDChapterBase.m_ChapterNameContainer.SetActive(false);
		CDChapterBase.m_LeftCloud.SetActive(false);
		CDChapterBase.m_RightCloud.SetActive(false);
		CDStageBase.SetActive(false);
		CDRewardBase.SetActive(false);
		//建立夥伴格
		CreateSlot(CDStageBase.m_gTeamPosArray, CDStageBase.m_slotTeamArray);
		//建立獎勵格
		CreateSlot(CDStageBase.m_gTreasuresPosArray, CDStageBase.m_slotTreasuresArray);
		//建立加成角色格
		CreateSlot(CDStageBase.m_gPlusPosArray, CDStageBase.m_slotPlusArray);
	}
	//-----------------------------------------------------------------------------------------------------
	private void Start()
	{
		iStarRewardIndex = -1;

		CDChapterBase.SetActive(true);

		//隱藏原始模型
		btnDungeonPrefab.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		if (uiTopStateView != null)
		{
			//設定掃蕩視窗顯示層級
			uiTopStateView.panelBase.depth = CDStageBase.m_panelBase.depth + 30;
			uiTopStateView.panelResultList.depth = uiTopStateView.panelBase.depth + 1;
		}
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	//設定滑動的目標章節
	public Slot_DungeonChapter CreateSlotChpater()
	{
		if(nowChapter < 1)
			return null;
		Slot_DungeonChapter go = ResourceManager.Instance.GetGUI(m_SlotChpaterName+nowChapter.ToString()).GetComponent<Slot_DungeonChapter>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_DungeonChapter load prefeb error,path:{0}", "GUI/"+m_SlotChpaterName+nowChapter.ToString()) );
			return null;
		}
		//Slot
		Slot_DungeonChapter newgo		= GameObject.Instantiate(go) as Slot_DungeonChapter;
		newgo.transform.parent			= this.transform;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
		newgo.transform.localPosition 	= Vector3.zero;
		newgo.gameObject.SetActive(true);
		return newgo;
	}
	//-----------------------------------------------------------------------------------------------------
	//收集章資料
	public void CollectDBFDataForChapter()
	{
		if(nowChapter < 1)
			return;
		
		CleanDBFDungeonList();
		//根據順序重新排序
		GameDataDB.DungeonDB.ResetByOrder();
		//收集同章節dbf
		for(int i=0; i<GameDataDB.DungeonDB.GetDataSize(); ++i)
		{
			//找下一筆DBF資料
			S_Dungeon_Tmp dbf = GameDataDB.DungeonDB.GetDataByOrder();
			//篩選條件
			switch(dbf.StageType)
			{
			case ENUM_StageType.ENUM_StageType_Tower:
				break;
			default:
				if(dbf.iGroup==nowChapter		&&	//同章
				   dbf.iPart==0					&&	//主段
				   dbf.iGroupRank==nowGroupType	)	//同難度
					m_dbfDungeonList.Add(dbf);
				break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//收集節資料
	private void CollectDBFDataForStage(S_Dungeon_Tmp mainDBF)
	{
		if(mainDBF == null)
			return;

		m_dbfDungeonList2.Clear();

		//根據順序重新排序
		GameDataDB.DungeonDB.ResetByOrder();
		//收集同章節dbf
		for(int i=0; i<GameDataDB.DungeonDB.GetDataSize(); ++i)
		{
			//找下一筆DBF資料
			S_Dungeon_Tmp dbf = GameDataDB.DungeonDB.GetDataByOrder();
			//篩選條件
			switch(dbf.StageType)
			{
			case ENUM_StageType.ENUM_StageType_Tower:
				break;
			default:
				if(dbf.iGroup==mainDBF.iGroup			&&	//同章
				   dbf.iGroupID==mainDBF.iGroupID		&&	//同節
				   dbf.iGroupRank==mainDBF.iGroupRank	)	//同難度
					m_dbfDungeonList2.Add(dbf);
				break;
			}
		}

		m_dbfDungeonList2.Sort(CompareAbout_iPart);
	}
	//-----------------------------------------------------------------------------------------------------
	//依iPart大小排序(小至大)
	private static int CompareAbout_iPart(S_Dungeon_Tmp sdtA, S_Dungeon_Tmp stdB)
	{
		if(sdtA.iPart == stdB.iPart)
			return 0;
		else if(sdtA.iPart < stdB.iPart)
			return -1;
		return 1;
	}
	//-----------------------------------------------------------------------------------------------------
	//找出進哪一關
	private S_Dungeon_Tmp FindEnterDungeonDBF()
	{
		S_Dungeon_Tmp enterDBF = null;

		for(int i=m_dbfDungeonList2.Count-1; i>=0; --i)
		{
			enterDBF = m_dbfDungeonList2[i];
			if(enterDBF == null)
				continue;

			//第一次進入 || 未成功過關過
			RoleStageData rsData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(enterDBF.GUID);
			if(rsData==null || rsData.iStar<=0)
			{
				return enterDBF;
			}
			//可重覆挑戰
			else if(enterDBF.DungeonOption.GetFlag(ENUM_DungeonOptionFlag.ENUM_DungeonOptionFlag_OnlyOne) == false)
			{
				return enterDBF;
			}
		}

		//進入最後一筆
		return enterDBF;
	}
	//-----------------------------------------------------------------------------------------------------
	private void CleanDBFDungeonList()
	{
		if(m_dbfDungeonList.Count > 0)
			m_dbfDungeonList.Clear();
	}
	//-----------------------------------------------------------------------------------------------------
	//動態建立章_介面的關卡點
	private void GenerateDungeonInfoForChapter()
	{
		if(m_dbfDungeonList.Count>0 && btnDungeonPrefab!=null)
		{
			int iCount = 0;
			List<DungeonInfo> dif = dungeonInfoList;
			//動態增減DungeonInfo物件
			if(m_dbfDungeonList.Count >= dif.Count)
			{
				iCount = m_dbfDungeonList.Count - dif.Count;
				for(int i=0; i<iCount; ++i)
				{
					GameObject newGO = Instantiate(btnDungeonPrefab.gameObject) as GameObject;
					newGO.transform.parent = CDChapterBase.m_wgButton.transform;
					newGO.transform.localScale = Vector3.one;
					newGO.SetActive(true);
					DungeonInfo dgInfo = new DungeonInfo();
					dif.Add(dgInfo);
					dgInfo.assigneGameObject(newGO.transform);
				}
				//Profiler.EndSample();
				//將新增的btnDungeonInfo加入點擊功能 
				m_CDState.SetDungeonInfoFunction(true);
			}
			else
			{
				iCount = dif.Count - m_dbfDungeonList.Count;
				for(int i=0; i<iCount; ++i)
				{
					DestroyImmediate(dif[dif.Count-1].btnInfo.gameObject);
					dif.Remove(dif[dif.Count-1]);
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//建立寶箱Slot
	private void CreateSlot(GameObject[] goArray, Slot_Item[] slotArray)
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		if(goArray==null || goArray.Length<=0 || slotArray==null || goArray.Length!=slotArray.Length)
		{
			UnityDebugger.Debugger.LogError("UI_ChooseDungeon CreateSlot Error!!");
			return;
		}
		//Slot
		for(int i=0; i<goArray.Length; ++i)
		{
			goArray[i].SetActive(false);
			Slot_Item newgo = GameObject.Instantiate(go) as Slot_Item;
			newgo.transform.parent			= goArray[i].transform.parent;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition 	= goArray[i].transform.localPosition;
			
			newgo.name = string.Format("slotItem{0:00}", i);
			newgo.gameObject.SetActive(true);
			slotArray[i] = newgo;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//更新章_介面
	public void UpdateChapterUI()
	{
		//取得最新進度
		S_Dungeon_Tmp NewestStageDBF = null;
		NewestStageDBF = ChooseDungeonState.getNewestDungeonDBF(ARPGApplication.instance.m_RoleSystem.iNowRunQuestID);
		if(NewestStageDBF != null)
			iMaxUnlockNormalChapter = NewestStageDBF.iGroup;
		NewestStageDBF = ChooseDungeonState.getNewestHardDungeonDBF(iMaxUnlockNormalChapter);
		if(NewestStageDBF != null)
			iMaxUnlockHardChapter = NewestStageDBF.iGroup;
		//收集資料
		CollectDBFDataForChapter();
		GenerateDungeonInfoForChapter();
		
		//設定關卡資料
		m_iChapterStars		= 0;
		m_iRewardMaxStars	= 0;
		//計算本章獲得的星數
		for(int i=0; i<dungeonInfoList.Count; ++i)
		{	
			if(dungeonInfoList[i]==null || m_dbfDungeonList[i]==null)
				break;

			dungeonInfoList[i].SetData(m_dbfDungeonList[i] , nowSlotChapter.RoadList);
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dungeonInfoList[i].dbfData.GUID) != null)
			{
				m_iChapterStars += ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dungeonInfoList[i].dbfData.GUID).iStar;
			}
		}
		
		//設定副本星數獎勵資訊
		SetStarRewardInfo();
		//指派關卡位置
		AssignStagePos();
		//顯示路線圖
		nowSlotChapter.gLineContainer.SetActive(true);
		//只顯示有圖的路線
		//先隱藏
		for(int i=0; i<nowSlotChapter.RoadList.Count; ++i)
		{
			if(i == 0)
				continue;
			nowSlotChapter.RoadList[i].gameObject.SetActive(false);
		}
		//再顯示
		for(int i=0; i<m_dbfDungeonList.Count; ++i)
		{
			if(m_dbfDungeonList[i].iGroupID == 1)
				continue;
			if(m_dbfDungeonList[i].iGroupID <= nowSlotChapter.RoadList.Count)
				nowSlotChapter.RoadList[m_dbfDungeonList[i].iGroupID-1].gameObject.SetActive(true);
		}
		
		//設定章節dbf
		S_Dungeon_Tmp chapterDBF = dungeonInfoList[0].dbfData;
		//設定章節名稱
		CDChapterBase.m_lbChapterName.text = GameDataDB.GetString( chapterDBF.iGroupName );
		if(CDChapterBase.m_panelBottomStageInfo.gameObject.activeSelf == false)
			CDChapterBase.m_panelBottomStageInfo.gameObject.SetActive(true);
		//設定左右按鈕是否顯示
		CDChapterBase.m_btnLeftChoose.gameObject.SetActive(nowChapter > 1);
		CDChapterBase.m_btnRightChoose.gameObject.SetActive(nowChapter < iMaxUnlockChapter);

		//指派節
		AssignStage();
	}
	//-----------------------------------------------------------------------------------------------------
	private void AssignStage()
	{
		bool bUseAssign = (nowChapter == m_CDState.recordChapter);
		bool bRecord = false;

		//自動導引已開啟
		if(ARPGApplication.instance.m_RoleSystem.autoMission != ENUM_AutoMission.AM_Close)
		{
			bRecord = true;
		}
		else
		{
			//比對紀錄
			if(bUseAssign == true)
				nowStageDBF = m_CDState.recordStageDBF;
		}

		int i = 0;
		for(i=0; i<dungeonInfoList.Count; ++i)
		{
			DungeonInfo di = dungeonInfoList[i];
			if(di == null)
				continue;
			if(di.dbfData == null)
				continue;
			if(nowStageDBF.iGroup==di.dbfData.iGroup && nowStageDBF.iGroupID==di.dbfData.iGroupID)
			{
				if(bUseAssign == true)
				{
					m_CDState.startChooseDungeonClick(di, i, bRecord);
					break;
				}
			}
		}

		//沒指定，找本關卡中最新進度
		if(i >= dungeonInfoList.Count)
			SelectFinalStage();
	}
	//-----------------------------------------------------------------------------------------------------
	//自動選擇所處章節之最後解鎖的一關
	private void SelectFinalStage()
	{
		int index = -1;
		int MaxUnlockQuestID = 0;
		//找出最新進度之關卡，不包含額外關卡(經驗、金錢關等)
		for(int i=0; i<dungeonInfoList.Count; ++i)
		{
			//找出最後面的任務編號
			if(dungeonInfoList[i].dbfData.iUnlockQuestID > MaxUnlockQuestID)
			{
				MaxUnlockQuestID = dungeonInfoList[i].dbfData.iUnlockQuestID;
				if(dungeonInfoList[i].dbfData.StageType==ENUM_StageType.ENUM_StageType_Normal	&&
				   dungeonInfoList[i].bDark==false												)
					index = i;
			}
		}
		if(index >= 0)
		{
			m_CDState.startChooseDungeonClick(dungeonInfoList[index], index);
			if(nowGroupType==ENUM_GroupRank_Type.ENUM_Rank_Hard					&&
			   dungeonInfoList[index].dbfData.iGroup==iMaxUnlockHardChapter		)
				ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_HardTheLatestStage = dungeonInfoList[index].dbfData;
		}
		else
			UnityDebugger.Debugger.Log("未找出最新進度關卡");
	}
	//-----------------------------------------------------------------------------------------------------
	//更新節_介面
	public void UpdateStageUI()
	{
		int i = 0;
		S_MobData_Tmp mobDBF = null;

		//收集資料
		CollectDBFDataForStage(m_CDState.selectedDBF);
		m_CDState.selectedDBF = FindEnterDungeonDBF();	//切換成確定進入關卡

		//紀錄反轉List順序(前置1->前置2->主關)
		List<S_Dungeon_Tmp> inverseDungeonInfoList = new List<S_Dungeon_Tmp>();
		inverseDungeonInfoList.Clear();
		inverseDungeonInfoList.AddRange(m_dbfDungeonList2);
		inverseDungeonInfoList.Reverse();

		//設置關卡資訊
		CDStageBase.m_lbStageName.text		= GameDataDB.GetString(m_CDState.selectedDBF.iName);
		CDStageBase.m_lbStageSummary.text	= GameDataDB.GetString(m_CDState.selectedDBF.iDungeonInfo);
		CDStageBase.m_lbNeedAP.text			= m_CDState.selectedDBF.iAP.ToString();
		Utility.ChangeAtlasSprite(CDStageBase.m_spriteStageIcon, m_CDState.selectedDBF.iDungeonIcon);
		CDStageBase.m_lb2StarInfo.text		= "";
		CDStageBase.m_lb3StarInfo.text		= "";
		//設置星等條件說明
		for(i=0; i<CDStageBase.m_spriteStarsInfo.Length; ++i)
		{
			switch(i)
			{
			case 0:	CDStageBase.m_lb2StarInfo.text = BattleResultState.timeFormatMS(m_CDState.selectedDBF.iStar2Time);	break;
			case 1:	CDStageBase.m_lb3StarInfo.text = BattleResultState.timeFormatMS(m_CDState.selectedDBF.iStar3Time);	break;
			}

			CDStageBase.m_spriteStarsInfo[i].gameObject.SetActive(m_CDState.selectedDBF.iPart == 0);
			if(i == 2)
				CDStageBase.m_spriteStarsInfo[i].gameObject.SetActive(false);
		}		
		//設置關卡對手圖
		List<S_Dungeon_Tmp> tempList = new List<S_Dungeon_Tmp>();
		tempList.Clear();
		tempList.AddRange(inverseDungeonInfoList);
		for(i=0; i<tempList.Count; ++i)
		{
			//排除已過關關卡
			if(tempList[i] != m_CDState.selectedDBF)
			{
				tempList.Remove(tempList[i]);
				--i;
			}
			else
				break;
		}
		for(i=0; i<CDStageBase.m_textureBosses.Length; ++i)
		{
			if(i < tempList.Count)
			{
				if(tempList[i] != null)
				{
					mobDBF = GameDataDB.MobDataDB.GetData( tempList[i].iMobGUID[0] );
					if(mobDBF != null)
					{
						Utility.ChangeTexture(CDStageBase.m_textureBosses[i], mobDBF.FullAvatar);
					}
				}
				CDStageBase.m_textureBosses[i].gameObject.SetActive(true);
			}
			else
			{
				Utility.ChangeTexture(CDStageBase.m_textureBosses[i], -1);
				CDStageBase.m_textureBosses[i].gameObject.SetActive(false);
			}
		}
		//設置關卡對手職業名稱
		mobDBF = GameDataDB.MobDataDB.GetData( m_CDState.selectedDBF.iMobGUID[0] );
		if(mobDBF != null)
		{
			Utility.ChangeAtlasSprite(CDStageBase.m_spriteBossVocation, ARPGApplication.instance.GetPetCalssIconID(mobDBF.emCharClass));
			CDStageBase.m_spriteBossVocation.gameObject.SetActive(true);
			CDStageBase.m_lbBossName.text = GameDataDB.GetString(mobDBF.iName);
			CDStageBase.m_lbBossName.gameObject.SetActive(true);
			CDStageBase.m_wgBossBoss1.gameObject.SetActive(mobDBF.MobType == ENUM_MobType.BossBoss1);
			CDStageBase.m_wgBossBoss2.gameObject.SetActive(mobDBF.MobType == ENUM_MobType.BossBoss2);
		}
		else
		{
			CDStageBase.m_spriteBossVocation.gameObject.SetActive(false);
			CDStageBase.m_lbBossName.gameObject.SetActive(false);
			CDStageBase.m_wgBossBoss1.gameObject.SetActive(false);
			CDStageBase.m_wgBossBoss2.gameObject.SetActive(false);
		}
		//設置隊伍圖
		for(i=0; i<CDStageBase.m_slotTeamArray.Length; ++i)
		{
			int iPetGUID = 0;
			switch(i)
			{
			case 0:
				iPetGUID = ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID;
				break;
			case 1:
				iPetGUID = ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID;
				break;
			}
			CDStageBase.m_slotTeamArray[i].SetSlotWithPetID(iPetGUID, false);
			CDStageBase.m_slotTeamArray[i].gameObject.SetActive(true);
			CDStageBase.m_spriteTeamVocation[i].gameObject.SetActive(true);
			CDStageBase.m_lbTeamAttribution[i].gameObject.SetActive(true);
			S_PetData_Tmp petDBF	= GameDataDB.PetDB.GetData(iPetGUID);
			S_MobData_Tmp bossDBF	= GameDataDB.MobDataDB.GetData(m_CDState.selectedDBF.iMobGUID[0]);
			float TotalEffectValue = 0;
			if(petDBF!=null && bossDBF!=null)
			{
				Utility.ChangeAtlasSprite(CDStageBase.m_spriteTeamVocation[i], ARPGApplication.instance.GetPetCalssIconID(petDBF.emCharClass));
				bool bRestrain = false;
				for(int j=0; j<bossDBF.DifficultAdversary.Count; ++j)
				{
					//檢查怪物弱點是否有這隻寵物
					if(bossDBF.DifficultAdversary[j]!=null && bossDBF.DifficultAdversary[j]==petDBF)
					{
						bRestrain = true;
						break;
					}
				}
				float sDBPDvalue = 0;
				for(int j=0;j<m_CDState.selectedDBF.sDBPD.Length;++j)
				{
					if(m_CDState.selectedDBF.sDBPD[j].m_iBonusPet == petDBF.GUID)
					{
						S_BuffData_Tmp buff = GameDataDB.BuffDataDB.GetData(m_CDState.selectedDBF.sDBPD[j].m_iBonusPetBuff);
						if( null != buff)
						{
							sDBPDvalue = (float)buff.sAbility.fAttackDmgIncr_Per;
							break;
						}
					}
				}
				if(bRestrain == true)
				{
					TotalEffectValue = petDBF.fAffectCharClass_Per + GameDataDB.GetCharacterTypeValueToMob(petDBF.GUID,bossDBF.GUID);
					TotalEffectValue += sDBPDvalue;
					CDStageBase.m_lbTeamAttribution[i].text = string.Format("{0}{1}{2}","+",TotalEffectValue*100,"%");
				}
				else
				{
					TotalEffectValue = GameDataDB.GetCharacterTypeValueToMob(petDBF.GUID,bossDBF.GUID);
					TotalEffectValue += sDBPDvalue;
					CDStageBase.m_lbTeamAttribution[i].text = string.Format("{0}{1}{2}","+",TotalEffectValue*100,"%");
				}
			}
			//無夥伴，預設值設定
			else
			{
				CDStageBase.m_slotTeamArray[i].InitialSlot();
				Utility.ChangeAtlasSprite(CDStageBase.m_spriteTeamVocation[i], -1);
				CDStageBase.m_lbTeamAttribution[i].text = "";
			}
		}//for
		//設置主玩家大頭圖
		CDStageBase.m_slotPlayerFace.SetSlot(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iFace, ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iFaceFrameID);
		//設置獎勵圖
		for(i=0; i<CDStageBase.m_slotTreasuresArray.Length; ++i)
		{
			m_CDState.m_chestItems[i] = null;
			S_Item_Tmp dbf = GameDataDB.ItemDB.GetData(m_CDState.selectedDBF.ShowChestItem[i]);
			if(dbf != null)
			{
				m_CDState.m_chestItems[i] = dbf;
				CDStageBase.m_slotTreasuresArray[i].SetSlotWithCount(dbf.GUID, 1, false);
				CDStageBase.m_slotTreasuresArray[i].SetActive(dbf.GUID > 0);
			}
		}
		
		//設置加成角色圖
		for(i=0; i<CDStageBase.m_slotPlusArray.Length; ++i)
		{
			CDStageBase.m_slotPlusArray[i].SetSlotWithPetID(m_CDState.selectedDBF.sDBPD[i].m_iBonusPet, false);
			CDStageBase.m_slotPlusArray[i].SetActive(true);
			//尋找是否帶符合夥伴
			int iSearch = 0;
			bool bCarry = ChooseDungeonState.bSearchSamePet(m_CDState.selectedDBF.sDBPD[i].m_iBonusPet, out iSearch);
			CDStageBase.m_spritePlusStar[i].gameObject.SetActive(bCarry);
		}
		
		//取得關卡掃蕩評級資料
		RoleStageData stageData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(m_CDState.selectedDBF.GUID);
		//挑戰次數(是否為解鎖後第一次進入關卡)
		if(stageData == null)
			CDStageBase.m_lbChallegeTimes2.text = m_CDState.selectedDBF.iPlayCount.ToString();
		//計算並顯示剩餘挑戰次數
		m_CDState.UpdateShowingUI(m_CDState.CalculateChallengeRemnant());
		//設定進度顯示
		float fForegroundWidth = CDStageBase.m_sliderDegree.foregroundWidget.localSize.x;
		bool bDegree = true;
		switch(m_dbfDungeonList2.Count)
		{
		case 1:
			{
				CDStageBase.m_sliderDegree.gameObject.SetActive(false);
				CDStageBase.m_spriteDegreeOne.gameObject.SetActive(true);
			}
			break;
		case 2:
			{
				CDStageBase.m_sliderDegree.gameObject.SetActive(true);
				CDStageBase.m_spriteDegreeOne.gameObject.SetActive(false);
				CDStageBase.m_spriteDegreePoint[0].transform.localPosition = Vector3.right * fForegroundWidth * (0.0f / 100.0f);
				CDStageBase.m_spriteDegreePoint[1].transform.localPosition = Vector3.right * fForegroundWidth * (100.0f / 100.0f);
				CDStageBase.m_spriteDegreePoint[2].transform.localPosition = Vector3.right * fForegroundWidth * (10000.0f / 100.0f);

				bDegree = true;
				CDStageBase.m_sliderDegree.value = 0.0f;
				for(i=0; i<inverseDungeonInfoList.Count; ++i)
				{
					CDStageBase.m_spriteDegreePoint[i+3].gameObject.SetActive(bDegree);
					if(bDegree == true)
						CDStageBase.m_sliderDegree.value += (1.0f * i);
					if(m_CDState.selectedDBF == inverseDungeonInfoList[i])
						bDegree = false;
				}
			}
			break;
		case 3:
			{
				CDStageBase.m_sliderDegree.gameObject.SetActive(true);
				CDStageBase.m_spriteDegreeOne.gameObject.SetActive(false);
				CDStageBase.m_spriteDegreePoint[0].transform.localPosition = Vector3.right * fForegroundWidth * (0.0f / 100.0f);
				CDStageBase.m_spriteDegreePoint[1].transform.localPosition = Vector3.right * fForegroundWidth * (50.0f / 100.0f);
				CDStageBase.m_spriteDegreePoint[2].transform.localPosition = Vector3.right * fForegroundWidth * (100.0f / 100.0f);

				bDegree = true;
				CDStageBase.m_sliderDegree.value = 0.0f;
				for(i=0; i<inverseDungeonInfoList.Count; ++i)
				{
					CDStageBase.m_spriteDegreePoint[i+3].gameObject.SetActive(bDegree);
					if(bDegree == true)
						CDStageBase.m_sliderDegree.value += (0.5f * i);
					if(m_CDState.selectedDBF == inverseDungeonInfoList[i])
						bDegree = false;
				}
			}
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//指派關卡位置
	void AssignStagePos()
	{
		if(nowChapter < 0)
			return;
		
		for(int i=0; i<dungeonInfoList.Count; ++i)
		{
			if(dungeonInfoList[i].dbfData.iGroupID <= 0)
				continue;
			
			if(dungeonInfoList[i].dbfData.iGroupID > nowSlotChapter.NodeList.Count)
			{
				UnityDebugger.Debugger.LogWarning("請企劃檢查第"+dungeonInfoList[i].dbfData.iGroup+"章的DungeonGUID"+dungeonInfoList[i].dbfData.GUID+"Index"+dungeonInfoList[i].dbfData.iGroupID);
				continue;
			}
			
			Transform dungeonPos = nowSlotChapter.NodeList[(dungeonInfoList[i].dbfData.iGroupID-1)];
			if (dungeonPos == null)
			{
				dungeonInfoList[i].btnInfo.gameObject.SetActive(false);
				continue;
			}
			dungeonInfoList[i].btnInfo.transform.position = dungeonPos.position;
		}
	}

#region 玩家操作
	//-----------------------------------------------------------------------------------------------------
	//下一章
	public void IncreseChapter()
	{
		//鎖定中
		if(m_CDState.fLockTime != ChooseDungeonState.fCloseLockTime)
			return;

		int iChapter = nowChapter + 1;
		
		if(iChapter > iMaxUnlockChapter)
			return;

		nowChapter += 1;
		//滑動前關閉其他物件
		for(int j=0; j<dungeonInfoList.Count; ++j)
			dungeonInfoList[j].btnInfo.gameObject.SetActive(false);
		//關閉紅圈標示
		spriteHighlight.gameObject.SetActive(false);
		//設定滑動的目標章節
		changedSlotChapter = CreateSlotChpater();
		//開始滑動
		if(changedSlotChapter!=null & nowSlotChapter!=null)
			SlideEffect(changedSlotChapter.textChapterMap,nowSlotChapter.textChapterMap, false);
	}
	//-----------------------------------------------------------------------------------------------------
	//前一章
	public void DecreaseChapter()
	{
		//鎖定中
		if(m_CDState.fLockTime != ChooseDungeonState.fCloseLockTime)
			return;

		int iChapter = nowChapter - 1;
		
		if(iChapter < 1)
			return;

		nowChapter -= 1;
		//滑動前關閉其他物件
		for(int j=0; j<dungeonInfoList.Count; ++j)
			dungeonInfoList[j].btnInfo.gameObject.SetActive(false);
		//關閉紅圈標示
		spriteHighlight.gameObject.SetActive(false);
		
		//設定滑動的目標章節
		changedSlotChapter = CreateSlotChpater();
		//開始滑動
		if(changedSlotChapter!=null & nowSlotChapter!=null)
			SlideEffect(changedSlotChapter.textChapterMap,nowSlotChapter.textChapterMap, true);
	}
	//-----------------------------------------------------------------------------------------------------
	//轉換模式
	public void SwitchChapterMode(ENUM_GroupRank_Type grType)
	{
		//鎖定中
		if(m_CDState.fLockTime != ChooseDungeonState.fCloseLockTime)
			return;

		//滑動前關閉其他物件
		for(int j=0; j<dungeonInfoList.Count; ++j)
			dungeonInfoList[j].btnInfo.gameObject.SetActive(false);
		
		//關閉紅圈標示
		spriteHighlight.gameObject.SetActive(false);
		
		//開始轉換
		if(nowSlotChapter != null)
			SwitchEffect(grType);
	}
#endregion 玩家操作

#region 效果
	//-----------------------------------------------------------------------------------------------------
	//(一般/困難)切換效果
	private void SwitchEffect(ENUM_GroupRank_Type grType)
	{
		nowSlotChapter.gLineContainer.SetActive(false);

		UpdateChapterUI();
		
		switch(grType)
		{
		case ENUM_GroupRank_Type.ENUM_Rank_Normal:	CDChapterBase.m_texHardMask.gameObject.SetActive(false);	break;
		case ENUM_GroupRank_Type.ENUM_Rank_Hard:	CDChapterBase.m_texHardMask.gameObject.SetActive(true);		break;
		}
		
		//
		ResetTweenEffect(CDChapterBase.m_LeftCloud);
		ResetTweenEffect(CDChapterBase.m_RightCloud);
		ResetTweenEffect(CDChapterBase.m_ChapterNameContainer);
		ResetTweenEffect(CDChapterBase.m_BottomZoneContainer);
	}
	//-----------------------------------------------------------------------------------------------------
	//滑動效果
	private void SlideEffect(UITexture fin,UITexture fout,bool bLeft)
	{		
		nowSlotChapter.gLineContainer.SetActive(false);
		changedSlotChapter.gLineContainer.SetActive(false);
		//重設depth
		ResetObjectAllDepth (fin, m_iNowChapterDepth);
		ResetObjectAllDepth (fout, m_iChangedChapterDepth);
		fin.alpha = 1;

		//鎖定
		m_CDState.fLockTime = fEffectTime;

		//移出效果
		TweenPosition movePos = fout.gameObject.AddComponent<TweenPosition>();
		movePos.from = Vector3.zero;
		if(bLeft == true)
			movePos.to = new Vector3((float)fout.width,0,0);
		else
			movePos.to = new Vector3(-((float)fout.width),0,0);
		movePos.duration = fEffectTime - 0.2f;
		//移入效果
		movePos = fin.gameObject.AddComponent<TweenPosition>();
		if(bLeft == true)
			movePos.from = new Vector3(-((float)fin.width),0,0);
		else
			movePos.from = new Vector3((float)fin.width,0,0);
		movePos.to = Vector3.zero;
		movePos.duration = fEffectTime -0.2f;

		movePos.AddOnFinished(SettingAfterChapterChange);
		movePos.AddOnFinished(UpdateChapterUI);
		//
		ResetTweenEffect(CDChapterBase.m_LeftCloud);
		ResetTweenEffect(CDChapterBase.m_RightCloud);
		ResetTweenEffect(CDChapterBase.m_ChapterNameContainer);
		ResetTweenEffect(CDChapterBase.m_BottomZoneContainer);
	}
	//-----------------------------------------------------------------------------------------------------
	//重新設定效果
	private void ResetTweenEffect(GameObject gb)
	{
		gb.SetActive(true);
		TweenPosition tp = gb.GetComponent<TweenPosition>();
		if(tp!=null)
		{
			//鎖定
			m_CDState.fLockTime = fEffectTime;
			tp.duration = fEffectTime -0.2f;
			tp.ResetToBeginning();
			tp.PlayForward();
		}
		TweenAlpha ta = gb.GetComponent<TweenAlpha>();
		if(ta!=null)
		{
			//鎖定
			m_CDState.fLockTime = fEffectTime;
			ta.duration = fEffectTime -0.2f;
			ta.ResetToBeginning();
			ta.PlayForward();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//移除效果
	void RemoveTweenEffect()
	{
		if(nowSlotChapter!=null && nowSlotChapter.textChapterMap.GetComponent<TweenPosition>()!=null)
			Destroy(nowSlotChapter.GetComponent<TweenPosition>());
		if(changedSlotChapter!=null && changedSlotChapter.textChapterMap.GetComponent<TweenPosition>()!=null)
			Destroy(changedSlotChapter.GetComponent<TweenPosition>());
	}
	//-----------------------------------------------------------------------------------------------------
	//Reset Texture跟裡面Sprite的層級
	private void ResetObjectAllDepth(UITexture tex,int idepth)
	{
		tex.depth = idepth;
		UISprite[] LocSprites  = tex.GetComponentsInChildren<UISprite>();
		for(int i=0;i<LocSprites.Length;++i)
		{
			LocSprites[i].depth = idepth+2;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void OnSwipe(SwipeGesture gesture ) 
	{
		//鎖定中
		if(m_CDState.fLockTime != ChooseDungeonState.fCloseLockTime)
			return;

		if(CDStageBase.IsActive() == true)
			return;

		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		UnityDebugger.Debugger.Log( direction );

		if(m_CDState != null)
			m_CDState.bEnterSecondUI = false;

		switch(direction)
		{
		case FingerGestures.SwipeDirection.Right:
			DecreaseChapter();
			break;
		case FingerGestures.SwipeDirection.Left:
			IncreseChapter();
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//章節切換後移除防點擊功能、Tween效果
	private void SettingAfterChapterChange()
	{
		RemoveTweenEffect();
		//移除之前章節的Slot
		DestroyImmediate(nowSlotChapter.gameObject);
		//儲存目前章節
		nowSlotChapter = changedSlotChapter;
		changedSlotChapter = null;
	}
	//-----------------------------------------------------------------------------------------------------
	//開啟節_介面時重啟Tween效果
	public void ResetStageTween()
	{
		int i = 0;
		for(i=0; i<CDStageBase.m_wgStageTweens.Length; ++i)
			ResetTweenEffect(CDStageBase.m_wgStageTweens[i].gameObject);
	}
#endregion 效果

	//-----------------------------------------------------------------------------------------------------
	//設定星數獎勵
	public void SetStarRewardInfo()
	{
		GetNowChapter();
		SetBtnStarRewardInfo();

		//安全檢查
		if(m_iRewardMaxStars <= 0)
		{
			UnityDebugger.Debugger.LogError("m_iRewardMaxStars value is less than zero!!");
			return;
		}

		CDChapterBase.m_btnReward1.gameObject.SetActive(false);
		CDChapterBase.m_btnReward2.gameObject.SetActive(false);
		CDChapterBase.m_btnReward3.gameObject.SetActive(false);
		CDChapterBase.m_btnReward4.gameObject.SetActive(false);
		CDChapterBase.m_okReward1.gameObject.SetActive(false);
		CDChapterBase.m_okReward2.gameObject.SetActive(false);
		CDChapterBase.m_okReward3.gameObject.SetActive(false);
		CDChapterBase.m_okReward4.gameObject.SetActive(false);
		CDChapterBase.m_getReward1.gameObject.SetActive(false);
		CDChapterBase.m_getReward2.gameObject.SetActive(false);
		CDChapterBase.m_getReward3.gameObject.SetActive(false);
		CDChapterBase.m_getReward4.gameObject.SetActive(false);
			
		if(sChapterItem != null)
		{
			//星數顯示
			CDChapterBase.m_lbrewardStarCount.text = m_iChapterStars.ToString() + "/" + m_iRewardMaxStars.ToString();
			//進度條顯示
			CDChapterBase.m_sliderStars.value = (float)m_iChapterStars / (float)m_iRewardMaxStars;
			float fForegroundWidth = CDChapterBase.m_sliderStars.foregroundWidget.localSize.x;
			//設定各獎勵按鈕相對位置
			CDChapterBase.m_btnReward0.transform.localPosition = Vector3.right * fForegroundWidth * (0.0f / (float)m_iRewardMaxStars);
			CDChapterBase.m_btnReward1.transform.localPosition = Vector3.right * fForegroundWidth * ((float)sChapterItem.iItemFactor[0] / (float)m_iRewardMaxStars);
			CDChapterBase.m_btnReward2.transform.localPosition = Vector3.right * fForegroundWidth * ((float)sChapterItem.iItemFactor[1] / (float)m_iRewardMaxStars);
			CDChapterBase.m_btnReward3.transform.localPosition = Vector3.right * fForegroundWidth * ((float)sChapterItem.iItemFactor[2] / (float)m_iRewardMaxStars);
			CDChapterBase.m_btnReward4.transform.localPosition = Vector3.right * fForegroundWidth * ((float)sChapterItem.iItemFactor[3] / (float)m_iRewardMaxStars);

			//設定各獎勵按鈕圖示
			for(int i=0; i<GameDefine.DUNGEON_CHAPTEFACTOR_MAX; ++i)
			{
				UIButton btn	= null;
				UIWidget ok		= null;
				UIWidget get	= null;
				switch(i)
				{
				case 0:	
					btn	= CDChapterBase.m_btnReward1;
					ok	= CDChapterBase.m_okReward1;
					get	= CDChapterBase.m_getReward1;
					break;
				case 1:
					btn	= CDChapterBase.m_btnReward2;
					ok	= CDChapterBase.m_okReward2;
					get	= CDChapterBase.m_getReward2;
					break;
				case 2:
					btn	= CDChapterBase.m_btnReward3;
					ok	= CDChapterBase.m_okReward3;
					get	= CDChapterBase.m_getReward3;
					break;
				case 3:
					btn	= CDChapterBase.m_btnReward4;
					ok	= CDChapterBase.m_okReward4;
					get	= CDChapterBase.m_getReward4;
					break;
				}

				if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckChapterItemUnlockOrNot(sChapterItem.GUID, i) == true)
					get.gameObject.SetActive(true);
				else if(m_iChapterStars >= sChapterItem.iItemFactor[i])
					ok.gameObject.SetActive(true);
				
				btn.gameObject.SetActive(true);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//取得現在章節獎勵資料
	private void GetNowChapter()
	{
		sChapterItem = null;
		GameDataDB.ChapterItemDB.ResetByOrder();
		for(int i=0; i<GameDataDB.ChapterItemDB.GetDataSize(); ++i)
		{
			S_ChapterItem_Tmp temp = GameDataDB.ChapterItemDB.GetDataByOrder();
			if(temp.iGroup==nowChapter && temp.iGroupRank==nowGroupType)
			{
				sChapterItem = temp;
				break;
			}
		}

		//計算本章最大獎勵星數
		if(sChapterItem != null)
		{
			m_iRewardMaxStars = sChapterItem.iItemFactor[sChapterItem.iItemFactor.Length-1];
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定星數獎勵按鈕
	public void SetBtnStarRewardInfo()
	{
		CDChapterBase.m_lbReward1Star.text	= "0";
		CDChapterBase.m_lbReward2Star.text	= "0";
		CDChapterBase.m_lbReward3Star.text	= "0";
		CDChapterBase.m_lbReward4Star.text	= "0";

		if(sChapterItem == null)
			return;

		//設定各星等獎勵數
		for(int i=0; i<sChapterItem.iItemFactor.Length; ++i)
		{
			m_Strb.Remove(0, m_Strb.Length);
			m_Strb.AppendFormat("{0}", sChapterItem.iItemFactor[i]);

			switch(i)
			{
			case 0:	CDChapterBase.m_lbReward1Star.text = m_Strb.ToString();	break;
			case 1:	CDChapterBase.m_lbReward2Star.text = m_Strb.ToString();	break;
			case 2:	CDChapterBase.m_lbReward3Star.text = m_Strb.ToString();	break;
			case 3:	CDChapterBase.m_lbReward4Star.text = m_Strb.ToString();	break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定獎勵條件及星星
	public void SetCSFactor()
	{
		if(sChapterItem == null)
			return;

		int index = 0;
		switch(iStarRewardIndex)
		{
		case 0:	index = 0;	break;
		case 1:	index = 1;	break;
		case 2:	index = 2;	break;
		case 3:	index = 3;	break;
		default:			return;
		}
		m_Strb.Remove(0, m_Strb.Length);
		m_Strb.AppendFormat(GameDataDB.GetString(2171), sChapterItem.iItemFactor[index]);	//"累計 {0} 星獎勵"
		CDRewardBase.m_lbCSStep.text = m_Strb.ToString();
		
		if(m_iChapterStars < sChapterItem.iItemFactor[index])
			CDRewardBase.m_lbCSStep.color = Color.gray;
		else
			CDRewardBase.m_lbCSStep.color = Color.green;
	}
	//-----------------------------------------------------------------------------------------------------
	//設定獎勵內容
	public void SetCSRewards()
	{
		if(sChapterItem == null)
			return;
		
		int index = 0;
		switch(iStarRewardIndex)
		{
		case 0:	index = 0;	break;
		case 1:	index = 3;	break;
		case 2:	index = 6;	break;
		case 3:	index = 9;	break;
		default:			return;
		}

		int iCount = CDRewardBase.m_CSRewardsArray.Length;
		for(int i=0; i<iCount; ++i)
		{
			if(sChapterItem.questReward[index+i].iRewardID <= 0)
				CDRewardBase.m_CSRewardsArray[i].gameObject.SetActive(false);
			else
			{
				CDRewardBase.m_CSRewardsArray[i].gameObject.SetActive(true);
				CDRewardBase.m_CSRewardsArray[i].SetSlotWithCount(sChapterItem.questReward[index+i].iRewardID, 
				                                                  sChapterItem.questReward[index+i].iRewardCount,
				                                                  false												);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定獎勵領取按鈕
	public void SetChapterStarGetBtn()
	{
		if(sChapterItem == null)
			return;
		
		int index = 0;
		switch(iStarRewardIndex)
		{
		case 0:	index = 0;	break;
		case 1:	index = 1;	break;
		case 2:	index = 2;	break;
		case 3:	index = 3;	break;
		default:			return;
		}

		if(m_iChapterStars < sChapterItem.iItemFactor[index])
		{
			Utility.ChangeAtlasSprite(CDRewardBase.m_spBtnCSGet, 101);
			SetButtonSprite4Type(CDRewardBase.m_btnCSGet, CDRewardBase.m_spBtnCSGet);
			CDRewardBase.m_btnCSGet.isEnabled = false;
			CDRewardBase.m_lbBtnGet.text = GameDataDB.GetString(2170);	//"未達成"
		}
		else if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckChapterItemUnlockOrNot(sChapterItem.GUID, index))
		{
			Utility.ChangeAtlasSprite(CDRewardBase.m_spBtnCSGet, 102);
			SetButtonSprite4Type(CDRewardBase.m_btnCSGet, CDRewardBase.m_spBtnCSGet);
			CDRewardBase.m_btnCSGet.isEnabled = true;
			CDRewardBase.m_lbBtnGet.text = GameDataDB.GetString(2169);	//"已領取"
		}
		else
		{
			Utility.ChangeAtlasSprite(CDRewardBase.m_spBtnCSGet, 101);
			SetButtonSprite4Type(CDRewardBase.m_btnCSGet, CDRewardBase.m_spBtnCSGet);
			CDRewardBase.m_btnCSGet.isEnabled = true;
			CDRewardBase.m_lbBtnGet.text = GameDataDB.GetString(2168);	//"領取"
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void SetButtonSprite4Type(UIButton btn, UISprite sp)
	{
		btn.normalSprite	= sp.spriteName;
		btn.hoverSprite		= sp.spriteName;
		btn.disabledSprite	= sp.spriteName;
		btn.pressedSprite	= sp.spriteName;
	}
}

