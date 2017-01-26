using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System;

public class UI_Talent : NGUIChildGUI 
{
	public UIButton		btnAllSkillUP		= null;
	public UILabel		lbAllSkillUp		= null;
	//SwitchButtons
	public UIButton		btnSword			= null;
	public UILabel		lbSword				= null;
	//public UISprite		SwordLight			= null;
	public UIToggle		tgSword				= null;
	public UIButton		btnAxe				= null;
	public UILabel		lbAxe				= null;
	//public UISprite		AxeLight			= null;
	public UIToggle		tgAxe				= null;
	public UIButton		btnBow				= null;
	public UILabel		lbBOW				= null;
	//public UISprite		BowLight			= null;
	public UIToggle		tgBow				= null;
	//SkillTable
	public UIScrollView svSkillTable		= null;
	public UIGrid		Grid				= null;
	public UITexture	texBackground		= null; //天賦表背景圖
	//SkillSwitch
	public UIWidget		wgSkillSwitch		= null; //切換技能的集合
	public UILabel		lbSwitchTitle		= null;
	public UILabel		lbSkillSlot1		= null;	//技能1文字
	public UILabel		lbSkillSlot2		= null;	//技能2文字
	public UILabel		lbSkillSlot3		= null;	//技能3文字
	public UIWidget		UsedSkillSlot1		= null; //技能1生成位置
	public UIWidget		UsedSkillSlot2		= null; //技能2生成位置
	public UIWidget		UsedSkillSlot3		= null; //技能3生成位置
	public UILabel		lbSkillSwitchNotes	= null; //技能變更說明
	public UIWidget		USkillLock2			= null; //技能2鎖
	public UIWidget		USkillLock3			= null; //技能3鎖
	public UILabel		lbsk2LevelLimit		= null; //技能2鎖等級
	public UILabel		lbsk3LevelLimit		= null; //技能3鎖等級
	//
	public UILabel		lbUsedorNot			= null; //使用中與否
	//SkillUpUnlock
	public UIWidget		SelectSkillSlot		= null; //選擇技能顯示
	public UILabel		lbNoteTitle			= null; //技能說明Title
	public UILabel		lbNoteContent		= null; //技能說明內容
	public UILabel		lbLimitTitle		= null; //技能條件：
	public UILabel		lbLimitCondtion		= null; //限制條件
	public UIButton		btnUpUnlock			= null; //解鎖/升級按鈕
	public UILabel		lbUpUnlock			= null; //解鎖/升級字樣
	public UISprite		spriteItem			= null; //解鎖/升級所需道具圖
	public UILabel		lbItemCount			= null; //道具數量
	public UILabel		lbMoneyCount		= null; //金錢數量
	public UILabel		lbClickItemTip		= null; //點擊道具圖提示
	//
	public UILabel		lbClickTip			= null; //點擊說明提示
	//
	public BoxCollider	Mask				= null; //遮閉作用
	public UISprite		SelectSkillList		= null; //選擇列
	public UIGrid		SelectSkillGrid		= null; //選擇技能列
	public UIButton		btnCloseSelSkill	= null; //關閉選擇技能列
	//
	public TalentDirectionPoints		TalentLine		= null; //天賦線
	public UISprite		clickMark			= null; //選擇框
	public UISprite		spriteUseSkillEffect= null; //使用技能的選擇效果
	//
	[HideInInspector]
	public List<Slot_Skill> SkillSlots		= new List<Slot_Skill>();
	[HideInInspector]
	public List<Slot_Skill> UsedSkillSlots 	= new List<Slot_Skill>();
	[HideInInspector]
	public List<Slot_Skill> SelSkillSlots 	= new List<Slot_Skill>();
	[HideInInspector]
	public Slot_Skill	SelectSlot;
	[HideInInspector]
	public List<UIWidget>	TalentSlots		= new List<UIWidget>();
	//
	[HideInInspector]
	public List<Transform> UsedSelectEffects = new List<Transform>();
	[HideInInspector]
	public List<TalentDirectionPoints>	Directions	= new List<TalentDirectionPoints>();

