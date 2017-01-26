using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildBuildingSetting : NGUIChildGUI  
{

	public	UIWidget		slotBuilding			= null;
	public	UISprite		SpriteGuildBuildingIcon	= null;	//建築ICON
	public	UISprite		SpriteLevelUP			= null;	//升級圖
	public	UILabel			LabelGuildBuildingName	= null;	//建築名稱
	public	UILabel			LabelGuildBuildingLv	= null;	//建築等級
	public	UILabel			LabelGuildBuildingCost	= null;	//建築花費
	public	UILabel			LabelGuildBuildingInfo	= null;	//建築說明
	public	UIButton		ButtonGuildBuilding		= null;	//建築按鈕
	public	UILabel			LabelGuildBuilding		= null;	//建築按鈕Label
	public	UISprite		SpriteGuildBuilding		= null;	//建築按鈕圖

	public	int				index	= 0;		//讀字串之類用的
	private	bool 			isOpen	= false;	//有沒有開放

	public	Color			disableColor	= new Color();
	private	Color			originColor		= new Color();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildBuildingSetting";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildBuildingSetting() : base(GUI_SMARTOBJECT_NAME)
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
		SpriteLevelUP.gameObject.SetActive(false);	//升級圖

  		LabelGuildBuildingName.text		= "";			
		LabelGuildBuildingLv.text		= GameDataDB.GetString(8056);	//等級：
		LabelGuildBuildingCost.text		= "0";	
		LabelGuildBuildingInfo.text		= "";	
		LabelGuildBuilding.text			= "";	
		originColor = SpriteGuildBuilding.color;
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(GuildBaseData data, int lv, bool showBtn)
	{
		if(lv > 0)
		{
			isOpen = true;
		}
		else
		{
			isOpen = false;
			return;
		}

		//建築名稱 -領地 商店 神樹 工坊 經閣 藥坊 客棧 公會王
		LabelGuildBuildingName.text	= GameDataDB.GetString(8105+index);	
		
		//建築等級
		LabelGuildBuildingLv.text	= string.Format("{0}{1}"
		                                          , GameDataDB.GetString(8056)		//等級：
		                                          , lv);
		//建築說明
		LabelGuildBuildingInfo.text	= GameDataDB.GetString(8142+index);	

		//建築花費
		int cost = 0;

		S_GuildLevel_Tmp dbf = GameDataDB.GuildLevelDB.GetData(lv+1);
		if(dbf == null && lv < GameDefine.GUILD_BUILDING_LEVEL_MAX)
		{
			return;
		}

		if(lv >= GameDefine.GUILD_BUILDING_LEVEL_MAX)
		{
			LabelGuildBuildingCost.text	= "";
		}
		else
		{
			switch(index)
			{
			case GameDefine.GUILD_BUILDING_TERRITORY:	// 公會建築編號-領地
				cost = dbf.GuildEXP;
				break;
			case GameDefine.GUILD_BUILDING_STORE:		// 公會建築編號-商店
				cost = dbf.GuildStoreEXP;
				break;
			case GameDefine.GUILD_BUILDING_TREE:		// 公會建築編號-神樹
				cost = dbf.TreeEXP;
				break;
			case GameDefine.GUILD_BUILDING_WORKSHOP:	// 公會建築編號-工坊
				cost = dbf.WorkEXP;
				break;
			case GameDefine.GUILD_BUILDING_LIBRARY:		// 公會建築編號-經閣
				cost = dbf.PavilionEXP;
				break;
			case GameDefine.GUILD_BUILDING_DRUGSTORE:	// 公會建築編號-藥坊
				cost = dbf.MedicineEXP;
				break;
			case GameDefine.GUILD_BUILDING_HOTEL:		// 公會建築編號-客棧
				cost = dbf.WorkEXP;
				break;
			case GameDefine.GUILD_BUILDING_BOSS:		// 公會建築編號-公會王
				break;
			}
			LabelGuildBuildingCost.text	= cost.ToString();
		}

		if(showBtn)
		{
			ButtonGuildBuilding.gameObject.SetActive(true);
			//建築按鈕
			if(lv >= GameDefine.GUILD_BUILDING_LEVEL_MAX)
			{
				ButtonGuildBuilding.isEnabled = false;
				SpriteGuildBuilding.color = disableColor;
				LabelGuildBuilding.text = GameDataDB.GetString(8057);	//尚未開放
				SpriteLevelUP.gameObject.SetActive(false);	//升級圖
			}
			else if(cost > data.GuildExp || data.GuildExp == 0)
			{
				ButtonGuildBuilding.isEnabled = false;
				SpriteGuildBuilding.color = disableColor;
				LabelGuildBuilding.text = GameDataDB.GetString(8054);	//資金不足
				SpriteLevelUP.gameObject.SetActive(false);	//升級圖
			}
			else
			{
				ButtonGuildBuilding.isEnabled = true;
				SpriteGuildBuilding.color = originColor;
				LabelGuildBuilding.text = GameDataDB.GetString(8055);	//升級
				SpriteLevelUP.gameObject.SetActive(true);	//升級圖
			}
		}
		else
		{
			ButtonGuildBuilding.gameObject.SetActive(false);
		}
	}
	
	//-------------------------------------------------------------------------------------------------
}
