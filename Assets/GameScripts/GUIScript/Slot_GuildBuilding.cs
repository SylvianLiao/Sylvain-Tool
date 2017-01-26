using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildBuilding : NGUIChildGUI  
{
	public	UIWidget		slotBuilding				= null;	//slot
	public	UIButton		ButtonGuildBuilding			= null;	//按鈕
	public	UISprite		SpriteGuildBuildingIcon		= null;	//建築圖
	public	UILabel			LabelName					= null;	//建築名字
	public	UISprite		SpriteWait					= null;	//還沒開放圖
	public	UISprite		SpriteLevel					= null;	//等級

	public	int				index	= 0;		//讀字串之類用的
	private	bool 			isOpen	= false;	//有沒有開放

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildBuilding";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildBuilding() : base(GUI_SMARTOBJECT_NAME)
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
		SpriteWait.gameObject.SetActive(false);
		SpriteLevel.gameObject.SetActive(false);

		LabelName.text		= "";			
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(int buildingLV)
	{
		//領地 商店 神樹 工坊 經閣	藥坊 客棧 公會王
		LabelName.text = GameDataDB.GetString(8105+index); 	

		if(buildingLV > 0)
		{
			isOpen = true;

			Utility.ChangeAtlasSprite(SpriteLevel, 17040 - 1 + buildingLV);

			//等級
			if(index == GameDefine.GUILD_BUILDING_BOSS)
			{
				SpriteLevel.gameObject.SetActive(false);
			}
			else
			{
				SpriteLevel.gameObject.SetActive(true);	
			}

			SpriteWait.gameObject.SetActive(false);	//還沒開放圖
		}
		else
		{
			isOpen = false;
			SpriteLevel.gameObject.SetActive(false);
			SpriteWait.gameObject.SetActive(true);
		} 
		
	}
	
	//-------------------------------------------------------------------------------------------------
	public bool CheckOpen()
	{
		return isOpen;
	}
}
