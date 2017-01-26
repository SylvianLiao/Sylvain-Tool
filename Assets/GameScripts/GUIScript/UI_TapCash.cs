using System;
using UnityEngine;
using GameFramework;

public class UI_TapCash : NGUIChildGUI
{
	//public UIPanel		panelBase				= null; 			//搖錢樹
	public UILabel		lbTapCashTitle			= null;				//搖錢樹標題
	public UISprite		spriteDiamond			= null;				//商城幣圖示
	public UIButton		btnClose				= null;				//關閉搖錢樹UI
	public UIButton		btnTapCashTree			= null;				//搖錢樹按鈕
	public GameObject	TreeSFXContainer		= null;				//搖錢樹效果
	public UILabel[]	lbCostTip				= new UILabel[4];
    public UILabel		lbMoneyValue			= null;				//搖錢樹遊戲幣
    public UILabel		lbGoldValue	    		= null;				//搖錢樹鑽石
	//
	public GameObject	TwiceEffect				= null;				//得到加倍的效果
	//[HideInInspector]
	//public UI_TopStateView		uiTopStateView				= null;		//資訊列
	//-----------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_TapCash";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_TapCash() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		
		/*if(uiTopStateView != null)
		{
			//重新設定Depath
			uiTopStateView.panelBase.depth = panelBase.depth + 800;
			uiTopStateView.Show();
		}*/
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		/*if(uiTopStateView!=null)
			uiTopStateView.Hide();*/
	}
	//-----------------------------------------------------------------------------------------------------
	private void Start()
	{
		lbTapCashTitle.text = GameDataDB.GetString(1903);		//"搖錢樹"
		UpdateLabelBuyCash();
	}
	//-----------------------------------------------------------------------------------------------------
	public void UpdateLabelBuyCash()
	{
		lbTapCashTitle.text = GameDataDB.GetString(1903);		//"搖錢樹"
		S_ShopPrize_Tmp shopTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_BUY_MONEY_ID);
		spriteDiamond.gameObject.SetActive(true);
		if (shopTmp == null)
			return;
		int	buyCashCost = shopTmp.GetPrize(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iBuyMoneyTreeCount);
		if (buyCashCost < 0)
			return;
		//lbExplanation.text	= buyCashCost.ToString()+GameDataDB.GetString(1904)+GameDefine.ITEMMALL_BUYMONEY_EACH_MONEY;		//"換遊戲幣"
		//lbExplanation.text	= GameDataDB.GetString(460)+buyCashCost.ToString()+GameDataDB.GetString(462)+GameDefine.ITEMMALL_BUYMONEY_EACH_MONEY+GameDataDB.GetString(463);		//"換遊戲幣"
		lbCostTip[0].text = GameDataDB.GetString(460);
		lbCostTip[1].text = buyCashCost.ToString()+GameDataDB.GetString(462);
		lbCostTip[2].text = GameDefine.ITEMMALL_BUYMONEY_EACH_MONEY.ToString();
		lbCostTip[3].text = GameDataDB.GetString(463);
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
}
