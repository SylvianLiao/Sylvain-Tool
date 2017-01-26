using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class StoreInfo
{
	public ENUM_ItemMallType	m_StoreType			= ENUM_ItemMallType.ENUM_ItemMallType_None;
	public UIButton				btnStore			= null;				//商店頁面按鈕
	public UILabel				lbStoreName			= null;
	public UISprite				spStoreTip			= null;				//目前顯示分頁提示
}
//-------------------------------------------------------------------------------------------------
public class UI_VipStore : NGUIChildGUI  {

	public UIPanel			panelBase				= null;				//商城介面集合
	public UIWidget     	wgFullScreen	        = null;             //全螢幕collider
	public UIWidget     	wgPanelScreen	        = null;             //商店分頁內的Collider
	public UIScrollView		svStorePage				= null;
	public UIPanel			panelStorePage			= null;				//商店分頁集合
	public UIGrid			gdGoods					= null;				//商品排列(複製用)
	//public UIWidget			wgGoods					= null;				//商品排列(複製用)
	[Header("StoreButton")]
	public UIScrollView		svStoreBtn				= null;
	public UIGrid			gdStoreBtn				= null;	
	public UIButton			btnStore				= null;				//商店按鈕樣板

	[Header("AnotherUI")]
	public UILabel      	lbCurrencyNum          	= null;             //目前貨幣數量
	public UISprite   		spCurrency           	= null;             //目前貨幣圖案
	public UIButton			btnRefresh				= null;				//刷新按鈕
	public UILabel     		lbRefresh       	    = null;             
	public UILabel     		lbRefreshTime       	= null;				//刷新時間	

	[Header("StoreGroup")]
	public UILabel      	lbStoreGroupTitle      	= null;             //目前商店群組名稱
	public UIButton			btnItemMall				= null;				//特惠商城
	public UILabel     		lbItemMall      	    = null;            
	public UISprite     	spItemMallChoosen    	= null;             
	public UIButton			btnNormalStore			= null;				//雜貨商店	
	public UILabel     		lbNormalStore       	= null;				
	public UISprite     	spNormalStoreChoosen    = null;             
	public UIButton			btnCrystalStore			= null;				//水晶商店	
	public UILabel     		lbCrystalStore       	= null;            
	public UISprite     	spCrystalStoreChoosen   = null;            
	public UIButton			btnGuildStore			= null;				//公會商店	
	public UILabel     		lbGuildStore       		= null;				
	public UISprite     	spGuildStoreChoosen     = null;          

