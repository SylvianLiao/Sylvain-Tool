using System;
using UnityEngine;
using GameFramework;

public class UI_PreviewMatList : NGUIChildGUI 
{
	public UIPanel		panelBase				= null;		//預覽素材介面
	public UIPanel		panelScrollPreviewList	= null;		//可捲動的區域
	public UIButton		btnEnterSellUI			= null;		//進入出售UI介面
	public UILabel		lbEnterSellUI			= null;		//出售UI文字
	public PetMatPopulator	ScrollList			= null;		//可捲動區域

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_PreviewMatList";
	//-----------------------------------------------------------------------------------------------------
	private UI_PreviewMatList() : base(GUI_SMARTOBJECT_NAME)
	{		
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
	void Start()
	{
		lbEnterSellUI.text = GameDataDB.GetString(1501);		//"進入出售介面"
	}
	//-----------------------------------------------------------------------------------------------------
}
