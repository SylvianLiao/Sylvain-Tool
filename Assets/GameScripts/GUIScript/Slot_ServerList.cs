using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum Enum_Slot_ServerList
{
	Left = 0,
	Right,
	
	Max
};

public class Slot_ServerList : NGUIChildGUI  
{
	//
	public  UIWidget		WidgetBase			= null;
	public	List<UIButton>	btnList				= new List<UIButton>();
//	public 	UIButton		ButtonLeft			= null;
//	public 	UIButton		ButtonRight			= null;
	public	List<UILabel>	labList				= new List<UILabel>();
//	public 	UILabel 		LabelLeft			= null;
//	public 	UILabel 		LabelRight			= null;
	public	List<UISprite>	spriteList			= new List<UISprite>();
//	public	UISprite		SpriteBusyL			= null;
//	public	UISprite		SpriteBusyR			= null;

	List<S_ServerList>	serverInfo	= new List<S_ServerList>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_ServerList";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_ServerList() : base(GUI_SMARTOBJECT_NAME)
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
		for(int i=0; i<(int)Enum_Slot_ServerList.Max; ++i)
		{
			btnList[i].gameObject.SetActive(false);
			labList[i].text 	= "";
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlotData(S_ServerList data)
	{
		if(serverInfo.Count <= (int)Enum_Slot_ServerList.Max)
		{
			serverInfo.Add(data);
		}
		else
		{
			UnityDebugger.Debugger.LogError("Slot_ServerList Add data Error");
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlotBtn(S_ServerList data, Enum_Slot_ServerList type)
	{
		//伺服器名稱
		string str = string.Format("S{0} {1}", data.WorldID, data.Name);
		labList[(int)type].text	= str;

		//伺服器忙碌狀態
		spriteList[(int)type].gameObject.SetActive(true);

		switch(data.emBusy)
		{
		case ENUM_ServerBusy_Type.ENUM_ServerBusy_Null:
//			spriteList[(int)type].gameObject.SetActive(false);
			Utility.ChangeAtlasSprite(spriteList[(int)type], 54);	//伺服器因為爆炸而睡覺
			break;
		case ENUM_ServerBusy_Type.ENUM_ServerBusy_Low:
//			spriteList[(int)type].color = new Color(1.0f, 0.5f, 0.1f, 1.0f);
			Utility.ChangeAtlasSprite(spriteList[(int)type], 52);	//伺服器擁擠
			break;
		case ENUM_ServerBusy_Type.ENUM_ServerBusy_Normal:
//			spriteList[(int)type].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			Utility.ChangeAtlasSprite(spriteList[(int)type], 51);	//伺服器開心
			break;
		case ENUM_ServerBusy_Type.ENUM_ServerBusy_High:
//			spriteList[(int)type].color = new Color(0.1f, 1.0f, 0.1f, 1.0f);
			Utility.ChangeAtlasSprite(spriteList[(int)type], 53);	//伺服器爆炸
			break;
		}

		btnList[(int)type].gameObject.SetActive(true);
	}
}
