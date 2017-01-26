using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ValuePVP_Opponent : NGUIChildGUI 
{
	//	玩家資訊
	public UISprite			SpriteBG 					= null;	//slot底版
	public UILabel 			LabelName					= null;	//對手名稱
	public Slot_RoleIcon	SpriteIcon 					= null;	//對手頭像
	public UISprite			SpriteIconPet1 				= null;	//對手夥伴1頭像
	public UISprite			SpriteIconPet2 				= null;	//對手夥伴2頭像

	public UILabel			LabelLVTitle				= null;	//對手等級
	public UILabel			LabelLV						= null;	//對手等級
//	public UISprite 		SpriteValue					= null; //對手戰力(圖)
	public UILabel 			LabelPowerValue				= null; //對手戰力
//	public UILabel 			LabelRankTitle				= null;	//對手排名
//	public UILabel 			LabelRank					= null;	//對手排名
//	public UISprite			SpriteValue					= null;	//戰力或積分數值圖
//	public UILabel			LabelResult					= null;	//戰鬥結果
	public UISprite			SpritelWin					= null;	//戰鬥結果 -贏
	public UISprite			SpritelLose					= null;	//戰鬥結果 -輸

	public UIButton			ButtonBattle				= null;	//戰鬥按鈕
//	public UISprite			SpriteBattle				= null;	//戰鬥按鈕底圖
	public UILabel 			LabelBattle					= null;	//戰鬥按鈕LABEL

	public UISprite			SpriteCover					= null;	//遮罩
	//
	public UIButton			btnPet1						= null; //寵物1
	public UIButton			btnPet2						= null; //寵物2

	private UI_AffectRelation	m_uiAffectRelation		= null;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_ValuePVP_Opponent";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_ValuePVP_Opponent() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialSlot(EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PVP);
	}
	
	//-------------------------------------------------------------------------------------------------
	public void InitialSlot(EMUM_ACTIVITY_TYPE pvpType)
	{
		SetUILabel(pvpType);
		Clear();
		//載入寵物屬性相關事件
		LoadAffectRelationEvent();
	}
	//-------------------------------------------------------------------------------------------------
	private void LoadAffectRelationEvent()
	{
		UIEventListener.Get(btnPet1.gameObject).onClick	+= DisplayAffectRelationUI;
		UIEventListener.Get(btnPet2.gameObject).onClick	+= DisplayAffectRelationUI;
	}
	//-------------------------------------------------------------------------------------------------
	private void RemoveAffectRelationEvent()
	{
		UIEventListener.Get(btnPet1.gameObject).onClick	-= DisplayAffectRelationUI;
		UIEventListener.Get(btnPet2.gameObject).onClick	-= DisplayAffectRelationUI;
	}
	//-------------------------------------------------------------------------------------------------
	void SetUILabel(EMUM_ACTIVITY_TYPE pvpType)
	{
		//對手等級
		LabelLVTitle.text	 = GameDataDB.GetString(1551); //等級	
		//對手戰力
//		LabelPowerTitle.text = GameDataDB.GetString(1552); //戰力
		//對手排名
//		LabelRankTitle.text	 = GameDataDB.GetString(1554); //排名
		//戰鬥按鈕LABEL
		if (pvpType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_PVP)
			LabelBattle.text	 = GameDataDB.GetString(1577); //挑戰
		else if (pvpType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_3VS3)
			LabelBattle.text	 = GameDataDB.GetString(1600);
	}

	//-------------------------------------------------------------------------------------------------
	public void Clear()
	{
		//slot底版
		SpriteBG.gameObject.SetActive(true);	
		//選取圖示
//		SpriteSelectMark.gameObject.SetActive(false);
		//對手頭像
//		SpriteIcon 	
		//對手等級
		LabelLV.text			= string.Format("{0}", "-----");	
		//對手名稱
		LabelName.text			= string.Format("{0}", "-----");	
		//對手戰力
		LabelPowerValue.text	= string.Format("{0}", "-----"); 
		//對手排名
//		LabelRank.text			= string.Format("{0}", "-----");
		//關閉遮罩
		SpriteCover.gameObject.SetActive(false);
		//戰鬥結果
		SpritelWin.gameObject.SetActive(false);
		SpritelLose.gameObject.SetActive(false);
		ButtonBattle.gameObject.SetActive(true);
		//
		btnPet1.userData = -1;
		btnPet2.userData = -1;
		RemoveAffectRelationEvent();
	}

	//-------------------------------------------------------------------------------------------------
