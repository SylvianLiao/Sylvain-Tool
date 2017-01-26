using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum ENUM_BlackSmithButton
{
	Button_UpRank = 0,
	Button_Melting,
	Button_Purification,
	Button_Fusion,
	Button_DisItem,
	ENUM_BlackSmithButton_MAX,
}

public class BlackSmithButton
{
	public string 		ButtonName	 	= null;
	public UISprite 	LightSprite	 	= null;
	public UISprite 	DarkSprite	 	= null;
}

public class Slot_BlackSmithPage : NGUIChildGUI
{
	[Header("UI")]
	public UIGrid 			gdPage 				= null;

	public UIButton			btnUpRank			= null;		// 升階分頁事件按鈕
	public UIButton			btnMelting			= null;		// 熔煉分頁事件按鈕
	public UIButton			btnPurification		= null;		// 洗鍊分頁事件按鈕
	public UIButton			btnFusion			= null;		// 合成分頁事件按鈕
	public UIButton			btnDisItem			= null;		// 拆解分頁事件按鈕

	public UISprite			spLightUpRank		= null;	
	public UISprite			spLightMelting		= null;	
	public UISprite			spLightPurification	= null;		
	public UISprite			spLightFusion		= null;	
	public UISprite			spLightDisItem		= null;	

	public UISprite			spDarkUpRank		= null;	
	public UISprite			spDarkMelting		= null;	
	public UISprite			spDarkPurification	= null;		
	public UISprite			spDarkFusion		= null;	
	public UISprite			spDarkDisItem		= null;	

	public UILabel			lbUpRank			= null;
	public UILabel			lbMelting			= null;
	public UILabel			lbPurification		= null;
	public UILabel			lbFusion			= null;
	public UILabel			lbDisItem			= null;

//	[Header("NewGuide")]
//	public UIPanel				panelGuide					= null; 	//導引相關集合
//	public UIButton				btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
//	public UIButton				btnFullScreen				= null; 	//任意鍵繼續導引
//	public UISprite				spGuideSelectItem			= null; 	//導引選擇物品
	//-------------------------------------------------------------------------------------------------
	[HideInInspector] private S_ItemData 			m_TargetItemData 	= null;
	[HideInInspector] private S_Item_Tmp 			m_TargetItemTmp 	= null;
	[HideInInspector] public ENUM_BlackSmithButton  m_CurrentPage 		= ENUM_BlackSmithButton.Button_UpRank;
	//開啟中的按鈕管理器
	[HideInInspector] public Dictionary<ENUM_BlackSmithButton, BlackSmithButton> m_ActiveBtnList = new Dictionary<ENUM_BlackSmithButton, BlackSmithButton>();

