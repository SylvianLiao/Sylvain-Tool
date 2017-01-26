using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public enum Enum_SettingPages
{
    ID = 0,
    Setting,
    Max,
}

public class SettingState : CustomBehaviorState
{
    private UI_Setting m_uiSetting = null;
    private UI_CommonBackground m_uiCommonBackground = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private RecordSystem m_recordSystem;
    private MusicGameSystem m_musicGameSys;

    private ENUM_LanguageType m_currentLanguage;    //玩家目前UI選擇的語言
    private bool m_bSoundOn;
    private bool IsSoundOn
    {
        get { return m_bSoundOn;}
        set
        {
            m_bSoundOn = value;
            SoundSystem ss = m_mainApp.GetSystem<SoundSystem>();
            if (m_bSoundOn) ss.SoundOn();
            else ss.SoundOff();
            m_uiSetting.SwitchSoundButton(m_bSoundOn);
        }
    }

    public SettingState(GameScripts.GameFramework.GameApplication app) : base(StateName.SETTING_STATE, StateName.SETTING_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_recordSystem = m_mainApp.GetSystem<RecordSystem>();
        m_musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("SettingState begin");

        base.SetGUIType(typeof(UI_Setting));
        base.SetGUIType(typeof(UI_CommonBackground));
        userData.Add(Enum_StateParam.UseTopBarUI, true);
        base.begin();

        if (m_bIsAync == false)
        {
            m_uiSetting = m_guiManager.AddGUI<UI_Setting>(typeof(UI_Setting).Name);
            m_uiCommonBackground = m_guiManager.AddGUI<UI_CommonBackground>(typeof(UI_CommonBackground).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_Setting)))
            m_uiSetting = m_guiManager.GetGUI(typeof(UI_Setting).Name) as UI_Setting;
        else if (operater.m_Type.Equals(typeof(UI_CommonBackground)))
            m_uiCommonBackground = m_guiManager.GetGUI(typeof(UI_CommonBackground).Name) as UI_CommonBackground;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_uiSetting.InitializeUI(m_mainApp.GetStringTable());
        //"設定", "設定", "帳號綁定"
        m_uiCommonBackground.InitializeUI(m_mainApp.GetString(33), new List<string> { m_mainApp.GetString(34), m_mainApp.GetString(33)});

