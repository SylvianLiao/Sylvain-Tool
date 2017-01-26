using System.Collections;
using UnityEngine;
using GameFramework;

public class UI_NewRoleTeach : NGUIChildGUI
{
	public UISprite			spriteFingerRight	= null;
	public UISprite			spriteFingerLeft	= null;
	public UILabel			labelFingerRight	= null;
	public UILabel			labelFingerLeft	= null;
	public UISprite			spriteBG			= null;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_NewRoleTeach";

	//-----------------------------------------------------------------------------------------------------
	private UI_NewRoleTeach() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{	
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetFingerLeftText(string text)
	{
		if (null != labelFingerLeft)
			labelFingerLeft.text = text;
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetFingerRightText(string text)
	{
		if (null != labelFingerRight)
			labelFingerRight.text = text;
	}
}