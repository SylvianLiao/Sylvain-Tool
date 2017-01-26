using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum ENUM_SettingSwitch
{
	OFF = 0,
	ON,
}

public enum ENUM_SettingQuality
{
	Defult = -2,
	OFF = -1,
	LOW = 0,
	MID = 1,
	HIGH = 2,
}

public class UI_Setting : NGUIChildGUI {

	public UIPanel	BasePanel	= null;

	//setting
	public UIButton	btnMusic		= null;
	public UIButton	btnSound		= null;
	public UIButton	btnPLCount		= null;
	public UIButton	btnPetCtrl		= null;
	public UIButton btnHitEffect	= null;
	public UIButton	ButtonGuildBoss	= null;	//公會王戰鬥顯示按鈕

	public UILabel	b_lbMusic		= null;
	public UILabel	b_lbSound		= null;
	public UILabel	b_lbPLCount		= null;
	public UILabel	b_lbPetCtrl 	= null;
	public UILabel	b_lbHitEffect	= null;
	public UILabel	LabelGuildBoss	= null;	//公會王戰鬥顯示狀況

	//bottom
	public UIButton	btnContact	= null;
	public UIButton	btnShutdown	= null;
	//
	public UIButton	btnClose	= null;
	//------------
	//Text
	public UILabel lbTitle		= null;
	public UILabel lbMusic		= null;
	public UILabel lbSound		= null;
	public UILabel lbPLCount	= null;
	public UILabel lbPetCtrl	= null;
	public UILabel lbContact	= null;
	public UILabel lbShutdown	= null;
	public UILabel lbHitEffect	= null;
	public UILabel LabelGuildBossTitle	= null;	//公會王戰鬥顯示標題

	//-----------
	public ENUM_SettingSwitch	emMusic;		//音樂狀態
	public ENUM_SettingSwitch	emSound;		//音效狀態
	public ENUM_SettingSwitch	emPetCtrl;		//寵物顯示狀況
	public ENUM_SettingQuality	emPLCount;		//顯示狀態
	public ENUM_SettingSwitch	emHitEffect;	//擊中特效狀態
	public ENUM_SettingSwitch	emGuildBoss;	//公會王戰鬥顯示狀況

	//---------------------關閉遊戲確認-----------------------------
	[Header("ExitCheck")]
	public UIWidget	wdExitCheck	= null;	//離開遊戲介面
	public UILabel	lbExitCheck	= null;	//離開遊戲確認字串
	public UILabel	lbExitYes	= null;	//確認
	public UILabel	lbExitNo	= null;	//取消
	public UIButton	btnExitYes	= null;
	public UIButton	btnExitNo	= null;

