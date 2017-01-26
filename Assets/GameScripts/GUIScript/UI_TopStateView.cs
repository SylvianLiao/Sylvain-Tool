using System;
using UnityEngine;
using GameFramework;

public class UI_TopStateView : NGUIChildGUI 
{
	public UIPanel			panelBase			= null;
	//
	public UIButton			btnQuit 			= null;
//	public UIButton			btnBack				= null;
	public UILabel			lbAP				= null;
	public UILabel			lbGold				= null;
	public UILabel			lbMoney				= null;
	public UILabel			lbFP 				= null;
	public UIButton			btnFullAP			= null;
	public UIButton			btnTapCash			= null;
	public UIButton			btnDepositDiamond	= null;
	public UIButton			btnCrystalAdd		= null;

	public bool				backEnable			= true;
	public bool				quitEnable			= true;
	//
// 	public delegate void closeBtnFunction();
// 	public closeBtnFunction onCloseBtnFunction;	//關閉按鈕時執行
	public delegate void backBtnFunction();
	public backBtnFunction onBackBtnFunction;	//回上面按鈕時執行

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_TopStateView";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_TopStateView() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		if (ARPGApplication.instance.m_TeachingSystem.m_CurrentGuideType == ENUM_GUIDETYPE.Null)
			SwitchBtnWork (true);
		AssignInfoToBar();
	}
	//-------------------------------------------------------------------------------------------------------------
	public void Update()
	{
		AssignInfoToBar();

		//手機回上一頁功能
		if(Input.GetKeyDown(KeyCode.Escape)&& this.gameObject.activeSelf)
		{
			if(quitEnable && ARPGApplication.instance.m_TeachingSystem.m_CurrentGuideType == ENUM_GUIDETYPE.Null)
			{
				EventDelegate.Remove(btnQuit.onClick, TopStateViewBack);
				TopStateViewBack();
				EventDelegate.Add(btnQuit.onClick, TopStateViewBack);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------------------
	void Awake()
	{
		EventDelegate.Add(btnQuit.onClick, TopStateViewBack);

//		UIEventListener.Get(btnQuit.gameObject).onClick			    += TopStateViewBack;
		UIEventListener.Get(btnFullAP.gameObject).onClick			+= OnBtnClick;
		UIEventListener.Get(btnTapCash.gameObject).onClick			+= OnBtnClick;
		UIEventListener.Get(btnDepositDiamond.gameObject).onClick 	+= OnBtnClick;
	}

	//-------------------------------------------------------------------------------------------------------------
	void AssignInfoToBar()
	{
		S_PlayerData_Tmp dbf = GameDataDB.PlayerDB.GetData(ARPGApplication.instance.m_RoleSystem.iBaseLevel);
		
		lbGold.text		= ARPGApplication.instance.m_RoleSystem.iBaseItemMallMoney.ToString();
		lbMoney.text	= ARPGApplication.instance.m_RoleSystem.iBaseBodyMoney.ToString();
		lbFP.text		= ARPGApplication.instance.m_RoleSystem.iBaseFP.ToString();
		lbAP.text		= ARPGApplication.instance.m_RoleSystem.iBaseAP.ToString () + "/" + dbf.iMaxAP.ToString();
	}
	//---------------------------------------------------------------------------------------------------
	public void TopStateViewBack(/*GameObject go*/)
	{
		if(onBackBtnFunction != null)
			onBackBtnFunction();
        else
			ARPGApplication.instance.PopState();
	}
// 	//---------------------------------------------------------------------------------------------------
// 	public void TopStateViewQuit()
// 	{
// // 		if(onCloseBtnFunction != null)
// // 			onCloseBtnFunction();
// 
// 		ARPGApplication.instance.PopStateUntilOne();
// 	}
	//---------------------------------------------------------------------------------------------------
	private void OnBtnClick(GameObject gb)
	{
		if(gb == btnFullAP.gameObject)
			ARPGApplication.instance.RequestEventBuyAP();
		else if(gb == btnDepositDiamond.gameObject)
			ARPGApplication.instance.ConnectToDepositDiamond();
		else if(gb == btnTapCash.gameObject)
			ARPGApplication.instance.ConnectTotTapCash();
	}
	//---------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------
	public void SwitchBtnWork(bool sw)
	{
		btnQuit.isEnabled 			= sw;
		btnFullAP.isEnabled 			= sw;
		btnDepositDiamond.isEnabled 	= sw;
		btnTapCash.isEnabled 			= sw;
		btnCrystalAdd.isEnabled 		= sw;
	}
	//---------------------------------------------------------------------------------------------------
}
