using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Softstar;


public class ThemeState : CustomBehaviorState
{
    private UI_Theme            m_uiTheme = null;
    private UI_ThemeIntroduce   m_uiThemeIntroduce= null;
    private UI_3D_Opening       m_ui3DOpening = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private PlayerDataSystem m_dataSystem;
    private SoundSystem m_soundSystem;

    public ThemeState(GameScripts.GameFramework.GameApplication app) : base(StateName.THEME_STATE, StateName.THEME_STATE, app)
	{
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_soundSystem = m_mainApp.GetSystem<SoundSystem>();
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
         UnityDebugger.Debugger.Log("ThemeState begin");

        //Set the GUI witch is this state want to use.
        SetGUIType(typeof(UI_Theme));
        SetGUIType(typeof(UI_ThemeIntroduce));
        SetGUIType(typeof(UI_3D_Opening) , true);

        base.begin();
        userData.Add(Enum_StateParam.UsePlayerInfoUI, true);

        if (!m_bIsAync)
        {
            m_uiTheme           = m_guiManager.AddGUI<UI_Theme>(typeof(UI_Theme).Name);
            m_uiThemeIntroduce  = m_guiManager.AddGUI<UI_ThemeIntroduce>(typeof(UI_ThemeIntroduce).Name);
            m_ui3DOpening      = m_gui3DManager.AddGUI<UI_3D_Opening>(base.GetGUIPath(typeof(UI_3D_Opening), m_dataSystem));

            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_Theme)))
            m_uiTheme = m_guiManager.GetGUI(typeof(UI_Theme).Name) as UI_Theme;
        else if (operater.m_Type.Equals(typeof(UI_ThemeIntroduce)))
            m_uiThemeIntroduce = m_guiManager.GetGUI(typeof(UI_ThemeIntroduce).Name) as UI_ThemeIntroduce;
        else if (operater.m_Type.Equals(typeof(UI_3D_Opening)))
            m_ui3DOpening = m_gui3DManager.GetGUI(typeof(UI_3D_Opening).Name) as UI_3D_Opening;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_gui3DManager.Initialize();
        m_uiTheme.SetInforTextureAndIcon(m_dataSystem);

        //Prepare player data
        PlayerData playerData = m_dataSystem.GetPlayerData();
        m_uiPlayerInfo.InitializeUI(playerData.Name, playerData.Diamond);

        SetThemeIntroduceContent();

        //Play Music
        PlayThemeMusic();

        //Play Theme Opening Anim
        m_mainApp.MusicApp.StartCoroutine(m_ui3DOpening.PlayThemeOpening());

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
        m_uiThemeIntroduce.Hide();
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        UnityDebugger.Debugger.Log("ThemeState End");

        if (m_uiTheme != null)
            RemoveCallBack();

        m_uiTheme = null;
        m_uiThemeIntroduce = null;

        base.end();
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        base.suspend();
        UnityDebugger.Debugger.Log("ThemeState suspend");

        if (m_bIsSuspendByFullScreenState && m_uiTheme != null)
        {
            m_uiTheme.Hide();
            m_uiThemeIntroduce.Hide();
        }
        RemoveCallBack();
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        base.resume();
        UnityDebugger.Debugger.Log("ThemeState resume");

        PlayThemeMusic();

        if (m_uiTheme != null)
        {
            m_uiTheme.SetInforTextureAndIcon(m_dataSystem);

            if (m_bIsSuspendByFullScreenState)
            {
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
                m_uiThemeIntroduce.Hide();
            }     
            else
            {
                CheckActiveCommonGUI();
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
    private void FadeInCommonUI()
    {
        m_uiPlayerInfo.FadeIn();
        //m_uiTopBar.FadeIn();
    }
    //---------------------------------------------------------------------------------------------------
    private void FadeOutCommonUI()
    {
        m_uiPlayerInfo.FadeOut();
        //m_uiTopBar.FadeOut();
    }
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        //Common UI Delegate
        m_uiPlayerInfo.OnButtonSettingClickEvent += SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.OnButtonListeningClickEvent += SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.OnButtonListeningClickEvent += StopThemeMusic;
        m_uiPlayerInfo.OnButtonMallClickEvent += SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.OnButtonTeachClickEvent = SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.AddCallBack();

        if (m_uiTheme != null)
        {
            UIEventListener.Get(m_uiTheme.m_buttonEnter.gameObject).onClick = OnButtonEnterClick;
            UIEventListener.Get(m_uiTheme.m_buttonIntroduce.gameObject).onClick = OnButtonIntroduceClick;
            UIEventListener.Get(m_uiThemeIntroduce.m_buttonBackToTheme.gameObject).onClick = OnButtonBackToThemeClick;
        }
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        //Common UI Delegate
        m_uiPlayerInfo.OnButtonSettingClickEvent = null;
        m_uiPlayerInfo.OnButtonListeningClickEvent = null;
        m_uiPlayerInfo.OnButtonMallClickEvent = null;
        m_uiPlayerInfo.OnButtonTeachClickEvent = null;
        m_uiPlayerInfo.RemoveCallBack();

        if (m_uiTheme != null)
        {
            UIEventListener.Get(m_uiTheme.m_buttonEnter.gameObject).onClick = null;
            UIEventListener.Get(m_uiTheme.m_buttonIntroduce.gameObject).onClick = null;
            UIEventListener.Get(m_uiThemeIntroduce.m_buttonBackToTheme.gameObject).onClick = null;
        }    
    }
    #region ButtonEvent
    //---------------------------------------------------------------------------------------------------
    private void OnButtonEnterClick(GameObject go)
    {
        //音樂延後至主題館Start動畫播完後關閉
        //StopSongMusic();
        SoundSystem soundsys = m_mainApp.GetSystem<SoundSystem>();
        soundsys.PlaySound(20);

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_SONG_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonIntroduceClick(GameObject go)
    {
        FadeOutCommonUI();
        RemoveCallBack();

        m_uiThemeIntroduce.OnFadeInFinish.Add(AddCallBack);
        m_guiManager.FadeIn(typeof(UI_ThemeIntroduce).Name);
        m_uiThemeIntroduce.RepositionContent();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackToThemeClick(GameObject go)
    {
        FadeInCommonUI();
        RemoveCallBack();

        m_uiThemeIntroduce.OnFadeOutFinish.Add(AddCallBack);
        m_guiManager.FadeOut(typeof(UI_ThemeIntroduce).Name);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    //Set Theme Introduce Content
    private void SetThemeIntroduceContent()
    {
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB< S_Themes_Tmp>().GetData(m_dataSystem.GetCurrentThemeID());
        m_uiThemeIntroduce.SetIntroduceTitle(m_mainApp.GetString(themeTmp.iShowName));
        foreach (ThemeIntroduce data in themeTmp.m_themeIntroStyle)
        {
            switch(data.m_style)
            {
                case Enum_ThemeIntroduceStyle.LabelStyle:
                    m_uiThemeIntroduce.AddLabel(m_mainApp.GetString(data.m_GUID));
                    break;
                case Enum_ThemeIntroduceStyle.TextureStyle:
                    S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(data.m_GUID);
                    m_uiThemeIntroduce.AddTexture(textureTmp.strTextureName);
                    break;
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    private void LoadAndPlayMusic(AsyncLoadOperation musicLoader)
    {
        if (musicLoader.m_assetObject != null)
        {
            AudioClip music = musicLoader.m_assetObject as AudioClip;
            AudioController controller = m_mainApp.GetAudioController();
            controller.PlayBGM(music, false);
        }
    }
    //-------------------------------------------------------------------------------------------------
    private void PlayThemeMusic()
    {
        string musicPath = m_dataSystem.GetThemeMusicName(m_dataSystem.GetCurrentThemeID());
        m_resourceManager.GetResourceASync(Enum_ResourcesType.Songs, musicPath, typeof(AudioClip), LoadAndPlayMusic);
    }
    //-------------------------------------------------------------------------------------------------
    private void StopThemeMusic()
    {
        AudioController controller = m_mainApp.GetAudioController();
        controller.StopBGM();
    }
    //---------------------------------------------------------------------------------------------------
    private string[] GetDelayDeleteGUIName()
    {
        return new string[] {typeof(UI_Theme).Name , typeof(UI_ThemeIntroduce).Name ,typeof(UI_3D_Opening).Name };
    } 
}