	[Header("CheckBuyBox")]
	public UIPanel			panelCheckBuy			= null;
	public UISprite			SpriteItemInfoBG		= null;
	public Transform		SlotItemPos				= null;
	public UILabel			LabelItemName			= null;
	public UILabel			lbItemNoteType			= null;
	public UILabel			LabelItemNote			= null;
	public UILabel			lbNTCost				= null;
	public UISprite			spCurrencyType			= null;				//商品購買貨幣類型: 水晶、寶石、遊戲幣
	public UILabel			lbNormalCost			= null;
	public UIButton			btnCheckBuy				= null;
	public UILabel			lbCheckBuy				= null;
	public UIButton			btnCloseCheckBuy		= null;
	//-------------------------------------------------------------------------------------------------	
	[HideInInspector]public List<GameObject>		m_StorePageList			= new List<GameObject>();		//商店分頁清單
	[HideInInspector]public List<Slot_VipGoods>		m_SlotVipGoodsList		= new List<Slot_VipGoods>();	//Vip商品清單
	[HideInInspector]public List<StoreInfo>			m_StoreInfoList			= new List<StoreInfo>();		//商店清單
	[HideInInspector]public UI_TopStateView			uiTopStateView			= null;							//資訊列
	//----------------------------------執行用變數-------------------------------------------------		
	private const int 			m_GoodsNumberOnePage		= 6;									//一頁可顯示的VIP商品數量
	private const int 			m_StoreNumberOnePage		= 5;									//一頁可顯示的商店分頁按鈕數量
	private const int 			m_GoodsVerticalNumber		= 2;									//VIP商品垂直的數量
	private Vector3 			m_PanelStorePagePos			= Vector3.zero;							//暫存panelGoods位置
	private Vector3 			m_GdStorePagePos			= Vector3.zero;							//暫存gdGoods位置
	//private Vector3 			m_FarPos					= new Vector3(655.0f , 135.0f , 0.0f);	//每次切換商店時，gdGoods暫時的位置
	[HideInInspector]public Slot_Item			m_ItemInCheckBuy 			= null;					//暫存SlotItem用
	[HideInInspector]public const string 		m_SlotVipGoodsName			= "Slot_VipGoods";		//VIP商品樣板Slot名稱
	[HideInInspector]public const string		m_SlotItemName 				= "Slot_Item";
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel				panelGuide					= null; //教學集合
	public UIButton				btnTopFullScreen			= null; //最上層的全螢幕按鈕
	public UIButton				btnFullScreen				= null; //全螢幕按鈕
	public UILabel				lbGuideIntroduce			= null;	//導引介紹商店
	public UILabel				lbGuideFinish				= null;	//導引教學結束
	// smartObjectName
	private const string 		GUI_SMARTOBJECT_NAME 		= "UI_VipStore";
	//-------------------------------------------------------------------------------------------------
	private UI_VipStore() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitLabel();
		InitVipStoreUI();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	private void InitVipStoreUI()
	{
		m_PanelStorePagePos = panelStorePage.transform.localPosition;
		m_GdStorePagePos = gdGoods.transform.localPosition;
		svStorePage.gameObject.SetActive(false);

		//生成商店
		CreateStore();

		//預設顯示目前商店群組中的第一個商店
//		if (m_StoreInfoList.Count > 0)
//		{
//			m_StoreInfoList[0].spStoreTip.gameObject.SetActive(true);
//			ShowStoreUI(m_StoreInfoList[0].m_StoreType);
//		}

		//設定確認購買視窗UI
		CreatItemInCheckBuy();

		svStorePage.onStoppedMoving += UnBlockStore;
		svStorePage.onDragStarted += BlockStore;
		svStoreBtn.onStoppedMoving += UnBlockStore;
		svStoreBtn.onDragStarted += BlockStore;
	}
	//-------------------------------------------------------------------------------------------------
	private void InitLabel()
	{
		lbCheckBuy.text = GameDataDB.GetString(1931);		//"確認購買"
		lbItemMall.text = GameDataDB.GetString(315);		//"商城"
		lbNormalStore.text = GameDataDB.GetString(15056);	//"雜貨商店" 
		lbCrystalStore.text = GameDataDB.GetString(2833);	//"水晶商店"	
		lbGuildStore.text = GameDataDB.GetString(1519);		//"公會商店"	
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary>設定商店資料 , 由外部開啟時設定 , StorePage = 第幾個商店分頁</summary>
	public void CreateAndSetStoreBtn(List<ENUM_ItemMallType> storeList)
	{
		if (storeList.Count <= 0)
			return;

		int creatCount = storeList.Count;
		if (m_StoreInfoList.Count > 0)
		{
			creatCount = storeList.Count - m_StoreInfoList.Count;
		}

		if (creatCount > 0)
		{
			//生成商店分頁
			for(int i=0; i < creatCount; ++i)
			{
				StoreInfo storeInfo = new StoreInfo();

				UIButton storeBtn = Instantiate(btnStore) as UIButton;
				storeBtn.transform.parent = btnStore.transform.parent;
				storeBtn.transform.localScale = Vector3.one;
				storeBtn.transform.localRotation = new Quaternion(0,0,0,0);
				
				storeInfo.btnStore = storeBtn;
				storeInfo.btnStore.name = "Button(Store"+(m_StoreInfoList.Count)+")";
				Transform trans = storeBtn.transform.FindChild("Label(Store)");
				if (trans == null)
					continue;
				storeInfo.lbStoreName = trans.GetComponent<UILabel>();
				trans = storeBtn.transform.FindChild("Sprite(Choosed)");
				if (trans == null)
					continue;
				storeInfo.spStoreTip = trans.GetComponent<UISprite>();
				m_StoreInfoList.Add(storeInfo);
			}
		}

		//設定商店分頁
		for(int i=0; i < m_StoreInfoList.Count; ++i)
		{
			//若實體商店分頁按鈕數量 > 目前需要的按鈕數量，則將多餘的按鈕關閉
			if (i >= storeList.Count)
			{
				m_StoreInfoList[i].m_StoreType = ENUM_ItemMallType.ENUM_ItemMallType_None;
				m_StoreInfoList[i].btnStore.gameObject.SetActive(false);
				continue;
			}

			m_StoreInfoList[i].m_StoreType = storeList[i];
			if (m_StoreInfoList[i].m_StoreType == ENUM_ItemMallType.ENUM_ItemMallType_None || 
			    m_StoreInfoList[i].m_StoreType == ENUM_ItemMallType.ENUM_ItemMallType_MAX)
				continue;

			S_Store_Tmp storeTmp = GameDataDB.StoreDB.GetData((int)m_StoreInfoList[i].m_StoreType);
			if (storeTmp == null)
				continue;
		
			m_StoreInfoList[i].lbStoreName.text = GameDataDB.GetString(storeTmp.iName);		//商店名稱
			m_StoreInfoList[i].btnStore.gameObject.SetActive(true);
			m_StoreInfoList[i].btnStore.enabled = true;
		}
		gdStoreBtn.enabled = true;
		gdStoreBtn.Reposition();

		//商店分頁按鈕小於5個就關閉拖曳功能
		svStoreBtn.enabled = (storeList.Count > m_StoreNumberOnePage);
	}
	//-------------------------------------------------------------------------------------------------
	//生成商店
	public void CreateStore()
	{
		int maxGoods = 0;		
		//找出所有商店中販售最多的商品數量，計算需要生成的商店分頁
		for(int i=0; i < m_StoreInfoList.Count; ++i)
		{
			S_Store_Tmp storeTmp =  GameDataDB.StoreDB.GetData((int)m_StoreInfoList[i].m_StoreType);
			if (storeTmp == null)
				continue;
		
			int GoodsNumber = 0;	
			//取得商店的商品數量(限購商店的商品數量取得方式為Server給予)
			if (storeTmp.CheckStoreOption(ENUM_StoreOption.ENUM_StoreFunction_LimitGoods))
			{
				S_ItemMallData goodsData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetVipGoodsByStore(m_StoreInfoList[i].m_StoreType);
				if (goodsData != null)
				{
					GoodsNumber = goodsData.iItemMallGoodsID.Count;
				}
			}
			else
			{
				GoodsNumber = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStoreGoodsNumber(storeTmp.GUID);	
			}
			if (GoodsNumber > maxGoods)
				maxGoods = GoodsNumber;
		}

		//生成商品Slot
		CreateGoodsAndPage(maxGoods);
	}
	//-------------------------------------------------------------------------------------------------
	//生成商品Slot
	private void CreateGoodsAndPage(int slotCount)
	{
		if (m_SlotVipGoodsList.Count > 0)
		{
			slotCount = slotCount - m_SlotVipGoodsList.Count;
		}

		Slot_VipGoods go = ResourceManager.Instance.GetGUI(m_SlotVipGoodsName).GetComponent<Slot_VipGoods>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("Slot_VipGoods load prefeb error,path:{0}", "GUI/"+m_SlotVipGoodsName) );
			return;
		}

