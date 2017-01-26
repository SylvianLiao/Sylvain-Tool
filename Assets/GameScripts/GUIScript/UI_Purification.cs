using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Purification : NGUIChildGUI 
{
	public UIPanel			panelBase				= null;

	[Header("BackGorund")]
	public GameObject 		gBackGround				= null;
	public UIButton			ButtonClose				= null;
	public UILabel			lbPurTitle				= null;
	
	[Header("RightInfo")]
	public GameObject		gRightInfo				= null;		//右邊資訊集合
	//
	public GameObject		gPurEffect				= null;		//洗煉成功後的動畫
	public GameObject		gAfterPurification		= null;		//洗煉成功後顯示的元件集合
	public UILabel			lbOriginalAttr			= null;		//原始屬性
	public TweenPosition	twOriginalAttr			= null;		//原始屬性於洗煉過程中的位移效果
	public UILabel			lbPurAttr				= null;		//洗煉後的屬性
	public UIButton			btnSave					= null;		//保存按鈕
	public UILabel			lbSave					= null;		
	public UIButton			btnGiveUp				= null;		//放棄按鈕
	public UILabel			lbGiveUp				= null;		
	//
	public UIGrid			gdPurType				= null;		//排列洗煉方式按鈕用
	public UIButton			btnGreatMaster			= null;		//宗師洗煉
	public UILabel			lbGreatMaster			= null;		
	public UISprite			spGreatMasterCheck		= null;
	public UIButton			btnMaster				= null;		//大師洗煉
	public UILabel			lbMaster				= null;	
	public UISprite			spMasterCheck			= null;
	public UIButton			btnExpert				= null;		//專家洗煉
	public UILabel			lbExpert				= null;		
	public UISprite			spExpertCheck			= null;
	//
	public UIButton			btnPurification			= null;		//洗煉按鈕
	public UILabel			lbPurification			= null;		
	public UILabel			lbTopRatio				= null;		//至頂機率
	public UILabel			lbDiamondCost			= null;		//寶石費用
	//public UILabel			lbPurStoneTitle			= null;		//洗煉石標題
	public UIButton			btnPurStone				= null;		//洗煉石圖示
	public UILabel			lbPurStoneValue			= null;		//洗煉石數量

	[Header("LeftInfo")]
	public GameObject		gLeftInfo				= null;		//左邊資訊集合
	//
	public GameObject		gItemTitle				= null;		// Tip上方資訊集合
	public GameObject		gTipItemPos				= null;		// 物品位置
	[HideInInspector]public Slot_Item	slotTipItem	= null;		// 物品
	public UILabel			lbTipItemName			= null;		// 名稱
	public UILabel			lbItemNoteType			= null;		// 物品類型
	public UILabel			lbInheritNum			= null;		// 裝備強化值(+999)
	//
	public UILabel			lbChoosePurAttrTitle	= null;		// 選擇洗煉屬性標題
	public UILabel[]		lbOtherrAttrArray		= new UILabel[3];		// 附加屬性字串
	public UIButton[]		btnOtherrAttrArray		= new UIButton[3];
	public UISprite			spChooseFrame			= null;
	public UIGrid			gdEquipSkill			= null;		// 排列特殊技能用
	public UILabel			lbEquipSkill1Title		= null;		// 特殊技能1標題
	public UISprite			spEquipSkill1Icon		= null;		// 特殊技能1圖示
	public UILabel			lbEquipSkill1			= null;		// 特殊技能1
	public UIButton			btnSkillNote1			= null;		// 特殊技能1說明按鈕
	public UILabel			lbEquipSkill2Title		= null;		// 特殊技能2標題
	public UISprite			spEquipSkill2Icon		= null;		// 特殊技能2圖示
	public UILabel			lbEquipSkill2			= null;		// 特殊技能2
	public UIButton			btnSkillNote2			= null;		// 特殊技能2說明按鈕

	[Header("ItemSkillNote")]
	public UIPanel			panelItemSkillNote		= null;
	public UIButton			btnCloseNote			= null;
	public UILabel			lbItemSkillNote			= null;

	//-------------------------------------管理器------------------------------------------------
	[HideInInspector]
	public Slot_EquipmentUPStar_Item		m_NowItemSlot			= null;
	[HideInInspector]
	public Slot_EquipmentUPStar_EQ			m_FusionEquipSlot		= null;
	[HideInInspector]
	public List<Slot_EquipmentUPStar_Item>  m_FusionMatList			= new List<Slot_EquipmentUPStar_Item>();	//合成材料的slot資料
	[HideInInspector]
	public Slot_Item []  					m_FusionObjectArray		= null;										//WrapContent中的實體物件
    [System.NonSerialized]
	public List<S_Fusion_Tmp>  				m_FusionDataList		= new List<S_Fusion_Tmp>();					//合成結果的合成ID清單
	//--------------------------------------執行用變數----------------------------------------------
	[HideInInspector]private string			m_SlotName				= "Slot_Item";
	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_Purification";

	//-------------------------------------洗煉教學用------------------------------------------------
	public UIPanel			panelGuide					= null; 	//導引相關集合
	public UIButton			btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton			btnFullScreen				= null; 	//防點擊
	public UISprite			spGuideChooseOtherAttr		= null; 	//導引可選擇附加屬性
	public UILabel			lbGuideChooseOtherAttr		= null; 	
	public UISprite			spGuideShowPurResult		= null;		//導引洗煉結果
	public UILabel			lbGuideShowPurResult		= null; 
	public UISprite			spGuideShowPurType			= null;		//導引洗練方式
	public UILabel			lbGuideShowPurType			= null;	

	//-------------------------------------------------------------------------------------------------
	private UI_Purification() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}

	//-------------------------------------------------------------------------------------------------
	private void InitialUI()
	{
		InitUILab();
		CreateSlot();
	}
	//-------------------------------------------------------------------------------------------------
	private void InitUILab()
	{
		lbPurTitle.text	 			= GameDataDB.GetString(5110);   					// "裝備洗煉"
		lbGreatMaster.text 			= GameDataDB.GetString(5111);						// "宗師洗煉"
		lbMaster.text 				= GameDataDB.GetString(5112);						// "大師洗煉"
		lbExpert.text 				= GameDataDB.GetString(5113);						// "專家洗煉"
		lbSave.text 				= GameDataDB.GetString(5116);						// "保存"
		lbGiveUp.text 				= GameDataDB.GetString(5117);						// "放棄"
		lbPurification.text			= GameDataDB.GetString(5104);						// "洗煉"
		lbChoosePurAttrTitle.text 	= GameDataDB.GetString(5109);						// "[FF5500]請選擇要洗煉的屬性[-]"
		//lbPurStoneTitle.text 			= GameDataDB.GetString(5114);					// "[00FFFF]洗煉石[-]"

		for(int i=0 ; i < btnOtherrAttrArray.Length; ++i)
		{
			btnOtherrAttrArray[i].gameObject.SetActive(false);
			btnOtherrAttrArray[i].userData = i;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_Purification load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		slotTipItem = Instantiate(go) as Slot_Item;
		
		slotTipItem.transform.parent		= gItemTitle.transform;
		slotTipItem.transform.localScale	= Vector3.one;
		slotTipItem.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		slotTipItem.transform.localPosition	= gTipItemPos.transform.localPosition;
		slotTipItem.gameObject.SetActive(true);
		
		slotTipItem.name = string.Format("EquipInfoSlot");
		
		slotTipItem.InitialSlot();
	}
	//-------------------------------------------------------------------------------------------------
	public void SetLeftEquipInfo(S_ItemData itemData)
	{
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(itemData.ItemGUID);
		if (itemTmp == null)
			return;

		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			//道具名稱
			lbTipItemName.text = GameDataDB.GetString(itemTmp.iName);
			itemTmp.SetRareColorString(lbTipItemName);
			
			//道具類型
			lbItemNoteType.gameObject.SetActive(itemTmp.iItemNote>0);
			if(itemTmp.iItemNote>0)
				lbItemNoteType.text = GameDataDB.GetString(itemTmp.iItemNote);

			//設定裝備Slot
			slotTipItem.SetSlotWithCount(itemData.ItemGUID,itemData.iCount,false);
			
			// 強化數值
			if(itemData.iInherit[GameDefine.GAME_INHERIT_Streng] > 0)
			{
				lbInheritNum.gameObject.SetActive(true);
				lbInheritNum.text = string.Format("+{0}", itemData.iInherit[GameDefine.GAME_INHERIT_Streng]);
			}
			else
			{
				lbInheritNum.gameObject.SetActive(false);
			}

			//附加屬性字串
			SetIemAttrString(itemData,itemTmp);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetIemAttrString(S_ItemData itemData,S_Item_Tmp itemTmp)
	{
		//附加屬性字串
		Dictionary<Enum_RandomAttribute, string> otherAttr = itemData.GetEquipOtherAttrString(Enum_GetEquipAttrType.Purify_Preview);
		if (otherAttr != null && otherAttr.Count <= lbOtherrAttrArray.Length)
		{
			int setIndex = 0;
			foreach(Enum_RandomAttribute attr in otherAttr.Keys)
			{
				if (String.IsNullOrEmpty(otherAttr[attr]) == false)
				{
					lbOtherrAttrArray[setIndex].text = otherAttr[attr];
					btnOtherrAttrArray[setIndex].gameObject.SetActive(true);
				}
				else
					btnOtherrAttrArray[setIndex].gameObject.SetActive(false);
				
				setIndex++;
			}
		}
		C_RoleDataEx roleDataEx = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		EquipSkill equipSkill = new EquipSkill();
		equipSkill.lbEquipSkillTitle = lbEquipSkill1Title;
		equipSkill.spEquipSkillIcon = spEquipSkill1Icon;
		equipSkill.lbEquipSkillName = lbEquipSkill1;
		equipSkill.btnEquipSkillNote = btnSkillNote1;
		bool isGet = roleDataEx.GetItemSkillString(itemTmp, itemData, 0, equipSkill);
		lbEquipSkill1Title.gameObject.SetActive(isGet);

		equipSkill = new EquipSkill();
		equipSkill.lbEquipSkillTitle = lbEquipSkill2Title;
		equipSkill.spEquipSkillIcon = spEquipSkill2Icon;
		equipSkill.lbEquipSkillName = lbEquipSkill2;
		equipSkill.btnEquipSkillNote = btnSkillNote2;
		isGet = roleDataEx.GetItemSkillString(itemTmp, itemData, 1, equipSkill);
		lbEquipSkill2Title.gameObject.SetActive(isGet);

		/*
		//裝備被動技能字串
		string [] strEqSkill = itemTmp.GetEquipSkillString();
		if (String.IsNullOrEmpty(strEqSkill[0]) == false)
		{
			lbEquipSkill1.text = strEqSkill[0];
			spEquipSkill1Icon.color = Color.white;
			
			S_SkillData_Tmp [] skillTmpArray = new S_SkillData_Tmp[GameDefine.ITEMSKILL_ACTIVE_LEVEL];
			for(int m=0; m < itemTmp.iSpecialAbilityArray_1.Length; ++m)
			{
				if (m >= skillTmpArray.Length)
					break;
				skillTmpArray[m] = itemTmp.GetItemSkillData_1(m);
			}
			int skillActive = itemData.WhichSpecialAbilityActive_1();
			int itemSkillLV = itemTmp.CheckSpecialSkill_1();
			if (skillActive < 0 && itemSkillLV < 0)
			{
				lbEquipSkill1Title.gameObject.SetActive(false);
			}
			else
			{
				if (skillActive >= 0)
				{
					//有觸發特殊技能
					Utility.ChangeAtlasSprite(spEquipSkill1Icon,skillTmpArray[skillActive].ACTField);
					int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_1(skillActive);
					lbEquipSkill1Title.text = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[skillActive].SkillName));
				}
				else if (itemSkillLV >= 0)
				{
					//沒有觸發特殊技能
					Utility.ChangeAtlasSprite(spEquipSkill1Icon,skillTmpArray[itemSkillLV].ACTField);
					int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_1(itemSkillLV);
					lbEquipSkill1Title.text = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[itemSkillLV].SkillName));
					spEquipSkill1Icon.color = Color.gray;
				}	
				lbEquipSkill1Title.gameObject.SetActive(true);
			}
		}
		else
			lbEquipSkill1Title.gameObject.SetActive(false);

		if (String.IsNullOrEmpty(strEqSkill[1]) == false)
		{
			lbEquipSkill2.text = strEqSkill[1];
			spEquipSkill2Icon.color = Color.white;
			S_SkillData_Tmp [] skillTmpArray = new S_SkillData_Tmp[GameDefine.ITEMSKILL_ACTIVE_LEVEL];
			for(int m=0; m < itemTmp.iSpecialAbilityArray_2.Length; ++m)
			{
				if (m >= skillTmpArray.Length)
					break;
				skillTmpArray[m] = itemTmp.GetItemSkillData_2(m);
			}
			
			int skillActive = itemData.WhichSpecialAbilityActive_2();
			int itemSkillLV = itemTmp.CheckSpecialSkill_2();
			if (skillActive < 0 && itemSkillLV < 0)
			{
				lbEquipSkill1Title.gameObject.SetActive(false);
			}
			else
			{
				if (skillActive >= 0)
				{
					//有觸發特殊技能
					Utility.ChangeAtlasSprite(spEquipSkill2Icon,skillTmpArray[skillActive].ACTField);
					int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_2(skillActive);
					lbEquipSkill2Title.text = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[skillActive].SkillName));
				}
				else if (itemSkillLV >= 0)
				{
					//沒有觸發特殊技能
					Utility.ChangeAtlasSprite(spEquipSkill2Icon,skillTmpArray[itemSkillLV].ACTField);
					int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_2(itemSkillLV);
					lbEquipSkill2Title.text = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[itemSkillLV].SkillName));
					spEquipSkill2Icon.color = Color.gray;
				}	
				lbEquipSkill2Title.gameObject.SetActive(true);
			}
		}
		else
			lbEquipSkill2Title.gameObject.SetActive(false);
			*/
		
		//被動技能設定完後重新排序
		gdEquipSkill.enabled = true;
		gdEquipSkill.Reposition();
	}
}

