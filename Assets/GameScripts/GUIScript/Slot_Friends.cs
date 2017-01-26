using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Friends : NGUIChildGUI 
{
	public	UIWidget		slotFriends			= null;
	public	int				index				= 0;
	//----------------------------------------------------------------------
	public	UISprite		SpriteState			= null;		//登入狀態
	public	UISprite		SpriteRoleIcon		= null;		//角色頭像
	public	UISprite		SpriteRoleFrame		= null;		//角色頭像外框
	public	UIButton		ButtonRoleIcon		= null;		//角色按鈕
	public  List<UISprite>	SpritePet			= new List<UISprite>();	//寵物頭像
	public	UILabel			LabelRoleName		= null;		//角色名稱
	public	UILabel			LabelLastLoginTime	= null;		//上次登入時間
	public	UILabel			LabelLevelTitle		= null;		//等級標題
	public	UILabel			LabelLevel			= null;		//等級數值
	public	UILabel			LabelPowerTitle		= null;		//戰力標題
	public	UILabel			LabelPower			= null;		//戰力數值
	//----------------------------------------------------------------------
	public	UIWidget		WidgetSendAndReceive	= null;	//收禮送禮
	public	UILabel			LabelReceive		= null;		//收禮
	public	UILabel			LabelSend			= null;		//送禮
	public	UIButton		ButtonReceive		= null;		//收禮按鈕
	public	UIButton		ButtonSend			= null;		//送禮按鈕
	//----------------------------------------------------------------------
	public	UIWidget		WidgetAcceptAndRefuse	= null;	//接受拒絕
	public	UILabel			LabelAccept			= null;		//接受
	public	UILabel			LabelRefuse			= null;		//拒絕
	public	UIButton		ButtonAccept		= null;		//接受按鈕
	public	UIButton		ButtonRefuse		= null;		//拒絕按鈕
	//----------------------------------------------------------------------
	public	UIWidget		WidgetInvite		= null;		//邀請
	public	UILabel			LabelInvite			= null;		//邀請
	public	UIButton		ButtonInvite		= null;		//邀請按鈕

	S_PetData_Tmp petDBF = null;

	public string	slotName 		= "Slot_Friends";
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Friends";

	//-------------------------------------------------------------------------------------------------
	private Slot_Friends() : base(GUI_SMARTOBJECT_NAME)
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
		LabelLevelTitle.text		= GameDataDB.GetString(5201);	//等級
		LabelPowerTitle.text		= GameDataDB.GetString(5202);	//戰力

		LabelReceive.text 			= GameDataDB.GetString(5203);		//收禮 
		LabelSend.text 				= GameDataDB.GetString(5204);		//送禮

		LabelAccept.text 			= GameDataDB.GetString(5205);		//接受
		LabelRefuse.text 			= GameDataDB.GetString(5206);		//拒絕

		LabelInvite.text			= GameDataDB.GetString(5207);		//邀請
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot(SimpleFriendData data, UI_FRIENDS_PAGE type, int value)
	{
		index = value;

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

		switch(type)
		{
		case UI_FRIENDS_PAGE.MyFriend:
			WidgetSendAndReceive.gameObject.SetActive(true);
			WidgetAcceptAndRefuse.gameObject.SetActive(false);
			WidgetInvite.gameObject.SetActive(false);
			//登入狀態
			SetOnlineTime(data);
			//收禮狀態
			if(data.baseFriendData.emGetFriendGift == ENUM_FriendGiftState.ENUM_FriendGiftState_HaveGift)
			{
				LabelReceive.gameObject.SetActive(true);
				ButtonReceive.isEnabled = true;
				LabelReceive.text 	= GameDataDB.GetString(5203);		//收禮 
			}
			else if(data.baseFriendData.emGetFriendGift == ENUM_FriendGiftState.ENUM_FriendGiftState_GotIt)
			{
				LabelReceive.gameObject.SetActive(true);
				ButtonReceive.isEnabled = false;
				LabelReceive.text 	= GameDataDB.GetString(5237);		//已收禮 
			}
			else
			{
				LabelReceive.gameObject.SetActive(false);
			}

			//送禮狀態
			if(data.baseFriendData.emGiveFreindGift == ENUM_GiveFreindGift.ENUM_GiveFreindGift_Clear)
			{
				LabelSend.gameObject.SetActive(true);
				ButtonSend.isEnabled = true;
				LabelSend.text	= GameDataDB.GetString(5204);		//送禮
			}
			else if(data.baseFriendData.emGiveFreindGift == ENUM_GiveFreindGift.ENUM_GiveFreindGift_Gave)
			{
				LabelSend.gameObject.SetActive(true);
				ButtonSend.isEnabled =false;
				LabelSend.text	= GameDataDB.GetString(5208);		//已送禮

			}
			else
			{
				LabelSend.gameObject.SetActive(false);
			}
			break;
		case UI_FRIENDS_PAGE.Invite:
			WidgetSendAndReceive.gameObject.SetActive(false);
			WidgetAcceptAndRefuse.gameObject.SetActive(true);
			WidgetInvite.gameObject.SetActive(false);

			Utility.ChangeAtlasSprite(SpriteState, 106);	//離線圖
			LabelLastLoginTime.gameObject.SetActive(false);
			break;
		case UI_FRIENDS_PAGE.Recommend:
			WidgetSendAndReceive.gameObject.SetActive(false);
			WidgetAcceptAndRefuse.gameObject.SetActive(false);
			WidgetInvite.gameObject.SetActive(true);

			Utility.ChangeAtlasSprite(SpriteState, 106);	//離線圖
			LabelLastLoginTime.gameObject.SetActive(false);
			break;
		}

	}

	//-------------------------------------------------------------------------------------------------
	public void SetOnlineTime(SimpleFriendData data)
	{
		//登入狀態
		if(data.baseFriendData.isOnline)
		{
			Utility.ChangeAtlasSprite(SpriteState, 105);	//上線圖
			LabelLastLoginTime.gameObject.SetActive(false);
		}
		else
		{
			Utility.ChangeAtlasSprite(SpriteState, 106);	//離線圖
			LabelLastLoginTime.gameObject.SetActive(true);

//			DateTime loginTime = DateTime.Parse(data.simpleData.m_LoginTime);

			DateTime loginTime;
			if(DateTime.TryParse(data.simpleData.m_LoginTime, out loginTime))
			{
				loginTime = DateTime.Parse(data.simpleData.m_LoginTime);
			}
			else
			{
				loginTime = DateTime.UtcNow;
				UnityDebugger.Debugger.Log(string.Format("DateTime.TryParse error! ID:{0} time:{1}"
				                           , data.baseFriendData.iTargetID
				                           , data.simpleData.m_LoginTime));
			}

			TimeSpan ts = DateTime.UtcNow - loginTime.ToUniversalTime();
			
			double hours = ts.TotalHours;
			if(hours >= 24)
			{
				LabelLastLoginTime.text = string.Format(GameDataDB.GetString(5209), (int)(hours/24));	//{0}天
			}
			else if(hours > 1 && hours < 24)
			{
				LabelLastLoginTime.text = string.Format(GameDataDB.GetString(5210), (int)hours);	//{0}小時
			}
			else if(hours < 1 && hours > 0)
			{
				LabelLastLoginTime.text = GameDataDB.GetString(5211);	//剛剛
			}
		}
	}
}
