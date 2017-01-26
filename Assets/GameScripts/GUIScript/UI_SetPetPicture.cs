using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

class UI_SetPetPicture : NGUIChildGUI 
{
	//Left Side Button
	public UIButton					btnPetPicture						= null;	//圖鑑
	public UILabel					lbPetPicture						= null; //圖鑑字樣
	public UISprite					spriteOptionMark					= null;	//圖鑑通知提示
	public UIButton					btnPetLink							= null; //關係
	public UILabel					lbPetLink							= null; //關係字樣
	//Right Side Filter
	public UIButton					btnPetTypeAll						= null; //全部顯示
	public UIButton					btnPetTypeAttack					= null; //攻擊篩選
	public UIButton					btnPetTypeDefend					= null; //防禦篩選
	public UIButton					btnPetTypeAssist					= null; //輔助篩選
	public UIButton					btnPetType1							= null; //職業1
	public UIButton					btnPetType2							= null; //職業2
	public UIButton					btnPetType3							= null; //職業3
	public UIButton					btnPetType4							= null; //職業4
	public UIButton					btnPetType5							= null; //職業5
	//
	public SetPetPicPopulator		scrollPetList						= null; //解鎖/突破寵物列
	public UIPanel					panelPetList						= null; //解鎖/突破寵物列
	//
	public GameObject				Page1_PetPicture					= null;
	//
	public UITexture				BlackBG								= null;
	//
	public UILabel					lbPetTypeAll						= null;
	public UILabel					lbPetTypeATK						= null;
	public UILabel					lbPetTypeDEF						= null;
	public UILabel					lbPetTypeASIS						= null;
	public UILabel					lbPetType1							= null;
	public UILabel					lbPetType2							= null;
	public UILabel					lbPetType3							= null;
	public UILabel					lbPetType4							= null;
	public UILabel					lbPetType5							= null;
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel					panelGuide							= null; //教學集合
	public UIButton					btnTopFullScreen					= null;	//最上層的全螢幕按鈕
	public UIButton					btnFullScreen						= null; //全螢幕按鈕
	public UILabel					lbGuideIntroduce					= null; //介紹圖鑑功能
	public UISprite					spGuideUnlock						= null; //導引解鎖按鈕
	public UILabel					lbGuideUnlock						= null;
	public UISprite					spGuidePetLink						= null; //導引寵物關係
	public UILabel					lbGuidePetLink						= null;
	public UILabel					lbGuideFinish						= null;
//	public UISprite					spGuideQuit							= null;	//導引離開UI
//	public UILabel					lbGuideQuit							= null;
//	public UIButton					btnFakeQuit							= null;	//新手教學專用離開按鈕
	//-----------------------------------------------------------------------
	[HideInInspector]
	public UI_RolesDetailInfo		uiRolesDetailInfo					= null;		//寵物詳細資訊框
	
	// smartObjectName
	private const string 			GUI_SMARTOBJECT_NAME 				= "UI_SetPetPicture";
	//-----------------------------------------------------------------------------------------------------
	private UI_SetPetPicture() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	
		if(uiRolesDetailInfo != null)
			uiRolesDetailInfo.Show();

		uiRolesDetailInfo.panelBase.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		if(uiRolesDetailInfo!=null)
			uiRolesDetailInfo.Hide();

		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		lbPetPicture.text 	= GameDataDB.GetString(2603);		//"圖鑑"
		lbPetLink.text		= GameDataDB.GetString(2604);		//"關係"
		lbPetTypeAll.text	= GameDataDB.GetString(1181);		//"全部"
		lbPetTypeATK.text	= GameDataDB.GetString(1182);		//"司戰"
		lbPetTypeDEF.text	= GameDataDB.GetString(1184);		//"持援"
		lbPetTypeASIS.text  = GameDataDB.GetString(1183);		//"利兵"
		lbPetType1.text		= GameDataDB.GetString(1185);		//"堅盾"
		lbPetType2.text		= GameDataDB.GetString(1186);		//"勅令"
		lbPetType3.text		= GameDataDB.GetString(1187);		//"懸壺"
		lbPetType4.text		= GameDataDB.GetString(1188);		//"伐謀"
		lbPetType5.text		= GameDataDB.GetString(1189);		//"蕩決"
	}
	//-----------------------------------------------------------------------------------------------------
}