        LoadClientSaveData();
        m_uiSetting.SetLanguageLabelPos(m_currentLanguage);
        SetLanguageSwitchButton(m_currentLanguage);
        m_uiSetting.SetAdjustNumber(m_musicGameSys.NoteTimeOffset);        
        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }

    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        m_uiSetting = null;

        m_currentLanguage = ENUM_LanguageType.None;

        m_guiManager.DeleteGUI(typeof(UI_Setting).Name);
        m_guiManager.DeleteGUI(typeof(UI_CommonBackground).Name);
        base.end();
        UnityDebugger.Debugger.Log("SettingState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        base.suspend();
        UnityDebugger.Debugger.Log("SettingState suspend");

        if (m_bIsSuspendByFullScreenState && m_uiSetting != null)
        {
            CheckActiveCommonGUI();
            m_uiSetting.Hide();
        }
        RemoveCallBack();
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        base.resume();
        UnityDebugger.Debugger.Log("SettingState resume");

        if (m_uiSetting != null)
        {
            LoadClientSaveData();
            m_uiSetting.SetLanguageLabelPos(m_currentLanguage);
            m_uiSetting.SetAdjustNumber(m_musicGameSys.NoteTimeOffset);
            //"設定", "設定", "帳號綁定"
            m_uiCommonBackground.InitializeUI(m_mainApp.GetString(33), new List<string> { m_mainApp.GetString(34), m_mainApp.GetString(33) }, false);
            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                ShowBeSetGUI();
                AddCallBack();
            }

            m_bIsSuspendByFullScreenState = false;
        }
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiTopBar.AddCallBack();
        m_uiCommonBackground.AddCallBack();
        UIEventListener.Get(m_uiSetting.m_buttonAdjust.gameObject).onClick          = OnButtonAdjustClick;
        UIEventListener.Get(m_uiSetting.m_buttonSoundOn.gameObject).onClick         = OnButtonSoundOnClick;
        UIEventListener.Get(m_uiSetting.m_buttonSoundOff.gameObject).onClick        = OnButtonSoundOffClick;
        UIEventListener.Get(m_uiSetting.m_buttonPreLanguage.gameObject).onClick     = OnButtonPreLanguageClick;
        UIEventListener.Get(m_uiSetting.m_buttonNextLanguage.gameObject).onClick    = OnButtonNextLanguageClick;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_SettingPages.ID].gameObject).onClick       += OnButtonIDClick;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_SettingPages.Setting].gameObject).onClick  += OnButtonSettingClick;
        UIEventListener.Get(m_uiSetting.m_buttonConfirmChange.gameObject).onClick   = OnButtonConfirmChangeClick;
        UIEventListener.Get(m_uiSetting.m_buttonConnectFacebook.gameObject).onClick  = OnButtonConnectFacebookClick;
        UIEventListener.Get(m_uiSetting.m_buttonConnectGoogle.gameObject).onClick   = OnButtonConnectGoogleClick;
        UIEventListener.Get(m_uiSetting.m_buttonEnterOTP.gameObject).onClick        = OnButtonEnterOTPClick;
        UIEventListener.Get(m_uiSetting.m_buttonGetOTP.gameObject).onClick          = OnButtonGetOTPClick;
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiTopBar.RemoveCallBack();
        m_uiCommonBackground.RemoveCallBack();
        UIEventListener.Get(m_uiSetting.m_buttonAdjust.gameObject).onClick          = null;
        UIEventListener.Get(m_uiSetting.m_buttonSoundOn.gameObject).onClick         = null;
        UIEventListener.Get(m_uiSetting.m_buttonSoundOff.gameObject).onClick        = null;
        UIEventListener.Get(m_uiSetting.m_buttonPreLanguage.gameObject).onClick     = null;
        UIEventListener.Get(m_uiSetting.m_buttonNextLanguage.gameObject).onClick    = null;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_SettingPages.ID].gameObject).onClick = null;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_SettingPages.Setting].gameObject).onClick = null;
        UIEventListener.Get(m_uiSetting.m_buttonConfirmChange.gameObject).onClick   = null;
        UIEventListener.Get(m_uiSetting.m_buttonConnectFacebook.gameObject).onClick  = null;
        UIEventListener.Get(m_uiSetting.m_buttonConnectGoogle.gameObject).onClick   = null;
        UIEventListener.Get(m_uiSetting.m_buttonEnterOTP.gameObject).onClick        = null;
        UIEventListener.Get(m_uiSetting.m_buttonGetOTP.gameObject).onClick          = null;
    }
    #region ButtonEvent
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        if (!isPlaying)
            return;

        m_mainApp.PopStateByScreenShot();
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonIDClick(GameObject go)
    {
        RemoveCallBack();
        m_uiSetting.SwitchToSettingPage(false, AddCallBack);
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonSettingClick(GameObject go)
    {
        RemoveCallBack();
        m_uiSetting.SwitchToSettingPage(true, AddCallBack);
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonSoundOnClick(GameObject go)
    {
        IsSoundOn = false;
        m_recordSystem.SetSoundSwitch(IsSoundOn);
        UnityDebugger.Debugger.Log("OnButtonSoundOnClick , Sound OFF");
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonSoundOffClick(GameObject go)
    {
        IsSoundOn = true;
        m_recordSystem.SetSoundSwitch(IsSoundOn);
        UnityDebugger.Debugger.Log("OnButtonSoundOnClick , Sound ON");
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonConnectFacebookClick(GameObject go)
    {
        m_mainApp.ThirdPartyBindSignIn(Softstar.ThirdParty.ThirdPartyName.FB);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonConnectGoogleClick(GameObject go)
    {
        m_mainApp.ThirdPartyBindSignIn(Softstar.ThirdParty.ThirdPartyName.GOOGLE);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonEnterOTPClick(GameObject go)
    {
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.EnterOTP, true);
        SetIsSuspendByFullScreenState();
        m_mainApp.PushStateByScreenShot(StateName.CHANGE_DEVICE_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonGetOTPClick(GameObject go)
    {
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.GetOTP, true);
        SetIsSuspendByFullScreenState();
        m_mainApp.PushStateByScreenShot(StateName.CHANGE_DEVICE_STATE, table);
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonAdjustClick(GameObject go)
    {
        AdjustState adjustState = m_mainApp.GetGameStateByName(StateName.ADJUST_STATE) as AdjustState;
        adjustState.IsPushBySettingState = true;

        SetIsSuspendByFullScreenState();
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        m_mainApp.PushStateByScreenShot(StateName.ADJUST_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonNextLanguageClick(GameObject go)
    {
        int enumIndex = (int)m_currentLanguage + 1;
        if (enumIndex > (int)ENUM_LanguageType.English)
            enumIndex = (int)ENUM_LanguageType.English;

        ENUM_LanguageType language = (ENUM_LanguageType)enumIndex;

        m_currentLanguage = language;
        m_uiSetting.TweenLanguageLabel(m_currentLanguage);
        SetLanguageSwitchButton(m_currentLanguage);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonPreLanguageClick(GameObject go)
    {
        int enumIndex = (int)m_currentLanguage - 1;
        if (enumIndex < 0)
            enumIndex = 0;

        ENUM_LanguageType language = (ENUM_LanguageType)enumIndex;

        m_currentLanguage = language;
        m_uiSetting.TweenLanguageLabel(language);
        SetLanguageSwitchButton(language);
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonConfirmChangeClick(GameObject go)
    {
        if (m_currentLanguage == m_mainApp.GetLanguage())
            return;

        //語言設定
        m_mainApp.SetLanguage(m_currentLanguage);
        OnUIFadeOutReloadUI();
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    private void LoadClientSaveData()
    {
        //音效設定
        IsSoundOn = m_recordSystem.GetSoundSwitch();

        //校正設定
        float adjustNumber = m_recordSystem.GetNoteAdjust();

        //語言設定
        m_currentLanguage = m_mainApp.GetLanguage();
    }
    //-------------------------------------------------------------------------------------------------
    private void SetLanguageSwitchButton(ENUM_LanguageType language)
    {
        m_uiSetting.m_buttonPreLanguage.isEnabled = (language != ENUM_LanguageType.TraditionalChinese);
        m_uiSetting.m_buttonNextLanguage.isEnabled = (language != ENUM_LanguageType.English);
        m_uiSetting.SetLanguageConfirmButton(language == m_mainApp.GetLanguage());
    }
    //-------------------------------------------------------------------------------------------------
    private void OnUIFadeOutReloadUI()
    {
        m_uiSetting.OnFadeOutFinish.Add(ReloadUI);
        m_uiSetting.FadeOut();
    }
    //-------------------------------------------------------------------------------------------------
    private void ReloadUI()
    {
        m_uiSetting.InitializeUI(m_mainApp.GetStringTable());
        m_uiCommonBackground.InitializeUI(m_mainApp.GetString(33), new List<string> { m_mainApp.GetString(34), m_mainApp.GetString(33) });
        m_uiSetting.SetLanguageConfirmButton(true);
        m_uiSetting.FadeIn();
    }
}

