using System;
using UnityEngine;
using GameFramework;

public class UI_Combo : NGUIChildGUI 
{
	private const string GUI_SMARTOBJECT_NAME = "UI_Combo";
	
	public UILabel uiComboCount = null;
	
	//-----------------------------------------------------------------------------------------------------
	private UI_Combo() : base(GUI_SMARTOBJECT_NAME)
	{		
	}

}
