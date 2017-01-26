using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_FunctionMessage : NGUIChildGUI  
{
	//功能性提示
	public UISprite			spFunctionMsg			= null; //功能性提示圖
	public UILabel			lbFunctionMsg			= null; //功能提示說明文
	public UIButton			btnFullScreen			= null; //全螢幕按鈕

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_FunctionMessage";
	
	//-------------------------------------------------------------------------------------------------
	private UI_FunctionMessage() : base(GUI_SMARTOBJECT_NAME)
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
		lbFunctionMsg.text = "";
	}

}
