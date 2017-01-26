using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;
using System.Collections;
using System.Text;

public class DungeonTowerInfo : DungeonInfo
{
	public void SetData(S_Dungeon_Tmp dbf)
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
		//
		lbLVLimit.text = string.Format(GameDataDB.GetString(1595), dbf.iLevelLimit.ToString());
		//設定鎖圖
		switch(dbf.DungeonIconSize)
		{
		/*case ENUM_DungeonIconSize.ENUM_DungeonIconSize_Small:
			spriteIcon.SetDimensions(100 , 100);
			break;
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_Medium:
			spriteIcon.SetDimensions(120 , 120);
			break;*/
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_big: //BOSS關
			Utility.ChangeAtlasSprite(spritelock,iBossLock);
			//spriteIcon.SetDimensions(190 , 190);
			//spritelock.SetDimensions(140 , 140);
			break;
		}
		Utility.ChangeAtlasSprite(spriteIcon,dbf.iDungeonIcon);
		spriteIcon.color = Color.white;
		//設定是否反灰(依挑戰可否狀態決定
		RoleStageData rsData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbf.GUID);
		if(rsData !=null)
		{
			if(rsData.iChallengeCount == dbf.iPlayCount)
			{
				spriteIcon.color = Color.cyan;
			}
		}

		bDark = false;
		
		//檢查關卡解鎖條件, 等級不足= 鎖住+不防點擊、任務未解= 鎖住+防點擊
		S_QuestData_Tmp questDBF = null;
		
		//判斷任務ID是否到達
		if(dbf.iUnlockQuestID > 0)
		{
			questDBF = GameDataDB.QuestDB.GetData(dbf.iUnlockQuestID);
			//判斷任務是否已達解鎖關卡
			if(questDBF!=null && questDBF.iFlag>=0)
			{
				if (ARPGApplication.instance.m_RoleSystem.sBaseQuestFlag.Get(questDBF.iFlag))
				{
					bDark = false;
				}
				else
				{
					bDark = true;
					S_Dungeon_Tmp DungeonTmp = null;
					GameDataDB.DungeonDB.ResetByOrder();
					for(int i=0;i<GameDataDB.DungeonDB.GetDataSize();++i)
					{
						DungeonTmp = GameDataDB.DungeonDB.GetDataByOrder();
						if(DungeonTmp.DungeonType == ENUM_DungeonType.ENUM_DungeonType_Normal || DungeonTmp.DungeonType == ENUM_DungeonType.ENUM_DungeonType_waveBattle )
						{
							if(DungeonTmp.iStartQuestID == questDBF.GUID || DungeonTmp.iEndQuestID == questDBF.GUID)
								break;
						}
					}
					string chapterName = "";
					if(DungeonTmp!=null)
						chapterName = GameDataDB.GetString(DungeonTmp.iGroupName);
					string MsgStr = string.Format(GameDataDB.GetString(548),chapterName,GameDataDB.GetString(questDBF.iName));
					spritelock.gameObject.GetComponent<UIButton>().userData = MsgStr;
				}
			}
		}
		//判斷上一關是否已經打過
		if(dbf.iUnlockDungeonID > 0 && bDark == false)
		{
			//取得上一關關卡資訊
			RoleStageData PreRSData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbf.iUnlockDungeonID);
			if(PreRSData == null || PreRSData.iStar == 0)
			{
				bDark = true;
				string MsgStr = GameDataDB.GetString(2107);					//"關卡未解鎖"
				spritelock.gameObject.GetComponent<UIButton>().userData = MsgStr;
			}
		}

		if(bDark == false)
		{

			//判斷等級是否足夠
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel()<dbf.iLevelLimit)
			{
				spritelock.gameObject.SetActive(true);
				if (spriteLockCollider)
					spriteLockCollider.enabled = false;
				lbLVLimit.gameObject.SetActive(true);
				return;
			}

			//顯示星級
			//取得關卡星等
			//RoleStageData rsData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbf.GUID);
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
		else
		{
			if(spriteLockCollider)
				spriteLockCollider.enabled = true;
				//
			spritelock.gameObject.SetActive(true);
			UIEventListener.Get(spritelock.gameObject).onClick += AlertNotYetEvent;
			btnInfo.enabled=false;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	protected void AlertNotYetEvent(GameObject gb)
	{
		UIButton uBtn = gb.GetComponent<UIButton>();
		if(uBtn == null)
			return;

		string str  = (string)uBtn.userData;
		//string str = GameDataDB.GetString(2107);					//"關卡未解鎖"
		ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
	}
	//-----------------------------------------------------------------------------------------------------
}

