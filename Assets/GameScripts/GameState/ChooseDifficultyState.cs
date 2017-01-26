using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using Softstar;

public class ChooseDifficultyState : CustomBehaviorState
{
    private UI_ChooseDifficulty m_uiChooseDifficulty = null;
    private UI_SongIntroduce m_uiSongIntroduce = null;
    private UI_3D_BattleBG m_ui3DBattleBG = null;
    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private PlayerDataSystem m_dataSystem;
    private SoundSystem m_soundSystem;
    private PacketHandler_GamePlay m_packetGamePlay;

    //RunTimeData
    private Enum_SongDifficulty m_currentSongDifficulty = Enum_SongDifficulty.Easy;
    private SongData m_currentSongData
    {
        get { return m_dataSystem.GetCurrentSongData(); }
        set { m_dataSystem.SetCurrentSong(value); }
    }
    private ThemeData m_currentThemeData
    {
        get { return m_dataSystem.GetCurrentThemeData(); }
        set { m_dataSystem.SetCurrentTheme(value); }
    }
    private AsyncLoadOperation m_musicLoader;
    private bool m_bIsSwiping;

    public ChooseDifficultyState(GameScripts.GameFramework.GameApplication app) : base(StateName.CHOOSE_DIFFICULTY_STATE, StateName.CHOOSE_DIFFICULTY_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_soundSystem = m_mainApp.GetSystem<SoundSystem>();
        m_packetGamePlay = m_mainApp.GetSystem<NetworkSystem>().GetPaketHandler(typeof(PacketHandler_GamePlay).Name) as PacketHandler_GamePlay;

        m_bIsSwiping = false;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("ChooseDifficultyState begin");

        base.SetGUIType(typeof(UI_ChooseDifficulty));
        base.SetGUIType(typeof(UI_SongIntroduce));
        base.SetGUIType(typeof(UI_3D_BattleBG), true);

        base.begin();
        userData.Add(Enum_StateParam.UseTopBarUI, true);

        if (m_bIsAync == false)
        {
            m_uiChooseDifficulty = m_guiManager.AddGUI<UI_ChooseDifficulty>(typeof(UI_ChooseDifficulty).Name);
            m_ui3DBattleBG = m_gui3DManager.AddGUI<UI_3D_BattleBG>(base.GetGUIPath(typeof(UI_3D_BattleBG), m_dataSystem));
            m_uiSongIntroduce = m_guiManager.AddGUI<UI_SongIntroduce>(base.GetGUIPath(typeof(UI_SongIntroduce), m_dataSystem));
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_strAssetName.Equals(typeof(UI_ChooseDifficulty).Name))
            m_uiChooseDifficulty = m_guiManager.GetGUI(typeof(UI_ChooseDifficulty).Name) as UI_ChooseDifficulty;
        if (operater.m_strAssetName.Equals(typeof(UI_SongIntroduce).Name))
            m_uiSongIntroduce = m_guiManager.GetGUI(typeof(UI_SongIntroduce).Name) as UI_SongIntroduce;
        else if (operater.m_Type.Equals(typeof(UI_3D_BattleBG)))
            m_ui3DBattleBG = m_gui3DManager.GetGUI(typeof(UI_3D_BattleBG).Name) as UI_3D_BattleBG; 
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_gui3DManager.Initialize();
        m_uiChooseDifficulty.InitializeUI(m_dataSystem, m_gameDataDB, m_mainApp.GetStringTable());
        m_uiSongIntroduce.InitializeUI(m_dataSystem, m_gameDataDB);

        m_ui3DBattleBG.SwitchChooseDifficulty(true);

        if (userData.ContainsKey(Enum_StateParam.SongDifficulty))
            m_currentSongDifficulty = (Enum_SongDifficulty)userData[Enum_StateParam.SongDifficulty];

        SetSongDifficultyDataUI(m_currentSongDifficulty, false);

        //Play Music
        string songPath = m_dataSystem.GetShortSongPath(m_currentSongData);
        m_musicLoader = m_resourceManager.GetResourceASync(Enum_ResourcesType.Songs, songPath, typeof(AudioClip), LoadingMusic);

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
        m_uiSongIntroduce.Hide();

        OnUIShowFinish += DoSomethingAfterUISHow;
    }
    //---------------------------------------------------------------------------------------------------
    private void DoSomethingAfterUISHow()
    {
        m_uiChooseDifficulty.PlayNumberPicker();
        m_uiChooseDifficulty.PlayTweenRank();
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        UnityDebugger.Debugger.Log("ChooseDifficultyState End");

        RealseRunTimeData();

        if (m_uiChooseDifficulty != null)
            RemoveCallBack();

        m_uiChooseDifficulty = null;
        OnUIShowFinish -= DoSomethingAfterUISHow;

        base.end();
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        base.suspend();
        UnityDebugger.Debugger.Log("ChooseDifficultyState suspend");

        StopSongMusic();

        if (m_bIsSuspendByFullScreenState && m_uiChooseDifficulty != null)
        {
            m_uiChooseDifficulty.Hide();
            m_uiSongIntroduce.Hide();
        }
        RemoveCallBack();
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        base.resume();
        UnityDebugger.Debugger.Log("ChooseDifficultyState resume");

        m_mainApp.MusicApp.StartCoroutine(PlaySongMusic());

        if (m_uiChooseDifficulty != null)
        {
            if (m_bIsSuspendByFullScreenState)
            {
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
                m_uiSongIntroduce.Hide();
            } 
            else
            {
                CheckActiveCommonGUI();
                m_uiChooseDifficulty.Show();
                AddCallBack();
            }

            m_bIsSuspendByFullScreenState = false;
        }
    }
    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
        DeterSetNodeSpeed();
    }
    //---------------------------------------------------------------------------------------------------
    //CallBack
    public override void AddCallBack()
    {
        //Common UI Delegate
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiPlayerInfo.AddCallBack();
        m_uiTopBar.AddCallBack();

        UIEventListener.Get(m_uiChooseDifficulty.m_buttonNextDifficulty.gameObject).onClick = OnButtonNextDifficultyClick;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonPreDifficulty.gameObject).onClick  = OnButtonPreDifficultyClick;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonStartGame.gameObject).onClick      = OnButtonStartGameClick;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonRanking.gameObject).onClick        = OnButtonRankingClick;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonIntroduce.gameObject).onClick      = OnButtonIntroduceClick;
        UIEventListener.Get(m_uiSongIntroduce.m_buttonCloseIntroduce.gameObject).onClick    = OnButtonCloseIntroduceClick;

        //EasyTouch.On_OverUIElement += OnSwipeDifficulty;
        EasyTouch.On_SwipeStart += OnSwipeDifficulty;
        EasyTouch.On_SwipeEnd += OnSwipeEnd;

        //Test
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonSpeedUp.gameObject).onClick = OnButtonSpeedUpClick;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonSpeedDown.gameObject).onClick = OnButtonSpeedDownClick;
    }
    public override void RemoveCallBack()
    {
        //Common UI Delegate
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiPlayerInfo.RemoveCallBack();
        m_uiTopBar.RemoveCallBack();

        UIEventListener.Get(m_uiChooseDifficulty.m_buttonNextDifficulty.gameObject).onClick = null;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonPreDifficulty.gameObject).onClick  = null;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonStartGame.gameObject).onClick      = null;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonRanking.gameObject).onClick        = null;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonIntroduce.gameObject).onClick      = null;
        UIEventListener.Get(m_uiSongIntroduce.m_buttonCloseIntroduce.gameObject).onClick    = null;

        //EasyTouch.On_OverUIElement -= OnSwipeDifficulty;
        EasyTouch.On_SwipeStart -= OnSwipeDifficulty;
        EasyTouch.On_SwipeEnd -= OnSwipeEnd;

        //Test
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonSpeedUp.gameObject).onClick = null;
        UIEventListener.Get(m_uiChooseDifficulty.m_buttonSpeedDown.gameObject).onClick = null;
    }
    #region ButtonEvents
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        if (!isPlaying)
            return;

        string[] guiName = new string[] { typeof(UI_ChooseDifficulty).Name, typeof(UI_3D_BattleBG).Name, typeof(UI_SongIntroduce).Name };
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, guiName);
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_SONG_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonNextDifficultyClick(GameObject go)
    {
        if (m_uiChooseDifficulty.m_scoreNumberPicker.IsPlaying || m_uiChooseDifficulty.m_comboNumberPicker.IsPlaying)
            return;

        int enumIndex = (int)m_currentSongDifficulty + 1;
        if (!m_currentSongData.CheckSongDifficulty((Enum_SongDifficulty)enumIndex))
            return;

        Enum_SongDifficulty difficulty = (Enum_SongDifficulty)enumIndex;
        if (m_currentSongData.CheckSongDifficulty(difficulty) == false)
            return;

        m_bIsSwiping = true;
        //m_soundSystem.PlaySound(8); //切換難度音效
        m_currentSongDifficulty = difficulty;
        SetSongDifficultyDataUI(m_currentSongDifficulty);

        m_uiChooseDifficulty.PlayNumberPicker();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonPreDifficultyClick(GameObject go)
    {
        if (m_uiChooseDifficulty.m_scoreNumberPicker.IsPlaying || m_uiChooseDifficulty.m_comboNumberPicker.IsPlaying)
            return;

        int enumIndex = (int)m_currentSongDifficulty - 1;
        if (!m_currentSongData.CheckSongDifficulty((Enum_SongDifficulty)enumIndex))
            return;

        Enum_SongDifficulty difficulty = (Enum_SongDifficulty)enumIndex;
        if (m_currentSongData.CheckSongDifficulty(difficulty) == false)
            return;

        //m_soundSystem.PlaySound(8); //切換難度音效
        m_bIsSwiping = true;
        m_currentSongDifficulty = (Enum_SongDifficulty)(enumIndex);
        SetSongDifficultyDataUI(m_currentSongDifficulty);

        m_uiChooseDifficulty.PlayNumberPicker();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonStartGameClick(GameObject go)
    {
        //檢查是否啟動<校正+戰鬥>新手教學
        TeachingSystem teachSys = m_mainApp.GetSystem<TeachingSystem>();
        if (!teachSys.CheckIsSkipBattleGuide() && !teachSys.CheckIsGuideFinished())
        {
            teachSys.NewGuideStart();
            teachSys.SetGuideEventToState(false);
            string[] guiName = new string[] { typeof(UI_ChooseDifficulty).Name, typeof(UI_SongIntroduce).Name };
            Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.LoadGUIAsync, true);
            table.Add(Enum_StateParam.DelayDeleteGUIName, guiName);
            m_mainApp.ChangeStateByScreenShot(StateName.ADJUST_STATE, table);
            return;
        }

        //檢查難度上鎖狀態
        SongDifficultyData diffData = m_currentSongData.GetSongDifficultyData(m_currentSongDifficulty);
        if (diffData == null)
            return;
        if (diffData.LockStatus == Enum_DifficultyLockStatus.Lock)
        {
            ShowCurrentDiffcultyUnlockCondition();
            return;
        }

        //檢查關卡是否合法
        if (m_mainApp.IsLogin())
        {
            S_Songs_Tmp songTmp = m_dataSystem.GetSongTmp(m_currentSongData, m_currentSongDifficulty);
            m_packetGamePlay.SendPacket_StartStage(songTmp.GUID);
        }
        else
        {
            PrepareToBattle();
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void OnSwipeDifficulty(Gesture gesture)
    {
        if (m_bIsSwiping)
            return;

        GameObject go = gesture.GetCurrentPickedObject();
        if (go == null)
            return;
        if (!go.transform.parent.gameObject.Equals(m_uiChooseDifficulty.m_allPanelList[1].gameObject))
            return;

        bool isRight = (gesture.position.x - gesture.startPosition.x) > 0;
        if (isRight)
        {
            OnButtonPreDifficultyClick(go);
        }
        else
        {
            OnButtonNextDifficultyClick(go);
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void OnSwipeEnd(Gesture gesture)
    {
        m_bIsSwiping = false;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonRankingClick(GameObject go)
    {
        return;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonIntroduceClick(GameObject go)
    {
        RemoveCallBack();
        m_uiTopBar.Hide();
        m_uiSongIntroduce.OnFadeInFinish.Add(AddCallBack);
        m_uiSongIntroduce.FadeIn();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonCloseIntroduceClick(GameObject go)
    {
        RemoveCallBack();
        m_uiSongIntroduce.OnFadeOutFinish.Add(m_uiTopBar.Show);
        m_uiSongIntroduce.OnFadeOutFinish.Add(AddCallBack);
        m_uiSongIntroduce.FadeOut();
    }
    #endregion  
    #region Setting Data
    //---------------------------------------------------------------------------------------------------
    private void SetSongDifficultyDataUI(Enum_SongDifficulty difficulty, bool tweenUI = true)
    {
        //設定左右切換按鈕的啟動與否
        m_uiChooseDifficulty.m_buttonPreDifficulty.isEnabled = m_currentSongData.CheckSongDifficulty(m_currentSongDifficulty-1);
        m_uiChooseDifficulty.m_buttonNextDifficulty.isEnabled = m_currentSongData.CheckSongDifficulty(m_currentSongDifficulty+1);

        //Tween難度圖示
        if (tweenUI)
            m_uiChooseDifficulty.TweenDifficultyTexture(difficulty);
        else
            m_uiChooseDifficulty.SetDifficultyTexturePos(difficulty);

        //設定難度資料於UI上
        SongDifficultyData diffData = m_currentSongData.GetSongDifficultyData(difficulty);
        m_uiChooseDifficulty.SetStar(diffData.Star);
        SetCircleRankSprite(diffData.Rank);
        m_uiChooseDifficulty.SetBestScore(diffData.Score);
        m_uiChooseDifficulty.SetBestCombo(diffData.Combo);

        //設定鎖頭
        if (diffData.LockStatus == Enum_DifficultyLockStatus.Lock)
            Softstar.Utility.ChangeButtonSprite(m_uiChooseDifficulty.m_buttonStartGame, 102);
        else
            Softstar.Utility.ChangeButtonSprite(m_uiChooseDifficulty.m_buttonStartGame, "", "", "", "", "");

        //Tween Rank圖示
        if (tweenUI)
            m_uiChooseDifficulty.PlayTweenRank();

        //測試資料-顯示總NOTE數量
        LoadGameDataToShowNoteNumber(diffData);  
        //Testing----------------
        m_uiChooseDifficulty.SetLabelNodeSpeed(m_dataSystem.GetSongNodeSpeed(m_currentSongData, m_currentSongDifficulty));
    }
    //---------------------------------------------------------------------------------------------------
    private void SetCircleRankSprite(Enum_SongRank rank)
    {
        int bgTextureID = 0;
        int rankTextureID = 0;
        switch (rank)
        {
            case Enum_SongRank.A:   bgTextureID = 1;  rankTextureID = 35; break;
            case Enum_SongRank.B:   bgTextureID = 2;  rankTextureID = 36; break;
            case Enum_SongRank.C:   bgTextureID = 3;  rankTextureID = 37; break;
            case Enum_SongRank.D:   bgTextureID = 4;  rankTextureID = 38; break;
            case Enum_SongRank.S:   bgTextureID = 5;  rankTextureID = 39; break;
            case Enum_SongRank.G:   bgTextureID = 6;  rankTextureID = 40; break;
            case Enum_SongRank.New: bgTextureID = 11; rankTextureID = 34; break;
            default:
                break;
        }

        string rankSpriteName = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(rankTextureID).strSpriteName;
        string bgSpriteName = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(bgTextureID).strSpriteName;
        m_uiChooseDifficulty.SetRankSprite(rankSpriteName, bgSpriteName);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    /// <summary>準備戰鬥</summary>
    public void PrepareToBattle()
    {
        //Setting Song Play Data
        SongDifficultyData diffData = m_dataSystem.GetCurrentSongData().GetSongDifficultyData(m_currentSongDifficulty);
        SongPlayData playData = m_dataSystem.GetCurrentSongPlayData();
        playData.SongDifficulty = m_currentSongDifficulty;
        playData.Star = diffData.Star;

        StopSongMusic();

        string[] guiName = new string[] { typeof(UI_ChooseDifficulty).Name, typeof(UI_SongIntroduce).Name };
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, guiName);
        m_mainApp.ChangeStateByScreenShot(StateName.LOAD_GAME_STATE, table);
    }
    //-------------------------------------------------------------------------------------------------
    private void LoadingMusic(AsyncLoadOperation musicLoader)
    {
        if (!isPlaying)
            return;

        if (musicLoader.m_assetObject != null)
        {
            //Set Music
            AudioClip music = musicLoader.m_assetObject as AudioClip;
            AudioController controller = m_mainApp.GetAudioController();
            controller.SetBGM(music);
            m_mainApp.MusicApp.StartCoroutine(PlaySongMusic());
        }

        m_musicLoader = null;
    }
    //-------------------------------------------------------------------------------------------------
    private IEnumerator PlaySongMusic()
    {
        AudioController controller = m_mainApp.GetAudioController();
        while (!controller.CheckBGM())
            yield return null;
        controller.PlayBGM(null, false);
    }
    //-------------------------------------------------------------------------------------------------
    private void StopSongMusic()
    {
        AudioController controller = m_mainApp.GetAudioController();
        controller.StopBGM();
    }
    //Test-----------------------------------------------------------------------------------------------------------
    private void DeterSetNodeSpeed()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnButtonSpeedUpClick(null);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnButtonSpeedDownClick(null);
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonSpeedUpClick(GameObject go)
    {
        m_uiChooseDifficulty.SetLabelNodeSpeed(m_dataSystem.SetSongNodeSpeed(m_currentSongData, m_currentSongDifficulty, 0.1f));
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonSpeedDownClick(GameObject go)
    {
        m_uiChooseDifficulty.SetLabelNodeSpeed(m_dataSystem.SetSongNodeSpeed(m_currentSongData, m_currentSongDifficulty, -0.1f));
    }
    //---------------------------------------------------------------------------------------------------
    //測試資料-顯示總NOTE數量
    private void LoadGameDataToShowNoteNumber(SongDifficultyData diffData)
    {
        ResourceManager resManager = m_mainApp.GetResourceManager();
        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        SongData songData = dataSystem.GetCurrentSongData();
        MusicGameSystem musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();
        // 讀曲譜面
        TextAsset ta = resManager.GetResourceSync<TextAsset>(Enum_ResourcesType.Songs, dataSystem.GetSheetMusicDataPath(songData, diffData.SongDifficulty));
        if (ta != null)
        {
            // 解析譜面資料
            musicGameSys.LoadMusicData(ta.text);
            m_uiChooseDifficulty.m_labelTotleNote.text = "Totle Note : " + musicGameSys.MusicPlayData.NotesCount.ToString();

        }
    }
    //-------------------------------------------------------------------------------------------------
    private void ShowCurrentDiffcultyUnlockCondition()
    { 
        S_Songs_Tmp songTmp = m_dataSystem.GetSongTmp(m_currentSongData, m_currentSongDifficulty);
        S_SongUnlock_Tmp unlockTmp = m_mainApp.GetSystem<SongUnlockSystem>().GetUnlockCondition(songTmp.GUID);
        string strUnlock = null;
        if (unlockTmp == null)
            strUnlock = m_mainApp.GetString(20); //20: "歌曲難度尚未解鎖"
        else
        {
            S_Songs_Tmp conditionSongTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(unlockTmp.iConditionSong);
            strUnlock = string.Format(m_mainApp.GetString(46),                              //"{0}:於歌曲<{1}>{2}難度達成{3}評價"
                            m_mainApp.GetString(47 + songTmp.iDifficulty),                  //"簡單"
                            m_mainApp.GetString(conditionSongTmp.iShowName),
                            m_mainApp.GetString(47 + (int)conditionSongTmp.iDifficulty),    //"簡單"
                            unlockTmp.iConditionRank.ToString());
        }
        m_mainApp.PushIconCheckBox(m_mainApp.GetString(20), m_mainApp.GetString(19), strUnlock, null, null, null, 106); //20:"歌曲難度尚未解鎖", 19:"解鎖條件" ,106:解鎖系統用圖案
    }

    //---------------------------------------------------------------------------------------------------
    private void RealseRunTimeData()
    {
        m_bIsSwiping = false;

        if (m_musicLoader != null)
            m_musicLoader.CancelLoad();
        m_musicLoader = null;
    }
}

