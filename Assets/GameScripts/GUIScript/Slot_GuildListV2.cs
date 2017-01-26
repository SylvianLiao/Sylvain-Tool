using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildListV2 : NGUIChildGUI  
{
	//
	public	UIWidget		slot				= null;
	public 	UIButton		ButtonSlot			= null;
	public 	UISprite		SpriteFrame			= null;		//當成選取框用
	
	//GuildListBase
	public	UILabel			LabelRankTitle		= null;
	public	UILabel			LabelRank			= null;
	public	UISprite		SpriteRank			= null;

	public	UILabel			LabelGuildName 		= null;

	public	UILabel			LabelGuildLVTitle	= null;
	public	UILabel			LabelGuildLV 		= null;

	public	UILabel			LabelTotalPower 	= null;

	public	UILabel			LabelGuildMembers 	= null;

	GuildListData	tempGuildListData = null;
	int tempMemberUpperLimit = 999;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildListV2";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildListV2() : base(GUI_SMARTOBJECT_NAME)
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
		//SpriteBG
//		SpriteBG.gameObject.SetActive(false);
		
		
		//GuildListBase
		LabelRankTitle.text		= GameDataDB.GetString(1683);
		LabelRank.text			= "";

		LabelGuildName.text 	= "";

		LabelGuildLVTitle.text	= "";//GameDataDB.GetString(8004);
		LabelGuildLV.text 		= "";

		LabelTotalPower.text 	= String.Format(GameDataDB.GetString(1643), "");//總戰力:{0} 1643
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetGuildListLabel(GuildListData data, int page)
	{
		if(data == null)
		{
			slot.gameObject.SetActive(false);
			return ;
		}
		//公會清單資料
		tempGuildListData		= data;

		//選取框
		SpriteFrame.gameObject.SetActive(false);

		//排名
//		LabelRank.text			= (page*(int)GameDefine.GUILD_PAGE_COUNT + data.iNum+1).ToString();
		LabelRank.text			= data.iNum.ToString();

		if(data.iNum == 1)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 300);
		}
		else if(data.iNum  == 2)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 301);
		}
		else if(data.iNum  == 3)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 302);
		}
		else 
		{
			Utility.ChangeAtlasSprite(SpriteRank, 303);
		}

		//公會名稱
		LabelGuildName.text 	= data.GuildName.ToString();
		//公會等級
		LabelGuildLV.text 		= data.GuildLevel.ToString();
		//總戰力
		LabelTotalPower.text 	= String.Format(GameDataDB.GetString(1643), "12345");//總戰力:{0} 1643
		//公會人數
		LabelGuildMembers.text 	= String.Format("{0}/{1}", 
		                                        data.MemberSize, 
		                                        ARPGApplication.instance.m_GuildSystem.GetMemberUpperLimit(data.GuildLevel));

	}
	
	//-------------------------------------------------------------------------------------------------
	public void ClearSelectMart()
	{
		SpriteFrame.gameObject.SetActive(false);
	}
	
	//-------------------------------------------------------------------------------------------------
	public void ShowSelectMart()
	{
		SpriteFrame.gameObject.SetActive(true);
	}
	
	//-------------------------------------------------------------------------------------------------
	
	
	//-------------------------------------------------------------------------------------------------
	
	//-------------------------------------------------------------------------------------------------
}

