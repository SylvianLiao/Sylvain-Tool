using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Wings : MonoBehaviour  
{
	public Slot_Item		slotItemWings		= null;
	public UISprite			spUpgradeTip		= null;

	[NonSerialized]public S_WingUpgrade_Tmp	m_WingsTmp		= null;

	private const string 	m_SlotItemName = "Slot_Item";
	//--------------------------------------------------------------------------------
	public void Initialize()
	{
		m_WingsTmp = null;
		spUpgradeTip.gameObject.SetActive(false);
		CreateSlotItem();
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSlotItem()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotItemName).GetComponent<Slot_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("Slot_Wings CreateSlotItem() error,path:{0}", "GUI/"+m_SlotItemName) );
			return;
		}
		
		Slot_Item newgo = NGUITools.AddChild(this.gameObject,go.gameObject).GetComponent<Slot_Item>();;
		newgo.InitialSlot();
		slotItemWings = newgo;
		slotItemWings.LabelCount.text = "";	
	}
	//--------------------------------------------------------------------------------
	public void SetSlotWings(S_WingUpgrade_Tmp wingtmp)
	{
		if (wingtmp == null)
			return;

		C_RoleDataEx roleDataEX = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		m_WingsTmp = wingtmp;
		slotItemWings.SetSlotWithCount(wingtmp.iItemID,0,false);
		bool isOwn = roleDataEX.CheckIsOwnWings(wingtmp.iItemID);
		bool canUpgrade = false;
		//設定翅膀升級提示
		if (isOwn)
			canUpgrade = roleDataEX.CheckWingsUpgradeMaterial(m_WingsTmp);
		spUpgradeTip.gameObject.SetActive(isOwn && canUpgrade);

		slotItemWings.SpriteItemIcon.color = (isOwn)?Color.white:Color.gray;
		SetItemStatus((roleDataEX.BaseRoleData.iCosBack == wingtmp.iItemID)?GameDataDB.GetString(1007):null);
	}
	//--------------------------------------------------------------------------------
	//設定物品狀態
	public void SetItemStatus(string str)
	{
		//利用SlotItem的數量字串顯示翅膀狀態(EX: 裝備中)
		if (string.IsNullOrEmpty(str))
			slotItemWings.LabelCount.text = "";
		else
			slotItemWings.LabelCount.text = str;
	}
}
