using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;


public class UI_Movie : NGUIChildGUI  {

//	public UILabel 		lbRoleName 			= null;
//	public UILabel 		lbTalkContent 		= null;
//	public UIButton 	btnFullScreen 		= null;
	private const string 	GUI_SMARTOBJECT_NAME = "UI_Movie";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Movie() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		//InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
//		lbRoleName.text = GameDataDB.GetString(101017);		//"于小雪"
//		lbTalkContent.text = GameDataDB.GetString(114566);	//"嗚……（陳哥哥……拓拔姊姊……小雪要怎麼做才好……）"
	}

}
