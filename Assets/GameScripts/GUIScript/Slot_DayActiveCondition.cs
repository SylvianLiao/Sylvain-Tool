using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class Slot_DayActiveCondition : NGUIChildGUI  
{
	//
	public 	UIButton		ButtonGoto			    = null;
    public 	UILabel 		LabelGoto			    = null;
    public 	UILabel 		LabelCount			    = null;
	public 	UILabel 		LabelConditionContent   = null;
	public 	UILabel 		LabelPoint			    = null;
	public  UISprite		SpriteProgressBar2	    = null;	
    public  UISprite		SpriteBG	            = null;	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_DayActiveCondition";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_DayActiveCondition() : base(GUI_SMARTOBJECT_NAME)
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
		LabelGoto.text 		= "";
		LabelCount.text 	= "";
        LabelConditionContent.text 	= "";
        LabelPoint.text 	= "";
	}

}
