using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Skill : NGUIChildGUI 
{
	public UIWidget			WidgetSlotSkill		= null;
	public UISprite			SpriteBorder		= null; //技能框圖
	public UIButton			ButtonSlot			= null;
	public UISprite			SpriteSkillIcon		= null; //技能圖
	public UILabel			LabelLevel			= null; //技能等級(包含最大級顯示)
	public UISprite			spriteSkillLVBG		= null; //技能等級背景
	public UILabel			LabelSkillName		= null; //技能名稱(特規使用)
	public UISprite			SpriteNameBG		= null; //技能名稱底板
	public UISprite			Background			= null; //無技能背景圖
	//
	[HideInInspector]
	public int 				SkillGUID 			= -1;	//技能編號
	[HideInInspector]
	public int 				iLevel				= 0; 	//等級	
	//
	//private float			RestoreColorR		= -1;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Skill";
	[HideInInspector]
	public delegate void onClickBtnEvent(int sGUID,int index);
	[HideInInspector]
	public onClickBtnEvent onclickEvent;
	//
	[HideInInspector]
	public bool				bCanUnlock			= false;	//當啟用文字顏色判別是否可解鎖的狀況時紀錄當下可否解鎖的狀態
	//-------------------------------------------------------------------------------------------------
	private Slot_Skill() : base(GUI_SMARTOBJECT_NAME)
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
		LabelLevel.text = "--/--";
		LabelSkillName.text = "";
		LabelSkillName.gameObject.SetActive(false);
		SpriteNameBG.gameObject.SetActive(false);
		SkillGUID = -1;
	}
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		UIEventListener.Get(ButtonSlot.gameObject).onClick		+= BtnSlotClick;
	}
	//-------------------------------------------------------------------------------------------------
	private void BtnSlotClick(GameObject gb)
	{
		if(onclickEvent!= null)
			onclickEvent(SkillGUID,(int)ButtonSlot.userData);
	}
	//-------------------------------------------------------------------------------------------------
	//設定樣版資料
	public void SetSlotWithLevel(int guid, bool showName,bool showColor,bool isPet = false,S_PetData pd = null,ENUM_PetSkill psk = ENUM_PetSkill.ENUM_PetSkill_ASkillID,bool showLevel = true)
	{
		if(guid == -1)
		{
			//init
			//設定技能空圖
			Utility.ChangeAtlasSprite(SpriteSkillIcon,-1);
			//設定技能框
			Utility.ChangeAtlasSprite(SpriteBorder,47);			//銀
			InitialSlot();
			return;
		}


		S_SkillData_Tmp 	sDataTmp = GameDataDB.SkillDB.GetData(guid);
		if(sDataTmp == null)
			return;

		int iPetPSkillLoc = 0;
		SkillGUID = guid;
		//技能圖
		if(sDataTmp.ACTField>0)
			Utility.ChangeAtlasSprite(SpriteSkillIcon,sDataTmp.ACTField);
		//技能名稱
		string SkillName = GameDataDB.GetString(sDataTmp.SkillName);
		LabelSkillName.text = SkillName;
		SetSlotItemName(showName);

		if(isPet == false)
		{
			
			//S_SkillTalent_Tmp	skTalentTmp	= GameDataDB.SkillTalentDB.GetData(guid);
			//技能等級(如果沒有對應的技能等級即為未解鎖 圖要反灰處理
			S_SkillData skData =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSkillDataByDBID(guid);
			if(skData == null)
			{
				LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
				//RestoreColorR = SpriteSkillIcon.color.r;
				SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
			}
			else
			{
				LabelLevel.text = skData.iLv.ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
				//if(RestoreColorR >0)
				SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				iLevel = skData.iLv;
			}
			/*//設定技能框
			if(sDataTmp.SkillType == ENUM_SkillType.ENUM_SkillType_S)
				Utility.ChangeAtlasSprite(SpriteBorder,45);			//金鑲
			else
			{
				bool bGroupPassive = false;
				//檢查前置技能
				for(int i=0;i<GameDefine.SKILLTALENT_PRESKILL;++i)
				{
					if(skTalentTmp.sPreSkill[i].iPreSkillID<=0)
						continue;
					
					bGroupPassive = true;
				}
				if(bGroupPassive)
					Utility.ChangeAtlasSprite(SpriteBorder,46);			//金
				else
					Utility.ChangeAtlasSprite(SpriteBorder,47);			//銀
			}*/
		}
		else
		{
			switch(psk)
			{
			case ENUM_PetSkill.ENUM_PetSkill_ASkillID:
				LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
				iLevel = pd.iSkillLv[(int)psk];
				break;
			case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
				iPetPSkillLoc = 0;
				if(pd.bPSkill[0] == 0)
				{
					LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				}
				else
				{
					LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
					iLevel = pd.iSkillLv[(int)psk];
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
				iPetPSkillLoc = 1;
				if(pd.bPSkill[1] == 0)
				{
					LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				}
				else
				{
					LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
					iLevel = pd.iSkillLv[(int)psk];
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
				if(pd.iPetLimitLevel<=0)
				{
					LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				}
				else
				{
					LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
					iLevel = pd.iSkillLv[(int)psk];
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
				if(pd.iPetLimitLevel<2)
				{
					LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				}
				else
				{
					LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
					iLevel = pd.iSkillLv[(int)psk];
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
				if(pd.iPetLimitLevel<3)
				{
					LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				}
				else
				{
					LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
					iLevel = pd.iSkillLv[(int)psk];
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
				if(pd.iPetLimitLevel<4)
				{
					LabelLevel.text = "0/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(0,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
				}
				else
				{
					LabelLevel.text = pd.iSkillLv[(int)psk].ToString()+"/"+sDataTmp.iUpgradeLimitSkillLv.ToString();
					SpriteSkillIcon.color = new Color(1,SpriteSkillIcon.color.g,SpriteSkillIcon.color.b);
					iLevel = pd.iSkillLv[(int)psk];
				}
				break;
			}
		}
		//設定技能框
		if(sDataTmp.SkillType == ENUM_SkillType.ENUM_SkillType_S)
			Utility.ChangeAtlasSprite(SpriteBorder,45);			//金鑲
		else
			Utility.ChangeAtlasSprite(SpriteBorder,47);			//銀

		if(showColor == true)
			//改變文字顏色與框
			LabelLevel.text = ChangeTextColorByCondition(LabelLevel.text,guid,isPet,pd,iPetPSkillLoc,psk);
		//控制等級顯示
		LabelLevel.gameObject.SetActive(showLevel);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlotItemName(bool bshow)
	{
		//技能名稱(特規使用)
		if(bshow)
		{
			LabelSkillName.gameObject.SetActive(bshow);
			LabelLevel.gameObject.SetActive(bshow);
			spriteSkillLVBG.gameObject.SetActive(!bshow);
		}
		else
		{
			LabelSkillName.gameObject.SetActive(bshow);
			Vector3 skillloc = LabelSkillName.transform.localPosition + Vector3.up*15;
			LabelLevel.transform.localPosition = skillloc;
			spriteSkillLVBG.transform.localPosition = skillloc;
			LabelLevel.gameObject.SetActive(!bshow);
			spriteSkillLVBG.gameObject.SetActive(!bshow);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetDepth(int depth)
	{
		WidgetSlotSkill.depth	+= depth;
		SpriteBorder.depth		+= depth; 	//技能框圖
		SpriteSkillIcon.depth	+= depth;	//技能圖
		LabelLevel.depth		+= depth;	//技能等級
		LabelSkillName.depth	+= depth;	//技能名稱(特規使用)
		SpriteNameBG.depth		+= depth;	//技能名稱底板
		Background.depth		+= depth;	//無技能背景圖
	}
	//-------------------------------------------------------------------------------------------------
	private string ChangeTextColorByCondition(string str,int SkillID,bool isPet,S_PetData pd,int pLoc,ENUM_PetSkill psk)
	{
		S_SkillData 		skData 		=  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSkillDataByDBID(SkillID);
		S_SkillTalent_Tmp	skTalentTmp	= GameDataDB.SkillTalentDB.GetData(SkillID);
		S_SkillData_Tmp 	skDataTmp 	= GameDataDB.SkillDB.GetData(SkillID);
		int MoneyCount 	= 0;
		int ItemCount 	= 0;
		if(skTalentTmp != null)
		{
			MoneyCount 	= skTalentTmp.iCostMoney;
			ItemCount 	= skTalentTmp.sUnLockItem[0].iCostItemCount;
		}

		SpriteBorder.color = Color.white;
		//寵物技能的解鎖與升級
		if(isPet)
		{
			bool bUpgrade = true;

			//解鎖
			switch(psk)
			{
			case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
			case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
				if(pd.bPSkill[pLoc] == 0)
				{
					S_PetData_Tmp	pdtmp		= GameDataDB.PetDB.GetData(pd.iPetDBFID);
					S_Item_Tmp skillBook = GameDataDB.ItemDB.GetData(pdtmp.PassiveSkill[pLoc].iPSkillCostItemID);
					if(skillBook!=null)
					{
						//技能書數量
						int GetSkillBookCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skillBook.GUID);
						if(pdtmp.PassiveSkill[pLoc].iPSkillCostItemCount>GetSkillBookCount)
						{
							SpriteBorder.color = new Color(0,1,1);
							bUpgrade = false;
						}
					}
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
				if(pd.iPetLimitLevel<=0)
				{
					SpriteBorder.color = new Color(0,1,1);
					bUpgrade = false;
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
				if(pd.iPetLimitLevel<2)
				{
					SpriteBorder.color = new Color(0,1,1);
					bUpgrade = false;
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
				if(pd.iPetLimitLevel<3)
				{
					SpriteBorder.color = new Color(0,1,1);
					bUpgrade = false;
				}
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
				if(pd.iPetLimitLevel<4)
				{
					SpriteBorder.color = new Color(0,1,1);
					bUpgrade = false;
				}
				break;
			}
			if(bUpgrade == false)
			{
				str = string.Format("{0}{1}{2}",GameDataDB.GetString(1328),str,GameDataDB.GetString(1329)); //白色
				return str;
			}
			//
			//先檢查是否已達最大值
			if(pd.iSkillLv[(int)psk] >= skDataTmp.iUpgradeLimitSkillLv)
			{
				str = string.Format("{0}{1}{2}",GameDataDB.GetString(2728),str,GameDataDB.GetString(1329)); //橙色
				return str;
			}
			//升級
			if(pd.iSkillLv[(int)psk]<skDataTmp.iUpgradeLimitSkillLv && bUpgrade == true)
			{
				//檢查文錢
				MoneyCount = ARPGApplication.instance.CalculateUpgradeMoneyCount(skDataTmp.fUpgradeCostMoneyRatio,pd.iSkillLv[(int)psk]);
				if(MoneyCount>0 && ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney()<MoneyCount)
				{
					bUpgrade = false;
				}
				//升級道具
				ItemCount = ARPGApplication.instance.CalculateUpgradeItemCount(skDataTmp.fUpgradeCostItemRatio,pd.iSkillLv[(int)psk]);
				if(ItemCount>0)
				{
					S_ItemData item 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skDataTmp.iUpgradeCostItemID);
					if(item == null)
					{
						bUpgrade = false;
					}
					else
					{
						int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
						if(CurrentItemCount<ItemCount)
						{
							bUpgrade = false;
						}
					}
				}
				//檢查技能等級是否超過寵物等級
				if(pd.iSkillLv[(int)psk] >= pd.iPetLevel)
				{
					switch(psk)
					{
					case ENUM_PetSkill.ENUM_PetSkill_ASkillID:
						bUpgrade = false;
						break;
					case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
					case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
						if(pd.bPSkill[pLoc] == 1)
							bUpgrade = false;
						break;
					case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
						if(pd.iPetLimitLevel>0)
							bUpgrade = false;
						break;
					case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
						if(pd.iPetLimitLevel>1)
							bUpgrade = false;
						break;
					case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
						if(pd.iPetLimitLevel>2)
							bUpgrade = false;
						break;
					case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
						if(pd.iPetLimitLevel>3)
							bUpgrade = false;
						break;
					}
				}

			}//end if
			if(bUpgrade == false)
			{
				str = string.Format("{0}{1}{2}",GameDataDB.GetString(1327),str,GameDataDB.GetString(1329)); //紅色
				return str;
			}

			str = string.Format("{0}{1}{2}",GameDataDB.GetString(2727),str,GameDataDB.GetString(1329)); //綠色
			return str;
		}
		else
		{
			if(skData == null)	
			{
				bCanUnlock = true;
				//未解鎖狀態
				//檢查文錢
				if(MoneyCount>0 && ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney()<MoneyCount)
					bCanUnlock = false;
				//檢查自身等級
				int UpGradeNeededLV = skTalentTmp.iPlayerLv;
				if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel()<UpGradeNeededLV)
					bCanUnlock = false;
				//檢查前置技能
				for(int i=0;i<GameDefine.SKILLTALENT_PRESKILL;++i)
				{
					if(skTalentTmp.sPreSkill[i].iPreSkillID<=0)
						continue;

					S_SkillData PreskData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSkillDataByDBID(skTalentTmp.sPreSkill[i].iPreSkillID);
					if(PreskData == null)
						bCanUnlock = false;
					else
					{
						if(PreskData.iLv<skTalentTmp.sPreSkill[i].iPreSkillLv)
							bCanUnlock = false;
					}
				}
				//檢查解鎖道具
				if(ItemCount>0)
				{
					S_ItemData item 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skTalentTmp.sUnLockItem[0].iCostItemID);
					if(item == null)
						bCanUnlock = false;
					else
					{
						int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skTalentTmp.sUnLockItem[0].iCostItemID);
						if(item.iCount<ItemCount)
							bCanUnlock = false;
					}
				}
				//設定顏色
				if(bCanUnlock == true)
					str = string.Format("{0}{1}{2}",GameDataDB.GetString(2727),str,GameDataDB.GetString(1329)); //綠色
				else
				{
					str = string.Format("{0}{1}{2}",GameDataDB.GetString(1328),str,GameDataDB.GetString(1329)); //白色
					SpriteBorder.color = new Color(0,1,1);
				}

			}
			else
			{
				//已解鎖狀態
				if(skData.iLv >= skDataTmp.iUpgradeLimitSkillLv)
					str = string.Format("{0}{1}{2}",GameDataDB.GetString(2728),str,GameDataDB.GetString(1329)); //橙色
				else
				{
					bCanUnlock = true;
					//檢查文錢
					MoneyCount = ARPGApplication.instance.CalculateUpgradeMoneyCount(skDataTmp.fUpgradeCostMoneyRatio,skData.iLv);
					if(MoneyCount>0 && ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney()<MoneyCount)
						bCanUnlock = false;
					//檢查自身等級
					int UpGradeNeededLV = ((skData.iLv+1)*skDataTmp.iUpgradPreRoleLv);
					int RoleCurrentLV	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
					if(RoleCurrentLV<UpGradeNeededLV)
						bCanUnlock = false;
					//檢查等級條件
					if(skData.iLv>=RoleCurrentLV)
						bCanUnlock = false;
					//升級道具
					if(skDataTmp.iUpgradeCostItemID>0)
					{
						ItemCount = ARPGApplication.instance.CalculateUpgradeItemCount(skDataTmp.fUpgradeCostItemRatio,skData.iLv);
						if(ItemCount>0)
						{
							S_ItemData item 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skDataTmp.iUpgradeCostItemID);
							if(item == null)
								bCanUnlock = false;
							else
							{
								int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
								if(item.iCount<ItemCount)
									bCanUnlock = false;
							}
						}
					}
					//設定顏色
					if(bCanUnlock == true)
						str = string.Format("{0}{1}{2}",GameDataDB.GetString(2727),str,GameDataDB.GetString(1329)); //綠色
					else
					{
						str = string.Format("{0}{1}{2}",GameDataDB.GetString(1327),str,GameDataDB.GetString(1329)); //紅色
						//SpriteBorder.color = new Color(0,1,1);
					}
				}
			}
		}
		return str;
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}
