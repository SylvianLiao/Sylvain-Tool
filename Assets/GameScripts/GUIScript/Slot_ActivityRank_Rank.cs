using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ActivityRank_Rank : NGUIChildGUI  
{
	public Slot_RoleIcon	SpriteIcon			= null; //
	public UILabel			LabelName			= null; //

	public UILabel			LabelLVTitle		= null; //
	public UILabel			LabelLV				= null; //

	public UILabel			LabelPowerValue		= null; //

	public UILabel			LabelRankTitle		= null; //
	public UILabel			LabelRank			= null; //
	public UISprite			SpriteRank			= null; //

	public UILabel			LabelPointTitle		= null; //
	public UILabel			LabelPoint			= null; //

	public UISprite			SpriteIconPet1		= null; //
	public UISprite			SpriteIconPet2		= null; //

	public UISprite			SpriteSelfMark		= null; //標記自己用邊框

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_ActivityRank_Rank";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_ActivityRank_Rank() : base(GUI_SMARTOBJECT_NAME)
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
		LabelName.text = "";
		LabelLV.text = "";
		LabelPowerValue.text = "";
		LabelRank.text = "";
		LabelPoint.text = "";
		SpriteIconPet1.gameObject.SetActive(true);
		SpriteIconPet2.gameObject.SetActive(true);
		SpriteSelfMark.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_ActivityRankData data, int index)
	{
		//	
        SpriteIcon.SetSlot(data.iFace,data.iFaceFrameID);
		//
		LabelName.text = data.strRoleName;
		//
		LabelLV.text = data.iLv.ToString();
		//
		LabelPowerValue.text = data.iPower.ToString();
		//排名
		LabelRank.text = (index+1).ToString();

		if(index == 0)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 300);
		}
		else if(index == 1)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 301);
		}
		else if(index == 2)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 302);
		}
		else if(index <= 9 && index >= 3)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 303);
		}
		else if(index <=99 && index >= 10)
		{
			Utility.ChangeAtlasSprite(SpriteRank, 304);
		}

		//積分設定
		if(ARPGApplication.instance.m_ActivityMgrSystem.GetSelectActivityType() == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PeakPVP)
		{
			LabelPointTitle.gameObject.SetActive(false);
		}
		else
		{
			LabelPointTitle.gameObject.SetActive(true);
			LabelPoint.text = data.iPoint.ToString();
		}
		
		//夥伴1頭像
		S_PetData_Tmp idbf = GameDataDB.PetDB.GetData(data.iPetDBID1);
		if(idbf != null)
		{
			Utility.ChangeAtlasSprite(SpriteIconPet1, idbf.AvatarIcon);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteIconPet1, -1);
		}
		//夥伴2頭像
		idbf = null;
		idbf = GameDataDB.PetDB.GetData(data.iPetDBID2);
		if(idbf != null)
		{
			Utility.ChangeAtlasSprite(SpriteIconPet2, idbf.AvatarIcon);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteIconPet2, -1);
		}

		//
		if(ARPGApplication.instance.m_RoleSystem.m_RoleGUID == data.iRoleID)
		{
			SpriteSelfMark.gameObject.SetActive(true);
		}
		else
		{
			SpriteSelfMark.gameObject.SetActive(false);
		}
	}
}
