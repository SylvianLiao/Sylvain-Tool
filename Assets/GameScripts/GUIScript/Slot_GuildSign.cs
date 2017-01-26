using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildSign : NGUIChildGUI 
{
	public	UIWidget		slot				= null;	

	public	UILabel			LabelCostTitle		= null;	//花費標題
	public	UISprite		SpriteCost			= null;	//花費圖案
	public	UILabel			LabelCost 			= null;	//花費金額


	public	UILabel			LabelGetTitle		= null;	//獲得標題
	public	UILabel			LabelGuildGetTitle	= null;	//公會獲得標題
	public	UILabel			LabelGulidGet		= null;	//公會獲得金額

	public	UILabel			LabelPlayerGetTitle	= null;	//會員獲得標題
	public	UILabel			LabelPlayerGet		= null;	//會員獲得金額

	public	UIButton		ButtonSign			= null;	//捐獻按鈕
	public	UISprite		SpriteSign			= null;	//捐獻圖
	public	UILabel			LabelSign			= null;	//捐獻Label	

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildSign";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildSign() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialSlot();
	}
	
	//-------------------------------------------------------------------------------------------------
	public void InitialSlot()
	{
//		LabelCostTitle.text 		= "";
		LabelGetTitle.text 			= GameDataDB.GetString(8093);	//獲得

		LabelGuildGetTitle.text 	= GameDataDB.GetString(8008);	//公會財庫
		LabelPlayerGetTitle.text 	= GameDataDB.GetString(8009);	//公會幣
		LabelSign.text 				= GameDataDB.GetString(8104);	//捐獻
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(GuildSignData data)
	{
		if(data == null)
		{
			slot.gameObject.SetActive(false);
			return ;
		}

		switch(data.m_Type)
		{
		case ENUM_GuildSignType.ENUM_GuildSignType_Low:
			LabelCostTitle.text = GameDataDB.GetString(8094);	//8094 消耗金幣
			break;
		case ENUM_GuildSignType.ENUM_GuildSignType_Middle:
			LabelCostTitle.text = GameDataDB.GetString(8095);	//8095 消耗寶石
			break;
		case ENUM_GuildSignType.ENUM_GuildSignType_High:
			LabelCostTitle.text = GameDataDB.GetString(8095);	//8095 消耗寶石
			break;
		}

		LabelCost.text		= data.m_SignCost.ToString();
		LabelGulidGet.text 	= data.m_GuildGetExp.ToString();
		LabelPlayerGet.text = data.m_MemberGetPoint.ToString();
	}
	
	//-------------------------------------------------------------------------------------------------
}
