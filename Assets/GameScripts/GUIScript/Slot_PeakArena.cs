using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_PeakArena : NGUIChildGUI 
{
	public	UIWidget		PeakArena			= null;
	[HideInInspector]
	public	int				index				= 0;
	//----------------------------------------------------------------------

	public	UISprite		SpriteRoleIcon		= null;		//角色頭像
	public	UISprite		SpriteFaceFrame		= null;		//

	public  List<UISprite>	SpritePet			= new List<UISprite>();	//寵物頭像
	public	UILabel			LabelRoleName		= null;		//角色名稱

	public	UILabel			LabelLevelTitle		= null;		//等級標題
	public	UILabel			LabelLevel			= null;		//等級數值

	public	UILabel			LabelRankTitle		= null;		//排名標題
	public	UILabel			LabelRank			= null;		//排名數值

//	public	UILabel			LabelPowerTitle		= null;		//戰力標題
	public	UILabel			LabelPower			= null;		//戰力數值
	
	public	UIWidget		WidgetBattle		= null;		//挑戰
	public	UIButton		ButtonBattle		= null;		//挑戰按鈕
	public	UILabel			LabelBattle			= null;		//挑戰
	public	UISprite		SpriteBattle2		= null;		//挑戰按鈕底圖
	[Header("PetButton")]
	public UIButton			btnPet1				= null;		//寵物1按鈕
	public UIButton			btnPet2				= null;		//寵物2按鈕

	Color	BtnColor 		= new Color();	//按鈕顏色
	S_PetData_Tmp petDBF 	= null;


	[System.NonSerialized]
	public string	slotName 		= "Slot_PeakArena";
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_PeakArena";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_PeakArena() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialSlot();
	}
	
	//-------------------------------------------------------------------------------------------------
	public void InitialSlot()
	{
		LabelLevelTitle.text	= GameDataDB.GetString(5201);	//等級
		LabelRankTitle.text		= GameDataDB.GetString(5300);	//排名
		LabelBattle.text		= GameDataDB.GetString(5301);	//挑戰

		LabelLevel.text			= "99";
		LabelPower.text			= "999999999";
		LabelRoleName.text		= "";
		LabelRank.text			= "99999";

		BtnColor = SpriteBattle2.color;
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_DataPVPRank data)
	{
		if(data == null)
		{
			PeakArena.gameObject.SetActive(false);
			return ;
		}
		else
		{
			PeakArena.gameObject.SetActive(true);
		}
//		index = value;
		
		//角色頭像
		Utility.ChangeAtlasSprite(SpriteRoleIcon, data.sRankData.iFace);
		Utility.ChangeAtlasSprite(SpriteFaceFrame, data.sRankData.iFaceFrameID);

		//寵物頭像
		petDBF = GameDataDB.PetDB.GetData(data.sRankData.iPetDBID1);
		if(petDBF != null)
		{
			Utility.ChangeAtlasSprite(SpritePet[0], petDBF.AvatarIcon);
			btnPet1.userData = data.sRankData.iPetDBID1;
		}
		else
		{
			Utility.ChangeAtlasSprite(SpritePet[0], -1);
			btnPet1.userData = 0;
		}
		petDBF = GameDataDB.PetDB.GetData(data.sRankData.iPetDBID2);
		if(petDBF != null)
		{
			Utility.ChangeAtlasSprite(SpritePet[1], petDBF.AvatarIcon);
			btnPet2.userData = data.sRankData.iPetDBID2;
		}
		else
		{
			Utility.ChangeAtlasSprite(SpritePet[1], -1);
			btnPet2.userData = 0;
		}
		//角色名稱
		LabelRoleName.text  = data.sRankData.strRoleName;		
		//等級數值
		LabelLevel.text		= data.sRankData.iLv.ToString();
		//戰力數值
		LabelPower.text		= data.sRankData.iPower.ToString();		
		//排名數值
		LabelRank.text		= (3000-data.sRankData.iPoint+1).ToString();
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlotButtonColor(bool val)
	{
		if(val)
		{
			SpriteBattle2.color = BtnColor;
		}
		else
		{
			//灰階變化
			SpriteBattle2.color = new Color(0.0f, SpriteBattle2.color.g, SpriteBattle2.color.b);
		}

	}
}
