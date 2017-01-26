using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Item : NGUIChildGUI  
{
	public UIWidget			WidgetSlot			= null;
	public UISprite			SpriteBG			= null; //道具底框(品質)
	public UIButton			ButtonSlot			= null;
	public UISprite			SpriteItemMask		= null;	//道具外框(品質)
	public UISprite			SpriteItemIcon		= null; //道具圖
	public UILabel			LabelCount			= null; //道具數量
	public UISprite			SpriteCompare		= null; //道具比較(上下箭頭)
	public UILabel			LabelItemName		= null; //道具名稱(特規使用)
	public UISprite			SpriteNameBG		= null; //道具名稱底板
	public UISprite			spriteBossBoss1		= null;	//魔王
	public UISprite			spriteBossBoss2		= null;	//頭目
	public int 				itemGUID 			= -1;	//道具編號
	public int 				iCount				= 0; 	//數量	

	public int  			Mask_Item			= 1139; //道具寵物怪物共用外框
	public int  			Mask_PetPiece		= 1140; //寵物碎片外框

	public List<int>		specialList			= new List<int>(); //特例排除不在道具格顯示數量清單
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Item";

	//-------------------------------------------------------------------------------------------------
	private Slot_Item() : base(GUI_SMARTOBJECT_NAME)
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
		LabelCount.text = "";
		LabelItemName.text = "";
		LabelItemName.gameObject.SetActive(false);
		SpriteNameBG.gameObject.SetActive(false);
		SpriteCompare.gameObject.SetActive(false);
/*		//白(150、150、150)
		SpriteItemMask.color 	= new Color((float)150/255, (float)150/255, (float)150/255);
		SpriteBG.color			= new Color((float)150/255, (float)150/255, (float)150/255);

		// 上層 預設白
		Utility.ChangeAtlasSprite(SpriteItemMask, 17060);
		// 下層 預設白
		Utility.ChangeAtlasSprite(SpriteBG, 17060);
 */
		// 上層 預設白
		Utility.ChangeAtlasSprite(SpriteItemMask, GameDefine.ITEM_RARITY_FRAME);
		// 下層 預設白
		Utility.ChangeAtlasSprite(SpriteBG, GameDefine.ITEM_RARITY_BACKGROUND);

		itemGUID = -1;
		//道具圖
		Utility.ChangeAtlasSprite(SpriteItemIcon, itemGUID);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetActive(bool bSet)
	{
		WidgetSlot.gameObject.SetActive(bSet);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlotBySirrialID(ulong serialID, bool showName)
	{
		S_ItemData  s_item = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);
		//道具編號
		itemGUID = s_item.ItemGUID;
		//道具數量
		iCount = s_item.iCount;

		SetSlotWithCount(itemGUID, iCount, showName);
	}

	//-------------------------------------------------------------------------------------------------
	// 樣板資料所以要自己填數量
	public void SetSlotWithCount(int guid, int count, bool showName, bool isOwnPet = false)
	{
		S_Item_Tmp item = GameDataDB.ItemDB.GetData(guid);
		if(item == null)
		{
			InitialSlot();
			UnityDebugger.Debugger.Log(string.Format("道具格設定錯誤 道具編號 {0}", guid));
			return;
		}

		//道具編號
		itemGUID = guid;
/*
		item.SetRareColor(SpriteItemMask,SpriteBG,isOwnPet);

		//寵物要換道具外框
		switch(item.ItemType)
		{
		case ENUM_ItemType.ENUM_ItemType_Normal:
		case ENUM_ItemType.ENUM_ItemType_Material:
		case ENUM_ItemType.ENUM_ItemType_Weapen:
		case ENUM_ItemType.ENUM_ItemType_Armor:
		case ENUM_ItemType.ENUM_ItemType_Money:
		case ENUM_ItemType.ENUM_ItemType_Pet:
		case ENUM_ItemType.ENUM_ItemType_EXPItem:
        case ENUM_ItemType.ENUM_ItemType_GiftBox:
			Utility.ChangeAtlasSprite(SpriteItemMask, Mask_Item);	//1139 UI_Backpack Equip_B 道具寵物共用外框
			break;
		case ENUM_ItemType.ENUM_ItemType_PetPiece:
			Utility.ChangeAtlasSprite(SpriteItemMask, Mask_PetPiece);//1140 UI_Backpack Equip_B 寵物碎片外框
			break;
		}
*/
		if(item.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece)
		{
//			//換顏色
//			item.SetRareColor(SpriteItemMask,SpriteBG,isOwnPet);
//			//換外框
//			Utility.ChangeAtlasSprite(SpriteItemMask, Mask_PetPiece);//1140 UI_Backpack Equip_B 寵物碎片外框
			//新版不換色直接換框
			item.SetPetPieceRarity(SpriteItemMask,SpriteBG,isOwnPet);
		}
		else
		{
			//新版不換色直接換框
			item.SetItemRarity(SpriteItemMask,SpriteBG,isOwnPet);
		}

		//道具圖
		Utility.ChangeAtlasSprite(SpriteItemIcon, item.ItemIcon);

		//道具數量
		iCount = count;
//		//如果在排除清單內不管多少數量都不顯示
//		if(specialList.Contains(guid))
//		{
//			LabelCount.text = "";
//		}
//		else
		{
			if(count > 1) //沒超過1不顯示BY阿強
				LabelCount.text = count.ToString();
			else
				LabelCount.text = "";
		}

		//道具比較(上下箭頭)
		SpriteCompare.gameObject.SetActive(false);

		//道具名稱(特規使用)
		LabelItemName.gameObject.SetActive(showName);
		SpriteNameBG.gameObject.SetActive(showName);
		if(showName)
		{
			LabelItemName.text = GameDataDB.GetString(item.iName);
			item.SetRareColorString(LabelItemName);
		}
	}
	//--------------------------------------------------------------------------------
	//顯示怪物Slot
	public void SetSlotWithMonster(int guid, bool showName, bool showTitle)
	{
		S_MobData_Tmp mobDBF = GameDataDB.MobDataDB.GetData(guid);

		if(mobDBF == null)
		{
			InitialSlot();
			UnityDebugger.Debugger.Log(string.Format("怪物格設定錯誤 怪物編號 {0}", guid));
			return;
		}
		
		//道具編號(怪物)
		itemGUID = guid;

		Utility.ChangeAtlasSprite(SpriteItemMask, Mask_Item);	//1139 UI_Backpack Equip_B 道具寵物共用外框
		//道具圖(怪物)
		Utility.ChangeAtlasSprite(SpriteItemIcon, mobDBF.AvatarIcon);
		
		//道具數量(怪物)
		iCount = 1;
//		//如果在排除清單內不管多少數量都不顯示
//		if(specialList.Contains(guid))
//		{
//			LabelCount.text = "";
//		}
//		else
		{
			if(iCount > 1) //沒超過1不顯示BY阿強
				LabelCount.text = iCount.ToString();
			else
				LabelCount.text = "";
		}
		
		//道具比較(上下箭頭)(怪物)
		SpriteCompare.gameObject.SetActive(false);
		
		//道具名稱(特規使用)(怪物)
		LabelItemName.gameObject.SetActive(showName);
		SpriteNameBG.gameObject.SetActive(showName);
		if(showName)
		{
			LabelItemName.text = GameDataDB.GetString(mobDBF.iName);
		}

		spriteBossBoss1.gameObject.SetActive(false);
		spriteBossBoss2.gameObject.SetActive(false);

		//顯示抬頭
		if(showTitle == true)
		{
			switch(mobDBF.MobType)
			{
			case ENUM_MobType.BossBoss1:	spriteBossBoss1.gameObject.SetActive(true);	break;
			case ENUM_MobType.BossBoss2:	spriteBossBoss2.gameObject.SetActive(true);	break;
			}
		}
	}

	//--------------------------------------------------------------------------------
	//顯示寵物Slot
	public void SetSlotWithPetID(int guid, bool showName, bool bUsePet = false)
	{
		S_PetData_Tmp petData = GameDataDB.PetDB.GetData(guid);
		if(petData == null)
		{
			InitialSlot();
			UnityDebugger.Debugger.Log(string.Format("寵物格設定錯誤 寵物編號 {0}", guid));
			return;
		}
		
		//道具編號(寵物)
		itemGUID = guid;
/*
		item.SetRareColor(SpriteItemMask,SpriteBG,bUsePet);

		Utility.ChangeAtlasSprite(SpriteItemMask, Mask_Item);	//1139 UI_Backpack Equip_B 道具寵物共用外框
*/
		//新版不換色直接換框
		petData.SetPetRarity(SpriteItemMask,SpriteBG,bUsePet);

		//道具圖(寵物)
		Utility.ChangeAtlasSprite(SpriteItemIcon, petData.AvatarIcon);

		//道具數量(寵物)
		iCount = 1;
//		//如果在排除清單內不管多少數量都不顯示
//		if(specialList.Contains(guid))
//		{
//			LabelCount.text = "";
//		}
//		else
		{
			if(iCount > 1) //沒超過1不顯示BY阿強
				LabelCount.text = iCount.ToString();
			else
				LabelCount.text = "";
		}
		
		//道具比較(上下箭頭)(寵物)
		SpriteCompare.gameObject.SetActive(false);

		LabelCount.gameObject.SetActive(false);
		//道具名稱(特規使用)(寵物)
		LabelItemName.gameObject.SetActive(showName);
		SpriteNameBG.gameObject.SetActive(showName);
		if(showName)
		{
			LabelItemName.text = GameDataDB.GetString(petData.iName);
			petData.SetRareColorString(LabelItemName,bUsePet);
		}
	}
	//--------------------------------------------------------------------------------
	//顯示玩家Slot, By PVP 3vs3組隊
	public void SetSlotWithPlayer(System.Object data, bool showName)
	{
		if(data == null)
		{
			InitialSlot();
			UnityDebugger.Debugger.Log("Slot_Item SetSlotWithPlayer Error! PlayerData ="+data);
			return;
		}
		itemGUID = -1;
		iCount = 0;

		//無需顯示之UI
		SpriteCompare.gameObject.SetActive(false);
		LabelCount.gameObject.SetActive(false);

		int faceID = -1;
		int frameID = -1;
		string playerName = null;
		if (data is C_RoleDataEx)
		{
			C_RoleDataEx myData = data as C_RoleDataEx;
			faceID = myData.BaseRoleData.iFace;
			frameID = myData.BaseRoleData.iFaceFrameID;
			playerName = myData.m_RoleName;
		}
		else if (data is WebRoleData)
		{
			WebRoleData enemyData = data as WebRoleData;
			faceID = enemyData.m_iFace;
			frameID = enemyData.m_iFaceFrameID;
			playerName = enemyData.m_strRoleName;
		}

		//更換圖示、外框
		Utility.ChangeAtlasSprite(SpriteItemIcon, faceID);
		Utility.ChangeAtlasSprite(SpriteItemMask, frameID);

		if (showName)
			LabelItemName.text = playerName;
		LabelItemName.gameObject.SetActive(showName);
		SpriteNameBG.gameObject.SetActive(showName);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlotItemName(string str)
	{
		//道具名稱(特規使用)
		LabelItemName.gameObject.SetActive(true);
		SpriteNameBG.gameObject.SetActive(true);

		LabelItemName.text = str;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSpriteItemMaskSize(int width, int height)
	{
		SpriteItemMask.width = width;
		SpriteItemMask.height = height;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetDepth(int depth)
	{
		WidgetSlot.depth		+= depth;
		SpriteBG.depth			+= depth; 	//道具底框(品質)
		SpriteItemMask.depth	+= depth;	//道具外框(品質)
		SpriteItemIcon.depth	+= depth;	//道具圖
		LabelCount.depth		+= depth;	//道具數量
		SpriteCompare.depth		+= depth;	//道具比較(上下箭頭)
		LabelItemName.depth		+= depth;	//道具名稱(特規使用)
		SpriteNameBG.depth		+= depth;	//道具名稱底板
	}

}