		if (slotCount > 0)
		{
			//生成商品Slot
			for(int i=0; i < slotCount; ++i)
			{
				Slot_VipGoods newgo = GameObject.Instantiate(go) as Slot_VipGoods;
				newgo.transform.parent			= gdGoods.transform;
				newgo.transform.localScale		= Vector3.one;
				newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
				newgo.transform.localPosition	= Vector3.zero;
				newgo.name =  string.Format("slotVipGoods{0:00}",i);
				//加入拖拉功能
				UIDragScrollView drag = newgo.gameObject.AddComponent<UIDragScrollView>();
				drag.scrollView = svStorePage;
				m_SlotVipGoodsList.Add(newgo);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//顯示切換商店需要切換的相關UI
	public void ShowStoreUI(ENUM_ItemMallType storeType)
	{
		//生成商店資料
		S_Store_Tmp storeTmp =  GameDataDB.StoreDB.GetData((int)storeType);
		if (storeTmp == null)
			return;
		//設定貨幣類型
		switch(storeTmp.iSpecialCurrency)
		{
		case ENUM_SpecialCurrencyType.ENUM_SpecialCurrencyType_None:
			spCurrency.gameObject.SetActive(false);
			break;
		case ENUM_SpecialCurrencyType.ENUM_SpecialCurrencyType_FP:
			Utility.ChangeAtlasSprite(spCurrency , GameDefine.ITEMMALL_CURRENCY_FP_ICONID);
			lbCurrencyNum.text = ARPGApplication.instance.m_RoleSystem.iBaseFP.ToString();
			spCurrency.gameObject.SetActive(true);
			break;
        case ENUM_SpecialCurrencyType.ENUM_SpecialCurrencyType_Guild:
            Utility.ChangeAtlasSprite(spCurrency, GameDefine.ITEMMALL_CURRENCY_GUILD_ICONID);
            lbCurrencyNum.text = ARPGApplication.instance.m_RoleSystem.iMemberMoney.ToString();
			spCurrency.gameObject.SetActive(true);
            break;
		}
		//判斷是否顯示刷新時間
		if (!storeTmp.CheckStoreOption(ENUM_StoreOption.ENUM_StoreFunction_ManualUpdate))
			btnRefresh.gameObject.SetActive(false);
		else
		{
			lbRefreshTime.text = GameDataDB.GetString(1926);			//"商店刷新時間: 每日04:00"
			lbRefresh.text = GameDataDB.GetString(1927);				//"刷新"
			btnRefresh.gameObject.SetActive(true);
		}
		AssignGoodsData(storeType);

		ResetStorePage();
	}
	//-------------------------------------------------------------------------------------------------
	//指定商店中的商品資料
	private void AssignGoodsData(ENUM_ItemMallType store)
	{
		S_Store_Tmp storeTmp =  GameDataDB.StoreDB.GetData((int)store);
		if (storeTmp == null)
			return;

		//先關閉所有商品，更換商店之後塞新的商品資料再打開
		for(int i=0; i < m_SlotVipGoodsList.Count ; ++i)
			m_SlotVipGoodsList[i].gameObject.SetActive(false);

//		if (storeTmp.iLimitList > m_SlotVipGoodsList.Count)
//		{
//			UnityDebugger.Debugger.Log("The Goods Number Of Store Are More Than Slot Numbers!");
//			return;
//		}
		C_RoleDataEx roleDataEx = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		int slotGoodsCount = 0;
		//限購商品資料於角色商城資料中讀取(由Server給予)
		if (storeTmp.CheckStoreOption(ENUM_StoreOption.ENUM_StoreFunction_LimitGoods))
		{
			S_ItemMallData goodsData = roleDataEx.GetVipGoodsByStore(store);
			if (goodsData != null)
			{
				for(int i=0; i < goodsData.iItemMallGoodsID.Count; ++i)
				{
					if (goodsData.iItemMallGoodsID[i] <= 0)
						continue;
					//重新設定商品被購買狀況
					if (goodsData.Flag.Get(i))
						m_SlotVipGoodsList[i].SetGoodStatus(Enum_GoodStatus.SellOut);
					else
						m_SlotVipGoodsList[i].SetGoodStatus(Enum_GoodStatus.Normal);
					m_SlotVipGoodsList[i].SetVipGoods(goodsData.iItemMallGoodsID[i] , (int)store);
					m_SlotVipGoodsList[i].gameObject.SetActive(true);
					slotGoodsCount++;
				}
			}
		}
		//非限購商品資料直接讀取DBF表
		else
		{
			GameDataDB.GoodsDB.ResetByOrder();
			for(int i=0 ; i<GameDataDB.GoodsDB.GetDataSize(); ++i)
			{
				S_Goods_Tmp goodsTmp = GameDataDB.GoodsDB.GetDataByOrder();
				if(goodsTmp != null)
				{
					//設定商品資料
					if (storeTmp.iGoodsListID == goodsTmp.iGoodsListID)
					{
						m_SlotVipGoodsList[slotGoodsCount].SetGoodStatus(Enum_GoodStatus.Normal);
						m_SlotVipGoodsList[slotGoodsCount].SetVipGoods(goodsTmp.GUID , (int)store);
						m_SlotVipGoodsList[slotGoodsCount].gameObject.SetActive(true);
						slotGoodsCount++;
					}
				}
			}
		}

		//商品Slot小於6個就關閉拖曳功能
		svStorePage.enabled = (slotGoodsCount > m_GoodsNumberOnePage);
	}
	//-------------------------------------------------------------------------------------------------
	//重置商店分頁集合中的ScrollView
	public void ResetStorePage()
	{
		//將商品位置初始化
		svStorePage.gameObject.SetActive(false);
		//排列商品
		gdGoods.enabled = true;
		gdGoods.Reposition();
		//重置商品分頁ScrollView位置
		bool isEnabled = svStorePage.enabled;
		svStorePage.enabled = false;
		panelStorePage.transform.localPosition = m_PanelStorePagePos;
		gdGoods.transform.localPosition = m_GdStorePagePos;
		panelStorePage.clipOffset = Vector2.zero;
		svStorePage.enabled = true;
		svStorePage.ResetPosition();
		svStorePage.enabled = isEnabled;
		//在Tween之前顯示商品UI
		svStorePage.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//重置商店按鈕的ScrollView
	public void ResetStoreBtn()
	{
		svStoreBtn.enabled = false;
		svStoreBtn.enabled = true;
		svStoreBtn.ResetPosition();
	}
	//-------------------------------------------------------------------------------------------------
	public void UnlockFullScreenAfterTw()
	{
		BlockFullScreen(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void BlockFullScreen(bool bBlock)
	{
		wgFullScreen.gameObject.SetActive(bBlock);
		wgPanelScreen.gameObject.SetActive(bBlock);
		uiTopStateView.SwitchBtnWork(!bBlock);
	}
	//-------------------------------------------------------------------------------------------------
	public void BlockStore()
	{
		for(int i=0; i<m_StoreInfoList.Count; ++i)
		{
			m_StoreInfoList[i].btnStore.enabled = false;
		}
		btnItemMall.enabled = false;
		btnNormalStore.enabled = false;
		btnCrystalStore.enabled = false;
		btnGuildStore.enabled = false;
	}
	//-------------------------------------------------------------------------------------------------
	public void UnBlockStore()
	{
		for(int i=0; i<m_StoreInfoList.Count; ++i)
		{
			m_StoreInfoList[i].btnStore.enabled = true;
		}
		btnItemMall.enabled = true;
		btnNormalStore.enabled = true;
		btnCrystalStore.enabled = true;
		btnGuildStore.enabled = true;
	}
	//-------------------------------------------------------------------------------------------------
	#region 確認購買視窗相關
	//-------------------------------------------------------------------------------------------------
	//生成確認購買視窗內的SlotItem
	private void CreatItemInCheckBuy()
	{
		if (m_ItemInCheckBuy != null)
			return;

		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotItemName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("UI_VipStore load prefeb error,path:{0}", "GUI/"+m_SlotItemName) );
			return;
		}

		m_ItemInCheckBuy = Instantiate(go) as Slot_Item;
		m_ItemInCheckBuy.transform.parent			= SpriteItemInfoBG.transform;
		m_ItemInCheckBuy.transform.localScale		= Vector3.one;
		m_ItemInCheckBuy.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		m_ItemInCheckBuy.transform.localPosition	= SlotItemPos.transform.localPosition;
		m_ItemInCheckBuy.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchCheckBuyBox(bool bSwitch)
	{
		if (uiTopStateView != null)
			uiTopStateView.SwitchBtnWork(!bSwitch);
		panelCheckBuy.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetStoreGroupUI(ENUM_StoreGroup storeGroup)
	{
		//切換暗亮圖 & 商店群組名稱
		spItemMallChoosen.gameObject.SetActive(false);
		spNormalStoreChoosen.gameObject.SetActive(false);
		spCrystalStoreChoosen.gameObject.SetActive(false);
		spGuildStoreChoosen.gameObject.SetActive(false);
		switch(storeGroup)
		{
		case ENUM_StoreGroup.ENUM_StoreGroup_ItemMall:
			spItemMallChoosen.gameObject.SetActive(true);
			lbStoreGroupTitle.text = GameDataDB.GetString(315);
			break;
		case ENUM_StoreGroup.ENUM_StoreGroup_Normal:
			spNormalStoreChoosen.gameObject.SetActive(true);
			lbStoreGroupTitle.text = GameDataDB.GetString(15056);
			break;
		case ENUM_StoreGroup.ENUM_StoreGroup_Crystal:
			spCrystalStoreChoosen.gameObject.SetActive(true);
			lbStoreGroupTitle.text = GameDataDB.GetString(2833);
			break;
		case ENUM_StoreGroup.ENUM_StoreGroup_Guild:
			spGuildStoreChoosen.gameObject.SetActive(true);
			lbStoreGroupTitle.text = GameDataDB.GetString(163);
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定確認購買視窗並顯示
	public void ShowCheckBuyBox(Slot_VipGoods goodsSlot)
	{
		S_Goods_Tmp goodsTmp = GameDataDB.GoodsDB.GetData(goodsSlot.m_VipGoodsID);
		if (goodsTmp == null)
			return;
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(goodsTmp.iItemID);
		if (itemTmp == null)
			return;
		if (m_ItemInCheckBuy != null)
			m_ItemInCheckBuy.SetSlotWithCount(goodsTmp.iItemID , goodsTmp.iGoodsHeap , false);
		//設定商品名稱
		/*
		if (goodsTmp.iGoodsHeap > 1)
			LabelItemName.text = GameDataDB.GetString(goodsTmp.iName)+" X"+goodsTmp.iGoodsHeap;
		else*/
		if(goodsTmp.iName <= 0)
		{
			LabelItemName.text = GameDataDB.GetString(itemTmp.iName);
		}
		else
		{
			LabelItemName.text = GameDataDB.GetString(goodsTmp.iName);
		}

		itemTmp.SetRareColorString(LabelItemName);
		//設定物品類型
		if(itemTmp.iItemNote>0)
		{
			lbItemNoteType.gameObject.SetActive(true);
			lbItemNoteType.text = GameDataDB.GetString(itemTmp.iItemNote);
		}
		else
			lbItemNoteType.gameObject.SetActive(false);

		//設定商品說明
		string str = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(itemTmp.iNote),null,SkillLevelType.Now);

		LabelItemNote.text = str;

		if (goodsTmp.iCurrencyType == ENUM_SpendType.ENUM_SpendType_Currency)
		{
			//設定商品價格
			if (goodsTmp.iCurrency == ENUM_CurrencyType.ENUM_CurrencyType_NT)
			{
				lbNTCost.text = string.Format(GameDataDB.GetString(1916) ,goodsTmp.fMoneyGoodsPrize.ToString());	//NT${0}
				lbNTCost.gameObject.SetActive(true); 
			}
			else
			{
				switch(goodsTmp.iCurrency)
				{
				case ENUM_CurrencyType.ENUM_CurrencyType_Diamond:
					Utility.ChangeAtlasSprite(spCurrencyType , GameDefine.ITEMMALL_CURRENCY_DIAMOND_ICONID);
					break;
				case ENUM_CurrencyType.ENUM_CurrencyType_GameCash:
					Utility.ChangeAtlasSprite(spCurrencyType , GameDefine.ITEMMALL_CURRENCY_MONEY_ICONID);
					break;
				case ENUM_CurrencyType.ENUM_CurrencyType_FP:
					Utility.ChangeAtlasSprite(spCurrencyType , GameDefine.ITEMMALL_CURRENCY_FP_ICONID);
					break;
				case ENUM_CurrencyType.ENUM_CurrencyType_GuildMoney:
					Utility.ChangeAtlasSprite(spCurrencyType, GameDefine.ITEMMALL_CURRENCY_GUILD_ICONID);
					break;
				}
				lbNormalCost.text = goodsTmp.fMoneyGoodsPrize.ToString();
				lbNormalCost.gameObject.SetActive(true);
			}
		}
		else if (goodsTmp.iCurrencyType == ENUM_SpendType.ENUM_SpendType_Item)
		{
			S_Item_Tmp currencyItemTmp = GameDataDB.ItemDB.GetData(goodsTmp.iCurrencyItemID);
			if (currencyItemTmp != null)
			{
				Utility.ChangeAtlasSprite(spCurrencyType, currencyItemTmp.ItemIcon);
				lbNormalCost.text = goodsTmp.iCurrencyItemCount.ToString();
				lbNormalCost.gameObject.SetActive(true);
			}
		}

		btnCheckBuy.userData = goodsSlot;
		SwitchCheckBuyBox(true);
	}
	#endregion
}
