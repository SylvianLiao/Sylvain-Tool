using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class UI_DepositDiamond : NGUIChildGUI
{
	public UIPanel		panelBase				= null;				//儲值介面集合
//	public UILabel[]	lbNeedDiamonds			= new UILabel[6];	//購買寶石數
//	public UILabel[]	lbPayHowMuchs			= new UILabel[6];	//購買金額
	//----------------------------------------------------------------------
	public UIWidget     wgFullScreen	        = null;             //全螢幕collider
	//左方分頁UI
	public UIWidget		wgDepositPage			= null;				//儲值頁面
	public UIGrid		gdDepositSlot			= null;				//排列儲值頁面
	public UIWidget		wgMonthCardPage			= null;				//月卡頁面
	public UIGrid		gdVipCardSlot			= null;				//排列月卡頁面
	//儲值頁面UI
	public UIButton		btnDepositPage			= null;				//儲值頁面按鈕
	public UILabel		lbDepositPage			= null;
	//public UISprite		spDepositPageTip		= null;				//目前顯示分頁提示
	//月卡頁面UI
	public UIButton		btnMonthCardPage		= null;				//月卡頁面按鈕
	public UILabel		lbMonthCardPage			= null;				
	//public UISprite		spMonthCardPageTip		= null;				//目前顯示分頁提示
	public UILabel		lbGetDiamondRemind		= null;				//每日寶石相關提醒
	public UIButton		btnGetDiamond			= null;				//每日領取寶石按鈕
	public UILabel		lbGetDiamond			= null;				
	public UILabel		lbGetRemnant			= null;				//剩餘領取每日寶石次數

	//上方UI
	public UILabel      lbYouCurrentIs          = null;             //您目前VIP為
	public UISprite     spCurrentVIP            = null;             //目前VIP
	public UILabel      lbNeedToSave            = null;             //再儲存N寶石
	public UILabel      lbCanLvUp         		= null;             //可以升級至VIP N
	public UISprite     spNextVIP          		= null;             //下一階VIP
	public UILabel      lbVipRatio              = null;             //儲值累積值
	public UISprite     spVipBar                = null;             //Vip值累積圖示
	public UIButton		btnVipNote				= null;				//Vip說明按鈕
	public UILabel		lbVipNote				= null;				

	//----------------------------管理器---------------------------------------
	public Transform[] 			tSlotDepositPos				= new Transform[6]; 	//購買寶石按鈕
	[HideInInspector]public List<Slot_VipGoods>	m_SlotVipCardsList			= new List<Slot_VipGoods>();	//Vip卡包清單
	[HideInInspector]public List<Slot_VipGoods>	m_SlotDepositList			= new List<Slot_VipGoods>();	//儲值Slot清單
	//----------------------------執行用變數---------------------------------------
	[HideInInspector]public UI_TopStateView		uiTopStateView				= null;		//資訊列
	[HideInInspector]public UI_VIPLvUp			uiVipLvUp					= null;		//VIP升級頁面
	[HideInInspector]public GameObject          m_PageOpened	    	   	= null;		//紀錄正在開啟的頁面
	private const float 		m_CardSlotNormalWidth		= 250.0f;	//月卡商品圖示間距
	private const float 		m_TwoCardSlotWidth			= 350.0f;	//兩個月卡商品圖示時的間距
	private int 				m_DepositGoodsID			= 1;		//儲值寶石商品樣板表起始ID
	private int 				m_MonthCardStoreID			= 2;		//月卡商店樣板表起始ID
	private const string 		m_SlotVipGoodsName			= "Slot_VipGoods";
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel				panelGuide					= null; //教學集合
	public UIButton				btnTopFullScreen			= null; //最上層的全螢幕按鈕
	public UIButton				btnFullScreen				= null; //全螢幕按鈕
	public UISprite				spGuideDeposit				= null; //導引介紹儲值
	public UILabel				lbGuideDeposit				= null;
	public UISprite				spGuideClickVipNote			= null; //導引點擊VIP說明按鈕
	public UILabel				lbGuideClickVipNote			= null;
	//-----------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_DepositDiamond";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_DepositDiamond() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitDepositUI();
	}
	//-----------------------------------------------------------------------------------------------------
	public void InitDepositUI()
	{
		lbYouCurrentIs.text 	= GameDataDB.GetString(1907);		//"您目前為"
		lbCanLvUp.text			= GameDataDB.GetString(1909);		//"即可升級到"
		lbVipNote.text	 		= GameDataDB.GetString(1910);		//"VIP特權"
		lbDepositPage.text 		= GameDataDB.GetString(GameDataDB.StoreDB.GetData(m_DepositGoodsID).iName);	//"儲值商店"
		lbMonthCardPage.text	= GameDataDB.GetString(GameDataDB.StoreDB.GetData(m_MonthCardStoreID).iName);	//"月/季卡商店"

		//生成商品Slot給儲值、月卡分頁用
		CreateVipGoodsSlot();

		//儲值分頁UI初始化
		for(int i=0; i < m_SlotDepositList.Count; ++i)
		{
			m_SlotDepositList[i].SetVipGoods(GameDefine.ITEMMALL_DEPOSITGOODS_GUID+i , m_DepositGoodsID);
			m_SlotDepositList[i].btnBuyVipGoods.userData = i;
		}
		gdDepositSlot.enabled = true;
		gdDepositSlot.Reposition();

		//月卡分頁UI初始化
		for(int i=0; i < m_SlotVipCardsList.Count; ++i)
		{
			m_SlotVipCardsList[i].SetVipGoods(GameDefine.ITEMMALL_MONTH_GUID+i , m_MonthCardStoreID);
		}
		gdVipCardSlot.enabled = true;
		gdVipCardSlot.Reposition();

		//預設為儲值分頁
		OpenPage(wgDepositPage.gameObject);
	}
	//-------------------------------------------------------------------------------------------------------------
	//生成儲值Slot、VIP卡包Slot
	public void CreateVipGoodsSlot()
	{
		Slot_VipGoods go = ResourceManager.Instance.GetGUI(m_SlotVipGoodsName).GetComponent<Slot_VipGoods>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotVipGoodsName) );
			return;
		}

		int goodsNumber = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStoreGoodsNumber(m_MonthCardStoreID);
		if (goodsNumber == 2)
			gdVipCardSlot.cellWidth = m_TwoCardSlotWidth;
		else
			gdVipCardSlot.cellWidth = m_CardSlotNormalWidth;
		//生成卡包Slot
		for(int i=0; i < goodsNumber; ++i)
		{
			Slot_VipGoods newgo= GameObject.Instantiate(go) as Slot_VipGoods;
			newgo.transform.parent			= gdVipCardSlot.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.name =  string.Format("slotVipGoods{0:00}",i);
			newgo.GetComponent<UIButton>().userData = i;
			m_SlotVipCardsList.Add(newgo);
		}

		goodsNumber = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStoreGoodsNumber(m_DepositGoodsID);
		//生成儲值Slot
		for(int i=0; i < goodsNumber; ++i)
		{
			Slot_VipGoods newgo= GameObject.Instantiate(go) as Slot_VipGoods;
			newgo.transform.parent			= gdDepositSlot.transform;
			newgo.transform.localScale		= new Vector3(0.9f , 0.9f , 1.0f);
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.name =  string.Format("slotDeposit{0:00}",i);
			newgo.GetComponent<UIButton>().userData = i;
			m_SlotDepositList.Add(newgo);
		}
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();

		if (uiVipLvUp != null)
			uiVipLvUp.panelBase.depth = this.panelBase.depth+1;
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	public void OpenPage(GameObject page/* , GameObject pageTip*/)
	{
		if (m_PageOpened != page.gameObject)
		{
			if (m_PageOpened != null)
				m_PageOpened.SetActive(false);
			//spDepositPageTip.gameObject.SetActive(false);
			//spMonthCardPageTip.gameObject.SetActive(false);

			page.SetActive(true);
			//pageTip.SetActive(true);
			TweenPosition twPos = page.GetComponent<TweenPosition>();
			if (twPos != null)
			{
				twPos.enabled = true;
				twPos.ResetToBeginning();
			}
			m_PageOpened = page;
		}
	}
}
