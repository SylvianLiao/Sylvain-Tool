using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

//public enum ENUM_EQSlotType
//{
//	ENUM_Before		= 0,	// 升星前(當前裝備)
//	ENUM_After,				// 升星後(結果預覽)
//}
////欲升級裝備的狀態
//public enum ENUM_UpgradeType
//{
//	ENUM_Melting,	//準備熔煉
//	ENUM_UpStar,	//準備升階
//}


public class UI_EquipMelting : NGUIChildGUI 
{
	//[HideInInspector]public UI_BagTip		uiBagTip				= null;
	public UIPanel			panelBase				= null;
	//BackGorund
	public UIWidget 		wgBackGround			= null;
	public UIButton			ButtonClose				= null;
	public UILabel			lbEquipMeltingTitle		= null;
	
	//RightInfo
	public UIWidget			wgRightInfo				= null;		//右邊資訊集合
	public UILabel			lbMeltingMaterial		= null;		
	public UIGrid			gdButton				= null;
	public UIButton			btnAutoAdd				= null;
	public UILabel			lbAutoAdd				= null;
	public UIButton			btnMelting				= null;
	public UILabel			lbMelting				= null;
	public UILabel			lbNeedMoney				= null;
	public Transform [] 	m_SelectMatPos			= new Transform[5];
	public Transform 		tCurrentEQPos			= null;		//目標裝備的位置(群組底下有熔煉特效)
	public Animation 		m_UpgradeEffect			= null;		//熔煉和升階的特效
	public GameObject 		gMeltingAnim			= null;		//熔煉的動畫

	//LeftInfo
	public UIWidget			wgLeftInfo				= null;		//右邊資訊集合
	//-------------------提示板上方UI-------------------
	public UIWidget			wgItemTitle				= null;		// Tip上方資訊集合
	public GameObject		gTipItemPos				= null;		// 物品位置
	[HideInInspector]public Slot_Item	slotTipItem			= null;		// 物品
	public UILabel			lbTipItemName			= null;		// 名稱
	public UILabel			lbItemNoteType			= null;		// 物品類型
	public UILabel			lbInheritNum			= null;		// INFO介面強化值(+999)
	//---------------------提示板內容UI(裝備)-------------------
	public UIWidget			wgEquipContent			= null;		// 裝備內容集合
	public UIPanel			panelAttribute			= null;		// 裝備屬性用Scrollview Panel
	//裝備內容上方資訊
	public UIWidget			wgEquipTopContent		= null;		// 裝備上方內容集合
	public UILabel			lbMeltingTopLv			= null;		// 熔煉等級
	public UIProgressBar	barMelitngExp			= null;		// 熔煉經驗值條
	public UILabel			lbMelitngExp			= null;		
	//裝備內容下方資訊
	public UIWidget			wgEquipBotContent		= null;		// 裝備下方內容集合
	public UIGrid			gdOtherAttr				= null;		// 排列附加屬性用
	public UILabel			lbOtherAttrTitle		= null;		// 附加屬性標題
	public UILabel[]		LabelOtherAttr			= new UILabel[5];
	public UILabel			lbItemSkillNote			= null;
//	public UIWidget			wgLeftInfo				= null;		//左邊資訊集合
//	public UISprite			spPlus					= null;
//	public Transform 		tUpEQPos				= null;		//下一階裝備位置(群組底下有該物品出現時的特效)
//	public GameObject 		gUpStarLight			= null;		//升階的高亮框


	//Melting Material UI
	public UIPanel			panelMaterial			= null;		//材料列捲動頁面
	public TweenPosition	twposMeltMaterial		= null;		//材料列位移
	public UIScrollView		svMaterial				= null;		//材料列捲動頁面
	//public UIGrid			gdSelectMaterial		= null;		//選擇的材料
	public UIWrapContentEX	wcMaterialInBag			= null;		//背包裡的材料
	
