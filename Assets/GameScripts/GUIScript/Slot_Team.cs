using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Team : NGUIChildGUI  
{
	public	UIWidget		slotTeam			= null;
	
	public	UISprite		SpriteRoleIcon		= null;		//角色頭像
	public	UISprite		SpriteRoleFrame		= null;		//角色頭像外框
	public	UILabel			LabelLevel			= null;		//等級數值

	public	UILabel			LabelRoleName		= null;		//角色名稱
	public	UILabel			LabelType			= null;		//好友

	public  List<UISprite>	SpritePet			= new List<UISprite>();	//寵物頭像
	
	public	UILabel			LabelPower			= null;		//戰力數值
	
	public	UISprite		SpriteCost			= null;		//金錢圖示
	public	UILabel			LabelCost			= null;		//金錢
	public	UILabel			LabelCostTitle		= null;		//金錢標題

	public	UILabel			LabelSelect			= null;		//出戰/未出戰
	public	UIButton		ButtonSelect		= null;		//選擇按鈕
	public	UISprite		SpriteSelect		= null;		//選擇按鈕

//	int index = -1;
	Color	spriteColor	= new Color();
	public	Color			changeColor			= new Color();

	S_PetData_Tmp petDBF = null;
//	public string	slotName 		= "Slot_Friends";

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Team";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_Team() : base(GUI_SMARTOBJECT_NAME)
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
		LabelType.text 		= GameDataDB.GetString(5411);	//好友
		LabelCostTitle.text = GameDataDB.GetString(5409);	//傭金費
		LabelCost.text 		= "0";
		LabelSelect.text 	= GameDataDB.GetString(5413); 	//未出戰

		spriteColor = SpriteSelect.color;
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(SimpleTeammateData data)
	{
		if(data == null)
		{
			slotTeam.gameObject.SetActive(false);
			return ;
		}
		else
		{
			slotTeam.gameObject.SetActive(true);
		}

		//角色頭像
		Utility.ChangeAtlasSprite(SpriteRoleIcon, data.simpleData.m_iFace);
		Utility.ChangeAtlasSprite(SpriteRoleFrame, data.simpleData.m_iFaceFrameID);
		
		//寵物頭像
		petDBF = GameDataDB.PetDB.GetData(data.simpleData.m_BattlePetID_0);
		if(petDBF != null)
		{
			Utility.ChangeAtlasSprite(SpritePet[0], petDBF.AvatarIcon);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpritePet[0], -1);
		}
		petDBF = GameDataDB.PetDB.GetData(data.simpleData.m_BattlePetID_1);
		if(petDBF != null)
		{
			Utility.ChangeAtlasSprite(SpritePet[1], petDBF.AvatarIcon);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpritePet[1], -1);
		}

		//角色名稱
		LabelRoleName.text  = data.simpleData.m_strRoleName;		
		//等級數值
		LabelLevel.text		= data.simpleData.m_iLevel.ToString();
		//戰力數值
		LabelPower.text		= data.simpleData.m_iPower.ToString();	

		//好友
		if(ARPGApplication.instance.m_FriendSystem.CheckFriendship(data.iRoleID))
		{
			LabelType.text = GameDataDB.GetString(5411);	//好友
			LabelType.gameObject.SetActive(true);
		}
		else
		{
			LabelType.text = "";
			LabelType.gameObject.SetActive(false);
		}
#if UNITY_EDITOR_WIN
		if(data.type == ENUM_ActivityData_Type.ENUM_ActivityData_BOT)
		{
			LabelType.text = "BOT";
			LabelType.gameObject.SetActive(true);
		}

#endif
		//需求金錢
		LabelCost.text = ARPGApplication.instance.m_FriendSystem.GetTeammateCost((int)ButtonSelect.userData).ToString();
		//選擇按鈕
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetSelectMark(bool val)
	{
		if(val)
		{
			LabelSelect.text = GameDataDB.GetString(5412);	//出戰
		}
		else
		{
			LabelSelect.text = GameDataDB.GetString(5413); 	//未出戰
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void ChangeColor(bool val)
	{
		if(val)
		{
			//原色
			SpriteSelect.color = spriteColor;
		}
		else
		{
			//灰階變化
//			SpriteSelect.color = new Color((float)(130f/255f), (float)(130f/255f), (float)(130f/255f));
			SpriteSelect.color = changeColor;
		}

	}

	//-------------------------------------------------------------------------------------------------
}
