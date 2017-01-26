using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_ChatChannels : NGUIChildGUI {

	public UIPanel			BasePanel		= null;

	public UIButton			btnWorldChannel	= null;
	public UIButton			btnGuildChannel	= null;
	public UIButton			btnMurmurChannel= null;
	public UIButton			btnSendMessage	= null;
	public UIButton			btnExit			= null;

	public UIButton			btnSetting		= null;
	public UIButton			btnCloseSetting	= null;
	public UIButton			btnYunVaSpeech	= null;
	public UIButton			btnYunVaVoice	= null;

	public UISprite			spriteRemind_W	= null;
	public UISprite			spriteRemind_G	= null;
	public UISprite			spriteRemind_M	= null;
	public UISprite			spriteCover		= null;


	public UILabel			lbWorldChannel	= null;
	public UILabel			lbGuildChannel	= null;
	public UILabel			lbMurmurChannel	= null;
	public UILabel			lbSendMessage	= null;
	public UILabel			lbSetting		= null;
	public UILabel			lbYunVaSpeech	= null;
	public UILabel			lbYunVaVoice	= null;

	public UIInput			ipChatContents	= null;		//對話輸入框
	
	public UIWrapContentEX	wrapcontent			= null;	//無限循環腳本
	public UIWrapContentEX	CoverWarpcontent	= null;

	//--World
	public UILabel			lbNote			= null;		//發言次數LB
	public UILabel			lbCDTime		= null;		//發言倒數時間
	[HideInInspector]
	public int				iSpeakTimes		= 0;		//可發言次數
	//--Guild
	//--MurMur
	public GameObject		gbMurmurInput	= null;		//密語目標名稱欄
	public UIInput			ipMurmurTarget	= null;	
	public UILabel			lbMurTo			= null;		//對
	public UILabel			lbMurSay		= null;		//說
	//--SetView
	[Header("SetView")]
	public UIPanel			panelSetView		= null;
	public UILabel			lbSetTitle			= null;
	public UILabel			lbSetNote1			= null;
	public UILabel			lbSetNote2			= null;
	public UILabel			lbSetNote3			= null;

	//Record
	public UISprite			spRecording			= null;
	public UISprite			spParsing			= null;
	public UILabel			lbRecording			= null;
	public UILabel			lbParsing			= null;

	public UIWidget			wgInput				= null;
	[HideInInspector]
	public List<Transform> tempChatlist			= null;
	[HideInInspector]
	public int				myID				= 0;	//角色ROLEID
	[HideInInspector]
	public long				TargetID			= 0;	//密語目標ID
	[HideInInspector]
	public string			TargetName			= "";	//密語目標名子
	//頻道Type
	[HideInInspector]
	public ENUM_MESSAGEBOARDTYPE emNowBoard;			//目前所在頁面

	public UIScrollView		scrollView			= null;
	public UIScrollView		scrollViewCover		= null;
	public int				ChatSlotNum			= 5;	//scrollView可顯示的數量

	//---------------------------玩家資訊-------------------------------------
	[Header("Player_Info")]//(彈出選擇框)
	public UIWidget			wgPlayerInfo				= null;
	public Slot_RoleIcon	slotRoleIcon				= null;

	public UIButton			btnCloseMsgBox				= null;
	public UIButton			btnSendApply				= null;
	public UIButton			btnMurmur					= null;
	public UIButton			btnBlockade					= null;

	public UILabel			lbSendApply					= null;
	public UILabel			lbMurmur					= null;
	public UILabel			lbBlockade					= null;

	public UILabel			lbInfoTitle					= null;
	public UILabel			lbInfoName					= null;
	public UILabel			lbInfoPower					= null;
	public UILabel			lbInfoLevel					= null;
	//---------------------------新手教學用-------------------------------------
	[Header("Guide_Panel")]
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UISprite		spGuideChat						= null; //導引介紹輸入文字
	public UILabel		lbGuideChat						= null;
	public UISprite		spGuideVoiceButton				= null; //導引介紹語音系統按鈕
	public UILabel		lbGuideVoiceButton				= null;
	public UILabel		lbGuideFinish					= null;	//導引教學結束
	//-------------------------------------------------------------------------------------------------
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ChatChannels";
	//-------------------------------------------------------------------------------------------------
	private UI_ChatChannels() : base(GUI_SMARTOBJECT_NAME)
	{}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
		emNowBoard = ENUM_MESSAGEBOARDTYPE.ENUM_MESSAGEBOARD_WORLD;
	}
	//-------------------------------------------------------------------------------------------------
	void Start () 
	{}
	//-------------------------------------------------------------------------------------------------
	void Update () 
	{}
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		tempChatlist = wrapcontent.GetChildren();

		InitialUILabel();
		//設定選擇功能框
		wgPlayerInfo.gameObject.SetActive(false);
		//設定提示圖案
		spriteRemind_W.gameObject.SetActive(false);
		spriteRemind_G.gameObject.SetActive(false);
		spriteRemind_M.gameObject.SetActive(false);
		//設定出是顯示頁面
		emNowBoard = ENUM_MESSAGEBOARDTYPE.ENUM_MESSAGEBOARD_WORLD;

		gbMurmurInput.SetActive(false);
		//檢查有無公會開起公會頻道按鈕功能
		btnGuildChannel.isEnabled = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.IsInGuild() > 0;

		wrapcontent.SortAlphabetically();

		btnYunVaSpeech.isEnabled = false;
		btnYunVaVoice.isEnabled = false;
		spRecording.gameObject.SetActive(false);
		spParsing.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	void InitialUILabel()
	{
		//世界按鈕
		lbWorldChannel.text			= GameDataDB.GetString(211);
		//公會按鈕
		lbGuildChannel.text			= GameDataDB.GetString(212);
		//密語按鈕
		lbMurmurChannel.text		= GameDataDB.GetString(213);
		//發送按鈕
		lbSendMessage.text			= GameDataDB.GetString(214);
		//黑名單按鈕
		lbSetting.text				= GameDataDB.GetString(252);
		//可發言次數
		lbNote.text					= string.Format("{0}{1}",GameDataDB.GetString(215),iSpeakTimes); //今日可發言次數 : XX
		//發言內容
		ipChatContents.label.text	= GameDataDB.GetString(216);
		//發言對象
		ipMurmurTarget.label.text	= "";

		lbSetTitle.text				= GameDataDB.GetString(223);
		lbSetNote1.text				= GameDataDB.GetString(225);
		lbSetNote2.text				= GameDataDB.GetString(226);
		lbSetNote3.text				= string.Format(GameDataDB.GetString(227),ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ChatCoverList.Count);

		//密語框
		lbMurTo.text				= GameDataDB.GetString(1147);	//對
		lbMurSay.text				= GameDataDB.GetString(1148);	//說

		//語音辨識
		lbYunVaSpeech.text = GameDataDB.GetString(421);

		//角色資訊
		lbInfoTitle.text = GameDataDB.GetString(2520);
		lbMurmur.text					= GameDataDB.GetString(218);
		lbBlockade.text					= GameDataDB.GetString(219);
		lbSendApply.text				= GameDataDB.GetString(241);
		lbInfoName.transform.parent.GetComponent<UILabel>().text = GameDataDB.GetString(2521);
		lbInfoPower.transform.parent.GetComponent<UILabel>().text = GameDataDB.GetString(2522);
		lbInfoLevel.transform.parent.GetComponent<UILabel>().text = GameDataDB.GetString(2523);

	}
	//-------------------------------------------------------------------------------------------------
	//設定發言次數
	public void SetUISpeakTimes(int times)
	{
		lbNote.text	= string.Format("{0}{1}",GameDataDB.GetString(215),times);
	}
	//-------------------------------------------------------------------------------------------------
	//設定發話次數顯示
	public void SetNoteLabel(int max)
	{
		if(emNowBoard != ENUM_MESSAGEBOARDTYPE.ENUM_MESSAGEBOARD_WORLD)
			return;
		lbNote.gameObject.SetActive(true);
		lbCDTime.gameObject.SetActive(max > 0);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetCoverCountLB(int count)
	{
		lbSetNote3.text	= string.Format(GameDataDB.GetString(227),count);
	}
	//-------------------------------------------------------------------------------------------------
	public void OpenSetView()
	{
		panelSetView.gameObject.SetActive(true);
		CoverWarpcontent.enabled = true;
		CoverWarpcontent.SortAlphabetically();
		SetCoverCountLB(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ChatCoverList.Count);
	}

	public void CloseSetView()
	{
		CoverWarpcontent.enabled = false;
		panelSetView.gameObject.SetActive(false);
	}

	public void SetMurMurRMShow(bool b)
	{
		spriteRemind_M.gameObject.SetActive(b);
	}

}
