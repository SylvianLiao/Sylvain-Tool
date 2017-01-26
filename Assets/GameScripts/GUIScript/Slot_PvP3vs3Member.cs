using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_PvP3vs3Member : UIDragDropItem
{
	public UIButton 		btnSlotBase 		= null;
	public Slot_Item 		SlotItem 			= null;
	public UIButton 		btnSlotMember		= null;
	public UILabel 			lbLevel				= null;
	public UISprite 		spPlus 				= null;
	public UISprite 		spUnknownEnemy		= null;
	public UILabel 			lbRolePower			= null;
	public UISprite 		spProperty			= null;

	//-------------------------------------------------------------------------------------------------
	[HideInInspector]public int 		PetID				= -1;
	[HideInInspector]public int 		RolePower 			= -1;	//該角色戰力
	[HideInInspector]public int 		Position 			= -1;	//該Slot是第幾個位置
	private object				 		RoleData			= null;	//暫存角色資料
	//-------------------------------------------------------------------------------------------------
	public  Enum_3vs3SlotMemberStatus	m_NowMemberStatus	= Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_Max;
	private List<Slot_PvP3vs3Member> 	m_SlotMemberList	= null;	//所有同組的隊員Slot

	//-------------------------------------------------------------------------------------------------
	private const string 	m_LabelPowerName	= "Label(RolePower)";
	//-------------------------------------------------------------------------------------------------
	public void Initialize(int slotIndex, List<Slot_PvP3vs3Member> slotMemberList)
	{
		Position = slotIndex;
		m_SlotMemberList = slotMemberList;
		spPlus.gameObject.SetActive(false);
		spUnknownEnemy.gameObject.SetActive(false);
		ResetValue();

		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	private void SwitchMemberStatus(Enum_3vs3SlotMemberStatus status)
	{
		if (status == m_NowMemberStatus)
			return;

		m_NowMemberStatus = status;

		switch(m_NowMemberStatus)
		{
		case Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_None:
			SlotItem.InitialSlot();
			ResetValue();
			break;
		case Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_Player:
		case Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_Pet:
			break;
		}
		Utility.ChangeAtlasSprite(SlotItem.SpriteBG, GameDefine.PVP_3VS3_SLOT_BG_ID);	//強制將Slot_Item背景圖換成指定的背景
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlotForPlayer(object playerData, int position, bool isEnemy)
	{
		if (playerData == null || position < 0 || position >= GameDefine.PVP_3VS3_PARTY_NUMBER_MAX*GameDefine.PVP_ONE_3VS3PARTY_MEMBER_MAX)
			return;

		RoleData = playerData;
		Position = position;
		int level = -1;
		if (playerData is C_RoleDataEx)
		{
			C_RoleDataEx myData = playerData as C_RoleDataEx;
			level = myData.GetLevel();
			SetRolePower(myData.GetSingleRolePower());
		}
		else if (playerData is WebRoleData)
		{
			WebRoleData enemyData = playerData as WebRoleData;
			level = enemyData.m_iLevel;
			SetRolePower(enemyData.m_iPower);
		}

		lbLevel.text = level.ToString();
		lbLevel.gameObject.SetActive(level>0);
		SlotItem.SetSlotWithPlayer(playerData,false);
		spProperty.gameObject.SetActive(false);
		SlotItem.SetSpriteItemMaskSize(120,120);
		interactable = !isEnemy;
		SwitchMemberStatus(Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_Player);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlotForPet(S_PetData petData, int position, bool isEnemy)
	{
		if (petData == null)
		{
			SwitchMemberStatus(Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_None);
			spPlus.gameObject.SetActive(!isEnemy);
			spUnknownEnemy.gameObject.SetActive(isEnemy);
			return;
		}
		if (position < 0 || position >= GameDefine.PVP_3VS3_PARTY_NUMBER_MAX*GameDefine.PVP_ONE_3VS3PARTY_MEMBER_MAX)
			return;
		S_PetData_Tmp petTmp = GameDataDB.PetDB.GetData(petData.iPetDBFID);
		if (petTmp == null)
			return;

		RoleData = petData;
		PetID = petData.iPetDBFID;
		Position = position;

		SetRolePower(petData.iPetPower);
		lbLevel.text = petData.iPetLevel.ToString();
		lbLevel.gameObject.SetActive(petData.iPetLevel>0);
		SlotItem.SetSlotWithPetID(PetID,false,false);
		SlotItem.SetSpriteItemMaskSize(110,100);
		Utility.ChangeAtlasSprite(spProperty, ARPGApplication.instance.GetPetCalssIconID(petTmp.emCharClass)); 
		spProperty.gameObject.SetActive(true);

		interactable = !isEnemy;
		SwitchMemberStatus(Enum_3vs3SlotMemberStatus.Enum_3vs3SlotMemberStatus_Pet);
	}
	//-------------------------------------------------------------------------------------------------
	private void SetRolePower(int power)
	{
		Transform tLabel = this.transform.parent.FindChild(m_LabelPowerName);
		if (tLabel == null)
			return;
		lbRolePower = tLabel.GetComponent<UILabel>();
		if (lbRolePower == null)
			return;

		RolePower = power;
		lbRolePower.text = RolePower.ToString();
		lbRolePower.gameObject.SetActive(RolePower>0);
	}
	//-------------------------------------------------------------------------------------------------
	private void ResetValue()
	{
		SetRolePower(0);
		PetID = -1;
		RoleData = null;
		spProperty.gameObject.SetActive(false);
		lbLevel.gameObject.SetActive(false);
		interactable = false;
	}
	//-------------------------------------------------------------------------------------------------
	protected override void OnDragDropStart ()
	{
		base.OnDragDropStart();
		//播放音效
		MusicControlSystem.PlaySound("Sound_System_003",1);
	}
	//-------------------------------------------------------------------------------------------------
	protected override void OnDragDropRelease (GameObject surface)
	{
		if (surface == null)
			return;
		PvP3vs3State pvp3vs3State = ARPGApplication.instance.GetGameStateByName(GameDefine.PvP3vs3S_STATE) as PvP3vs3State;
		if (pvp3vs3State == null)
			return;
		Transform tempParent = mParent;
		base.OnDragDropRelease(surface);
		//若放下位置為其他Slot則互換
		for(int i=0; i<m_SlotMemberList.Count; ++i)
		{
			if (surface == m_SlotMemberList[i].gameObject)
			{
				//播放音效
				MusicControlSystem.PlaySound("Sound_System_002",1);
				//換Parent
				m_SlotMemberList[i].transform.parent = tempParent;
				m_SlotMemberList[i].transform.localPosition = Vector3.zero;
				//換戰力
				SetRolePower(RolePower);
				m_SlotMemberList[i].SetRolePower(m_SlotMemberList[i].RolePower);
				//換位置資訊、Party3vs3Manager資料
				int tempPos = Position;
				Position = m_SlotMemberList[i].Position;
				pvp3vs3State.SetMyOneMemberData(RoleData,Position);
				m_SlotMemberList[i].Position = tempPos;
				pvp3vs3State.SetMyOneMemberData(m_SlotMemberList[i].RoleData,m_SlotMemberList[i].Position);
				break;
			}
		}
		this.transform.localPosition = Vector3.zero;
	}
}
