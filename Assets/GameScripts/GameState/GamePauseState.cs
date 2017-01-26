using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class GamePauseState : CustomBehaviorState
{
    private UI_GamePause m_uiGamePause = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;

    public GamePauseState(GameScripts.GameFramework.GameApplication app) : base(StateName.GAME_PAUSE_STATE, StateName.GAME_PAUSE_STATE, app)
    {
        m_mainApp = app as MainApplication;
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_guiManager = m_mainApp.GetGUIManager();
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("ChooseSongState begin");

        SetGUIType(typeof(UI_GamePause));

        base.begin();

        if (m_bIsAync == false)
        {
            m_uiGamePause = m_guiManager.AddGUI<UI_GamePause>(typeof(UI_GamePause).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        m_uiGamePause = m_guiManager.GetGUI(typeof(UI_GamePause).Name) as UI_GamePause;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_uiGamePause.InitializeUI(m_mainApp.GetStringTable());
        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }

    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        m_uiGamePause = null;

        UI_3D_BattleBG ui3DBattleBG = m_mainApp.GetGUI3DManager().GetGUI(typeof(UI_3D_BattleBG).Name) as UI_3D_BattleBG;
        ui3DBattleBG.SwitchGamePause(false);
        m_guiManager.DeleteGUI(typeof(UI_GamePause).Name);
        base.end();
        UnityDebugger.Debugger.Log("GamePauseState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        if (m_bIsSuspendByFullScreenState && m_uiGamePause != null)
        {
            m_uiGamePause.Hide();
        }
        base.suspend();
        UnityDebugger.Debugger.Log("GamePauseState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        if (m_uiGamePause != null)
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
        UnityDebugger.Debugger.Log("GamePauseState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        UIEventListener.Get(m_uiGamePause.m_buttonRestart.gameObject).onClick           = OnButtonRestartClick;
        UIEventListener.Get(m_uiGamePause.m_buttonChooseSong.gameObject).onClick        = OnButtonChooseSongClick;
        UIEventListener.Get(m_uiGamePause.m_buttonResume.gameObject).onClick            = OnButtonResumeClick;
        UIEventListener.Get(m_uiGamePause.m_buttonChooseDifficulty.gameObject).onClick  = OnButtonChooseDifficultyClick;
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        UIEventListener.Get(m_uiGamePause.m_buttonRestart.gameObject).onClick           = null;
        UIEventListener.Get(m_uiGamePause.m_buttonChooseSong.gameObject).onClick        = null;
        UIEventListener.Get(m_uiGamePause.m_buttonResume.gameObject).onClick            = null;
        UIEventListener.Get(m_uiGamePause.m_buttonChooseDifficulty.gameObject).onClick  = null;
    }
    #region ButtonEvents
    //---------------------------------------------------------------------------------------------------
    private void OnButtonRestartClick(GameObject go)
    {
        if (!isPlaying)
            return;

        m_mainApp.PopStateByScreenShot();
        GamePlayState gpState = m_mainApp.GetGameStateByName(StateName.GAME_PLAY_STATE) as GamePlayState;
        gpState.m_bReplay = true;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonResumeClick(GameObject go)
    {
        if (!isPlaying)
            return;

        m_mainApp.PopStateByScreenShot();
        GamePlayState gpState = m_mainApp.GetGameStateByName(StateName.GAME_PLAY_STATE) as GamePlayState;
        gpState.m_bResume = true;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonChooseSongClick(GameObject go)
    {
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, new string[] { typeof(UI_GamePause).Name, typeof(UI_GamePlay).Name, typeof(UI_3D_BattleBG).Name });
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_SONG_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonChooseDifficultyClick(GameObject go)
    {
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, new string[] { typeof(UI_GamePause).Name, typeof(UI_GamePlay).Name });
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_DIFFICULTY_STATE, table);
    }
    #endregion
}

