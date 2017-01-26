using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class AdjustState : CustomBehaviorState, IGamePlayNoteCreater, IGamePlayParticleCreater
{
    private UI_Adjust m_uiAdjust = null;
    private UI_CommonBackground m_uiCommonBackground = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private Softstar.AudioController m_audioController;
    private SoundSystem m_soundSystem;
    private MusicGameSystem m_musicGameSys;

    private Dictionary<int, int> m_pressEventDict;

    private bool bLoadFinish = false;
    private bool bTriggerTimePartical = false;
    private float m_AdjustNumber = 0.0f;
    public float AdjustNumber
    {
        get { return m_AdjustNumber; }
        set
        {
            m_AdjustNumber = value;
            if (m_uiAdjust != null)
                m_uiAdjust.SetAdjustNumber(m_AdjustNumber);
        }
    }

    private CountDownSystem m_CdSystem;
    private CdTimer m_AdjustPressCdTimer;
    private MusicData m_tempMusicData;

    private float m_fPlayTimeByDelta; // 利用Time.deltaTime自行計算播放時間

    public bool IsPushBySettingState;
    //---------------------------------------------------------------------------------------------------
    public AdjustState(GameScripts.GameFramework.GameApplication app) : base(StateName.ADJUST_STATE, StateName.ADJUST_STATE, app)
    {
        m_mainApp = app as MainApplication;
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_guiManager = m_mainApp.GetGUIManager();
        m_CdSystem = m_mainApp.GetSystem<CountDownSystem>();
        m_musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();
        m_audioController = m_mainApp.GetAudioController();
        m_soundSystem = m_mainApp.GetSystem<SoundSystem>();

        m_pressEventDict = new Dictionary<int, int>();
        IsPushBySettingState = false;
        m_tempMusicData = null;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("AdjustState begin");

        bLoadFinish = false;
        bTriggerTimePartical = false;
        AdjustNumber = 0;

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_Adjust));
        base.SetGUIType(typeof(UI_CommonBackground));
        userData.Add(Enum_StateParam.UseTopBarUI, true);
        base.begin();
        
        if (m_bIsAync == false)
        {
            m_uiAdjust = m_guiManager.AddGUI<UI_Adjust>(typeof(UI_Adjust).Name);
            m_uiCommonBackground = m_guiManager.AddGUI<UI_CommonBackground>(typeof(UI_CommonBackground).Name);
            StateInit();
        }     
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_Adjust)))
            m_uiAdjust = m_guiManager.GetGUI(typeof(UI_Adjust).Name) as UI_Adjust;
        else if (operater.m_Type.Equals(typeof(UI_CommonBackground)))
            m_uiCommonBackground = m_guiManager.GetGUI(typeof(UI_CommonBackground).Name) as UI_CommonBackground;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();
        m_soundSystem.LoadGamePlaySound();
        //UI初始化
        m_guiManager.Initialize();
        m_uiAdjust.InitializeUI(m_mainApp.GetStringTable());
        //"校正"
        m_uiCommonBackground.InitializeUI(m_mainApp.GetString(30), null);

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));

        LoadSomething();
    }

    //---------------------------------------------------------------------------------------------------w
    public override void end()
    {
        bLoadFinish = false;
        EndSomething();

        m_uiAdjust = null;
        m_guiManager.DeleteGUI(typeof(UI_Adjust).Name);
        if (!IsPushBySettingState)
        {
            m_uiCommonBackground = null;
            m_guiManager.DeleteGUI(typeof(UI_CommonBackground).Name);
        }
        else
            m_soundSystem.UnloadGamePlaySound();

        m_musicGameSys.MusicPlayData = null;
        m_tempMusicData = null;
        IsPushBySettingState = false;
        base.end();
        UnityDebugger.Debugger.Log("AdjustState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        base.suspend();
        if (m_bIsSuspendByFullScreenState && m_uiAdjust != null)
        {
            HideBeSetGUI();
        }
        m_tempMusicData = m_musicGameSys.MusicPlayData;
        RemoveCallBack();  
        UnityDebugger.Debugger.Log("AdjustState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        m_musicGameSys.NoteCreate.SetState(this);
        m_musicGameSys.ParticleCreate.SetState(this);
        if (m_uiAdjust != null)
        {
            m_musicGameSys.MusicPlayData = m_tempMusicData;
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
        UnityDebugger.Debugger.Log("AdjustState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
        UpdateSomething();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiTopBar.AddCallBack();
        UIEventListener.Get(m_uiAdjust.m_ButtonIncrease.gameObject).onClick = OnButtonIncreaseClick;
        UIEventListener.Get(m_uiAdjust.m_ButtonIncrease.gameObject).onPress = OnButtonIncreasePress;
        UIEventListener.Get(m_uiAdjust.m_ButtonDecrease.gameObject).onClick = OnButtonDecreaseClick;
        UIEventListener.Get(m_uiAdjust.m_ButtonDecrease.gameObject).onPress = OnButtonDecreasePress;

        for (int i = 0; i < m_uiAdjust.GameLineList.Count; i++)
        {
            UIEventListener.Get(m_uiAdjust.GameLineList[i].hitPoint).onPress = OnNotePress;            
        }
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiTopBar.RemoveCallBack();
        UIEventListener.Get(m_uiAdjust.m_ButtonIncrease.gameObject).onClick = null;
        UIEventListener.Get(m_uiAdjust.m_ButtonIncrease.gameObject).onPress = null;
        UIEventListener.Get(m_uiAdjust.m_ButtonDecrease.gameObject).onClick = null;
        UIEventListener.Get(m_uiAdjust.m_ButtonDecrease.gameObject).onPress = null;

        for (int i = 0; i < m_uiAdjust.GameLineList.Count; i++)
        {
            UIEventListener.Get(m_uiAdjust.GameLineList[i].hitPoint).onPress = null;
        }
    }
    #region ButtonEvents
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        if (!isPlaying)
            return;

        TeachingSystem teachSys = m_mainApp.GetSystem<TeachingSystem>();
        //正常教學時
        if (!teachSys.CheckIsGuideFinished())
        {
            Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.LoadGUIAsync, true);
            SetIsSuspendByFullScreenState();
            m_mainApp.PushStateByScreenShot(StateName.LOAD_GAME_STATE, table);
        }
        //複查教學時
        else if (m_mainApp.IsNewGuiding())
        {
            //             TutorialState ts = m_mainApp.GetGameStateByName(StateName.TUTORIAL_STATE) as TutorialState;
            //             ts.OnUIShowFinish += teachSys.StartGuideTypeByState;
            teachSys.SetGuideEventToState(false);
            m_mainApp.PopStateByScreenShot(); 
        }    
        //非教學時
        else
            m_mainApp.PopStateByScreenShot();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonIncreaseClick(GameObject go)
    {
        IncreaseNumber();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonIncreasePress(GameObject go, bool isPress)
    {
        if (isPress && m_AdjustPressCdTimer == null)
        {
            m_AdjustPressCdTimer = m_CdSystem.StartCountDown(GameDefine.CDTIMER_ADJUST_TICKET, GameDefine.PRESS_TIME, IncreaseNumber);
        }
        else if (!isPress)
        {
            m_CdSystem.CloseCountDown(GameDefine.CDTIMER_ADJUST_TICKET);
            m_AdjustPressCdTimer = null;
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonDecreaseClick(GameObject go)
    {
        DecreaseNumber();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonDecreasePress(GameObject go, bool isPress)
    {
        if (isPress && m_AdjustPressCdTimer == null)
        {
            m_AdjustPressCdTimer = m_CdSystem.StartCountDown(GameDefine.CDTIMER_ADJUST_TICKET, GameDefine.PRESS_TIME, DecreaseNumber);
        }
        else if (!isPress)
        {
            m_CdSystem.CloseCountDown(GameDefine.CDTIMER_ADJUST_TICKET);
            m_AdjustPressCdTimer = null;
        }
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    private void IncreaseNumber(string ticket)
    {
        AdjustNumber += 0.05f;
    }
    //-------------------------------------------------------------------------------------------------
    private void IncreaseNumber()
    {
        AdjustNumber += 0.05f;
        //TODO : 實際調整音樂播放
        if (m_musicGameSys != null)
        {
            m_musicGameSys.NoteTimeOffset = AdjustNumber;
        }        
    }
    //-------------------------------------------------------------------------------------------------
    private void DecreaseNumber(string ticket)
    {
        AdjustNumber -= 0.05f;
    }
    //-------------------------------------------------------------------------------------------------
    private void DecreaseNumber()
    {       
        AdjustNumber -= 0.05f;
        //TODO : 實際調整音樂播放
        if (m_musicGameSys!= null)
        {
            m_musicGameSys.NoteTimeOffset = AdjustNumber;
        }        
    }
    //-------------------------------------------------------------------------------------------------
    void LoadSomething()
    {
        m_pressEventDict.Clear();        
        m_musicGameSys.PlaySpeed = 1;
        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        //取得校正值
        AdjustNumber = m_musicGameSys.NoteTimeOffset;

        SongData songData = new SongData();
        songData.SongGroupID = 999;
               
        // 讀曲譜面            
        TextAsset ta = m_resourceManager.GetResourceSync<TextAsset>(Enum_ResourcesType.Songs, dataSystem.GetAdjustSheetMusicDataPath(songData));
        //TextAsset ta = Resources.Load<TextAsset>("Songs/Adjust/trigger");
        if (ta == null)        
            UnityDebugger.Debugger.LogError("Load TextAsset FAILED");        
        else
            // 解析譜面資料
            m_musicGameSys.LoadMusicData(ta.text);

        // 讀取音樂
        m_audioController = m_mainApp.GetAudioController();
        AudioClip audioClip = m_resourceManager.GetResourceSync<AudioClip>(Enum_ResourcesType.Songs,dataSystem.GetAdjustSongPath(songData));
        //AudioClip audioClip = Resources.Load<AudioClip>("Songs/Adjust/trigger_music_120bpm");
        if (audioClip == null)        
            UnityDebugger.Debugger.LogError("Load AudioClip FAILED");        
        else
            m_audioController.SetBGM(audioClip);

        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        //關閉LOOP 第一個音節Replay時會重複播放
        m_audioController.SetBGMLoop(false);

        // 建立Notes
        m_uiAdjust.NoteBaseObjectMap[NoteType.Single] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Notes, "SingleNote");
        // 初始化Note物件，預設各個Note各五個
        m_uiAdjust.InitializeNoteObject();

        // 建立Particle
        m_uiAdjust.ParticleBaseObjectMap[EParticleType.Fantastic] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Fantastic");
        m_uiAdjust.ParticleBaseObjectMap[EParticleType.Great] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Great");
        m_uiAdjust.ParticleBaseObjectMap[EParticleType.Weak] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Weak");
        m_uiAdjust.ParticleBaseObjectMap[EParticleType.Miss] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Miss");        
        m_uiAdjust.InitializeParticleObject();

        // 讀取音效
        m_uiAdjust.TambourineClipList.Clear();
        m_uiAdjust.TambourineClipList.Add(m_resourceManager.GetResourceSync<AudioClip>(Enum_ResourcesType.Sound, "SE/tambourine1"));

        // 建立效果文字
        m_uiAdjust.HitJudgeBaseObjectMap[HitJudgeType.Miss] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconMiss");
        m_uiAdjust.HitJudgeBaseObjectMap[HitJudgeType.Weak] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconWeak");
        m_uiAdjust.HitJudgeBaseObjectMap[HitJudgeType.Good] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconGood");
        m_uiAdjust.HitJudgeBaseObjectMap[HitJudgeType.Perfect] = m_resourceManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconPerfect");

        GameObject gameLineObj = m_resourceManager.GetResourceSync(Enum_ResourcesType.Notes, "GameLine");
        
        GameObject go = Object.Instantiate<GameObject>(gameLineObj);
        go.transform.SetParent(m_uiAdjust.transform);
        go.transform.localScale = Vector3.one;
        GameLine gameLine = go.GetComponent<GameLine>();
        gameLine.v0.transform.position = new Vector3(m_uiAdjust.v0.position.x, m_uiAdjust.v0.position.y, -1.5f);
        gameLine.v1.transform.position = new Vector3(m_uiAdjust.v1.position.x, m_uiAdjust.v1.position.y, -1.5f);
        gameLine.v2.transform.position = new Vector3(m_uiAdjust.v2.position.x, m_uiAdjust.v2.position.y, -1.5f);
        gameLine.v3.transform.position = new Vector3(m_uiAdjust.v3.position.x, m_uiAdjust.v3.position.y, -1.5f);
        gameLine.RefreshPosition();
        gameLine.LineID = 0;
        BoxCollider col = gameLine.hitPoint.GetComponent<BoxCollider>();
        col.size = m_uiAdjust.HitPointSize;
        m_uiAdjust.GameLineList.Add(gameLine);
        
        //-------Init----------
        m_audioController = m_mainApp.GetAudioController();
        m_musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();
        m_musicGameSys.NoteCreate.SetState(this);
        m_musicGameSys.ParticleCreate.SetState(this);

        m_musicGameSys.InitializeGame();
        m_musicGameSys.PlayLength = m_audioController.GetBGMLength();

        m_mainApp.MusicApp.StartCoroutine(StartDelay());
    }
    //-------------------------------------------------------------------------------------------------
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);
        bLoadFinish = true;
        PlayBGM();
    }
    //-------------------------------------------------------------------------------------------------
    void EndSomething()
    {
        StopBGM();
        //將loop開回來
        m_audioController.SetBGMLoop(true);
        SaveNoteOffset();
        m_audioController.ClearBGM();
    }
    //-------------------------------------------------------------------------------------------------
    void UpdateSomething()
    {
        if (!bLoadFinish)
            return;

        // 更新音樂播放時間
        m_fPlayTimeByDelta += Time.deltaTime;
        m_musicGameSys.PlayTime = m_fPlayTimeByDelta;

        // 更新給Note的計算時間
        m_musicGameSys.MusicPlayData.PlayTime = m_musicGameSys.PlayTime;

        // 檢查並建立Note
        while (m_musicGameSys.CheckCreateNote())
        {
            // 建立Note
            CreateNote();
        }

        List<AbstractNote> destroyList = new List<AbstractNote>();
        for (int i = 0; i < m_musicGameSys.Notes.Count; i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];
            // 更新每個Note位置
            note.SyncUpdate(true,false);

            /*if(!bTriggerTimePartical && note.CheckOnTrigger())
            {
                bTriggerTimePartical = true;
                note.ForceShowPartical();
            }*/

            if (note.State == NoteState.Destroyed)
            {
                destroyList.Add(note);
            }
        }
        // 釋放Note
        if (destroyList.Count > 0)
        {
            for (int i = 0; i < destroyList.Count; i++)
            {
                AbstractNote note = destroyList[i];
                m_musicGameSys.Notes.Remove(note);                
            }
        }

        // 檢查遊戲結束
        if (m_musicGameSys.PlayTime >= (m_musicGameSys.PlayLength)) // 音樂結束前的0.1秒結束遊戲
        {
            Replay();
            bTriggerTimePartical = false;
            m_musicGameSys.SetMusicNoteTimeOffset();
        }
    }
    //-------------------------------------------------------------------------------------------------
    private void CreateNote()
    {
        MusicData musicData = m_musicGameSys.MusicPlayData;
        AbstractNote note = musicData.Notes[musicData.CurrentNoteIndex];

        // 賦與該Note GameObject給邏輯Note
        note.LineList = m_uiAdjust.GameLineList;
        note.CreateNote();
        // 將Note加到工作中的Notes
        //SetHitPointDepth(note);
        m_musicGameSys.Notes.Add(note);

        musicData.CurrentNoteIndex++;
    }
    //-------------------------------------------------------------------------------------------------
    public void Replay()
    {
        StopBGM();
        PlayBGM();
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
    }
    //-------------------------------------------------------------------------------------------------
    public void PlayBGM()
    {
        m_audioController.PlayBGM(null, false);
    }
    //-------------------------------------------------------------------------------------------------
    public void StopBGM()
    {
        List<AbstractNote> destroyNote = new List<AbstractNote>(m_musicGameSys.Notes);
        for (int i = 0; i < destroyNote.Count; i++)
        {
            destroyNote[i].DestroyNote();
        }
        m_musicGameSys.Notes.Clear();
        m_audioController.StopBGM();
        m_audioController.SetBGMPlayTime(0);
        if (m_musicGameSys.MusicPlayData != null)
        {
            m_musicGameSys.MusicPlayData.CurrentNoteIndex = 0;
            m_musicGameSys.InitializeGame();
        } 
        m_musicGameSys.PlayLength = m_audioController.GetBGMLength();
    }
    //-------------------------------------------------------------------------------------------------
    public void SignalAllNotes(ControlSignal signal)
    {
        for (int i = 0; i < m_musicGameSys.Notes.Count; i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];
            NoteHandleReturn ret = note.HandleControl(signal);
            if (ret != NoteHandleReturn.NotHandle)
            {
                if (ret == NoteHandleReturn.HandleAndDestroy)
                {
                    /*HitJudgeType hitJudgeType;
                    switch (note.NodeScoreType)
                    {
                        case ScoreType.Fantastic:
                            hitJudgeType = HitJudgeType.Perfect;
                            break;
                        case ScoreType.Great:
                            hitJudgeType = HitJudgeType.Good;
                            break;
                        case ScoreType.Weak:
                            hitJudgeType = HitJudgeType.Weak;
                            break;
                        default:
                        case ScoreType.Lost:
                            hitJudgeType = HitJudgeType.Miss;
                            break;
                    }
                    if (hitJudgeType != HitJudgeType.Miss)
                    {
                        PlayTambourine();
                    }*/
                }
            }
        }
    }

    public void PlayTambourine()
    {
        AudioClip clip = m_uiAdjust.TambourineClipList[m_uiAdjust.TambourineClipIndex];

        m_audioController.PlaySE(clip);

        m_uiAdjust.TambourineClipIndex++;
        m_uiAdjust.TambourineClipIndex %= m_uiAdjust.TambourineClipList.Count;
    }
    //-------------------------------------------------------------------------------------------------
    //----
    public GameObject CreateNote(NoteType type)
    {
        GameObject go = m_uiAdjust.Dequeue(type);
        UISprite uiSprite = go.GetComponent<UISprite>();
        if (uiSprite != null)
        {
            uiSprite.color = Color.white;
            uiSprite.depth = GameDefine.DEFAULT_NOTE_DEPTH;
        }
        return go;
    }
    //-------------------------------------------------------------------------------------------------
    public void ReleaseNote(NoteType type, GameObject go)
    {
        if(m_uiAdjust != null)
            m_uiAdjust.Enqueue(type, go);
    }
    //-------------------------------------------------------------------------------------------------
    public GameObject CreateParticleObject(EParticleType type)
    {
        GameObject go = m_uiAdjust.ParticleDequeue(type);
        return go;
    }
    //-------------------------------------------------------------------------------------------------
    public void ReleaseParticleObject(EParticleType type, GameObject go)
    {
        m_uiAdjust.ParticleEnqueue(type, go);
    }
    //-------------------------------------------------------------------------------------------------
    public void ShowParticle(EParticleType type, Transform tran)
    {
        GameObject go = m_uiAdjust.ParticleDequeue(type);
        go.transform.position = tran.position;
        ParticleInstance pi = go.GetComponent<ParticleInstance>();
        pi.initParticle();

        m_uiAdjust.StartCoroutine(ReleaseParticle(type, go));
    }
    //-------------------------------------------------------------------------------------------------
    public IEnumerator ReleaseParticle(EParticleType type, GameObject go, int time = 3)
    {
        yield return new WaitForSeconds(time);
        if (m_uiAdjust != null)
            m_uiAdjust.ParticleEnqueue(type, go);
    }
    //-------------------------------------------------------------------------------------------------
    public void ShowHitJudge(HitJudgeType hitJudgeType, Transform tran)
    {
        
        HitJudgeIcon hitJudgeIcon = m_uiAdjust.CreateHitJudgeIcon(hitJudgeType);
        hitJudgeIcon.transform.position = new Vector3(tran.position.x, tran.position.y, hitJudgeIcon.transform.position.z);
        hitJudgeIcon.textureJudge.depth = GameDefine.DEFAULT_JudgeIcon_DEPTH;
        hitJudgeIcon.PlayJudgeIcon();
        
    }
    //-------------------------------------------------------------------------------------------------
    private void SaveNoteOffset()
    {
        m_musicGameSys.SaveNoteOffset();
    }
    //-------------------------------------------------------------------------------------------------
    // 根據類型播放點擊音效
    public void PlayNoteSoundEffect(EParticleType type)
    {
        switch (type)
        {
            case EParticleType.Miss:
                break;
            case EParticleType.Double:
                m_soundSystem.PlayDoubleClickNoteSound();
                break;
            default:
                m_soundSystem.PlayOneClickNoteSound();
                break;
        }
    }
    public void CreateDoubleFadeOutObj(Vector3 vecA, Vector3 vecB, int depth)
    {
        
    }
    //-------------------------------------------------------------------------------------------------
    #region ButtonEvent
    public void OnNotePress(GameObject go, bool bPress)
    {
        GameLine gameLine = go.GetComponent<GameLineClickPoint>().m_gameLine;
        //UnityDebugger.Debugger.Log("OnNotePress " + gameLine.LineID + " " + bPress + " UICamera.currentTouchID " + UICamera.currentTouchID);
        if (bPress)
        {
            // 按下
            m_pressEventDict[UICamera.currentTouchID] = gameLine.LineID;            
            ControlSignal signal = new ControlSignal(ControlSignalType.ControllDown, UICamera.currentTouchID, gameLine.LineID, gameLine.LineID, gameLine.LineID);
            SignalAllNotes(signal);
        }
        else
        {
            // 放開
            ControlSignal signal = new ControlSignal(ControlSignalType.ControllUp, UICamera.currentTouchID, gameLine.LineID, gameLine.LineID, gameLine.LineID);
            SignalAllNotes(signal);
            m_pressEventDict.Remove(UICamera.currentTouchID);            
        }
    }
    #endregion
}

