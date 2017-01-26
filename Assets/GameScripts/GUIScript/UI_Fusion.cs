using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Fusion : NGUIChildGUI 
{
	[HideInInspector]public UI_BagTip		uiTargetEquipTip		= null;		//左邊合成升階目標裝備的資訊
	[HideInInspector]public UI_BagTip		uiMaterialEquipTip		= null;		//材料中裝備的Tip
	public UIPanel			panelBase				= null;
	[Header("BackGorund")]
	public UIWidget 		wgBackGround			= null;
	public UIButton			ButtonClose				= null;
	public UILabel			lbFusionTitle			= null;

	[Header("RightInfo")]
	public UIWidget			wgRightInfo				= null;		//右邊資訊集合
	public UIGrid			gdButtons				= null;
	public UIButton			btnFusionOrUpRank		= null;
	public UIButton			btnAutoFusion 			= null;
	public UILabel			lbFusionOrUpRank		= null;
	public UILabel			lbNeedMoney				= null;
	public Transform [] 	m_FusionMatPos			= new Transform[5];
	public Transform 		tCurrentEQPos			= null;		//目標裝備的位置(群組底下有熔煉特效)
	public Animation 		m_UpgradeEffect			= null;		//合成和升階的特效
	public GameObject 		gFusionAnim				= null;		//熔煉的動畫
	public Transform 		tFusionEQPos			= null;		//合成結果裝備位置(群組底下有熔煉特效)
	public GameObject 		gUpStarLight			= null;		//升階的高亮框
	public UISprite			spPlus					= null;

	[Header("Fusion List UI")]
	public UIPanel			panelFusionList			= null;		//合成表集合
	public UIScrollView		svFusionList			= null;		//合成表捲動頁面
	public UIWrapContentEX	wcFusionList			= null;		//排列合成表實體物件
	public UISprite			spFusionChoosen			= null; 	//選擇框

	[Header("Auto Fusion")]
	public UIPanel			panelAutoFusion 		= null;
	public UILabel			lbAutoFusionTitle		= null;
	public GameObject		gMatCost 				= null;
	public UILabel			lbMatCost 				= null;
	public UILabel			lbAutoFusionMoney		= null;
	public GameObject		gFusionResult 			= null;
	public UILabel			lbFusionResult 			= null;
	public UIButton			btnAutoFusionOK 		= null;
	public UILabel			lbAutoFusionOK 			= null;
	public UIButton			btnAutoFusionCancel 	= null;
	public UILabel			lbAutoFusionCancel		= null;
	public Slot_Item []		m_AutoFusionMatObj		= new Slot_Item[6];
	public Slot_Item		m_AutoFusionResultObj	= null;
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
	[HideInInspector]public ENUM_UI_Mode 	m_UI_Mode				= ENUM_UI_Mode.ENUM_Fusion;
	public const int 						m_EQSlotCount 			= 2;
	public const int						m_SelectMatCount		= 5;	//可選擇材料總數
	public const int						m_strBeginGUID 			= 1300;
	//
	[HideInInspector]public string			slotNameEQ 				= "Slot_EquipmentUPStar_EQ";	// Slot_EquipmentUPStar_EQ
	[HideInInspector]public string			slotNameItem 			= "Slot_EquipmentUPStar_Item"; 	// Slot_EquipmentUPStar_Item
	//[HideInInspector]private string			m_SlotName				= "Slot_Item";
	[HideInInspector]private string			m_BagTipName			= "UI_BagTip";
	[HideInInspector]public string			animMatAppearName		= "UI_EquipMelting_Enable";	
	[HideInInspector]public string			animFusionName 			= "UI_Fusion_Fusion"; 
	[HideInInspector]public string			animUpRankName 			= "UI_Fusion_UpRank"; 

	//-------------------------------------拆解合成教學用------------------------------------------------
	[Header("Guide")]
	public UIPanel			panelGuide					= null; 	//導引相關集合
	public UIButton			btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton			btnFullScreen				= null; 	//防點擊
	public UISprite			spGuideIntroduceFusion		= null; 	//導引介紹合成目標與材料
	public UILabel			lbGuideIntroduceFusion		= null; 	
	public UISprite			spGuideSelectResult			= null;		//導引點擊合成目標
	public UILabel			lbGuideSelectResult			= null;
	public UISprite			spGuideFusion				= null;		//導引合成
	public UILabel			lbGuideFusion				= null; 
	public UISprite			spGuideShowUpstarMat1		= null;		//導引顯示合成材料1
	public UILabel			lbGuideShowUpstarMat		= null;
	public UISprite			spGuideShowUpstarMat2		= null;		//導引顯示合成材料2
	public UISprite			spGuideQuit					= null;		//導引離開合成
	public UILabel			lbGuideQuit					= null; 
	//-------------------------------------熔煉升階教學用------------------------------------------------
	public UISprite			spGuideShowEqInfo			= null; 	//導引介紹升階裝備資訊
	public UILabel			lbGuideShowEqInfo			= null; 	
	public UILabel			lbGuideEnd					= null; 	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_Fusion";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Fusion() : base(GUI_SMARTOBJECT_NAME)
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
		CreateBagTip();
		CreateEquipSlot();
		CreateFusionMatSlot();
		
		InitUILab();
		InitDepth();
		InitObjSetActive();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		m_UpgradeEffect.Play(animMatAppearName);
	}
	#region UI初始化相關
	//-------------------------------------------------------------------------------------------------
	void InitObjSetActive()
	{
		panelAutoFusion.gameObject.SetActive(false);
		for(int i=0; i < m_AutoFusionMatObj.Length; ++i)
		{
			m_AutoFusionMatObj[i].gameObject.SetActive(false);
		}
	}
	//-------------------------------------------------------------------------------------------------
	void InitDepth()
	{
		panelFusionList.depth = panelBase.depth+1;
		uiTargetEquipTip.SetPanelDepth(panelBase.depth+5);
		uiMaterialEquipTip.SetPanelDepth(uiTargetEquipTip.panelBase.depth+5);
		for(int i=0; i < m_AutoFusionMatObj.Length; ++i)
		{
			m_AutoFusionMatObj[i].SetDepth(lbMatCost.depth+1);
		}
		m_AutoFusionResultObj.SetDepth(lbFusionResult.depth+1);
	}
	//-------------------------------------------------------------------------------------------------
	void InitUILab()
	{
		if (m_UI_Mode == ENUM_UI_Mode.ENUM_Fusion)
		{
			lbFusionTitle.text = GameDataDB.GetString(5108);   		// "裝備合成";
			lbFusionOrUpRank.text = GameDataDB.GetString(5107);		// "確定合成";
			lbAutoFusionTitle.text = GameDataDB.GetString(5134);	// "一鍵合成"
			lbMatCost.text = GameDataDB.GetString(5135);			// "花費材料"	
			lbFusionResult.text = GameDataDB.GetString(5136);		// "合成結果"
			lbAutoFusionOK.text = GameDataDB.GetString(299);		// "確定"	
			lbAutoFusionCancel.text = GameDataDB.GetString(1034);	// "取消"
			lbAutoFusionMoney.text = "";
		}
		else
		{
			lbFusionTitle.text = GameDataDB.GetString(1339);   		// "裝備升階";
			lbFusionOrUpRank.text = GameDataDB.GetString(1304);		// "確定升階";
		}
		
		lbNeedMoney.text = "";
	}
	//-------------------------------------------------------------------------------------------------
	void CreateBagTip()
	{
		UI_BagTip go = ResourceManager.Instance.GetGUI(m_BagTipName).GetComponent<UI_BagTip>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_Fusion load prefeb error,path:{0}", "GUI/"+m_BagTipName) );
			return;
		}
		//左邊合成目標資訊
		UI_BagTip newgo	= Instantiate(go) as UI_BagTip;
		newgo.transform.parent			= this.transform;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.transform.localScale		= Vector3.one;
		newgo.gameObject.SetActive(true);
		uiTargetEquipTip = newgo;
		uiTargetEquipTip.animTipBoard.enabled = false;
		//由於不把UI_BagTip加入GUI_Maneger，故不呼叫初始化Initialize()
		uiTargetEquipTip.InitialTip();

		//材料若為已持有裝備時的TIP
		newgo	= Instantiate(go) as UI_BagTip;
		newgo.transform.parent			= this.transform;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.transform.localScale		= Vector3.one;
		newgo.gameObject.SetActive(false);
		uiMaterialEquipTip = newgo;
		//由於不把UI_BagTip加入GUI_Maneger，故不呼叫初始化Initialize()
		uiMaterialEquipTip.InitialTip();
	}
	//-------------------------------------------------------------------------------------------------
	void CreateEquipSlot()
	{
		if(slotNameEQ == "")
		{
			slotNameEQ = GameDataDB.GetString(1305); //"Slot_EquipmentUPStar_EQ";
		}
		Slot_EquipmentUPStar_EQ go = ResourceManager.Instance.GetGUI(slotNameEQ).GetComponent<Slot_EquipmentUPStar_EQ>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_Fusion load prefeb error,path:{0}", "GUI/"+slotNameEQ) );
			return;
		}

		Slot_EquipmentUPStar_EQ newEquipslot	= Instantiate(go) as Slot_EquipmentUPStar_EQ;
		//放至特定位置及群組關係，以便2D特效運作
		newEquipslot.transform.parent			= tFusionEQPos;
		newEquipslot.transform.localPosition	= Vector3.zero;
		newEquipslot.transform.localScale		= Vector3.one * 0.9f;
		newEquipslot.gameObject.SetActive(true);
		m_FusionEquipSlot = newEquipslot;

	}
	//-------------------------------------------------------------------------------------------------
	public void CreateFusionMatSlot()
	{
		if(slotNameItem == "")
		{
			slotNameItem = GameDataDB.GetString(1306); //"Slot_EquipmentUPStar_Item";
		}

		Slot_EquipmentUPStar_Item go = ResourceManager.Instance.GetGUI(slotNameItem).GetComponent<Slot_EquipmentUPStar_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_Fusion load prefeb error,path:{0}", "GUI/"+slotNameItem) );
			return;
		}
		//合成目標材料
		Slot_EquipmentUPStar_Item newgo	= Instantiate(go) as Slot_EquipmentUPStar_Item;
		//放至特定位置及群組關係，以便2D特效運作
		newgo.transform.parent			= tCurrentEQPos;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.transform.localScale		= Vector3.one;
		newgo.gameObject.SetActive(true);
		m_NowItemSlot = newgo;

		//設定熔煉特效層級
