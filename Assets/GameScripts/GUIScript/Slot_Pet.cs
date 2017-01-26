using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Pet : NGUIChildGUI 
{
	public UIButton		btnPetSet			= null;					//PetSet
	[Header("Pet Property")]
	public UILabel		lbPetName			= null;					//名稱
	public UILabel		lbPetLevel			= null;					//等級
	public UISprite[]	PetStars			= new UISprite[4];		//突破星等
	public UIWidget		PetIconLoc			= null;
	public Slot_Item	PetIcon				= null;					//圖像
	public UILabel		PetlbCareerTag		= null; 				//職業字樣
	public UISprite		PetspTypeTag		= null; 				//型態標籤
	public UISprite		PetspFormation		= null;					//戰陣/出戰狀態
	public UISprite[]	PetEquipIcons		= new UISprite[5];		//裝備Icon
	public UILabel[]	PetlbStrengthens	= new UILabel[5];		//強化數
	public UISprite[]	PetMeltings			= new UISprite[5];		//熔煉顯示
	public UISprite[]	PetMasks			= new UISprite[5];		//邊框
	public UISprite[]	PetBackGrounds		= new UISprite[5];		//背景圖
	public UILabel		lbPetAffectNum		= null;
	//
	[HideInInspector]
	public S_PetData petData	= null;
	[HideInInspector]
	public delegate void OnSelectThisSlot(S_PetData pd);
	[HideInInspector]
	public OnSelectThisSlot onSelectPet;
	//
	private Dictionary<ENUM_WearPosition,S_ItemData> PetEquipList	= new Dictionary<ENUM_WearPosition, S_ItemData>();
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Pet";
	//-------------------------------------------------------------------------------------------------
	private Slot_Pet() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		UIEventListener.Get(btnPetSet.gameObject).onClick				+= SelectPetEvent;
		UIEventListener.Get(PetIcon.ButtonSlot.gameObject).onClick		+= SelectPetEvent;
	}
	//-------------------------------------------------------------------------------------------------
	private void SelectPetEvent(GameObject gb)
	{
		if(onSelectPet != null)
			onSelectPet(petData);
	}
	//-------------------------------------------------------------------------------------------------
	//設定顯示資料
	public void SetData(S_PetData pd,S_MobData_Tmp EnemyTmp = null,S_PetData_Tmp EnemyPetTmp = null,float sDBPDvalue = 0)
	{
		if(pd == null)
		{
			this.gameObject.SetActive(false);
			return;
		}
		//
		petData = pd;
		SetPetData(EnemyTmp,EnemyPetTmp,sDBPDvalue);
		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	private void SetPetData(S_MobData_Tmp EnemyTmp,S_PetData_Tmp EnemyPetTmp,float sDBPDvalue = 0)
	{
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(petData.iPetDBFID);
		btnPetSet.gameObject.SetActive(true);
		btnPetSet.userData = petData.iPetDBFID;
		lbPetName.text 		= GameDataDB.GetString(petData.GetPetName());
		lbPetName.text 		= lbPetName.text + (petData.iPetLimitLevel == 0?"":"+"+petData.iPetLimitLevel.ToString());
		lbPetLevel.text		= GameDataDB.GetString(1056)+" "+petData.iPetLevel.ToString();
		pdTmp.SetRareColorString(lbPetName,true);
		//突破星數
		for(int i=0;i<PetStars.Length;++i)
		{
			/*if(i<petData.iPetLimitLevel)
				PetStars[i].gameObject.SetActive(true);
			else*/
				PetStars[i].gameObject.SetActive(false);
		}
		//設定寵物圖像
		PetIcon.SetSlotWithPetID(petData.iPetDBFID,false,true);
		PetIcon.SetDepth(30);

		//設定收集寵物擁有的裝備
		SetCollectPetEquipDatas();
		//
		lbPetAffectNum.gameObject.SetActive(true);
		float TotalEffectValue = 0;
		if(EnemyTmp != null)
		{
			if(pdTmp.IsWork(EnemyTmp.emCharClass))
			{
				TotalEffectValue = pdTmp.fAffectCharClass_Per;
			}
			TotalEffectValue += GameDataDB.GetCharacterTypeValueToMob(petData.iPetDBFID,EnemyTmp.GUID);
			TotalEffectValue += sDBPDvalue;
			lbPetAffectNum.text = string.Format(GameDataDB.GetString(2680),(TotalEffectValue*100));
		}
		else
		{
			if(EnemyPetTmp != null)
			{
				if(pdTmp.IsWork(EnemyPetTmp.emCharClass))
				{
					TotalEffectValue = pdTmp.fAffectCharClass_Per;
				}
				TotalEffectValue += GameDataDB.GetCharacterTypeValueToPet(petData.iPetDBFID,EnemyTmp.GUID);
				TotalEffectValue += sDBPDvalue;
				lbPetAffectNum.text = string.Format(GameDataDB.GetString(2680),(TotalEffectValue*100));
			}
			else
				lbPetAffectNum.text = string.Format(GameDataDB.GetString(2680),0);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void SetCollectPetEquipDatas()
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
			
			if(tempItem.iTargetID == petData.iPetDBFID)
				PetEquipList.Add(tempItem.emWearPos,tempItem);
		}
		//先清除圖 強化數 熔煉 層級換色
		for(int i=0;i<PetEquipIcons.Length;++i)
		{
			Utility.ChangeAtlasSprite(PetEquipIcons[i],-1);
			PetlbStrengthens[i].gameObject.SetActive(false);
			PetMeltings[i].gameObject.SetActive(false);
			PetMasks[i].color = Color.white;
			PetBackGrounds[i].color = Color.white;
			PetEquipIcons[i].gameObject.SetActive(false);
		}
		//再設定
		if(PetEquipList.Count !=0)
		{
			foreach(ENUM_WearPosition wp in PetEquipList.Keys)
			{
				S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(PetEquipList[wp].ItemGUID);
				//換圖
				PetEquipIcons[(int)wp].gameObject.SetActive(true);
				Utility.ChangeAtlasSprite(PetEquipIcons[(int)wp],itemTmp.ItemIcon);
				//層級換色
				itemTmp.SetItemRarity(PetMasks[(int)wp],PetBackGrounds[(int)wp]);
				//物品強化數 
				if(PetEquipList[wp].iInherit[0]>0)
				{
					PetlbStrengthens[(int)wp].gameObject.SetActive(true);
					PetlbStrengthens[(int)wp].text = string.Format("+{0}",PetEquipList[wp].iInherit[0]);	//LabelStrengthen
				}
				//熔煉顯示
				if(PetEquipList[wp].iExp >0 || PetEquipList[wp].iMeltingLV>0)
				{
					PetMeltings[(int)wp].gameObject.SetActive(true);
				}
				else
				{
					PetMeltings[(int)wp].gameObject.SetActive(false);
				}
			}
		}
		//設定職業種族 出戰/戰陣
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(petData.iPetDBFID);
		PetlbCareerTag.text		= GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(PetspTypeTag,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//設定 出戰/戰陣與否
		bool bUsed = false;
		bUsed = (ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID == petData.iPetDBFID);
		bUsed = bUsed == false? (ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID == petData.iPetDBFID):bUsed;
		bUsed = bUsed == false? ARPGApplication.instance.CheckFormationNodes(petData.iPetDBFID):bUsed;
		PetspFormation.gameObject.SetActive(bUsed);
	}
	//-------------------------------------------------------------------------------------------------
}
