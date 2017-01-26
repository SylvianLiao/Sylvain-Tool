using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_GuildSign : NGUIChildGUI
{
	public UILabel			LabelGuildControlTitle		= null; //捐獻介面標題
	public UIButton			ButtonGuildSginClose		= null; //

	public UILabel			LabelGuildMoneyTitle		= null; //公會財庫標題
	public UILabel			LabelGuildMoney			    = null;	//公會財庫
	public UILabel			LabelPlayerSignTitle		= null;	//我的捐獻標題
	public UILabel			LabelPlayerSign	        	= null;	//我的捐獻
	public UILabel  		LabelNowGameMoneyTitle		= null;	//金幣標題
	public UILabel  		LabelNowGameMoney          	= null;	//金幣
	public UILabel  		LabelNowDiamondTitle		= null;	//寶石標題
	public UILabel  		LabelNowDiamond           	= null;	//寶石
    public UILabel  		LabelInfo1                  = null;	//說明1
    public UILabel  		LabelInfo2                  = null;	//說明2
	public UILabel  		LabelInfo3					= null;	//說明3
	public UILabel  		LabelInfo4					= null;	//說明4
		
	public List<Slot_GuildSign>		slotGuildSign	= new List<Slot_GuildSign>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_GuildSign";
	
	//-------------------------------------------------------------------------------------------------
	private UI_GuildSign() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		LabelGuildControlTitle.text 	= GameDataDB.GetString(8104);	//捐獻
		LabelGuildMoneyTitle.text 		= GameDataDB.GetString(8008);	//公會財庫	  
		LabelPlayerSignTitle.text 		= GameDataDB.GetString(8009);  	//公會幣
		LabelNowGameMoneyTitle.text 	= GameDataDB.GetString(10002);	//金幣  
		LabelNowDiamondTitle.text 		= GameDataDB.GetString(10001);	//寶石
		LabelInfo1.text 				= GameDataDB.GetString(8088);	//※捐獻越多，公會資金獲取速度越快         
		LabelInfo2.text 				= GameDataDB.GetString(8087);	//※每日可捐獻1次，每日04:00重置次數
		LabelInfo3.text 				= GameDataDB.GetString(8085);	//※公會財庫是建築升級所需資金
		LabelInfo4.text 				= GameDataDB.GetString(8086);	//※公會幣可至公會商店購買商品

		for(int i=0; i<slotGuildSign.Count; ++i)
		{
			slotGuildSign[i].InitialSlot();
			slotGuildSign[i].SetSlot(ARPGApplication.instance.m_GuildSystem.GetGuildSignData(i));
			slotGuildSign[i].ButtonSign.userData = i;
		}
	}

	//-------------------------------------------------------------------------------------------------
//	public void SetGuildSignInfo(int Guildmoney, int PlayerGuildmoney, int NowGameMoney,int NowDiamond)
//  {
//        LabelGuildMoney.text	   = Guildmoney.ToString();	  
//        LabelPlayerSign.text	   = PlayerGuildmoney.ToString();	  
//        LabelNowGameMoney.text     = NowGameMoney.ToString();	 
//       LabelNowDiamond.text       = NowDiamond.ToString();	  
//	}

	//-------------------------------------------------------------------------------------------------
	public void SetGuildSignInfo()
	{
		GuildBaseData data = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();

		//公會財庫
		LabelGuildMoney.text	= data.GuildExp.ToString();
		//我的捐獻
		LabelPlayerSign.text	= ARPGApplication.instance.m_RoleSystem.iMemberMoney.ToString();
		//金幣
		LabelNowGameMoney.text	= ARPGApplication.instance.m_RoleSystem.iBaseBodyMoney.ToString();
		//寶石
		LabelNowDiamond.text	= ARPGApplication.instance.m_RoleSystem.iBaseItemMallMoney.ToString();
	}
	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