//		RenderQueueModifier render = gFusionAnim.GetComponent<RenderQueueModifier>();
//		if (render != null)
//			render.m_target = m_NowItemSlot.SpriteItemMask.gameObject.GetComponent<UIWidget>();

		//除了合成目標之外的材料
		for(int i=0; i< m_FusionMatPos.Length; ++i)
		{
			newgo = Instantiate(go) as Slot_EquipmentUPStar_Item;
			
			newgo.transform.parent			= m_FusionMatPos[i].transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= Quaternion.identity;
			newgo.transform.localPosition	= Vector3.zero;
			newgo.ClearSlot();
			
			m_FusionMatList.Add(newgo);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void InitWrapContentObj()
	{
		if (m_FusionDataList == null)
		{
			UnityDebugger.Debugger.Log("Get FusionList faild!!");
			return;
		}
		//取得WrapContent要用的實體物件
		m_FusionObjectArray = wcFusionList.transform.GetComponentsInChildren<Slot_Item>();
		for(int i=0; i<m_FusionObjectArray.Length; ++i)
		{
			if (i < m_FusionDataList.Count)
				m_FusionObjectArray[i].SetSlotWithCount(m_FusionDataList[i].iItemID,0,false);
			else
			{
				m_FusionObjectArray[i].InitialSlot();
				m_FusionObjectArray[i].gameObject.SetActive(false);
			}
			m_FusionObjectArray[i].ButtonSlot.userData = i;
		}
		wcFusionList.onInitializeItem = AssignFusionResult;
		wcFusionList.maxIndex = m_FusionDataList.Count-1;
		wcFusionList.enabled = true;

		svFusionList.enabled = m_FusionDataList.Count > 5;
	}
	#endregion
	//-------------------------------------------------------------------------------------------------
	//WrapContent更新資料用的CallBack
	public void AssignFusionResult(GameObject fusionItem, int wrapIndex, int realIndex)
	{
		if (realIndex > m_FusionDataList.Count-1 || realIndex < 0)
		{
			fusionItem.SetActive(false);
			return;
		}
		Slot_Item matSlot = fusionItem.GetComponent<Slot_Item>();
		if (matSlot == null)
			return;
		
		//matSlot.SetSlot(m_FusionDataList[realIndex].itemID,m_FusionDataList[realIndex].nowCount,(int)m_FusionDataList[realIndex].selid);
		//更新slot圖示
		matSlot.SetSlotWithCount(m_FusionDataList[realIndex].iItemID,0,false);
		//記錄合成資料的Index
		matSlot.ButtonSlot.userData = realIndex;
		if (!fusionItem.activeSelf)
			fusionItem.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//設定合成目標材料資訊
	public void SetCurrentItemSlot(S_ItemData itemData,S_FusionMaterial matData)
	{
		//合成後才有可能沒有主要目標的物品資料
		if (itemData == null)
		{
			m_NowItemSlot.SetSlotByFusion(itemData,matData,true);
			return;
		}
		
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(itemData.ItemGUID);
		if (itemTmp == null)
			return;
		//材料為道具才show數量
		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || 
		    itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			m_NowItemSlot.SetSlotByFusion(itemData,matData,false);
		}
		else
			m_NowItemSlot.SetSlotByFusion(itemData,matData,true);
	}
	//------------------------------------------------------------------------------------------------
	public void SetNeedMoney(S_Fusion_Tmp fusionTmp)
	{
		if (fusionTmp == null)
			return;
		
		int needMoney = fusionTmp.iCost;
		
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney() < needMoney)
		{
			//錢不夠
			lbNeedMoney.text = string.Format("{0}{1}{2}", 
			                                 GameDataDB.GetString(1327), 
			                                 needMoney.ToString(),
			                                 GameDataDB.GetString(1329));	//[FF0000] [-] 紅色
			btnFusionOrUpRank.isEnabled = false;
		}
		else
		{
			lbNeedMoney.text = needMoney.ToString();
			btnFusionOrUpRank.isEnabled = true;
		}
	}
	//------------------------------------------------------------------------------------------------
	public void SetAutoFusionMoney(S_Fusion_Tmp fusionTmp, int fusionCount)
	{
		if (fusionTmp == null)
			return;
		
		int needMoney = fusionTmp.iCost * fusionCount;
		
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBodyMoney() < needMoney)
		{
			//錢不夠
			lbAutoFusionMoney.text = string.Format("{0}{1}{2}", 
			                                 GameDataDB.GetString(1327), 
			                                 needMoney.ToString(),
			                                 GameDataDB.GetString(1329));	//[FF0000] [-] 紅色
			btnAutoFusionOK.isEnabled = false;
		}
		else
		{
			lbAutoFusionMoney.text = needMoney.ToString();
			btnAutoFusionOK.isEnabled = true;
		}
	}
}
