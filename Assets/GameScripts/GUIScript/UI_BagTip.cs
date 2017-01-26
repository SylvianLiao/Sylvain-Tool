using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum Enum_TipBoardPosition
{
	NoControl = 0,
	Left,
	Right,
	Mid,
}
public enum Enum_TipBoardStatus
{
	None					= 0,
	BagTip_InItemBag,			//背包物品欄中的Tip
	BagTip_InEquipment,			//背包裝備欄中的Tip
	BagTip_NoButton,			//給合成升階UI中顯示材料TIP用
	Fusion_EquipTip,			//合成&拆解UI左方裝備Tip
	UpRank_EquipTip,			//升階UI左方裝備Tip
}
//-------------------------------------------------------------------------------------------------
public class EquipSkill 
{
	public UILabel 		lbEquipSkillTitle 	= null;			//可允許null
	public UISprite 	spEquipSkillIcon 	= null;		
	public UILabel 		lbEquipSkillName 	= null;		
	public UIButton 	btnEquipSkillNote 	= null;		
}
//-------------------------------------------------------------------------------------------------
/// <summary> 主要按鈕監聽須於外部添加</summary>
public class UI_BagTip : NGUIChildGUI  
{
	public UIPanel						panelBase			= null;		
	public GameObject					TipLocRight			= null;		// Tip位置右邊
	public GameObject					TipLocLeft			= null; 	// Tip位置左邊
	public GameObject					TipLocMid			= null; 	// Tip位置中間
	public GameObject					gTipBoard			= null;		// 提示版容器
	public Animation					animTipBoard		= null;		// Tip的出現動畫
	public UISprite						spriteTipBoard		= null;		// 提示版背景

	[Header("提示板上方UI")]
	public GameObject					gItemTitle			= null;		// Tip上方資訊集合
	public GameObject					gTipItemPos			= null;		// 物品位置
	[HideInInspector]public Slot_Item	slotTipItem			= null;		// 物品
	public UILabel						lbTipItemName		= null;		// 名稱
	public UILabel						lbItemNoteType		= null;		// 物品類型
	public UIButton						btnCloseTip			= null;		// 關閉btn

	[Header("裝備內容上方資訊")]
	public GameObject					gEquipContent		= null;		// 裝備內容集合
	public UIPanel						panelAttribute		= null;		// 裝備屬性用Scrollview Panel
	//
	public GameObject					gEquipTopContent	= null;		// 裝備上方內容集合
	public UIButton						btnEnhance 			= null;		// 強化btn
	public UILabel						lbEnhance			= null;	
	public UILabel						lbEnhanceMoney		= null;	
	public UILabel						lbEqMainAttrTitle	= null;		// 裝備主要屬性標題
	public UILabel						lbEqMainAttr		= null;		// 裝備主要屬性
	public UILabel						lbMeltingTopLv		= null;		// 熔煉等級
	public UIProgressBar				barMelitngExp		= null;		// 熔煉經驗值條

	[Header("裝備內容下方資訊")]
	public GameObject					gEquipBotContent	= null;		// 裝備下方內容集合
	public UITable						tbOtherAttr			= null;		// 排列附加屬性用
	public UILabel						lbOtherAttrTitle	= null;		// 附加屬性標題
	public UILabel[]					lbOtherAttr			= new UILabel[3];
	public GameObject[]					gEquipSkill			= new GameObject[2];
	public EquipSkill[]					equipSkillArray		= new EquipSkill[2];
	public UIPanel						panelItemSkillNote	= null;
	public UIButton						btnCloseNote		= null;
	public UILabel						lbItemSkillNote		= null;
	public UILabel						lbMeltingBotLv		= null;		// 熔煉等級
	//
	public UILabel						lbInheritNum		= null;		// INFO介面強化值(+999)

	[Header("提示板內容UI(一般道具)")]
	public GameObject					gItemContent		= null;		// 一般道具內容集合
	public UILabel						lbItemNote			= null;		// 物品說明

	[Header("下方按鈕UI")]
	public GameObject					gBottomBtn			= null;		// 下方按鈕集合
	public UIGrid						gdBottomBtn			= null;		// 排列下方按鈕
	public UIButton						btnEquip			= null;		// 卸裝 / 裝備btn
	public UIButton						btnUseItem			= null;		// 使用道具btn
	public UIButton						btnUseAllItem		= null;		// 使用全部道具btn
	public UIButton						btnBlackSmithing	= null;		// 鍛造btn
	public UIButton						btnOpenItemLootInfo	= null;		// 掉落關卡按鈕
	public UILabel						lbEquip				= null; 	
	public UILabel						lbBlackSmithing		= null;			
	public UILabel						lbUseItem			= null;	
	public UILabel						lbUseAllItem		= null;	
	public UILabel						lbLootInfo			= null; 	

	[Header("鍛造清單")]
	public UIPanel						panelSmithingList	= null;		// 鍛造按鈕清單集合
	public UIGrid						gdSmithingBtn		= null;		// 排列鍛造中的各按鈕
	//public UISprite						spSmithingListBG	= null;		// 鍛造清單背景圖
	public UISprite						spListArrow			= null;		// 鍛造清單開啟指示箭頭
	public UIButton						btnFullCollider		= null;		// 全螢幕透明按鈕
	public UIButton						btnFusion			= null;		// 合成btn
	public UIButton						btnUpRank			= null;		// 升階btn
	public UIButton						btnMelting			= null;		// 熔煉btn
	public UIButton						btnPurification		= null;		// 洗煉btn
	public UIButton						btnDisItem			= null;		// 拆解btn
	
	public UILabel						lbFusion			= null;	
	public UILabel						lbUpRank			= null;	
	public UILabel						lbMelting			= null;	
	public UILabel						lbPurification		= null; 	
	public UILabel						lbDisItem			= null;	

