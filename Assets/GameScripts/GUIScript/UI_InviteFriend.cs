using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_InviteFriend : NGUIChildGUI 
{
	public UILabel				LabelRoleInfoTitle		= null;
	public UISprite				SpriteRoleInfoIcon		= null;
	public UISprite				Spriteframe				= null;
//	public UILabel				LabelRoleInfoView		= null;
//	public UIButton				ButtonRoleInfoView		= null;
	public UILabel				LabelRoleInfoNameTitle	= null;
	public UILabel				LabelRoleInfoName		= null;
	public UILabel				LabelRoleInfoPowerTitle	= null;
	public UILabel				LabelRoleInfoPower		= null;
	public UILabel				LabelRoleInfoLVTitle	= null;
	public UILabel				LabelRoleInfoLV			= null;
	public UILabel				LabelRoleInfoNumberTitle	= null;
	public UILabel				LabelRoleInfoNumber			= null;

	public UILabel				LabelInviteFriend		= null;
	public UIButton				ButtonInviteFriend		= null;
	
	public UIButton				ButtonRoleInfoClose		= null;

	public UILabel				LabelInviteFriendNote	= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_InviteFriend";
	
	//-------------------------------------------------------------------------------------------------
	private UI_InviteFriend() : base(GUI_SMARTOBJECT_NAME)
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
		LabelRoleInfoTitle.text			= GameDataDB.GetString(5219);	//角色資訊

		LabelRoleInfoNameTitle.text		= GameDataDB.GetString(5222);	//名稱
		LabelRoleInfoPowerTitle.text	= GameDataDB.GetString(5202);	//戰力	
		LabelRoleInfoLVTitle.text		= GameDataDB.GetString(5201);	//等級
		LabelRoleInfoNumberTitle.text	= GameDataDB.GetString(5223);	//ID

		LabelInviteFriend.text			= GameDataDB.GetString(5406);	//邀請好友
		LabelInviteFriendNote.text		= GameDataDB.GetString(5407);	//是否要邀請為好友呢？
	}
	
	//-------------------------------------------------------------------------------------------------
	public void ShowRoleInfo(SimpleTeammateData data)
	{
		//換頭像
		Utility.ChangeAtlasSprite(SpriteRoleInfoIcon, data.simpleData.m_iFace);
		//換頭像外框
		Utility.ChangeAtlasSprite(Spriteframe, data.simpleData.m_iFaceFrameID);
		
		//名稱
		LabelRoleInfoName.text		= data.simpleData.m_strRoleName;
		//戰力
		LabelRoleInfoPower.text		= data.simpleData.m_iPower.ToString();
		//等級
		LabelRoleInfoLV.text		= data.simpleData.m_iLevel.ToString();
		//ID
		LabelRoleInfoNumber.text	= data.simpleData.m_iRoleID.ToString();
		
	}

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
