using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Friends : NGUIChildGUI  
{
	public UILabel				LabelTital			= null;

	// 頁籤
//	public UIButton				ButtonMyFriend		= null;		//starting state
	public UILabel				LabelMyFriend		= null;

//	public UIButton				ButtonInvite		= null;
	public UILabel				LabelInvite			= null;

//	public UIButton				ButtonRecommend		= null;
	public UILabel				LabelRecommend		= null;

	public List<UIButton>		ButtonPageList			= new List<UIButton>();
	public List<UISprite>		SpriteBGPageList		= new List<UISprite>();
	public List<UISprite>		SpriteMarkPageList		= new List<UISprite>();

	// 好友底部BAR
	public UIWidget				WidgetBottom		= null;
	public UILabel				LabelFriendCount	= null;
	public UILabel				LabelReceiveCount	= null;
	public UILabel				LabelNote			= null;
	public UILabel				LabelNote2			= null;
	public UILabel				LabelAllSend		= null;
	public UILabel				LabelAllReceive		= null;
	public UIButton				ButtonAllSend		= null;
	public UIButton				ButtonAllReceive	= null;

	public UILabel				LabelCommissionTitle	= null;
	public UILabel				LabelCommission			= null;
	public UIButton				ButtonGetCommission		= null;
	public UILabel				LabelGetCommission		= null;
	
	// 好友列表
	public UIPanel				panelScrollView		= null;
	public UIWrapContentEX		WrapFriends			= null;
	public UILabel				LabelEmptyNote		= null;

	// 好友搜尋
	public UIWidget				ContainerSearch		= null;
	public UILabel				LabelSearch			= null;
	public UIInput				InputSearch			= null;
	public UILabel				LabelInput			= null;
	public UIButton				ButtonSearch		= null;
	public UIButton				ButtonSwitchSearch	= null;
	public UISprite				SpriteSwitchSearch	= null;
	public UILabel				LabelSwitchSearch	= null;

	//好友資訊
	public UIPanel				PanelRoleInfo		= null;
	public UIWidget				ContainerRoleInfo	= null;
	public UILabel				LabelRoleInfoTitle	= null;
	public UISprite				SpriteRoleInfoIcon	= null;
	public UISprite				Spriteframe			= null;
	public UILabel				LabelRoleInfoView	= null;
	public UIButton				ButtonRoleInfoView	= null;
	public UILabel				LabelRoleInfoDelete		= null;
	public UIButton				ButtonRoleInfoDelete	= null;
	public UILabel				LabelRoleInfoNameTitle	= null;
	public UILabel				LabelRoleInfoName		= null;
	public UILabel				LabelRoleInfoPowerTitle	= null;
	public UILabel				LabelRoleInfoPower		= null;
	public UILabel				LabelRoleInfoLVTitle	= null;
	public UILabel				LabelRoleInfoLV			= null;
	public UILabel				LabelRoleInfoNumberTitle	= null;
	public UILabel				LabelRoleInfoNumber			= null;
	public UILabel				LabelRoleInfoMsg		= null;
	public UIButton				ButtonRoleInfoMsg		= null;

	public UIButton				ButtonRoleInfoClose		= null;

	//true-Name; false-ID
	public bool 				searchMode 			= true;	
	public int					searchModeNameNum   = -1;
	public int					searchModeIDNum  	= -1;

	//讀取轉圈圈
	public UIWidget				MaskContainer		= null;
	
	//
	string slotName									= "Slot_Friends";
	public List<Slot_Friends>		slotList		= new List<Slot_Friends>();

	//-------------------------------------新手教學用------------------------------------------------
	public UIPanel				panelGuide					= null; 	//導引相關集合
	public UIButton				btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton				btnFullScreen				= null; 	//任意鍵繼續導引
	public UISprite				spGuideClickPageBtn			= null; 	//導引點擊好友分頁按鈕
	public UILabel				lbGuideClickPageBtn			= null;
	public UISprite				spGuidePageInfo				= null;		//導引介紹分頁內容
	public UILabel				lbGuidePageInfo				= null; 
	public UISprite				spGuideSearch				= null;		//導引好友搜尋
	public UILabel				lbGuideSearch				= null; 
	public UILabel				lbGuideFinsih				= null; 	//導引好友教學結束

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME 	= "UI_Friends";
	//-------------------------------------------------------------------------------------------------
	private UI_Friends() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
//		WrapFriends.gameObject.SetActive(false);
//		GameDataDB.GetString(5211)
		LabelTital.text			= "";

		LabelMyFriend.text		= GameDataDB.GetString(5212);	//我的好友
		LabelInvite.text		= GameDataDB.GetString(5213);	//好友邀請
		LabelRecommend.text		= GameDataDB.GetString(5214);	//好友搜尋

		LabelSearch.text        = GameDataDB.GetString(5215);	//輸入ID搜尋
		LabelInput.text			= "";
		LabelSwitchSearch.text	= GameDataDB.GetString(5239);	//搜尋切換

		LabelAllSend.text		= GameDataDB.GetString(5216);	//全部贈禮
		LabelAllReceive.text	= GameDataDB.GetString(5217);	//全部收禮

		LabelNote.text 			= GameDataDB.GetString(5218);	//※點擊好友頭像可進行互動 
		LabelNote2.text 		= GameDataDB.GetString(5233);	//※每日04:00恢復贈禮次數

		LabelCommissionTitle.text	= GameDataDB.GetString(5415);	//傭金財庫
		LabelCommission.text		= string.Format("{0}/{1}", 0, GameDefine.DEF_FRIENDCOMMISSION_BASE);	// 
		LabelGetCommission.text		= GameDataDB.GetString(5416);	//領取財庫

		for(int i=0; i<ButtonPageList.Count; ++i)
		{
			ButtonPageList[i].userData = i;
		}
		
		panelScrollView.alpha = 0.001f;
		CreatSlot();

		//好友資訊
//		ContainerRoleInfo.gameObject.SetActive(false);
		PanelRoleInfo.gameObject.SetActive(false);

		LabelRoleInfoTitle.text			= GameDataDB.GetString(5219);	//角色資訊
		LabelRoleInfoView.text			= GameDataDB.GetString(5220);	//檢視
		LabelRoleInfoDelete.text		= GameDataDB.GetString(5221);	//刪除好友
		LabelRoleInfoNameTitle.text		= GameDataDB.GetString(5222);	//名稱
		LabelRoleInfoPowerTitle.text	= GameDataDB.GetString(5202);	//戰力	
		LabelRoleInfoLVTitle.text		= GameDataDB.GetString(5201);	//等級
		LabelRoleInfoNumberTitle.text	= GameDataDB.GetString(5223);	//ID
		LabelRoleInfoMsg.text			= GameDataDB.GetString(5224);	//密語

		LabelFriendCount.text 	= string.Format(GameDataDB.GetString(5225), 
		                                       	0, 
		                                       	0);		//好友數量 {0}/{1}
		
		LabelReceiveCount.text 	= string.Format(GameDataDB.GetString(5226), 
		                                        0, 
		                                        0);		//收禮次數 {0}/{1}

		searchMode = true;
		SetSearchMode(true);
	}

	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_Friends"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_Friends go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_Friends>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_ActivityMenu load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		// GuildList
		for(int i=0; i< 10 ; ++i) 
		{
			Slot_Friends newgo	= Instantiate(go) as Slot_Friends;
			
			newgo.transform.parent			= WrapFriends.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slot{0:00}",i);
			
			newgo.InitialSlot();
			slotList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
/*	public void SetPage(int index)
	{
		for(int i=0; i<ButtonPageList.Count; ++i)
		{
			if(i == index)
			{
				ButtonPageList[i].gameObject.SetActive(true);
			}
			else
			{
				ButtonPageList[i].gameObject.SetActive(false);
			}
		}
	}
*/
	//-------------------------------------------------------------------------------------------------
	public void SetBottomText()
	{
		LabelFriendCount.text 	= string.Format(GameDataDB.GetString(5225), 
		                                       ARPGApplication.instance.m_FriendSystem.dicFriendData.Count, 
		                                       ARPGApplication.instance.m_RoleSystem.GetMaxFriendsCount());	//好友數量 {0}/{1}

		LabelReceiveCount.text 	= string.Format(GameDataDB.GetString(5226), 
		                                        ARPGApplication.instance.m_RoleSystem.MaxFriendGift, 
		                                        GameDefine.DEF_FRIENDGIFT_MAX);		//收禮次數 {0}/{1}

		//檢查全送按鈕
		if(ARPGApplication.instance.m_FriendSystem.CheckSendGift() > 0)
		{
			ButtonAllSend.isEnabled = true;
		}
		else
		{
			ButtonAllSend.isEnabled = false;
		}

		//檢查全收按鈕
		if(ARPGApplication.instance.m_FriendSystem.CheckReceiveGift() > 0 
		   && ARPGApplication.instance.m_RoleSystem.MaxFriendGift < GameDefine.DEF_FRIENDGIFT_MAX)
		{
			ButtonAllReceive.isEnabled = true;
		}
		else
		{
			ButtonAllReceive.isEnabled = false;
		}

		SetFriendCommission();
	}

	//-------------------------------------------------------------------------------------------------
	//設定傭金財庫金額
	public void SetFriendCommission()
	{
		int money = ARPGApplication.instance.m_RoleSystem.iFriendCommission;

		//傭金財庫 滿綠色普通白色
		if(money >= GameDefine.DEF_FRIENDCOMMISSION_BASE)
		{
//			LabelCommission.text			= string.Format("[FF0000]{0}[-]/[FF0000]{1}[-]", money, GameDefine.DEF_FRIENCommission_Base);
			LabelCommission.text			= string.Format("[00EC00]{0}[-]/[00EC00]{1}[-]", money, GameDefine.DEF_FRIENDCOMMISSION_BASE);
		}
		else
		{
			LabelCommission.text			= string.Format("{0}/{1}", money, GameDefine.DEF_FRIENDCOMMISSION_BASE);
		}

		if(money > 0)
		{
			ButtonGetCommission.isEnabled 	= true;
		}
		else
		{
			ButtonGetCommission.isEnabled 	= false;
		}

	}

	//-------------------------------------------------------------------------------------------------
	public void ShowRoleInfo(SimpleFriendData data)
	{
//		ContainerRoleInfo.gameObject.SetActive(true);
		PanelRoleInfo.gameObject.SetActive(true);

		//換頭像
		Utility.ChangeAtlasSprite(SpriteRoleInfoIcon, data.simpleData.m_iFace);
		//換頭像外框
		Utility.ChangeAtlasSprite(Spriteframe, data.simpleData.m_iFaceFrameID);

		//名稱
		LabelRoleInfoName.text		= data.simpleData.m_strRoleName;
		//戰力
		LabelRoleInfoPower.text		= data.simpleData.m_iPower.ToString();
		//等級
		LabelRoleInfoLV.text		= data.simpleData.m_iLevel.ToString();
		//ID
		LabelRoleInfoNumber.text	= data.baseFriendData.iTargetID.ToString();

	}

	//-------------------------------------------------------------------------------------------------
	public void HideRoleInfo()
	{
		LabelRoleInfoName.text		= "";
		LabelRoleInfoPower.text		= "";
		LabelRoleInfoLV.text		= "";
		LabelRoleInfoNumber.text	= "";

//		ContainerRoleInfo.gameObject.SetActive(false);
		PanelRoleInfo.gameObject.SetActive(false);
	}

	//-------------------------------------------------------------------------------------------------
	public void SwitchSearchMode()
	{
		InputSearch.value = null;

		searchMode = !searchMode;

		SetSearchMode(searchMode);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSearchMode(bool val)
	{
		//true 用名稱搜尋
		if(val)
		{
			LabelEmptyNote.text     = GameDataDB.GetString(5236);	//輸入完整名稱即可搜尋

			LabelSearch.text        = GameDataDB.GetString(5238);	//輸入名稱搜尋
			if(searchModeNameNum >= 0)
			{
				Utility.ChangeAtlasSprite(SpriteSwitchSearch, searchModeNameNum);
			}
		}
		else
		{
			LabelEmptyNote.text     = GameDataDB.GetString(5240);	//輸入完整ID編號即可搜尋

			LabelSearch.text        = GameDataDB.GetString(5215);	//輸入ID搜尋
			if(searchModeIDNum >= 0)
			{
				Utility.ChangeAtlasSprite(SpriteSwitchSearch, searchModeIDNum);
			}
		}
	}
}
