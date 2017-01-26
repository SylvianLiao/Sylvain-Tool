using System;
using UnityEngine;
using GameFramework;

public class UI_Joystick : NGUIChildGUI
{	
	public UISprite		spriteBG		= null;
	public UISprite		spriteCenter	= null;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Joystick";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_Joystick() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	//控制顯示/隱藏UI
	public void ShowOrHideUI(bool bSwitch)
	{
		if(bSwitch)
			Show();
		else
			Hide();
	}
}