	//-------------------教學相關元件---------------------------
	[Header("Guide")]
	public UIPanel			panelGuide				= null; //指引集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen 			= null; //全螢幕按鍵
	public UILabel			lbGuideIntroduce		= null; //導引介紹系統設定
	public UISprite			spGuideClose			= null; //導引關閉
	public UILabel			lbGuideClose			= null;  

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Setting";
	//-----------------------------------------------------------------------------------------------------
	private UI_Setting() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Hide ()
	{
		base.Hide ();
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Initialize()
	{

		base.Initialize();
		//判斷執行環境IOS下隱藏關閉遊戲按鈕
#if UNITY_ANDROID
		UnityDebugger.Debugger.Log("IsinAndroid");
#elif UNITY_IOS
		btnShutdown.gameObject.SetActive(false);
		btnContact.transform.position = new Vector3(0.0f,btnContact.transform.position.y,btnContact.transform.position.z);
#endif

		InitLabel();

		emMusic		= ENUM_SettingSwitch.OFF;
		emSound		= ENUM_SettingSwitch.OFF;
		emPLCount	= ENUM_SettingQuality.OFF;
		emPetCtrl	= ENUM_SettingSwitch.ON;
		emHitEffect = ENUM_SettingSwitch.ON;

		wdExitCheck.gameObject.SetActive(false);
		//設定各按鈕狀態
		SetSysBtn();

	}
	//-------------------------------------------------------------------------------------------------------------
	void Update ()
	{
	}
	//-------------------------------------------------------------------------------------------------------------
	public void InitLabel()
	{
		//Title
		lbTitle.text		= GameDataDB.GetString(230);	//系統設置
		//功能按鈕
		b_lbMusic.text		= "";
		b_lbSound.text		= "";
		b_lbPLCount.text	= "";
		b_lbPetCtrl.text	= "";
		b_lbHitEffect.text	= "";
		LabelGuildBoss.text	= "";

		//項目文字
		lbMusic.text			= GameDataDB.GetString(236);	//"音樂開關"
		lbSound.text			= GameDataDB.GetString(237);	//"音效開關"
		lbPLCount.text			= GameDataDB.GetString(238);	//"顯示玩家數量"
		lbPetCtrl.text			= GameDataDB.GetString(297);	//"其他玩家夥伴"
		lbHitEffect.text		= GameDataDB.GetString(985);	//"擊中特效簡化"
		LabelGuildBossTitle.text= GameDataDB.GetString(966);	//"省略會戰顯示"

		//底部按鈕
		lbContact.text		= GameDataDB.GetString(239);	//"聯絡客服"
		lbShutdown.text		= GameDataDB.GetString(240);	//"離開遊戲"
		//離開遊戲介面
		lbExitCheck.text	= GameDataDB.GetString(180);	//"是否離開遊戲"
		lbExitYes.text		= GameDataDB.GetString(277);	//"確定"
		lbExitNo.text		= GameDataDB.GetString(278);	//"取消"

	}
	//-------------------------------------------------------------------------------------------------------------
	//開關型設定
	public void SetSwitchBtn(string sKey,UILabel label,ref ENUM_SettingSwitch emSwitch)
	{
		if(PlayerPrefs.HasKey(sKey))
		{
			if(PlayerPrefs.GetInt(sKey) == (int)ENUM_SettingSwitch.OFF)
			{
				label.text = GameDataDB.GetString(232);	//關
				emSwitch = ENUM_SettingSwitch.OFF;
			}
			else
			{
				label.text = GameDataDB.GetString(231);	//開
				emSwitch = ENUM_SettingSwitch.ON;
			}
		}
		else
		{
			label.text = GameDataDB.GetString(231);
			PlayerPrefs.SetInt(sKey,(int)ENUM_SettingSwitch.ON);
		}
	}

	//-------------------------------------------------------------------------------------------------------------
	//開關型設定
	public void ReSetSwitchBtn(string sKey,UILabel label,ref ENUM_SettingSwitch emSwitch)
	{
		if(emSwitch == (int)ENUM_SettingSwitch.OFF)
		{
			label.text = GameDataDB.GetString(231);	//開
			emSwitch = ENUM_SettingSwitch.ON;
		}
		else
		{
			label.text = GameDataDB.GetString(232);	//關
			emSwitch = ENUM_SettingSwitch.OFF;
		}

		ARPGApplication.instance.SaveUserPrefInt(sKey, (int)emSwitch);
	}
	//-------------------------------------------------------------------------------------------------------------
	//品質型設定
	private void SetQualityBtn(string sKey,UILabel label,ref ENUM_SettingQuality emQuality)
	{
		if(PlayerPrefs.HasKey(sKey))
		{
			switch(PlayerPrefs.GetInt(sKey))
			{
			case (int)ENUM_SettingQuality.OFF:
				label.text = GameDataDB.GetString(232);	//關
				emQuality = ENUM_SettingQuality.OFF;
				break;
			case (int)ENUM_SettingQuality.LOW:
				label.text = GameDataDB.GetString(233);	//低
				emQuality = ENUM_SettingQuality.LOW;
				break;
			case (int)ENUM_SettingQuality.MID:
				label.text = GameDataDB.GetString(234);	//中
				emQuality = ENUM_SettingQuality.MID;
				break;
			case (int)ENUM_SettingQuality.HIGH:
				label.text = GameDataDB.GetString(235);	//高
				emQuality = ENUM_SettingQuality.HIGH;
				break;
			}

			UnityDebugger.Debugger.Log(string.Format("##########{0}:Quility is {1}##########","Quility Has Key",(int)emQuality));
		}
		else
		{
			Enum_QuilitySettingType emQuilityType = ARPGApplication.instance.m_QualityLevel;
			switch(emQuilityType)
			{
			case Enum_QuilitySettingType.Fastest:
				label.text = GameDataDB.GetString(233);	//低
				emQuality = ENUM_SettingQuality.LOW;
				break;
			case Enum_QuilitySettingType.Good:
				label.text = GameDataDB.GetString(234);	//中
				emQuality = ENUM_SettingQuality.MID;
				break;
			case Enum_QuilitySettingType.Fantastic:
				label.text = GameDataDB.GetString(235);	//高
				emQuality = ENUM_SettingQuality.HIGH;
				break;
			}

			UnityDebugger.Debugger.Log(string.Format("##########{0}:Quility is {1}##########","Quility No Key",(int)emQuilityType));
		}

	}
	//-------------------------------------------------------------------------------------------------------------
	//擊中特效設定
	private void SetHitEffectBtn(string sKey, UILabel label, ref ENUM_SettingSwitch heType)
	{		
		if(PlayerPrefs.HasKey(sKey))
		{
			switch(PlayerPrefs.GetInt(sKey))
			{
			case (int)ENUM_SettingSwitch.OFF:
				label.text = GameDataDB.GetString(232);	//"關"
				heType = ENUM_SettingSwitch.OFF;
				break;
			case (int)ENUM_SettingSwitch.ON:
				label.text = GameDataDB.GetString(231);	//"開"
				heType = ENUM_SettingSwitch.ON;
				break;
			}
			
			UnityDebugger.Debugger.Log(string.Format("##########{0}:HitEffect is {1}##########","HitEffect Has Key",(int)heType));
		}
		else
		{
			Enum_QuilitySettingType emQuilityType = ARPGApplication.instance.m_QualityLevel;
			switch(emQuilityType)
			{
			case Enum_QuilitySettingType.Fastest:
				label.text = GameDataDB.GetString(232);	//"關"
				heType = ENUM_SettingSwitch.OFF;
				break;
			case Enum_QuilitySettingType.Good:
			case Enum_QuilitySettingType.Fantastic:
				label.text = GameDataDB.GetString(231);	//"開"
				heType = ENUM_SettingSwitch.ON;
				break;
			}

			PlayerPrefs.SetInt(sKey, (int)heType);			
			UnityDebugger.Debugger.Log(string.Format("##########{0}:HitEffect is {1}##########","HitEffect No Key",(int)heType));
		}		
	}

	//-------------------------------------------------------------------------------------------------------------
	//開啟時設定各按鈕狀態
	public void SetSysBtn()
	{
		//寵物顯式開關只開放給效能高級機種
		lbPetCtrl.gameObject.SetActive(ARPGApplication.instance.m_QualityLevel == Enum_QuilitySettingType.Fantastic);

		//音樂
		SetSwitchBtn(GameDefine.DEF_SYSSETTING_MUSIC,b_lbMusic,ref emMusic);
		//音效
		SetSwitchBtn(GameDefine.DEF_SYSSETTING_SOUND,b_lbSound,ref emSound);
		//顯示人數
		SetQualityBtn(GameDefine.DEF_SYSSETTING_PLCOUNT,b_lbPLCount,ref emPLCount);
		//寵物顯示
		SetSwitchBtn(GameDefine.DEF_SYSSETTING_PETCTRL,b_lbPetCtrl,ref emPetCtrl);
		if(ARPGApplication.instance.m_QualityLevel == Enum_QuilitySettingType.Fantastic)
			lbPetCtrl.gameObject.SetActive(ARPGApplication.instance.m_LobbySystem.SysOpenPetDisplay);
		//擊中特效顯示
		SetHitEffectBtn(GameDefine.DEF_SYSSETTING_HITEFFECT,b_lbHitEffect,ref emHitEffect);
		//省略會戰顯示
		SetSwitchBtn(GameDefine.DEF_SYSSETTING_GuildBossBattleShow, LabelGuildBoss, ref emGuildBoss);

	}
	//-------------------------------------------------------------------------------------------------------------


}
