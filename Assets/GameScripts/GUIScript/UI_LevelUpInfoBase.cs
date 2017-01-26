using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_LevelUpInfoBase : NGUIChildGUI 
{
	public string			LevelUpBGMusic = "Sound_System_017";	//升級背景音樂 
	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_LevelUpInfoBase";
	
	//-------------------------------------------------------------------------------------------------
	private UI_LevelUpInfoBase() : base(GUI_SMARTOBJECT_NAME)
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
		
	}
	
}
