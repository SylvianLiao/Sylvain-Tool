using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildBossBattlePet : MonoBehaviour 
{
	public Slot_Item 		m_SlotItem 			= null;
	public UILabel 			lbScoreBonus 		= null;
	public UILabel 			lbPetLV				= null;
	public UILabel 			lbPartyNumber		= null;
	public UILabel 			lbRolePower			= null;
	public UISprite 		spGet 				= null;
	public GameObject		gInspireEffect		= null;
	
	private int 			m_PetID				= -1;
	public int PetID	{get {return m_PetID;} set {m_PetID = value;}}
	[HideInInspector]public int SlotIndex 			= -1;	//該Slot於管理器中是第幾個
	//-------------------------------------------------------------------------------------------------
	public void InitialUI()
	{
		m_SlotItem.LabelCount.gameObject.SetActive(false);
		//尚未取得寵物戰力資訊
		lbRolePower.gameObject.SetActive(false);
		lbScoreBonus.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(int petID)
	{
		S_PetData petData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(petID);
		if (petData == null)
		{
			SwitchPetDataUI(false);
			return;
		}
			
		SetSlot(petData);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_PetData petData)
	{
		if (petData == null)
		{
			SwitchPetDataUI(false);
			return;
		}
		S_PetData_Tmp petTmp = GameDataDB.PetDB.GetData(petData.iPetDBFID);
		if (petTmp == null)
		{
			SwitchPetDataUI(false);
			return;
		}
			
		m_PetID = petData.iPetDBFID;
		lbPetLV.text = petData.iPetLevel.ToString();
		//lbScoreBonus.text = petTmp.iGuildWarPoints.ToString();
		m_SlotItem.SetSlotWithPetID(petData.iPetDBFID,false);
		SwitchPetDataUI(true);
	}
	//-------------------------------------------------------------------------------------------------
	//有無寵物資料時UI的顯示會不同
	private void SwitchPetDataUI(bool bSwitch)
	{
		if (!bSwitch)
		{
			m_PetID = 0;
			m_SlotItem.SetSlotWithPetID(m_PetID,false);
		}
			
		//lbScoreBonus.gameObject.SetActive(bSwitch);
		lbPetLV.gameObject.SetActive(bSwitch);
		spGet.gameObject.SetActive(!bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void PlayInspireEffect()	
	{
		gInspireEffect.SetActive(false);
		gInspireEffect.SetActive(true);
	}
}
