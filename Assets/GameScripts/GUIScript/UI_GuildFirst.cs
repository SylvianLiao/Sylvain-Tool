using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_GuildFirst : NGUIChildGUI  
{
	const   int 			guildSlotCount 	= (int)GameDefine.GUILD_PAGE_COUNT;

	//共用
	public UIGrid				GridGuildList			= null; // Slot用
	public UIButton				ButtonArrowR			= null; // 右翻頁
	public UIButton				ButtonArrowL			= null;	// 左翻頁

	public UIButton				ButtonGuildPage 		= null; // 我的公會頁簽按鈕
	public UIButton				ButtonRankPage			= null; // 公會排名頁簽按鈕
	public UILabel				LabelGuildPage 			= null; // 我的公會頁簽Label
	public UILabel				LabelRankPage 			= null; // 公會排名頁簽Label

	public UILabel				LabelGuildName 			= null; // 公會名稱

	public UILabel				LabelGuildIDTitle 		= null; // 公會編號標題
	public UILabel				LabelGuildID 			= null; // 公會編號
	public UILabel				LabelGuildEXPTitle 		= null; // 公會經驗值標題
	public UILabel				LabelGuildEXP 			= null; // 公會經驗值
	public UILabel				LabelGuildLeaderTitle 	= null; // 公會會長標題
	public UILabel				LabelGuildLeader 		= null; // 公會會長
	public UILabel				LabelGuildMemberTitle 	= null; // 公會人數標題
	public UILabel				LabelGuildMember 		= null; // 公會人數
	public UILabel				LabelGuildNoteTitle 	= null; // 公會公告標題
	public UILabel				LabelGuildNote 			= null; // 公會公告
	public UILabel				LabelGuildLV 			= null; // 公會等級
	public UILabel				LabelCreatTimeTitle 	= null; // 公會創立時間標題
	public UILabel				LabelCreatTime 			= null; // 公會創立時間
	public UILabel				LabelTotalPowerTitle 	= null; // 公會總戰力標題
	public UILabel				LabelTotalPower 		= null; // 公會總戰力
	public UILabel				LabelGuildRankTitle 	= null; // 公會排行標題
	public UILabel				LabelGuildRank 			= null; // 公會排行
	public UILabel				LabelJoinLimitTitle 	= null; // 公會加入限制標題
	public UILabel				LabelJoinLimit 			= null; // 公會加入限制

	public UISprite				SpriteRankPageMark		= null; // RankPage選取標示
	public UISprite				SpriteGuildPageMark		= null; // GuildPage選取標示

	//--------------公會排行榜 & 建立加入公會------------------------------------------
	// 排行榜
	public UIWidget				RankBase			= null;	// 排行榜頁簽時顯示

	// 無公會時顯示
	public UIWidget				NoGuildBase			= null;	// 無公會按鈕base
	public UIButton				ButtonJoin 			= null;	// 加入按鈕
	public UIButton				ButtonCreat 		= null; // 建立按鈕
	public UILabel				LabelJoin 			= null; // 加入Label
	public UILabel				LabelCreat 			= null;	// 建立Label
	public UILabel				LabelTimeLimit 		= null; // 限制時間
	// 搜尋base
	public UIWidget				SearchBase			= null;	// 搜尋按鈕base
	public UIButton				ButtonSearch		= null; // 搜尋按鈕
	public UILabel				LabelSearch 		= null; // 搜尋Label
	public UIInput				InputSearch			= null; // 搜尋Input
	// 建立公會
	public UISprite				CreatGuildBG		= null;
	public UIInput				input				= null;
	public UIButton				ButtonCancel 		= null;	// Cancel按鈕
	public UIButton				ButtonOK 			= null;	// OK按鈕
	public UILabel				LabelCreatGuildTitle= null;	// 創立公會需消耗
	public UILabel				LabelCreatGuildMoney= null;	// 100

	//--------------我的公會----------------------------------------------------
	//我的公會
	public UIWidget				GuildBase			= null;	// 公會按鈕base
	public UIButton				ButtonSign 			= null;	// 簽到按鈕
	public UIButton				ButtonBuild 		= null;	// 公會建設按鈕
	public UIButton				ButtonControl 		= null;	// 公會管理按鈕
	public UIButton				ButtonStore 		= null;	// 公會商店按鈕
	public UIButton				ButtonRaid 			= null;	// 公會副本按鈕
	public UIButton				ButtonGuildWar 		= null;	// 公會戰按鈕
	public UIButton				ButtonLeaveGuild 	= null;	// 退出公會按鈕

	public UILabel				LabelSign 			= null; // 簽到 Label
	public UILabel				LabelBuild 			= null; // 公會建設 Label
	public UILabel				LabeControl 		= null; // 公會管理 Label
	public UILabel				LabelStore 			= null; // 公會商店 Label
	public UILabel				LabelRaid 			= null; // 公會副本 Label
	public UILabel				LabelGuildWar 		= null; // 公會戰 Label
	public UILabel				LabelLeaveGuild 	= null; // 退出公會 Label

	public UILabel				LabelSelfGuildMoney	= null; // 貢獻值

	//會員資訊
	public UIWidget				MemberInfoBase		= null;	// 會員資訊base

	public UILabel				LabelMemberInfoLV 				= null; // 會員資訊-等級
	public UILabel				LabelMemberInfoName 			= null; // 會員資訊-名稱
	public UILabel				LabelMemberInfoJobTitle 		= null; // 會員資訊-職位標題
	public UILabel				LabelMemberInfoJob 				= null; // 會員資訊-職位
	public UILabel				LabelMemberInfoPowerTitle 		= null; // 會員資訊-戰力標題
	public UILabel				LabelMemberInfoPower 			= null; // 會員資訊-戰力
	public UILabel				LabelMemberInfoGuildMoneyTitle 	= null; // 會員資訊-貢獻標題
	public UILabel				LabelMemberInfoGuildMoney 		= null; // 會員資訊-貢獻

	public UIButton				ButtonUpLevel 			= null;	// 升等按鈕
	public UILabel				LabelUpLevel 			= null; // 升等 Label
	public UIButton				ButtonDownLevel 		= null;	// 降等按鈕
	public UILabel				LabelDownLevel 			= null; // 降等 Label
	public UIButton				ButtonMemberInfoClose 	= null; // 關閉

	//公會設定
	public UIWidget				GuildControlBase		= null;	// 公會設定base
	public UILabel				LabelJoinLimiltTital2	= null;	// 加入戰力限制標題2
	public UIInput				InputJoinLimit			= null; // 加入戰力限制Input
	public UILabel				LabelInputJoinLimit		= null; // 加入戰力限制Label
	public UILabel				LabelGuildNoteTitle2	= null;	// 公會公告標題2
	public UIInput				InputGuildNote			= null; // 公會公告Input
	public UILabel				LabelInputGuildNote		= null; // 公會公告Label
	public UIButton				ButtonGuildControlClose = null; // 關閉
	public UIButton				ButtonGuildControlOK 	= null; // 確定
	public UILabel				LabelGuildControlOK		= null; // 確定Label

	//SLOT
	public string					slotName 		= "Slot_GuildList";
	public List<Slot_GuildList> 	slotGuildLists 	= new List<Slot_GuildList>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_GuildFirst";

	//-------------------------------------------------------------------------------------------------
	private UI_GuildFirst() : base(GUI_SMARTOBJECT_NAME)
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
		InitialLabel();
		SetJoinGuildCD(0);
		CreatSlot();

		ButtonUpLevel.userData = 0;
		ButtonDownLevel.userData = 1;

		GuildControlBase.gameObject.SetActive(false);
		MemberInfoBase.gameObject.SetActive(false);
		CreatGuildBG.gameObject.SetActive(false);
		GuildBase.gameObject.SetActive(false);
	}

	//-------------------------------------------------------------------------------------------------
	public void InitialLabel()
	{	
		//共用
		LabelGuildPage.text 		= GameDataDB.GetString(1621); 	// 我的公會	1621	
		LabelRankPage.text 			= GameDataDB.GetString(1622); 	// 公會排名	1622

		LabelGuildName.text			="";			// 公會名稱

		LabelGuildIDTitle.text 		= GameDataDB.GetString(1623);	// 編號		1623
		LabelGuildID.text			="";
		LabelGuildEXPTitle.text 	= GameDataDB.GetString(1624);	// 經驗值	1624
		LabelGuildEXP.text			="";
		LabelGuildLeaderTitle.text 	= GameDataDB.GetString(1625);	// 會長		1625
		LabelGuildLeader.text		="";
		LabelGuildMemberTitle.text 	= GameDataDB.GetString(1626);	// 公會人數	1626
		LabelGuildMember.text		="";
		LabelGuildNoteTitle.text 	= GameDataDB.GetString(1627);	// 公會布告欄	1627
		LabelGuildNote.text			="";
		LabelGuildLV.text			="";			// 公會等級
		LabelCreatTimeTitle .text	= GameDataDB.GetString(1628);	// 創立時間	1628
		LabelCreatTime.text			="";
		LabelTotalPowerTitle.text	= GameDataDB.GetString(1629);	// 公會總戰力1629
		LabelTotalPower.text		="";
		LabelGuildRankTitle.text 	= GameDataDB.GetString(1630);	// 公會排行	1630
		LabelGuildRank.text			="";
		LabelJoinLimitTitle.text 	= GameDataDB.GetString(1631);	// 加入限制	1631
		LabelJoinLimit.text			="";

		// 無公會時顯示
		LabelJoin.text 				= GameDataDB.GetString(1632);	// 加入公會1632
		LabelCreat.text 			= GameDataDB.GetString(1633);	// 建立公會1633
		LabelTimeLimit.text 		= ""; 							// 限制時間

		// 搜尋base
		LabelSearch.text 			= GameDataDB.GetString(1634);	// 搜尋公會	1634
		InputSearch.value			= GameDataDB.GetString(1635);	// 請輸入名稱	1635

		// 建立公會
		input.value					= GameDataDB.GetString(1636);	// 請輸入名稱	1636
		LabelCreatGuildTitle.text 	= GameDataDB.GetString(1651);	// 創立公會需消耗	1651
		LabelCreatGuildMoney.text 	= "100";						// 100

		//我的公會
		LabelSign.text  			= GameDataDB.GetString(1637); 	// 公會簽到 1637
		LabelBuild.text  			= GameDataDB.GetString(1638);  	// 公會建設 1638
		LabeControl.text  			= GameDataDB.GetString(1639);  	// 公會管理 1639
		LabelStore.text  			= GameDataDB.GetString(1640);  	// 公會商店 1640
		LabelRaid.text 				= GameDataDB.GetString(1641);  	// 公會副本 1641
		LabelGuildWar.text  		= GameDataDB.GetString(1644);	// 公會戰	1644
		LabelLeaveGuild.text  		= GameDataDB.GetString(1645); 	// 退出公會	1645
		LabelSelfGuildMoney.text 	= GameDataDB.GetString(1646);	// 貢獻值：	1646

		LabelMemberInfoLV.text 				= ""; 		// 會員資訊-等級
		LabelMemberInfoName.text 			= ""; 		// 會員資訊-名稱
		LabelMemberInfoJobTitle.text 		= GameDataDB.GetString(1653); 	//職位 1653
		LabelMemberInfoJob.text 			= ""; 		// 會員資訊-職位
		LabelMemberInfoPowerTitle.text 		= GameDataDB.GetString(1654);  	// 戰力 1654
		LabelMemberInfoPower.text 			= ""; 		// 會員資訊-戰力
		LabelMemberInfoGuildMoneyTitle.text = GameDataDB.GetString(1655); 	// 貢獻 1655
		LabelMemberInfoGuildMoney.text 		= ""; 		// 會員資訊-貢獻
		LabelUpLevel.text 			= "";
		LabelDownLevel.text 		= "";

		//公會設定
		LabelJoinLimiltTital2.text 	= GameDataDB.GetString(1656);	// 加入公會戰力限制 1656
		LabelGuildNoteTitle2.text 	= GameDataDB.GetString(1657);	// 公會佈告 1657
		LabelInputJoinLimit.text 	= "0";							// 加入戰力限制Label
		LabelInputGuildNote.text 	= GameDataDB.GetString(1658);	// 最大200個字 1658
//		InputJoinLimit.value		= "0";
//		InputGuildNote.value		= "最大50個字(?)";
		LabelGuildControlOK.text 	= GameDataDB.GetString(1659);	// 確定
	}	

	//-------------------------------------------------------------------------------------------------
	public void SetJoinGuildCD(int time)
	{
		if(time > 0)
		{
			LabelTimeLimit.text  = string.Format(GameDataDB.GetString(1642), time); //離下次加入公會還需要 {0:00} 小時 1642
		}
		else
		{
			LabelTimeLimit.text  = "";
		}
	}

	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_GuildList"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_GuildList go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_GuildList>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildFirst load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		// GuildList
		for(int i=0; i<guildSlotCount; ++i) 
		{
			Slot_GuildList newgo	= Instantiate(go) as Slot_GuildList;
			
			
			newgo.transform.parent			= GridGuildList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slot{0:00}",i);
			newgo.ButtonSlot.userData = i;
			
			newgo.InitialSlot();
			slotGuildLists.Add(newgo);
		}
		
	}

	//-------------------------------------------------------------------------------------------------
	public void SetGuildRankPage()
	{
		RankBase.gameObject.SetActive(true);
		GuildBase.gameObject.SetActive(false);
		CreatGuildBG.gameObject.SetActive(false);

		SearchBase.gameObject.SetActive(true);

		SpriteRankPageMark.gameObject.SetActive(true);		// RankPage選取標示
		SpriteGuildPageMark.gameObject.SetActive(false);	// GuildPage選取標示

		MemberInfoBase.gameObject.SetActive(false);			// 會員資訊base

		// 檢查狀態
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.IsInGuild()>0)
		{
			NoGuildBase.gameObject.SetActive(false);
		}
		else
		{
			NoGuildBase.gameObject.SetActive(true);
		}

	}

	//-------------------------------------------------------------------------------------------------
	public void SetMyGuildPage()
	{
		RankBase.gameObject.SetActive(false);
		GuildBase.gameObject.SetActive(true);
		CreatGuildBG.gameObject.SetActive(false);
		
		SearchBase.gameObject.SetActive(false);

		SpriteRankPageMark.gameObject.SetActive(false);
		SpriteGuildPageMark.gameObject.SetActive(true);

		MemberInfoBase.gameObject.SetActive(false);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetCreatGuild(bool val)
	{
		CreatGuildBG.gameObject.SetActive(val);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetJoinGuildSlot(List<JSONPG_MtoC_GetGuildListData> guildList, int page)
	{
		//set label string
		if(guildList.Count <=0)
		{
			UnityDebugger.Debugger.LogError(string.Format("SetJoinGuild error List.Count: {0}", guildList.Count));
		}

		for(int i=0; i<guildSlotCount; ++i)
		{
			if(i<guildList.Count)
			{
				slotGuildLists[i].gameObject.SetActive(true);
				slotGuildLists[i].SetGuildListLabel(guildList[i]);
			}
			else
			{
				slotGuildLists[i].gameObject.SetActive(false);
			}
		}

		if(page == 0)
		{
			ButtonArrowL.gameObject.SetActive(false);
		}
		else
		{
			ButtonArrowL.gameObject.SetActive(true);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMemberListSlot(List<JSONPG_MtoC_GuildMemberData> list, int page)
	{
		
		//set label string
		if(list.Count <=0)
		{
			UnityDebugger.Debugger.LogError(string.Format("SetJoinGuild error List.Count: {0}", list.Count));
		}
		
		for(int i=0; i<guildSlotCount; ++i)
		{
			if(i<list.Count)
			{
				slotGuildLists[i].gameObject.SetActive(true);
				slotGuildLists[i].SetMemberListLabel(list[i]);
			}
			else
			{
				slotGuildLists[i].gameObject.SetActive(false);
			}
		}
		
		if(page == 0)
		{
			ButtonArrowL.gameObject.SetActive(false);
		}
		else
		{
			ButtonArrowL.gameObject.SetActive(true);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public string GetInputString()
	{
		return input.value;
	}

	//-------------------------------------------------------------------------------------------------
	public void InitialInfo()
	{
		LabelGuildID.text 		= "";
		LabelGuildName.text 	= "";
		LabelGuildLV.text		= "";
		LabelGuildLeader.text	= "";
		LabelGuildMember.text	= "";
		LabelGuildNote.text		= "";
	}

	//-------------------------------------------------------------------------------------------------
	public void SetOtherGuildInfo(JSONPG_MtoC_GetGuildListData data)
	{
		LabelGuildID.text 		= data.iGuildID.ToString();		//公會編號
		LabelGuildName.text 	= data.GuildName;				//公會名稱
		LabelGuildLV.text		= data.GuildLevel.ToString();	//公會等級
		LabelGuildLeader.text 	= data.LeaderName;				//公會會長名稱
		LabelGuildEXP.text		= "";							//公會經驗值
		LabelJoinLimit.text 	= data.RestrictionPower.ToString();		//戰力限制
		LabelGuildNote.text		= ""; //暫無

		//公會人數
		S_GuildLevel_Tmp temp = GameDataDB.GuildLevelDB.GetData(data.GuildLevel);

		LabelGuildMember.text = string.Format("{0,2}/{1,2}", data.MemberSize, temp.GuildMember);			
		
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMyGuildInfo(JSONPG_MtoC_GuildBaseData data, string name, int membercount)
	{
//		iSelfGuildMoney;	//要求者公會積分
//		GuildMoney;			//公會金
//		GuildSettings;		//公會設定

		LabelGuildID.text 		= data.iGuildID.ToString();		//公會編號
		LabelGuildName.text 	= data.GuildName;				//公會名稱
		LabelGuildLV.text		= data.BuildingLevel[(int)ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Guild].ToString();		//公會等級
		LabelGuildLeader.text 	= name;							//公會會長名稱
		LabelGuildEXP.text		= data.GuildExp.ToString();		//公會經驗值
		LabelJoinLimit.text = data.RestrictionPower.ToString();	//戰力限制
		LabelGuildNote.text = data.GuildNoticeForIn;			//公會公告

		//公會人數
		S_GuildLevel_Tmp temp = GameDataDB.GuildLevelDB.GetData(data.BuildingLevel[(int)ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Guild]);
		
		LabelGuildMember.text = string.Format("{0,2}/{1,2}", membercount, temp.GuildMember);			
		
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMemberInfo(JSONPG_MtoC_GuildMemberData data, JSONPG_MtoC_GuildMemberData selfData)
	{
		MemberInfoBase.gameObject.SetActive(true);

		//設定數值
		LabelMemberInfoLV.text 				= data.MemberLv.ToString(); 		// 會員資訊-等級
		LabelMemberInfoName.text 			= data.Name.ToString(); 			// 會員資訊-名稱
		LabelMemberInfoJob.text 			= data.GuildPowerLevel.ToString(); 	// 會員資訊-職位
		LabelMemberInfoPower.text 			= data.MemberPower.ToString(); 		// 會員資訊-戰力
		LabelMemberInfoGuildMoney.text 		= data.MemberScore.ToString(); 		// 會員資訊-貢獻

		//設定按鈕
		//對方的職位
		switch(data.GuildPowerLevel)
		{
		case 0:	//會員
			LabelUpLevel.text 			= GameDataDB.GetString(1660);	// 升為副會長
			LabelDownLevel.text 		= GameDataDB.GetString(1661);	// 踢出公會
			break;
		case 1:	//副會長
			LabelUpLevel.text 			= GameDataDB.GetString(1662);	//升為會長
			LabelDownLevel.text 		= GameDataDB.GetString(1663);	//降為一般會員
			break;
		case 2:	//會長
			LabelUpLevel.text 			= GameDataDB.GetString(1664);	//錯誤
			LabelDownLevel.text 		= GameDataDB.GetString(1664);	//錯誤
			break;
		}

		int temp = selfData.GuildPowerLevel*10 + data.GuildPowerLevel;

		//自己的職位
		switch(temp)
		{
		case 10: 	//副會長 會員
			ButtonUpLevel.gameObject.SetActive(false);
			ButtonDownLevel.gameObject.SetActive(true);
			break;
		case 12:	//副會長 會長
			ButtonUpLevel.gameObject.SetActive(false);
			ButtonDownLevel.gameObject.SetActive(false);
			break;
		case 20:	//會長 會員
			ButtonUpLevel.gameObject.SetActive(true);
			ButtonDownLevel.gameObject.SetActive(true);
			break;
		case 21:	//會長 副會長
			ButtonUpLevel.gameObject.SetActive(true);
			ButtonDownLevel.gameObject.SetActive(true);
			break;
		case 22:	//會長 會長
			ButtonUpLevel.gameObject.SetActive(false);
			ButtonDownLevel.gameObject.SetActive(false);
			break;
		default:
			ButtonUpLevel.gameObject.SetActive(false);
			ButtonDownLevel.gameObject.SetActive(false);
			UnityDebugger.Debugger.Log("+++temp = "+temp);
			break;
		}
	}

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
