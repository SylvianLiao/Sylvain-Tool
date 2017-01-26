using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

class UI_BattleResult : NGUIChildGUI
{
	public UIButton 	btnLeaveBattle			= null;				//離開關卡按鈕
	public UILabel		lbLeaveBattle			= null;				//離開關卡字樣
	//戰勝結算相關
	public UIPanel		panelWinanimation		= null;				//戰勝前置畫面
	public UIPanel		panelWinState 			= null;				//戰勝畫面顯示
	public UIWidget		wgResultReward 			= null;				//戰鬥結果獎勵字樣集合
	public UISprite		spriteExpIcon			= null;				//經驗值圖示
	public UISprite		spriteMoneyIcon			= null;				//金錢圖示
	public UISprite		spritePointIcon			= null;				//積分圖示
	public UISprite		spriteMobsIcon			= null;				//殺怪圖示
	public UISprite		spriteStoneIcon			= null;				//通天塔獲得洗煉石
	public UILabel		lbTimeTitle				= null;				//通關時間標題
	public UILabel		lbTimeValue				= null;				//通關時間數值
	public UILabel		lbExpValue				= null;				//經驗值數值
	public UILabel		lbMoneyValue			= null;				//金錢數值
	public UILabel		lbPointValue			= null;				//積分數值
	public UILabel		lbMobsValue				= null;				//殺怪數值
	public UILabel		lbStoneValue			= null;				//洗煉石顆數
	public UILabel[]	lbBonusList				= new UILabel[3];	//通天塔中如果使用buff會顯示有額外的Bonus
	public Transform[]	PointAnchor;								//積分顯示定位
	//
	public UIWidget			TreasureList		= null;				//寶箱列
	public GameObject		Treasure			= null;				//寶箱Prefab
	public TreasureInfo[]	TreasureInfos		= null;				//複制的寶箱資訊
	public UIButton[]		btnTreasures		= null;				//寶箱陣列
	[HideInInspector]
	public int				iCurrentOpenChest		= 0;			//紀錄目前開啟的為哪一個寶箱
	[HideInInspector]
	public int 				OpenChestBoxNum			= 0;			//紀錄開啟前開了幾個寶箱
	public UILabel			lbOpenChestTip			= null;			//開啟付費寶箱需付商城幣告知
	public UISprite			spriteOpenChestTip		= null;			//商城幣圖示
	//
	public UITexture		texLoseBackground		= null;			//競技戰敗背景圖
	public UIWidget			LoseString				= null;			//戰敗字樣
	public UIWidget			TimeResultSet			= null;			//時間集合

	[HideInInspector]
	public long			GetiExp					= 0;	//
	[HideInInspector]
	public long			GetiMoney				= 0;	//
	[HideInInspector]
	public long			GetiPoint				= 0;	//
	[HideInInspector]
	public long			GetiMobs				= 0;	//
	[HideInInspector]
	public long			GetiStone				= 0;	//
	//戰敗結算相關
	public UIPanel		panelLoseState 			= null;	//戰敗畫面顯示

