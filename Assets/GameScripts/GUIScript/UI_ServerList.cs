using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

class UI_ServerList : NGUIChildGUI
{
	const	int	rowCount	= 2;
	public 	UIButton 		btnNowServer				= null;	
	public 	UILabel  		LabelNowServer				= null;	
	public	UISprite		SpriteBusy					= null;	
	public 	UIPanel			PanelBase					= null;	
	public 	UIGrid			gridServerList				= null;
	public	UIButton		ButtonClose					= null;
	
	private S_ServerList	nowServerInfo				= null;

	// slot
	string 	slotItemName = "Slot_ServerList";
	public  List<Slot_ServerList> 	slotList 	= new List<Slot_ServerList>();
//	public  List<UIButton> 			btnList 	= new List<UIButton>();
	
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_ServerList";

	//---------------------------------------------------------------------------------------------------
	private UI_ServerList() : base(GUI_SMARTOBJECT_NAME)
	{

	}

	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}

	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		CreatSlot();
	}

	//---------------------------------------------------------------------------------------------------
	private void Start()
	{
		for(int i=0; i<slotList.Count; ++i)
		{

			slotList[i].SetSlotBtn(ARPGApplication.instance.GetServerListByIndex(i*2),
			                       Enum_Slot_ServerList.Left);
			if(ARPGApplication.instance.GetServerListSize()<=i*2+1)
			{
				continue;
			}
			slotList[i].SetSlotBtn(ARPGApplication.instance.GetServerListByIndex(i*2+1),
			                       Enum_Slot_ServerList.Right);

		}

		int worldID = ARPGApplication.instance.serverSevice.tempWorldID;

		SetNowServerButton(worldID);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetNowServerButton(int worldID)
	{
		nowServerInfo = ARPGApplication.instance.GetServerListByWorldID(worldID);
		if(nowServerInfo != null)
		{
			//伺服器名稱
			string str = string.Format("S{0} {1}", nowServerInfo.WorldID, nowServerInfo.Name);
			LabelNowServer.text	= str;
			
			//伺服器忙碌狀態
			SpriteBusy.gameObject.SetActive(true);
			
			switch(nowServerInfo.emBusy)
			{
			case ENUM_ServerBusy_Type.ENUM_ServerBusy_Null:
				//SpriteBusy.gameObject.SetActive(false);
				Utility.ChangeAtlasSprite(SpriteBusy, 54);
				break;
			case ENUM_ServerBusy_Type.ENUM_ServerBusy_Low:
				//				SpriteBusy.color = new Color(1.0f, 0.5f, 0.1f, 1.0f);
				Utility.ChangeAtlasSprite(SpriteBusy, 52);
				break;
			case ENUM_ServerBusy_Type.ENUM_ServerBusy_Normal:
				//				SpriteBusy.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
				Utility.ChangeAtlasSprite(SpriteBusy, 51);
				break;
			case ENUM_ServerBusy_Type.ENUM_ServerBusy_High:
				//				SpriteBusy.color = new Color(0.1f, 1.0f, 0.1f, 1.0f);
				Utility.ChangeAtlasSprite(SpriteBusy, 53);
				break;
			}
			
			btnNowServer.gameObject.SetActive(true);
		}
		else
		{
			btnNowServer.gameObject.SetActive(false);
		}
	}
	//-------------------------------------------------------------------------------------------------
	// 建立SLOT
	void CreatSlot()
	{
		Slot_ServerList go = ResourceManager.Instance.GetGUI(slotItemName).GetComponent<Slot_ServerList>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ServerList load prefeb error,path:{0}", "GUI/"+slotItemName) );
			return;
		}

		int iServerCount = ARPGApplication.instance.GetServerListSize();

		int iMax = (int)Math.Ceiling((double)iServerCount/rowCount);

		for(int i=0; i<iMax; ++i)
		{
			Slot_ServerList slot = Instantiate(go) as Slot_ServerList;

			slot.transform.parent = gridServerList.transform;
			slot.transform.localScale = Vector3.one;		
			slot.transform.localRotation = gridServerList.transform.localRotation;
			slot.transform.localPosition = gridServerList.transform.localPosition;

			slot.name = string.Format("slot{0:00}",i);
			slot.InitialSlot();
			slot.btnList[(int)Enum_Slot_ServerList.Left].userData = i*rowCount;
			slot.btnList[(int)Enum_Slot_ServerList.Right].userData = i*rowCount+1;

			slotList.Add(slot);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public S_ServerList	GetNowServerInfo()
	{
		return nowServerInfo;
	}
		
}
