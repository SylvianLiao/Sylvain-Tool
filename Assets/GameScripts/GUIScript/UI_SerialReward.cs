using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_SerialReward : NGUIChildGUI {

	//-----------------------------------------------------
	//序號領獎
	public UIPanel basePanel 		= null;
	public UILabel lbSerTitle		= null;	//序號領取TT
	public UILabel lbSerConteact	= null;	//序號領取說明
	public UILabel lbBtnGet			= null;	//按鈕文字
	public UIButton btnGet			= null;
	public UIButton btnClose		= null;
									
	public UIInput ipSerial			= null;


	private const string GUI_SMARTOBJECT_NAME = "UI_SerialReward";
	//-----------------------------------------------------------------------------------------------------
	private UI_SerialReward() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Hide ()
	{
		base.Hide ();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitializeLabel();
	}
	//-----------------------------------------------------------------------------------------------------
	void Update () 
	{
	
	}
	//-----------------------------------------------------------------------------------------------------
	void InitializeLabel()
	{
		//序號領獎
		lbSerTitle.text		= GameDataDB.GetString(9760);	//序號領獎
		lbSerConteact.text	= GameDataDB.GetString(9761);	//來來來！輸入遊戲序號拿獎勵！
		lbBtnGet.text		= GameDataDB.GetString(9763);	//領取…
		ipSerial.label.text = GameDataDB.GetString(9762);	//請在此輸入序號…
	}

}
