using UnityEngine;
using System.Collections;
using GameFramework;

public class UI_OKCancelBox : NGUIChildGUI
{
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UIOKCancelBox";

	public UILabel 		LabelMessage		= null;
	public UIButton		ButtonOK			= null;
	public UIButton 	ButtonCancel 		= null;
	public UILabel 		LabelButtonOK		= null;
	public UILabel 		LabelButtonCancel	= null;
	//教學用
	public UISprite 	SpriteGuideOK		= null;
	public UILabel 		LabelGuideOK		= null;

//---------------------------------------------------------------------------------------------
	private UI_OKCancelBox() : base(GUI_SMARTOBJECT_NAME)
	{
		
	}

	public void OnCloseBtn()
	{

	}
}
