using System;
using System.Collections;
using UnityEngine;
using GameFramework;

public class UI_CommandSync : NGUIChildGUI 
{
	public UILabel	LabelMessage;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_CommandSync";
	//-------------------------------------------------------------------------------------------------------------
	private UI_CommandSync() : base(GUI_SMARTOBJECT_NAME)
	{
	}

	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();

		if (LabelMessage)
			LabelMessage.text = GameDataDB.GetString(15082);
	}

}
