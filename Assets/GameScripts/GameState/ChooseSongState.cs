using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using HedgehogTeam.EasyTouch;

public class ChooseSongState : CustomBehaviorState
{
    private UI_ChooseSong m_uiChooseSong = null;
    private UI_3D_SongMenu m_ui3DSongMenu = null;
    private UI_3D_Background m_ui3DBackground = null;
    private UI_3D_BGEffect m_ui3DBGEffect = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private PlayerDataSystem m_dataSystem;
    private SoundSystem m_SoundSystem;

    private SongData m_currentSongData
    {
        get {return m_dataSystem.GetCurrentSongData();}
        set { m_dataSystem.SetCurrentSong(value);}
    }
    private ThemeData m_currentThemeData
    {
        get { return m_dataSystem.GetCurrentThemeData(); }
        set { m_dataSystem.SetCurrentTheme(value); }
    }

    //Swipe
    private SwipeController m_SwipeController;

    //Runtime Data
    private AsyncLoadOperation m_musicLoader;
    private Coroutine m_coroutineLoadMusic;
    public bool IsUpdateSongList;
    //---------------------------------------------------------------------------------------------------
    public ChooseSongState(GameScripts.GameFramework.GameApplication app) : base(StateName.CHOOSE_SONG_STATE, StateName.CHOOSE_SONG_STATE, app)
	{
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_SoundSystem = m_mainApp.GetSystem<SoundSystem>();
        IsUpdateSongList = false;
    }

    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("ChooseSongState begin");

        SetGUIType(typeof(UI_ChooseSong));
        SetGUIType(typeof(UI_3D_SongMenu), true);
        SetGUIType(typeof(UI_3D_Background), true);
        SetGUIType(typeof(UI_3D_BGEffect), true);

        base.begin();
        userData.Add(Enum_StateParam.UseTopBarUI, true);
        userData.Add(Enum_StateParam.UsePlayerInfoUI, true);

