using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildBattlefield : NGUIChildGUI  
{
	public	Transform		slotGuildBattlefield	= null;

	public	UISprite		SpritePetIconBG			= null;
	public	UISprite		SpritePetIcon			= null;
	public	UISprite		SpriteMask				= null;

//	public string	slotName 		= "Slot_Friends";
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildBattlefield";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildBattlefield() : base(GUI_SMARTOBJECT_NAME)
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
		
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(SimpleFriendData data, UI_FRIENDS_PAGE type, int value)
	{
		
		
	}
	
	//-------------------------------------------------------------------------------------------------
}
