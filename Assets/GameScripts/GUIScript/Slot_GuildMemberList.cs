using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildMemberList : NGUIChildGUI 
{
	public	UILabel			LabelMemberScore 		= null;
	public	UILabel			LabelMemberName 		= null;
	public	UILabel			LabelMemberLV 			= null;
	public	UILabel			LabelMemberJobTitle 	= null;
	public	UILabel			LabelMemberUpdateTime 	= null;

	public	UIButton		ButtonMember			= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildList";

	//-------------------------------------------------------------------------------------------------
	private Slot_GuildMemberList() : base(GUI_SMARTOBJECT_NAME)
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
		LabelMemberScore.text 		= "";
		LabelMemberName.text 		= "";
		LabelMemberLV.text 			= "";
		LabelMemberJobTitle.text 	= "";
		LabelMemberUpdateTime.text 	= "";
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
}
