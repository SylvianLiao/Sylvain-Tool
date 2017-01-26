using System;
using UnityEngine;
using GameFramework;

public class UI_ForSalePetMaterial  : NGUIChildGUI 
{
	public UIPanel		panelBase				= null;		//素材出售介面

	//-----------------------------------------------------------------------
	//SelectedMaterialSet
	public UIPanel 	panelSelectedMaterialSet	= null; //已選擇強化素材相關集合
	public UILabel	lbSelected					= null; //已選材料字樣
	//
	public UILabel	lbMoneyValue				= null; //強化所需費用
	[HideInInspector]public int		iEnhanceCost 	=0;		//紀錄強化所需費用
	//
	public UIButton[]	btnSelectedMaterialSet	= new UIButton[5];
	public UISprite[] 	spriteMatPetIconSet		= new UISprite[5];
	public UILabel[]	lbStarRankNumSet		= new UILabel[5];
	//-----------------------------------------------------------------------
	//CanUseMaterialsBG
	public UIPanel	panelMaterialList			= null;	//備選材料列表
	public UILabel	lbPrepareList				= null; //備選材料字樣
	public ColPopulator scrollCanSellPetList 	= null; //scroll列中可用素材
	//-----------------------------------------------------------------------
	public UIButton btnSellMatPet				= null; //出售按鈕
	public UIButton btnAutoPutMaterial			= null; //自動放出售素材
	public UISprite	spriteLeftPicture			= null; //左邊的示意圖
	public UILabel	lbSellMatPet				= null; //出售字樣
	public UILabel	lbAutoPutMaterial			= null; //自動放字樣

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_ForSalePetMaterial";
	//-----------------------------------------------------------------------------------------------------
	private UI_ForSalePetMaterial() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
// 	//-----------------------------------------------------------------------------------------------------
// 	public override void Show()
// 	{
// 		base.Show();
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	public override void Hide()
// 	{
// 		base.Hide();
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void Start()
// 	{
// 		lbSelected.text 		= GameDataDB.GetString(1502); 	//"待出售素材"
// 		lbPrepareList.text 		= GameDataDB.GetString(1503);	//"可出售素材"
// 		lbSellMatPet.text 		= GameDataDB.GetString(1504);	//"出 售"
// 		lbAutoPutMaterial.text 	= GameDataDB.GetString(1407);	//"自動選擇"
// 		lbMoneyValue.text = "0";
// 		InitSelectedMaterialList();
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	//出售時暫停所有按鈕類功能
// 	public void SwitchBtnFunDuringAuto(bool bswitch)
// 	{
// 		btnSellMatPet.isEnabled = bswitch;
// 		btnAutoPutMaterial.isEnabled = bswitch;
// 		for(int i=0;i<btnSelectedMaterialSet.Length;++i)
// 			btnSelectedMaterialSet[i].isEnabled=bswitch;
// 		uiTopStateView.btnQuit.isEnabled = bswitch;
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//(已選擇的材料按鈕UI事件)到state再指派
// 	public void SetbtnSelectedMaterial(GameObject gb)
// 	{
// 		for(int i=0;i<spriteMatPetIconSet.Length;++i)
// 		{
// 			if(gb==btnSelectedMaterialSet[i].gameObject)
// 			{
// 				if(spriteMatPetIconSet[i].gameObject.activeSelf == true)
// 					MusicControlSystem.PlaySound("Sound_System_002",1);
// 				spriteMatPetIconSet[i].gameObject.SetActive(false);
// 			}
// 		}
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	public void InitSelectedMaterialList()
// 	{
// 		for(int i=0;i<spriteMatPetIconSet.Length;++i)
// 			spriteMatPetIconSet[i].gameObject.SetActive(false);
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
}