	[Header("EnhanceCost")]
	public UILabel						lbEnhanceCost		= null;
	public UILabel						lbEnhanceCostMoney	= null;
	public UILabel						lbEnhanceCostItem	= null;
	public UISprite						spEnhanceCostItem	= null;
	//-----------------------------------------------------------------------------------------
	[HideInInspector]public Slot_BlackSmithPage	slotBlackSmithPage  = null;

	//---------------------------------------執行用變數--------------------------------------------------
	private Enum_TipBoardStatus			m_TipBoardStatus	= Enum_TipBoardStatus.None;
	private const string				m_SlotPageName		= "Slot_BlackSmithPage";
	private const string				m_SlotName			= "Slot_Item";
	[HideInInspector]public string		m_TipOpenAnimName	= "UI_Scale_Open";
	[HideInInspector]public string		m_TipCloseAnimName	= "UI_Scale_Close";
	private const float					m_ButtonHeight		= 82.0f;
	//------------------------------------------各種費用----------------------------------------------
	public int							m_EnhanceMoneyCost	= 0;
	public int							m_EnhanceItemCost	= 0;
	public int							m_PlayerEnhanceItem	= 0;
	// smartObjectName
	private const string 				GUI_SMARTOBJECT_NAME 	= "UI_BagTip";
	
