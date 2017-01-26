using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_AffectRelation : NGUIChildGUI 
{

	public BoxCollider 		BGboxCollider			= null;
	public UIButton			btnClose				= null;
	public UILabel			lbTitle					= null;
	[Header("UpSelfInfo")]
	public UISprite			spRoleIcon				= null; //角色Icon
	public UILabel			lbRoleName				= null; //角色名稱
	public UISprite			spCareerTag				= null; //職業標籤
	public UILabel			lbCareerTag				= null; //職業字樣
	public UISprite			spTypeTag				= null; //型態標籤
	[Header("AffectList")]
	public UILabel			lbUp					= null; //優
	public UIScrollView 	svUp					= null;
	public UIGrid			gdUp					= null;
	public UILabel			lbDown					= null; //劣
	public UIScrollView		svDown					= null;
	public UIGrid			gdDown					= null;

	private List<Slot_AffectRoleIcon>	UpAffectSlots 	= new List<Slot_AffectRoleIcon>();
	private List<Slot_AffectRoleIcon>	DownAffectSlots = new List<Slot_AffectRoleIcon>();
	private const int		CareerNameIDBegin		= 1213;
	private const string 	GUI_SMARTOBJECT_NAME 	= "UI_AffectRelation";
	private const string 	Slot_AffectName			= "Slot_AffectRoleIcon";
	private bool			bRePosScrollView		= false;
	//-------------------------------------------------------------------------------------------------
	private UI_AffectRelation() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		CreateAffectIconSlots(ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP);
		CreateAffectIconSlots(ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN);

		UIEventListener.Get(btnClose.gameObject).onClick		+= EndAffectRelationUI;
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
		HideAllSlot(UpAffectSlots);
		HideAllSlot(DownAffectSlots);
	}
	//-------------------------------------------------------------------------------------------------
	private void HideAllSlot(List<Slot_AffectRoleIcon> slotList)
	{
		if(slotList == null)
			return;

		if(slotList.Count<=0)
			return;

		for(int i=0;i<slotList.Count;++i)
		{
			slotList[i].gameObject.SetActive(false);
		}
	}
	//-------------------------------------------------------------------------------------------------
	void Update()
	{
		if(bRePosScrollView)
		{
			svUp.ResetPosition();
			svDown.ResetPosition();
			bRePosScrollView = false;
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定資訊
	public void SetAffectInfo(int GUID,bool isPet = true)
	{
		S_PetData_Tmp pdTmp = null;
		S_MobData_Tmp mbTmp = null;
		
		if(isPet)
		{
			pdTmp = GameDataDB.PetDB.GetData(GUID);
			if(pdTmp == null)
			{
				Hide();
				return;
			}
		}
		else
		{
			mbTmp = GameDataDB.MobDataDB.GetData(GUID);
			if(mbTmp == null)
			{
				Hide();
				return;
			}
		}
		//設定頭像圖
		Utility.ChangeAtlasSprite(spRoleIcon,isPet?pdTmp.AvatarIcon:mbTmp.AvatarIcon);
		//設定名稱
		lbRoleName.text = GameDataDB.GetString(isPet?pdTmp.iName:mbTmp.iName);
		//設定職業名稱
		ENUM_CHARACTER_TYPE cType = isPet?pdTmp.emCharType:mbTmp.emCharType;
		Utility.ChangeAtlasSprite(spCareerTag,2110); //角色類別底圖
		lbCareerTag.text = GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(cType));
		//設定類別
		ENUM_CHARACTER_CALSS cClass = isPet?pdTmp.emCharClass:mbTmp.emCharClass;
		Utility.ChangeAtlasSprite(spTypeTag,ARPGApplication.instance.GetPetCalssIconID(cClass));
		//設定列表
		lbUp.text 	= string.Format(GameDataDB.GetString(954),lbRoleName.text);
		lbDown.text = string.Format(GameDataDB.GetString(955),lbRoleName.text);
		if(isPet && pdTmp != null)
		{
			SetAffectList(pdTmp.DifficultAdversary,ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP,pdTmp);
			SetAffectList(pdTmp.EasyAdversary,ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN);
		}
		else if(isPet == false && mbTmp != null)
		{
			SetAffectList(mbTmp.DifficultAdversary,ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP,null,mbTmp);
			SetAffectList(mbTmp.EasyAdversary,ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN);
		}

		Show();
		bRePosScrollView = true;
	}
	//-------------------------------------------------------------------------------------------------
	//設定列表(寵物列表,影響型態)
	private void SetAffectList(List<S_PetData_Tmp> pList, ENUM_AFFECT_TYPE AcType,S_PetData_Tmp pdTmp=null,S_MobData_Tmp MobTmp=null)
	{
		if(pList == null)
			return;
		if(pList.Count <= 0)
			return;

		Slot_AffectRoleIcon SlotIcon = ResourceManager.Instance.GetGUI(Slot_AffectName).GetComponent<Slot_AffectRoleIcon>();
		if(SlotIcon == null)
		{
			UnityDebugger.Debugger.Log( string.Format("Slot_AffectRoleIcon load prefeb error,path:{0}", "GUI/"+Slot_AffectName) );
			return;
		}
		//排序
		pList.Sort((x, y) => { 
			if(x.iRank == y.iRank)
			{
				if(x.fAffectCharClass_Per == y.fAffectCharClass_Per)
					return x.GUID.CompareTo(y.GUID);

				return -x.fAffectCharClass_Per.CompareTo(y.fAffectCharClass_Per);
			}
			return -x.iRank.CompareTo(y.iRank);
		});

		for(int i=0;i<pList.Count;++i)
		{
			switch(AcType)
			{
			case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP:
				UpAffectSlots[i].SetPetData(pList[i],MobTmp,pdTmp,true);
				UpAffectSlots[i].gameObject.SetActive(true);
				break;
			case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN:
				DownAffectSlots[i].SetPetData(pList[i],MobTmp,pdTmp,true);
				DownAffectSlots[i].gameObject.SetActive(true);
				break;
			}
		}
		switch(AcType)
		{
		case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP:
			gdUp.repositionNow = true;
			break;
		case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN:
			gdDown.repositionNow = true;
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateAffectIconSlots(ENUM_AFFECT_TYPE AcType)
	{
		Slot_AffectRoleIcon SlotIcon = ResourceManager.Instance.GetGUI(Slot_AffectName).GetComponent<Slot_AffectRoleIcon>();
		if(SlotIcon == null)
		{
			UnityDebugger.Debugger.Log( string.Format("Slot_AffectRoleIcon load prefeb error,path:{0}", "GUI/"+Slot_AffectName) );
			return;
		}
		//
		switch(AcType)
		{
		case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP:
			UpAffectSlots.Clear();
			break;
		case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN:
			DownAffectSlots.Clear();
			break;
		}
		//
		for(int i=0;i<GameDefine.PET_AFFECTLISTCOUNT_MAX;++i)
		{
			//clone
			Slot_AffectRoleIcon IconSlot = Instantiate(SlotIcon) as Slot_AffectRoleIcon;
			switch(AcType)
			{
			case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP:
				IconSlot.transform.parent				= gdUp.transform;
				IconSlot.lbPercentNum.color = Color.green;
				break;
			case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN:
				IconSlot.transform.parent				= gdDown.transform;
				IconSlot.lbPercentNum.color = Color.red;
				break;
			}
			IconSlot.transform.localScale			= Vector3.one;
			IconSlot.transform.localRotation		= Quaternion.identity;
			IconSlot.transform.localPosition		= Vector3.zero;
			IconSlot.transform.name					= AcType== ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP?"slotUP"+i.ToString():"slotDOWN"+i.ToString();
			IconSlot.gameObject.SetActive(false);
			switch(AcType)
			{
			case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_UP:
				UpAffectSlots.Add(IconSlot);
				break;
			case ENUM_AFFECT_TYPE.ENUM_AFFECT_TYPE_DOWN:
				DownAffectSlots.Add(IconSlot);
				break;
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//關閉事件
	private void EndAffectRelationUI(GameObject gb)
	{
		Hide();
	}
	//-------------------------------------------------------------------------------------------------
}
