using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildList : NGUIChildGUI  
{
	//
	public 	UIButton		ButtonSlot			= null;
	public 	UISprite		SpriteBG			= null;		//當成選取框用

	//GuildListBase
	public	UIWidget		GuildListBase		= null;
//	public	UILabel			LabelGuildID 		= null;
	public	UILabel			LabelGuildLV 		= null;
	public	UILabel			LabelGuildName 		= null;
//	public	UILabel			LabelGuildMembers 	= null;
	public	UILabel			LabelTotalPower 	= null;
	
	//MemberListBase
	public	UIWidget		MemberListBase		= null;
	public	UILabel			LabelMemberLV 		= null;
	public	UILabel			LabelMemberName 	= null;
	public	UILabel			LabelMemberPower 	= null;
	public	UILabel			LabelMemberJobTitle = null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildList";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildList() : base(GUI_SMARTOBJECT_NAME)
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
		SpriteBG.gameObject.SetActive(false);


		//GuildListBase
		GuildListBase.gameObject.SetActive(false);
		LabelGuildLV.text 		= "";
		LabelGuildName.text 	= "";
		LabelTotalPower.text 	= String.Format(GameDataDB.GetString(1643), "");//戰力:{0} 1643

		//MemberListBase
		MemberListBase.gameObject.SetActive(false);
		LabelMemberLV.text 		= "";
		LabelMemberName.text 	= "";
		LabelMemberPower.text 	= String.Format(GameDataDB.GetString(1643), "");
		LabelMemberJobTitle.text 	= "";
	}

	//-------------------------------------------------------------------------------------------------
	public void SetGuildListLabel(JSONPG_MtoC_GetGuildListData data)
	{
		GuildListBase.gameObject.SetActive(true);
		MemberListBase.gameObject.SetActive(false);
		SpriteBG.gameObject.SetActive(false);

//		LabelGuildID.text 		= id;
		LabelGuildLV.text 		= data.GuildLevel.ToString();
		LabelGuildName.text 	= data.GuildName.ToString();
//		LabelGuildMembers.text 	= count;
		LabelTotalPower.text 	= String.Format(GameDataDB.GetString(1643), "");//戰力:{0} 1643
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMemberListLabel(JSONPG_MtoC_GuildMemberData data)
	{
		GuildListBase.gameObject.SetActive(false);
		MemberListBase.gameObject.SetActive(true);
		SpriteBG.gameObject.SetActive(false);

		LabelMemberLV.text 		= data.MemberLv.ToString();
		LabelMemberName.text 	= data.Name;
		LabelMemberPower.text 	= string.Format(GameDataDB.GetString(1643), data.MemberPower);	//戰力:{0}

		string title ="";
		switch((EnumGuildMemberPowerLevel)data.GuildPowerLevel)
		{
		case EnumGuildMemberPowerLevel.EGMPL_Member:
				break;
		case EnumGuildMemberPowerLevel.EGMPL_Elder: //副會長 1649
			title = GameDataDB.GetString(1649);
				break;
		case EnumGuildMemberPowerLevel.EGMPL_Leader:	//會長 1648
			title = GameDataDB.GetString(1648);
				break;
		}
		LabelMemberJobTitle.text = title;
	}

	//-------------------------------------------------------------------------------------------------
	public void ClearSelectMart()
	{
		SpriteBG.gameObject.SetActive(false);
	}

	//-------------------------------------------------------------------------------------------------
	public void ShowSelectMart()
	{
		SpriteBG.gameObject.SetActive(true);
	}

	//-------------------------------------------------------------------------------------------------


	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
