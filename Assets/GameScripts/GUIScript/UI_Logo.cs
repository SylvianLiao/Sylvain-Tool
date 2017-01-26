using System;
using UnityEngine;
using GameFramework;

public class UI_Logo : NGUIChildGUI
{
	public UIButton BtnLogo = null;
    public UILabel  lbClick = null;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Logo";

	//-----------------------------------------------------------------------------------------------------
	private UI_Logo() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
    public override void Initialize()
    {
        base.Initialize();
        lbClick.text = GameDataDB.GetString(15051); //請點擊開始更新
    }
}

