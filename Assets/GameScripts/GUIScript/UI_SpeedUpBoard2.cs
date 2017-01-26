using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_SpeedUpBoard2 : MonoBehaviour 
{
	public UIPanel 				panelBase				= null;

	[Header("Target Item")]
	public UILabel 				lbTitle					= null;
	public GameObject 			gItemPosition			= null;
	public UILabel 				lbItemName 				= null;
	public UILabel 				lbItemType				= null;

	[Header("Content")]
	public UILabel 				lbLevel					= null;
	public UILabel 				lbBeforeLevel			= null;
	public UILabel 				lbAfterLevel			= null;
	public UILabel 				lbBeforeAttribute1		= null;
	public UILabel 				lbAfterAttribute1		= null;
	public UILabel 				lbBeforeAttribute2		= null;
	public UILabel 				lbAfterAttribute2		= null;
	public UILabel 				lbCost					= null;
	public UIGrid 				gdCost					= null;
	public UIButton 			btnCostItem1			= null;
	public UILabel 				lbCostItem1				= null;
	public UIButton 			btnCostItem2			= null;
	public UILabel 				lbCostItem2				= null;

	[Header("Button")]
	public UIButton 			btnConfirm				= null;
	public UILabel 				lbConfirm				= null;
	public UIButton 			btnClose				= null;

	//-------------------------------------------------------------------------------------------------
	private Slot_Item 			m_TargetSlotItem		= null;
	//----------------------------------------裝備強化專用---------------------------------------------------------
	[HideInInspector] public int	m_EqEnhanceValue	= 0;		//裝備強化數值
	[HideInInspector] public int	m_EqEnhanceMoney	= 0;		//裝備強化金錢費用
	[HideInInspector] public int	m_EqEnhanceItem		= 0;		//裝備強化道具費用
	private const string 		m_SlotItemName			= "Slot_Item";
	//-------------------------------------------------------------------------------------------------
	public void InitialUI()
	{
		lbBeforeAttribute1.gameObject.SetActive(false);
		lbBeforeAttribute2.gameObject.SetActive(false);
		CreateSlotItem();
		SwitchUI(false);
	}
	//-------------------------------------------------------------------------------------------------
	public bool InitialLabel(int iTitle, int iLevel, int iCost, int iConfirm)
	{
		if (iTitle<=0 || iLevel<=0 || iCost<=0 || iConfirm<=0)
			return false;
		lbTitle.text 			= GameDataDB.GetString(iTitle);
		lbLevel.text			= GameDataDB.GetString(iLevel);
		lbCost.text				= GameDataDB.GetString(iCost);
		lbConfirm.text			= GameDataDB.GetString(iConfirm);
		return true;
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSlotItem()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotItemName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_SpeedUpBoard2 CreateSlotItem() error,path:{0}", "GUI/"+m_SlotItemName) );
			return;
		}
		
		Slot_Item newgo = NGUITools.AddChild(gItemPosition,go.gameObject).GetComponent<Slot_Item>();;
		newgo.InitialSlot();
		newgo.gameObject.SetActive(true);
		m_TargetSlotItem = newgo;
	}
	//-------------------------------------------------------------------------------------------------------------
	public void SwitchUI(bool bSwitch)
	{
		this.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------------------
	private void ResetValue()
	{
		m_EqEnhanceValue	= 0;
     	m_EqEnhanceMoney	= 0;
     	m_EqEnhanceItem		= 0;
	}
	#region 按鈕事件相關
	//-----------------------------------------------------------------------------------------------------
	public void AddCallBack()
	{
		UIEventListener.Get(btnClose.gameObject).onClick						+= OnBtnCloseClick;
		UIEventListener.Get(btnCostItem1.gameObject).onClick					+= OnBtnCostItemClick;
		UIEventListener.Get(btnCostItem2.gameObject).onClick					+= OnBtnCostItemClick;
	}
	//-----------------------------------------------------------------------------------------------------
	public void RemoveCallBack()
	{
		UIEventListener.Get(btnClose.gameObject).onClick						-= OnBtnCloseClick;
		UIEventListener.Get(btnCostItem1.gameObject).onClick					-= OnBtnCostItemClick;
		UIEventListener.Get(btnCostItem2.gameObject).onClick					-= OnBtnCostItemClick;
	}
	//-------------------------------------------------------------------------------------------------------------
	public void OnBtnCloseClick(GameObject go)
	{
		SwitchUI(false);
		ResetValue();
	}
	//-------------------------------------------------------------------------------------------------------------
	public void OnBtnCostItemClick(GameObject go)
	{
		UIButton btn = go.GetComponent<UIButton>();
		if (btn == null)
			return;
		if (btn.userData == null)
			return;
		int costItemID = (int)btn.userData;
		if (costItemID <= 0)
			return;
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(costItemID,0,true);
	}
	#endregion
	//-------------------------------------------------------------------------------------------------------------
	//設定裝備快速強化內容
	public bool SetEnhanceUI(UI_ItemBag uiItemBag)
	{
		S_ItemData itemData = uiItemBag.GetShowingItemData();
		if (itemData == null)
			return false;
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(itemData.ItemGUID);
		if (itemTmp == null)
			return false;
		string strBeforeAttr = itemData.GetEquipMainAttrString(itemTmp);
		if (string.IsNullOrEmpty(strBeforeAttr))
			return false;

		C_RoleDataEx roleDataEX = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(roleDataEX.GetVIPRank()+1);
		if (vipTmp == null)
			return false;

		lbBeforeAttribute2.gameObject.SetActive(false);
		
		//設定物品資訊
		lbItemName.text = GameDataDB.GetString(itemTmp.iName);
		lbItemType.text = GameDataDB.GetString(itemTmp.iItemNote);
		m_TargetSlotItem.SetSlotWithCount(itemData.ItemGUID,itemData.iCount,false);
		m_TargetSlotItem.SetDepth(lbItemName.depth);

		//計算根據玩家資源可升至最高的強化等級
		int plusLV = itemData.GetEqEnhanceValue();
		int canEnhanceLv = (roleDataEX.GetLevel()*2+vipTmp.AddEqStrenghMax) - plusLV;
		int enhanceCostItemID = itemData.GetEqEnhanceItemID(canEnhanceLv+plusLV);

		int costMoney = itemData.GetEqEnhanceMoney(canEnhanceLv+plusLV)-itemData.GetEqEnhanceMoney(plusLV);
		int playerMoney = roleDataEX.GetBodyMoney();
		
		int costItem = itemData.GetEqEnhanceItemCount(canEnhanceLv+plusLV)-itemData.GetEqEnhanceItemCount(plusLV);
		int playerCostItem = roleDataEX.GetSpecifiedItemCountInBag(enhanceCostItemID);
		
		m_EqEnhanceValue = itemData.GetEqEnhanceAttrValue(canEnhanceLv);

		if (costMoney > playerMoney || costItem > playerCostItem)
		{
			ENUM_EqStrengthen emESType = itemData.GetEqStrengthenType();
			costMoney = 0;
			costItem = 0;
			m_EqEnhanceValue = 0;
			int i;
			for(i = plusLV+1; i <= canEnhanceLv+plusLV; ++i)
			{
				S_EqStrengthen_Tmp data = GameDataDB.EqStrengthenDB.GetData(i);
				
				if(data!= null)
				{
					if (data.UpStrengthening_Price[(int)emESType] > 0)
						costMoney += data.UpStrengthening_Price[(int)emESType];
					if (data.iUpStrItemCount > 0)
						costItem += data.iUpStrItemCount;
					
					m_EqEnhanceValue += data.UpStrengthening_Value[(int)emESType];
					if (costMoney > playerMoney || costItem > playerCostItem)
					{
						//因所需費用超過玩家擁有金錢，故將資料退回至上一步
						costMoney 	-= data.UpStrengthening_Price[(int)emESType];
						costItem 	-= data.iUpStrItemCount;
						m_EqEnhanceValue -= data.UpStrengthening_Value[(int)emESType];
						i--;
						break;
					}
				}
			}
			canEnhanceLv = i-plusLV;
		}
		
		//設定強化等級
		lbBeforeLevel.text = /*GameDataDB.GetString(5128)+*/plusLV.ToString();
		lbAfterLevel.text = /*GameDataDB.GetString(5128)+*/(canEnhanceLv+plusLV).ToString();
		//設定強化數值
		lbBeforeAttribute1.text = strBeforeAttr;
		lbAfterAttribute1.text = (itemData.GetEquipMainAttrValue()+m_EqEnhanceValue).ToString();
		lbBeforeAttribute1.gameObject.SetActive(true);
		
		//設定強化費用
		Utility.SetButtonPic(btnCostItem2,10002,10002,10002,10002);		//金幣圖示
		lbCostItem2.text = costMoney.ToString();
		m_EqEnhanceMoney = costMoney;
		if (costItem > 0)
		{
			string strPlayerCostItem = (playerCostItem < costItem ? GameDataDB.GetString(1327)+playerCostItem.ToString()+GameDataDB.GetString(1329) : playerCostItem.ToString());
			lbCostItem1.text = " "+strPlayerCostItem+"/"+costItem;
			m_EqEnhanceItem = costItem;
			S_Item_Tmp costItemTmp = GameDataDB.ItemDB.GetData(enhanceCostItemID);
			Utility.SetButtonPic(btnCostItem1,costItemTmp.ItemIcon,costItemTmp.ItemIcon,costItemTmp.ItemIcon,costItemTmp.ItemIcon);	//強化石

			btnCostItem1.userData = enhanceCostItemID;
			btnCostItem1.gameObject.SetActive(true);
		}
		else
			btnCostItem1.gameObject.SetActive(false);
		
		gdCost.enabled = true;
		gdCost.Reposition();
		
		//設定強化成功提示字串位置
		uiItemBag.RecordStrengthUpLoc();
		
		return true;
	}
	//-------------------------------------------------------------------------------------------------------------
	//設定翅膀升級資料
	public bool SetWingsUpgradeData(S_WingUpgrade_Tmp nowWingsTmp)
	{
		if (nowWingsTmp == null)
			return false;

		C_RoleDataEx roleDataEX = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		S_WingUpgrade_Tmp nextWingsTmp = roleDataEX.GetWingsTmpInDBF(nowWingsTmp.iItemID, nowWingsTmp.iLevel+1);
		if (nextWingsTmp == null)
			return false;

		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(nowWingsTmp.iItemID);
		if (itemTmp == null)
			return false;

		//設定物品資訊
		lbItemName.text = GameDataDB.GetString(itemTmp.iName);
		lbItemType.text = GameDataDB.GetString(itemTmp.iItemNote);
		m_TargetSlotItem.SetSlotWithCount(itemTmp.GUID,0,false);
		m_TargetSlotItem.SetDepth(lbItemName.depth);

		lbBeforeLevel.text = GameDataDB.GetString(8325)+nowWingsTmp.iLevel.ToString()+GameDataDB.GetString(1329);
		lbAfterLevel.text = GameDataDB.GetString(8325)+nextWingsTmp.iLevel.ToString()+GameDataDB.GetString(1329);

		lbBeforeAttribute1.gameObject.SetActive(false);
		lbBeforeAttribute2.gameObject.SetActive(false);
		//設定翅膀等級、屬性
		for(int i=0; i<nextWingsTmp.WingsAttributeList.Count; ++i)
		{
			if (nowWingsTmp.WingsAttributeList[i].fAttritubeValue < 0.0001f && 
			    nextWingsTmp.WingsAttributeList[i].fAttritubeValue < 0.0001f)
				continue;
			string str = nowWingsTmp.GetWingsOneAttributeString(i,true);
			str = GameDataDB.GetString(8325)+str+GameDataDB.GetString(1329);
			string strNext = nextWingsTmp.GetWingsOneAttributeString(i,true);
			strNext = GameDataDB.GetString(8325)+strNext+GameDataDB.GetString(1329);
			if (i==0)
			{
				lbBeforeAttribute1.text = str;
				lbAfterAttribute1.text = strNext;
				lbBeforeAttribute1.gameObject.SetActive(true);
			}
			else if (i==1)
			{
				lbBeforeAttribute2.text = str;
				lbAfterAttribute2.text = strNext;
				lbBeforeAttribute2.gameObject.SetActive(true);
			}
		}

		//設定消耗道具
		if (nowWingsTmp.iUpgradeItemType_1 == ENUM_Use_ItemType.ENUM_Use_ItemType_None && 
		    nowWingsTmp.iUpgradeItemType_2 == ENUM_Use_ItemType.ENUM_Use_ItemType_None)
			return false;
		SetWingsUpgradeCost(nowWingsTmp,1);
		SetWingsUpgradeCost(nowWingsTmp,2);

		gdCost.enabled = true;
		gdCost.Reposition();

		return true;
	}
	//-------------------------------------------------------------------------------------------------------------
	//設定翅膀消耗道具
	private void SetWingsUpgradeCost(S_WingUpgrade_Tmp nowWingsTmp, int costItemNumber)
	{
		int costCount = 0;
		UIButton btnCostItem = null;
		UILabel	lbCostItem = null;
		ENUM_Use_ItemType useType = ENUM_Use_ItemType.ENUM_Use_ItemType_None;
		switch(costItemNumber)
		{
		case 1:
			btnCostItem = btnCostItem1;
			lbCostItem = lbCostItem1;
			costCount = nowWingsTmp.iUpgradeCount_1;
			useType = nowWingsTmp.iUpgradeItemType_1;
			break;
		case 2:
			btnCostItem = btnCostItem2;
			lbCostItem = lbCostItem2;
			costCount = nowWingsTmp.iUpgradeCount_2;
			useType = nowWingsTmp.iUpgradeItemType_2;
			break;
		}
		if (useType == ENUM_Use_ItemType.ENUM_Use_ItemType_None)
		{
			btnCostItem.gameObject.SetActive(false);
			return;
		}

		C_RoleDataEx roleDataEX = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		S_Item_Tmp costItemTmp = null;
		int itemIcon = 0;
		switch(useType)
		{
		case ENUM_Use_ItemType.ENUM_Use_ItemType_None:
			break;
		case ENUM_Use_ItemType.ENUM_Use_ItemType_Money:
			int playerCount = roleDataEX.GetBodyMoney();
			itemIcon = 10002;
			int colorID = (playerCount < costCount)?8324:8325;
			string strCost = GameDataDB.GetString(colorID)+costCount.ToString()+GameDataDB.GetString(1329);
			lbCostItem.text = strCost;
			break;
		case ENUM_Use_ItemType.ENUM_Use_ItemType_Diamond:
			playerCount = roleDataEX.GetItemMallMoney();
			itemIcon = 10001;
			colorID = (playerCount < costCount)?8324:8325;
			strCost = GameDataDB.GetString(colorID)+costCount.ToString()+GameDataDB.GetString(1329);
			lbCostItem.text = strCost;
			break;
		case ENUM_Use_ItemType.ENUM_Use_ItemType_Item:
			if (costItemNumber == 1)
				costItemTmp = GameDataDB.ItemDB.GetData(nowWingsTmp.iUpgradeItem_1);
			else if (costItemNumber == 2)
				costItemTmp = GameDataDB.ItemDB.GetData(nowWingsTmp.iUpgradeItem_2);
			playerCount = roleDataEX.GetSpecifiedItemCountInBag(costItemTmp.GUID);
			itemIcon = costItemTmp.ItemIcon;
			colorID = (playerCount < costCount)?8324:8325;
			strCost = GameDataDB.GetString(colorID)+playerCount.ToString()+GameDataDB.GetString(1329);
			lbCostItem.text = " "+strCost+"/"+costCount;
			break;
		}

		Utility.SetButtonPic(btnCostItem,itemIcon,itemIcon,itemIcon,itemIcon);
		btnCostItem.userData = (useType == ENUM_Use_ItemType.ENUM_Use_ItemType_Item)?costItemTmp.GUID:-1;
		btnCostItem.gameObject.SetActive(true);
	}
}