public class UI_TowerTrail : NGUIChildGUI 
{
	public UIPanel			panelBase				= null;
	public UITexture		textBottomColldier		= null; //全螢幕Collider
	//章節名稱
	public UILabel			lbChapterName			= null; //章節名稱
	//地圖關卡
	[HideInInspector]
	public Slot_DungeonChapter	m_NowSlotChpater	= null;	//目前章節Slot
	[HideInInspector]
	public Slot_DungeonChapter	m_changedSlotChapter = null;//要切換之目標章節Slot
	[HideInInspector]
	public List<DungeonTowerInfo>	dungeonInfoList		= new List<DungeonTowerInfo>();	//副本相關資訊陣列
	public UIButton			btnDungeonPrefab		= null;				//副本的Prefab
	public UISprite			spriteHighlight		= null;				//選取關卡指引圈
	//上下切換鈕
	public UIWidget			wgMap					= null; //放副本按鈕的parent
	public UIWidget			wgChooseChapter			= null; //上下切換相關
	public UIButton			btnDownChoose			= null; //向下切換
	public UIButton			btnUpChoose				= null; //向上切換
	//底下各關卡資訊列
	public UIPanel			panelBottomStageInfo	= null; //各章節關卡相關
	public UILabel			lbStageName				= null; //關卡名稱
	public UIWidget			containerNormal			= null; //關卡名稱底圖一般
	public UIWidget			wgTreasuresContainer	= null; //關卡獎勵集合
	public GameObject[]		gTreasuresPosArray		= new GameObject[3]; //關卡獎勵圖
	[HideInInspector]
	public Slot_Item[]		slotTreasuresArray		= new Slot_Item[3]; //獎勵按鈕
	public UILabel			lbStageReward			= null; //關卡獎勵
	public UILabel			lbResetTimes			= null; //重置次數
	public UILabel			lbChallegeTimes			= null; //挑戰次數
	public UIButton			btnSpeedMopUp			= null; //一鍵掃蕩
	public UILabel			lbSpeedMopUp			= null; //一鍵掃蕩 字樣
	public UIButton			btnSpeedReset			= null; //一鍵重置
	public UILabel			lbSpeedReset			= null; //一鍵重置 字樣
	public UIButton			btnStartStage			= null; //開始關卡
	public UILabel			lbStartStage			= null; //開始關卡字樣
	public UILabel			lb3StarsTime			= null; //關卡三星時間
	public UILabel			lb2StarsTime			= null; //關卡二星時間
	//
	public SellBuffInfo 	BuyBuffPrefab			= null; //Buff Prefab
	public UIWidget			BuyBuff2Loc				= null; //Buff2位置
	public UIWidget			BuyBuff3Loc				= null; //Buff3位置
	//一鍵掃蕩與買Buff頁面相關
	[Header("SpeedModUp BuyBuff")]
	public UIWidget			SpeedModUpContainer		= null; //一鍵掃蕩集合
	public UILabel			lbSpeedModUpTitle		= null; //一鍵掃蕩標題
	public UIButton			btnCloseSpeedModUp		= null; //關閉一鍵掃蕩
	public UIButton			btnOKSpeedModUp			= null; //確認一鍵掃蕩
	public UILabel			lbOKSpeedModUp			= null; //確認一鍵掃蕩文字
	public UIScrollView		svFloorList				= null; //通天塔樓層列表
	public UIGrid			gridFloorList			= null; //Grid
	public TowerSelectData	ModUpFloorPrefab		= null; //一鍵掃蕩的樓層prefab
	public UILabel			lbSpendAll				= null; //一共花費多少的敘述
	public UILabel			lbAllValue				= null; //一共花費多少
	public UIWidget			BuyBuffPageContainer	= null; //買Buff頁
	public UILabel			lbBuyBuffTitle			= null; //買Buff標題
	public UIButton			btnCloseBuyBuff			= null; //關閉買Buff
	public UIButton			btnOKBuyBuff			= null; //確認買Buff
	public UILabel			lbOKBuyBuff				= null; //確認買Buff字樣
	public TowerSelectData[]BuffSelects				= new TowerSelectData[3];
	public UILabel			lbBuffSpendAll			= null; //Buff一共花費多少的敘述
	public UILabel			lbBuffAllValue			= null; //Buff一共花費多少
	public UILabel			lbRewardEx				= null; //額外獎勵
	//
	[System.NonSerialized]
	public List<SellBuffInfo>		SellBuffBtns		= new List<SellBuffInfo>();
	[System.NonSerialized]
	public List<int>				SellBuffCosts		= new List<int>();
	[System.NonSerialized]
	public List<TowerSelectData>	ModUpFloors			= new List<TowerSelectData>();
	//
	//
	[HideInInspector]
	public int				iChapter			= 1;
	[HideInInspector]
	public int 				iStoreChapter		= -1;
	[HideInInspector]
	public int 				iStoreDungeonNum	= -1;
	[HideInInspector]
	public bool 			bFirstIn			= true;
	[HideInInspector]
	public int 				iUnlockChapter		= 1;		//紀錄己解鎖章數
	
	public GameObject		TreasureContainer	= null;
	
