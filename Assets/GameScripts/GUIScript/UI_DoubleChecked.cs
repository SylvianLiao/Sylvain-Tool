using System;
using UnityEngine;
using GameFramework;

public class UI_DoubleChecked : NGUIChildGUI
{
	//Common component
	public UILabel	lbCheckedContent	= null; //確認內容
	//SingleBtnSet
	public UIPanel	panelSingleBtnSet	= null;	//單一按鈕確認頁面
	public UIButton	btnOneChecked		= null; //單一按鈕
	public UILabel	lbOneChecked		= null; //按鈕內文
	//TwoBtnSet
	public UIPanel	panelTwoBtnSet		= null; //兩個按鈕確認頁面
	public UIButton	btnChecked			= null; //確認按鈕
	public UIButton btnCancel			= null; //取消按鈕
	public UILabel	lbChecked			= null; //確認按鈕內文
	public UILabel	lbCancel			= null; //取消按鈕內文

	
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_DoubleChecked";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_DoubleChecked() : base(GUI_SMARTOBJECT_NAME)
	{		
	}

	void Start()
	{

	}
}

