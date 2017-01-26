using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_PetPair : NGUIChildGUI 
{
	public UIButton		btnPet1Set			= null;					//Pet1Set
	public UIButton		btnPet2Set			= null;					//Pet2Set
	[Header("Pet1 Property")]
	public UILabel		lbPet1Name			= null;					//名稱
	public UILabel		lbPet1Level			= null;					//等級
	public UISprite[]	Pet1Stars			= new UISprite[4];		//突破星等
	public UIWidget		Pet1IconLoc			= null;
	public Slot_Item	Pet1Icon			= null;					//圖像
	public UILabel		Pet1lbCareerTag		= null; 				//職業字樣
	public UISprite		Pet1spTypeTag		= null; 				//型態標籤
	public UISprite		Pet1spFormation		= null;					//戰陣/出戰狀態
	public UISprite[]	Pet1EquipIcons		= new UISprite[5];		//裝備Icon
	public UILabel[]	Pet1lbStrengthens	= new UILabel[5];		//強化數
	public UISprite[]	Pet1Meltings		= new UISprite[5];		//熔煉顯示
	public UISprite[]	Pet1Masks			= new UISprite[5];		//邊框
	public UISprite[]	Pet1BackGrounds		= new UISprite[5];		//背景圖
	public UILabel		lbPet1AffectNum		= null;					//影響數值
	public UISprite		Pet1spStarEX		= null;					//加成角色顯示
	[Header("Pet2 Property")]
	public UILabel		lbPet2Name			= null;					//名稱
	public UILabel		lbPet2Level			= null;					//等級
	public UISprite[]	Pet2Stars			= new UISprite[4];		//突破星等
	public UIWidget		Pet2IconLoc			= null;
	public Slot_Item	Pet2Icon			= null;					//圖像
	public UILabel		Pet2lbCareerTag		= null; 				//職業字樣
	public UISprite		Pet2spTypeTag		= null; 				//型態標籤
	public UISprite		Pet2spFormation		= null;					//戰陣/出戰狀態
	public UISprite[]	Pet2EquipIcons		= new UISprite[5];		//裝備Icon
	public UILabel[]	Pet2lbStrengthens	= new UILabel[5];		//強化數
	public UISprite[]	Pet2Meltings		= new UISprite[5];		//熔煉顯示
	public UISprite[]	Pet2Masks			= new UISprite[5];		//邊框
	public UISprite[]	Pet2BackGrounds		= new UISprite[5];		//背景圖
	public UILabel		lbPet2AffectNum		= null;					//影響數值
	public UISprite		Pet2spStarEX		= null;					//加成角色顯示
	//
	[HideInInspector]
	public S_PetData pet1Data	= null;
	[HideInInspector]
	public S_PetData pet2Data	= null;
	[HideInInspector]
	public delegate void OnSelectThisSlot(S_PetData pd,float sDBPDvalue);
	[HideInInspector]
	public OnSelectThisSlot onSelectPet;
	//
	private Dictionary<ENUM_WearPosition,S_ItemData> PetEquipList	= new Dictionary<ENUM_WearPosition, S_ItemData>();
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_PetPair";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_PetPair() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		UIEventListener.Get(btnPet1Set.gameObject).onClick				+= SelectPet1Event;
		UIEventListener.Get(Pet1Icon.ButtonSlot.gameObject).onClick		+= SelectPet1Event;
		UIEventListener.Get(btnPet2Set.gameObject).onClick				+= SelectPet2Event;
		UIEventListener.Get(Pet2Icon.ButtonSlot.gameObject).onClick		+= SelectPet2Event;
	}
	//-------------------------------------------------------------------------------------------------
	private void SelectPet1Event(GameObject gb)
	{
		if(onSelectPet != null)
			onSelectPet(pet1Data,(float)btnPet1Set.userData);
	}
	//-------------------------------------------------------------------------------------------------
	private void SelectPet2Event(GameObject gb)
	{
		if(onSelectPet != null)
			onSelectPet(pet2Data,(float)btnPet2Set.userData);
	}
	//-------------------------------------------------------------------------------------------------
	//設定顯示資料
	public void SetData(PetPairData ppd,S_MobData_Tmp MobTmp = null,DungeonBonusPetData[] sDBPD = null)
	{
		if(ppd == null)
		{
			this.gameObject.SetActive(false);
			return;
		}
		//
		pet1Data = ppd.PetData1;
		pet2Data = ppd.PetData2;
		SetPet1Data(MobTmp,sDBPD);
		SetPet2Data(MobTmp,sDBPD);
		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	private void SetPet1Data(S_MobData_Tmp MobTmp = null,DungeonBonusPetData[] sDBPD = null)
	{
		if(pet1Data == null)
		{
			btnPet1Set.gameObject.SetActive(false);
			return;
		}
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pet1Data.iPetDBFID);
		btnPet1Set.gameObject.SetActive(true);
		btnPet1Set.userData = 0.0f;	//紀錄加成角色數值
		Pet1spStarEX.gameObject.SetActive(false);
		lbPet1Name.text 		= GameDataDB.GetString(pet1Data.GetPetName());
		lbPet1Name.text 		= lbPet1Name.text + (pet1Data.iPetLimitLevel == 0?"":"+"+pet1Data.iPetLimitLevel.ToString());
		lbPet1Level.text		= GameDataDB.GetString(1056)+" "+pet1Data.iPetLevel.ToString();
		pdTmp.SetRareColorString(lbPet1Name,true);
		//突破星數
		for(int i=0;i<Pet1Stars.Length;++i)
		{
			/*if(i<pet1Data.iPetLimitLevel)
				Pet1Stars[i].gameObject.SetActive(true);
			else*/
				Pet1Stars[i].gameObject.SetActive(false);
		}
		//設定寵物圖像
		int ItemID = pet1Data.GetPetItemID();
		if(ItemID>0)
		{
			Pet1Icon.SetSlotWithCount(ItemID,1,false,true);
			//Pet1Icon.SetDepth(30);
		}
		else
			Pet1Icon.gameObject.SetActive(false);
		//設定收集寵物擁有的裝備
		SetCollectPet1EquipDatas();
		//
		lbPet1AffectNum.gameObject.SetActive(true);
		if(MobTmp != null)
		{
			float TotalEffectValue = 0;
			if(pdTmp.IsWork(MobTmp.emCharClass))
			{
				TotalEffectValue = pdTmp.fAffectCharClass_Per;
			}
			TotalEffectValue += GameDataDB.GetCharacterTypeValueToMob(pet1Data.iPetDBFID,MobTmp.GUID);
			// 檢查是否是特定清單內
			if(sDBPD != null)
			{
				for(int i=0;i<sDBPD.Length;++i)
				{
					if(sDBPD[i].m_iBonusPet == pet1Data.iPetDBFID)
					{
						S_BuffData_Tmp buff = GameDataDB.BuffDataDB.GetData(sDBPD[i].m_iBonusPetBuff);
						if (null != buff)
						{
							TotalEffectValue += (float)buff.sAbility.fAttackDmgIncr_Per;
							btnPet1Set.userData = (float)buff.sAbility.fAttackDmgIncr_Per;
							Pet1spStarEX.gameObject.SetActive(true);
						}
						break;
					}
				}
			}//end if sDBPD != null

			lbPet1AffectNum.text = string.Format(GameDataDB.GetString(2680),(TotalEffectValue*100));
		}
		else
		{
			lbPet1AffectNum.text = string.Format(GameDataDB.GetString(2680),0);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void SetPet2Data(S_MobData_Tmp MobTmp = null,DungeonBonusPetData[] sDBPD = null)
	{
		if(pet2Data == null)
		{
			btnPet2Set.gameObject.SetActive(false);
			return;
		}
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pet2Data.iPetDBFID);
		btnPet2Set.gameObject.SetActive(true);
		btnPet2Set.userData = 0.0f;	//紀錄加成角色數值
		Pet2spStarEX.gameObject.SetActive(false);
		lbPet2Name.text 		= GameDataDB.GetString(pet2Data.GetPetName());
		lbPet2Name.text 		= lbPet2Name.text + (pet2Data.iPetLimitLevel == 0?"":"+"+pet2Data.iPetLimitLevel.ToString());
		lbPet2Level.text		= GameDataDB.GetString(1056)+" "+pet2Data.iPetLevel.ToString();
		pdTmp.SetRareColorString(lbPet2Name,true);
		//突破星數
		for(int i=0;i<Pet2Stars.Length;++i)
		{
			/*if(i<pet2Data.iPetLimitLevel)
				Pet2Stars[i].gameObject.SetActive(true);
			else*/
				Pet2Stars[i].gameObject.SetActive(false);
		}
		//設定寵物圖像
		int ItemID = pet2Data.GetPetItemID();
		if(ItemID>0)
		{
			Pet2Icon.SetSlotWithCount(ItemID,1,false,true);
			//Pet2Icon.SetDepth(30);
		}
		else
			Pet2Icon.gameObject.SetActive(false);
		//設定收集寵物擁有的裝備
		SetCollectPet2EquipDatas();
		lbPet2AffectNum.gameObject.SetActive(true);
		//
		if(MobTmp != null)
		{
			float TotalEffectValue = 0;
			if(pdTmp.IsWork(MobTmp.emCharClass))
			{
				TotalEffectValue = pdTmp.fAffectCharClass_Per;
			}
			TotalEffectValue += GameDataDB.GetCharacterTypeValueToMob(pet2Data.iPetDBFID,MobTmp.GUID);
			// 檢查是否是特定清單內
			if(sDBPD != null)
			{
				for(int i=0;i<sDBPD.Length;++i)
				{
					if(sDBPD[i].m_iBonusPet == pet2Data.iPetDBFID)
					{
						S_BuffData_Tmp buff = GameDataDB.BuffDataDB.GetData(sDBPD[i].m_iBonusPetBuff);
						if (null != buff)
						{
							TotalEffectValue += (float)buff.sAbility.fAttackDmgIncr_Per;
							btnPet2Set.userData = (float)buff.sAbility.fAttackDmgIncr_Per;
							Pet2spStarEX.gameObject.SetActive(true);
						}
						break;
					}
				}
			}//end if sDBPD != null

			lbPet2AffectNum.text = string.Format(GameDataDB.GetString(2680),(TotalEffectValue*100));
		}
		else
		{
			lbPet2AffectNum.text = string.Format(GameDataDB.GetString(2680),0);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void SetCollectPet1EquipDatas()
	{
		//收集
		PetEquipList.Clear();
		//擷取相對應裝備的物品
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			//剔除未裝備的
			if(tempItem.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None)
				continue;
			
			if(tempItem.iTargetID == pet1Data.iPetDBFID)
				PetEquipList.Add(tempItem.emWearPos,tempItem);
		}
		//先清除圖 強化數 熔煉 層級換色
		for(int i=0;i<Pet1EquipIcons.Length;++i)
		{
			Utility.ChangeAtlasSprite(Pet1EquipIcons[i],-1);
			Pet1lbStrengthens[i].gameObject.SetActive(false);
			Pet1Meltings[i].gameObject.SetActive(false);
			Pet1Masks[i].color = Color.white;
			Pet1BackGrounds[i].color = Color.white;
			Pet1EquipIcons[i].gameObject.SetActive(false);
		}
		//再設定
		if(PetEquipList.Count !=0)
		{
			foreach(ENUM_WearPosition wp in PetEquipList.Keys)
			{
				S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(PetEquipList[wp].ItemGUID);
				//換圖
				Pet1EquipIcons[(int)wp].gameObject.SetActive(true);
				Utility.ChangeAtlasSprite(Pet1EquipIcons[(int)wp],itemTmp.ItemIcon);
				//層級換色
				itemTmp.SetItemRarity(Pet1Masks[(int)wp],Pet1BackGrounds[(int)wp]);
				//物品強化數 
				if(PetEquipList[wp].iInherit[0]>0)
				{
					Pet1lbStrengthens[(int)wp].gameObject.SetActive(true);
					Pet1lbStrengthens[(int)wp].text = string.Format("+{0}",PetEquipList[wp].iInherit[0]);	//LabelStrengthen
				}
				//熔煉顯示
				if(PetEquipList[wp].iExp >0 || PetEquipList[wp].iMeltingLV>0)
				{
					Pet1Meltings[(int)wp].gameObject.SetActive(true);
				}
				else
				{
					Pet1Meltings[(int)wp].gameObject.SetActive(false);
				}
			}
		}
		//設定職業種族 出戰/戰陣
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pet1Data.iPetDBFID);
		Pet1lbCareerTag.text		= GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(Pet1spTypeTag,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//設定 出戰/戰陣與否
		bool bUsed = false;
		bUsed = (ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID == pet1Data.iPetDBFID);
		bUsed = bUsed == false? (ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID == pet1Data.iPetDBFID):bUsed;
		bUsed = bUsed == false? ARPGApplication.instance.CheckFormationNodes(pet1Data.iPetDBFID):bUsed;
		Pet1spFormation.gameObject.SetActive(bUsed);
	}
	//-------------------------------------------------------------------------------------------------
	private void SetCollectPet2EquipDatas()
	{
		//收集
		PetEquipList.Clear();
		//擷取相對應裝備的物品
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			//剔除未裝備的
			if(tempItem.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None)
				continue;
			
			if(tempItem.iTargetID == pet2Data.iPetDBFID)
				PetEquipList.Add(tempItem.emWearPos,tempItem);
		}
		//先清除圖 強化數 熔煉 層級換色
		for(int i=0;i<Pet1EquipIcons.Length;++i)
		{
			Utility.ChangeAtlasSprite(Pet2EquipIcons[i],-1);
			Pet2lbStrengthens[i].gameObject.SetActive(false);
			Pet2Meltings[i].gameObject.SetActive(false);
			Pet2Masks[i].color = Color.white;
			Pet2BackGrounds[i].color = Color.white;
			Pet2EquipIcons[i].gameObject.SetActive(false);
		}
		//再設定
		if(PetEquipList.Count !=0)
		{
			foreach(ENUM_WearPosition wp in PetEquipList.Keys)
			{
				S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(PetEquipList[wp].ItemGUID);
				//換圖
				Pet2EquipIcons[(int)wp].gameObject.SetActive(true);
				Utility.ChangeAtlasSprite(Pet2EquipIcons[(int)wp],itemTmp.ItemIcon);
				//層級換色
				itemTmp.SetItemRarity(Pet2Masks[(int)wp],Pet2BackGrounds[(int)wp]);
				//物品強化數 
				if(PetEquipList[wp].iInherit[0]>0)
				{
					Pet2lbStrengthens[(int)wp].gameObject.SetActive(true);
					Pet2lbStrengthens[(int)wp].text = string.Format("+{0}",PetEquipList[wp].iInherit[0]);	//LabelStrengthen
				}
				//熔煉顯示
				if(PetEquipList[wp].iExp >0 || PetEquipList[wp].iMeltingLV>0)
				{
					Pet2Meltings[(int)wp].gameObject.SetActive(true);
				}
				else
				{
					Pet2Meltings[(int)wp].gameObject.SetActive(false);
				}
			}
		}
		//設定職業種族 出戰/戰陣
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pet2Data.iPetDBFID);
		Pet2lbCareerTag.text		= GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(Pet2spTypeTag,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//設定 出戰/戰陣與否
		bool bUsed = false;
		bUsed = (ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID == pet2Data.iPetDBFID);
		bUsed = bUsed == false? (ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID == pet2Data.iPetDBFID):bUsed;
		bUsed = bUsed == false? ARPGApplication.instance.CheckFormationNodes(pet2Data.iPetDBFID):bUsed;
		Pet2spFormation.gameObject.SetActive(bUsed);
	}
	//-------------------------------------------------------------------------------------------------
}
