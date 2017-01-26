using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_PetSkillUpInfo : MonoBehaviour
{
	public UIButton			btnSkillFrame	= null; //技能圖按鈕
	public UISprite			SkillFrame		= null; //技能圖
	public UILabel			lbSkilName 		= null; //技能名稱
	public UILabel			lbSkillLV 		= null; //技能等級
	public UIButton			btnUpUnlock 	= null; //技能解鎖/升級/快速升
	public UISprite			spCostItem		= null; //消耗物品圖
	public UILabel			lbCostItemNum	= null; //消耗物品數
	public UILabel			lbMoneyCost		= null; //消耗金額
	public UISprite			spLimitBreak	= null; //突破字樣


	//
	[HideInInspector]
	public int				iSkillID			= 0;
	[HideInInspector]
	public Slot_Skill		SkillSlot		= null;
	[HideInInspector]
	public S_PetData		PetData;
	[HideInInspector]
	public int 				iPSkillPos					= -1;

	private Color			GraySprite		= new Color(0,1,1);
	private Color			NormalSprite	= new Color(1,1,1);
	private ENUM_PetSkill	petSkillType	= ENUM_PetSkill.ENUM_PetSkill_Max;
	[HideInInspector]
	public bool				bSkillUpgrade			= false;//判斷按鈕是用來升級還是學習
	[HideInInspector]
	public bool				bCanUpgrade				= true; //判斷是否可以升級
	//
	private	int				NeededItemID		= 0;
	private bool			SpeedFlag			= false; 	//控制press事件
	private float 			PressTime 			= 0;		//控制press事件
	//
	[HideInInspector]public int iCurrentSkillBookCount		= 0;
	[HideInInspector]public int iCurrentNeedBookCount		= 0;
	//
	[HideInInspector]
	public delegate void OnPressButton(int skillID,bool bSkillUp,bool bCanUp,ENUM_PetSkill pskill);
	[HideInInspector]
	public OnPressButton onPressButtonEvent;
	[HideInInspector]
	public delegate void OnClickSkillFrame(ENUM_PetSkill pskill,S_PetData pd);
	[HideInInspector]
	public OnClickSkillFrame onClickSkill;
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		UIEventListener.Get(btnUpUnlock.gameObject).onClick		+= GetLearnPassiveSkill;
		UIEventListener.Get(btnUpUnlock.gameObject).onPress		+= PressEvent;				//外部定義事件宣告
		UIEventListener.Get(SkillFrame.gameObject).onClick		+= ClickSkillEvent;			//外部定義事件宣告
		UIEventListener.Get(spCostItem.gameObject).onClick		+= OpenItemDetail;
	}
	//-------------------------------------------------------------------------------------------------
	void Update()
	{
		if(SpeedFlag)
		{
			PressTime += Time.deltaTime;
			if(PressTime>1)
			{
				if(onPressButtonEvent != null)
					onPressButtonEvent(iSkillID,bSkillUpgrade,bCanUpgrade,petSkillType);

				SpeedFlag = false;
				PressTime=0;
			}
		}

		if(!SpeedFlag && PressTime!=0)
			PressTime = 0;
	}
	//-------------------------------------------------------------------------------------------------
	//設定資訊
	public void SetSkillData(ENUM_PetSkill skillType, int SkillID,S_PetData pd)
	{
		if(pd==null)
			return;

		PetData = pd;

		Slot_Skill go = ResourceManager.Instance.GetGUI("Slot_Skill").GetComponent<Slot_Skill>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Skill load prefeb error") );
			return;
		}

		iSkillID = SkillID;
		petSkillType = skillType;
		//插入技能預設圖
		switch(skillType)
		{
		case ENUM_PetSkill.ENUM_PetSkill_ASkillID:
			spLimitBreak.gameObject.SetActive(false);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
			Utility.ChangeAtlasSprite(SkillFrame,17500);
			spLimitBreak.gameObject.SetActive(false);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
			Utility.ChangeAtlasSprite(SkillFrame,17501);
			spLimitBreak.gameObject.SetActive(false);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
			Utility.ChangeAtlasSprite(SkillFrame,17502);
			spLimitBreak.gameObject.SetActive(true);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
			Utility.ChangeAtlasSprite(SkillFrame,17503);
			spLimitBreak.gameObject.SetActive(true);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
			Utility.ChangeAtlasSprite(SkillFrame,17504);
			spLimitBreak.gameObject.SetActive(true);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
			Utility.ChangeAtlasSprite(SkillFrame,17505);
			spLimitBreak.gameObject.SetActive(true);
			break;
		}
		//設定技能圖
		S_SkillData_Tmp skillTmp = GameDataDB.SkillDB.GetData(SkillID);
		if(skillTmp == null)
			return;

		if(skillTmp.ACTField>0)
		{
			SkillSlot = CreateProcess(go,SkillFrame.transform,65);
			if(SkillSlot !=null)
			{
				SkillSlot.SetSlotWithLevel(SkillID,false,false,true,pd,skillType,false);
				SkillSlot.ButtonSlot.userData = (ENUM_PetSkill)btnSkillFrame.userData;
				UIEventListener.Get(SkillSlot.ButtonSlot.gameObject).onClick		-= ClickSkillEvent;
				UIEventListener.Get(SkillSlot.ButtonSlot.gameObject).onClick		+= ClickSkillEvent;
			}
		}
		//設定技能資訊
		lbSkilName.text 		= GameDataDB.GetString(skillTmp.SkillName);
		lbSkillLV.text			= pd.iSkillLv[(int)skillType]<=0?"":string.Format("{0}{1}{2}",GameDataDB.GetString(1056),":",pd.iSkillLv[(int)skillType].ToString());
		switch(skillType)
		{
		case ENUM_PetSkill.ENUM_PetSkill_ASkillID:
			UpgradeProcess(skillType);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
			if(pd.bPSkill[0] == 0)
				UnLockProcess(skillType);
			else
				UpgradeProcess(skillType);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
			if(pd.bPSkill[1] == 0)
				UnLockProcess(skillType);
			else
				UpgradeProcess(skillType);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
			if(pd.iPetLimitLevel==0)
				UnLockProcess(skillType);
			else
				UpgradeProcess(skillType);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
			if(pd.iPetLimitLevel<2)
				UnLockProcess(skillType);
			else
				UpgradeProcess(skillType);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
			if(pd.iPetLimitLevel<3)
				UnLockProcess(skillType);
			else
				UpgradeProcess(skillType);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
			if(pd.iPetLimitLevel<4)
				UnLockProcess(skillType);
			else
				UpgradeProcess(skillType);
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void UnLockProcess(ENUM_PetSkill pskill)
	{
		S_PetData_Tmp	pdtmp		= GameDataDB.PetDB.GetData(PetData.iPetDBFID);

		SkillFrame.color = GraySprite;
		lbSkillLV.gameObject.SetActive(false);

		switch(pskill)
		{
		case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
			iPSkillPos =0;
			SetLearnPassiveSkill(pdtmp.PassiveSkill[0]);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
			iPSkillPos =1;
			SetLearnPassiveSkill(pdtmp.PassiveSkill[1]);
			break;
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
		case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
			btnUpUnlock.gameObject.SetActive(false);
			spCostItem.gameObject.SetActive(false);
			lbMoneyCost.gameObject.SetActive(false);
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	//被動技能顯示設定
	private void SetLearnPassiveSkill(S_PassiveSkill spskill)
	{
		int GetSkillBookCount =0;
		S_Item_Tmp skillBook = GameDataDB.ItemDB.GetData(spskill.iPSkillCostItemID);
		//解鎖物品
		if(spskill.iPSkillCostItemID<=0)
		{
			spCostItem.gameObject.SetActive(false);
		}
		else
		{
			//設定技能書圖示
			if(skillBook != null)
			{
				//skillBook.SetItemRarity(spriteBorder,spriteBG);
				spCostItem.gameObject.SetActive(true);
				Utility.ChangeAtlasSprite(spCostItem,skillBook.ItemIcon);
				//NeededItemID = skillBook.GUID;
			}
			//技能書數量
			GetSkillBookCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skillBook.GUID);
			//先判斷是否需要數量
			if(spskill.iPSkillCostItemCount == 0)
			{
				spCostItem.gameObject.SetActive(false);
			}
			else
				lbCostItemNum.text = GetSkillBookCount.ToString()+"/"+spskill.iPSkillCostItemCount.ToString();
		}
		//設定金錢(解鎖不需要有)
		lbMoneyCost.gameObject.SetActive(false);
		
		iCurrentSkillBookCount = GetSkillBookCount;
		iCurrentNeedBookCount = spskill.iPSkillCostItemCount;
	}
	//-------------------------------------------------------------------------------------------------
	private void UpgradeProcess(ENUM_PetSkill pskill)
	{
		//init
		int ItemCount = 0;
		int MoneyCount = 0;
		bCanUpgrade = true;
		S_SkillData_Tmp skillData 	= GameDataDB.SkillDB.GetData(iSkillID);
		Dictionary<int,S_SkillData> PSkillList = PetData.GetTalentSkill();
		//預設開啟
		SkillFrame.color = NormalSprite;
		lbSkillLV.gameObject.SetActive(true);
		btnUpUnlock.gameObject.SetActive(true);
		
		if(PSkillList[iSkillID].iLv >= skillData.iUpgradeLimitSkillLv)
		{
			btnUpUnlock.gameObject.SetActive(false);
			spCostItem.gameObject.SetActive(false);
			lbMoneyCost.gameObject.SetActive(false);
			bCanUpgrade = false;
			return;
		}

		bSkillUpgrade = true;
		//
		//升級道具
		if(skillData.fUpgradeCostItemRatio<=0)
		{
			spCostItem.gameObject.SetActive(false);
		}
		else
		{
			spCostItem.gameObject.SetActive(true);
			ItemCount = ARPGApplication.instance.CalculateUpgradeItemCount(skillData.fUpgradeCostItemRatio,PetData.iSkillLv[(int)pskill]);
			if(ItemCount<=0)
			{
				spCostItem.gameObject.SetActive(false);
			}
			else
			{
				S_Item_Tmp costItem = GameDataDB.ItemDB.GetData(skillData.iUpgradeCostItemID);
				Utility.ChangeAtlasSprite(spCostItem,costItem.ItemIcon);
				lbCostItemNum.text = GetUpgradeItemColorString(ItemCount,skillData.iUpgradeCostItemID);
				NeededItemID = skillData.iUpgradeCostItemID;
			}
		}
		//升級金錢
		if(skillData.fUpgradeCostMoneyRatio<=0)
			lbMoneyCost.gameObject.SetActive(false);
		else
		{
			lbMoneyCost.gameObject.SetActive(true);
			MoneyCount = ARPGApplication.instance.CalculateUpgradeMoneyCount(skillData.fUpgradeCostMoneyRatio,PetData.iSkillLv[(int)pskill]);
			if(MoneyCount<=0)
				lbMoneyCost.gameObject.SetActive(false);
			else
			{
				lbMoneyCost.gameObject.SetActive(true);
				lbMoneyCost.text = GetUpgradeMoneyColorString(MoneyCount);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	private Slot_Skill CreateProcess(Slot_Skill prefab,Transform parentObj,int iDepth)
	{
		if(parentObj == null)
			return null;

		if(parentObj.GetComponentInChildren<Slot_Skill>() != null)
			return parentObj.GetComponentInChildren<Slot_Skill>();
		
		Slot_Skill newgo = Instantiate(prefab) as Slot_Skill;
		
		newgo.transform.parent			= parentObj;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		newgo.transform.localPosition	= Vector3.zero;
		newgo.gameObject.SetActive(true);
		newgo.SetDepth(iDepth);
		return newgo;
	}
	//-------------------------------------------------------------------------------------------------
	//生成顯示升級金錢的字樣與條件顏色
	private string GetUpgradeMoneyColorString(int MoneyCount) 
	{
		int CurrentMoney = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney();
		if(CurrentMoney>=MoneyCount)
			return string.Format("{0}{1}{2}",GameDataDB.GetString(2727),MoneyCount.ToString(),GameDataDB.GetString(1329));
		
		//其他狀況就直接傳回紅色字樣
		bCanUpgrade = false;
		return string.Format("{0}{1}{2}",GameDataDB.GetString(1327),MoneyCount.ToString(),GameDataDB.GetString(1329));
	}
	//-------------------------------------------------------------------------------------------------
	//生成顯示升級道具的字樣與條件顏色
	private string GetUpgradeItemColorString(int ItemCount,int ItemGUID) 
	{
		bool bCanUpGrade = false;
		S_Item_Tmp iDataTmp = GameDataDB.ItemDB.GetData(ItemGUID);
		S_ItemData iData 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(ItemGUID);
		//技能書數量
		int GetSkillBookCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(ItemGUID);
		//有這個道具且數量足夠即直接傳回綠色字樣
		if(iData != null && GetSkillBookCount>=ItemCount)
			return string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(2727),GetSkillBookCount,"/",ItemCount,GameDataDB.GetString(1329));
		
		//其他狀況就直接傳回紅色字樣
		bCanUpgrade = false;
		return string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(1327),GetSkillBookCount,"/",ItemCount,GameDataDB.GetString(1329));
	}
	//-------------------------------------------------------------------------------------------------
	#region 事件宣告
	//-------------------------------------------------------------------------------------------------
	//被動技能學習事件
	private void GetLearnPassiveSkill(GameObject gb)
	{
		//學習功能
		if(bSkillUpgrade == false)
		{
			//取得技能書名稱
			if(iPSkillPos<0)
				return;

			int iSkillBookID = GameDataDB.PetDB.GetData(PetData.iPetDBFID).PassiveSkill[iPSkillPos].iPSkillCostItemID;
			S_Item_Tmp skillbookTmp = GameDataDB.ItemDB.GetData(iSkillBookID);
			//不足時
			if(iCurrentSkillBookCount < iCurrentNeedBookCount)
			{
				if (skillbookTmp != null)
				{
					string str = skillbookTmp.GetNameWithColor()+" "+GameDataDB.GetString(561);		//"(技能書)不足"
					ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
				}
				return;
			}
			//足夠時
			JsonSlot_Pet.Send_CtoM_PetPSkillUnlock(PetData.iPetDBFID,iPSkillPos);
		}
		else
			//升級功能
		{
			S_SkillData_Tmp 	skDataTmp 	= GameDataDB.SkillDB.GetData(iSkillID);
			//技能等級已達最大
			if(PetData.iSkillLv[(int)petSkillType]>=skDataTmp.iUpgradeLimitSkillLv)
			{
				ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(9702)); //技能已是最高級。
				MusicControlSystem.StopOnceSound(GameDefine.SKILL_NONUP_SOUND);
				MusicControlSystem.PlaySound(GameDefine.SKILL_NONUP_SOUND,1);
				return;
			}
			//技能等級再升上去會超過寵物本身等級
			if(PetData.iSkillLv[(int)petSkillType] == PetData.iPetLevel)
			{
				ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(9715)); //無法超過角色最大等級!。
				MusicControlSystem.StopOnceSound(GameDefine.SKILL_NONUP_SOUND);
				MusicControlSystem.PlaySound(GameDefine.SKILL_NONUP_SOUND,1);
				return;
			}
			//還有限制條件未達成
			if(bCanUpgrade == false)
			{
				ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(9703)); //條件不足，請詳閱技能條件！
				MusicControlSystem.StopOnceSound(GameDefine.SKILL_NONUP_SOUND);
				MusicControlSystem.PlaySound(GameDefine.SKILL_NONUP_SOUND,1);
				return;
			}
			
			JsonSlot_Pet.Send_CtoM_PetSkillLvUp(PetData.iPetDBFID,petSkillType);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void PressEvent(GameObject gb,bool isDown)
	{
		SpeedFlag = isDown;
	}
	//-------------------------------------------------------------------------------------------------
	private void ClickSkillEvent(GameObject gb)
	{
		if(onClickSkill!= null)
			onClickSkill(petSkillType,PetData);
	}
	//-------------------------------------------------------------------------------------------------
	//開啟物品資訊
	private void  OpenItemDetail(GameObject gb)
	{
		if(NeededItemID == 0)
			return;
		
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(NeededItemID , 1);
	}
	//-------------------------------------------------------------------------------------------------
	#endregion 事件宣告
	//-------------------------------------------------------------------------------------------------


}
