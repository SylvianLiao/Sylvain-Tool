using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using HedgehogTeam.EasyTouch;

public enum Enum_MusicPlayMode
{
    None,
    OneLoop,
    AllLoop,
    Random,
}
public enum Enum_MusicPages
{
    AllSong,
    Favorite,
}
public class MusicListeningState : CustomBehaviorState
{
    #region Music Player
    /// <summary>音樂聆聽用的音樂播放器(配合UI控制)</summary>
    public class MusicPlayer
    {
        private UI_MusicPlayer      m_uiMusicPlayer;
        private GameDataDB          m_gameDataDB;
        private RecordSystem        m_recordSystem;
        private AudioController     m_audioController;
        private Camera              m_camera;                               //NGUI的Camera

        //RunTimeData
        private Enum_MusicPlayMode  m_playMode;
        /// <summary>玩家是否正在控制進度條按鈕</summary>
        public bool                 m_bIsPlayerControlProgress;             
        /// <summary>玩家控制進度條按鈕時音樂是否正在播放 </summary>
        private bool                m_bIsPlayingWhenProgressCotrolled;
        private AudioController.BGMPlayFinishEvent OnBGMPlayFinish;
        public MusicPlayer() {}
        //---------------------------------------------------------------------------------------------------
        public void Initialize( UICamera camera, 
                                UI_MusicPlayer ui, 
                                GameDataDB gameDB, 
                                RecordSystem recordSys,
                                AudioController ac, 
                                AudioController.BGMPlayFinishEvent callback)
        {
            m_camera = camera.GetComponent<Camera>(); ;
            m_uiMusicPlayer = ui;
            m_uiMusicPlayer.Initialize();

            m_gameDataDB = gameDB;
            m_recordSystem = recordSys;
            m_audioController = ac;

            m_playMode = Enum_MusicPlayMode.None;
            m_bIsPlayerControlProgress = false;
            m_bIsPlayingWhenProgressCotrolled = false;

            //停止先前的音樂
            StopMusic();
            SetSongMusic(null);

            OnBGMPlayFinish = callback;

            SetPlayMode(m_recordSystem.GetMusicPlayMode());
        }
        //---------------------------------------------------------------------------------------------------
        public void AddCallBack()
        {
            UIEventListener.Get(m_uiMusicPlayer.m_buttonOneLoop.gameObject).onClick = OnButtonOneLoopClick;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonAllLoop.gameObject).onClick = OnButtonAllLoopClick;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonRandom.gameObject).onClick = OnButtonRandomClick;
            m_audioController.OnBGMPlayFinish += OnBGMPlayFinish;
            AddMusicControlCallBack();
        }
        //---------------------------------------------------------------------------------------------------
        public void RemoveCallBack()
        {
            UIEventListener.Get(m_uiMusicPlayer.m_buttonOneLoop.gameObject).onClick = null;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonAllLoop.gameObject).onClick = null;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonRandom.gameObject).onClick = null;
            m_audioController.OnBGMPlayFinish -= OnBGMPlayFinish;
            RemoveMusicControlCallBack();
        }
        //---------------------------------------------------------------------------------------------------
        public void AddMusicControlCallBack()
        {
            UIEventListener.Get(m_uiMusicPlayer.m_buttonPlay.gameObject).onClick = OnButtonPlayClick;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonPause.gameObject).onClick = OnButtonPlayClick;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonProgressLine.gameObject).onClick = OnButtonProgressClick;
            EasyTouch.On_Swipe += OnSwipeButtonProgress;
            EasyTouch.On_SwipeEnd += OnSwipeButtonProgressEnd;
        }
        //---------------------------------------------------------------------------------------------------
        public void RemoveMusicControlCallBack()
        {
            UIEventListener.Get(m_uiMusicPlayer.m_buttonPlay.gameObject).onClick = null;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonPause.gameObject).onClick = null;
            UIEventListener.Get(m_uiMusicPlayer.m_buttonProgressLine.gameObject).onClick = null;
            EasyTouch.On_Swipe -= OnSwipeButtonProgress;
            EasyTouch.On_SwipeEnd -= OnSwipeButtonProgressEnd;
        }
        public void Update()
        {
            if (m_uiMusicPlayer == null)
                return;

            UpdateMusicProgress();
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>玩家控制音樂進度</summary>
        public void ControlMusicProgress(Gesture gesture)
        {
            m_bIsPlayerControlProgress = true;

            if (m_audioController.BGMIsPlay())
            {
                m_bIsPlayingWhenProgressCotrolled = true;
                PauseMusic();
            }

            m_uiMusicPlayer.SetProgressByButton(gesture.deltaPosition.x, m_audioController.GetBGMLength());

            float timeRatio = m_uiMusicPlayer.GetProgressLineRatio();
            float time = timeRatio * m_audioController.GetBGMLength();
            m_audioController.SetBGMPlayTime(time);

            //UnityUnityDebugger.Debuggerger.UnityDebugger.Debuggerger.Log("---------------------deltaVector = " + gesture.deltaPosition);
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>停止控制音樂進度</summary>
        public void StopControlMusicProgress()
        {
            if (m_bIsPlayingWhenProgressCotrolled)
                PlayMusic(false);

            m_bIsPlayerControlProgress = false;
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>更新音樂播放進度</summary>
        private void UpdateMusicProgress()
        {
            if (m_bIsPlayerControlProgress || !MusicIsPlaying())
                return;

            m_uiMusicPlayer.SetProgressByMusic(m_audioController.GetBGMPlayTime(), m_audioController.GetBGMLength()); 
        }
    
        //-------------------------------------------------------------------------------------------------
        public void SetPlayMode(Enum_MusicPlayMode mode)
        {
            m_playMode = mode;
            int oneLoopTextureID = 0;
            int allLoopTextureID = 0;
            int randomTextureID = 0;
            switch (m_playMode)
            {
                case Enum_MusicPlayMode.OneLoop:
                    oneLoopTextureID = 80;
                    allLoopTextureID = 83;
                    randomTextureID = 81;
                    break;
                case Enum_MusicPlayMode.AllLoop:
                    oneLoopTextureID = 79;
                    allLoopTextureID = 84;
                    randomTextureID = 81;
                    break;
                case Enum_MusicPlayMode.Random:
                    oneLoopTextureID = 79;
                    allLoopTextureID = 83;
                    randomTextureID = 82;
                    break;
                case Enum_MusicPlayMode.None:
                default:
                    oneLoopTextureID = 79;
                    allLoopTextureID = 83;
                    randomTextureID = 81;
                    break;              
            }

            m_uiMusicPlayer.ChangeButtonOneLoop(oneLoopTextureID);
            m_uiMusicPlayer.ChangeButtonAllLoop(allLoopTextureID);
            m_uiMusicPlayer.ChangeButtonRandom(randomTextureID);
            m_recordSystem.SetMusicPlayMode(mode);
        }
        //-------------------------------------------------------------------------------------------------
        public Enum_MusicPlayMode GetPlayMode()
        {
            return m_playMode;
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>音樂是否正在播放</summary>
        public bool MusicIsPlaying()
        {
            return m_audioController.BGMIsPlay();
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>播放音樂</summary>
        public void PlayMusic(bool forcePlay)
        {
            m_audioController.PlayBGM(null, forcePlay);
            ChangePlayButtonSprite(true);
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>暫停音樂</summary>
        public void PauseMusic()
        {
            m_audioController.PauseBGM();
            ChangePlayButtonSprite(false);
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>停止音樂</summary>
        public void StopMusic()
        {
            m_audioController.StopBGM();
            m_audioController.SetBGMPlayTime(0.0f);
            ChangePlayButtonSprite(false);
            m_uiMusicPlayer.SetProgressByMusic(0.0f, 0.0f);
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>設定音樂曲目</summary>
        public void SetSongMusic(AudioClip clip)
        {
            m_audioController.SetBGM(clip);
            if (clip != null)
                PlayMusic(true);
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>設定音樂循環</summary>
        public void SetMusicLoop(bool isLoop)
        {
            m_audioController.SetBGMLoop(isLoop);
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>切換播放/暫停按鈕</summary>
        public void ChangePlayButtonSprite(bool isPlaying)
        {
            m_uiMusicPlayer.m_buttonPlay.gameObject.SetActive(!isPlaying);
            m_uiMusicPlayer.m_buttonPause.gameObject.SetActive(isPlaying);
        }
        //-------------------------------------------------------------------------------------------------
        //監聽事件
        public void OnButtonOneLoopClick(GameObject go)
        {
            Enum_MusicPlayMode mode = (m_playMode == Enum_MusicPlayMode.OneLoop)
                                            ? Enum_MusicPlayMode.None
                                            : Enum_MusicPlayMode.OneLoop;
            SetPlayMode(mode);
        }
        public void OnButtonAllLoopClick(GameObject go)
        {
            Enum_MusicPlayMode mode = (m_playMode == Enum_MusicPlayMode.AllLoop) 
                                            ? Enum_MusicPlayMode .None 
                                            : Enum_MusicPlayMode.AllLoop;
            SetPlayMode(mode);        
        }
        public void OnButtonPlayClick(GameObject go)
        {
            if (MusicIsPlaying()) PauseMusic();
            else PlayMusic(false);
        }
        public void OnButtonRandomClick(GameObject go)
        {
            Enum_MusicPlayMode mode = (m_playMode == Enum_MusicPlayMode.Random) ? Enum_MusicPlayMode.None : Enum_MusicPlayMode.Random;
            SetPlayMode(mode);
        }
        public void OnButtonProgressClick(GameObject go)
        {
            Vector3 buttonScreenPos = m_camera.WorldToScreenPoint(m_uiMusicPlayer.m_spriteProgressLine.transform.position);
            float deltaMove = Input.mousePosition.x - buttonScreenPos.x;
            m_uiMusicPlayer.SetProgressByButton(deltaMove, m_audioController.GetBGMLength());

            float timeRatio = m_uiMusicPlayer.GetProgressLineRatio();
            float time = timeRatio * m_audioController.GetBGMLength();
            m_audioController.SetBGMPlayTime(time);
        }
        public void OnSwipeButtonProgress(Gesture gesture)
        {
            GameObject target = gesture.GetCurrentPickedObject();
            if (target == null)
                return;
            else if (!target.Equals(m_uiMusicPlayer.m_buttonProgressLine.gameObject))
                return;

            ControlMusicProgress(gesture);
        }
        public void OnSwipeButtonProgressEnd(Gesture gesture)
        {
            GameObject target = gesture.GetCurrentPickedObject();
            if (target == null)
                return;
            else if (!target.Equals(m_uiMusicPlayer.m_buttonProgressLine.gameObject))
                return;

            StopControlMusicProgress();
        }
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    private UI_MusicListening   m_uiMusicListening;
    private UI_CommonBackground m_uiCommonBackground;

    private ResourceManager     m_resourceManager;
    private GameDataDB          m_gameDataDB;
    private PlayerDataSystem    m_playerDataSystem;
    private RecordSystem        m_recordSystem;

    //RunTime Data
    private bool                m_bIsInitFinish;            //初始化是否完成
    private SongData            m_currentChosenSongData;    //玩家目前選擇的曲目資料

    private MusicPlayer         m_musicPlayer;              //音樂播放器(結合UI)
    private Enum_MusicPages     m_currentShowPlayList;      //目前顯示哪個播放清單
    private List<SongData>      m_unlockSongDataList;       //完全解鎖的歌曲資料
    private List<SongData>      m_favorSongDataList;        //最愛的歌曲資料
    private List<SongData>      m_currentSongDataList
    {
        get{return (m_currentShowPlayList == Enum_MusicPages.AllSong) ? m_unlockSongDataList : m_favorSongDataList;}
    }

    //處理歌曲相關資源
    private List<AsyncLoadOperation>    m_songMusicLoader;
    private List<AsyncLoadOperation>    m_songTextureLoader;
    private List<Coroutine>     m_LoadSongResourceCoroutine;
    private bool                m_bIsMusicLoadFinish;
    private bool                m_bIsTextureLoadFinish;
    //---------------------------------------------------------------------------------------------------
    public MusicListeningState(GameScripts.GameFramework.GameApplication app) : base(StateName.MUSIC_LISTENING_STATE, StateName.MUSIC_LISTENING_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_playerDataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_recordSystem = m_mainApp.GetSystem<RecordSystem>();

        m_LoadSongResourceCoroutine = new List<Coroutine>();
        m_songMusicLoader = new List<AsyncLoadOperation>();
        m_songTextureLoader = new List<AsyncLoadOperation>();
        m_unlockSongDataList = new List<SongData>();
        m_favorSongDataList = new List<SongData>();
        m_bIsInitFinish = false;
        m_currentChosenSongData = null;
        m_bIsMusicLoadFinish = true;
        m_bIsTextureLoadFinish = true;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("MusicListeningState begin");

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_MusicListening));
        base.SetGUIType(typeof(UI_CommonBackground));
        AsyncLoadOperation loader = m_resourceManager.GetResourceASync(Enum_ResourcesType.GUI, typeof(Slot_SongMusic).Name, typeof(Slot_SongMusic), null);
        m_guiLoadOperator.Add(loader);
        userData.Add(Enum_StateParam.UseTopBarUI, true);
        base.begin();

        if (m_bIsAync == false)
        {
            m_uiMusicListening = m_guiManager.AddGUI<UI_MusicListening>(typeof(UI_MusicListening).Name);
            m_uiCommonBackground = m_guiManager.AddGUI<UI_CommonBackground>(typeof(UI_CommonBackground).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        if (operater.m_assetObject == null)
        {
            UnityDebugger.Debugger.Log("Load GUI[" + operater.m_strAssetName + "] With Async Is Failed!!");
            return;
        }

        if (operater.m_Type.Equals(typeof(UI_MusicListening)))
            m_uiMusicListening = m_guiManager.GetGUI(typeof(UI_MusicListening).Name) as UI_MusicListening;
        else if (operater.m_Type.Equals(typeof(UI_CommonBackground)))
            m_uiCommonBackground = m_guiManager.GetGUI(typeof(UI_CommonBackground).Name) as UI_CommonBackground;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        //"音樂聆聽", "最愛曲目", "所有曲目"
        m_uiCommonBackground.InitializeUI(m_mainApp.GetString(65), new List<string> { m_mainApp.GetString(67), m_mainApp.GetString(66) });
        //音樂播放器初始化
        m_musicPlayer = new MusicPlayer();
        m_musicPlayer.Initialize(m_guiManager.m_uiCamera,
                                    m_uiMusicListening.m_uiMusicPlayer,
                                    m_gameDataDB,
                                    m_recordSystem,
                                    m_mainApp.GetAudioController(),
                                    MusicPlayModeBehavior);

        //取得歌曲資料
        GetAllTrueUnlockSong();
        GetFavoriteSong();

        //WrapContent初始化並預設顯示所有曲目清單
        m_currentShowPlayList = Enum_MusicPages.AllSong;
        InitWrapContent();
        m_uiMusicListening.m_wcSongMenu.onInitializeItem += AssignSongDataToWrapContent;
        m_uiMusicListening.m_wcSongMenu.onInitializeItem += SetDefaultSong;
        m_uiMusicListening.m_scrollSongMenu.enabled = m_currentSongDataList.Count > UI_MusicListening.m_iEachPageSongCount;

        m_mainApp.MusicApp.StartCoroutine(CheckStateInitFinish());
    }
    //---------------------------------------------------------------------------------------------------
    public IEnumerator CheckStateInitFinish()
    {
        yield return m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
        while (m_LoadSongResourceCoroutine.Count > 0)
            yield return null;

        yield return m_LoadSongResourceCoroutine;
        m_bIsInitFinish = true;
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        if (m_uiMusicListening != null)
            RemoveCallBack();
        m_uiMusicListening = null;

        if (m_musicPlayer != null)
        {
            m_musicPlayer.StopMusic();
        }
        m_musicPlayer = null;

        RealseRunTimeData();

        m_guiManager.DeleteGUI(typeof(UI_MusicListening).Name);
        m_guiManager.DeleteGUI(typeof(UI_CommonBackground).Name);
        base.end();
        UnityDebugger.Debugger.Log("MusicListeningState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        if (m_bIsSuspendByFullScreenState && m_uiMusicListening != null)
        {
            CheckActiveCommonGUI();
            m_uiMusicListening.Hide();
        }
        RemoveCallBack();
        base.suspend();
        UnityDebugger.Debugger.Log("MusicListeningState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        if (m_uiMusicListening != null)
        {
            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                ShowBeSetGUI();
                AddCallBack();
            }

            m_bIsSuspendByFullScreenState = false;
        }
        base.resume();
        UnityDebugger.Debugger.Log("MusicListeningState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();

        if (m_musicPlayer != null)
            m_musicPlayer.Update();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiTopBar.AddCallBack();
        m_uiCommonBackground.AddCallBack();

        for (int i = 0, iCount = m_uiMusicListening.m_slotSongObjList.Count; i < iCount; ++i)
        {
            UIEventListener.Get(m_uiMusicListening.m_slotSongObjList[i].gameObject).onClick = OnButtonSongClick;
            UIEventListener.Get(m_uiMusicListening.m_slotSongObjList[i].m_buttonStar.gameObject).onClick = OnButtonStarClick;
        }

        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_MusicPages.Favorite].gameObject).onClick += OnButtonAllSongClick;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_MusicPages.AllSong].gameObject).onClick += OnButtonFavoriteClick;
        m_musicPlayer.AddCallBack();
        UIEventListener.Get(m_uiMusicListening.m_uiMusicPlayer.m_buttonPreSong.gameObject).onClick = OnButtonPreSongClick;
        UIEventListener.Get(m_uiMusicListening.m_uiMusicPlayer.m_buttonNextSong.gameObject).onClick = OnButtonNextSongClick;
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiTopBar.RemoveCallBack();
        m_uiCommonBackground.RemoveCallBack();

        for (int i = 0, iCount = m_uiMusicListening.m_slotSongObjList.Count; i < iCount; ++i)
        {
            UIEventListener.Get(m_uiMusicListening.m_slotSongObjList[i].gameObject).onClick = null;
            UIEventListener.Get(m_uiMusicListening.m_slotSongObjList[i].m_buttonStar.gameObject).onClick = null;
        }

        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_MusicPages.Favorite].gameObject).onClick = null;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_MusicPages.AllSong].gameObject).onClick = null;
        m_musicPlayer.RemoveCallBack();
        UIEventListener.Get(m_uiMusicListening.m_uiMusicPlayer.m_buttonPreSong.gameObject).onClick = null;
        UIEventListener.Get(m_uiMusicListening.m_uiMusicPlayer.m_buttonNextSong.gameObject).onClick = null;
    }
    #region ButtonEvents
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        if (!isPlaying)
            return;

        m_mainApp.PopStateByScreenShot();
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonSongClick(GameObject go)
    {
        Slot_SongMusic slot = go.GetComponent<Slot_SongMusic>();
        if (slot == null)
            return;
        if (slot.m_songData == null)
            return;

        m_currentChosenSongData = slot.m_songData;

        //讀取音樂之前先將原本播放之音樂清空
        m_musicPlayer.StopMusic();
        m_musicPlayer.SetSongMusic(null);

        m_LoadSongResourceCoroutine.Add(m_mainApp.MusicApp.StartCoroutine(LoadSongResource(m_currentChosenSongData)));

        SetChosenSongUI(m_currentChosenSongData);
        MoveHighLight(go);

        //記錄歌曲清單選擇的曲目
        switch (m_currentShowPlayList)
        {
            case Enum_MusicPages.AllSong: m_recordSystem.SetAllSongChosenID(m_currentChosenSongData); break;
            case Enum_MusicPages.Favorite: m_recordSystem.SetFavoriteChosenID(m_currentChosenSongData); break;
            default: break;
        }    
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonStarClick(GameObject go)
    {
        Slot_SongMusic slot = go.GetComponentInParent<Slot_SongMusic>();
        if (slot == null)
            return;
        if (slot.m_songData == null)
            return;

        bool isFavor = !m_recordSystem.GetSongIsFavorite(slot.m_songData);
        m_recordSystem.SetSongIsFavorite(slot.m_songData, isFavor);
        slot.SetStar(GetStarSpriteName(isFavor)); 
        slot.SetBackground(GetSlotBgSpriteName(isFavor));

        if (isFavor)
            m_favorSongDataList.Add(slot.m_songData);
        else if (m_favorSongDataList.Contains(slot.m_songData))
            m_favorSongDataList.Remove(slot.m_songData);
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonAllSongClick(GameObject go)
    {
        m_musicPlayer.StopMusic();

        m_currentShowPlayList = Enum_MusicPages.AllSong;

        m_uiMusicListening.OnFadeOutFinish.Add(ReloadUI);
        m_uiMusicListening.OnFadeOutFinish.Add(m_uiMusicListening.FadeIn);
        m_uiMusicListening.FadeOut();
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonFavoriteClick(GameObject go)
    {
        m_musicPlayer.StopMusic();

        m_currentShowPlayList = Enum_MusicPages.Favorite;

        m_uiMusicListening.OnFadeOutFinish.Add(ReloadUI);
        m_uiMusicListening.OnFadeOutFinish.Add(m_uiMusicListening.FadeIn);
        m_uiMusicListening.FadeOut();
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonPreSongClick(GameObject go)
    {
        int index = m_currentSongDataList.IndexOf(m_currentChosenSongData) - 1;
        bool play = m_musicPlayer.MusicIsPlaying();
        if (!MoveToSongAndPlay(index, play))
        {
            if (play)
                m_musicPlayer.PlayMusic(true);
        }
    }
    public void OnButtonNextSongClick(GameObject go)
    {
        int index = m_currentSongDataList.IndexOf(m_currentChosenSongData) + 1;
        bool play = m_musicPlayer.MusicIsPlaying();
        if (!MoveToSongAndPlay(index, play))
        {
            if (play)
                m_musicPlayer.PlayMusic(true);
        }
    }
    #endregion
    #region Data Setting
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得所有完全解鎖的歌曲資料</summary>
    private void GetAllTrueUnlockSong()
    {
        List<SongData> songList = m_playerDataSystem.GetCurrentThemeData().GetSongDataList();
        for (int i = 0, iCount = songList.Count; i < iCount; ++i)
        {
            if (songList[i].LockStatus == Enum_SongLockStatus.Unlock)
            {
                m_unlockSongDataList.Add(songList[i]);
            }  
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得被設為最愛的歌曲資料</summary>
    private void GetFavoriteSong()
    {
        for (int i = 0, iCount = m_unlockSongDataList.Count; i < iCount; ++i)
        {
            if (m_recordSystem.GetSongIsFavorite(m_unlockSongDataList[i]))
            {
                m_favorSongDataList.Add(m_unlockSongDataList[i]);
            }
        }  
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>生成WrapContent物件，並做基本初始化</summary>
    private void InitWrapContent()
    {
        GameObject slotObj = null;
        foreach (var loader in m_guiLoadOperator)
        {
            if (loader.m_Type.Equals(typeof(Slot_SongMusic)))
            {
                slotObj = loader.m_assetObject as GameObject;
                break;
            }
        }
        m_uiMusicListening.InitWrapContentValue(m_currentSongDataList.Count);
        m_uiMusicListening.CreateSlotSongMusic(slotObj);
    }

    //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// WrapContent塞資料的delegate Function
    /// </summary>
    /// <param name="go"></param>
    /// <param name="wrapIndex">WrapContent物件Index</param>
    /// <param name="realIndex">資料Index</param>
    private void AssignSongDataToWrapContent(GameObject song, int wrapIndex, int realIndex)
    {
        Slot_SongMusic slotSongMusic = song.GetComponent<Slot_SongMusic>();
        realIndex = Mathf.Abs(realIndex);
        //沒有資料可塞給物件時則清除物件上的資料並關閉
        if (realIndex > m_currentSongDataList.Count - 1)
        {
            slotSongMusic.Hide();
            slotSongMusic.ClearData();
            return;
        }

        SongData songData = m_currentSongDataList[realIndex];
        if (songData == null)
            return;

        //將完全解鎖的歌曲資料Assign給Slot實體
        bool isFavor = m_recordSystem.GetSongIsFavorite(songData);
        slotSongMusic.SetData(songData, m_playerDataSystem, GetStarSpriteName(isFavor), GetSlotBgSpriteName(isFavor));

        //若為玩家選擇之曲目則高亮
        if (m_currentChosenSongData != null && m_currentChosenSongData.SongGroupID == songData.SongGroupID)
        {
            MoveHighLight(slotSongMusic.gameObject);
        }
        //若不是，且自己擁有高亮框則關閉高亮
        else if (m_uiMusicListening.IsOwnHighLightSongObj(slotSongMusic.gameObject))
        {
            m_uiMusicListening.m_spriteHighLight.gameObject.SetActive(false);
        }

        slotSongMusic.Show();
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>設定預設歌曲</summary>
    private void SetDefaultSong(GameObject song, int wrapIndex, int realIndex)
    {
        //等待WrapContent塞完最後一筆資料後繼續
        if (wrapIndex != m_uiMusicListening.m_slotSongObjList.Count - 1)
            return;

        //若無清單中無歌曲則清空目前歌曲資料&UI
        if (m_currentSongDataList.Count == 0)
        {
            ClearSongDataAndUI();
            //記錄歌曲清單選擇的曲目
            switch (m_currentShowPlayList)
            {
                case Enum_MusicPages.AllSong: m_recordSystem.SetAllSongChosenID(null); break;
                case Enum_MusicPages.Favorite: m_recordSystem.SetFavoriteChosenID(null); break;
                default: break;
            }
            return;
        }

        //取得記錄選取的曲目ID
        int defaultSongID = (m_currentShowPlayList == Enum_MusicPages.AllSong) 
                            ? m_recordSystem.GetAllSongChosenID()
                            : m_recordSystem.GetFavoriteChosenID();

        //若沒有記錄的曲目或是紀錄的曲目不在歌曲清單內則預設選擇第一首歌曲
        if (defaultSongID < 0 || !CheckSongInPlayList(defaultSongID))
        {
            int songGroupID = m_currentSongDataList[0].SongGroupID;
            Slot_SongMusic slot = m_uiMusicListening.GetSlotSongMusicByGroupID(songGroupID);
            MoveHighLight(slot.gameObject);
            m_currentChosenSongData = slot.m_songData;
        }
        else
        {
            Slot_SongMusic slot = m_uiMusicListening.GetSlotSongMusicByGroupID(defaultSongID);
            //若記錄選取的曲目在第一頁內則高亮
            if (slot != null)
                MoveHighLight(slot.gameObject);
            m_currentChosenSongData = m_playerDataSystem.GetSongDataByGroupID(defaultSongID);
        }

        m_LoadSongResourceCoroutine.Add(m_mainApp.MusicApp.StartCoroutine(LoadSongResource(m_currentChosenSongData)));
        SetChosenSongUI(m_currentChosenSongData);

        m_uiMusicListening.m_wcSongMenu.onInitializeItem -= SetDefaultSong;
    }

    //-------------------------------------------------------------------------------------------------
    /// <summary>讀取讀取玩家選擇曲目之資源</summary>
    private IEnumerator LoadSongResource(SongData songData)
    {
        bool loadMusicSync = LoadSongMusic(songData);
        bool loadTextureSync = LoadSongTexture(songData);
        //若有其中一個資源用非同步讀取便顯示loading UI且移除音樂控制的監聽事件
        if (!loadMusicSync || !loadTextureSync)
        {
            m_uiMusicListening.SwitchLoadingUI(true);
            m_musicPlayer.RemoveMusicControlCallBack();
        }

        if (loadMusicSync && loadTextureSync)
            yield break;

        while(!m_bIsMusicLoadFinish || !m_bIsTextureLoadFinish)
            yield return null;

        if (!isPlaying)
            yield break;

        if (m_LoadSongResourceCoroutine.Count > 0)
        {
            foreach(Coroutine co in m_LoadSongResourceCoroutine)
            {
                if (co != null)
                    m_mainApp.MusicApp.StopCoroutine(co);
            }
            m_LoadSongResourceCoroutine.Clear();
        }

        m_uiMusicListening.SwitchLoadingUI(false);
        if (m_bIsInitFinish)
            m_musicPlayer.AddMusicControlCallBack();
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>讀取讀取玩家選擇曲目之音樂(回傳讀取方式: True=同步, False=非同步)</summary>
    private bool LoadSongMusic(SongData songData)
    {
        string songPath = m_playerDataSystem.GetSongPath(songData);
        if (m_resourceManager.CheckLoaderResource(Enum_ResourcesType.Songs, songPath))
        {
            AudioClip audio = m_resourceManager.GetResourceSync<AudioClip>(Enum_ResourcesType.Songs, songPath);
            m_musicPlayer.SetSongMusic(audio);
            m_bIsMusicLoadFinish = true;
            return true;
        }
        else
        {
            m_bIsMusicLoadFinish = false;

            if (m_songMusicLoader.Count > GameDefine.MUSCIC_LISTENING_LOADER_MAX)
            {
                m_songMusicLoader[0].CancelLoad();
                m_songMusicLoader.RemoveAt(0);
            }

            AsyncLoadOperation loader = m_resourceManager.GetResourceASync(Enum_ResourcesType.Songs, songPath, typeof(AudioClip), SetMusicWhenLoadFinish);
            m_songMusicLoader.Add(loader);
            return false;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>讀取讀取玩家選擇曲目之貼圖(回傳讀取方式: True=同步, False=非同步)</summary>
    private bool LoadSongTexture(SongData songData)
    {
        string texturePath = m_playerDataSystem.GetSongTexturePath(songData);
        if (m_resourceManager.CheckLoaderResource(Enum_ResourcesType.Texture, texturePath))
        {
            Texture texture = m_resourceManager.GetResourceSync<Texture>(Enum_ResourcesType.Texture, texturePath);
            m_uiMusicListening.SetSongTexture(texture);
            m_bIsTextureLoadFinish = true;
            return true;
        }
        else
        {
            m_bIsTextureLoadFinish = false;

            if (m_songTextureLoader.Count > GameDefine.MUSCIC_LISTENING_LOADER_MAX)
            {
                m_songTextureLoader[0].CancelLoad();
                m_songTextureLoader.RemoveAt(0);
            }

            AsyncLoadOperation loader = m_resourceManager.GetResourceASync(Enum_ResourcesType.Texture, texturePath, typeof(Texture), SetTextureWhenLoadFinish);
            m_songTextureLoader.Add(loader);
            return false;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>當讀取完成後自動設定曲目之音樂</summary>
    private void SetMusicWhenLoadFinish(AsyncLoadOperation loader)
    {
        if (loader.m_assetObject == null)
        {
            UnityDebugger.Debugger.Log("SetMusicWhenLoadFinish Failed------------------ Song Path = " + loader.m_strAssetName);
            return;
        }

        if (m_playerDataSystem.GetSongPath(m_currentChosenSongData).Equals(loader.m_strAssetName))
        {
            m_musicPlayer.SetSongMusic((AudioClip)loader.m_assetObject);
            m_bIsMusicLoadFinish = true;
        }
        m_songMusicLoader.Remove(loader);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>當讀取完成後自動設定曲目之貼圖</summary>
    private void SetTextureWhenLoadFinish(AsyncLoadOperation loader)
    {
        if (loader.m_assetObject == null)
        {
            UnityDebugger.Debugger.Log("SetTextureWhenLoadFinish Failed------------------ Texture Path = " + loader.m_strAssetName);
            return;
        }

        //Set Texture
        if (m_playerDataSystem.GetSongTexturePath(m_currentChosenSongData).Equals(loader.m_strAssetName))
        {
            Texture texture = loader.m_assetObject as Texture;
            m_uiMusicListening.SetSongTexture(texture);
            m_bIsTextureLoadFinish = true;
        }
        m_songTextureLoader.Remove(loader);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>清空目前歌曲資料和顯示相關資訊的UI</summary>
    private void ClearSongDataAndUI()
    {
        m_currentChosenSongData = null;
        SetChosenSongUI(null);
        m_musicPlayer.SetSongMusic(null);
        S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(58);   //專案空白圖
        Texture texture = m_resourceManager.GetResourceSync<Texture>(Enum_ResourcesType.Texture, textureTmp.strTextureName);
        m_uiMusicListening.SetSongTexture(texture);
    }
    #endregion
    #region UI Setting
    //-------------------------------------------------------------------------------------------------
    /// <summary>設定玩家選擇曲目之UI</summary>
    private void SetChosenSongUI(SongData songData)
    {
        string str = (songData == null) ? "" : m_playerDataSystem.GetSongNameTexture(songData);
        m_uiMusicListening.SetSongName(str);
        str = (songData == null) ? "" : m_playerDataSystem.GetSongComposerName(songData);
        m_uiMusicListening.SetSongComposer(str);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>移動高亮框</summary>
    private void MoveHighLight(GameObject go)
    {
        m_uiMusicListening.m_spriteHighLight.transform.parent = go.transform;
        m_uiMusicListening.m_spriteHighLight.transform.localPosition = Vector3.zero;
        m_uiMusicListening.m_spriteHighLight.gameObject.SetActive(true);
    }
    /// <summary>重新讀取UI</summary>-------------------------------------------------------------------------------------------------
    private void ReloadUI()
    {
        m_uiMusicListening.InitWrapContentValue(m_currentSongDataList.Count);
        m_uiMusicListening.m_wcSongMenu.onInitializeItem += SetDefaultSong;
        m_uiMusicListening.SortWrapContent();
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    private void RealseRunTimeData()
    {
        m_unlockSongDataList.Clear();
        m_favorSongDataList.Clear();
        m_bIsInitFinish = false;
        m_currentChosenSongData = null;
        m_bIsMusicLoadFinish = true;
        m_bIsTextureLoadFinish = true;

        m_LoadSongResourceCoroutine.Clear();
        if (m_songMusicLoader != null)
        {
            foreach (var loader in m_songMusicLoader)
            {
                loader.CancelLoad();
            }
            m_songMusicLoader.Clear();
        }
        if (m_songTextureLoader != null)
        {
            foreach (var loader in m_songTextureLoader)
            {
                loader.CancelLoad();
            }
            m_songTextureLoader.Clear();
        }
    }
    //-------------------------------------------------------------------------------------------------
    private string GetStarSpriteName(bool isFavorite)
    {
        int id = 0;
        id = (isFavorite)?85:86;    //實心愛心，空心愛心

        S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(id);
        if (textureTmp == null)
            return null;

        return textureTmp.strSpriteName;
    }
    //-------------------------------------------------------------------------------------------------
    private string GetSlotBgSpriteName(bool isFavorite)
    {
        int id = 0;
        id = (isFavorite) ? 88 : 87;    //最愛曲目背景，一般背景

        S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(id);
        if (textureTmp == null)
            return null;

        return textureTmp.strSpriteName;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>播放模式的實際行為</summary>
    private void MusicPlayModeBehavior()
    {
        int currentSongDataIndex = m_currentSongDataList.IndexOf(m_currentChosenSongData);
        switch (m_musicPlayer.GetPlayMode())
        {
            case Enum_MusicPlayMode.OneLoop:
                m_musicPlayer.SetMusicLoop(true);
                m_musicPlayer.PlayMusic(false);
                break;
            case Enum_MusicPlayMode.AllLoop:
                m_musicPlayer.SetMusicLoop(false);
                int index = currentSongDataIndex + 1;
                if (!MoveToSongAndPlay(index, true))
                    MoveToSongAndPlay(0, true);
                break;
            case Enum_MusicPlayMode.Random:
                m_musicPlayer.SetMusicLoop(false);
                //隨機歌曲
                int randomIndex = Random.Range(0, m_currentSongDataList.Count - 1);
                //若隨機出的歌曲跟目前已選歌曲一樣則重新隨機
                while (randomIndex == currentSongDataIndex)
                {
                    randomIndex = Random.Range(0, m_currentSongDataList.Count - 1);
                }
                MoveToSongAndPlay(randomIndex, true);
                break;
            case Enum_MusicPlayMode.None:
            default:
                m_musicPlayer.SetMusicLoop(false);
                m_musicPlayer.StopMusic();
                break;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>移動至指定歌曲並選擇是否播放，回傳成功與否</summary>
    private bool MoveToSongAndPlay(int dataIndex, bool play)
    {
        if (dataIndex >= m_currentSongDataList.Count || dataIndex < 0)
            return false;

        //歌曲數量超過一個頁面可顯示的範圍才需要滑動WrapContent
        if (m_currentSongDataList.Count > UI_MusicListening.m_iEachPageSongCount)
        {
            //移動WrapContent
            m_uiMusicListening.MoveWrapContentTo(dataIndex);
        }

        //點選指定歌曲
        int songGroupID = m_currentSongDataList[dataIndex].SongGroupID;
        Slot_SongMusic slot = m_uiMusicListening.GetSlotSongMusicByGroupID(songGroupID);
        OnButtonSongClick(slot.gameObject);

        if (play)
            m_mainApp.MusicApp.StartCoroutine(AutoPlayMusicWhenLoaded(songGroupID));

        return true;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>歌曲讀取完畢後自動播放</summary>
    private IEnumerator AutoPlayMusicWhenLoaded(int songGroupID)
    {
        if (m_LoadSongResourceCoroutine.Count > 0)
        {
            Coroutine lastCoroutine = m_LoadSongResourceCoroutine[m_LoadSongResourceCoroutine.Count - 1];
            yield return lastCoroutine;
        }
    
        //若玩家選擇其他曲目則取消播放
        if (m_currentChosenSongData.SongGroupID != songGroupID)
            yield break;

        m_musicPlayer.PlayMusic(true);
    }
    
    //-------------------------------------------------------------------------------------------------
    /// <summary>查詢目前的歌曲清單是否有該首歌</summary>
    private bool CheckSongInPlayList(int songGroupID)
    {
        for (int i = 0, iCount = m_currentSongDataList.Count; i < iCount; ++i)
        {
            if (songGroupID == m_currentSongDataList[i].SongGroupID)
                return true;
        }

        return false;
    }
}