	//選擇數量UI
	public UIPanel			panelSetItemCount		= null;
	public UIButton			btnPlus					= null;	// 增加數量btn
	public UIButton			btnMinus				= null;	// 減少數量btn
	public UIButton			btnNOForSet				= null;	// 離開
	public UIButton			btnYESForSet			= null;	// 送出需求
	public UILabel			lbNOForSet				= null;
	public UILabel			lbYESForSet				= null;
	public UILabel			lbForCounter			= null;	// 顯示數量
	//-------------------------------------新手教學用------------------------------------------------
	public UIPanel			panelGuide					= null; 	//導引相關集合
	public UIButton			btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton			btnFullScreen				= null; 	//防點擊
	public UIButton			btnScreenInAttrPanel		= null; 	//屬性Panel中的全螢幕按鈕
	public UILabel			lbGuideBeginAndEnd			= null; 	//導引準備熔煉
	public UISprite			spGuideShowEqInfo			= null;		//導引顯示裝備資訊
	public UILabel			lbGuideShowEqInfo			= null; 
	public UISprite			spGuideSelectMat			= null;		//導引選擇熔煉材料
	public UILabel			lbGuideSelectMat			= null;		
	public UISprite			spGuideMelting				= null;		//導引熔煉
	public UILabel			lbGuideMelting				= null; 
	public UISprite			spGuideQuit					= null;		//導引離開熔煉
	public UILabel			lbGuideQuit					= null; 

	//-------------------------------------管理器------------------------------------------------
	[HideInInspector]
	public Slot_EquipmentUPStar_EQ			m_EquipSlot				= null;
	[HideInInspector]
	public List<Slot_EquipmentUPStar_Item>  m_SelectMatList			= new List<Slot_EquipmentUPStar_Item>();	//已選材料的slot資料
	[HideInInspector]
	public Slot_EquipmentUPStar_Item []  	m_MatObjectArray		= null;										//WrapContent中的實體物件
	[HideInInspector]
	public List<Slot_EquipmentUPStar_Item>  m_MatSlotDataList		= new List<Slot_EquipmentUPStar_Item>();	//經驗值道具的Slot資料
	//--------------------------------------執行用變數----------------------------------------------
	//public ENUM_UpgradeType m_UpgradeType			= ENUM_UpgradeType.ENUM_Melting;
	//public const int 		m_EQSlotCount 			= 2;
	//public const int 		ItemSlotCount 	= 1;
	public const int		m_SelectMatCount		= 5;	//可選擇材料總數
	public const int		m_strBeginGUID 			= 1300;
	//
	[HideInInspector]public string			slotNameEQ 			= "Slot_EquipmentUPStar_EQ";	// Slot_EquipmentUPStar_EQ
	[HideInInspector]public string			slotNameItem 		= "Slot_EquipmentUPStar_Item"; 	// Slot_EquipmentUPStar_Item
	[HideInInspector]private string			m_SlotName			= "Slot_Item";
	[HideInInspector]public string			animMatAppearName	= "UI_EquipMelting_Enable";	
	[HideInInspector]public string			animMeltName 		= "UI_EquipMelting_Melting"; 
	//[HideInInspector]public float			overExpRaito 		= 0.0f;							//本階經驗超出的值所佔下一階經驗值的比例(由升階預覽計算後給予)
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_EquipMelting";

	//-------------------------------------------------------------------------------------------------
	private UI_EquipMelting() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		InitPanelDepth();
		InitialEquipSlot();
		InitSelectMatItemSlot();
		InitTipItemSlot();

		InitUILab();
		InitUISetCount();

