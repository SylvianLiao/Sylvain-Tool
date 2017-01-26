using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class UI_ThirdLogin : NGUIChildGUI 
{

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ThirdLogin";
	
	//-----------------------------------------------------------------------------------------
	private UI_ThirdLogin() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-----------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-----------------------------------------------------------------------------------------
	void InitialUI()
	{
		
	}

}