	private List<S_Dungeon_Tmp> dbfDungeonList	= new List<S_Dungeon_Tmp>(); //同章節內的DBF副本資料
	//
	public GameObject		LeftCloud			= null;
	public GameObject		RightCloud			= null;
	public GameObject		ChapterNameContainer= null;
	//
	public UITexture		TowerMainBody		= null;		//通天塔主體
	public UITexture		TowerBottom			= null;		//通天塔底部

	//-------------------------------------執行用變數----------------------------------------------------
	private TowerTrailState m_TsState		= null;
	private bool 			bSwitchDone			= false;	
	public float			fChestUpdateTime	= 0.8f;
	public float			fChangeChapterTime	= 0.1f;
	public const int		iNowChapterDepth	= 20;
	public const int		iChangedChapterDepth = 10;
	private const string	m_SlotName			="Slot_Item";
	private const string	m_SlotChpaterName	="Slot_DungeonChapter";
	private const string	m_StagePosName		="StagePosition";
	private StringBuilder 	m_Strb 				= new StringBuilder();
	[HideInInspector]
	public S_Dungeon_Tmp	selectedDBF;
	[HideInInspector]
	public S_Item_Tmp[]		m_chestItems 		= new S_Item_Tmp[3];
	//通天塔
	private const string	m_SlotTowerName		="Slot_Tower1";
	//-------------------------------------關卡總星數獎勵----------------------------------------------------
	public int				iChapterStars			= 0;				//章節星星數
	public UISprite			spCollider				= null;
	public UISprite[]		spriteStar				= new UISprite[3]; 	//星級評價
	public UIButton			btnChapterStars			= null;				//
	public UIButton			btnCSQuit				= null;
	public UILabel			lbChapterStarsCount		= null;				//已獲得星數
	public UISprite			spCSBG					= null;
	public UILabel			lbCSTitle				= null;
	public GameObject		gStarReward1			= null;				//星數獎勵1容器
	public GameObject		gStarReward2			= null;				//星數獎勵2容器
	public GameObject		gStarReward3			= null;				//星數獎勵3容器
	public UILabel[]		lbCSStepArray			= new UILabel[3];	//條件
	public Slot_Item[]		CSRewardsArray			= new Slot_Item[9];	//獎勵道具
	public UILabel[]		lbBtnGetArray			= new UILabel[3];	//取得按鈕LABEL
	public UISprite[]		spBtnCSGetArray			= new UISprite[3];
	public UIButton			btnCSGet_1				= null;				//取得按鈕1
	public UIButton			btnCSGet_2				= null;				//取得按鈕2
	public UIButton			btnCSGet_3				= null;				//取得按鈕3
	[HideInInspector]
	public List<UIButton>	btnCSGetPool			= new List<UIButton>();
	[HideInInspector]
	public S_ChapterItem_Tmp sChapterItem			= null;

