using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PvP3vs3 : NGUIChildGUI 
{
	public UIPanel		panelBase				= null;

	[Header("My UI")]
	public UILabel		lbMyName				= null;
	public UIButton 	btnRecommend 			= null;
	public UILabel 		lbRecommend 			= null;
	public GameObject[] gMyPartyPosition 		= null;

	[Header("Enemy UI")]
	public UILabel		lbEnemyName				= null;
	public GameObject[] gEnemyPartyPosition 	= null;

	[Header("Other UI")]
	public UIButton 	btnBattle 				= null;
	public UILabel 		lbBattle 				= null;
	public UIButton 	btnExplain				= null;
	public UIButton 	btnClose 				= null;

	//----------------------------------------管理器---------------------------------------------------------
	public List<Slot_PvP3vs3Member>	m_SlotMyMemberList		= new List<Slot_PvP3vs3Member>();
	[HideInInspector]public List<Slot_PvP3vs3Member>	m_SlotEnemyMemberList	= new List<Slot_PvP3vs3Member>();
	//-------------------------------------------------------------------------------------------------
	private const string 	m_SlotMemberName 	= "Slot_PvP3vs3Member";
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PvP3vs3";
	//-------------------------------------------------------------------------------------------------
	private UI_PvP3vs3() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitialLabel();
		m_SlotMyMemberList = CreateMemberSlot(gMyPartyPosition);
		m_SlotEnemyMemberList = CreateMemberSlot(gEnemyPartyPosition);
		//關閉敵方Slot的拖曳功能
		for(int i=0; i<m_SlotEnemyMemberList.Count; ++i)
		{
			m_SlotEnemyMemberList[i].interactable = false;
		}
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	private void InitialLabel()
	{
		lbBattle.text 		= GameDataDB.GetString(1569);	//"開始戰鬥"
		lbRecommend.text 	= GameDataDB.GetString(1598);	//"推薦角色"
	}
	//-------------------------------------------------------------------------------------------------
	private List<Slot_PvP3vs3Member> CreateMemberSlot(GameObject[] partyPos)
	{
		if (partyPos == null)
			return null;
		GameObject go = ResourceManager.Instance.GetGUI(m_SlotMemberName);
		if (go == null)
		{
			UnityDebugger.Debugger.Log("UI_SetBattlePet3vs3 CreateMemberSlot ResourceLoad Error!! "+m_SlotMemberName+" = "+go);
			return null;
		}

		List<Slot_PvP3vs3Member> slotMemberList = new List<Slot_PvP3vs3Member>();
		for(int i=0; i<partyPos.Length; ++i)
		{
			Slot_PvP3vs3Member slotMember = NGUITools.AddChild(partyPos[i],go).GetComponent<Slot_PvP3vs3Member>();
			if (slotMember == null)
			{
				UnityDebugger.Debugger.Log("UI_SetBattlePet3vs3 CreateMemberSlot GetComponent Error!! "+m_SlotMemberName+" = "+slotMember);
				continue;
			}
			slotMember.name = "Slot_PvP3vs3Member_"+i;
			slotMemberList.Add(slotMember);
			slotMember.gameObject.SetActive(false);
		}
		int slotIndex = 0;
		//slot初始化
		foreach(Slot_PvP3vs3Member data in slotMemberList)
		{
			data.Initialize(slotIndex, slotMemberList);
			slotIndex++;
		}
		return slotMemberList; 
	}
	#region 設定UI
	//-----------------------------------------------------------------------------------------------------
	public void SetMyPartyAllMemberUI(object[] partyData)
	{
		if (partyData.Length != m_SlotMyMemberList.Count)
			return;
		for(int i=0; i<partyData.Length;++i)
		{
			if (partyData[i] == null)
			{
				m_SlotMyMemberList[i].SetSlotForPet(null,i,false);
				continue;
			}

			if (partyData[i] is S_PetData)
			{
				S_PetData petData = partyData[i] as S_PetData;
				m_SlotMyMemberList[i].SetSlotForPet(petData,i, false);
			}
			else if (partyData[i] is C_RoleDataEx)
			{
				C_RoleDataEx playerData = partyData[i] as C_RoleDataEx;
				m_SlotMyMemberList[i].SetSlotForPlayer(playerData,i, false);
				lbMyName.text = playerData.m_RoleName;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetEnemyPartyAllMemberUI(object[] partyData)
	{
		if (partyData.Length != m_SlotEnemyMemberList.Count)
			return;

		for(int i=0; i<partyData.Length;++i)
		{
			if (partyData[i] == null)
			{
				m_SlotEnemyMemberList[i].SetSlotForPet(null,i,true);
				continue;
			}

			if (partyData[i] is S_PetData)
			{
				S_PetData petData = partyData[i] as S_PetData;
				m_SlotEnemyMemberList[i].SetSlotForPet(petData,i, true);
			}
			else if (partyData[i] is WebRoleData)
			{
				WebRoleData playerData = partyData[i] as WebRoleData;
				m_SlotEnemyMemberList[i].SetSlotForPlayer(playerData,i, true);
				lbEnemyName.text = playerData.m_strRoleName;
			}
		}
	}
	#endregion
	//-------------------------------------------------------------------------------------------------
	public void ReAssignSlotMyMember()
	{
		if (gMyPartyPosition.Length != m_SlotMyMemberList.Count)
			return;
		for(int i=0; i<gMyPartyPosition.Length;++i)
		{
			Slot_PvP3vs3Member slotMember = gMyPartyPosition[i].GetComponentInChildren<Slot_PvP3vs3Member>();
			if (slotMember == null)
			{
				UnityDebugger.Debugger.Log("UI_PvP3vs3 ReAssignSlotMyMember() Error! Someone Slot_PvP3vs3Member is Empty!");
				return;
			}
			slotMember.Position = i;
			m_SlotMyMemberList[i] = slotMember;
		}
	}
}
