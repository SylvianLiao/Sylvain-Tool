using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_EquipmentUPStar_Item : NGUIChildGUI
{

	public UISprite  			SpriteItemIcon		= null;
	public UISprite				SpriteItemMask		= null;
	public UISprite				SpriteItemBG		= null;
	public UILabel 				LabelNum			= null;
	public UILabel				LabelExpPoint		= null;
    [System.NonSerialized]
	public S_FusionMaterial 	fusionData			= new S_FusionMaterial();
	public int					itemID				= 0;
	public int 					itemCount			= 0;	//此Slot所存物品之實際數量
	public int					nowCount			= 0;	//目前現有數量 (非實際該物品數量，而是經過玩家操作而改變之數量)
	public int					iSerialID			= 0;
	public int					eachItemExp			= 0;
	private int					iUpNeedCount		= 0;
	//暫存已選擇的數量，keys = 該物品在材料List中的index , values = 已選擇數量
	public Dictionary<int,int> sItem = new Dictionary<int, int>();
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_EquipmentUPStar_Item";
	
	//-------------------------------------------------------------------------------------------------
	public Slot_EquipmentUPStar_Item() : base(GUI_SMARTOBJECT_NAME)
	{		
	}

	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
	
	}
	//-------------------------------------------------------------------------------------------------
	public void ClearSlot()
	{
		SpriteItemIcon.atlas = null;
		SpriteItemMask.color = Color.white;
		SpriteItemBG.color = Color.white;
		LabelNum.text = "";
		itemID				= 0;
		itemCount			= 0;
		nowCount			= 0;
		iSerialID 				= 0;
		eachItemExp			= 0;
		iUpNeedCount 		= 0;
		sItem.Clear();
		this.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	//設定合成升階材料用
	public void SetSlotByFusion(S_FusionMaterial data, bool showNumber)
	{
		fusionData = data;
		itemID	= data.iItemID;
		iUpNeedCount = data.iCount;

		SetSpriteIcon(data.iItemID);
		if (showNumber)
			SetLabelTitle(data.iItemID, iUpNeedCount);

		LabelNum.gameObject.SetActive(showNumber);
		LabelExpPoint.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//設定合成升階材料用
	public void SetSlotByFusion(S_ItemData itemData,S_FusionMaterial matData,bool showNumber)
	{
		iUpNeedCount = matData.iCount;

		//沒有物品資料就將slot上的記錄數量設為預設值, 但保留物品ID和SerialID給UI顯示用
		if (itemData == null)
		{
			if (showNumber)
				SetLabelTitle(0, iUpNeedCount);
			else
			{
				itemCount = 0;
				nowCount = 0;
			}
		}
		else
		{
			itemID	= itemData.ItemGUID;

			SetSpriteIcon(itemData.ItemGUID);
			if (showNumber)
				SetLabelTitle(itemData.ItemGUID, iUpNeedCount);
			else
			{
				itemCount = itemData.iCount;
				nowCount = itemData.iCount;
				iSerialID = (int)itemData.iSerial;
			}
		}
		
		LabelNum.gameObject.SetActive(showNumber);
		LabelExpPoint.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//將該Slot設定成經驗值道具資料，而不是顯示UI用的Slot
	public void SetSlotByExpItem(S_ItemData data)
	{
		itemID	= data.ItemGUID;
		itemCount = data.iCount;
		nowCount = itemCount;
		iSerialID = (int)data.iSerial;
		eachItemExp = GameDataDB.ItemDB.GetData(itemID).iAddExp;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlotByExpItem(Slot_EquipmentUPStar_Item matSlot)
	{
		itemID	= matSlot.itemID;
		itemCount = matSlot.itemCount;
		nowCount = matSlot.nowCount;
		iSerialID = matSlot.iSerialID;
		eachItemExp = matSlot.eachItemExp;
	}
	//-------------------------------------------------------------------------------------------------
	//將經驗值道具資料設定至WrapContent的實體物件上時使用
//	public void SetSlot(int itemGUID,int count,int sid , bool isOnlyData = false)
//	{
//		itemID	= itemGUID;
//		//itemCount = count;
//		nowCount = count;
//		selid = sid;
//		eachItemExp = GameDataDB.ItemDB.GetData(itemID).iAddExp;
//		if (!isOnlyData)
//		{
//			SetSpriteIcon(itemID);
//			//第一次設定物件現有數量 = 最大數量
//			SetNotRepeatLabel(nowCount,itemCount);
//			//		SetLabelTitle(itemID, itemCount);
//		}
//	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(int itemGUID,int count,int sid,bool showTotalCount)
	{
		itemID	= itemGUID;
//		S_ItemData itemData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(itemGUID);
//		if (itemData != null)
//			itemCount = itemData.iCount;
		itemCount = count;
		nowCount = count;
		iSerialID = sid;
		eachItemExp = GameDataDB.ItemDB.GetData(itemID).iAddExp;
		
		SetSpriteIcon(itemID);
		//第一次設定物件現有數量 = 最大數量
		if (showTotalCount)
			SetSelectCount(nowCount,itemCount);
		else
			SetSelectCount(nowCount);

		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	public void RefreshSelectMatSlot(int addCount)
	{
		nowCount += addCount;
		SetSpriteIcon(itemID);
		SetSelectCount(nowCount);
	}
	//-------------------------------------------------------------------------------------------------
	public Dictionary<int,int> GetsItemDic()
	{
		return sItem;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetDicItem(int index,int count)
	{
		if(sItem.ContainsKey(index))
		{
			sItem[index] += count;
		}
		else
		{
			sItem[index] = count;
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSpriteIcon(int guid)
	{
		S_Item_Tmp temp = GameDataDB.ItemDB.GetData(guid);
		if(temp == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("S_Item_Tmp == null {0}", guid));
			return;
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteItemIcon, temp.ItemIcon);
//			temp.SetRareColor(SpriteItemMask,SpriteItemBG);
			temp.SetItemRarity(SpriteItemMask,SpriteItemBG);

		}

		if(LabelExpPoint != null)
		{
			//順便設定提供的經驗值
			string str = string.Format("{0}{1}{2}{3}",GameDataDB.GetString(1326),temp.iAddExp.ToString(),GameDataDB.GetString(1329),GameDataDB.GetString(207));//exp,綠色,點,[-]
			
			LabelExpPoint.text = str;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void SetLabelTitle(int guid, int count)
	{
		nowCount = 0;
		int maxCount = 0;

		if (guid > 0)
		{
			foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
			{
				if(tempItem.ItemGUID == guid)
				{
					nowCount += tempItem.iCount;
					if (nowCount > maxCount)
					{
						maxCount = nowCount;
						//只記錄堆疊數量最多的iSerialID
						iSerialID = (int)tempItem.iSerial;
					}
				}
			}
		}

		string strNowCount = (count>nowCount ? "[FF0000]"+nowCount.ToString()+"[-]" : nowCount.ToString());
				
		string str = string.Format("{0:D}/{1:D}", strNowCount, count);

		LabelNum.text = str;

		if(nowCount >= iUpNeedCount)
		{
			SpriteItemIcon.color = Color.white;
		}
		else
		{
			SpriteItemIcon.color = Color.gray;
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSelectCount(int nowHave, int count)
	{
		string str;
		if(nowHave <= 0)
		{
			SpriteItemIcon.color = Color.gray;

			str = string.Format("{0:D}/{1:D}", nowHave, count);
		}
		else
		{
			SpriteItemIcon.color = Color.white;

			str = string.Format("{0:D}/{1:D}", nowHave, count);
		}
		
		LabelNum.text = str;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSelectCount(int nowHave)
	{
		string str;
		if(nowHave <= 0)
		{
			SpriteItemIcon.color = Color.gray;
			
			str = string.Format("{0:D}", nowHave);
		}
		else
		{
			SpriteItemIcon.color = Color.white;
			
			str = string.Format("{0:D}", nowHave);
		}
		
		LabelNum.text = str;
	}
	//-------------------------------------------------------------------------------------------------
//	public void SetSelectCount(int count)
//	{
//		string str = string.Format("{0:D}/{1:D}",count);
//		LabelNum.text = str;
//	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//給戰陣升級材料定用
	public void SetSlotForFormation(S_FormationCost_Tmp data)
	{
		itemID	= data.iFormationCostItemID;
		iUpNeedCount = data.iFormationCostItemCount;
		SetSpriteIcon(data.iFormationCostItemID);
		SetLabelTitle(data.iFormationCostItemID, data.iFormationCostItemCount);
		this.gameObject.SetActive(true);
	}
}
