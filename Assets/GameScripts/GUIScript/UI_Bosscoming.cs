using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class UI_Bosscoming : NGUIChildGUI
{
	public UIPanel		BossComeType2			= null;	//Boss登場
	public UIPanel		BossComeType3			= null;	//Boss登場

	public UIButton     m_btnSkip                   = null; //跳過劇情對話內容
	public UILabel      m_lbSkip                    = null; //跳過劇情

	//public UIButton					btnLoseCheck						= null;
	//
	// smartObjectName
	private const string 			GUI_SMARTOBJECT_NAME 				= "UI_Bosscoming";

	//-----------------------------------------------------------------------------------------------------
	private UI_Bosscoming() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		if (m_lbSkip)
		{
			m_lbSkip.text = GameDataDB.GetString(541);	//"跳過"
		}
	}

	public void SetSkipBtnVisible(bool bVisible)
	{
		if (m_btnSkip)
		{
			m_btnSkip.gameObject.SetActive(bVisible);
		}
	}

	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	public void showBossCome(bool bShow, int type, float duration)
	{
		UIPanel BossCome = null;
		switch(type)
		{
		case 2:	BossCome = BossComeType2;	break;
		case 3:	BossCome = BossComeType3;	break;
		}
		EnableFalse enableFalseEffect = BossCome.gameObject.AddComponent<EnableFalse>();
		enableFalseEffect.duration = duration;
		BossCome.gameObject.SetActive( bShow );
	}
}
