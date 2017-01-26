using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_EquipmentUPStar_EQ : NGUIChildGUI 
{
	public UISprite 		SpriteIcon		= null;
	public UISprite			SpriteIconBG	= null;
	public UISprite			SpriteItemMask	= null;
	//public UILabel 			LabelTitle		= null;
	//public UILabel 			LabelInfo		= null;
	public UILabel 			LabelPlus		= null;
	public S_ItemData 		m_itemData		= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_EquipmentUPStar_EQ";

	//-------------------------------------------------------------------------------------------------
	private Slot_EquipmentUPStar_EQ() : base(GUI_SMARTOBJECT_NAME)
	{		
	}

	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{

	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_Item_Tmp itemTmp)
	{
		if(itemTmp == null)
		{
			UnityDebugger.Debugger.LogError("SetEquipmentSlot sItemTemp == null");
			return;
		}

		//圖示
		SetSpriteIcon(itemTmp.GUID);
		
		LabelPlus.gameObject.SetActive(false);
//		//設定能力
//		ItemBagState state = ARPGApplication.instance.GetGameStateByName(GameDefine.ITEMBAG_STATE) as ItemBagState;
//		if(state == null)
//		{
//			UnityDebugger.Debugger.LogError("ItemBagState state == null ItemGUID = " + data.ItemGUID);
//			return;
//		}

		//設定強化值
//		int plusLV = data.iInherit[GameDefine.GAME_INHERIT_Streng];
//		if(plusLV<0)
//			plusLV = 0;
//		LabelPlus.text = string.Format("+{0}", plusLV);
	}

	//-------------------------------------------------------------------------------------------------
//	public void SetSlot(int itemID)
//	{
//		S_Item_Tmp sItemTemp = GameDataDB.ItemDB.GetData(itemID);
//		
//		if(sItemTemp == null)
//		{
//			UnityDebugger.Debugger.LogError("SetEquipmentSlot sItemTemp == null ItemGUID = " + itemID);
//			return;
//		}
//		
//		SetSpriteIcon(sItemTemp.ItemIcon);
//
//		string tempStr = GetTempletEQBuffInfo(sItemTemp);
//
//		SetLabelInfo(tempStr);
//	}

	//-------------------------------------------------------------------------------------------------
	public void SetSpriteIcon(int guid)
	{
		S_Item_Tmp temp = GameDataDB.ItemDB.GetData(guid);
		if(temp != null)
		{
			Utility.ChangeAtlasSprite(SpriteIcon, temp.ItemIcon);
//			temp.SetRareColor(SpriteItemMask,SpriteIconBG);
			temp.SetItemRarity(SpriteItemMask, SpriteIconBG);
		}
	}
	/*
	//-------------------------------------------------------------------------------------------------
	public void SetLabelTitle(string str)
	{
		LabelTitle.text = str;
	}*/
	/*
	//-------------------------------------------------------------------------------------------------
	public void SetLabelInfo(string str)
	{
		LabelInfo.text = str;
	}*/
	//-------------------------------------------------------------------------------------------------
}
