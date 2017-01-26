using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;


public enum Enum_GoodStatus
{
	Normal			= 0,	//正常
	AlreadyBought 	= 1,	//已購買
	SellOut			= 2,	//已售完
	NoSell			= 3,	//已下架
}

public class Slot_VipGoods : MonoBehaviour{

	public UISprite			spVipGoodsBG			= null;		//商品背景
	public UIButton 		btnBuyVipGoods			= null; 	//購買商品按鈕
	public UILabel			lbVipGoodsName			= null;		//商品名稱
	public UILabel			lbVipGoodsAD			= null;		//商品廣告說明
	public UISprite			spVipGoodsIcon			= null;		//商品圖案
	public UILabel			lbVipGoodsCost			= null;		//商品金額
	public UISprite			spCurrencyType			= null;		//商品購買貨幣類型: 水晶、寶石、遊戲幣
	public UILabel			lbVipGoodsNTCost		= null;		//商品金額(台幣)
	public UILabel 			lbInfoAfterBuy	      	= null; 	//購買後顯示相關資訊
	//-------------------------------------------------------------------------------------------------
	public Slot_Item		m_SlotItem				= null;		//紀錄物品Slot
	public int 				m_VipGoodsID      		= 0; 		//此Slot所存的VIP商品編號
	//-----------------------執行用變數--------------------------------------------------------------
	[HideInInspector]
	public const string 	m_SlotItemName			= "Slot_Item";		//物品Slot名稱
	//-------------------------------------------------------------------------------------------------
 	private void Awake()
	{
		Initialize();
		//生成物品樣板
		CreateItemSlot();
	}
	//-------------------------------------------------------------------------------------------------
	private void Initialize()
	{
		lbVipGoodsName.text = "";
		lbVipGoodsAD.text = "";
		lbVipGoodsCost.text = "";
		lbVipGoodsNTCost.text = "";
		lbInfoAfterBuy.text = "";
		lbVipGoodsNTCost.gameObject.SetActive(false); 
		lbVipGoodsCost.gameObject.SetActive(false); 
		lbVipGoodsAD.gameObject.SetActive(false);
		lbInfoAfterBuy.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	//生成商店分頁與其商品Slot
	private void CreateItemSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotItemName).GetComponent<Slot_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("Slot_VipGoods load prefeb error,path:{0}", "GUI/"+m_SlotItemName) );
			return;
		}
		//生成商品Slot
		Slot_Item newgo = GameObject.Instantiate(go) as Slot_Item;
		//newgo.gameObject.SetActive(false);
		newgo.transform.parent			= this.transform;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		newgo.transform.localPosition	= spVipGoodsIcon.transform.localPosition;
		newgo.gameObject.SetActive(true);
		m_SlotItem = newgo;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetVipGoods(int goodsID , int storeID)
	{
		//儲存VIP商品編號
		m_VipGoodsID = goodsID;
		S_Goods_Tmp goodsTmp = GameDataDB.GoodsDB.GetData(goodsID);
		if (goodsTmp == null)
		{
			m_SlotItem.gameObject.SetActive(false);
			Initialize();
			SetGoodStatus(Enum_GoodStatus.NoSell);
			return;
		}
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(goodsTmp.iItemID);
		if (itemTmp == null)
		{
			m_SlotItem.gameObject.SetActive(false);
			Initialize();
			SetGoodStatus(Enum_GoodStatus.NoSell);
			return;
		}
		m_SlotItem.gameObject.SetActive(true);
		//SetInfoAfterBuy(false);
        if(goodsTmp.iName != 0)
		    lbVipGoodsName.text = GameDataDB.GetString(goodsTmp.iName);
		else
            lbVipGoodsName.text = GameDataDB.GetString(itemTmp.iName);
		itemTmp.SetRareColorString(lbVipGoodsName);
		//設定圖示
		m_SlotItem.SetSlotWithCount(itemTmp.GUID , goodsTmp.iGoodsHeap , false);
		//設定廣告
		if (goodsTmp.iGoodsAD > 0)
		{
			string adStr = GameDataDB.GetString(goodsTmp.iGoodsAD);
			lbVipGoodsAD.text = adStr;
			lbVipGoodsAD.gameObject.SetActive(true);
		}
		else
			lbVipGoodsAD.gameObject.SetActive(false);

		if (goodsTmp.iCurrencyType == ENUM_SpendType.ENUM_SpendType_Currency)
		{
			//設定商品價格
			if (goodsTmp.iCurrency == ENUM_CurrencyType.ENUM_CurrencyType_NT)
			{
				lbVipGoodsNTCost.text = string.Format(GameDataDB.GetString(1916) ,goodsTmp.fMoneyGoodsPrize.ToString());	//NT${0}
				lbVipGoodsNTCost.gameObject.SetActive(true); 
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
				lbVipGoodsCost.text = goodsTmp.fMoneyGoodsPrize.ToString();
				lbVipGoodsCost.gameObject.SetActive(true);
			}
		}
		else if (goodsTmp.iCurrencyType == ENUM_SpendType.ENUM_SpendType_Item)
		{
			S_Item_Tmp currencyItemTmp = GameDataDB.ItemDB.GetData(goodsTmp.iCurrencyItemID);
			if (currencyItemTmp != null)
			{
				Utility.ChangeAtlasSprite(spCurrencyType, currencyItemTmp.ItemIcon);
				lbVipGoodsCost.text = goodsTmp.iCurrencyItemCount.ToString();
				lbVipGoodsCost.gameObject.SetActive(true);
			}
		}

		S_Store_Tmp storeTmp = GameDataDB.StoreDB.GetData(storeID);
		if (storeTmp == null)
			return;

		//設定底圖
		Utility.ChangeAtlasSprite(spVipGoodsBG , storeTmp.iUnderlay);

		//資料設定完後顯示Slot
		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//調整Depth
	public void SetSlotDepth(int slotDepth)
	{
		spVipGoodsBG.depth 		+= slotDepth;			//商品背景
		lbVipGoodsName.depth 	+= slotDepth;			//商品名稱
		lbVipGoodsAD.depth 		+= slotDepth;			//商品廣告說明
		spVipGoodsIcon.depth 	+= slotDepth;			//商品圖案
		lbVipGoodsCost.depth	+= slotDepth;			//商品金額
		spCurrencyType.depth	+= slotDepth;			//商品購買貨幣類型: 水晶、寶石、遊戲幣
		lbVipGoodsNTCost.depth 	+= slotDepth;			//商品金額(台幣)
		lbInfoAfterBuy.depth 	+= slotDepth;      		//已購買圖示
	}
	//-------------------------------------------------------------------------------------------------
	//設定商品繼續販售與否
	public void SetGoodStatus(Enum_GoodStatus status)
	{
		switch(status)
		{
		case Enum_GoodStatus.Normal:
			lbInfoAfterBuy.gameObject.SetActive(false);
			btnBuyVipGoods.isEnabled = true;
			break;
		case Enum_GoodStatus.AlreadyBought:
//			if (m_VipGoodsID == GameDefine.ITEMMALL_MONTH_GUID  ||
//			    m_VipGoodsID == GameDefine.ITEMMALL_SEASON_GUID ||
//			    m_VipGoodsID == GameDefine.ITEMMALL_YEAR_GUID)
			lbInfoAfterBuy.text = GameDataDB.GetString(1929);	//"已購買"
			lbInfoAfterBuy.gameObject.SetActive(true);
			btnBuyVipGoods.isEnabled = false;
			break;
		case Enum_GoodStatus.SellOut:
			lbInfoAfterBuy.text = GameDataDB.GetString(1930);	//"已售完"
			lbInfoAfterBuy.gameObject.SetActive(true);
			btnBuyVipGoods.isEnabled = false;
			break;
		case Enum_GoodStatus.NoSell:
			lbInfoAfterBuy.text = GameDataDB.GetString(1935);	//"已下架"
			lbInfoAfterBuy.gameObject.SetActive(true);
			btnBuyVipGoods.isEnabled = false;
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
}
