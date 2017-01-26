using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_AffectRoleIcon : NGUIChildGUI   
{
	public UIWidget			WidgetSlot			= null;
	public UIButton			btnSlot				= null;
	public UISprite			spriteBG			= null; 
	public UISprite			spriteRoleIcon		= null; 
	public UISprite			spriteFrame			= null;
	public UILabel			lbPercentNum		= null;
	public UILabel			lbRoleName			= null;

	private const string 	GUI_SMARTOBJECT_NAME = "Slot_AffectRoleIcon";

	//-------------------------------------------------------------------------------------------------
	private Slot_AffectRoleIcon() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
	}
	//-------------------------------------------------------------------------------------------------
	//設定寵物資料
	public void SetPetData(S_PetData_Tmp pdTmp,S_MobData_Tmp EnemyTmp,S_PetData_Tmp EnemyPetTmp,bool bshowValue = false,bool bshowName = false)
	{
		if(pdTmp == null)
		{
			WidgetSlot.gameObject.SetActive(false);
			return;
		}
		//設定圖像
		Utility.ChangeAtlasSprite(spriteRoleIcon,pdTmp.AvatarIcon);
		//設定灰階
		S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(pdTmp.GUID);
		bool isHave = false;
		if(pd !=null && pd.iPetLevel>0)
			isHave = true;
		spriteRoleIcon.color = new Color(isHave?1.0f:0.0f,1.0f,1.0f);
		//設定品階框
		pdTmp.SetPetRarity(spriteFrame,spriteBG);

		//設定名稱
		lbRoleName.text = GameDataDB.GetString(pdTmp.iName);
		pdTmp.SetRareColorString(lbRoleName);
		lbRoleName.gameObject.SetActive(bshowName);
		//設定傷害數值
		float TotalEffectValue =0;
		if(EnemyTmp!=null)
			TotalEffectValue = pdTmp.fAffectCharClass_Per + GameDataDB.GetCharacterTypeValueToMob(pdTmp.GUID,EnemyTmp.GUID);
		if(EnemyPetTmp!=null)
			TotalEffectValue = pdTmp.fAffectCharClass_Per + GameDataDB.GetCharacterTypeValueToPet(pdTmp.GUID,EnemyPetTmp.GUID);
		 
		lbPercentNum.text = ((int)(TotalEffectValue*100)).ToString()+"%";
		lbPercentNum.gameObject.SetActive(bshowValue);
	}
	//-------------------------------------------------------------------------------------------------
}
