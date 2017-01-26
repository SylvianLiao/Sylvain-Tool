using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_GuildV2 : NGUIChildGUI  
{
	const   int 			guildSlotCount 	= (int)GameDefine.GUILD_PAGE_COUNT;

	//共用
	public UIButton				ButtonRankPage			= null; // 公會排名頁簽按鈕
	public UILabel				LabelRankPage 			= null; // 公會排名頁簽Label
	public UISprite				SpriteRankPageMark		= null; // RankPage選取標示

	public UIButton				ButtonGuildPage 		= null; // 我的公會頁簽按鈕
	public UILabel				LabelGuildPage 			= null; // 我的公會頁簽Label
	public UISprite				SpriteGuildPageMark		= null; // GuildPage選取標示
	public UIButton				ButtonGuildClose		= null; // 關閉公會介面按鈕
	public UIGrid				GridGuildList			= null; // Slot用
	public UIButton				ButtonArrowR			= null; // 右翻頁
	public UIButton				ButtonArrowL			= null;	// 左翻頁

	//--------------公會排行榜 & 建立加入公會------------------------------------------
	// 排行榜
	public Transform			RankBase			= null;	// 排行榜頁簽時顯示

	// 搜尋base
	public UIWidget				SearchBase			= null;	// 搜尋按鈕base
	public UIButton				ButtonSearch		= null; // 搜尋按鈕
	public UIInput				InputSearch			= null; // 搜尋Input
	public UILabel				LabelSearch			= null; // 搜尋Label

	public UILabel				LabelClearSearch	= null; // 清除Label
	public UIButton				ButtonClearSearch	= null; // 清除按鈕

	//公會資訊
	public TweenPosition		twposGuildInfoBase		= null;	
	public UILabel				LabelGuildName 			= null; // 公會名稱
	public UILabel				LabelGuildLVTitle		= null; // 公會等級標題
	public UILabel				LabelGuildLV 			= null; // 公會等級
	public UILabel				LabelGuildIDTitle 		= null; // 公會編號標題
	public UILabel				LabelGuildID 			= null; // 公會編
	public UISprite				SpriteGuildLeaderIcon	= null; // 會長頭像 外框(?)
	public UILabel				LabelGuildLeaderNameTitle 	= null; // 會長名稱標題
	public UILabel				LabelGuildLeaderName 		= null; // 會長名稱
	public UILabel				LabelGuildNoteTitle 	= null; // 公會對外公告標題
	public UILabel				LabelGuildNote 			= null; // 公會對外公告
	public UILabel				LabelJoinLimitTitle 	= null; // 公會加入限制標題
	public UILabel				LabelJoinLimit 			= null; // 公會加入限制

	// NoGuildBase 
	public UIWidget				NoGuildBase			= null;	// 無公會按鈕base
	public UIButton				ButtonJoin 			= null;	// 加入按鈕
	public UISprite				SpriteJoin			= null;	// 加入按鈕圖片
	public UILabel				LabelJoin 			= null; // 加入Label
	public UIButton				ButtonCreat 		= null; // 建立按鈕
	public UISprite				SpriteCreat			= null; // 建立按鈕圖片
	public UILabel				LabelCreat 			= null;	// 建立Label

	//GridGuildList
	public UISprite				SpriteGuildListBG2			= null;	// GRID底版
	public UILabel				LabelGuildRankTitle 		= null;	// 排名標題
	public UILabel				LabelGuildNameTitle 		= null;	// 名稱標題
	public UILabel				LabelGuildLevelTitle 		= null;	// 公會等級標題
	public UILabel				LabelGuildMemberCountTitle 	= null;	// 人數標題
	
	// 建立公會
	public Transform			CreatGuildBase			= null; // 建立公會
//	public UISprite				CreatGuildBG			= null;
	public UILabel				LabelCreatGuildTitle	= null;	// 創立公會標題
	
	public UIButton				ButtonCancel 			= null;	// Cancel按鈕
	public UIButton				ButtonOK 				= null;	// OK按鈕
	public UILabel				LabelOK					= null;	// 創立
	public UIInput				InputCreatGuild			= null; // 創立公會輸入框
	public UILabel				LabelCreatGuild			= null; // 創立公會

	public UILabel				LabelNameTip			= null;	// 公會名提示
	public UILabel				LabelCreatLimitTitle	= null;	// 創立限制標題
//	public List<UILabel>		ListCreatLimitLabel		= new List<UILabel>();	// 創立限制
	public UILabel				LabelCreatLimit			= null;	//創立限制

	//--------------我的公會----------------------------------------------------
	//我的公會
	public Transform			GuildBase				= null;	// 公會按鈕base
	public UILabel				LabelMyGuildInfoTitle 			= null;	// 我的公會名稱	
	public UILabel				LabelMyGuildInfoLVTitle 		= null; // 我的公會等級標題
	public UILabel				LabelMyGuildInfoLV				= null; // 我的公會等級
	public UILabel				LabelMyGuildInfoRankTitle		= null; // 我的公會排名標題
	public UILabel				LabelMyGuildInfoRank			= null; // 我的公會排名
	public UILabel				LabelMyGuildInfoMembersTitle	= null; // 我的公會人數標題
	public UILabel				LabelMyGuildInfoMembers			= null; // 我的公會人數
	public UILabel				LabelMyGuildInfoPowerTitle		= null; // 我的公會戰力標題
	public UILabel				LabelMyGuildInfoPower			= null; // 我的公會戰力
	public UILabel				LabelMyGuildInfoFundTitle		= null; // 我的公會基金標題
	public UILabel				LabelMyGuildInfoFund			= null; // 我的公會基金
	public UILabel				LabelMyGuildInfoSelfMoneyTitle	= null; // 我的公會貢獻標題
	public UILabel				LabelMyGuildInfoSelfMoney		= null; // 我的公會貢獻
	public UILabel				LabelMyGuildInfoNoticeTitle		= null; // 我的公會公告標題
	public UIButton				ButtonGuildSetting				= null; // 我的公會設置按鈕
	public UILabel				LabelGuildSetting				= null; // 我的公會設置
	public UILabel				LabelMyGuildInfoNotice			= null; // 我的公會公告
	public UILabel				LabelGuildWarOpenNote			= null; // 公會戰開放時間
	public UILabel				LabelGuildWarOpenReamin			= null; // 公會戰剩餘多久開放

	public UIButton				ButtonGuildHistory			= null; // 公會動態按鈕
	public UILabel				LabelGuildHistory			= null; // 公會動態Label
	public UISprite				SpriteNewHistoryTip			= null; // 公會動態Tip

	public UIButton				ButtonGuildMember			= null; // 公會成員按鈕
	public UILabel				LabelGuildMember			= null; // 公會成員Label
	public UIButton				ButtonGuildChatroom			= null; // 聊天室按鈕
	public UILabel				LabelGuildChatroom			= null; // 聊天室Label
	public UIButton				ButtonGuildDonation			= null; // 捐獻按鈕
	public UILabel				LabelGuildDonation			= null; // 捐獻Label

//  public UIButton				ButtonStore 		        = null;	// 公會商店按鈕
	
	public SpriteRenderer		SpriteTreeCanUse			= null; // 神樹可以使用

	//會員清單
	public Transform			MemberListBase				= null;	// 會員清單base
	public UILabel				LabelMemberListTitle		= null; // 會員清單標題
	public UIButton				ButtonMemberListClose 		= null; // 關閉會員清單
	public UILabel				LabelMemberListJobInfo		= null; // 職稱說明標題
	public UIButton				ButtonMemberListJobInfo		= null; // 職稱說明按鈕
	public UIGrid				GridMemberList				= null; // 會員清單Grid
	public UILabel				LabelMemberListTip1			= null; // 會員清單小提示
	public UILabel				LabelMemberListTip2			= null; // 會員清單小提示
	public UIButton				ButtonMemberListArrowL		= null; // 會員清單左翻頁
	public UIButton				ButtonMemberListArrowR		= null; // 會員清單右翻頁
	public UIButton				ButtonLeaveGuild 			= null;	// 退出公會按鈕
	public UILabel				LabelLeaveGuild 			= null; // 退出公會 Label

	public Transform			JobInfoBase					= null;	// 職稱說明base
	public UILabel				LabelJobInfo				= null; // 職稱說明Label
	public UIButton				ButtonJobInfoClose			= null; // 職稱說明關閉按鈕

	//會員資訊
	public Transform			MemberInfoBase		= null;	// 會員資訊base

	public UILabel				LabelMemberInfoTitle		= null; // 會員資訊標題
	public UIButton				ButtonMemberInfoClose 		= null; // 關閉會員資訊
	public UISprite				SpriteMemberInfoIcon		= null; // 會員資訊-角色頭像
	public UISprite				Spriteframe					= null; // 會員資訊-角色框
	public UILabel				LabelMemberInfoIDTitle		= null; // 會員資訊-角色編號標題
	public UILabel				LabelMemberInfoID			= null; // 會員資訊-角色編號
	public UILabel				LabelMemberInfoNameTitle	= null; // 會員資訊-名稱標題
	public UILabel				LabelMemberInfoName 		= null; // 會員資訊-名稱
	public UILabel				LabelMemberInfoJobTitle 	= null; // 會員資訊-職位標題
	public UILabel				LabelMemberInfoJob 			= null; // 會員資訊-職位
	public UILabel				LabelMemberInfoLVTitle		= null; // 會員資訊-等級標題
	public UILabel				LabelMemberInfoLV 			= null; // 會員資訊-等級
	public UILabel				LabelMemberInfoMsg			= null; // 會員資訊-訊息
	public UIButton				ButtonMemberInfoMsg			= null; // 會員資訊-訊息按鈕

	public UIButton				ButtonUpLevel 			= null;	// 升等按鈕
	public UILabel				LabelUpLevel 			= null; // 升等 Label
	public UIButton				ButtonDownLevel 		= null;	// 降等按鈕
	public UILabel				LabelDownLevel 			= null; // 降等 Label

	
	//公會設定
	public Transform			GuildControlBase			= null;	// 公會設定base
	public UILabel				LabelGuildControlTitle		= null;	// 公會設定標題
	public UIButton				ButtonGuildControlClose 	= null; // 關閉
	public UILabel				LabelJoinLimiltTital2		= null;	// 戰力限制標題2
	public UILabel				LabelJoinLimiltTip			= null;	// 戰力限制提示
	public UIInput				InputJoinLimit				= null; // 加入戰力限制Input
	public UILabel				LabelInputJoinLimit			= null; // 加入戰力限制Label
	public UILabel				LabelGuildNoteOutsideTitle	= null;	// 對外公會公告標題
	public UIInput				InputGuildNoteOutside		= null;	// 對外公會公告輸入框
	public UILabel				LabelInputGuildNoteOutside	= null;	// 對外公會公告
	public UILabel				LabelGuildNoteInsideTitle	= null;	// 對內公會公告標題
	public UIInput				InputGuildNoteInside		= null;	// 對內公會公告輸入框
	public UILabel				LabelInputGuildNoteInside	= null;	// 對內公會公告
	public UIButton				ButtonGuildControlOK 		= null; // 確定
	public UILabel				LabelGuildControlOK			= null; // 確定Label
	public UILabel				LabelGuildControlFundTitle	= null; // 公會基金標題
	public UILabel				LabelGuildControlFund		= null; // 公會基金
	public UILabel				LabelBuildingUpgrade		= null; // 升級建築
	public UIGrid				GridBuildingUpgrade			= null; // 公會建築grid

	public List<Slot_GuildBuildingSetting>	slotGuildBuildingSetting	= new List<Slot_GuildBuildingSetting>();
	public List<Slot_GuildBuilding>			slotGuildBuilding			= new List<Slot_GuildBuilding>();

	//按鈕顏色
	public Color	changeColor			= new Color();
	private Color	BtnNoGuildColor 	= new Color();	//按鈕顏色

	//SLOT
	public string					slotName 		= "Slot_GuildListV2";
	public List<Slot_GuildListV2> 	slotGuildLists 	= new List<Slot_GuildListV2>();

	public string					slotBuildingName 	= "Slot_GuildBuilding";

	public string							slotMemberName 		= "Slot_GuildMemberListV2";
	public List<Slot_GuildMemberListV2> 	slotMemberLists 	= new List<Slot_GuildMemberListV2>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_GuildV2";

	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UILabel		lbGuideIntroduce				= null;
	public UISprite		spGuideGuild					= null; //導引加入或建立工會按鈕
	public UILabel		lbGuideGuild					= null;
	//-------------------------------------------------------------------------------------------------
	private UI_GuildV2() : base(GUI_SMARTOBJECT_NAME)
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

		CreatMemberSlot();

		ButtonUpLevel.userData = 0;
		ButtonDownLevel.userData = 1;
/*
		GuildControlBase.gameObject.SetActive(false);
		MemberInfoBase.gameObject.SetActive(false);

//		CreatGuildBG.gameObject.SetActive(false);
		CreatGuildBase.gameObject.SetActive(false);
*/
		GuildBase.gameObject.SetActive(false);

		BtnNoGuildColor = SpriteJoin.color;

		int i=0;
		for(i=0; i<slotGuildBuildingSetting.Count; ++i)
		{
			slotGuildBuildingSetting[i].InitialSlot();
				
		}	
		for(i=0; i<slotGuildBuilding.Count; ++i)
		{
			slotGuildBuilding[i].InitialSlot();
		}

	}
	
	//-------------------------------------------------------------------------------------------------
	public void InitialLabel()
	{	
		//共用
		LabelGuildPage.text 		= GameDataDB.GetString(8002); 	// 我的公會	
		LabelRankPage.text 			= GameDataDB.GetString(8001); 	// 公會排名

		//--------------RankBase------------------------------------------------
		LabelGuildName.text			= "";
		LabelGuildLVTitle.text		= GameDataDB.GetString(8004);	// 等級
		LabelGuildLV.text			= "";			
		LabelGuildIDTitle.text 		= GameDataDB.GetString(8049);	// ID
		LabelGuildID.text			= "";
		LabelGuildLeaderNameTitle.text 	= GameDataDB.GetString(8014);	// 會長：
		LabelGuildLeaderName.text	= "";
		LabelGuildNoteTitle.text 	= GameDataDB.GetString(8010);	// 公會公告
		LabelGuildNote.text			= "";
		LabelJoinLimitTitle.text 	= GameDataDB.GetString(8015);	// 加入條件
		LabelJoinLimit.text			= "";

		//GridGuildList
		LabelGuildRankTitle.text 		= GameDataDB.GetString(8005);	// 排名標題
		LabelGuildNameTitle.text 		= GameDataDB.GetString(8003);	// 名稱標題
		LabelGuildLevelTitle.text 		= GameDataDB.GetString(8004);	// 等級標題
		LabelGuildMemberCountTitle.text = GameDataDB.GetString(8006);	// 人數

		LabelJoin.text 				= GameDataDB.GetString(8017);	// 加入公會
		LabelCreat.text 			= GameDataDB.GetString(8018);	// 創立公會
		
		// 搜尋base
		InputSearch.value			= "";	// 請輸入ID
		LabelSearch.text 			= GameDataDB.GetString(8012);	// 請輸入ID

		LabelClearSearch.text 		= GameDataDB.GetString(8071);	// 清除
		
		// 建立公會
//		InputCreatGuild.value		= GameDataDB.GetString(8019);	// (輸入公會名稱)
		InputCreatGuild.value		= "";
		LabelOK.text 				= GameDataDB.GetString(8072);	// 創立
		LabelCreatGuild.text 		= GameDataDB.GetString(8019);	// (輸入公會名稱)
		LabelCreatGuildTitle.text 	= GameDataDB.GetString(8018);	// 創立公會
		LabelNameTip.text			= GameDataDB.GetString(8020);	// ※名稱2~6個文字或4~12個字符
		LabelCreatLimitTitle.text	= GameDataDB.GetString(8021);	// 創立條件：
		LabelCreatLimit.text		= string.Format("{0}\n{1}\n{2}\n{3}{4}",
		                                      GameDataDB.GetString(8022),
		                                      GameDataDB.GetString(8023),
		                                      GameDataDB.GetString(8024),
		                                      GameDataDB.GetString(8025),
		                                      GameDataDB.GetString(8026));

		//--------------GuildBase------------------------------------------------
		//我的公會
		LabelMyGuildInfoLVTitle.text 		= GameDataDB.GetString(8004); // 等級
		LabelMyGuildInfoRankTitle.text		= GameDataDB.GetString(8005); // 排名
		LabelMyGuildInfoMembersTitle.text	= GameDataDB.GetString(8006); // 人數
		LabelMyGuildInfoPowerTitle.text		= GameDataDB.GetString(8007); // 總戰力
		LabelMyGuildInfoFundTitle.text		= GameDataDB.GetString(8008); // 公會財庫
		LabelMyGuildInfoSelfMoneyTitle.text	= GameDataDB.GetString(8009); // 公會幣
		LabelMyGuildInfoNoticeTitle.text	= GameDataDB.GetString(8010); // 公會公告
		LabelGuildSetting.text				= GameDataDB.GetString(8029); // 設置
		LabelMyGuildInfoNotice.text			= GameDataDB.GetString(8011); // 優秀的公會，由我開始！
		//上方按鈕


		//下方按鈕
		LabelGuildHistory.text				= GameDataDB.GetString(8101); // 公會動態
		LabelGuildMember.text				= GameDataDB.GetString(8102); // 成員資訊
		LabelGuildChatroom.text				= GameDataDB.GetString(8103); // 聊天
		LabelGuildDonation.text				= GameDataDB.GetString(8104); // 捐獻

		//公會設定
		LabelGuildControlTitle.text			= GameDataDB.GetString(8050);	// 公會設置
		LabelJoinLimiltTital2.text 			= GameDataDB.GetString(8030);	// 加入公會戰力限制設定
		LabelJoinLimiltTip.text 			= GameDataDB.GetString(8031);	// ※限制玩家加入條件 
		InputJoinLimit.value				= "";
		LabelInputJoinLimit.text 			= "0";							// 加入戰力限制Label
		LabelGuildNoteOutsideTitle.text		= GameDataDB.GetString(8032);	// 公會對外公告
		InputGuildNoteOutside.value			= "";	// 優秀的公會，由我開始！
		LabelInputGuildNoteOutside.text		= GameDataDB.GetString(8011);	// 優秀的公會，由我開始！
		LabelGuildNoteInsideTitle.text		= GameDataDB.GetString(8033);	// 公會對內宣言
		InputGuildNoteInside.value			= "";	// 請大家每日去灌溉神樹！
		LabelInputGuildNoteInside.text		= GameDataDB.GetString(8028);	// 請大家每日去灌溉神樹！
		LabelGuildControlOK.text 			= GameDataDB.GetString(8046);	// 套用設定
		LabelGuildControlFundTitle.text		= GameDataDB.GetString(8008); 	// 公會財庫
		LabelGuildControlFund.text			= "0"; 							// 公會基金
		LabelBuildingUpgrade.text			= GameDataDB.GetString(8034); 	// 建築升級

		//會員
		LabelMemberListTitle.text			= GameDataDB.GetString(8102);	// 成員資訊 
		LabelMemberListJobInfo.text			= GameDataDB.GetString(8035);	// 職稱說明 
		LabelJobInfo.text					= GameDataDB.GetString(8141); 	// 職稱說明

		//會員資訊
		LabelMemberInfoTitle.text 			= GameDataDB.GetString(8102);	// 成員資訊
		LabelMemberInfoIDTitle.text 		= GameDataDB.GetString(8049);	// ID
		LabelMemberInfoNameTitle.text 		= GameDataDB.GetString(8003); 	// 名稱
		LabelMemberInfoJobTitle.text  		= GameDataDB.GetString(8051); 	// 職稱
		LabelMemberInfoLVTitle.text 		= GameDataDB.GetString(8004); 	// 等級
		LabelMemberInfoMsg.text 			= GameDataDB.GetString(8053); 	// 密語
		LabelLeaveGuild.text 				= GameDataDB.GetString(8036);	// 離開公會
		LabelMemberListTip1.text			= GameDataDB.GetString(8041);	// ※以職位進行排行 
		LabelMemberListTip2.text			= GameDataDB.GetString(8042);	// ※點擊成員頭像能進行密語，會長可升降職位 

		SetGuildWarOpenTime();
	}	
	
	//-------------------------------------------------------------------------------------------------
	public void SetJoinGuildCD(int time)
	{
		if(time > 0)
		{
//			LabelTimeLimit.text  = string.Format(GameDataDB.GetString(1642), time); //離下次加入公會還需要 {0:00} 小時 1642
		}
		else
		{
//			LabelTimeLimit.text  = "";
		}
	}
	
	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_GuildListV2"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_GuildListV2 go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_GuildListV2>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildV2 load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		// GuildList
		for(int i=0; i<guildSlotCount; ++i) 
		{
			Slot_GuildListV2 newgo	= Instantiate(go) as Slot_GuildListV2;

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
	// 公會會員
	void CreatMemberSlot()
	{
		if(slotMemberName == "")
		{
			slotMemberName = "Slot_GuildMemberListV2";
		}
		
		Slot_GuildMemberListV2 go = ResourceManager.Instance.GetGUI(slotMemberName).GetComponent<Slot_GuildMemberListV2>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildV2 load prefeb error,path:{0}", "GUI/"+slotMemberName) );
			return;
		}
		
		// GuildList
		for(int i=0; i<guildSlotCount; ++i) 
		{
			Slot_GuildMemberListV2 newgo	= Instantiate(go) as Slot_GuildMemberListV2;
			
			newgo.transform.parent			= GridMemberList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slot{0:00}",i);
			newgo.ButtonMemberSlot.userData = i;
			
			newgo.InitialSlot();
			slotMemberLists.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	//設定公會排行榜介面
	public void SetGuildRankPage()
	{
		RankBase.gameObject.SetActive(true);			// 公會排行base
		GuildBase.gameObject.SetActive(false);			// 我的公會base
		CreatGuildBase.gameObject.SetActive(false);		// 建立公會base
		MemberInfoBase.gameObject.SetActive(false);		// 會員資訊base
		GuildControlBase.gameObject.SetActive(false);	// 公會設定base		
		SearchBase.gameObject.SetActive(true);			// 搜尋按鈕base
		
		SpriteRankPageMark.gameObject.SetActive(true);		// RankPage選取標示
		SpriteGuildPageMark.gameObject.SetActive(false);	// GuildPage選取標示

		// 檢查狀態
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.IsInGuild()>0)
		{
//			NoGuildBase.gameObject.SetActive(false);
			ButtonJoin.isEnabled = false;
			SpriteJoin.color = changeColor;
			ButtonCreat.isEnabled = false;
			SpriteCreat.color = changeColor;		
		}
		else
		{
//			NoGuildBase.gameObject.SetActive(true);
			ButtonJoin.isEnabled = true;
			SpriteJoin.color = BtnNoGuildColor;
			ButtonCreat.isEnabled = true;
			SpriteCreat.color = BtnNoGuildColor;	
		}
		
	}
	
	//-------------------------------------------------------------------------------------------------
	//設定我的公會介面
	public void SetMyGuildPage()
	{
		RankBase.gameObject.SetActive(false);			// 公會排行base
		GuildBase.gameObject.SetActive(true);			// 我的公會base	
		CreatGuildBase.gameObject.SetActive(false); 	// 建立公會base
		MemberInfoBase.gameObject.SetActive(false);		// 會員資訊base
		GuildControlBase.gameObject.SetActive(false);	// 公會設定base	
		SearchBase.gameObject.SetActive(false);			// 搜尋按鈕base

		MemberListBase.gameObject.SetActive(false);		// 會員清單base

		SpriteRankPageMark.gameObject.SetActive(false);	// RankPage選取標示
		SpriteGuildPageMark.gameObject.SetActive(true);	// GuildPage選取標示

	}
	
	//-------------------------------------------------------------------------------------------------
	// 設定建立工會UI是否顯示
	public void SetCreatGuild(bool val)
	{
//		CreatGuildBG.gameObject.SetActive(val);
		CreatGuildBase.gameObject.SetActive(val);
	}
	
	//-------------------------------------------------------------------------------------------------
	//設定公會排行榜Slot
	public void SetJoinGuildSlot(List<GuildListData> guildList, int page)
	{
		//set label string
		if(guildList.Count <=0)
		{
			UnityDebugger.Debugger.Log(string.Format("SetJoinGuild error List.Count: {0}", guildList.Count));
		}
		
		for(int i=0; i<guildSlotCount; ++i)
		{
			if(i<guildList.Count)
			{
				slotGuildLists[i].gameObject.SetActive(true);
				slotGuildLists[i].SetGuildListLabel(guildList[i], page);
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
	//設定公會會員Slot
	public void SetMemberListSlot(List<GuildMemberData> list, int page)
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
				slotMemberLists[i].gameObject.SetActive(true);
				slotMemberLists[i].SetMemberListLabel(list[i]);
			}
			else
			{
				slotMemberLists[i].gameObject.SetActive(false);
			}
		}

		if(page == 0)
		{
			ButtonMemberListArrowL.gameObject.SetActive(false);
		}
		else
		{
			ButtonMemberListArrowL.gameObject.SetActive(true);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public string GetInputString()
	{
		return InputCreatGuild.value;
	}
	
	//-------------------------------------------------------------------------------------------------
	public void InitialInfo()
	{
		LabelGuildID.text 		= "";
		LabelGuildName.text 	= "";
		LabelGuildLV.text		= "";
		LabelGuildLeaderName.text	= "";
//		LabelGuildMember.text	= "";
		LabelGuildNote.text		= "";
	}

	//-------------------------------------------------------------------------------------------------
	public void InitialRankPage()
	{
		LabelGuildName.text 	= "";	// 公會名稱
		LabelGuildLV.text		= "";	// 公會等級
		LabelGuildID.text 		= "";	// 公會編號
		Utility.ChangeAtlasSprite(SpriteGuildLeaderIcon, -1);	// 會長頭像

		LabelGuildLeaderName.text	= "";	// 會長名稱
		LabelGuildNote.text			= "";	// 對外公告
		LabelJoinLimit.text			= "";	// 加入限制
	}

	//-------------------------------------------------------------------------------------------------
	// 設定排行榜頁面右邊的工會資訊
	public void SetOtherGuildInfo(GuildListData data)
	{
		if(data == null)
		{
			return ;
		}
		LabelGuildName.text 	= data.GuildName;				// 公會名稱
		LabelGuildLV.text		= data.GuildLevel.ToString();	// 公會等級
		LabelGuildID.text 		= data.iGuildID.ToString();		// 公會編號

		Utility.ChangeAtlasSprite(SpriteGuildLeaderIcon, data.iLeaderIcon);	// 會長頭像

		LabelGuildLeaderName.text 	= data.LeaderName;			// 會長名稱
		//戰力限制
		LabelJoinLimit.text 	= string.Format("{0}{1}", GameDataDB.GetString(8016), // 戰力：
		                                     			  data.RestrictionPower);		
		LabelGuildNote.text		= data.GuildNoticeForOut; //暫無			
	}
	
	//-------------------------------------------------------------------------------------------------
	// 設定我的公會的公會資訊
	public void SetMyGuildInfo()
	{
		GuildBaseData data = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		string name = ARPGApplication.instance.m_GuildSystem.GetLeaderName();
		int membercount = ARPGApplication.instance.m_GuildSystem.GetMemberCount();

		if(data == null || string.IsNullOrEmpty(name) || membercount <0)
		{
			return;
		}

		//公會名稱
		LabelMyGuildInfoTitle.text 		= string.Format("{0}({1})", data.GuildName, data.iGuildID);	
		//公會等級
		LabelMyGuildInfoLV.text			= data.BuildingLevel[(int)ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Guild].ToString();
		// 我的公會排名
		LabelMyGuildInfoRank.text		= (data.iNum+1).ToString();							
		// 我的公會人數
		LabelMyGuildInfoMembers.text	= string.Format("{0}/{1}", 
		                                            	 membercount, 
		                                              	 ARPGApplication.instance.m_GuildSystem.GetMemberUpperLimit(data.BuildingLevel[(int)ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Guild]));
//		// 我的公會戰力
//		LabelMyGuildInfoPower.text		= ""; 
		// 我的公會基金
		LabelMyGuildInfoFund.text		= data.GuildExp.ToString(); 
		// 我的公會貢獻
//		LabelMyGuildInfoSelfMoney.text	= string.Format("{0}/{1}", 
//		                                               ARPGApplication.instance.m_RoleSystem.iMemberMoney, 
//		                                               ARPGApplication.instance.m_GuildSystem.GetSelfData().MemberScore);	
		SetMyGuildMoney();
		// 我的公會公告
		LabelMyGuildInfoNotice.text		= data.GuildNoticeForIn; 

		//上面八棟建築	
/*		for(int i=0; i<slotGuildBuilding.Count; ++i)
		{
			//有開放
			if(i < data.BuildingLevel.Length)
			{
				slotGuildBuilding[i].SetSlot(data.BuildingLevel[i]);
			}
			else
			{
				slotGuildBuilding[i].SetSlot(-1);
			}
		}
*/
		SetGuildBuilding();
	}

	//-------------------------------------------------------------------------------------------------
	// 設定我的公會的公會資訊
	public void UpdateUseTreeInfo()
	{
/*
		// 我的公會基金
		LabelMyGuildInfoFund.text		= data.GuildExp.ToString(); 
		// 我的公會貢獻
		LabelMyGuildInfoSelfMoney.text	= string.Format("{0}/{1}", 
		                                               ARPGApplication.instance.m_RoleSystem.iMemberMoney, 
		                                               ARPGApplication.instance.m_GuildSystem.GetSelfData().MemberScore);
*/
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMemberInfo(GuildMemberData data, GuildMemberData selfData)
	{
		MemberInfoBase.gameObject.SetActive(true);
		
		//設定數值
		LabelMemberInfoLV.text 				= data.MemberLv.ToString(); 		// 會員資訊-等級
		LabelMemberInfoName.text 			= data.Name.ToString(); 			// 會員資訊-名稱
		LabelMemberInfoID.text				= data.RoleID.ToString(); 			// 會員資訊-角色編號
		//會員資訊-角色頭像
		Utility.ChangeAtlasSprite(SpriteMemberInfoIcon, data.MemberIcon);
//		LabelMemberInfoJob.text 			= GameDataDB.GetString(data.GuildPowerLevel); 	// 會員資訊-職位

		//設定按鈕
		//對方的職位
		switch(data.GuildPowerLevel)
		{
		case 0:	//會員
			LabelUpLevel.text 			= GameDataDB.GetString(1660);	// 升為副會長
			LabelDownLevel.text 		= GameDataDB.GetString(1661);	// 踢出公會

			LabelMemberInfoJob.text		= GameDataDB.GetString(8040);	// 成員
			break;
		case 1:	//副會長
			LabelUpLevel.text 			= GameDataDB.GetString(1662);	// 升為會長
			LabelDownLevel.text 		= GameDataDB.GetString(1663);	// 降為一般會員

			LabelMemberInfoJob.text		= GameDataDB.GetString(8038);	// 副會長
			break;
		case 2:	//會長
			LabelUpLevel.text 			= GameDataDB.GetString(1664);	// 錯誤
			LabelDownLevel.text 		= GameDataDB.GetString(1664);	// 錯誤

			LabelMemberInfoJob.text		= GameDataDB.GetString(8037);	// 會長
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
	// 設定公會設置資訊
	public void SetGuildSettingInfo()
	{
		GuildBaseData data = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if(data == null)
		{
			return;
		}

		// 對外公會公告
		InputGuildNoteOutside.value 	= data.GuildNoticeForOut;
		LabelInputGuildNoteOutside.text = data.GuildNoticeForOut;

		// 對內公會公告
		InputGuildNoteInside.value 		= data.GuildNoticeForIn;
		LabelInputGuildNoteInside.text 	= data.GuildNoticeForIn;

		// 加入戰力限制
		InputJoinLimit.value 			= data.RestrictionPower.ToString();
		LabelInputJoinLimit.text 		= data.RestrictionPower.ToString();

		LabelGuildControlFund.text 		= data.GuildExp.ToString();

		// 公會建設設定
		for(int i=0; i<slotGuildBuildingSetting.Count; ++i)
		{
			//有開放
			if(i < data.BuildingLevel.Length && i!=GameDefine.GUILD_BUILDING_BOSS)
			{
				slotGuildBuildingSetting[i].ButtonGuildBuilding.userData = i;
				slotGuildBuildingSetting[i].SetSlot(data, data.BuildingLevel[i], true);
				slotGuildBuildingSetting[i].gameObject.SetActive(true);
			}
			else
			{
//				slotGuildBuildingSetting[i].SetSlot(data, -1);
				slotGuildBuildingSetting[i].gameObject.SetActive(false);
			}
		}

		GuildControlBase.gameObject.SetActive(true);
	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetGuildBuilding()
	{
		GuildBaseData data = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if(data == null)
		{
			return;
		}

		//上面八棟建築	
		for(int i=0; i<slotGuildBuilding.Count; ++i)
		{
			//有開放
			if(i < data.BuildingLevel.Length)
			{
				slotGuildBuilding[i].SetSlot(data.BuildingLevel[i]);
			}
			else
			{
				slotGuildBuilding[i].SetSlot(-1);
			}
		}

		// 神樹可以使用
		if(ARPGApplication.instance.m_RoleSystem.GetRoleProgressFlag(ENUM_RoleProgressFlag.ENUM_RoleProgressFlag_GuildTree))
		{
			SpriteTreeCanUse.gameObject.SetActive(false);
		}
		else
		{
			SpriteTreeCanUse.gameObject.SetActive(true);
		}

	}
	
	//-------------------------------------------------------------------------------------------------
	public void SetGuildBuildingSettingSlot()
	{
		GuildBaseData data = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if(data == null)
		{
			return;
		}

		// 公會建設設定
		for(int i=0; i<slotGuildBuildingSetting.Count; ++i)
		{
			//有開放
			if(i < data.BuildingLevel.Length && i!=GameDefine.GUILD_BUILDING_BOSS)
			{
				slotGuildBuildingSetting[i].ButtonGuildBuilding.userData = i;
				slotGuildBuildingSetting[i].SetSlot(data, data.BuildingLevel[i], true);
				slotGuildBuildingSetting[i].gameObject.SetActive(true);
			}
			else
			{
//				slotGuildBuildingSetting[i].SetSlot(data, -1);
				slotGuildBuildingSetting[i].gameObject.SetActive(false);
			}
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMyGuildMoney()
	{
		if(GuildBase.gameObject.activeSelf)
		{
			// 我的公會貢獻
//			LabelMyGuildInfoSelfMoney.text	= string.Format("{0}/{1}", 
//			                                               ARPGApplication.instance.m_RoleSystem.iMemberMoney, 
//			                                               ARPGApplication.instance.m_GuildSystem.GetSelfData().MemberScore);
			LabelMyGuildInfoSelfMoney.text	=  ARPGApplication.instance.m_RoleSystem.iMemberMoney.ToString();
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void ClearSearchInput()
	{
		InputSearch.value			= "";	// 請輸入ID
		LabelSearch.text 			= GameDataDB.GetString(8012);	// 請輸入ID
	}

	//-------------------------------------------------------------------------------------------------
	//設定公會新歷程提示
	public void SetGuildNewHistoryTip()
	{
		SpriteNewHistoryTip.gameObject.SetActive(ARPGApplication.instance.m_GuildSystem.GetNewHistoryFlag());
	}
	//-------------------------------------------------------------------------------------------------
	public void SetGuildWarOpenTime()
	{
		S_Activity_Tmp actTmp = GameDataDB.ActivityDB.GetData(GameDefine.GUILDWAR_ACTIVTY_ID);
		if (actTmp == null)
		{
			LabelGuildWarOpenNote.gameObject.SetActive(false);
			return;
		}
		LabelGuildWarOpenNote.text = GameDataDB.GetString(actTmp.ActivityPeriod);
		LabelGuildWarOpenNote.gameObject.SetActive(true);

		S_Activity actData = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityByActType(EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_GuildWar);
		LabelGuildWarOpenReamin.gameObject.SetActive(actData != null);
		if (actData != null)
		{
			LabelGuildWarOpenReamin.text = GameDataDB.GetString(8665);	//"公會戰開打中"
		}
		else
		{
//			//將GMT+8轉換成UTC
//			int utcEndHour = actTmp.EndHour-8;
//			if (utcEndHour < 0)
//				utcEndHour = 24 + utcEndHour;
//			DateTime endDate = new DateTime(DateTime.UtcNow.Year, 
//			                                DateTime.UtcNow.Month, 
//			                                DateTime.UtcNow.Day, 
//			                                utcEndHour,0,0);
		}
	}
}

