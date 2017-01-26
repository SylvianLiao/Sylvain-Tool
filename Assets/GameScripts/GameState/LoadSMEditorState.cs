using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadSMEditorState : CustomBehaviorState
{
    //==========================================================================

    UI_GamePlay m_uiGamePlay;
    ResourceManager resManager;
    SMEditorEnvironment m_SMEEnvironment;
    Softstar.MainApplication m_app;

    //==========================================================================

    public LoadSMEditorState(GameScripts.GameFramework.GameApplication app) : base(StateName.LOAD_SME_STATE, StateName.LOAD_SME_STATE, app)
    {
    }

    public override void begin()
    {
        UnityDebugger.Debugger.Log("LoadSMEditorState begin");

        SetGUIType(typeof(UI_GamePlay));

        base.begin();

        m_app = gameApplication as Softstar.MainApplication;
        Softstar.GUIManager guiManager = m_app.GetGUIManager();

        m_uiGamePlay = guiManager.AddGUI<UI_GamePlay>(typeof(UI_GamePlay).Name);
        m_guiManager.Initialize();

        resManager = m_app.GetResourceManager();

        m_SMEEnvironment = GameObject.FindObjectOfType<SMEditorEnvironment>();

        //depth only會有殘影問題
        m_app.GetGUI3DManager().m_uiCameraGO.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
    }

    public override void end()
    {
        m_uiGamePlay = null;
        base.end();
    }

    public override void onGUI()
    {
        base.onGUI();
    }

    public override void update()
    {
        base.update();

//         if (m_SMEEnvironment == null)
//         {
//             UnityDebugger.Debugger.LogError("SMEditorEnvironment is Null");
//             return;
//         }
//                 
//         if (m_SMEEnvironment.musicData == null && m_SMEEnvironment.auidoClip == null)
//         {
//             UnityDebugger.Debugger.LogError("SMEditorEnvironment's Data is not set");
//             return;
//         }
// 
//         InitNoteData();
    }

    public override void suspend()
    {
        base.suspend();
    }

    public override void resume()
    {
        base.resume();
    }

    //==========================================================================

    public void InitNoteData()
    {
        if (m_SMEEnvironment == null)
        {
            UnityDebugger.Debugger.Log("SMEEnvironment is Null!!");
            return;
        }
            
        // 讀曲譜面
        MusicGameSystem musicGameSys = m_app.GetSystem<MusicGameSystem>();
        musicGameSys.MusicPlayData = m_SMEEnvironment.musicData;
        // 讀取音樂
        Softstar.AudioController audioController = m_app.GetAudioController();
        audioController.SetBGM(m_SMEEnvironment.auidoClip);
        //初始設定
        musicGameSys.InitializeGame();
        musicGameSys.PlayLength = audioController.GetBGMLength();
        musicGameSys.MusicPlayData.ScoreCont = musicGameSys.ScoreCont;
        musicGameSys.MusicPlayData.NoteCreate = musicGameSys.NoteCreate;
        musicGameSys.MusicPlayData.ParticleCreate = musicGameSys.ParticleCreate;
        musicGameSys.MusicPlayData.SetNoteScoreCont(musicGameSys.MusicPlayData.Notes);

        musicGameSys.LoadGameResource(m_uiGamePlay);

        m_mainApp.MusicApp.StartCoroutine(ChangeToGamePlayState());
    }

    private IEnumerator ChangeToGamePlayState()
    {
        yield return m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(null));

        if (isPlaying)
            gameApplication.PopState();
        m_mainApp.ChangeState(StateName.GAME_PLAY_STATE);
    }
}
