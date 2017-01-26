using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

class UI_StrengthenBox : NGUIChildGUI 
{
	
	public UIPanel					panelScrollViewStrengthensView		= null;
	//
	public UILabel					lbStrengthenBoxTitle				= null;
	public UIButton					btnClose							= null;

	//public UISprite					spriteStrengthenBody				= null;
	public UISprite[]				spriteSlots							= new UISprite[4];
	public UIButton[] 				btnStrengthenList					= new UIButton[4];
	public UILabel[] 				lbStrengthenSentences 				= new UILabel[4];
	public UILabel[] 				lbStrengthenTitles 					= new UILabel[4];

	//
	// smartObjectName
	private const string 			GUI_SMARTOBJECT_NAME 				= "UI_StrengthenBox";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_StrengthenBox() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		panelScrollViewStrengthensView.GetComponent<UIScrollView>().ResetPosition();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	void Awake()
	{
		//初始先設定ScrollView為false
		//panelScrollViewStrengthensView.GetComponent<UIScrollView>().enabled = false;
	}
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		lbStrengthenBoxTitle.text 		= GameDataDB.GetString(1950);		//"我要變強"
		lbStrengthenSentences[0].text 	= GameDataDB.GetString(2501);		//"換上更強的裝備"
		lbStrengthenSentences[1].text 	= GameDataDB.GetString(2502);		//"召喚更強的夥伴"
		lbStrengthenSentences[2].text 	= GameDataDB.GetString(2503);		//"讓夥伴變得更強大"
		lbStrengthenSentences[3].text 	= GameDataDB.GetString(2507);		//"學習更強的技能"
		lbStrengthenTitles[0].text		= GameDataDB.GetString(2504);		//"裝備"
		lbStrengthenTitles[1].text		= GameDataDB.GetString(2505);		//"召喚"
		lbStrengthenTitles[2].text		= GameDataDB.GetString(2506);		//"煉化"
		lbStrengthenTitles[3].text		= GameDataDB.GetString(2726);		//"天賦"
	}
	//-----------------------------------------------------------------------------------------------------
}
