using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_History : NGUIChildGUI 
{
	public UIWidget			slotHistory			= null;
	public UISprite			SpriteMsgTitleBG	= null;
	public UILabel			LabelTime			= null;
	public UILabel			LabelEven			= null;
	public UISprite			SpriteNew			= null;

	ENUM_UI_History_Type 	m_SlotType			= ENUM_UI_History_Type.PeakArena;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_History";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_History() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialSlot();
	}

	//-------------------------------------------------------------------------------------------------
	public void InitialSlot()
	{
		LabelTime.text		= "";
		LabelEven.text		= "";
		SpriteNew.gameObject.SetActive(false);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_HistoryLog data, ulong serial)
	{
		LabelTime.text		= data.tEventTime.ToString("yyyy/MM/dd HH:mm");
		LabelEven.text		= data.strLog;

/*		if(data.ui64Serial > ARPGApplication.instance.m_ActivityMgrSystem.GetLastLogSerial())
		{
			SpriteNew.gameObject.SetActive(true);
		}
		else
		{
			SpriteNew.gameObject.SetActive(false);
		}
*/
		if(data.ui64Serial > serial)
		{
			SpriteNew.gameObject.SetActive(true);
		}
		else
		{
			SpriteNew.gameObject.SetActive(false);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetUIType(ENUM_UI_History_Type type)
	{
		m_SlotType = type;
	}
}