	[Header("NewGuide")]
	//------------------------------------指引教學相關元件-----------------------------------------------
	public UIPanel 			panelGuide 				= null;	//指引教學集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen			= null; //全螢幕按鈕
	public UILabel 			lbGuildeIntroduce 		= null;
	public UISprite 		spGuideBuyBonus 		= null;
	public UILabel 			lbGuideBuyBonus			= null;
	public UISprite 		spGuideMopUp 			= null;
	public UILabel 			lbGuideMopUp 			= null;
	public UISprite 		spGuideStartTower 		= null;
	public UILabel 			lbGuideStartTower 		= null;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_TowerTrail";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_TowerTrail() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		m_TsState = (TowerTrailState)ARPGApplication.instance.GetGameStateByName(GameDefine.TOWERTRAIL_STATE);
		//預設關閉的UI
		ChapterNameContainer.SetActive(false);
		LeftCloud.SetActive(false);
		RightCloud.SetActive(false);
		panelBottomStageInfo.gameObject.SetActive(false);
		spCSBG.gameObject.SetActive(false);
		CreateTreasuresSlot();
		CreateBuffBtnSlots();
		CreateModUpFloorSlots();
		SetCSGetBtnPool();
		//設定選單中BUFF的名稱與價格
		SetBuffSelectData();
		lbSpeedMopUp.text			= GameDataDB.GetString(450); //一鍵掃蕩
		lbSpeedReset.text			= GameDataDB.GetString(448); //一鍵重置
		lbSpeedModUpTitle.text		= GameDataDB.GetString(450); //一鍵掃蕩
		lbRewardEx.text				= GameDataDB.GetString(458); //額外獎勵
		lbOKBuyBuff.text			= GameDataDB.GetString(299); //確定
		lbOKSpeedModUp.text			= GameDataDB.GetString(299); //確定
	}
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		//隱藏原始模型
		btnDungeonPrefab.gameObject.SetActive(false);
		lbStartStage.text 		= GameDataDB.GetString(2102); //"挑戰"
		lbStageReward.text		= GameDataDB.GetString(2101); //"關卡獎勵"
		
		lbCSTitle.text			= "副本星數獎勵";
		lbCSStepArray[0].text		= "累計 XX 星獎勵";
		lbCSStepArray[1].text		= "累計 XX 星獎勵";
		lbCSStepArray[2].text		= "累計 XX 星獎勵";
		lbBtnGetArray[0].text		= "領取";
		lbBtnGetArray[1].text		= "領取";
		lbBtnGetArray[2].text		= "領取";
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		//ResetTweenEffect(LeftCloud);
		//ResetTweenEffect(RightCloud);
		ResetTweenEffect(ChapterNameContainer);
		base.Show();
		StartUpdateUI();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	public Slot_DungeonChapter CreateSlotTower(int chapter)
	{
		if (chapter < 1)
			return null;
		Slot_DungeonChapter go = ResourceManager.Instance.GetGUI(m_SlotTowerName).GetComponent<Slot_DungeonChapter>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_DungeonChapter load prefeb error,path:{0}", "GUI/"+m_SlotTowerName) );
			return null;
		}
		//Slot
		Slot_DungeonChapter newgo= GameObject.Instantiate(go) as Slot_DungeonChapter;
		newgo.transform.parent			= this.transform;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
		newgo.transform.localPosition 	= Vector3.zero;
		//變換貼圖
		switch(chapter)
		{
		case 1:
			Utility.ChangeTexture(newgo.textChapterMap,GameDefine.TOWER_TEXTUREBG1_GUID);
			break;
		case 2:
			Utility.ChangeTexture(newgo.textChapterMap,GameDefine.TOWER_TEXTUREBG2_GUID);
			break;
		case 3:
			Utility.ChangeTexture(newgo.textChapterMap,GameDefine.TOWER_TEXTUREBG3_GUID);
			break;
		case 4:
			Utility.ChangeTexture(newgo.textChapterMap,GameDefine.TOWER_TEXTUREBG4_GUID);
			break;
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
			Utility.ChangeTexture(newgo.textChapterMap,GameDefine.TOWER_TEXTUREBG5_GUID);
			break;
		default:
			Utility.ChangeTexture(newgo.textChapterMap,GameDefine.TOWER_TEXTUREBG1_GUID);
			break;
		}
		newgo.gameObject.SetActive(true);
		return newgo;
	}
	//-----------------------------------------------------------------------------------------------------
	private void SetBuffSelectData()
	{
		//Buff一共花費多少的敘述
		lbBuyBuffTitle.text				= GameDataDB.GetString(466); //額外獎勵
		lbBuffSpendAll.text 			= GameDataDB.GetString(523); //一共花費
		//將所有的buff花費列到list中
		S_ShopPrize_Tmp spTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.TOWER_BUFF_COST1_ID);
		SellBuffCosts.Clear();
		SellBuffCosts.Add(spTmp.GetPrize(1));
		spTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.TOWER_BUFF_COST2_ID);
		SellBuffCosts.Add(spTmp.GetPrize(1));
		spTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.TOWER_BUFF_COST3_ID);
		SellBuffCosts.Add(spTmp.GetPrize(1));
		//
		S_BuffData_Tmp buffTmp = GameDataDB.BuffDataDB.GetData(GameDefine.TOWER_BUYBUFF1_GUID);
		//string BuffName = buffTmp.iName<=0?"":GameDataDB.GetString(buffTmp.iName)+"-";
		string BuffName = "";
		BuffSelects[0].lbContent.text 		= BuffName+GameDataDB.GetString(467); //BUFF1名稱+bonus
		BuffSelects[0].lbValue.text 		= SellBuffCosts[0].ToString();									//BUFF1花費
		buffTmp = GameDataDB.BuffDataDB.GetData(GameDefine.TOWER_BUYBUFF2_GUID);
		BuffName = buffTmp.iName<=0?"":GameDataDB.GetString(buffTmp.iName)+"-";
		BuffSelects[1].lbContent.text 		= BuffName+GameDataDB.GetString(468); //BUFF2名稱+bonus
		BuffSelects[1].lbValue.text 		= SellBuffCosts[1].ToString();									//BUFF2花費
		buffTmp = GameDataDB.BuffDataDB.GetData(GameDefine.TOWER_BUYBUFF3_GUID);
		BuffName = buffTmp.iName<=0?"":GameDataDB.GetString(buffTmp.iName)+"-";
		BuffSelects[2].lbContent.text 		= BuffName+GameDataDB.GetString(469); //BUFF3名稱+bonus
		BuffSelects[2].lbValue.text 		= SellBuffCosts[2].ToString(); 									//BUFF3花費
	}
	//-----------------------------------------------------------------------------------------------------
	private void CollectDBFData()
	{
		if(iChapter <= 0)
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
			if(dbf.iGroup == iChapter && dbf.StageType == ENUM_StageType.ENUM_StageType_Tower)
				dbfDungeonList.Add(dbf);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void CleanDBFDungeonList()
	{
		if (dbfDungeonList.Count > 0)
			dbfDungeonList.Clear();
	}
	//-----------------------------------------------------------------------------------------------------
	//動態增減DungeonInfo物件
	private void GenerateDungeonInfo()
	{
		if(dbfDungeonList.Count > 0 && btnDungeonPrefab!=null)
		{
			int iCount = 0;
			//動態增減DungeonInfo物件
			if (dbfDungeonList.Count >= dungeonInfoList.Count)
			{
				iCount = dbfDungeonList.Count - dungeonInfoList.Count;
				for(int i = 0; i < iCount; ++i)
				{
					GameObject newGO = Instantiate(btnDungeonPrefab.gameObject) as GameObject;
					newGO.transform.parent = wgMap.transform;
					newGO.transform.localScale = Vector3.one;
					newGO.SetActive(true);
					DungeonTowerInfo dgInfo = new DungeonTowerInfo();
					dungeonInfoList.Add(dgInfo);
					dgInfo.assigneGameObject(newGO.transform);
				}
				//將新增的btnDungeonInfo加入點擊功能 
				m_TsState.SetDungeonInfoFunction(true);
			}
			else
			{
				iCount = dungeonInfoList.Count - dbfDungeonList.Count;
				for(int i = 0; i < iCount; ++i)
				{
					DestroyImmediate(dungeonInfoList[dungeonInfoList.Count-1].btnInfo.gameObject);
					dungeonInfoList.Remove(dungeonInfoList[dungeonInfoList.Count-1]);
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//生成顯示獎勵的物件
	private void CreateTreasuresSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		//Slot
		for(int i=0; i < gTreasuresPosArray.Length; ++i)
		{
			gTreasuresPosArray[i].SetActive(false);
			Slot_Item newgo= GameObject.Instantiate(go) as Slot_Item;
			newgo.transform.parent			= wgTreasuresContainer.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition 	= gTreasuresPosArray[i].transform.localPosition;
			
			newgo.name = string.Format("slotItem{0:00}",i);
			newgo.gameObject.SetActive(true);
			slotTreasuresArray[i] = newgo;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//生成購買Buff按鈕的物件
	private void CreateBuffBtnSlots()
	{
		if(BuyBuffPrefab == null)
		{
			UnityDebugger.Debugger.LogError( "BuffPrefab has errors" );
			return;
		}
		SellBuffBtns.Clear();
		//生成外層開啟購買Buff選項
		for(int i=0;i<GameDefine.TOWER_BUYBUFFCOUNT_MAX;++i)
		{
			if(i == 0)
			{
				SellBuffBtns.Add(BuyBuffPrefab);
				continue;
			}
			SellBuffInfo newBuff= GameObject.Instantiate(BuyBuffPrefab) as SellBuffInfo;
			newBuff.transform.parent			= BuyBuff2Loc.transform.parent;
			switch(i)
			{
			case 1:
				newBuff.transform.localPosition		= BuyBuff2Loc.transform.localPosition;
				Utility.ChangeAtlasSprite(newBuff.spriteBuffIcon,129507);
				Utility.ChangeAtlasSprite(newBuff.spriteBG,129506);
				break;
			case 2:
				newBuff.transform.localPosition		= BuyBuff3Loc.transform.localPosition;
				Utility.ChangeAtlasSprite(newBuff.spriteBuffIcon,129509);
				Utility.ChangeAtlasSprite(newBuff.spriteBG,129508);
				break;
			}
			newBuff.transform.parent			= BuyBuff2Loc.transform.parent;
			newBuff.transform.localScale		= Vector3.one;
			newBuff.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);

			SellBuffBtns.Add(newBuff);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//生成掃蕩樓層選擇的物件
	private void CreateModUpFloorSlots()
	{
		if(ModUpFloorPrefab == null)
		{
			UnityDebugger.Debugger.LogError( "ModUpFloorPrefab has errors" );
			return;
		}
		ModUpFloors.Clear();
		//生成掃蕩樓層選擇的物件
		for(int i=0;i<10;++i)
		{
			TowerSelectData TowerFloor 			= GameObject.Instantiate(ModUpFloorPrefab) as TowerSelectData;
			TowerFloor.transform.parent			= gridFloorList.transform;
			TowerFloor.transform.localPosition	= Vector3.zero;
			TowerFloor.transform.localScale		= Vector3.one;
			TowerFloor.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			TowerFloor.name						= string.Format("Floor{0}",i<10?"0"+i.ToString():i.ToString());
			TowerFloor.gameObject.SetActive(false);
			ModUpFloors.Add(TowerFloor);
		}
		ModUpFloorPrefab.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public void StartUpdateUI()
	{
		UpdateUI();
	}
	//-----------------------------------------------------------------------------------------------------
	private void UpdateUI()
	{
		//收集資料
		CollectDBFData();
		GenerateDungeonInfo();
		//設定關卡資料
		for(int i=0; i<dungeonInfoList.Count; ++i)
		{	
			if (dungeonInfoList[i] == null || dbfDungeonList[i] == null)
				break;
			dungeonInfoList[i].SetData(dbfDungeonList[i]);
			//計算獲得的星數
			if(	ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbfDungeonList[i].GUID) != null)
			{
				iChapterStars += ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbfDungeonList[i].GUID).iStar;
			}
		}
		//設定副本星數獎勵資訊
		SetStarRewardInfo();
		//指派關卡位置
		AssignStagePos();

		//第一次進入時指派的關卡功能
		FirstInAssign();
		
		//設定章節dbf
		S_Dungeon_Tmp chapterDBF = dungeonInfoList[0].dbfData;
		//設定章節名稱
		lbChapterName.text = GameDataDB.GetString( chapterDBF.iGroupName );
		if (panelBottomStageInfo.gameObject.activeSelf == false)
			panelBottomStageInfo.gameObject.SetActive(true);
		//設定上下按鈕是否顯示
		if(iChapter == 1)
		{
			if(iChapter == iUnlockChapter)
			{
				btnDownChoose.gameObject.SetActive(false);
				btnUpChoose.gameObject.SetActive(false);
			}
			else
			{
				btnDownChoose.gameObject.SetActive(false);
				btnUpChoose.gameObject.SetActive(true);
			}
		}
		else if(iChapter == iUnlockChapter)
		{
			btnDownChoose.gameObject.SetActive(true);
			btnUpChoose.gameObject.SetActive(false);
		}
		else
		{
			btnDownChoose.gameObject.SetActive(true);
			btnUpChoose.gameObject.SetActive(true);
		}
		//設定是否顯示通天塔底座圖
		TowerBottom.gameObject.SetActive(iChapter==1);
	}
	//-----------------------------------------------------------------------------------------------------
	//第一次進入時指派的關卡功能
	private void FirstInAssign()
	{
		S_Dungeon_Tmp dgTmp = null;

		dgTmp = ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_TowerStage;
	
		if (dgTmp != null && dgTmp.iGroup == iChapter)
		{
			for(int i = 0; i < dungeonInfoList.Count ; ++i)
			{
				//找出上次玩家所選之關卡
				if (dungeonInfoList[i].dbfData == dgTmp)
					m_TsState.startChooseDungeonClick(dungeonInfoList[i],i);
			}
		}
		else
			SelectFinalStage();
	}
	//-----------------------------------------------------------------------------------------------------
	//自動選擇所處章節之最後解鎖的一關
	private void SelectFinalStage()
	{
		int index = -1;
		//找出最新進度之關卡
		for(int i = 0; i < dungeonInfoList.Count ; ++i)
		{
			//取得自己的關卡資訊
			RoleStageData CurRSData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dungeonInfoList[i].dbfData.GUID);
			if (CurRSData == null)
			{
				if (dungeonInfoList[i].dbfData.StageType == ENUM_StageType.ENUM_StageType_Tower)
				{
					if(dungeonInfoList[i].bDark == false)
					{
						index = i;
						break;
					}
					else
					{
						if(i>0)
						{
							index = i-1;
							break;
						}
					}
				}
			}
			else if(CurRSData.iStar == 0)
			{
				if (dungeonInfoList[i].dbfData.StageType == ENUM_StageType.ENUM_StageType_Tower && dungeonInfoList[i].bDark == false)
				{
					index = i;
					break;
				}
			}
		}
		if(index>=0)
		{
			m_TsState.startChooseDungeonClick(dungeonInfoList[index],index);
			ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_TowerTheLatestStage = dungeonInfoList[index].dbfData;
		}
		else
			m_TsState.startChooseDungeonClick(dungeonInfoList[dungeonInfoList.Count-1],dungeonInfoList.Count-1);
	}
	//-----------------------------------------------------------------------------------------------------
	//下一章
	public void IncreseChapter()
	{
		if(m_TsState.m_bswitchSlide == true)
		{
			++iChapter;
			
			if(iChapter>iUnlockChapter)
			{
				--iChapter;
				return;
			}
			//滑動前關閉其他物件
			for(int j=0;j<dungeonInfoList.Count;++j)
				dungeonInfoList[j].btnInfo.gameObject.SetActive(false);
			spriteHighlight.gameObject.SetActive(false);
			//設定滑動的目標章節
			m_changedSlotChapter = CreateSlotTower(iChapter);
			//開始滑動
			if (m_changedSlotChapter != null & m_NowSlotChpater != null)
				SlideEffect(m_changedSlotChapter.textChapterMap,m_NowSlotChpater.textChapterMap,false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//前一章
	public void DecreaseChapter()
	{
		if(m_TsState.m_bswitchSlide == true)
		{
			--iChapter;
			
			if(iChapter < 1)
			{
				++iChapter;
				return;
			}
			//滑動前關閉其他物件
			for(int j=0;j<dungeonInfoList.Count;++j)
				dungeonInfoList[j].btnInfo.gameObject.SetActive(false);
			spriteHighlight.gameObject.SetActive(false);
			
			//設定滑動的目標章節
			m_changedSlotChapter = CreateSlotTower(iChapter);
			//開始滑動
			if (m_changedSlotChapter != null & m_NowSlotChpater != null)
				SlideEffect(m_changedSlotChapter.textChapterMap,m_NowSlotChpater.textChapterMap,true);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//指派關卡位置
	void AssignStagePos()
	{
		if(iChapter<0)
			return;

		for(int i=0;i < dungeonInfoList.Count;++i)
		{
			if(dungeonInfoList[i].dbfData.iGroupID <=0)
				continue;
			
			if(dungeonInfoList[i].dbfData.iGroupID >m_NowSlotChpater.NodeList.Count)
			{
				UnityDebugger.Debugger.LogWarning("請企劃檢查第"+dungeonInfoList[i].dbfData.iGroup+"章的DungeonGUID"+dungeonInfoList[i].dbfData.GUID+"Index"+dungeonInfoList[i].dbfData.iGroupID);
				continue;
			}
			
			Transform dungeonPos = m_NowSlotChpater.NodeList[(dungeonInfoList[i].dbfData.iGroupID-1)];
			if (dungeonPos == null)
			{
				dungeonInfoList[i].btnInfo.gameObject.SetActive(false);
				continue;
			}
			dungeonInfoList[i].btnInfo.transform.position = dungeonPos.position;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void SlideEffect(UITexture fin,UITexture fout,bool bDown)
	{
		TowerBottom.gameObject.SetActive(false);
		m_TsState.m_bswitchSlide = false;
		textBottomColldier.gameObject.SetActive(true);
		UnityDebugger.Debugger.Log("章節切換 , 可否切換"+m_TsState.m_bswitchSlide);

		//重設depth
		ResetObjectAllDepth (fin, iNowChapterDepth);
		ResetObjectAllDepth (fout, iChangedChapterDepth);
		fin.alpha = 1;
		//淡出效果
		//FadeOut FadeoutEffect =  fout.gameObject.AddComponent<FadeOut>();
		//FadeoutEffect.duration = 0.5f;
		//移出效果
		TweenPosition movePos = fout.gameObject.AddComponent<TweenPosition>();
		movePos.from = Vector3.zero;
		if(bDown == true)
			movePos.to = new Vector3(0,(float)fout.height,0);
		else
			movePos.to = new Vector3(0,-((float)fout.height),0);
		movePos.duration = 0.5f;
		//移入效果
		movePos = fin.gameObject.AddComponent<TweenPosition>();
		if(bDown == true)
			movePos.from = new Vector3(0,-((float)fin.height),0);
		else
			movePos.from = new Vector3(0,(float)fin.height,0);
		movePos.to = Vector3.zero;
		movePos.duration = 0.5f;
		movePos.AddOnFinished(SettingAfterChapterChange);
		movePos.AddOnFinished(StartUpdateUI);
		//塔移效果
		float fMoveDiection = bDown?-20f:20f;
		float fFinalLength	= TowerMainBody.uvRect.y+fMoveDiection;
		iTween.ValueTo(
			gameObject,
			iTween.Hash(
			"time", 0.5f,
			"from", TowerMainBody.uvRect.y,
			"to", fFinalLength,
			"onupdate",	"updateFromValue"
			)
			);
		//
		//ResetTweenEffect(LeftCloud);
		//ResetTweenEffect(RightCloud);
		ResetTweenEffect(ChapterNameContainer);
	}
	//-----------------------------------------------------------------------------------------------------
	void updateFromValue(float newValue)
	{
		TowerMainBody.uvRect = new Rect(0f,newValue,1f,1f);
	}
	//-----------------------------------------------------------------------------------------------------
	private void ResetTweenEffect(GameObject gb)
	{
		gb.SetActive(true);
		TweenPosition tp = gb.GetComponent<TweenPosition>();
		if(tp!=null)
		{
			tp.ResetToBeginning();
			tp.PlayForward();
		}
		TweenAlpha ta = gb.GetComponent<TweenAlpha>();
		if(ta!=null)
		{
			ta.ResetToBeginning();
			ta.PlayForward();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void RemoveTweenEffect()
	{
		if (m_NowSlotChpater.textChapterMap.GetComponent<TweenPosition>()!=null)
			Destroy(m_NowSlotChpater.GetComponent<TweenPosition>());
		if (m_changedSlotChapter.textChapterMap.GetComponent<TweenPosition>()!=null)
			Destroy(m_NowSlotChpater.GetComponent<TweenPosition>());
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
	void OnSwipe( SwipeGesture gesture ) 
	{
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		UnityDebugger.Debugger.Log( direction );
		
		switch(direction)
		{
		case FingerGestures.SwipeDirection.Up:
			DecreaseChapter();
			break;
		case FingerGestures.SwipeDirection.Down:
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
		DestroyImmediate(m_NowSlotChpater.gameObject);
		//儲存目前章節
		m_NowSlotChpater = m_changedSlotChapter;
		m_changedSlotChapter = null;
		//取消防點擊功能
		textBottomColldier.gameObject.SetActive(false);
		m_TsState.m_bswitchSlide = true;
		UnityDebugger.Debugger.Log("章節切換結束 , 可否切換"+m_TsState.m_bswitchSlide);
	}
	#region 星數獎勵相關
	//-----------------------------------------------------------------------------------------------------
	private void SetCSGetBtnPool()
	{
		btnCSGetPool.Clear();
		btnCSGetPool.Add(btnCSGet_1);
		btnCSGetPool.Add(btnCSGet_2);
		btnCSGetPool.Add(btnCSGet_3);
	}
	//-----------------------------------------------------------------------------------------------------
	//設定星數獎勵按鈕
	public void SetBtnStarRewardInfo()
	{
		m_Strb.Remove(0,m_Strb.Length);
		m_Strb.AppendFormat("{0}/{1}",iChapterStars,(dungeonInfoList.Count * 3));
		lbChapterStarsCount.text = m_Strb.ToString();
	}
	//-----------------------------------------------------------------------------------------------------
	//設定獎勵條件及星星
	public void SetCSFactor(S_ChapterItem_Tmp temp)
	{
		for(int i = 0;i<lbCSStepArray.Length;++i)
		{
			m_Strb.Remove(0,m_Strb.Length);
			m_Strb.AppendFormat("累計 {0} 星獎勵",temp.iItemFactor[i]);
			lbCSStepArray[i].text = m_Strb.ToString();
			
			if(iChapterStars < temp.iItemFactor[i])
			{
				spriteStar[i].gameObject.SetActive(false);
				lbCSStepArray[i].color = Color.gray;
			}
			else
			{
				spriteStar[i].gameObject.SetActive(true);
				lbCSStepArray[i].color = Color.green;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定獎勵內容
	public void SetCSRewards(S_ChapterItem_Tmp temp)
	{
		for(int i = 0;i<9;++i)
		{
			if(temp.questReward[i].iRewardID <= 0)
				CSRewardsArray[i].gameObject.SetActive(false);
			else
			{
				CSRewardsArray[i].gameObject.SetActive(true);
				CSRewardsArray[i].SetSlotWithCount(temp.questReward[i].iRewardID,temp.questReward[i].iRewardCount,false);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定獎勵領取按鈕
	public void SetChapterStarGetBtn(S_ChapterItem_Tmp temp)
	{
		for(int i = 0;i<temp.iFactorFlag.Length;++i)
		{
			if(iChapterStars < temp.iItemFactor[i])
			{
				Utility.ChangeAtlasSprite(spBtnCSGetArray[i],101);
				SetButtonSprite4Type(btnCSGetPool[i],spBtnCSGetArray[i]);
				btnCSGetPool[i].isEnabled = false;
				lbBtnGetArray[i].text = "未達成";
				continue;
			}
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckChapterItemUnlockOrNot(temp.GUID,i))
			{
				Utility.ChangeAtlasSprite(spBtnCSGetArray[i],102);
				SetButtonSprite4Type(btnCSGetPool[i],spBtnCSGetArray[i]);
				btnCSGetPool[i].isEnabled = true;
				lbBtnGetArray[i].text = "已領取";
			}
			else
			{
				Utility.ChangeAtlasSprite(spBtnCSGetArray[i],101);
				SetButtonSprite4Type(btnCSGetPool[i],spBtnCSGetArray[i]);
				btnCSGetPool[i].isEnabled = true;
				lbBtnGetArray[i].text = "領取";
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void SetButtonSprite4Type(UIButton btn,UISprite sp)
	{
		btn.normalSprite = sp.spriteName;
		btn.hoverSprite = sp.spriteName;
		btn.disabledSprite = sp.spriteName;
		btn.pressedSprite = sp.spriteName;
	}
	//-----------------------------------------------------------------------------------------------------
	//設定星數獎勵
	public void SetStarRewardInfo()
	{
		SetBtnStarRewardInfo();
		GetNowChapter();
		if(sChapterItem == null)
		{
			btnChapterStars.gameObject.SetActive(false);
			return;
		}
		else
		{
			btnChapterStars.gameObject.SetActive(true);
			SetCSFactor(sChapterItem);
			SetCSRewards(sChapterItem);
			SetChapterStarGetBtn(sChapterItem);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void GetNowChapter()
	{
		sChapterItem = null;
		GameDataDB.ChapterItemDB.ResetByOrder();
		for(int i = 0;i < GameDataDB.ChapterItemDB.GetDataSize();++i)
		{
			S_ChapterItem_Tmp temp = GameDataDB.ChapterItemDB.GetDataByOrder();
			/*if(temp.iGroup == iChapter && temp.iGroupRank == GroupType)
			{
				sChapterItem = temp;
				break;
			}*/
		}
	}
	//-----------------------------------------------------------------------------------------------------
	#endregion 星數獎勵相關
}
