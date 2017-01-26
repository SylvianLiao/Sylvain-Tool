using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadGameState : CustomBehaviorState
{
    private UI_GamePlay m_uiGamePlay;
    private UI_Tutorial m_uiTutorial;

    public LoadGameState(GameScripts.GameFramework.GameApplication app) : base(StateName.LOAD_GAME_STATE, StateName.LOAD_GAME_STATE, app)
    {
    }

    public override void begin()
    {
        UnityDebugger.Debugger.Log("LoadGameState begin");
        bool isTutorial = m_mainApp.IsNewGuiding();
        base.SetGUIType((isTutorial)? typeof(UI_Tutorial) : typeof(UI_GamePlay));

        base.begin();

        if (m_bIsAync == false)
        {
            if (isTutorial)
                m_uiTutorial = m_guiManager.AddGUI<UI_Tutorial>(typeof(UI_Tutorial).Name);
            else
                m_uiGamePlay = m_guiManager.AddGUI<UI_GamePlay>(typeof(UI_GamePlay).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_Tutorial)))
            m_uiTutorial = m_guiManager.GetGUI(typeof(UI_Tutorial).Name) as UI_Tutorial;
        else if (operater.m_Type.Equals(typeof(UI_GamePlay)))
            m_uiGamePlay = m_guiManager.GetGUI(typeof(UI_GamePlay).Name) as UI_GamePlay;
    }

    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();

        ResourceManager resManager = m_mainApp.GetResourceManager();

        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        SongData songData = dataSystem.GetCurrentSongData();
        Enum_SongDifficulty nowDifficulty = dataSystem.GetCurrentSongPlayData().SongDifficulty;
        MusicGameSystem musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();


        // 初始化Properties
        if (m_mainApp.IsNewGuiding())
        {
            m_uiTutorial.GameDB = m_mainApp.GetGameDataDB();
            m_uiTutorial.playerDataSystem = m_mainApp.GetSystem<PlayerDataSystem>();            

            musicGameSys.LoadGameResource(m_uiTutorial);
            m_mainApp.MusicApp.StartCoroutine(ChangeToTutorialState());
        }            
        else
        {            
            musicGameSys.PlaySpeed = dataSystem.GetSongNodeSpeed(songData, nowDifficulty);

            // 讀曲譜面
            TextAsset ta = resManager.GetResourceSync<TextAsset>(Enum_ResourcesType.Songs, dataSystem.GetSheetMusicDataPath(songData, nowDifficulty));
            if (ta == null)
            {
                UnityDebugger.Debugger.LogError("Load TextAsset FAILED");
            }
            else
            {
                // 解析譜面資料
                //UnityDebugger.Debugger.Log(ta.text);
                musicGameSys.LoadMusicData(ta.text);
            }

            // 讀取音樂
            Softstar.AudioController audioController = m_mainApp.GetAudioController();
            AudioClip audioClip = resManager.GetResourceSync<AudioClip>(Enum_ResourcesType.Songs, dataSystem.GetSongPath(songData));
            if (audioClip == null)
            {
                UnityDebugger.Debugger.LogError("Load AudioClip FAILED");
            }
            else
            {
                audioController.SetBGM(audioClip);
            }


            m_uiGamePlay.GameDB = m_mainApp.GetGameDataDB();
            m_uiGamePlay.playerDataSystem = m_mainApp.GetSystem<PlayerDataSystem>();

            musicGameSys.LoadGameResource(m_uiGamePlay);
            //載入FullCombo動畫
            m_uiGamePlay.SetFullComboAnim(resManager.GetResourceSync(Enum_ResourcesType.GUI, "UI_Fullcombo"));            

            m_mainApp.MusicApp.StartCoroutine(ChangeToGamePlayState());
        }
                                
    }   
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        m_uiGamePlay = null;
        base.end();
        UnityDebugger.Debugger.Log("LoadGameState end");
    }

    public override void onGUI()
    {
        base.onGUI();
    }

    public override void update()
    {
        base.update();
    }

    public override void suspend()
    {
        base.suspend();
        UnityDebugger.Debugger.Log("LoadGameState suspend");
    }

    public override void resume()
    {
        base.resume();
        UnityDebugger.Debugger.Log("LoadGameState resume");
    }

    //==========================================================================

    private IEnumerator ChangeToGamePlayState()
    {
        //Show GamePlay UI
        yield return m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(null));

        m_mainApp.ChangeState(StateName.GAME_PLAY_STATE, new Hashtable());
    }

    private IEnumerator ChangeToTutorialState()
    {
        //Show Tutorial UI
        yield return m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(null));

        m_mainApp.PushState(StateName.TUTORIAL_STATE, new Hashtable());
    }
}