		SetMaterialOfBag();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		m_UpgradeEffect.Play(animMatAppearName);
	}
	#region UI初始化相關
	//-------------------------------------------------------------------------------------------------
	void InitPanelDepth()
	{
		panelAttribute.depth 	= panelBase.depth+1;
		panelMaterial.depth 	= panelBase.depth+1;
		panelSetItemCount.depth = panelBase.depth+2;
		//uiBagTip.SetPanelDepth(panelBase.depth+2);
	}
	//-------------------------------------------------------------------------------------------------
	void InitialEquipSlot()
	{
		if(slotNameEQ == "")
		{
			slotNameEQ = GameDataDB.GetString(1305); //"Slot_EquipmentUPStar_EQ";
		}
		
		//Slot_EquipmentUPStar_EQ go = Resources.Load("GUI/"+slotNameEQ, typeof(Slot_EquipmentUPStar_EQ)) as Slot_EquipmentUPStar_EQ;
		Slot_EquipmentUPStar_EQ go = ResourceManager.Instance.GetGUI(slotNameEQ).GetComponent<Slot_EquipmentUPStar_EQ>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_EquipmentUPStar load prefeb error,path:{0}", "GUI/"+slotNameEQ) );
			return;
		}
		
		Slot_EquipmentUPStar_EQ newgo	= Instantiate(go) as Slot_EquipmentUPStar_EQ;
		//放至特定位置及群組關係，以便2D特效運作
		newgo.transform.parent			= tCurrentEQPos;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.gameObject.SetActive(true);
		m_EquipSlot = newgo;
		
		//設定熔煉特效層級
		//RenderQueueModifier render = gMeltingAnim.GetComponent<RenderQueueModifier>();
		//if (render != null)
		//	render.m_target =m_EquipSlot.SpriteItemMask.gameObject.GetComponent<UIWidget>();
	}
	//-------------------------------------------------------------------------------------------------
	public void InitSelectMatItemSlot()
	{
		if(slotNameItem == "")
		{
			slotNameItem = GameDataDB.GetString(1306); //"Slot_EquipmentUPStar_Item";
		}
		
		//Slot_EquipmentUPStar_Item go = Resources.Load("GUI/"+slotNameItem, typeof(Slot_EquipmentUPStar_Item)) as Slot_EquipmentUPStar_Item;
		Slot_EquipmentUPStar_Item go = ResourceManager.Instance.GetGUI(slotNameItem).GetComponent<Slot_EquipmentUPStar_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_EquipmentUPStar load prefeb error,path:{0}", "GUI/"+slotNameItem) );
			return;
		}
		
		for(int i=0; i<m_SelectMatPos.Length; ++i)
		{
			Slot_EquipmentUPStar_Item newgo = Instantiate(go) as Slot_EquipmentUPStar_Item;
			
			newgo.transform.parent			= m_SelectMatPos[i].transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= Quaternion.identity;
			newgo.transform.localPosition	= Vector3.zero;
			newgo.ClearSlot();
			newgo.LabelExpPoint.gameObject.SetActive(false);
			m_SelectMatList.Add(newgo);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void InitTipItemSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_BagTip load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		slotTipItem = Instantiate(go) as Slot_Item;
		
		slotTipItem.transform.parent		= wgItemTitle.transform;
		slotTipItem.transform.localScale	= Vector3.one;
		slotTipItem.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		slotTipItem.transform.localPosition	= gTipItemPos.transform.localPosition;
		slotTipItem.gameObject.SetActive(true);

		slotTipItem.name = string.Format("slotItem00");
		
		slotTipItem.InitialSlot();
	}
	//-------------------------------------------------------------------------------------------------
	//數量選擇介面初始化
	public void InitUISetCount()
	{
		lbNOForSet.text = "否";
		lbYESForSet.text = "是";
		lbForCounter.text = "0/0";
	}
	//-------------------------------------------------------------------------------------------------
	public void InitWrapContentObj()
	{
		//先取得背包內物品堆疊
		EquipMeltingState Eqstate = (EquipMeltingState)ARPGApplication.instance.GetGameStateByName(GameDefine.EQUIPMELTING_STATE);
		List<S_ItemData> expItemDataLsit = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqStarExpItem(Eqstate.GetSelectSerialID());
		//排序物品先經驗道具再裝備
		expItemDataLsit.Sort((x,y)=>{	if(x.GetItemType() == y.GetItemType())
			return	x.GetItemExpValue().CompareTo(y.GetItemExpValue());
			return ((int)x.GetItemType()).CompareTo((int)y.GetItemType());
		});
		//取得WrapContent要用的實體物件
		m_MatObjectArray = wcMaterialInBag.transform.GetComponentsInChildren<Slot_EquipmentUPStar_Item>();
		for(int i=0; i<m_MatObjectArray.Length; ++i)
		{
			if (i > expItemDataLsit.Count -1)
				break;
			m_MatObjectArray[i].SetSlotByExpItem(expItemDataLsit[i]);
			m_MatObjectArray[i].gameObject.SetActive(true);
		}
		wcMaterialInBag.onInitializeItem += AssignMaterialData;
		wcMaterialInBag.maxIndex = m_MatSlotDataList.Count-1;
		wcMaterialInBag.enabled = true;
	}
	//-------------------------------------------------------------------------------------------------
	//設定可用材料列
	public void SetMaterialOfBag()
	{
		//先取得背包內物品堆疊
		EquipMeltingState Eqstate = (EquipMeltingState)ARPGApplication.instance.GetGameStateByName(GameDefine.EQUIPMELTING_STATE);
		List<S_ItemData> expItemDataLsit = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqStarExpItem(Eqstate.GetSelectSerialID());
		
		//排序物品先經驗在物品
		expItemDataLsit.Sort((x,y)=>{	if(x.GetItemType() == y.GetItemType())
			return	x.GetItemExpValue().CompareTo(y.GetItemExpValue());
			return ((int)x.GetItemType()).CompareTo((int)y.GetItemType());
		});
		//將經驗值道具資料存成升階Slot格式並儲存
		foreach(S_ItemData itemData in expItemDataLsit)
		{
			Slot_EquipmentUPStar_Item upStarSlot = new Slot_EquipmentUPStar_Item();
			upStarSlot.SetSlotByExpItem(itemData);
			m_MatSlotDataList.Add(upStarSlot);
		}	
	}
	#endregion
	//-------------------------------------------------------------------------------------------------
	public void SetEquipInfo(S_ItemData itemData, S_Item_Tmp itemTmp)
	{
		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			//裝備Slot
			slotTipItem.SetSlotWithCount(itemData.ItemGUID,itemData.iCount,false);

			//道具名稱
			lbTipItemName.text = GameDataDB.GetString(itemTmp.iName);
			itemTmp.SetRareColorString(lbTipItemName);
			
			//道具類型
			lbItemNoteType.gameObject.SetActive(itemTmp.iItemNote>0);
			if(itemTmp.iItemNote>0)
				lbItemNoteType.text = GameDataDB.GetString(itemTmp.iItemNote);

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
			
			SetEquipMeltingInfo(itemData , itemTmp);

			//附加屬性設定完後重新排序
			gdOtherAttr.enabled = true;
			gdOtherAttr.Reposition();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetEquipMeltingInfo(S_ItemData itemData, S_Item_Tmp itemTmp)
	{
		//設定熔煉等級
		int meltLv = itemData.iMeltingLV;
		if (meltLv > itemTmp.iMeltingLimit)
			meltLv = itemTmp.iMeltingLimit;
		lbMeltingTopLv.text = string.Format(GameDataDB.GetString(256),meltLv ,itemTmp.iMeltingLimit);

		//設定附加屬性字串先將字串狀態還原
		for(int i=0;i<LabelOtherAttr.Length;++i)
		{
			LabelOtherAttr[i].text = null;
			LabelOtherAttr[i].gameObject.SetActive(false);
		}
		lbOtherAttrTitle.gameObject.SetActive(false);
		
		//附加屬性字串
		Dictionary<Enum_RandomAttribute, string> otherAttr = itemData.GetEquipOtherAttrString(Enum_GetEquipAttrType.Attribute_String);
		if (otherAttr != null && otherAttr.Count < LabelOtherAttr.Length)
		{
			int setIndex = 0;
			foreach(Enum_RandomAttribute attr in otherAttr.Keys)
			{
				if (String.IsNullOrEmpty(otherAttr[attr]) == false)
				{
					LabelOtherAttr[setIndex].text = otherAttr[attr];
					LabelOtherAttr[setIndex].gameObject.SetActive(true);
					lbOtherAttrTitle.gameObject.SetActive(true);
				}
				else
					LabelOtherAttr[setIndex].gameObject.SetActive(false);
				
				setIndex++;
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//WrapContent更新資料用的CallBack
	public void AssignMaterialData(GameObject material, int wrapIndex, int realIndex)
	{
		if (realIndex > m_MatSlotDataList.Count-1 || realIndex < 0)
		{
			material.SetActive(false);
			return;
		}
		Slot_EquipmentUPStar_Item matSlot = material.GetComponent<Slot_EquipmentUPStar_Item>();
		if (matSlot == null)
			return;

		matSlot.SetSlot(m_MatSlotDataList[realIndex].itemID,m_MatSlotDataList[realIndex].nowCount,(int)m_MatSlotDataList[realIndex].iSerialID,true);
		if (!material.activeSelf)
			material.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetMeltingTarget(S_ItemData itemData , S_Item_Tmp itemTmp)
	{
		m_EquipSlot.SetSlot(itemTmp);
		SetEquipInfo(itemData,itemTmp);
	}
	//-------------------------------------------------------------------------------------------------
	void InitUILab()
	{
		//lbExpBarNote.text = GameDataDB.GetString(206);	// "裝備熔煉值上限可進行升星";
		lbEquipMeltingTitle.text = GameDataDB.GetString(254);   // "裝備熔煉";
		lbAutoAdd.text = GameDataDB.GetString(1341);   		// "自動添加";
		lbMelting.text = GameDataDB.GetString(205);			// "熔煉";
		lbMeltingMaterial.text = GameDataDB.GetString(255);	// "熔煉材料";
		lbNeedMoney.text = "0";
		//Tip Label
		lbOtherAttrTitle.text = GameDataDB.GetString(1343);	// "附加屬性";
	}
	//-------------------------------------------------------------------------------------------------
	//設定經驗條, 並回傳經驗值是否已滿
	public bool SetExpBarValue(S_Item_Tmp itemtmp, S_ItemData itemdata,S_MeltingExp_Tmp meltExpTmp,int addExp)
	{
		bool isExpFull = false;
		float nowRate = 0;
		if(itemdata.iExp + addExp > 0)
		{
			nowRate = (float)(itemdata.iExp + addExp) / (float)meltExpTmp.iEXP * itemtmp.fMeltingVar;
			barMelitngExp.alpha = 1;
		}
		else
		{
			barMelitngExp.alpha = 0;
		}
		if (nowRate >= 1)
		{
			nowRate = 1.0f;
			isExpFull=  true;
		}
		barMelitngExp.value = nowRate;

		if (itemdata.iMeltingLV >= itemtmp.iMeltingLimit)
		{
			barMelitngExp.value = 1.0f;
			SetExpLabel(itemdata.iExp,addExp,meltExpTmp.iEXP,true);
		}
		else
			SetExpLabel(itemdata.iExp,addExp,meltExpTmp.iEXP);

		return isExpFull;
	}
	//-------------------------------------------------------------------------------------------------
	//設定經驗條文字
	public void SetExpLabel(int now,int addExp,int max,bool isMax = false)
	{
		if (isMax)
		{
			lbMelitngExp.text = GameDataDB.GetString(5125);
			return;
		}

		if(addExp <= 0)
		{
			lbMelitngExp.text = string.Format(GameDataDB.GetString(257), now ,max);
		}
		else
		{
			if(addExp > (max - now))
			{
				lbMelitngExp.text = string.Format(GameDataDB.GetString(258),now,GameDataDB.GetString(1327),addExp,GameDataDB.GetString(1329),max); //"融煉值: now[FF0000](+addExp)[-]/max"
			}
			else
			{
				lbMelitngExp.text = string.Format(GameDataDB.GetString(258),now,GameDataDB.GetString(1326),addExp,GameDataDB.GetString(1329),max);
			}
		}
	}
}