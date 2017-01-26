using System;
using UnityEngine;
using GameFramework;

public class UI_BeginEndScreen : NGUIChildGUI
{
	//Common component
	public UITexture	texBlackBackGround 	= null; //背景黑圖

	public UISprite		spriteTipSprite		= null; //開始提示圖
	public UISprite		spriteTipBG			= null;	//提示圖底圖
	public UIButton		btnLeave			= null; //關閉按鈕
	public UITexture	texGuideTexture		= null;	//導引圖
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_BeginEndScreen";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_BeginEndScreen() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	void Start()
	{
	}
}