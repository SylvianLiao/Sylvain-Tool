using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_DisItem : NGUIChildGUI 
{
	//--------------------------------------UI-------------------------------------------------------
	[HideInInspector]public UI_BagTip		uiTargetEquipTip		= null;		//左邊合成升階目標裝備的資訊
	public UIPanel							panelBase				= null;
	[Header("BackGorund")]  				
	public UIWidget 						wgBackGround			= null;
	public UIButton							btnClose				= null;
	public UILabel							lbDisItemTitle			= null;
	
	[Header("RightInfo")]
	public UIWidget							wgRightInfo				= null;		//右邊資訊集合
	public UIButton							btnDisItem				= null;
	public UILabel							lbDisItem				= null;
	public Transform [] 					m_DisItemMatPos			= new Transform[6];

	[Header("NewGuide")]
	public UIPanel			panelGuide					= null; 	//導引相關集合
	public UIButton			btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton			btnFullScreen				= null; 	//防點擊
	public UISprite			spGuideDisItem				= null;		//導引拆解
	public UILabel			lbGuideDisItem				= null; 

	//------------------------------------管理容器-----------------------------------------------------------
	[HideInInspector] private List<Slot_Item> 	m_ResultSlotList	= new List<Slot_Item>();
	
	//-------------------------------------------------------------------------------------------------
	[HideInInspector]private string			m_SlotName				= "Slot_Item";
	[HideInInspector]private string			m_BagTipName			= "UI_BagTip";
	// smartObjectName
	private const string 					GUI_SMARTOBJECT_NAME 	= "UI_DisItem";
	
	//-------------------------------------------------------------------------------------------------
	private UI_DisItem() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitialBagTip();
		InitDisItemResultSlot();
		InitialUI();
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
	private void InitialUI()
	{	
		lbDisItemTitle.text		= GameDataDB.GetString(5105); 	//拆解	
		lbDisItem.text			= GameDataDB.GetString(5140); 	//確定拆解
	}
	//-------------------------------------------------------------------------------------------------
	private void InitialBagTip()
	{
		UI_BagTip go = ResourceManager.Instance.GetGUI(m_BagTipName).GetComponent<UI_BagTip>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_DisItem load prefeb error,path:{0}", "GUI/"+m_BagTipName) );
			return;
		}
		//左邊拆解目標資訊
		UI_BagTip newgo	= Instantiate(go) as UI_BagTip;
		newgo.transform.parent			= this.transform;
		newgo.transform.localPosition	= Vector3.zero;
		newgo.transform.localScale		= Vector3.one;
		newgo.gameObject.SetActive(true);
		uiTargetEquipTip = newgo;
		uiTargetEquipTip.animTipBoard.enabled = false;
		//由於不把UI_BagTip加入GUI_Maneger，故不呼叫初始化Initialize()
		uiTargetEquipTip.InitialTip();
		uiTargetEquipTip.SetPanelDepth(panelBase.depth+1);
	}
	//-------------------------------------------------------------------------------------------------
	private void InitDisItemResultSlot()
	{
		if(m_SlotName == "")
		{
			m_SlotName = "Slot_Item";
		}
		
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_DisItem load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}

		for(int i=0; i< m_DisItemMatPos.Length; ++i)
		{
			//拆解結果材料
			Slot_Item newgo	= Instantiate(go) as Slot_Item;
			newgo.transform.parent	= m_DisItemMatPos[i];
			newgo.transform.localPosition	= Vector3.zero;
			newgo.transform.localScale		= Vector3.one;
			newgo.gameObject.SetActive(false);
			m_ResultSlotList.Add(newgo);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定右方拆解結果
	public void SetDisItemResult(S_Item_Tmp itemTmp)
	{
		if (m_ResultSlotList.Count <= 1)
			return;

		m_ResultSlotList[1].SetSlotWithCount(itemTmp.iDisItem, itemTmp.iDisItemCount, true);
		m_ResultSlotList[1].gameObject.SetActive(true);
	}
}