	// smartObjectName
	private const string 				GUI_SMARTOBJECT_NAME 	= "Slot_BlackSmithPage";
	//-------------------------------------------------------------------------------------------------
	private Slot_BlackSmithPage() : base(GUI_SMARTOBJECT_NAME)
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
	public override void Show()
	{
		base.Show();
		//AddCallBack();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
		//RemoveCallBack();
	}
	//-------------------------------------------------------------------------------------------------
	private void InitialUI()
	{
		lbFusion.text			= GameDataDB.GetString(5103); 	//合成
		lbUpRank.text			= GameDataDB.GetString(1009); 	//升階
		lbMelting.text			= GameDataDB.GetString(205); 	//熔煉		
		lbPurification.text		= GameDataDB.GetString(5104); 	//洗煉	
		lbDisItem.text			= GameDataDB.GetString(5105); 	//拆解
	}
	//-------------------------------------------------------------------------------------------------
	public void AddCallBack()
	{
		UIEventListener.Get(btnUpRank.gameObject).onClick 				+= OnFusionOrUprankClick;
		UIEventListener.Get(btnMelting.gameObject).onClick 				+= OnMeltingClick;
		UIEventListener.Get(btnPurification.gameObject).onClick 		+= OnPurificationClick;
		UIEventListener.Get(btnFusion.gameObject).onClick 				+= OnFusionOrUprankClick;
		UIEventListener.Get(btnDisItem.gameObject).onClick 				+= OnDisItemClick;
	}
	//-------------------------------------------------------------------------------------------------
	public void RemoveCallBack()
	{
		UIEventListener.Get(btnUpRank.gameObject).onClick 				-= OnFusionOrUprankClick;
		UIEventListener.Get(btnMelting.gameObject).onClick 				-= OnMeltingClick;
		UIEventListener.Get(btnPurification.gameObject).onClick 		-= OnPurificationClick;
		UIEventListener.Get(btnFusion.gameObject).onClick 				-= OnFusionOrUprankClick;
		UIEventListener.Get(btnDisItem.gameObject).onClick 				-= OnDisItemClick;
	}
	//-------------------------------------------------------------------------------------------------
	//從鍛造按鈕第一次打開分頁時做的設定
	public void FirstOpenPage()
	{
		if(m_TargetItemData == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnBtnBlackSmithClick() GetTargetItemData() == {0}", m_TargetItemData));
			return;
		}
		
		//開啟預設的分頁
		switch(m_CurrentPage)
		{
		case ENUM_BlackSmithButton.Button_UpRank:
		case ENUM_BlackSmithButton.Button_Fusion:
			FusionState fusionState = ARPGApplication.instance.GetGameStateByName(GameDefine.FUSION_STATE) as FusionState;
			if(fusionState == null)
			{
				UnityDebugger.Debugger.LogError(string.Format(" OnBtnBlackSmithClick() fusionState == {0}", fusionState));
				return;
			}
			if (m_CurrentPage == ENUM_BlackSmithButton.Button_UpRank)
				fusionState.m_UI_Mode = ENUM_UI_Mode.ENUM_UpRank;
			else if (m_CurrentPage == ENUM_BlackSmithButton.Button_Fusion)
				fusionState.m_UI_Mode = ENUM_UI_Mode.ENUM_Fusion;
			
			fusionState.SetCurrentItemData(m_TargetItemData);
			
			ARPGApplication.instance.PushState(GameDefine.FUSION_STATE);
			break;
		case ENUM_BlackSmithButton.Button_Melting:
			EquipMeltingState meltingState = ARPGApplication.instance.GetGameStateByName(GameDefine.EQUIPMELTING_STATE) as EquipMeltingState;
			if(meltingState == null)
			{
				UnityDebugger.Debugger.LogError(string.Format(" OnBtnBlackSmithClick() meltingState == {0}", meltingState));
				return;
			}
			meltingState.SetSelectSerialID(m_TargetItemData.iSerial);
			meltingState.SetSelectItemData(m_TargetItemData);
			
			ARPGApplication.instance.PushState(GameDefine.EQUIPMELTING_STATE);
			break;
		case ENUM_BlackSmithButton.Button_Purification:
			PurificationState purState = ARPGApplication.instance.GetGameStateByName(GameDefine.PURIFICATION_STATE) as PurificationState;
			if(purState == null)
			{
				UnityDebugger.Debugger.LogError(string.Format(" OnBtnBlackSmithClick() purState == {0}", purState));
				return;
			}
			purState.SetCurrentItemData(m_TargetItemData);
			
			ARPGApplication.instance.PushState(GameDefine.PURIFICATION_STATE);
			break;
		case ENUM_BlackSmithButton.Button_DisItem:
			DisItemState disItemState = ARPGApplication.instance.GetGameStateByName(GameDefine.DISITEM_STATE) as DisItemState;
			if(disItemState == null)
			{
				UnityDebugger.Debugger.LogError(string.Format(" OnDisItemClick() disItemState == {0}", disItemState));
				return;
			}
			disItemState.SetCurrentItem(m_TargetItemData);
			
			ARPGApplication.instance.PushState(GameDefine.DISITEM_STATE);
			break;
		}

		//將所有按鈕的反灰圖按關閉
		foreach(BlackSmithButton btn in m_ActiveBtnList.Values)
		{
			btn.LightSprite.gameObject.SetActive(false);
		}

		ChangePage(m_CurrentPage);
	
		Show();
	}
	#region 存取資料
	//-------------------------------------------------------------------------------------------------
	public void SetTargetItem(ulong iSerial)
	{
		m_TargetItemData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(iSerial);
		if (m_TargetItemData != null)
			m_TargetItemTmp = GameDataDB.ItemDB.GetData(m_TargetItemData.ItemGUID);
		else
			m_TargetItemTmp = null;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetTargetItem(S_ItemData itemData)
	{
		if (itemData == null)
			return;
		m_TargetItemData = itemData;
		m_TargetItemTmp = GameDataDB.ItemDB.GetData(m_TargetItemData.ItemGUID);
	}
	//-------------------------------------------------------------------------------------------------
	public S_ItemData GetTargetItemData()
	{
		return m_TargetItemData;
	}
	//-------------------------------------------------------------------------------------------------
	public S_Item_Tmp GetTargetItemTmp()
	{
		return m_TargetItemTmp;
	}
	#endregion
	#region 按鈕功能
	//-----------------------------------------------------------------------------------------------------
	public void OnFusionOrUprankClick(GameObject go)
	{
		if(m_TargetItemData == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnFusionOrUpRankClick() m_TargetItemData == {0}", m_TargetItemData));
			return;
		}

		FusionState fusionState = ARPGApplication.instance.GetGameStateByName(GameDefine.FUSION_STATE) as FusionState;
		if(fusionState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnFusionOrUpRankClick() fusionState == {0}", fusionState));
			return;
		}

		if (go == btnFusion.gameObject)
		{
			if (m_CurrentPage == ENUM_BlackSmithButton.Button_Fusion)
				return;

			ChangePage(ENUM_BlackSmithButton.Button_Fusion);
			fusionState.m_UI_Mode = ENUM_UI_Mode.ENUM_Fusion;
		}
		else if (go == btnUpRank.gameObject)
		{
			if (m_CurrentPage == ENUM_BlackSmithButton.Button_UpRank)
				return;

			ChangePage(ENUM_BlackSmithButton.Button_UpRank);
			fusionState.m_UI_Mode = ENUM_UI_Mode.ENUM_UpRank;
		}

		fusionState.SetCurrentItemData(m_TargetItemData);

		BackToUI();

		ARPGApplication.instance.PushState(GameDefine.FUSION_STATE);
	}
	//-----------------------------------------------------------------------------------------------------
	public void OnMeltingClick(GameObject go)
	{
		if(m_TargetItemData == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnMeltingClick() data == {0}", m_TargetItemData));
			return;
		}

		EquipMeltingState meltingState = ARPGApplication.instance.GetGameStateByName(GameDefine.EQUIPMELTING_STATE) as EquipMeltingState;
		if(meltingState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnMeltingClick() meltingState == {0}", meltingState));
			return;
		}
		if (m_CurrentPage == ENUM_BlackSmithButton.Button_Melting)
			return;

		ChangePage(ENUM_BlackSmithButton.Button_Melting);
		meltingState.SetSelectSerialID(m_TargetItemData.iSerial);
		meltingState.SetSelectItemData(m_TargetItemData);

		BackToUI();

		ARPGApplication.instance.PushState(GameDefine.EQUIPMELTING_STATE);
	}
	//-----------------------------------------------------------------------------------------------------
	public void OnPurificationClick(GameObject go)
	{
		if(m_TargetItemData == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnPurificationClick() m_TargetItemData == {0}", m_TargetItemData));
			return;
		}

		PurificationState purState = ARPGApplication.instance.GetGameStateByName(GameDefine.PURIFICATION_STATE) as PurificationState;
		if(purState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnPurificationClick() purState == {0}", purState));
			return;
		}
		if (m_CurrentPage == ENUM_BlackSmithButton.Button_Purification)
			return;

		ChangePage(ENUM_BlackSmithButton.Button_Purification);

		purState.SetCurrentItemData(m_TargetItemData);

		BackToUI();

		ARPGApplication.instance.PushState(GameDefine.PURIFICATION_STATE);
	}
	//-----------------------------------------------------------------------------------------------------
	public void OnDisItemClick(GameObject go)
	{
		if(m_TargetItemData == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnDisItemClick() m_TargetItemData == {0}", m_TargetItemData));
			return;
		}
		
		DisItemState disItemState = ARPGApplication.instance.GetGameStateByName(GameDefine.DISITEM_STATE) as DisItemState;
		if(disItemState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnDisItemClick() disItemState == {0}", disItemState));
			return;
		}
		if (m_CurrentPage == ENUM_BlackSmithButton.Button_DisItem)
			return;

		ChangePage(ENUM_BlackSmithButton.Button_DisItem);

		disItemState.SetCurrentItem(m_TargetItemData);

		BackToUI();

		ARPGApplication.instance.PushState(GameDefine.DISITEM_STATE);
	}
	#endregion
	#region 按鈕開關
	//-----------------------------------------------------------------------------------------------------
	public void SwitchFusionButton(bool bSwitch)
	{
		if(btnFusion == null)
			return;
		btnFusion.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchUpRankButton(bool bSwitch)
	{
		if(btnUpRank == null)
			return;
		btnUpRank.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchMeltingButton(bool bSwitch)
	{
		if(btnMelting == null)
			return;
		btnMelting.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchPurificationButton(bool bSwitch)
	{
		if(btnPurification == null)
			return;
		btnPurification.gameObject.SetActive(bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SwitchDisItemButton(bool bSwitch)
	{
		if(btnDisItem == null)
			return;
		btnDisItem.gameObject.SetActive(bSwitch);
	}
	#endregion
	//-----------------------------------------------------------------------------------------------------
	//切換頁籤圖示
	public void ChangePage(ENUM_BlackSmithButton btnPage)
	{
		//反灰原按鈕
		if (m_ActiveBtnList.ContainsKey(m_CurrentPage))
		{
			m_ActiveBtnList[m_CurrentPage].LightSprite.gameObject.SetActive(false);
			m_ActiveBtnList[m_CurrentPage].DarkSprite.gameObject.SetActive(true);
		}

		m_CurrentPage = btnPage;

		//高亮新按鈕
		if (m_ActiveBtnList.ContainsKey(btnPage))
		{
			m_ActiveBtnList[btnPage].LightSprite.gameObject.SetActive(true);
			m_ActiveBtnList[btnPage].DarkSprite.gameObject.SetActive(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設定顯示的分頁,若所有分頁皆關閉則關閉鍛造按鈕, 回傳按鈕啟動的數量
	public int SetBtnActiveOrNot(S_Item_Tmp itemTmp, S_ItemData itemData)
	{
		m_ActiveBtnList.Clear();
		BlackSmithButton btn = null;

		bool isActive = ARPGApplication.instance.CheckBlackSmithBtnIsActive(ENUM_BlackSmithButton.Button_UpRank,itemTmp,itemData);
		SwitchUpRankButton(isActive);
		if (isActive)
		{
			btn = new BlackSmithButton();
			btn.ButtonName = lbUpRank.text;
			btn.LightSprite = spLightUpRank;
			btn.DarkSprite = spDarkUpRank;
			m_ActiveBtnList.Add(ENUM_BlackSmithButton.Button_UpRank, btn);		
		}
		//
		isActive = ARPGApplication.instance.CheckBlackSmithBtnIsActive(ENUM_BlackSmithButton.Button_Fusion,itemTmp,itemData);
		SwitchFusionButton(isActive);
		if (isActive)
		{
			btn = new BlackSmithButton();
			btn.ButtonName = lbFusion.text;
			btn.LightSprite = spLightFusion;
			btn.DarkSprite = spDarkFusion;
			m_ActiveBtnList.Add(ENUM_BlackSmithButton.Button_Fusion, btn);	
		}
		//
		isActive = ARPGApplication.instance.CheckBlackSmithBtnIsActive(ENUM_BlackSmithButton.Button_Melting,itemTmp,itemData);
		SwitchMeltingButton(isActive);
		if (isActive)
		{
			btn = new BlackSmithButton();
			btn.ButtonName = lbMelting.text;
			btn.LightSprite = spLightMelting;
			btn.DarkSprite = spDarkMelting;
			m_ActiveBtnList.Add(ENUM_BlackSmithButton.Button_Melting,btn);	
		}
		//
		isActive = ARPGApplication.instance.CheckBlackSmithBtnIsActive(ENUM_BlackSmithButton.Button_DisItem,itemTmp,itemData);
		SwitchDisItemButton(isActive);
		if (isActive)
		{
			btn = new BlackSmithButton();
			btn.ButtonName = lbDisItem.text;
			btn.LightSprite = spLightDisItem;
			btn.DarkSprite = spDarkDisItem;
			m_ActiveBtnList.Add(ENUM_BlackSmithButton.Button_DisItem, btn);			
		}
		//
		isActive = ARPGApplication.instance.CheckBlackSmithBtnIsActive(ENUM_BlackSmithButton.Button_Purification,itemTmp,itemData);
		SwitchPurificationButton(isActive);
		if (isActive)
		{
			btn = new BlackSmithButton();
			btn.ButtonName = lbPurification.text;
			btn.LightSprite = spLightPurification;
			btn.DarkSprite = spDarkPurification;
			m_ActiveBtnList.Add(ENUM_BlackSmithButton.Button_Purification, btn);			
		}

		//重新排列分頁
		gdPage.enabled = true;
		gdPage.Reposition();

		//預設打開可啟動的第一個分頁
		if (ARPGApplication.instance.GetCurrentGameState().name == GameDefine.ITEMBAG_STATE)
		{
			foreach(ENUM_BlackSmithButton button in m_ActiveBtnList.Keys)
			{
				ChangePage(button);
				break;
			}
		}

		return m_ActiveBtnList.Count;
	}
	//-----------------------------------------------------------------------------------------------------
	public bool CheckBtnActiveOrNot(ENUM_BlackSmithButton checkBtn)
	{
		return ARPGApplication.instance.CheckBlackSmithBtnIsActive(checkBtn,m_TargetItemTmp,m_TargetItemData);
	}
	//-----------------------------------------------------------------------------------------------------
	private void BackToUI()
	{
		ARPGApplication.instance.PopState();
	}
}