	public UILabel		lbRequestReviveFore		= null;	//復活使用前言
	public UILabel		lbRequestReviveBack 	= null;	//後活使用後語
	public UIButton		btnReviveYes			= null;	//確定復活
	public UIButton		btnReviveNo				= null;	//離開關卡
	public UILabel		lbReviveYes				= null; //是
	public UILabel		lbReviveNo				= null; //否
	public UISprite		spriteReviveSpinIcon	= null; //復活時使用幣別圖示
	//
	private const int 	minValue 				= 0;
	private const int 	maxValue 				= 9;
	private int 		iExpRCount				= 6;
	private int			iMoneyRCount			= 6;
	private int			iPointRCount			= 6;
	private int			iMobsRCount				= 6;
	private int			iStoneRCount			= 6;
	private long[]		iExpRnums				= new long[6];
	private long[]		iMoneyRnums				= new long[6];
	private long[]		iPointRnums				= new long[6];
	private long[]		iMobsRnums				= new long[6];
	private long[]		iStoneRnums				= new long[6];
	System.Random		rnd						= new System.Random();
	//
	public const int	iTreasureNum			= 3;
	//
	public UIWidget		StarSets				= null;
	public UIWidget		StarConditions			= null;
	public UISprite[] 	StageStars				= null;
	public UISprite[]	StageStarsBack			= null;
	public UILabel		lb2StarCondition		= null;
	public UILabel		lb3StarCondition		= null;
	//底部按鈕列
	[Header("BottomBtnList")]
	public UIPanel		panelBottomBtnList		= null; //底部按鈕集合
	public UIButton		btnReChallenge			= null; //重新挑戰
	public UIButton		btnBackToLobby			= null; //回大廳
	public UIButton		btnChooseDungeon		= null; //關卡選單
	public UIButton		btnOther				= null; //綜合按鈕(包含戰勝==>下一關 繼續任務 戰敗==>我要變強
	public UILabel		lbReChallenge			= null; //重新挑戰文字
	public UILabel		lbBackToLobby			= null; //回大廳文字
	public UILabel		lbChooseDungeon			= null; //關卡選單文字
	public UILabel		lbOther					= null; //綜合按鈕文字
	public UIGrid		GridBtnList				= null; //排按鈕
	//--------------------------------指引教學相關元件---------------------------------------------------------------------
	public UIPanel 		panelGuide 					= null;
	public UIButton		btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton		btnFullScreen 				= null;
	public GameObject 	objGuideVictory 			= null;
	public UILabel 		lbGuideVictoryExp 			= null;
	public UISprite 	spriteGuideResult 			= null;
	public UILabel 		lbGuideResult 				= null;
	public GameObject 	objGuideTreasure 			= null;
	public UILabel 		lbGuideTreasureExp 			= null;
	public UISprite 	spriteGuideChooseTreasure	= null;
	public UILabel 		lbGuideChooseTreasureExp 	= null;
	public GameObject 	gGuideShowTreasure 			= null;
	public UILabel 		lbGuideShowTreasure 		= null;
	public UISprite 	spGuideSpendDiamond 		= null;
	public UILabel 		lbGuideSpendDiamond 		= null;
	public UISprite 	spriteGuideQuiteLevel 		= null;
	public UILabel 		lbGuideQuiteLevelExp 		= null;
	[HideInInspector]
	public bool 		uiMoneyCheck 				= false;		//等待金錢跳完用

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_BattleResult";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_BattleResult() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize ();
		SwitchBottomBtnList(false,false,false,false);
		panelBottomBtnList.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		for(int i=0;i<lbBonusList.Length;++i)
			lbBonusList[i].text =GameDataDB.GetString(524);

		lbLeaveBattle.text = GameDataDB.GetString(553);			//"離開關卡"
		//
		lbReChallenge.text 		= GameDataDB.GetString(1770); //重新挑戰
		lbBackToLobby.text		= GameDataDB.GetString(1771); //回大廳
		lbChooseDungeon.text	= GameDataDB.GetString(1772); //關卡選單
		//panelBottomBtnList.gameObject.SetActive(false);
		lbOpenChestTip.gameObject.SetActive(false);
		//StarSets.gameObject.SetActive(false);
		StarConditions.gameObject.SetActive(false);

