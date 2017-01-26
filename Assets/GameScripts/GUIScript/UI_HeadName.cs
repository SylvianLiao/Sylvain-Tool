using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_HeadName : NGUIChildGUI
{
    public static UI_HeadName Instance;

    // smartObjectName
    private const string GUI_SMARTOBJECT_NAME = "UI_HeadName";

	//-----------------------------------------------------------------------------------------------------
    private UI_HeadName()
        : base(GUI_SMARTOBJECT_NAME)
	{
	}

    void Awake() 
    {
        if (Instance == null)
            Instance = this; 
    }

}
