using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class TitleState : CustomBehaviorState
{
    public enum Enum_TitleLoginStatus
    {
        WaitForLogin,
        SigningUP,
        SigningIn,
        Online,
        Offline,
    }

    private UI_Title        m_uiTitle = null;
    private UI_3D_Opening   m_ui3DOpenning = null;
    private ResourceManager m_resourceManager;
    private SoundSystem m_SoundSystem;
    private LoginSystem m_loginSystem;

    public TitleState(GameScripts.GameFramework.GameApplication app) : base(StateName.TITLE_STATE, StateName.TITLE_STATE, app)
    {
        m_resourceManager = m_mainApp.GetResourceManager();
        m_SoundSystem = m_mainApp.GetSystem<SoundSystem>();
        m_loginSystem = m_mainApp.GetSystem<LoginSystem>();
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("TitleState begin");

        //檢查是否啟動<開始遊戲>新手教學
        TeachingSystem teachSys = m_mainApp.GetSystem<TeachingSystem>();
        if (!teachSys.CheckIsSkipGameStartGuide() && !teachSys.CheckIsGuideFinished())
        {
            teachSys.NewGuideStart();
            teachSys.SetGuideEventToState(true);
        }

        base.SetGUIType(typeof(UI_Title));
        base.SetGUIType(typeof(UI_3D_Opening) , true);
        base.begin();

        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        //Load GUI
        if (m_bIsAync == false)
        {
            m_uiTitle = m_guiManager.AddGUI<UI_Title>(typeof(UI_Title).Name);
            m_ui3DOpenning = m_gui3DManager.AddGUI<UI_3D_Opening>(base.GetGUIPath(typeof(UI_3D_Opening), dataSystem));

            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_Title)))
        {
            m_uiTitle = m_guiManager.GetGUI(typeof(UI_Title).Name) as UI_Title;
        }
        else if (operater.m_Type.Equals(typeof(UI_3D_Opening)))
        {
            m_ui3DOpenning = m_gui3DManager.GetGUI(typeof(UI_3D_Opening).Name) as UI_3D_Opening;
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_gui3DManager.Initialize();
        m_uiTitle.InitializeUI(m_mainApp.GetStringTable());
        m_uiTitle.SetLabelVersion(m_mainApp.GetVersion());

        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        //Play Music
        string musicPath = dataSystem.GetThemeMusicName(dataSystem.GetCurrentThemeID());
        AsyncLoadOperation muiscLoader = m_resourceManager.GetResourceASync(Enum_ResourcesType.Songs, musicPath, typeof(AudioClip), null);
        m_mainApp.MusicApp.StartCoroutine(LoadAndPlayMusic(muiscLoader));

        CheckSingUp();

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
         UnityDebugger.Debugger.Log("TitleState End");

        if (m_uiTitle != null)
            RemoveCallBack();

        m_uiTitle = null;

        //m_guiManager.DeleteGUI(typeof(UI_Title).Name);
        base.end();
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        UnityDebugger.Debugger.Log("TitleState suspend");

        if (m_bIsSuspendByFullScreenState && m_uiTitle != null)
        {
            m_uiTitle.Hide();
        }
        RemoveCallBack();

        base.suspend();
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        UnityDebugger.Debugger.Log("TitleState resume");

        if (m_uiTitle != null)
        {
            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                CheckActiveCommonGUI();
                AddCallBack();
            }
            m_bIsSuspendByFullScreenState = false;
        }

        base.resume();
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        UIEventListener.Get(m_uiTitle.m_buttonEnterGame.gameObject).onClick = OnButtonEnterGameClick;
        UIEventListener.Get(m_uiTitle.m_buttonQuickLogin.gameObject).onClick = OnButtonQuickLoginClick;
        UIEventListener.Get(m_uiTitle.m_buttonConntectFacebook.gameObject).onClick = OnButtonConnectThirdPartyClick;
        UIEventListener.Get(m_uiTitle.m_buttonConntectGoogle.gameObject).onClick = OnButtonConnectThirdPartyClick;
        UIEventListener.Get(m_uiTitle.m_buttonChangeDevice.gameObject).onClick = OnButtonChangeDeviceClick;
        UIEventListener.Get(m_uiTitle.m_buttonRuleOK.gameObject).onClick = OnButtonRuleOKClick;
        UIEventListener.Get(m_uiTitle.m_buttonRuleCancel.gameObject).onClick = OnButtonRuleCancelClick;