	//-------------------------------------------------------------------------------------------------
	private UI_BagTip() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialTip();
		//InitialSlotPage();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------
	public void InitialTip()
	{
		CreateSlot();
		SetPanelDepth(panelBase.depth);

		lbEquip.text 			= GameDataDB.GetString(1007); 	//裝備
		lbUseItem.text  		= GameDataDB.GetString(1337);	//使用
		lbUseAllItem.text  		= GameDataDB.GetString(1351);	//全部使用
		lbLootInfo.text 		= GameDataDB.GetString(472);	//掉落關卡

		lbEnhance.text 			= GameDataDB.GetString(1008); 	//強化
		lbFusion.text			= GameDataDB.GetString(5103); 	//合成
		lbUpRank.text			= GameDataDB.GetString(1009); 	//升階
		lbMelting.text			= GameDataDB.GetString(205); 	//熔煉		
		lbPurification.text		= GameDataDB.GetString(5104); 	//洗煉	
		lbDisItem.text			= GameDataDB.GetString(5105); 	//拆解	
		lbEnhanceCost.text		= GameDataDB.GetString(1141); 	//強化需求:

		lbEqMainAttrTitle.text	= GameDataDB.GetString(1342); 	//[FF5500]主要屬性[-]	
		lbOtherAttrTitle.text	= GameDataDB.GetString(1343); 	//[FF5500]附加屬性[-]	

		//預設物件開關
		lbEnhanceCost.gameObject.SetActive(false);
		gEquipContent.SetActive(false);
		gItemContent.SetActive(false);
		gBottomBtn.SetActive(false);

		if (gEquipSkill.Length == equipSkillArray.Length)
		{
			for(int i=0;i<gEquipSkill.Length;++i)
			{
				equipSkillArray[i] = new EquipSkill();
				Transform trans = gEquipSkill[i].transform.FindChild("Label(SkillNote)");
				if (trans != null)
				{
					equipSkillArray[i].lbEquipSkillName = trans.GetComponent<UILabel>();
				}
				trans = gEquipSkill[i].transform.FindChild("Button(ItemSkillNote)");
				if (trans != null)
				{
					equipSkillArray[i].btnEquipSkillNote = trans.GetComponent<UIButton>();
				}
				trans = gEquipSkill[i].transform.FindChild("Sprite(SkillIcon)");
				if (trans != null)
				{
					equipSkillArray[i].spEquipSkillIcon = trans.GetComponent<UISprite>();
				}
				gEquipSkill[i].SetActive(false);
			}
		}

		gEquipContent.gameObject.SetActive(false);

		this.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void AddCallBack()
	{
		for(int i=0;i<equipSkillArray.Length;++i)
		{
			UIEventListener.Get(equipSkillArray[i].btnEquipSkillNote.gameObject).onClick 	+= OpenActiveItemSkill;
		}
		EventDelegate.Add(btnBlackSmithing.onClick , OnBtnBlackSmithClick);
		UIEventListener.Get(btnCloseNote.gameObject).onClick 								+= OnCloseItemSkillClick;
	}
	//-------------------------------------------------------------------------------------------------
	public void RemoveCallBack()
	{
		for(int i=0;i<equipSkillArray.Length;++i)
		{
			UIEventListener.Get(equipSkillArray[i].btnEquipSkillNote.gameObject).onClick 	-= OpenActiveItemSkill;
		}
		EventDelegate.Remove(btnBlackSmithing.onClick , OnBtnBlackSmithClick);
		UIEventListener.Get(btnCloseNote.gameObject).onClick 								-= OnCloseItemSkillClick;
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_BagTip load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		slotTipItem = Instantiate(go) as Slot_Item;
		
		slotTipItem.transform.parent		= gItemTitle.transform;
		slotTipItem.transform.localScale	= Vector3.one;
		slotTipItem.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		slotTipItem.transform.localPosition	= gTipItemPos.transform.localPosition;
		slotTipItem.gameObject.SetActive(true);

		slotTipItem.name = string.Format("slotItem00");
		
		slotTipItem.InitialSlot();
	}
	//-------------------------------------------------------------------------------------------------
	public void SetPanelDepth(int panelDepth)
	{
		panelBase.depth = panelDepth;
		panelAttribute.depth = panelBase.depth+1;
		panelSmithingList.depth = panelBase.depth+2;
		panelItemSkillNote.depth = panelBase.depth+3;
	}
	//-------------------------------------------------------------------------------------------------
	public void RepostionOtherAttr()
	{
		//附加屬性設定完後重新排序
		tbOtherAttr.enabled = true;
		tbOtherAttr.Reposition();
	}
	//-------------------------------------------------------------------------------------------------
	public void RepostionBottomBtn()
	{
		//附加屬性設定完後重新排序
		gdBottomBtn.enabled = true;
		gdBottomBtn.Reposition();
	}
	//-------------------------------------------------------------------------------------------------
	public void RepositionSmithingBtn()
	{
		//附加屬性設定完後重新排序
		gdSmithingBtn.enabled = true;
		gdSmithingBtn.Reposition();
	}
	//-------------------------------------------------------------------------------------------------
	public void SetTipCommonData (Enum_TipBoardStatus status, Enum_TipBoardPosition tipPos, S_ItemData itemData, S_Item_Tmp itemTmp)
	{
		m_TipBoardStatus = status;

		switch(tipPos)
		{
		case Enum_TipBoardPosition.Left:
			gTipBoard.transform.localPosition = TipLocLeft.transform.localPosition;
			break;
		case Enum_TipBoardPosition.Right:
			gTipBoard.transform.localPosition = TipLocRight.transform.localPosition;
			break;
		case Enum_TipBoardPosition.Mid:
			gTipBoard.transform.localPosition = TipLocMid.transform.localPosition;
			break;
		default:
			break;
		}

		//若Tip加入GUI管理器使用時設定鍛造分頁資料
		if (slotBlackSmithPage != null && itemData != null)
			slotBlackSmithPage.SetTargetItem(itemData);
		
		//道具名稱
		lbTipItemName.text = GameDataDB.GetString(itemTmp.iName);
		itemTmp.SetRareColorString(lbTipItemName);
		
		//道具類型
		lbItemNoteType.gameObject.SetActive(itemTmp.iItemNote>0);
		if(itemTmp.iItemNote>0)
			lbItemNoteType.text = GameDataDB.GetString(itemTmp.iItemNote);
	}
	#region 根據TIP狀態設定內容
	//-----------------------------------------------------------------------------------------------------
	public void SetTipInItemBag(S_ItemData itemData , S_Item_Tmp itemTmp, Enum_TipBoardPosition tipPos)
	{
		Enum_TipBoardStatus status = Enum_TipBoardStatus.BagTip_InItemBag;
		if (itemData == null)
		{
			UnityDebugger.Debugger.Log ("S_ItemData is null while "+status);
			return;
		}
		if (itemTmp == null)
		{
			UnityDebugger.Debugger.Log ("S_Item_Tmp is null while "+status+", GUID = "+itemData.ItemGUID);
			return;
		}

		//TIP共同UI設定
		SetTipCommonData(status, tipPos ,itemData, itemTmp);
		slotTipItem.SetSlotWithCount(itemData.ItemGUID,itemData.iCount,false);

		if (slotBlackSmithPage != null)
		{
			//判斷鍛造分頁按鈕開啟與否
			int btnCount = slotBlackSmithPage.SetBtnActiveOrNot(itemTmp,itemData);
			SwitchBlackSmithingButton(!(btnCount == 0));
			if (btnCount == 1)
			{
				lbBlackSmithing.text = slotBlackSmithPage.m_ActiveBtnList[slotBlackSmithPage.m_CurrentPage].ButtonName;
			}
			else
			{
				lbBlackSmithing.text 	= GameDataDB.GetString(5102);	//鍛造
			}
		}
			
		//掉落關卡按鈕
		CheckBtnLootItemInfo(itemTmp);

		spriteTipBoard.gameObject.SetActive(true);
		btnCloseTip.gameObject.SetActive(true);

		//--------------------------------Tip內容設定--------------------------------
		gBottomBtn.gameObject.SetActive(true);

		//若為裝備
		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			gEquipContent.gameObject.SetActive(true);
			gItemContent.gameObject.SetActive(false);
			SwitchUseItemButton(false);
			SwitchUseAllItemButton(false);

			//設定熔煉等級
			int meltLv = itemData.iMeltingLV;
			if (meltLv > itemTmp.iMeltingLimit)
				meltLv = itemTmp.iMeltingLimit;
			lbMeltingBotLv.text = string.Format(GameDataDB.GetString(256),meltLv ,itemTmp.iMeltingLimit);

			//裝備按鈕
			if(itemData.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None)
			{
				SwitchEquipButton(true);
				lbEquip.text = GameDataDB.GetString(1007); //裝備
			}
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
			
			//強化按鈕
			int strengthenValue = itemData.iInherit[GameDefine.GAME_INHERIT_Streng];
			if (strengthenValue < 0)
				strengthenValue = 0;
			S_EqStrengthen_Tmp data = GameDataDB.EqStrengthenDB.GetData(1+strengthenValue);
			if(data == null || itemTmp.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Amulet)
			{
				SwitchEnhanceButton(false);
				lbInheritNum.gameObject.SetActive(false);
			}
			else
			{
				SetEhanceCost(itemData);
				SwitchEnhanceButton(true);
			}
			
			SetEquipAttrString(itemTmp, itemData);

			SetItemSkillString(itemTmp, itemData);

			RepostionOtherAttr();
		}
		//若為一般道具
		else
		{
			//開啟
			gItemContent.gameObject.SetActive(true);
			
			//關閉
			gEquipContent.gameObject.SetActive(false);
			SwitchEquipButton(false);
			SwitchEnhanceButton(false);

			//使用按鈕
			SwitchUseItemButton(itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_GiftBox);
			SwitchUseAllItemButton(itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_GiftBox && itemData.iCount > 1);
			/*
			if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_GiftBox && itemData.iCount > 1)
			{
				bool needKey = false;
				for(int i=0; i<itemTmp.iGiftBoxIDs.Length;++i)
				{
					S_GiftBox_Tmp giftTmp = GameDataDB.GiftBoxDB.GetData(itemTmp.iGiftBoxIDs[i]);
					if (giftTmp == null)
						continue;
					if (giftTmp.iKeyItem > 0)
					{
						needKey = true;
						break;
					}
				}
				SwitchUseAllItemButton(!needKey);
			}
			else
				SwitchUseAllItemButton(false);*/
			//道具說明
			string str = GameDataDB.GetString(itemTmp.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			lbItemNote.text = str;
			//lbEquipInfo.text = GameDataDB.GetString (DBFforTip.iNote);
		}
		//鍛造清單預設關閉
//		if (panelSmithingList.gameObject.activeSelf)
//			OnBtnBlackSmithClick();

		//重新排列下方按鈕
		RepostionBottomBtn();
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetTipInEquipment(S_ItemData itemData, 
		                           S_Item_Tmp itemTmp,
		                           Enum_TipBoardPosition tipPos,
		                           bool bNUPetEquipTip,
		                           bool showEq, 
		                           bool updateTip)
	{
		Enum_TipBoardStatus status = Enum_TipBoardStatus.BagTip_InEquipment;
		if (itemData == null)
		{
			UnityDebugger.Debugger.Log ("S_ItemData is null while "+status);
			return;
		}
		if (itemTmp == null)
		{
			UnityDebugger.Debugger.Log ("S_Item_Tmp is null while "+status+", GUID = "+itemData.ItemGUID);
			return;
		}
		
		//TIP共同UI設定
		SetTipCommonData(status, tipPos ,itemData, itemTmp);
		slotTipItem.SetSlotWithCount(itemData.ItemGUID,itemData.iCount,false);

		if (slotBlackSmithPage != null)
		{
			//判斷鍛造分頁按鈕開啟與否
			int btnCount = slotBlackSmithPage.SetBtnActiveOrNot(itemTmp,itemData);
			SwitchBlackSmithingButton(!(btnCount == 0));
			if (btnCount == 1)
			{
				lbBlackSmithing.text = slotBlackSmithPage.m_ActiveBtnList[slotBlackSmithPage.m_CurrentPage].ButtonName;
			}
			else
			{
				lbBlackSmithing.text 	= GameDataDB.GetString(5102);	//鍛造
			}
		}
		//掉落關卡按鈕
		CheckBtnLootItemInfo(itemTmp);

		spriteTipBoard.gameObject.SetActive(true);
		btnCloseTip.gameObject.SetActive(true);

		gBottomBtn.gameObject.SetActive(true);
		SwitchUseItemButton(false);
		SwitchUseAllItemButton(false);

		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			gEquipContent.gameObject.SetActive(true);
			gItemContent.gameObject.SetActive(false);

			//設定熔煉等級
			int meltLv = itemData.iMeltingLV;
			if (meltLv > itemTmp.iMeltingLimit)
				meltLv = itemTmp.iMeltingLimit;
			lbMeltingBotLv.text = string.Format(GameDataDB.GetString(256),meltLv ,itemTmp.iMeltingLimit);

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
			
			//裝備按鈕
			if (bNUPetEquipTip == false)
			{
				if (!showEq)
				{
					SwitchEquipButton(false);
				}
				else
				{
					SwitchEquipButton(true);
					lbEquip.text = GameDataDB.GetString(1060); //卸裝
				}
			}
			else
			{
				SwitchEquipButton(true);
				lbEquip.text = GameDataDB.GetString(1060); //卸裝
			}
			//強化按鈕
			int strengthenValue = itemData.iInherit[GameDefine.GAME_INHERIT_Streng];
			if (strengthenValue < 0)
				strengthenValue = 0;
			S_EqStrengthen_Tmp data = GameDataDB.EqStrengthenDB.GetData(1+strengthenValue);
			if(data == null || itemTmp.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Amulet)
			{
				SwitchEnhanceButton(false);
				lbInheritNum.gameObject.SetActive(false);
			}
			else
			{
				//設定強化價錢
//				int emtype = (int)itemData.GetEqStrengthenType();
//				if(emtype <= -1)
//					emtype = itemTmp.GetEQTypeForStrengthen();
//				if(emtype > -1)
//				{
//					lbEnhanceMoney.text = data.UpStrengthening_Price[emtype].ToString();
//				}
				SetEhanceCost(itemData);
				SwitchEnhanceButton(true);
			}
			
			if(itemData.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None)
			{
				SwitchEquipButton(true);
				lbEquip.text = GameDataDB.GetString(1007); //裝備
			}

			SetEquipAttrString(itemTmp, itemData);

			SetItemSkillString(itemTmp, itemData);

			RepostionOtherAttr();
		}

		//鍛造清單預設關閉
//		if (panelSmithingList.gameObject.activeSelf)
//			OnBtnBlackSmithClick();


		//重新排列下方按鈕
		RepostionBottomBtn();
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetTipNoButton(S_ItemData itemData , S_Item_Tmp itemTmp, Enum_TipBoardPosition tipPos, bool showBG)
	{
		Enum_TipBoardStatus status = Enum_TipBoardStatus.BagTip_NoButton;
		if (itemData == null)
		{
			UnityDebugger.Debugger.Log ("S_ItemData is null while "+status);
			return;
		}
		if (itemTmp == null)
		{
			UnityDebugger.Debugger.Log ("S_Item_Tmp is null while "+status+", GUID = "+itemData.ItemGUID);
			return;
		}

		//TIP共同UI設定
		SetTipCommonData(status, tipPos ,itemData, itemTmp);
		slotTipItem.SetSlotWithCount(itemData.ItemGUID,itemData.iCount,false);

		//是否顯示背景
		btnCloseTip.gameObject.SetActive(showBG);
		spriteTipBoard.gameObject.SetActive(showBG);

		//--------------------------------Tip內容設定--------------------------------
		gBottomBtn.gameObject.SetActive(false);
		SwitchLootInfoButton(false);

		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			gEquipContent.gameObject.SetActive(true);

			//設定熔煉等級
			int meltLv = itemData.iMeltingLV;
			if (meltLv > itemTmp.iMeltingLimit)
				meltLv = itemTmp.iMeltingLimit;
			lbMeltingBotLv.text = string.Format(GameDataDB.GetString(256),meltLv ,itemTmp.iMeltingLimit);

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

			SetEquipAttrString(itemTmp, itemData);

			SetItemSkillString(itemTmp, itemData);

			RepostionOtherAttr();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetFusionTip(S_Item_Tmp itemTmp, Enum_TipBoardPosition tipPos, bool showBG)
	{
		Enum_TipBoardStatus status = Enum_TipBoardStatus.BagTip_NoButton;
		if (itemTmp == null)
		{
			UnityDebugger.Debugger.Log ("S_Item_Tmp is null while "+status+", GUID = "+itemTmp.GUID);
			return;
		}

		//TIP共同UI設定
		SetTipCommonData(status, tipPos ,null, itemTmp);
		slotTipItem.SetSlotWithCount(itemTmp.GUID,0,false);

		//是否顯示背景
		btnCloseTip.gameObject.SetActive(showBG);
		spriteTipBoard.gameObject.SetActive(showBG);

		//--------------------------------Tip內容設定--------------------------------
		SwitchLootInfoButton(false);
		gBottomBtn.gameObject.SetActive(false);

		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			gItemContent.gameObject.SetActive(false);
			gEquipContent.gameObject.SetActive(true);

			//強化數值
			lbInheritNum.gameObject.SetActive(false);
			//設定熔煉等級
			lbMeltingBotLv.text = string.Format(GameDataDB.GetString(256),0,itemTmp.iMeltingLimit);

			SetEquipAttrString(itemTmp);

			SetItemSkillString(itemTmp);

			RepostionOtherAttr();
		}
		//若為一般道具
		else
		{
			//開啟
			gItemContent.gameObject.SetActive(true);

			//關閉
			gEquipContent.gameObject.SetActive(false);

			//道具說明
			string str = GameDataDB.GetString(itemTmp.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			lbItemNote.text = str;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetUpRankTip(S_ItemData itemData, S_Item_Tmp upRankItemTmp, S_Fusion_Tmp fusionTmp, Enum_TipBoardPosition tipPos, bool showBG)
	{
		Enum_TipBoardStatus status = Enum_TipBoardStatus.BagTip_NoButton;
		S_Item_Tmp currentItemTmp = GameDataDB.ItemDB.GetData(itemData.ItemGUID);
		if (currentItemTmp == null)
			return;

		if (upRankItemTmp == null)
		{
			UnityDebugger.Debugger.Log ("S_Item_Tmp is null while "+status+", GUID = "+upRankItemTmp.GUID);
			return;
		}

		if (fusionTmp == null && status == Enum_TipBoardStatus.UpRank_EquipTip)
		{
			UnityDebugger.Debugger.Log ("S_Fusion_Tmp is null while "+status+", GUID = "+itemData.ItemGUID);
			return;
		}
		//TIP共同UI設定
		SetTipCommonData(status, tipPos ,itemData, upRankItemTmp);
		slotTipItem.SetSlotWithCount(upRankItemTmp.GUID,0,false);

		//是否顯示背景
		spriteTipBoard.gameObject.SetActive(showBG);
		btnCloseTip.gameObject.SetActive(showBG);

		//--------------------------------Tip內容設定--------------------------------
		gBottomBtn.gameObject.SetActive(false);
		SwitchLootInfoButton(false);
		gBottomBtn.gameObject.SetActive(false);

		if (upRankItemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    upRankItemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			gEquipContent.gameObject.SetActive(true);
			
			//設定熔煉等級
			int meltLv = itemData.iMeltingLV;
			if (meltLv > upRankItemTmp.iMeltingLimit)
				meltLv = upRankItemTmp.iMeltingLimit;

			string strMeltLv = meltLv.ToString();
			strMeltLv = (meltLv < fusionTmp.iMeltingLevel ?"[FF0000]"+strMeltLv+"[-]":strMeltLv);
			lbMeltingBotLv.text = string.Format(GameDataDB.GetString(256),strMeltLv,upRankItemTmp.iMeltingLimit);
			
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

			SetEquipAttrString(upRankItemTmp, itemData);

			//若升階後附加屬性數量會增加
			int randomAttrCount = upRankItemTmp.iPropertyCount - currentItemTmp.iPropertyCount;
			if (randomAttrCount > 0 && 
			    currentItemTmp.iPropertyCount < 4 && 
			    string.IsNullOrEmpty(lbOtherAttr[currentItemTmp.iPropertyCount].text))
			{
				lbOtherAttr[currentItemTmp.iPropertyCount].text = string.Format(GameDataDB.GetString(5100),randomAttrCount);
				lbOtherAttr[currentItemTmp.iPropertyCount].gameObject.SetActive(true);
				lbOtherAttrTitle.gameObject.SetActive(true);
			}

			SetItemSkillString(upRankItemTmp);

			RepostionOtherAttr();
		}
		btnCloseTip.gameObject.SetActive(false);
		spriteTipBoard.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	#endregion

	#region 按鈕事件相關，部分按鈕事件由外部掛載
	//-----------------------------------------------------------------------------------------------------
	public void OnBtnBlackSmithClick()
	{
		slotBlackSmithPage.FirstOpenPage();
	}
	//-----------------------------------------------------------------------------------------------------
//	public void OnBtnBlackSmithClick()
//	{
//		panelSmithingList.gameObject.SetActive(!panelSmithingList.gameObject.activeSelf);
//		float rotateAxisZ = (panelSmithingList.gameObject.activeSelf ? 180.0f:0.0f);
//		spListArrow.transform.localEulerAngles = new Vector3 (0,0,rotateAxisZ);
//	}
	//-----------------------------------------------------------------------------------------------------
	private void OpenActiveItemSkill(GameObject go)
	{
		UIButton btn = go.GetComponent<UIButton>();
		if (btn == null)
			return;
		if (btn.userData == null)
			return;
		List<string> stringList = btn.userData as List<string>;
		//填入字串前先清空
		lbItemSkillNote.text = "";
		for(int i=0; i < stringList.Count; ++i)
		{
			lbItemSkillNote.text += stringList[i]+"\n";
		}
		lbItemSkillNote.text += "\n[FF]"+GameDataDB.GetString(5141);
		panelItemSkillNote.gameObject.SetActive(true);
	}
	//-----------------------------------------------------------------------------------------------------
	private void OnCloseItemSkillClick(GameObject go)
	{
		panelItemSkillNote.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	private void OnEnhanceCostItemClick(GameObject go)
	{
		//ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(GameDefine.ENHANCE_COSTITEM_GUID,0,true);
	}
	#endregion
	//-----------------------------------------------------------------------------------------------------
	#region 各按鈕控制開關
	//-----------------------------------------------------------------------------------------------------
	public void SwitchFusionButton(bool bSwitch)
	{
		btnFusion.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchUpRankButton(bool bSwitch)
	{
		btnUpRank.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchMeltingButton(bool bSwitch)
	{
		btnMelting.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchPurificationButton(bool bSwitch)
	{
		btnPurification.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchDisItemButton(bool bSwitch)
	{
		btnDisItem.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchBlackSmithingButton(bool bSwitch)
	{
		btnBlackSmithing.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchEquipButton(bool bSwitch)
	{
		btnEquip.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchEnhanceButton(bool bSwitch)
	{
		btnEnhance.gameObject.SetActive(bSwitch);
		lbEnhanceCost.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchLootInfoButton(bool bSwitch)
	{
		btnOpenItemLootInfo.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchUseItemButton(bool bSwitch)
	{
		btnUseItem.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchUseAllItemButton(bool bSwitch)
	{
		btnUseAllItem.gameObject.SetActive(bSwitch);
	}
	#endregion
	//-----------------------------------------------------------------------------------------------------
	//檢查是否顯示鍛造清單中的按鈕,若所有按鈕皆關閉則關閉鍛造按鈕
//	public void CheckBtnActiveOrNot(S_Item_Tmp itemTmp, S_ItemData itemData)
//	{
//		bool isAllClose = true;
//
//		//拆解、合成按鈕
//		if(ARPGApplication.instance.m_TeachingSystem.CheckAlreadyGuide(ENUM_GUIDETYPE.DisItemAndFusion) == false)
//		{
//			SwitchFusionButton(false);
//			SwitchDisItemButton(false);
//		}
//		else
//		{
//			List<S_Fusion_Tmp> fusionList = GameDataDB.GetFusionResult(itemTmp.GUID,true);
//			if (fusionList != null && fusionList.Count >0)
//			{
//				SwitchFusionButton(true);
//				isAllClose = false;
//			}
//			else
//				SwitchFusionButton(false);
//
//			if (m_TipBoardStatus == Enum_TipBoardStatus.BagTip_InEquipment)
//				SwitchDisItemButton(false);
//			else
//			{
//				S_Item_Tmp disItemTmp = GameDataDB.ItemDB.GetData(itemTmp.iDisItem);
//				if (disItemTmp == null)
//				{
//					SwitchDisItemButton(false);
//				}
//				else
//				{
//					isAllClose = false;
//					SwitchDisItemButton(true);
//				}
//			}
//		}
//		
//		//熔煉、升階按鈕
//		if(ARPGApplication.instance.m_TeachingSystem.CheckAlreadyGuide(ENUM_GUIDETYPE.MeltAndUpRank) == false)
//		{
//			SwitchUpRankButton(false);
//			SwitchMeltingButton(false);
//		}
//		else
//		{
//			if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
//			    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor &&
//			    itemData.uiAttributeMask > 0)
//			{
//				SwitchMeltingButton(true);
//				isAllClose = false;
//			}
//			else
//				SwitchMeltingButton(false);
//
//			List<S_Fusion_Tmp> upRankList = GameDataDB.GetFusionResult(itemTmp.GUID,false);
//			if (upRankList != null && upRankList.Count >0)
//			{
//				SwitchUpRankButton(true);
//				isAllClose = false;
//			}
//			else
//				SwitchUpRankButton(false);
//		}
//
//		//洗煉按鈕
//		if(ARPGApplication.instance.m_TeachingSystem.CheckAlreadyGuide(ENUM_GUIDETYPE.Purification) == false)
//		{
//			SwitchPurificationButton(false);
//		}
//		else
//		{
//			if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
//			    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
//			{
//				S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetVIPRank()+1);
//				if (itemData.uiAttributeMask > 0 && vipTmp != null && vipTmp.CheckPurTypeUnlock(ENUM_PurifyType.ENUM_PurifyType_Normal))
//				{
//					SwitchPurificationButton(true);
//					isAllClose = false;
//				}
//				else
//				{
//					SwitchPurificationButton(false);
//				}
//			}
//			else
//			{
//				SwitchPurificationButton(false);
//			}
//		}
//
//		//重新排列鍛造清單按鈕
//		RepostionSmithingBtn();
//
//		SwitchBlackSmithingButton(!isAllClose);
//	}
	//-----------------------------------------------------------------------------------------------------
	public void CheckBtnLootItemInfo(S_Item_Tmp itemTmp)
	{
		/*//掉落連結按鈕顯示與否
		bool bshowLoot = true;
		//若Note為貨幣、稀有道具、玩家法寶、時裝、翅膀、夥伴法寶類型時即不顯示
		switch(itemTmp.iItemNote)
		{
		case 0:
		case 5000:
		case 5002:
		case 5011:
		case 5012:
		case 5013:
		case 5022:
			bshowLoot = false;
			break;
		}
		SwitchLootInfoButton(bshowLoot);*/
		//例外處理 在符合上面條件時 但又沒有掉落資訊即隱藏按鈕
		//if(bshowLoot)
		//{
		bool bShowLinks = false;
		for(int i=0;i<GameDefine.ITEM_DROPDUNGEON_MAX;++i)
		{
			if(itemTmp.iDropDungeonID[i]>0)
			{
				bShowLinks = true;
				break;
			}
		}
		SwitchLootInfoButton(bShowLinks);
		//}
	}
	//-----------------------------------------------------------------------------------------------------
	private void SetEquipAttrString(S_Item_Tmp itemTmp, S_ItemData itemData)
	{
		// 主要屬性字串
		lbEqMainAttr.text = itemData.GetEquipMainAttrString(itemTmp);
		
		//設定附加屬性字串先將字串狀態還原
		for(int i=0;i<lbOtherAttr.Length;++i)
		{
			lbOtherAttr[i].text = null;
			lbOtherAttr[i].gameObject.SetActive(false);
		}
		lbOtherAttrTitle.gameObject.SetActive(false);
		
		//附加屬性字串
		Dictionary<Enum_RandomAttribute, string> otherAttr = itemData.GetEquipOtherAttrString(Enum_GetEquipAttrType.Attribute_String);
		if (otherAttr != null && otherAttr.Count <= lbOtherAttr.Length)
		{
			int setIndex = 0;
			foreach(Enum_RandomAttribute attr in otherAttr.Keys)
			{
				if (String.IsNullOrEmpty(otherAttr[attr]) == false)
				{
					lbOtherAttr[setIndex].text = otherAttr[attr];
					lbOtherAttr[setIndex].gameObject.SetActive(true);
					lbOtherAttrTitle.gameObject.SetActive(true);
				}
				else
					lbOtherAttr[setIndex].gameObject.SetActive(false);
				
				setIndex++;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//合成UI用
	private void SetEquipAttrString(S_Item_Tmp itemTmp)
	{
		//裝備主要屬性
		lbEqMainAttr.text = itemTmp.GetEquipMainAttrString();
		
		//設定附加屬性字串先將字串狀態還原
		for(int i=0;i<lbOtherAttr.Length;++i)
		{
			lbOtherAttr[i].text = null;
			lbOtherAttr[i].gameObject.SetActive(false);
		}
		lbOtherAttrTitle.gameObject.SetActive(false);
		
		//裝備附加屬性
		string otherAttr = itemTmp.GetEquipOtherAttrString();
		if (String.IsNullOrEmpty(otherAttr) == false)
		{
			lbOtherAttr[0].text = otherAttr;
			lbOtherAttrTitle.gameObject.SetActive(true);
		}
		lbOtherAttr[0].gameObject.SetActive(String.IsNullOrEmpty(otherAttr)==false);
	}
	//-----------------------------------------------------------------------------------------------------
	private void SetItemSkillString(S_Item_Tmp itemTmp, S_ItemData itemData)
	{
		//裝備被動技能字串
		/*
		string [] strEqSkill = itemTmp.GetEquipSkillString();
		if (strEqSkill.Length != equipSkillArray.Length)
		{
			UnityDebugger.Debugger.Log("EquipSkill String Number "+strEqSkill.Length+" != EquipSkill UI "+equipSkillArray.Length);
			return;
		}*/
		C_RoleDataEx roleDataEx = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		for(int i=0;i<equipSkillArray.Length;++i)
		{
			bool isGet = roleDataEx.GetItemSkillString(itemTmp, itemData, i,equipSkillArray[i]);
			gEquipSkill[i].SetActive(isGet);

			/*
			if (String.IsNullOrEmpty(strEqSkill[i]) == false)
			{
				equipSkillArray[i].lbEquipSkillName.text = strEqSkill[i];
				equipSkillArray[i].spEquipSkillIcon.color = Color.white;
				
				S_SkillData_Tmp [] skillTmpArray = new S_SkillData_Tmp[GameDefine.ITEMSKILL_ACTIVE_LEVEL];
				if (i == 0)
				{
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
						gEquipSkill[i].SetActive(false);
					}
					else
					{
						//設定裝備技能說明按鈕內容
						List<string> skillNoteList = new List<string>();
						for(int m=0; m < skillTmpArray.Length; ++m)
						{
							if (skillTmpArray[m] != null)
							{
								int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_1(m);
								string strTitle = null;
								if (m == skillActive)
									strTitle += "[FF]"+string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
								else
									strTitle += "[88]"+string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
								skillNoteList.Add(strTitle);
								string strNote = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillTmpArray[m].iNote),null,SkillLevelType.Now);
								skillNoteList.Add(strNote);
							}
						}
						equipSkillArray[i].btnEquipSkillNote.userData = skillNoteList;
						
						if (skillActive >= 0)
						{
							//有觸發特殊技能
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[skillActive].ACTField);
						}
						else if (itemSkillLV >= 0)
						{
							if (skillNoteList.Count > 0)
								skillNoteList[0] = "[88]"+skillNoteList[0];
							//沒有觸發特殊技能
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[itemSkillLV].ACTField);
							equipSkillArray[i].spEquipSkillIcon.color = Color.gray;
						}	
						gEquipSkill[i].SetActive(true);
					}
				}
				else if (i == 1)
				{
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
						gEquipSkill[i].SetActive(false);
					}
					else
					{
						List<string> skillNoteList = new List<string>();
						for(int m=0; m < skillTmpArray.Length; ++m)
						{
							if (skillTmpArray[m] != null)
							{
								int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_2(m);
								string strTitle = null;
								if (m == skillActive)
									strTitle += "[FF]"+string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
								else
									strTitle += "[88]"+string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
								skillNoteList.Add(strTitle);
								string strNote = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillTmpArray[m].iNote),null,SkillLevelType.Now);
								skillNoteList.Add(strNote);
							}
						}
						equipSkillArray[i].btnEquipSkillNote.userData = skillNoteList;
						
						if (skillActive >= 0)
						{
							//有觸發特殊技能
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[skillActive].ACTField);
						}
						else if (itemSkillLV >= 0)
						{
							if (skillNoteList.Count > 0)
								skillNoteList[0] = "[88]"+skillNoteList[0];
							//沒有觸發特殊技能
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[itemSkillLV].ACTField);
							equipSkillArray[i].spEquipSkillIcon.color = Color.gray;
						}		
						gEquipSkill[i].SetActive(true);
					}
				}
			}
			else
				gEquipSkill[i].SetActive(false);
			*/
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//合成&升階UI用
	private void SetItemSkillString(S_Item_Tmp itemTmp)
	{
		//裝備被動技能字串
		string [] strEqSkill = itemTmp.GetEquipSkillString();
		if (strEqSkill.Length != equipSkillArray.Length)
		{
			UnityDebugger.Debugger.Log("EquipSkill String Number "+strEqSkill.Length+" != EquipSkill UI "+equipSkillArray.Length);
		}
		else
		{
			for(int i=0;i<equipSkillArray.Length;++i)
			{
				if (String.IsNullOrEmpty(strEqSkill[i]) == false)
				{
					equipSkillArray[i].lbEquipSkillName.text = strEqSkill[i];
					equipSkillArray[i].spEquipSkillIcon.color = Color.white;
					
					S_SkillData_Tmp [] skillTmpArray = new S_SkillData_Tmp[GameDefine.ITEMSKILL_ACTIVE_LEVEL];
					if (i == 0)
					{
						for(int m=0; m < itemTmp.iSpecialAbilityArray_1.Length; ++m)
						{
							if (m >= skillTmpArray.Length)
								break;
							skillTmpArray[m] = itemTmp.GetItemSkillData_1(m);
						}
						int itemSkillLV = itemTmp.CheckSpecialSkill_1();
						if (itemSkillLV < 0)
						{
							gEquipSkill[i].SetActive(false);
						}
						else
						{
							List<string> skillNoteList = new List<string>();
							for(int m=0; m < skillTmpArray.Length; ++m)
							{
								if (skillTmpArray[m] != null)
								{
									int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_1(m);
									string strTitle = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
									skillNoteList.Add(strTitle);
									string strNote = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillTmpArray[m].iNote),null,SkillLevelType.Now);
									skillNoteList.Add(strNote);
								}
							}
							if (skillNoteList.Count > 0)
								skillNoteList[0] = "[88]"+skillNoteList[0];

							equipSkillArray[i].btnEquipSkillNote.userData = skillNoteList;
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[itemSkillLV].ACTField);
							gEquipSkill[i].SetActive(true);
						}
					}
					else if (i == 1)
					{
						for(int m=0; m < itemTmp.iSpecialAbilityArray_2.Length; ++m)
						{
							if (m >= skillTmpArray.Length)
								break;
							skillTmpArray[m] = itemTmp.GetItemSkillData_2(m);
						}
						int itemSkillLV = itemTmp.CheckSpecialSkill_2();
						if (itemSkillLV < 0)
						{
							gEquipSkill[i].SetActive(false);
						}
						else
						{
							List<string> skillNoteList = new List<string>();
							for(int m=0; m < skillTmpArray.Length; ++m)
							{
								if (skillTmpArray[m] != null)
								{
									int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_2(m);
									string strTitle = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
									skillNoteList.Add(strTitle);
									string strNote = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillTmpArray[m].iNote),null,SkillLevelType.Now);
									skillNoteList.Add(strNote);
								}
							}
							if (skillNoteList.Count > 0)
								skillNoteList[0] = "[88]"+skillNoteList[0];

							equipSkillArray[i].btnEquipSkillNote.userData = skillNoteList;
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[itemSkillLV].ACTField);	
							gEquipSkill[i].SetActive(true);
						}
					}
				}
				else
					gEquipSkill[i].SetActive(false);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void SetEhanceCost(S_ItemData itemData)
	{
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(itemData.ItemGUID);
		if (itemTmp == null)
			return;
		//檢查是不是可強化之道具
//		if (itemTmp.ItemType != ENUM_ItemType.ENUM_ItemType_Weapen && 
//		    itemTmp.ItemType != ENUM_ItemType.ENUM_ItemType_Armor)
//		{
//			lbEnhanceCost.gameObject.SetActive(false);
//			return;
//		}

		int plusLv = itemData.GetEqEnhanceValue();
		//檢查強化是否已滿
//		if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel()*2 <= plusLv)
//		{
//			lbEnhanceCost.gameObject.SetActive(false);
//			return;
//		}
			
		S_EqStrengthen_Tmp nextStrengthenTmp = GameDataDB.EqStrengthenDB.GetData(plusLv+1);
		if (nextStrengthenTmp == null)
			return;
		S_Item_Tmp enhanceCostItem = GameDataDB.ItemDB.GetData(nextStrengthenTmp.iUpStrItemID);
		if (enhanceCostItem == null)
			return;
		//材料
		m_EnhanceItemCost = nextStrengthenTmp.iUpStrItemCount;
		if (m_EnhanceItemCost > 0)
		{
			m_PlayerEnhanceItem = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetSpecifiedItemCountInBag(enhanceCostItem.GUID);
			string strPlayerCostItem = (m_PlayerEnhanceItem < m_EnhanceItemCost ? GameDataDB.GetString(1327)+m_PlayerEnhanceItem.ToString()+GameDataDB.GetString(1329) : m_PlayerEnhanceItem.ToString());
			lbEnhanceCostItem.text = " "+strPlayerCostItem+"/"+m_EnhanceItemCost;
		}
		spEnhanceCostItem.gameObject.SetActive(m_EnhanceItemCost > 0);
		
		//金錢
		int	emtype = itemTmp.GetEQTypeForStrengthen();
		int costMoney = nextStrengthenTmp.UpStrengthening_Price[emtype];
		if(emtype > -1 && costMoney > 0)
		{
			m_EnhanceMoneyCost = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney();
			string strCostMoney = (m_EnhanceMoneyCost < costMoney ? GameDataDB.GetString(1327)+costMoney.ToString()+GameDataDB.GetString(1329) : costMoney.ToString());
			lbEnhanceCostMoney.text = " "+strCostMoney;
		}
		lbEnhanceCost.gameObject.SetActive(true);
	}
}