//	public void ClearSelectMark()
//	{
//		SpriteSelectMark.gameObject.SetActive(false);
//	}

	//-------------------------------------------------------------------------------------------------
//	public void SetSeletMark()
//	{
//		SpriteSelectMark.gameObject.SetActive(true);
//	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlotValue(S_DataPVPRank data)
	{
		UnityDebugger.Debugger.Log(data.sRankData.emType);

		int test = (int)data.sRankData.emType;
		//slot底版
		SpriteBG.gameObject.SetActive(true);

		//對手頭像
        SpriteIcon.SetSlot(data.sRankData.iFace,data.sRankData.iFaceFrameID);

		//夥伴1頭像
		S_PetData_Tmp idbf = GameDataDB.PetDB.GetData(data.sRankData.iPetDBID1);
		if(idbf != null)
		{
			Utility.ChangeAtlasSprite(SpriteIconPet1, idbf.AvatarIcon);
			btnPet1.userData = data.sRankData.iPetDBID1;
		}
		idbf = null;
		//夥伴2頭像
		idbf = GameDataDB.PetDB.GetData(data.sRankData.iPetDBID2);
		if(idbf != null)
		{
			Utility.ChangeAtlasSprite(SpriteIconPet2, idbf.AvatarIcon);
			btnPet2.userData = data.sRankData.iPetDBID2;
		}

		//對手等級
		LabelLV.text			= string.Format("{0}", data.sRankData.iLv);	
		//對手名稱
		LabelName.text			= string.Format("{0}", data.sRankData.strRoleName);	
		//對手戰力
		LabelPowerValue.text	= string.Format("{0}", data.sRankData.iPower); 

		//打過沒+勝敗
		switch(data.emBattle)
		{
		case ENUM_Battle_Type.ENUM_Battle_Notyet:
			SpriteCover.gameObject.SetActive(false);
			SpritelWin.gameObject.SetActive(false);
			SpritelLose.gameObject.SetActive(false);
			ButtonBattle.gameObject.SetActive(true);
			break;
		case ENUM_Battle_Type.ENUM_Battle_Win:
			SpriteCover.gameObject.SetActive(true);
			SpritelWin.gameObject.SetActive(true);
			SpritelLose.gameObject.SetActive(false);
			ButtonBattle.gameObject.SetActive(false);
			break;
		case ENUM_Battle_Type.ENUM_Battle_Lose:
			SpriteCover.gameObject.SetActive(true);
			SpritelWin.gameObject.SetActive(false);
			SpritelLose.gameObject.SetActive(true);
			ButtonBattle.gameObject.SetActive(false);
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void DisplayAffectRelationUI(GameObject gb)
	{
		UIButton btn = gb.GetComponent<UIButton>();
		if(btn == null)
			return;

		int PetGUID = (int)btn.userData;
		if(PetGUID <=0)
			return;

		m_uiAffectRelation = ARPGApplication.instance.guiManager.GetGUI(typeof(UI_AffectRelation).Name) as UI_AffectRelation;
		if(m_uiAffectRelation == null)
			return;

		m_uiAffectRelation.SetAffectInfo(PetGUID);
	}
	//-------------------------------------------------------------------------------------------------
	//設定結果
/*	private void SetOpponentResult(EMUM_COMPETITOR_RESULT eResult)
	{
		switch(eResult)
		{
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_NONE:
			SpriteCover.gameObject.SetActive(false);
			SpritelWin.gameObject.SetActive(false);
			SpritelLose.gameObject.SetActive(false);
			break;
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_WIN:
			SpriteCover.gameObject.SetActive(true);
			SpritelWin.gameObject.SetActive(true);
			SpritelLose.gameObject.SetActive(false);
			break;
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_LOSE:
			SpriteCover.gameObject.SetActive(true);
			SpritelWin.gameObject.SetActive(false);
			SpritelLose.gameObject.SetActive(true);
			break;
		case EMUM_COMPETITOR_RESULT.EMUM_COMPETITOR_RESULT_MAX:
		default:
			UnityDebugger.Debugger.LogError("***Opponent's battle Result is empty");
			break;
		}
	}
*/
}
