using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_RoleIcon : NGUIChildGUI  
{
    public UIWidget			WidgetSlot			= null;
    public UIButton			ButtonSlot			= null;
	public UISprite			SpriteBG			= null; 
	public UISprite			SpriteRoleIcon		= null; 

	//public int 				itemGUID 			= -1;	//道具編號	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_RoleIcon";

	//-------------------------------------------------------------------------------------------------
	private Slot_RoleIcon() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot(int iconGuid, int FrameGuid)
	{
		Utility.ChangeAtlasSprite(SpriteRoleIcon, iconGuid);

		//設定框
       	Utility.ChangeAtlasSprite(SpriteBG, FrameGuid);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetDepth(int depth)
	{
		WidgetSlot.depth		+= depth;
		SpriteBG.depth			+= depth; 	
		SpriteRoleIcon.depth	+= depth;	
	}
}