#if DEVELOP
        UIEventListener.Get(m_uiTitle.m_buttonForceSignUp.gameObject).onClick = OnButtonForceSignUpClick;
        UIEventListener.Get(m_uiTitle.m_buttonClearData.gameObject).onClick = OnButtonClearDataClick;
#endif
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        UIEventListener.Get(m_uiTitle.m_buttonEnterGame.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonQuickLogin.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonConntectFacebook.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonConntectGoogle.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonChangeDevice.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonRuleOK.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonRuleCancel.gameObject).onClick = null;
#if DEVELOP
        UIEventListener.Get(m_uiTitle.m_buttonForceSignUp.gameObject).onClick = null;
        UIEventListener.Get(m_uiTitle.m_buttonClearData.gameObject).onClick = null;
#endif
    }
    #region ButtonEvent
    //---------------------------------------------------------------------------------------------------
    private void OnButtonQuickLoginClick(GameObject go)
    {
        m_uiTitle.m_buttonRuleOK.userData = go;
        m_uiTitle.SwitchMemberRuleUI(true);    
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonEnterGameClick(GameObject go)
    {        
        /*
        if (m_mainApp.IsNewGuiding())
        {
            Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.LoadGUIAsync, false);
            table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
            m_mainApp.ChangeStateByScreenShot(StateName.ADJUST_STATE, table);
        }            
        else
        {*/
            // patch
            Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
            table.Add(GameDefine.FILEUPDATE_NEXTSTATE, StateName.THEME_STATE);
            m_mainApp.ChangeState(StateName.FILE_UPDATE_STATE,table);

            /*Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.LoadGUIAsync, false);
            table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
            m_mainApp.ChangeStateByScreenShot(StateName.THEME_STATE, table);*/
       // }            
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonChangeDeviceClick(GameObject go)
    {
        //TODO: 換機密碼
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.EnterOTP, true);
        SetIsSuspendByFullScreenState();
        m_mainApp.PushStateByScreenShot(StateName.CHANGE_DEVICE_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonConnectThirdPartyClick(GameObject go)
    {
        m_uiTitle.m_buttonRuleOK.userData = go;
        m_uiTitle.SwitchMemberRuleUI(true);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonRuleOKClick(GameObject go)
    {
        UIButton btn = go.GetComponent<UIButton>();
        if (btn == null)
            return;

        m_uiTitle.SwitchMemberRuleUI(false);
        if (btn.userData.Equals(m_uiTitle.m_buttonQuickLogin.gameObject))
            m_loginSystem.SignUp();
        else if (btn.userData.Equals(m_uiTitle.m_buttonConntectFacebook.gameObject))
            m_mainApp.ThirdPartySignIn(Softstar.ThirdParty.ThirdPartyName.FB);
        else if (btn.userData.Equals(m_uiTitle.m_buttonConntectGoogle.gameObject))
            m_mainApp.ThirdPartySignIn(Softstar.ThirdParty.ThirdPartyName.GOOGLE);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonRuleCancelClick(GameObject go)
    {
        m_uiTitle.SwitchMemberRuleUI(false);
    }
    #endregion
#if DEVELOP
    //---------------------------------------------------------------------------------------------------
    private void OnButtonForceSignUpClick(GameObject go)
    {
        LoginUISetting(Enum_TitleLoginStatus.SigningUP);
        m_loginSystem.SignUp();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonClearDataClick(GameObject go)
    {
        PlayerPrefs.DeleteAll();
        PlayerDataSystem dataSys = m_mainApp.GetSystem<PlayerDataSystem>();
        dataSys.Initialize();
        LoginUISetting(Enum_TitleLoginStatus.WaitForLogin);
    }
#endif
    //-------------------------------------------------------------------------------------------------
    private IEnumerator LoadAndPlayMusic(AsyncLoadOperation musicLoader)
    {
        if (!isPlaying)
            musicLoader.CancelLoad();

        yield return m_mainApp.MusicApp.StartCoroutine(musicLoader);

        if (musicLoader.m_assetObject != null)
        {
            AudioClip music = musicLoader.m_assetObject as AudioClip;
            AudioController controller = m_mainApp.GetAudioController();
            controller.PlayBGM(music, false);
        }
    }
    //-------------------------------------------------------------------------------------------------
    private void StopSongMusic()
    {
        AudioController controller = m_mainApp.GetAudioController();
        controller.StopBGM();
    }
    #region Login Process
    //-------------------------------------------------------------------------------------------------
    /// <summary>檢查是否有註冊帳號，若有則直接登入</summary>
    public void CheckSingUp()
    {
        if (m_loginSystem.IsSignUp())
        {
            LoginUISetting(Enum_TitleLoginStatus.SigningIn);
            m_loginSystem.SignIn();
        }
        else
            LoginUISetting(Enum_TitleLoginStatus.WaitForLogin);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    public void LoginUISetting(Enum_TitleLoginStatus status)
    {
        if (!isPlaying)
            return;

        string msg = "";
        switch (status)
        {
            case Enum_TitleLoginStatus.WaitForLogin:
                msg = "Please Login!";
                m_uiTitle.m_buttonEnterGame.gameObject.SetActive(false);
                m_uiTitle.m_buttonChangeDevice.gameObject.SetActive(true);
                m_uiTitle.SwitchLoginBackground(true);
                m_uiTitle.SwitchLoginButtons(true);
                break;
            case Enum_TitleLoginStatus.SigningUP:
                msg = "Signing UP...";
                m_uiTitle.m_buttonEnterGame.gameObject.SetActive(false);
                m_uiTitle.m_buttonChangeDevice.gameObject.SetActive(false);
                m_uiTitle.SwitchLoginBackground(true);
                m_uiTitle.SwitchLoginButtons(false);
                break;
            case Enum_TitleLoginStatus.SigningIn:
                msg = "Signing In...";
                m_uiTitle.m_buttonEnterGame.gameObject.SetActive(false);
                m_uiTitle.m_buttonChangeDevice.gameObject.SetActive(false);
                m_uiTitle.SwitchLoginBackground(true);
                m_uiTitle.SwitchLoginButtons(false);
                break;
            case Enum_TitleLoginStatus.Online:
                msg = "Online Login Success!";
                m_uiTitle.m_buttonEnterGame.gameObject.SetActive(true);
                m_uiTitle.m_buttonChangeDevice.gameObject.SetActive(true);
                m_uiTitle.SwitchLoginBackground(false);
                m_uiTitle.SwitchLoginButtons(false);
                break;
            case Enum_TitleLoginStatus.Offline:
                msg = "Offline Login Success!";
                m_uiTitle.m_buttonEnterGame.gameObject.SetActive(true);
                m_uiTitle.m_buttonChangeDevice.gameObject.SetActive(false);
                m_uiTitle.SwitchLoginBackground(false);
                m_uiTitle.SwitchLoginButtons(false);
                break;
            default:
                break;
        }

        m_uiTitle.SetLoginLog(msg);
    }
    //---------------------------------------------------------------------------------------------------
    private string[] GetDelayDeleteGUIName()
    {
        return new string[] { typeof(UI_Title).Name };
    }
}