		spriteOpenChestTip.gameObject.SetActive(false);
		GenerateTreasureInfo();									//生成寶箱
		//TreasureHideOrShow(false);							//隱藏寶箱
		Utility.ChangeAtlasSprite (spriteOpenChestTip, 10001);
		Utility.ChangeAtlasSprite (spriteReviveSpinIcon, 10001);
	}
	//-----------------------------------------------------------------------------------------------------
	void Update()
	{
		if(panelWinState.gameObject.activeSelf)
		{
			//戰勝結算經驗值跟金錢的數字跳動
			//
			if(lbPointValue.gameObject.activeSelf)
			{
				if(GetiPoint == 0)
					lbPointValue.text = "000000";
				else
				{
					if(iPointRCount>=0)	
						lbPointValue.text 	= RandomNum(iPointRnums,iPointRCount);
				}
			}
			if(lbExpValue.gameObject.activeSelf)
			{
				if(GetiExp == 0)
					lbExpValue.text = "000000";
				else
				{
					if(iExpRCount>=0)	
						lbExpValue.text 	= RandomNum(iExpRnums,iExpRCount);
				}
			}
			if(lbMoneyValue.gameObject.activeSelf)
			{
				if(GetiMoney == 0)
					lbMoneyValue.text = "000000";
				else
				{
					if(iMoneyRCount>=0)	
						lbMoneyValue.text = RandomNum(iMoneyRnums,iMoneyRCount); 
				}
			}
			if(lbMobsValue.gameObject.activeSelf)
			{
				if(GetiMobs == 0)
					lbMobsValue.text = "000000";
				else
				{
					if(iMobsRCount>=0)	
						lbMobsValue.text = RandomNum(iMobsRnums,iMobsRCount); 
				}
			}
			if(lbStoneValue.gameObject.activeSelf)
			{
				if(GetiStone == 0)
					lbMobsValue.text = "000000";
				else
				{
					if(iStoneRCount>=0)	
						lbStoneValue.text = RandomNum(iStoneRnums,iStoneRCount); 
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//隨機快速跳數字顯示
	string RandomNum(long[] iRnum,int iRcount)
	{
		string strRandomNum = null;
		for (int i=0;i<iRcount;++i)
		{
			iRnum[i] = rnd.Next(minValue,maxValue);
		}
		for (int i=0;i<iRnum.Length;++i)
		{
			strRandomNum = iRnum[i].ToString()+	strRandomNum ;
		}
		return strRandomNum;
	}
	//-----------------------------------------------------------------------------------------------------
	//戰勝前置畫面
	public void WinForeAnim()
	{
		StartCoroutine("ForeAnim", 4.0f);
	}
	//-----------------------------------------------------------------------------------------------------
	//戰敗前置畫面
	public void LoseForeAnim()
	{
		StartCoroutine("ForeAnim", 1.0f);
	}
	//-----------------------------------------------------------------------------------------------------
	//戰勝/戰敗 前置畫面
	private IEnumerator ForeAnim(float delayTime)
	{
		long iTempGetMoney 	= GetiMoney;
		long iTempGetExp	= GetiExp;
		long iTempGetPoint	= GetiPoint;
		long iTempGetMobs	= GetiMobs;
		long iTempGetStone	= GetiStone;

		yield return new WaitForSeconds(delayTime);

		panelWinanimation.gameObject.SetActive(false);
		panelWinState.gameObject.SetActive(true);

		uiMoneyCheck = true;//GuideState等待金錢跳完用
		//
		//將寶箱指派事件
		for(int i=0;i<btnTreasures.Length;++i)
		{
			UIEventListener.Get(btnTreasures[i].gameObject).onClick += OpenChestBoxEvent;
		}
		//星等顯示
		DungeonBaseState dbs = ARPGApplication.instance.GetCurrentBottomGameState() as DungeonBaseState;
		BattleResultState brs = ARPGApplication.instance.GetGameStateByName(GameDefine.BATTLERESULT_STATE) as BattleResultState;
		for(int j=0; j<StageStarsBack.Length; ++j)
		{
			StageStarsBack[j].gameObject.SetActive(dbs.getDBF.iPart == 0);
		}
		for(int j=0; j<StageStars.Length; ++j)
		{
			StageStars[j].gameObject.SetActive(false);
			if(dbs.getDBF.iPart==0 && j<brs.iThisStageStarRank)	//主關卡才顯示
				StageStars[j].gameObject.SetActive(true);
		}
		//
		lbOpenChestTip.gameObject.SetActive(true);
		StarConditions.gameObject.SetActive(true);

		//帶加成角色觸發設定劇情對話
		for(int i=0; i<dbs.getDBF.sDBPD.Length; ++i)
		{
			int iPetNumber = 0;
			if(dbs.getDBF.sDBPD[i].m_iBonusPet>0														&&
			   ChooseDungeonState.bSearchSamePet(dbs.getDBF.sDBPD[i].m_iBonusPet, out iPetNumber)==true	)
			{
				ARPGApplication.instance.m_DialogueSystem.GenerateDialogue(dbs.getDBF.iBonusPet_Dialogue);
				break;
			}
		}

		yield return new WaitForSeconds (0.5f);
		if (GetiMoney > 0)
		{
			for(int a=5;a>=0;--a)
			{
				yield return new WaitForSeconds (0.2f);
				--iMoneyRCount;
				iMoneyRnums[a] = (iTempGetMoney/(long)(Math.Pow (10,a)));
				iTempGetMoney = iTempGetMoney - (iMoneyRnums[a]*(long)(Math.Pow (10,a)));
			}
		}
		//
		if (GetiExp >0)
		{
			for(int a=5;a>=0;--a)
			{
				yield return new WaitForSeconds (0.2f);
				--iExpRCount;
				iExpRnums[a] = (iTempGetExp/(long)(Math.Pow(10,a)));
				iTempGetExp = iTempGetExp -(iExpRnums[a]*(long)(Math.Pow(10,a)));
			}
		}
		//
		if (GetiPoint >0)
		{
			for(int a=5;a>=0;--a)
			{
				yield return new WaitForSeconds (0.2f);
				--iPointRCount;
				iPointRnums[a] = (iTempGetPoint/(long)(Math.Pow (10,a)));
				iTempGetPoint = iTempGetPoint - (iPointRnums[a]*(long)(Math.Pow (10,a)));
			}
		}

		if (GetiMobs > 0)
		{
			for(int a=5;a>=0;--a)
			{
				yield return new WaitForSeconds (0.2f);
				--iMobsRCount;
				iMobsRnums[a] = (iTempGetMobs/(long)(Math.Pow (10,a)));
				iTempGetMobs = iTempGetMobs - (iMobsRnums[a]*(long)(Math.Pow (10,a)));
			}
		}

		if (GetiStone > 0)
		{
			for(int a=5;a>=0;--a)
			{
				yield return new WaitForSeconds (0.2f);
				--iStoneRCount;
				iStoneRnums[a] = (iTempGetStone/(long)(Math.Pow (10,a)));
				iTempGetStone = iTempGetStone - (iStoneRnums[a]*(long)(Math.Pow (10,a)));
			}
		}
		//panelBottomBtnList.gameObject.SetActive(true);
		//TreasureHideOrShow(true);
		//TreasureShockEffect();
	}
	//-----------------------------------------------------------------------------------------------------
	//商城寶箱開啟
	void ItemMallBoxOpen()
	{
		JsonSlot_ItemMall.Send_CtoM_BuyDungeonChest();
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	//生成寶箱
	private void GenerateTreasureInfo()
	{
		if(Treasure!=null)
		{
			TreasureInfos = new TreasureInfo[iTreasureNum];
			btnTreasures = new UIButton[iTreasureNum];
			
			//動態產生DungeonInfo物件

			for(int i=0; i<TreasureInfos.Length; ++i)
			{
				GameObject newGO = Instantiate(Treasure) as GameObject;
				newGO.transform.parent = Treasure.transform.parent;
				
				//調整位置
				newGO.transform.localPosition = new Vector3(Treasure.transform.localPosition.x + 370.0f*i,
				                                            Treasure.transform.localPosition.y,
				                                            Treasure.transform.localPosition.z				);
				newGO.transform.rotation = Treasure.transform.rotation;
				newGO.transform.localScale = Treasure.transform.localScale;
				
				TreasureInfos[i] = newGO.GetComponent<TreasureInfo>();
				TreasureInfos[i].btnTreasureBox.userData = i;
				btnTreasures[i] = TreasureInfos[i].btnTreasureBox;
			}
			//隱藏原始模型
			Treasure.gameObject.SetActive(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//寶箱的顯示與隱藏
	private void TreasureHideOrShow(bool bsw)
	{
		if(TreasureInfos!=null)
		{
			for(int i=0;i<TreasureInfos.Length;++i)
				TreasureInfos[i].gameObject.SetActive(bsw);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	/*//寶箱出現的效果
	private void TreasureShockEffect()
	{
		TweenScale ts;
		if(TreasureInfos!=null)
		{
			for(int i=0;i<TreasureInfos.Length;++i)
			{
				ts = TreasureInfos[i].btnTreasureBox.GetComponent<TweenScale>();
				ts.enabled = true;
				if(i==(iTreasureNum-1))
					ts.AddOnFinished(EnableClickFunction);
			}
		}
	}*/
	//-----------------------------------------------------------------------------------------------------
	private void EnableClickFunction()
	{
		if(TreasureInfos!=null)
		{
			for(int i=0;i<TreasureInfos.Length;++i)
				TreasureInfos[i].btnTreasureBox.GetComponent<BoxCollider>().enabled=true;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//寶箱開啟設定事件
	public void OpenChestBoxEvent(GameObject gb)
	{
		//判斷開啟哪個寶箱
		iCurrentOpenChest = (int)gb.GetComponent<UIButton>().userData;
		//設定物品
		BattleResultState BRstate = (BattleResultState)ARPGApplication.instance.GetGameStateByName(GameDefine.BATTLERESULT_STATE);
		//設定寶箱內容
		BRstate.SetChestContent();
		//
		if(OpenChestBoxNum==0)
		{
			BRstate.OpenChestBoxEffect();
			panelBottomBtnList.gameObject.SetActive(true);
			if (ARPGApplication.instance.m_TeachingSystem.CheckReadyEvent(GameDefine.LOBBY_STATE))
			{
				btnReChallenge.gameObject.SetActive(false);
				btnChooseDungeon.gameObject.SetActive(false);
			}
			SwitchBottomBtnList(true,true,true,true); //開啟可以離開結算

			//如果是不可購買寶箱的關卡
			//if(ARPGApplication.instance.m_ActivityMgrSystem.bActivityProcessFlag == true)
			if(BRstate.getDBF.DungeonOption.GetFlag(ENUM_DungeonOptionFlag.ENUM_DungeonOptionFlag_CantBuyChest)==true)
			{
				for(int i=0;i<btnTreasures.Length;++i)
					btnTreasures[i].isEnabled=false;
				lbOpenChestTip.text = GameDataDB.GetString(18);			//""
				//ARPGApplication.instance.m_ActivityMgrSystem.bActivityProcessFlag = false;
			}
		}
		else
		{
			//呼叫Server告知開啟寶箱
			JsonSlot_ItemMall.Send_CtoM_BuyDungeonChest();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void AddToDisplay (Transform obj)
	{
		for(int i=0;i<PointAnchor.Length;i++)
		{
			if (0 == PointAnchor[i].childCount)
			{
				obj.SetParent(PointAnchor[i]);
				obj.localPosition = Vector3.zero;
				obj.gameObject.SetActive(true);
				break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//控制底下按鈕是否可以按
	public void SwitchBottomBtnList(bool bRC, bool bBL, bool bCD, bool bO)
	{
		btnReChallenge.isEnabled 	= bRC;
		btnBackToLobby.isEnabled 	= bBL;
		btnChooseDungeon.isEnabled 	= bCD;
		btnOther.isEnabled			= bO;
	}
	//-----------------------------------------------------------------------------------------------------
}