	private const string 	GUI_SMARTOBJECT_NAME 	= "UI_Talent";
	private const int 		iTableCounts			= 48;
	private const int		iUsedSkillCount			= 3;
	//
	[HideInInspector]
	public int 			RecordSwordClickIndex			= -1; //劍頁紀錄index
	[HideInInspector]
	public int 			RecordPikeClickIndex			= -1; //斧頁紀錄index
	[HideInInspector]
	public int 			RecordBowClickIndex				= -1; //弓頁紀錄index
	[HideInInspector]
	public int 			RecordSwordClickSkillID			= -1; //劍頁紀錄SkillID
	[HideInInspector]
	public int 			RecordPikeClickSkillID			= -1; //斧頁紀錄SkillID
	[HideInInspector]
	public int 			RecordBowClickSkillID			= -1; //弓頁紀錄SkillID
	[HideInInspector]
	public int 			RecordUsedIndex				= 0;
	[HideInInspector]
	public int[] 		UsedSkillID 				= new int[(int)ENUM_UseSkillPosType.ENUM_UseSkillPosType_Max];
	[HideInInspector]
	public bool			PassInUp					= true;
	//
	[HideInInspector]
	public ENUM_SkillTalentType		TempSkillTalentType 	= ENUM_SkillTalentType.ENUM_SkillTalent_Null;
	//
	[HideInInspector]
	public ENUM_UseSkillPosType		SelectskPosType			= ENUM_UseSkillPosType.ENUM_UseSkillPosType_Max; 
	//
	public UILabel			lbSelectTitle			= null;	//選擇列標頭
	//
	public UIWidget			NextSkillLVNote			= null; //下一等級技能說明
	public UILabel			lbNextSkillLVNote		= null; //下一等級技能說明文字
	public UILabel			lbNextSkillLVTitle		= null; //下一等級標題
	public UILabel			lbNextSkillName			= null; //下一等級技能名
	public UILabel			lbNextSkillLV			= null; //下一等級數
	//
	public UIWidget			TalentSlotsPrefab		= null;
	//紀錄所需道具GUID
	private int				NeededItemID			= 0;
	// 快速升級UI
	public UILabel			lbPressToSpeedUpTip		= null; //※可長壓快速升級Tip
	public UIButton			btnSpeedUp				= null; //快速升級
	public UILabel			lbSpeedUp				= null; //快速升級字樣
	//
	public UIWidget			SpeedEffect				= null; //快速升級效果
	[HideInInspector]
	public UIWidget			SelSpeedEffect			= null; //被選擇快速升級效果
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UILabel		lbGuideIntroduce				= null;
	public UISprite		spGuideSelectSkill				= null; //導引選擇第一個技能
	public UILabel		lbGuideSelectSkill				= null;
	public UISprite		spGuideSkillLvup				= null; //導引點擊技能升級
	public UILabel		lbGuideSkillLvup				= null;
	public UISprite		spGuideActiveSkill				= null; //導引設定主動技能
	public UILabel		lbGuideActiveSkill				= null;
	public UILabel		lbGuideFinish					= null;	//導引教學結束
	//-------------------------------------------------------------------------------------------------
	private UI_Talent() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		SpeedEffect.gameObject.SetActive(false);
		CreateSkillSlots();
		GetUsedSkillID();
		AssignWords();
		lbPressToSpeedUpTip.gameObject.SetActive(false);
		btnSpeedUp.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		//隱藏技能 二 三slot
		//lbSkillSlot2.gameObject.SetActive(false);
		//lbSkillSlot3.gameObject.SetActive(false);
		//lbSkillSlot1.gameObject.SetActive(false);
		//lbSkillSwitchNotes.gameObject.SetActive(false);
		//
		UIEventListener.Get(spriteItem.gameObject).onClick		+= OpenItemDetail;
	}
	//-------------------------------------------------------------------------------------------------
	private void AssignWords()
	{
		lbSword.text 			= GameDataDB.GetString(9706); //「劍」專用
		lbAxe.text				= GameDataDB.GetString(9707); //「斧」專用
		lbBOW.text				= GameDataDB.GetString(9708); //「弓」專用
		//
		lbSwitchTitle.text 		= GameDataDB.GetString(9709); //戰鬥技能設置
		//lbSkillSlot1.text		= GameDataDB.GetString(2721); //技能1
		//lbSkillSlot2.text		= GameDataDB.GetString(2722); //技能2
		//lbSkillSlot3.text		= GameDataDB.GetString(2723); //技能3
		lbSkillSlot1.text		= GameDataDB.GetString(9771); //第一招
		lbSkillSlot2.text		= GameDataDB.GetString(9772); //第二招
		lbSkillSlot3.text		= GameDataDB.GetString(9773); //第三招
		lbSkillSwitchNotes.text	= GameDataDB.GetString(9712); //※可點擊切換戰鬥技能
		lbNoteTitle.text		= GameDataDB.GetString(2720); //技能說明
		lbSelectTitle.text		= GameDataDB.GetString(9704); //請選擇技能
		//lbClickTip.text			= GameDataDB.GetString(9713); //※可點擊說明查看下一級技能說明
		lbLimitTitle.text		= GameDataDB.GetString(9714); //技能條件：
		lbNextSkillLVTitle.text = GameDataDB.GetString(9705); //下一級技能說明
		//
		lbPressToSpeedUpTip.text= GameDataDB.GetString(9727); //※可長壓快速升級
		lbClickItemTip.text		= GameDataDB.GetString(9721); //※可點擊查看來源
		//
		lbSpeedUp.text			= GameDataDB.GetString(9722); //快速升級
		lbAllSkillUp.text		= GameDataDB.GetString(1597); //全部升級
	}
	//-------------------------------------------------------------------------------------------------
	private void GetUsedSkillID()
	{
		//
		UsedSkillID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetUseSkillID();
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSkillSlots()
	{
		SkillSlots.Clear();
		UsedSkillSlots.Clear();
		SelSkillSlots.Clear();
		UsedSelectEffects.Clear();
		TalentSlots.Clear();
		Directions.Clear();

		Slot_Skill go = ResourceManager.Instance.GetGUI("Slot_Skill").GetComponent<Slot_Skill>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Skill load prefeb error") );
			return;
		}
		UIWidget TalentSlot;
		Slot_Skill newgo;

		//Table
		for(int i=0;i<iTableCounts;++i)
		{
			TalentSlot = CreateTalentSlot(TalentSlotsPrefab,go,Grid.transform,10);
			if(i<10)
				TalentSlot.name = "SkillSlot0"+i.ToString();
			else
				TalentSlot.name = "SkillSlot"+i.ToString();
			//
			SkillSlots[i].ButtonSlot.userData 	= i;
			TalentSlot.gameObject.SetActive(false);
			TalentSlots.Add(TalentSlot);
		}
		//Switch
		newgo = CreateProcess(go,UsedSkillSlot1.transform,20);
		newgo.transform.localScale = Vector3.one*0.9f;
		newgo.ButtonSlot.userData=0;
		//PlugSelectedEffect(newgo);
		UsedSkillSlots.Add(newgo);
		newgo = CreateProcess(go,UsedSkillSlot2.transform,20);
		newgo.transform.localScale = Vector3.one*0.9f;
		newgo.ButtonSlot.userData=1;
		UsedSkillSlots.Add(newgo);
		newgo = CreateProcess(go,UsedSkillSlot3.transform,20);
		newgo.transform.localScale = Vector3.one*0.9f;
		newgo.ButtonSlot.userData=2;
		UsedSkillSlots.Add(newgo);
		//Assign usedskillListClickEvent
		/*for(int i=0;i<UsedSkillSlots.Count;++i)
		{
			UsedSkillSlots[i].onclickEvent			+= TalentTableSlotEvent;
			UsedSkillSlots[i].onclickEvent			+= TalentTableSlotEventSound;
		}*/
		//SelectedSkill
		SelectSlot = CreateProcess(go,SelectSkillSlot.transform,30);
		SelectSlot = PlugSpeedEffect(SelectSlot);
		//
		for(int i=0;i<10;++i)
		{
			newgo = CreateProcess(go,SelectSkillGrid.transform,10);
			if(i<10)
				newgo.name = "SkillSlot0"+i.ToString();
			else
				newgo.name = "SkillSlot"+i.ToString();
			//
			newgo.transform.localScale 	= Vector3.one;
			newgo.ButtonSlot.userData 	= i;
			newgo.onclickEvent			+= SelectSkillSlotEvent;
			SelSkillSlots.Add(newgo);
		}
	}
	//-------------------------------------------------------------------------------------------------
	/*private void PlugScaleEffect(GameObject gb)
	{
		TweenScale ts = gb.AddComponent<TweenScale>();
		ts.from 	= new Vector3(0.8f,0.8f,0.8f);
		ts.to		= new Vector3(1f,1f,1f);
		ts.duration = 0.3f;
		ts.style = UITweener.Style.Once;
	}*/
	//-------------------------------------------------------------------------------------------------
	private UIWidget CreateTalentSlot(UIWidget slotPrefab,Slot_Skill prefab,Transform parentObj,int iDepth)
	{
		if(parentObj == null)
			return null;

		UIWidget newgo = Instantiate(slotPrefab) as UIWidget;
		Slot_Skill skillgo = Instantiate(prefab) as Slot_Skill;
		
		newgo.transform.parent			= parentObj;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= Quaternion.identity;
		newgo.transform.localPosition	= Vector3.zero;
		//
		skillgo.transform.parent		= newgo.transform;
		skillgo.transform.localScale	= Vector3.one;
		skillgo.transform.localRotation	= Quaternion.identity;
		skillgo.transform.localPosition	= Vector3.zero;
		skillgo.SetDepth(iDepth);
		skillgo.onclickEvent			+= TalentTableSlotEvent;
		skillgo.onclickEvent			+= TalentTableSlotEventSound;
		skillgo.gameObject.SetActive(false);
		SkillSlots.Add(skillgo);
		//
		PlugTalentLines(newgo);
		return newgo;
	}
	//-------------------------------------------------------------------------------------------------
	private Slot_Skill PlugSpeedEffect(Slot_Skill prefab)
	{
		if(prefab == null)
			return null;

		UIWidget newgo = Instantiate(SpeedEffect) as UIWidget;
		
		newgo.transform.parent			= prefab.transform;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= Quaternion.identity;
		newgo.transform.localPosition	= Vector3.zero;
		SelSpeedEffect = newgo;
		return prefab;
	}
	//-------------------------------------------------------------------------------------------------
	private Slot_Skill CreateProcess(Slot_Skill prefab,Transform parentObj,int iDepth)
	{
		if(parentObj == null)
			return null;
		
		Slot_Skill newgo = Instantiate(prefab) as Slot_Skill;
		
		newgo.transform.parent			= parentObj;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= Quaternion.identity;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.SetDepth(iDepth);
		return newgo;
	}
	//-------------------------------------------------------------------------------------------------
	//掛載天賦樹的線
	private void PlugTalentLines(UIWidget skslot)
	{
		TalentDirectionPoints newgo = Instantiate(TalentLine) as TalentDirectionPoints;
		newgo.transform.parent			= skslot.transform;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= Quaternion.identity;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.SetAllDepth(20);
		Directions.Add(newgo);
	}
	//-------------------------------------------------------------------------------------------------
	//只掛載被選擇技能特效框
	public void PlugSelectedEffect(Slot_Skill skslot)
	{
		spriteUseSkillEffect.transform.parent			= skslot.transform;
		spriteUseSkillEffect.transform.localScale		= Vector3.one;
		spriteUseSkillEffect.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		spriteUseSkillEffect.transform.localPosition	= Vector3.zero;
		spriteUseSkillEffect.alpha =1;
	}
	//-------------------------------------------------------------------------------------------------
	public void AssignClickMark(Slot_Skill skslot)
	{
		clickMark.transform.parent = skslot.transform;
		clickMark.transform.localScale		= Vector3.one;
		clickMark.transform.localRotation	= Quaternion.identity;
		clickMark.transform.localPosition	= Vector3.zero;
	}
	//-------------------------------------------------------------------------------------------------
	//天賦表中按鈕事件
	public void TalentTableSlotEvent(int GUID,int index)
	{
		int ItemCount = 0;
		int MoneyCount = 0;
		int UpGradeNeededLV = 0;
		string NonFinishList = "";
		PassInUp = true;
		int GetCurrentMoney = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney();
		int GetCurrentLevel	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
		lbLimitCondtion.gameObject.SetActive(true);
		lbPressToSpeedUpTip.gameObject.SetActive(false);
		btnSpeedUp.gameObject.SetActive(false);
		lbClickTip.gameObject.SetActive(true);
		//天賦樹中的選擇框
		AssignClickMark(SkillSlots[index]);
		//
		SelectSlot.SetSlotWithLevel(GUID,true,true);
		S_SkillTalent_Tmp	skTalentTmp	= GameDataDB.SkillTalentDB.GetData(GUID);
		TempSkillTalentType = skTalentTmp.emType;
		switch(TempSkillTalentType)
		{
		case ENUM_SkillTalentType.ENUM_SkillTalent_Sword:
			RecordSwordClickIndex = index;
			RecordSwordClickSkillID = GUID;
			break;
		case ENUM_SkillTalentType.ENUM_SkillTalent_Pike:
			RecordPikeClickIndex = index;
			RecordPikeClickSkillID = GUID;
			break;
		case ENUM_SkillTalentType.ENUM_SkillTalent_Bow:
			RecordBowClickIndex = index;
			RecordBowClickSkillID = GUID;
			break;
		}
		//
		S_SkillData_Tmp 	skDataTmp 	= GameDataDB.SkillDB.GetData(GUID);
		if(skDataTmp == null)
			return;

		string str = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skDataTmp.iNote),ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SkillDataList,SkillLevelType.Now);
		lbNoteContent.text = str;

		S_SkillData skData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSkillDataByDBID(GUID);
		if(skData == null)
		{
			//解鎖
			lbUpUnlock.text = GameDataDB.GetString(2601);						//解鎖.
			//解鎖道具
			if(skTalentTmp.sUnLockItem[0].iCostItemID<=0)
				lbItemCount.gameObject.SetActive(false);
			else
			{
				lbItemCount.gameObject.SetActive(true);
				S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(skTalentTmp.sUnLockItem[0].iCostItemID);
				Utility.ChangeAtlasSprite(spriteItem,itemTmp.ItemIcon);
				NeededItemID = skTalentTmp.sUnLockItem[0].iCostItemID;
				if(skTalentTmp.sUnLockItem[0].iCostItemCount<=0)
					lbItemCount.gameObject.SetActive(false);
				else
				{
					//技能書數量
					int GetSkillBookCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skTalentTmp.sUnLockItem[0].iCostItemID);
					ItemCount = skTalentTmp.sUnLockItem[0].iCostItemCount;
					if(GetSkillBookCount<ItemCount)
					{
						lbItemCount.text = string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(1327),GetSkillBookCount,"/",ItemCount,GameDataDB.GetString(1329)); //紅色
						PassInUp = false;
					}
					else
						lbItemCount.text = string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(1326),GetSkillBookCount,"/",ItemCount,GameDataDB.GetString(1329)); //綠色
				}
			}
			//解鎖金錢
			if(skTalentTmp.iCostMoney<=0)
				lbMoneyCount.gameObject.SetActive(false);
			else
			{
				lbMoneyCount.gameObject.SetActive(true);
				MoneyCount = skTalentTmp.iCostMoney;
				if(GetCurrentMoney<MoneyCount)
				{
					lbMoneyCount.text = string.Format("{0}{1}{2}",GameDataDB.GetString(1327),MoneyCount,GameDataDB.GetString(1329)); //紅色
					PassInUp = false;
				}
				else
				{
					lbMoneyCount.text = string.Format("{0}{1}{2}",GameDataDB.GetString(1326),MoneyCount,GameDataDB.GetString(1329)); //綠色
				}
			}
			//條件顯示
			//文錢
			//if(MoneyCount>0 && ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney()<MoneyCount)
				//NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(10002)," "+MoneyCount,"\n");		//文錢
			//自身等級
			UpGradeNeededLV = skTalentTmp.iPlayerLv;
			if(GetCurrentLevel<UpGradeNeededLV)
			{
				NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(2725),UpGradeNeededLV,GameDataDB.GetString(1329),"\n"); //(紅色)自身等級
				PassInUp = false;
			}
			else
				NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1326),GameDataDB.GetString(2725),UpGradeNeededLV,GameDataDB.GetString(1329),"\n"); //(綠色)自身等級
			//前置技能
			for(int i=0;i<GameDefine.SKILLTALENT_PRESKILL;++i)
			{
				if(skTalentTmp.sPreSkill[i].iPreSkillID<=0)
					continue;

				S_SkillData_Tmp PreSkDataTmp = GameDataDB.SkillDB.GetData(skTalentTmp.sPreSkill[i].iPreSkillID);
				S_SkillData PreskData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSkillDataByDBID(skTalentTmp.sPreSkill[i].iPreSkillID);
				if(PreskData == null)
				{
					NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}{6}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(PreSkDataTmp.SkillName),GameDataDB.GetString(1596),skTalentTmp.sPreSkill[i].iPreSkillLv,GameDataDB.GetString(1329),"\n");		//(紅色)前置技能
					PassInUp = false;
				}
				else
				{
					if(PreskData.iLv<skTalentTmp.sPreSkill[i].iPreSkillLv)
					{
						NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}{6}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(PreSkDataTmp.SkillName),GameDataDB.GetString(1596),skTalentTmp.sPreSkill[i].iPreSkillLv,GameDataDB.GetString(1329),"\n");	//(紅色)前置技能
						PassInUp = false;
					}
					else
						NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}{6}",NonFinishList,GameDataDB.GetString(1326),GameDataDB.GetString(PreSkDataTmp.SkillName),GameDataDB.GetString(1596),skTalentTmp.sPreSkill[i].iPreSkillLv,GameDataDB.GetString(1329),"\n");	//(綠色)前置技能
				}
			}
			//解鎖道具
			if(ItemCount>0)
			{
				S_Item_Tmp itemTmp 	= GameDataDB.ItemDB.GetData(skTalentTmp.sUnLockItem[0].iCostItemID);
				S_ItemData item 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skTalentTmp.sUnLockItem[0].iCostItemID);
				if(item == null)
				{
					NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(itemTmp.iName)," "+ItemCount,GameDataDB.GetString(1329),"\n");	//(紅色)道具
					PassInUp = false;
				}
				else
				{
					int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
					if(CurrentItemCount<ItemCount)
					{
						NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(itemTmp.iName)," "+ItemCount,GameDataDB.GetString(1329),"\n");	//(紅色)道具
						PassInUp = false;
					}
					else
						NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1326),GameDataDB.GetString(itemTmp.iName)," "+ItemCount,GameDataDB.GetString(1329),"\n");	//(綠色)道具
				}
			}
			//
			lbClickTip.text			= GameDataDB.GetString(9718); //※解鎖後開放Lv1

		}//解鎖結束
		else
		{
			lbUpUnlock.text = GameDataDB.GetString(261);						//升級
			if(skData.iLv >= skDataTmp.iUpgradeLimitSkillLv)
			{

				lbItemCount.gameObject.SetActive(false);
				lbMoneyCount.gameObject.SetActive(false);
				lbLimitCondtion.gameObject.SetActive(false);
				NonFinishList = GameDataDB.GetString(9702); //技能已是最高級。
				lbClickTip.gameObject.SetActive(false);
			}
			else
			{
				//升級
				lbPressToSpeedUpTip.gameObject.SetActive(true);
				btnSpeedUp.gameObject.SetActive(true);
				//升級道具
				if(skDataTmp.iUpgradeCostItemID<=0)
					lbItemCount.gameObject.SetActive(false);
				else
				{
					lbItemCount.gameObject.SetActive(true);
					S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(skDataTmp.iUpgradeCostItemID);
					Utility.ChangeAtlasSprite(spriteItem,itemTmp.ItemIcon);
					ItemCount = ARPGApplication.instance.CalculateUpgradeItemCount(skDataTmp.fUpgradeCostItemRatio,skData.iLv);
					NeededItemID = skDataTmp.iUpgradeCostItemID;
					if(ItemCount<=0)
						lbItemCount.gameObject.SetActive(false);
					else
					{
						//技能書數量
						int GetSkillBookCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
						if(GetSkillBookCount<ItemCount)
						{
							lbItemCount.text = string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(1327),GetSkillBookCount,"/",ItemCount,GameDataDB.GetString(1329)); //紅色
							PassInUp = false;
						}
						else
							lbItemCount.text = string.Format("{0}{1}{2}{3}{4}",GameDataDB.GetString(1326),GetSkillBookCount,"/",ItemCount,GameDataDB.GetString(1329)); //綠色
					}
				}
				//升級金錢
				if(skDataTmp.fUpgradeCostMoneyRatio<=0)
					lbMoneyCount.gameObject.SetActive(false);
				else
				{
					lbMoneyCount.gameObject.SetActive(true);
					MoneyCount = ARPGApplication.instance.CalculateUpgradeMoneyCount(skDataTmp.fUpgradeCostMoneyRatio,skData.iLv);
					if(MoneyCount<=0)
						lbItemCount.gameObject.SetActive(false);
					else
					{
						if(GetCurrentMoney<MoneyCount)
						{
							lbMoneyCount.text = string.Format("{0}{1}{2}",GameDataDB.GetString(1327),MoneyCount,GameDataDB.GetString(1329)); //紅色
							PassInUp = false;
						}
						else
						{
							lbMoneyCount.text = string.Format("{0}{1}{2}",GameDataDB.GetString(1326),MoneyCount,GameDataDB.GetString(1329)); //綠色
						}
					}
				}
				//未達成升級的條件顯示
				//文錢
				//if(MoneyCount>0 && ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney()<MoneyCount)
					//NonFinishList = string.Format("{0}{1}{2}{3}",NonFinishList,GameDataDB.GetString(10002)," "+MoneyCount,"\n");		//文錢
				//自身等級條件1
				UpGradeNeededLV =((skData.iLv+1)*skDataTmp.iUpgradPreRoleLv);
				if(GetCurrentLevel<UpGradeNeededLV)
				{
					NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(2725),UpGradeNeededLV,GameDataDB.GetString(1329),"\n"); //(紅色)自身等級
					PassInUp = false;
				}
				else
					NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1326),GameDataDB.GetString(2725),UpGradeNeededLV,GameDataDB.GetString(1329),"\n"); //(綠色)自身等級
				/*//自身等級條件2
				if(skData.iLv >= GetCurrentLevel)
				{
					NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(9715),UpGradeNeededLV,GameDataDB.GetString(1329),"\n"); //(紅色)無法超過角色最大等級
					PassInUp = false;
				}*/

				//升級道具
				if(ItemCount>0)
				{
					S_Item_Tmp itemTmp 	= GameDataDB.ItemDB.GetData(skDataTmp.iUpgradeCostItemID);
					S_ItemData item 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataByGUID(skDataTmp.iUpgradeCostItemID);
					if(item == null)
					{
						NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(itemTmp.iName)," "+ItemCount,GameDataDB.GetString(1329),"\n");	//(紅色)道具
						PassInUp = false;
					}
					else
					{
						int CurrentItemCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(skDataTmp.iUpgradeCostItemID);
						if(CurrentItemCount<ItemCount)
						{
							NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1327),GameDataDB.GetString(itemTmp.iName)," "+ItemCount,GameDataDB.GetString(1329),"\n");	//(紅色)道具
							PassInUp = false;
						}
						else
							NonFinishList = string.Format("{0}{1}{2}{3}{4}{5}",NonFinishList,GameDataDB.GetString(1326),GameDataDB.GetString(itemTmp.iName)," "+ItemCount,GameDataDB.GetString(1329),"\n");	//(綠色)道具
					}
				}
			}
			//
			lbClickTip.text			= GameDataDB.GetString(9713); //※可點擊說明查看下一級技能說明
		}//升級結束

		lbLimitCondtion.text = NonFinishList;
		//ClickEffects[index].value=true;
	}
	//-------------------------------------------------------------------------------------------------
	//天賦表中按鈕事件
	public void TalentTableSlotEventSound(int GUID,int index)
	{
		//音效
		MusicControlSystem.StopOnceSound(GameDefine.SKILL_CLICK_SOUND);
		MusicControlSystem.PlaySound(GameDefine.SKILL_CLICK_SOUND,1);
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//技能變更事件
	public void SelectSkillSlotEvent(int GUID,int index)
	{
		if(RecordUsedIndex == -1)
			return;

		int UseSkillIDIndex = -1;
		if(UsedSkillSlots[RecordUsedIndex].SkillGUID>0)
		{
			for(int i=0;i<UsedSkillSlots.Count;++i)
			{
				if(i == RecordUsedIndex)
					continue;

				if(UsedSkillSlots[i].SkillGUID == GUID)
				{
					UsedSkillSlots[i].SetSlotWithLevel(UsedSkillSlots[RecordUsedIndex].SkillGUID,true,false);
					UseSkillIDIndex = (int)SelectskPosType + i;
					UsedSkillID[UseSkillIDIndex] = UsedSkillSlots[RecordUsedIndex].SkillGUID;
				}
			}
		}
		S_SkillTalent_Tmp skTmp = GameDataDB.SkillTalentDB.GetData(GUID);
		UsedSkillSlots[RecordUsedIndex].SetSlotWithLevel(GUID,true,false);
		UseSkillIDIndex = (int)SelectskPosType + RecordUsedIndex;
		UsedSkillID[UseSkillIDIndex] = GUID;
		RecordUsedIndex = -1;
		//
		SelectSkillList.gameObject.SetActive(false);
		//
		TalentState talState = (TalentState)ARPGApplication.instance.GetGameStateByName(GameDefine.TALENT_STATE);
		if(talState.CurrentSkillTalentType == skTmp.emType)
			talState.SetTalentDatasToSlots();

		talState.DeactiveMaskHit();
		//
		spriteUseSkillEffect.alpha = 0;
		//音效
		MusicControlSystem.StopOnceSound(GameDefine.SKILL_SELECT_SOUND);
		MusicControlSystem.PlaySound(GameDefine.SKILL_SELECT_SOUND,1);
	}
	//-----------------------------------------------------------------------------------------------------
	//開啟物品資訊
	private void  OpenItemDetail(GameObject gb)
	{
		if(NeededItemID == 0)
			return;

		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(NeededItemID , 1);
	}
	//-----------------------------------------------------------------------------------------------------
}
