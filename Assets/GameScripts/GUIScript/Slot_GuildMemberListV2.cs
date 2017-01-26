using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildMemberListV2 : NGUIChildGUI 
{
	public	UIWidget		slotGuildMemberListV2	= null;
	public	UIButton		ButtonMemberSlot		= null;
	public 	UISprite		SpriteSelectMark		= null;

	public	UILabel			LabelMemberRankTitle	= null;	//角色排名標題
	public	UILabel			LabelMemberRank			= null;	//角色排名
	public 	UISprite		SpriteMemberRank		= null;	//角色排名圖
	
	public 	UISprite		SpriteFace				= null;	//角色頭像
	public 	UISprite		SpriteFaceFrame			= null;	//角色外框
	
	public	UILabel			LabelMemberName 		= null;	//角色名稱
	
	public 	UISprite		SpriteMemberPet1		= null;	//寵物頭像1
	public 	UISprite		SpriteMemberPet2		= null;	//寵物頭像2
	
	public	UILabel			LabelMemberLVTitle		= null;	//角色等級標題
	public	UILabel			LabelMemberLV 			= null;	//角色等級
	
	public	UILabel			LabelMemberPower		= null;	//角色戰力
	
	public 	UISprite		SpriteState				= null;	//上線狀態
	public	UILabel			LabelMemberUpdateTime 	= null;
	public	UILabel			LabelLastLoginTime		= null;	//上次登入時間
	
	public	UILabel			LabelMemberJobTitle 	= null;	//角色職稱標題
	public	UILabel			LabelMemberJob		 	= null;	//角色職稱
	
	public	UILabel			LabelMemberScoreTitle	= null;	//角色貢獻標題
	public	UILabel			LabelMemberScore 		= null; //角色貢獻
	
	string title ="";
	S_PetData_Tmp petDBF 	= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildMemberListV2";
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildMemberListV2() : base(GUI_SMARTOBJECT_NAME)
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
//		LabelMemberRankTitle.text	= "";
		LabelMemberRank.text		= "";	//角色排名

		LabelMemberName.text 		= "";	//角色名稱

		LabelMemberLVTitle.text		= GameDataDB.GetString(8004);	//等級
		LabelMemberLV.text 			= "";	//角色等級

		LabelMemberPower.text		= "";	//角色戰力

		LabelMemberUpdateTime.text 	= "";
		LabelLastLoginTime.text		= "";	//上次登入時間

		LabelMemberJobTitle.text 	= GameDataDB.GetString(8051);	//角色職稱
		LabelMemberJob.text		 	= "";	//角色職稱

		LabelMemberScoreTitle.text	= GameDataDB.GetString(8065);	//貢獻度﻿
		LabelMemberScore.text 		= ""; 	//角色貢獻

		SpriteSelectMark.gameObject.SetActive(false);
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetTitleLabel()
	{
		LabelMemberScore.text 		= GameDataDB.GetString(1655);	//貢獻 1655
		LabelMemberName.text 		= GameDataDB.GetString(1679);	//名稱 1679
		LabelMemberLV.text 			= GameDataDB.GetString(1680);	//等級 1680
		LabelMemberJobTitle.text 	= GameDataDB.GetString(1653);	//職位 1653
		LabelMemberUpdateTime.text 	= GameDataDB.GetString(1681);	//狀態 1681
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetLabel(string Score, string Name, string LV, string JobTitle, string UpdateTime)
	{
		LabelMemberScore.text 		= Score;
		LabelMemberName.text 		= Name;
		LabelMemberLV.text 			= LV;
		LabelMemberJobTitle.text 	= JobTitle;
		LabelMemberUpdateTime.text 	= UpdateTime;
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMemberListLabel(GuildMemberData data)
	{
		//角色排名
		LabelMemberRank.text	= "";	//角色排名
		//頭像
		Utility.ChangeAtlasSprite(SpriteFace, data.MemberIcon);
		//角色名
		LabelMemberName.text 	= data.Name;
		
		//寵物頭像1		
		petDBF = GameDataDB.PetDB.GetData(data.Pet1DBID);
		if(petDBF != null)
		{
			Utility.ChangeAtlasSprite(SpriteMemberPet1, petDBF.AvatarIcon);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteMemberPet1, -1);
		}

		//寵物頭像2
		petDBF = GameDataDB.PetDB.GetData(data.Pet2DBID);
		if(petDBF != null)
		{
			Utility.ChangeAtlasSprite(SpriteMemberPet2, petDBF.AvatarIcon);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteMemberPet2, -1);
		}

		//等級
		LabelMemberLV.text 		= data.MemberLv.ToString();
		//戰力
		LabelMemberPower.text 	= string.Format("{0}", data.MemberPower);

		//上線狀態
		//登入狀態
		if(data.isOnline)
		{
			Utility.ChangeAtlasSprite(SpriteState, 105);	//上線圖
			LabelLastLoginTime.gameObject.SetActive(false);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteState, 106);	//離線圖
			LabelLastLoginTime.gameObject.SetActive(true);
			//60*60*24
			if(data.LoginTime >= 86400)
			{
				LabelLastLoginTime.text = string.Format(GameDataDB.GetString(5209), (int)(data.LoginTime/86400));	//{0}天
			}
			else if(data.LoginTime > 3600 && data.LoginTime < 86400)
			{
				LabelLastLoginTime.text = string.Format(GameDataDB.GetString(5210), (int)(data.LoginTime/3600));	//{0}小時
			}
			else if(data.LoginTime > 0 && data.LoginTime <= 3600)
			{
				LabelLastLoginTime.text = GameDataDB.GetString(5211);	//剛剛
			}
		}


		//貢獻
		LabelMemberScore.text 	= data.MemberScore.ToString();
		
		//角色職稱
		title ="";
		switch((EnumGuildMemberPowerLevel)data.GuildPowerLevel)
		{
		case EnumGuildMemberPowerLevel.EGMPL_Member:
			title = GameDataDB.GetString(8040);	// 成員
			break;
		case EnumGuildMemberPowerLevel.EGMPL_Elder: //副會長
			title = GameDataDB.GetString(8038);
			break;
		case EnumGuildMemberPowerLevel.EGMPL_Leader:	//會長
			title = GameDataDB.GetString(8037);
			break;
		}
		LabelMemberJob.text = title;
	}
	
	//-------------------------------------------------------------------------------------------------
	public void ClearSelectMark()
	{
		SpriteSelectMark.gameObject.SetActive(false);
	}
	
	//-------------------------------------------------------------------------------------------------
	public void ShowSelectMarK()
	{
		SpriteSelectMark.gameObject.SetActive(true);
	}

	//-------------------------------------------------------------------------------------------------
}