        if (m_bIsAync == false)
        {
            m_uiChooseSong = m_guiManager.AddGUI<UI_ChooseSong>(typeof(UI_ChooseSong).Name);
            m_ui3DSongMenu = m_gui3DManager.AddGUI<UI_3D_SongMenu>(base.GetGUIPath(typeof(UI_3D_SongMenu), m_dataSystem));
            m_ui3DBackground = m_gui3DManager.AddGUI<UI_3D_Background>(base.GetGUIPath(typeof(UI_3D_Background), m_dataSystem));
            m_ui3DBGEffect = m_gui3DManager.AddGUI<UI_3D_BGEffect>(base.GetGUIPath(typeof(UI_3D_BGEffect), m_dataSystem));

            m_mainApp.MusicApp.StartCoroutine(WaitForAnimThenScreenShot());
        }
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>因要在過場時插入動態背景動畫，故自己實作StateInitAsync</summary>
    protected override IEnumerator StateInitAsync()
    {
        if (m_bUseLoadingUI)
            m_guiManager.ShowLoadingUI(true);

        foreach (var op in m_guiLoadOperator)
        {
            yield return m_mainApp.MusicApp.StartCoroutine(op);

            GetGUIAsync(op);
        }

        if (m_bUseLoadingUI)
            m_guiManager.ShowLoadingUI(false);

        m_mainApp.MusicApp.StartCoroutine(WaitForAnimThenScreenShot());
    }
    //---------------------------------------------------------------------------------------------------
    private IEnumerator WaitForAnimThenScreenShot()
    {
        UI_3D_Opening ui3DOP = m_gui3DManager.GetGUI(typeof(UI_3D_Opening).Name) as UI_3D_Opening;   
        if (ui3DOP != null)
        {
            ui3DOP.SwitchChooseSongFlag(true);
            while (!ui3DOP.GetChooseSongFinishFlag())
                yield return null;

            ui3DOP.SwitchChooseSongFinishFlag(false);
        }

        //主題館音樂延後至此關閉
        StopSongMusic();

        m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_ChooseSong)))
            m_uiChooseSong = m_guiManager.GetGUI(typeof(UI_ChooseSong).Name) as UI_ChooseSong;
        else if (operater.m_Type.Equals(typeof(UI_3D_SongMenu)))
            m_ui3DSongMenu = m_gui3DManager.GetGUI(typeof(UI_3D_SongMenu).Name) as UI_3D_SongMenu;
        else if (operater.m_Type.Equals(typeof(UI_3D_Background)))
            m_ui3DBackground = m_gui3DManager.GetGUI(typeof(UI_3D_Background).Name) as UI_3D_Background;
        else if (operater.m_Type.Equals(typeof(UI_3D_BGEffect)))
            m_ui3DBGEffect = m_gui3DManager.GetGUI(typeof(UI_3D_BGEffect).Name) as UI_3D_BGEffect;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_gui3DManager.Initialize();
        m_uiChooseSong.InitializeUI(m_mainApp.GetStringTable(), m_gameDataDB);

        m_SwipeController = new SwipeController(m_SoundSystem, m_uiChooseSong.m_SwipeZone);
       
        //資料初始化
        SetSongList();

        //設定主頁歌曲
        Slot_Song song;
        if (m_ui3DSongMenu.GetSpecifiedSlotSong(0, out song))
            m_SwipeController.SetMainSong(song);
      
        SetCurrentSongData();
        LoadCurrentSongMusic();

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        if (m_uiChooseSong != null)
            RemoveCallBack();

        m_uiChooseSong = null;

        RealseRunTimeData();

        base.end();
        UnityDebugger.Debugger.Log("ChooseSongState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        base.suspend();
        UnityDebugger.Debugger.Log("ChooseSongState suspend");

        if (m_bIsSuspendByFullScreenState && m_uiChooseSong != null)
        {
            //CheckActiveCommonGUI();
            m_uiChooseSong.Hide();
            m_ui3DSongMenu.Hide();
            m_ui3DBGEffect.Hide();
        }
        RemoveCallBack();
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        base.resume();
        UnityDebugger.Debugger.Log("ChooseSongState resume");

        if (m_uiChooseSong != null)
        {
            //更新歌曲選單
            if (IsUpdateSongList)
            {
                SetSongList();
                SetCurrentSongData();
                IsUpdateSongList = false;
            }    

            //重新讀取語言相關UI
            m_uiChooseSong.InitializeUI(m_mainApp.GetStringTable(), m_gameDataDB);
            m_uiChooseSong.SetSongName(m_dataSystem.GetSongNameTexture(m_currentSongData));
            m_uiChooseSong.SetSongComposer(m_dataSystem.GetSongComposerName(m_currentSongData));

            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                CheckActiveCommonGUI();
                AddCallBack();
            }

            m_bIsSuspendByFullScreenState = false;
        }

        LoadCurrentSongMusic();
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }
  
    //---------------------------------------------------------------------------------------------------
    //根據玩家歌曲資料設定歌曲選單並讀取更換材質貼圖(同步讀取)
    private void SetSongList()
    {
        //Set Song Texture
        List<Slot_Song> songObjectList = m_ui3DSongMenu.m_songObjectList;
        string[] songMaterialPath = new string[songObjectList.Count];
        SongData[] songsDataArray = new SongData[songObjectList.Count];

        int middleIndex = songObjectList.Count / 2;
        int currentSongIndex = m_dataSystem.GetSongIndex(m_currentSongData.SongGroupID);

        SongData songData = m_currentSongData;
        //設定第一首前面的歌曲
        for (int i = middleIndex; i > 0; --i)
        {
            songData = m_dataSystem.GetPreSongData(songData.SongGroupID);
            songsDataArray[i - 1] = songData;
        }
        //設定第一首後面的歌曲
        for (int i = middleIndex, iCount = songObjectList.Count; i < iCount; ++i)
        {
            songData = (i == middleIndex) ? m_currentSongData : m_dataSystem.GetNextSongData(songData.SongGroupID);
            songsDataArray[i] = songData;
        }
        m_ui3DSongMenu.SetSongListUI(m_resourceManager, m_SwipeController, m_dataSystem, songsDataArray);
    }
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        //Common UI Delegate
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiTopBar.AddCallBack();
        m_uiPlayerInfo.OnButtonSettingClickEvent    = SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.OnButtonListeningClickEvent  += SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.OnButtonListeningClickEvent  += StopSongMusic;
        m_uiPlayerInfo.OnButtonMallClickEvent       = SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.OnButtonTeachClickEvent      = SetIsSuspendByFullScreenState;
        m_uiPlayerInfo.AddCallBack();

        //EasyTouch
        if (m_SwipeController != null)
        {
            EasyTouch.On_SwipeStart += m_SwipeController.OnSwipeStart;
            EasyTouch.On_Swipe += m_SwipeController.OnSwipe;
           // EasyTouch.On_SwipeEnd += m_SwipeController.OnSwipeEnd;
        }
        EasyTouch.On_TouchUp += OnTouchUP;
        EasyTouch.On_SwipeEnd += OnSwipeEnd;
        if (m_uiChooseSong != null)
        {
            UIEventListener.Get(m_uiChooseSong.m_buttonLimpid.gameObject).onClick = OnSongClick;
            UIEventListener.Get(m_uiChooseSong.m_buttonWaitForUnlock.gameObject).onClick = OnBtnWaitForUnlockClick;
#if DEVELOP
            UIEventListener.Get(m_uiChooseSong.m_tempButtonSong.gameObject).onClick = OnSongClick;
#endif
        }
    }
    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        //Common UI Delegate
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiTopBar.RemoveCallBack();
        m_uiPlayerInfo.OnButtonSettingClickEvent = null;
        m_uiPlayerInfo.OnButtonListeningClickEvent = null;
        m_uiPlayerInfo.OnButtonMallClickEvent = null;
        m_uiPlayerInfo.OnButtonTeachClickEvent = null;
        m_uiPlayerInfo.RemoveCallBack();

        //EasyTouch
        if (m_SwipeController != null)
        {
            EasyTouch.On_SwipeStart -= m_SwipeController.OnSwipeStart;
            EasyTouch.On_Swipe      -= m_SwipeController.OnSwipe;
            //EasyTouch.On_SwipeEnd   -= m_SwipeController.OnSwipeEnd;
        }
        EasyTouch.On_TouchUp -= OnTouchUP;
        EasyTouch.On_SwipeEnd -= OnSwipeEnd;
        if (m_uiChooseSong != null)
        {
            UIEventListener.Get(m_uiChooseSong.m_buttonLimpid.gameObject).onClick = null;
            UIEventListener.Get(m_uiChooseSong.m_buttonWaitForUnlock.gameObject).onClick = null;
#if DEVELOP
            UIEventListener.Get(m_uiChooseSong.m_tempButtonSong.gameObject).onClick = null;
#endif
        }
    }
    #region ButtonEvent
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        if (!isPlaying)
            return;

        StopSongMusic();
        Hashtable table = new Hashtable(); ;
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
        m_mainApp.ChangeStateByScreenShot(StateName.THEME_STATE, table);
    }
    //----------------------------------------------------------------------------------------------
    private void OnSongClick(GameObject go)
    {
        if (m_SwipeController.m_isSongSwiping)
            return;

        if (m_currentSongData.LockStatus == Enum_SongLockStatus.WaitForUnlock)
            return;
        else if (m_currentSongData.LockStatus == Enum_SongLockStatus.Lock)
        {
            ShowCurrentSongUnlockCondition();
            return;
        }

        Hashtable table = new Hashtable();
        Dictionary<Enum_SongDifficulty, SongDifficultyData> songDifficultyData = m_currentSongData.GetSongDifficultyPlayDataList();
        foreach (Enum_SongDifficulty diff in songDifficultyData.Keys)
        {
            //預設顯示該歌曲難度資料中的第一個難度
            table.Add(Enum_StateParam.SongDifficulty, diff);
            break;
        }
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_DIFFICULTY_STATE, table);
    }
    //-------------------------------------------------------------------------------------------------
    private void ShowCurrentSongUnlockCondition()
    {
        string strUnlock = null;
        List<S_Songs_Tmp> songTmpList = m_gameDataDB.m_songTmpDict[m_currentSongData.SongGroupID];
        for (int i = 0, iCount = songTmpList.Count; i < iCount; ++i)
        {
            S_SongUnlock_Tmp unlockTmp = m_mainApp.GetSystem<SongUnlockSystem>().GetUnlockCondition(songTmpList[i].GUID);
            if (unlockTmp == null)
                strUnlock = m_mainApp.GetString(45); //45: 歌曲尚未解鎖
            else
            {
                if (i > 0)
                    strUnlock += "\n";

                S_Songs_Tmp conditionSongTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(unlockTmp.iConditionSong);
                strUnlock += string.Format(m_mainApp.GetString(46),                         //"{0}:於歌曲<{1}>{2}難度達成{3}評價"
                            m_mainApp.GetString(47 + songTmpList[i].iDifficulty),           //"簡單"
                            m_mainApp.GetString(conditionSongTmp.iShowName),
                            m_mainApp.GetString(47 + (int)conditionSongTmp.iDifficulty),  //"簡單"
                            unlockTmp.iConditionRank.ToString());
            }
        }

        m_mainApp.PushIconCheckBox(m_mainApp.GetString(45), m_mainApp.GetString(19), strUnlock, null, null, null, 106); //20:"歌曲尚未解鎖", 19:"解鎖條件" ,106:解鎖系統用圖案
    }

    //-------------------------------------------------------------------------------------------------
    public void OnSwipeEnd(Gesture ges)
    {
        if (!m_SwipeController.m_isSongSwiping)
            return;

        m_SwipeController.OnSwipeEnd(ges);

        StopSongMusic();

        if (m_coroutineLoadMusic != null)
            m_mainApp.MusicApp.StopCoroutine(m_coroutineLoadMusic);

        m_coroutineLoadMusic = m_mainApp.MusicApp.StartCoroutine(UpdateDataAfterSwipe());
    }
    //-------------------------------------------------------------------------------------------------
    public void OnTouchUP(Gesture ges)
    {
        if (!m_SwipeController.m_isSongSwiping)
            return;

        //小幅度滑移當作點擊歌曲事件處理
        Vector2 subVec = ges.position - ges.startPosition;
        if (Mathf.Abs(subVec.x) > 1.0f)
            return;
        OnSongClick(m_uiChooseSong.m_buttonLimpid.gameObject);
    }
    //-------------------------------------------------------------------------------------------------
    public void OnBtnWaitForUnlockClick(GameObject go)
    {
        if (m_SwipeController.m_isSongSwiping)
            return;
        if (m_currentSongData.LockStatus == Enum_SongLockStatus.Lock)
        {
            ShowCurrentSongUnlockCondition();
            return;
        }
        if (m_currentSongData.LockStatus != Enum_SongLockStatus.WaitForUnlock)
            return;
      
        Slot_Song song;
        if (m_ui3DSongMenu.GetSpecifiedSlotSong(0, out song))
        {
            m_currentSongData.LockStatus = Enum_SongLockStatus.Unlock;
            song.UpdateData(m_resourceManager, m_dataSystem, m_currentSongData);
        }
        //TODO: 解鎖動畫
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    private void LoadCurrentSongMusic()
    {
        if (m_musicLoader != null)
        {
            m_musicLoader.CancelLoad();
            //UnityDebugger.Debugger.Log("-----------------------Cancel Load Music [" + m_musicLoader.m_strAssetName + "]");
        }

        //Async Load Music
        string songPath = m_dataSystem.GetShortSongPath(m_currentSongData);
        m_musicLoader = m_resourceManager.GetResourceASync(  Enum_ResourcesType.Songs,
                                                                                    songPath,
                                                                                    typeof(AudioClip),
                                                                                    null);
      
        m_mainApp.MusicApp.StartCoroutine(WaitForMusicLoader(m_musicLoader));
    }
    //-------------------------------------------------------------------------------------------------
    private IEnumerator WaitForMusicLoader(AsyncLoadOperation musicLoader)
    {
        yield return musicLoader;

        if (musicLoader.m_bCanacel || !isPlaying)
        {
            yield break;
        }  

        Object obj = musicLoader.m_assetObject;
        //非同步讀取失敗便嘗試用同步讀取
        if (obj == null)
        {
            obj = m_resourceManager.GetResourceSync(Enum_ResourcesType.Songs, musicLoader.m_strAssetName);
        }
        if (obj != null)
        {
            //Set Music
            AudioClip music = obj as AudioClip;
            AudioController controller = m_mainApp.GetAudioController();
            controller.SetBGM(music);

            m_mainApp.MusicApp.StartCoroutine(PlaySongMusic());
        }
        else
        {
            UnityDebugger.Debugger.Log("-----------------------Load Music [" + m_musicLoader.m_strAssetName + "] Failed !");
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
    //-------------------------------------------------------------------------------------------------
    private void SetCurrentSongData()
    {
        //Set Song Name
        m_uiChooseSong.SetSongName(m_dataSystem.GetSongNameTexture(m_currentSongData));
        //Set Song Composer
        m_uiChooseSong.SetSongComposer(m_dataSystem.GetSongComposerName(m_currentSongData));
        //Set Song Rank
        m_uiChooseSong.ShowSongRank(m_currentSongData);
        //Set Song Background
        m_ui3DBackground.SetBackground(m_dataSystem.GetSongBgResourcePath(m_currentSongData));
        //Set New Song Sprite
        m_uiChooseSong.SwitchNewSong(m_currentSongData.IsNewSong);
        //Set Original Song Sprite   
        m_uiChooseSong.SwitchOriginalSong(m_currentSongData.IsOriginalSong);
#if DEVELOP
        //Set Song Group ID
        m_uiChooseSong.SetSongGroupID(m_currentSongData.SongGroupID);
#endif
    }
    //-------------------------------------------------------------------------------------------------
    private IEnumerator UpdateDataAfterSwipe()
    {
        m_ui3DSongMenu.UpdateSongListData(m_dataSystem, m_resourceManager);
        Slot_Song song;
        if (m_ui3DSongMenu.GetSpecifiedSlotSong(0, out song))
        {
            m_currentSongData = m_dataSystem.GetSongDataByGroupID(song.m_iSongGroup);
            SetCurrentSongData();
            m_SwipeController.SetMainSong(song);
        }

        //關閉滑移中旗標
        m_SwipeController.m_isSongSwiping = false;

        yield return new WaitForSeconds(m_ui3DSongMenu.m_WaitPlaySongTime);
        if (!isPlaying)
            yield break;

        LoadCurrentSongMusic();

        m_coroutineLoadMusic = null;
    }
    //---------------------------------------------------------------------------------------------------
    private string[] GetDelayDeleteGUIName()
    {
        return new string[] {   typeof(UI_ChooseSong).Name,
                                typeof(UI_3D_SongMenu).Name,
                                typeof(UI_3D_BGEffect).Name,
                                typeof(UI_3D_Background).Name };
    }
    //---------------------------------------------------------------------------------------------------
    private void RealseRunTimeData()
    {
        m_SwipeController.ReleaseData();
        m_SwipeController = null;
        IsUpdateSongList = false;

        if (m_musicLoader != null)
            m_musicLoader.CancelLoad();
        m_musicLoader = null;
        m_coroutineLoadMusic = null;
    }
}

