using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Territory : NGUIChildGUI  
{
	public UIButton	 		ButtonTerritoryClose	= null;
	public UILabel			LabelTerritoryTitle		= null;
	public UIScrollView		ScrollViewTerritory		= null;

	public List<Slot_GuildBuildingSetting>	slotGuildBuildingInfo	= new List<Slot_GuildBuildingSetting>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_Territory";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Territory() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		LabelTerritoryTitle.text = GameDataDB.GetString(8068);	//建築狀態

		int i=0;
		for(i=0; i<slotGuildBuildingInfo.Count; ++i)
		{
			slotGuildBuildingInfo[i].InitialSlot();
			
		}	
	}

	//-------------------------------------------------------------------------------------------------
	public void SetGuildBuildingInfo()
	{
		GuildBaseData data = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if(data == null)
		{
			return;
		}

		// 公會建設設定
		for(int i=0; i<slotGuildBuildingInfo.Count; ++i)
		{
			//有開放
			if(i < data.BuildingLevel.Length && i!=GameDefine.GUILD_BUILDING_BOSS)
			{
				slotGuildBuildingInfo[i].ButtonGuildBuilding.userData = i;
				slotGuildBuildingInfo[i].SetSlot(data, data.BuildingLevel[i], false);
				slotGuildBuildingInfo[i].gameObject.SetActive(true);
			}
			else
			{
				slotGuildBuildingInfo[i].gameObject.SetActive(false);
			}
		}

//		ScrollViewTerritory.ResetPosition();
//		ScrollViewTerritory.UpdatePosition();
	}
}
