using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class UI_SpeedUpBoard : NGUIChildGUI 
{
	// 快速升級UI
	public UIPanel			panelBase				= null; 
	public UIWidget			SpeedUpgrade			= null; //技能快速升級
	public UILabel			lbSpeedTitle			= null; //快速升級Title
	public GameObject[]		gValueComparison 		= null; //參數對照群組
	public UILabel[]		lbCurLevel				= null; //目前等級
	public UILabel[]		lbAfterUpLevel			= null; //可到達等級
	public UILabel			lbNeedListTitle			= null; //所需物品列表Title
	public UILabel			lbNeedList				= null; //所需物品列表
	public UILabel			lbSpeedTipNote			= null; //快速升級提示
	public UIButton			btnUPOK					= null; //確定快速升級
	public UIButton			btnUPCancel				= null; //取消快速升級
	public UILabel			lbUPOK					= null; //確定字樣
	public UILabel			lbUPCancel				= null; //取消字樣

	[System.NonSerialized]
	public bool				bUseItembyUpgarde		= false;

	//-----------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_SpeedUpBoard";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_SpeedUpBoard() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		lbUPOK.text				= GameDataDB.GetString(2609); //確認
		lbUPCancel.text			= GameDataDB.GetString(2610); //取消
		lbSpeedTitle.text		= GameDataDB.GetString(9722); //快速升級
		lbNeedListTitle.text 	= GameDataDB.GetString(9725); //消耗項目
		lbSpeedTipNote.text		= GameDataDB.GetString(9726); //將一次消耗所需項目，是否要快速升級?

		for(int i=0; i<gValueComparison.Length; ++i)
		{
			gValueComparison[i].SetActive(false);
		}
	}
	//-------------------------------------------------------------------------------------------------------------
	public void InitialForWingsUpgrade(int depath)
	{
		/*
		panelBase.depth = depath;
		lbUPOK.text				= GameDataDB.GetString(8304); 	//升級
		lbUPCancel.text			= GameDataDB.GetString(8305); 	//取消
		lbSpeedTitle.text		= GameDataDB.GetString(8303);	//翅膀升級
		lbNeedListTitle.text 	= GameDataDB.GetString(8323); 	//消耗
		*/
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------------------
	//設定天賦表中技能快速升級內容
	public void SetSkillSpeedUpData(int iPlayerSkillID)
	{
		Show();
		gValueComparison[2].SetActive(true);
		S_SkillData 		skillData 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSkillDataByDBID(iPlayerSkillID);
		S_SkillData_Tmp 	skDataTmp 	= GameDataDB.SkillDB.GetData(iPlayerSkillID);
		int RoleLevel 		= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
		int CurrentMoney 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney(); 
		S_ItemData			useItemData	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skDataTmp.iUpgradeCostItemID);
		S_Item_Tmp			uItemTmp	= GameDataDB.ItemDB.GetData(skDataTmp.iUpgradeCostItemID);
		int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
		int MoneyCount = 0;
		int ItemCount = 0;
		int UpGradeNeededLV = 0;
		bool CanUpgrade = true;
		int LoopCycle = 0;
		//設定
		lbCurLevel[2].text = string.Format("{0}{1}{2}",GameDataDB.GetString(9723),GameDataDB.GetString(1596),skillData.iLv);
		while(CanUpgrade)
		{
			//截停機制 避免進入無限迴圈
			if(LoopCycle >200)
			{
				CanUpgrade = false;
				UnityDebugger.Debugger.LogError("Enter Infinite Loop");
			}
			//升級檢查
			//文錢
			MoneyCount 	= ARPGApplication.instance.CalculateUpgradeMoneyCount(skDataTmp.fUpgradeCostMoneyRatio,(skillData.iLv+LoopCycle));
			if(MoneyCount>0 && CurrentMoney<MoneyCount)
				CanUpgrade = false;
			//道具
			ItemCount 	= ARPGApplication.instance.CalculateUpgradeItemCount(skDataTmp.fUpgradeCostItemRatio,(skillData.iLv+LoopCycle));
			if(skDataTmp.iUpgradeCostItemID>0 && ItemCount>0 && CurrentItemCount<ItemCount)
				CanUpgrade = false;
			
			//升級級距
			UpGradeNeededLV =((skillData.iLv+LoopCycle+1)*skDataTmp.iUpgradPreRoleLv);
			if(UpGradeNeededLV>0 && RoleLevel<UpGradeNeededLV)
				CanUpgrade = false;
			//是否達最大限制等級
			if((skillData.iLv+LoopCycle)>=skDataTmp.iUpgradeLimitSkillLv)
				CanUpgrade = false;
			
			if(CanUpgrade == false)
				continue;
			
			CurrentMoney 		-= MoneyCount;
			CurrentItemCount 	-= ItemCount;
			++LoopCycle;
		}
		//升級後等級
		lbAfterUpLevel[2].text = string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(9724),GameDataDB.GetString(1596),GameDataDB.GetString(1326),(skillData.iLv+LoopCycle),GameDataDB.GetString(1329));
		//消耗項目
		int ConsumeMoney = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney() - CurrentMoney;
		string ListContent = string.Format("{0}{1}{2}",GameDataDB.GetString(10002)," ",ConsumeMoney);
		if(useItemData != null)
		{
			int ConsumeItem	= useItemData.iCount - CurrentItemCount;
			if(ConsumeItem>0)
			{
				ListContent = string.Format("{0}{1}{2}{3}{4}",ListContent,"\n",GameDataDB.GetString(uItemTmp.iName)," ",ConsumeItem);
				bUseItembyUpgarde = true;
		}
			else
			{
				bUseItembyUpgarde = false;
			}
		}
		lbNeedList.text = ListContent;
	}
	//-------------------------------------------------------------------------------------------------------------
	//設定培養中寵物技能快速升級內容
	public void SetPetSkillSpeedUpData(int iPetSkillID,S_PetData pd,ENUM_PetSkill psk)
	{
		Show();
		gValueComparison[2].SetActive(true);
		S_SkillData_Tmp 	skDataTmp 	= GameDataDB.SkillDB.GetData(iPetSkillID);
		int CurrentMoney 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney(); 
		S_ItemData			useItemData	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skDataTmp.iUpgradeCostItemID);
		S_Item_Tmp			uItemTmp	= GameDataDB.ItemDB.GetData(skDataTmp.iUpgradeCostItemID);
		int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
		int MoneyCount = 0;
		int ItemCount = 0;
		int UpGradeNeededLV = 0;
		bool CanUpgrade = true;
		int LoopCycle = 0;
		//設定
		lbCurLevel[2].text = string.Format("{0}{1}{2}",GameDataDB.GetString(9723),GameDataDB.GetString(1596),pd.iSkillLv[(int)psk]);
		while(CanUpgrade)
		{
			//截停機制 避免進入無限迴圈
			if(LoopCycle >200)
			{
				CanUpgrade = false;
				UnityDebugger.Debugger.LogError("Enter Infinite Loop");
			}
			//升級檢查
			//文錢
			MoneyCount 	= ARPGApplication.instance.CalculateUpgradeMoneyCount(skDataTmp.fUpgradeCostMoneyRatio,(pd.iSkillLv[(int)psk]+LoopCycle));
			if(MoneyCount>0 && CurrentMoney<MoneyCount)
				CanUpgrade = false;
			//道具
			ItemCount 	= ARPGApplication.instance.CalculateUpgradeItemCount(skDataTmp.fUpgradeCostItemRatio,(pd.iSkillLv[(int)psk]+LoopCycle));
			if(skDataTmp.iUpgradeCostItemID>0 && ItemCount>0 && CurrentItemCount<ItemCount)
				CanUpgrade = false;
			//升級級距
			UpGradeNeededLV =((pd.iSkillLv[(int)psk]+LoopCycle+1)*skDataTmp.iUpgradPreRoleLv);
			if(UpGradeNeededLV>0 && pd.iPetLevel<UpGradeNeededLV)
				CanUpgrade = false;
			//是否達最大限制等級
			if((pd.iSkillLv[(int)psk]+LoopCycle)>=skDataTmp.iUpgradeLimitSkillLv)
				CanUpgrade = false;
			
			if(CanUpgrade == false)
				continue;
			
			CurrentMoney 		-= MoneyCount;
			CurrentItemCount 	-= ItemCount;
			++LoopCycle;
		}
		//升級後等級
		lbAfterUpLevel[2].text = string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(9724),GameDataDB.GetString(1596),GameDataDB.GetString(1326),(pd.iSkillLv[(int)psk]+LoopCycle),GameDataDB.GetString(1329));
		//消耗項目
		int ConsumeMoney = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney() - CurrentMoney;
		string ListContent = string.Format("{0}{1}{2}",GameDataDB.GetString(10002)," ",ConsumeMoney);
		if(useItemData != null)
		{
			int ConsumeItem	= useItemData.iCount - CurrentItemCount;
			if(ConsumeItem>0)
				ListContent = string.Format("{0}{1}{2}{3}{4}",ListContent,"\n",GameDataDB.GetString(uItemTmp.iName)," ",ConsumeItem);
		}
		lbNeedList.text = ListContent;
	}
	//-------------------------------------------------------------------------------------------------------------
}
