using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class UI_SummonResult : NGUIChildGUI 
{
	public UIPanel		panelResultSet		= null;		//召喚結果集合
	public UIPanel		panelMaskHit		= null;		//遮罩點擊層級
	public UISprite		spriteLightBorder	= null;		//顯示召喚的LightBorder Prefab
	public UISprite		spriteSfx			= null;		//顯示召喚的sfx
	public UILabel		lbResultTitle		= null;		//召喚結果Title
	public UISprite		spriteBorder		= null;		//召喚內容框
	public UIGrid		GridMultiUsed		= null;		//連十召喚的parent
	public UILabel		lbSummonAgainPrice	= null;		//再次召喚的價格
	public UISprite		spritePriceIcon		= null;		//再次召喚的圖樣
	public UIButton		btnSummonAgain		= null;		//再次召喚按鈕
	public UILabel		lbSummonAgain		= null;		//再次召喚字樣
	public UIButton		btnEndSummon		= null;		//結束召喚按鈕
	public UILabel		lbEndSummon			= null;		//結束召喚字樣
	public UIWidget		MaskHit				= null;		//遮罩點擊
	public UIWidget		SkipHit				= null;		//略過點擊
	public UILabel		lbPieceTip			= null;		//重複召喚自動換算成碎片說明
	//
	[HideInInspector]public List<GameObject>		LightBorderList	= new List<GameObject>();		//召喚物特別顯示
	[HideInInspector]public List<GameObject>		SfxList			= new List<GameObject>();		//召喚物特效
    [System.NonSerialized]
    public List<S_Item_Tmp> ItemList = new List<S_Item_Tmp>();	//召喚物tmp集合
	[HideInInspector]public List<bool>				bFirstShow		= new List<bool>();
	[HideInInspector]public List<UISprite>			ItemSpriteBGs	= new List<UISprite>();
	//
	public Transform	SfxTransform		= null;
	//
	private const int iMultiSummonWidth		= 900;
	private const int iSingleSummonWidth	= 650;
	//
	private const string	m_SlotName			="Slot_Item";
	[HideInInspector]public List<Slot_Item> 	itemSlotList 		= new List<Slot_Item>();	// 道具格slot
	[HideInInspector]public List<UIWidget>		itemSlotAlpha		= new List<UIWidget>();		// 道具格透明度
	[HideInInspector]public Slot_Item  singleItem;
	private Slot_Item  ItemPrefab;
	private GameObject	SingleLightBorder;
	private GameObject	SingleSfx;
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UISprite		spGuideQuit						= null;	//導引離開召喚結果UI
	public UILabel		lbGuideQuit						= null;
	// smartObjectName
	private const string 			GUI_SMARTOBJECT_NAME 				= "UI_SummonResult";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_SummonResult() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		MultiSummonInit();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		panelResultSet.gameObject.SetActive(false);
		panelMaskHit.gameObject.SetActive(true);
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		lbResultTitle.text 	= GameDataDB.GetString(1105);			//召喚結果
		lbSummonAgain.text 	= GameDataDB.GetString(1103);			//再次召喚
		lbEndSummon.text	= GameDataDB.GetString(1104);			//結束召喚
		lbPieceTip.text 	= GameDataDB.GetString(974);			//[FFDC35]已獲得夥伴將轉化為碎片，轉化數量等同於召喚該夥伴所需碎片數量[-]
	}
	//-----------------------------------------------------------------------------------------------------
	private void MultiSummonInit()
	{
		ItemPrefab = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		if(ItemPrefab == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		singleItem = Instantiate(ItemPrefab) as Slot_Item;

		singleItem.transform.parent = spriteBorder.transform;
		singleItem.transform.localScale		= new Vector3(1.2f,1.2f,1.2f);
		singleItem.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
		singleItem.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
		singleItem.gameObject.name = "singleItem";
		//GameObject.Destroy(singleItem.GetComponent<UIButton>());
		singleItem.InitialSlot();
		//顯示特效控制
		SingleLightBorder 	= NGUITools.AddChild(singleItem.gameObject,spriteLightBorder.gameObject);
		SingleSfx			= NGUITools.AddChild(singleItem.gameObject,spriteSfx.gameObject);
		//
		Transform t;
		//Slot
		for(int i=0;i<GameDefine.ITEMMALL_PETLOTTERY_EX_COUNT;++i)
		{
			Slot_Item newgo= Instantiate(ItemPrefab) as Slot_Item;
			UIWidget ud = newgo.transform.GetComponent<UIWidget>();
			itemSlotAlpha.Add(ud);
			//
			newgo.transform.parent			= GridMultiUsed.transform;
			newgo.transform.localScale		= new Vector3(1.2f,1.2f,1.2f);
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
			newgo.name = string.Format("slotItem{0:00}",i);
			//GameObject.Destroy(newgo.GetComponent<UIButton>());
			newgo.InitialSlot();
			//
			t= newgo.transform.FindChild("SpriteBG");
			ItemSpriteBGs.Add(t.GetComponent<UISprite>());
			//
			LightBorderList.Add(NGUITools.AddChild(newgo.gameObject,spriteLightBorder.gameObject));
			SfxList.Add(NGUITools.AddChild(newgo.gameObject,spriteSfx.gameObject));
			//
			newgo.gameObject.SetActive(false);
			itemSlotList.Add(newgo);

		}
		singleItem.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	//設定召喚顯示相關顯示
	public void SetSummonTypeDisplay(ENUM_PetLotteryType ptype)
	{
		S_Item_Tmp sItem;
		ItemList.Clear();
		bFirstShow.Clear();
		int PetID = -1;
		if(ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_DiamondEX ||
		   ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_MoneyEX	)
		{
			singleItem.gameObject.SetActive(false);
			spriteBorder.width = iMultiSummonWidth;
			GridMultiUsed.gameObject.SetActive(true);
			//設定資料
			//判斷蒐集的資料是否等於召喚的資料
			if(ARPGApplication.instance.m_TempCollectorSystem.Get_dIntDataSize() == GameDefine.ITEMMALL_PETLOTTERY_EX_COUNT)
			{
				//先初始化暫存器
				ARPGApplication.instance.m_TempCollectorSystem.Iterate_dIntCollector();
				for(int i=0;i<GameDefine.ITEMMALL_PETLOTTERY_EX_COUNT;++i)
				{
					KeyValuePair<int,int> kv = ARPGApplication.instance.m_TempCollectorSystem.Get_dIntData();
					sItem = GameDataDB.ItemDB.GetData(kv.Key);
					ItemList.Add(sItem);
					LightBorderList[i].SetActive(IsLightItem(sItem,ptype));
					if(kv.Value>1)
						itemSlotList[i].SetSlotWithCount(sItem.GUID,kv.Value,false);
					else
						itemSlotList[i].SetSlotWithCount(sItem.GUID,0,false);
					SfxList[i].SetActive(true);
				}
			}
			else
				UnityDebugger.Debugger.LogError("DataLost only Get"+ARPGApplication.instance.m_TempCollectorSystem.GetIntDataSize().ToString());
		}
		else if(ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_Diamond ||
		   		ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_Money ||
		        ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_VIP)
		{
			singleItem.gameObject.SetActive(true);
			spriteBorder.width = iSingleSummonWidth;
			GridMultiUsed.gameObject.SetActive(false);
			//spritePetPrefab.gameObject.SetActive(true);
			//設定資料
			//判斷蒐集的資料是否等於召喚的資料
			if(ARPGApplication.instance.m_TempCollectorSystem.Get_dIntDataSize() == 1)
			{
				//先初始化暫存器
				ARPGApplication.instance.m_TempCollectorSystem.Iterate_dIntCollector();
				KeyValuePair<int,int> kv = ARPGApplication.instance.m_TempCollectorSystem.Get_dIntData();
				sItem = GameDataDB.ItemDB.GetData(kv.Key);
				ItemList.Add(sItem);
				SingleLightBorder.SetActive(IsLightItem(sItem,ptype));
				PetID = GetPetID(sItem);
				bFirstShow.Add(PetID != -1?ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.IsFirstTimeGetThisPet(PetID):false);
				SingleSfx.SetActive(true);
				if(kv.Value>1)
					singleItem.SetSlotWithCount(sItem.GUID,kv.Value,false);
				else
					singleItem.SetSlotWithCount(sItem.GUID,0,false);
			}
			else
				UnityDebugger.Debugger.LogError("DataLost only Get"+ARPGApplication.instance.m_TempCollectorSystem.GetIntDataSize().ToString());
		}
		//判斷哪些需要首演
		SaveNeedShowFirstFlag();
	}
	//-----------------------------------------------------------------------------------------------------
	private int GetPetID(S_Item_Tmp sitem)
	{
		//非寵物相關即跳錯
		if(sitem.ItemType == ENUM_ItemType.ENUM_ItemType_Pet/* || sitem.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece*/)
			return sitem.iPetID;
		
		//非寵物相關
		return -1;
	}
	//-----------------------------------------------------------------------------------------------------
	public void InitItemState()
	{
		for(int i=0;i<GameDefine.ITEMMALL_PETLOTTERY_EX_COUNT;++i)
		{
			itemSlotList[i].gameObject.SetActive(false);
			ItemSpriteBGs[i].alpha = 0;
			SfxList[i].GetComponent<TweenAlpha>().ResetToBeginning();
			SfxList[i].GetComponent<TweenHeight>().ResetToBeginning();
			SfxList[i].GetComponent<TweenWidth>().ResetToBeginning();
			//
			SfxList[i].GetComponent<TweenAlpha>().PlayForward();
			SfxList[i].GetComponent<TweenHeight>().PlayForward();
			SfxList[i].GetComponent<TweenWidth>().PlayForward();
		}
		singleItem.transform.FindChild("SpriteBG").GetComponent<UISprite>().alpha = 0;
		singleItem.gameObject.SetActive(false);
		SingleSfx.GetComponent<TweenAlpha>().ResetToBeginning();
		SingleSfx.GetComponent<TweenHeight>().ResetToBeginning();
		SingleSfx.GetComponent<TweenWidth>().ResetToBeginning();
		//
		SingleSfx.GetComponent<TweenAlpha>().PlayForward();
		SingleSfx.GetComponent<TweenHeight>().PlayForward();
		SingleSfx.GetComponent<TweenWidth>().PlayForward();
	}
	//-----------------------------------------------------------------------------------------------------
	private bool IsLightItem(S_Item_Tmp sitem,ENUM_PetLotteryType ptype)
	{
		/*if(ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_Diamond ||
		   ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_DiamondEX	)
		{
			if(sitem.ItemType == ENUM_ItemType.ENUM_ItemType_Pet || 
			   sitem.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece )
				return true;
		}
		else if(ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_Money ||
		        ptype == ENUM_PetLotteryType.ENUM_PetLotteryType_MoneyEX	)
		{
			if(sitem.ItemType == ENUM_ItemType.ENUM_ItemType_Pet || 
			   sitem.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece )
				return true;
		}*/
		if(sitem.ItemType == ENUM_ItemType.ENUM_ItemType_Pet)
			return true;
			
		return false;
	}
	//-----------------------------------------------------------------------------------------------------
	private void SaveNeedShowFirstFlag()
	{
		if(ItemList.Count<=1)
			return;

		List<int> PetIDList = new List<int>();

		bFirstShow.Clear();
		for(int i=0;i<ItemList.Count;++i)
		{
			int petID = GetPetID(ItemList[i]);
			if (petID > 0 && PetIDList.Contains(petID))
			{
				PetIDList.Add(-1);
				continue;
			}
			PetIDList.Add(GetPetID(ItemList[i]));
		}

		for(int i=0;i<PetIDList.Count;++i)
		{
			if(PetIDList[i] > 0)
			{
				bFirstShow.Add(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.IsFirstTimeGetThisPet(PetIDList[i]));
				//CheckPID.Add(PetID[i]);
			}
			else
				bFirstShow.Add(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
}
