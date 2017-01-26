using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using System;

public class TutorialState : CustomBehaviorState, IGamePlayNoteCreater, IGamePlayParticleCreater
{
    private UI_Tutorial m_uiTutorial = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private AudioController m_audioController;
    private SoundSystem m_soundSystem;
    private MusicGameSystem m_musicGameSys;
    private TeachingSystem m_teachingSystem;

    private Dictionary<int, int> m_pressEventDict;
    private Dictionary<int, int> m_dragEventDict;

    private ArcLine m_arcLineState = ArcLine.None;

    private bool bAllowUpdate = false;

    //RuntimeData
    private int m_iHitNoteDepth = 1;
    private int m_uiBarQueue = 0;
    public bool m_bReplay;
    public bool m_bResume;
    private MusicData m_tempMusicData;
    private DateTime m_lastTime;
    private bool m_bFirstPlay = true;
    public bool PauseByChooseGuideUI;

    private Dictionary<Enum_NewGuideType, int> m_NoteTypeDict;

    private float m_fPlayTimeByDelta;
    //===================================================================================================================================
    public TutorialState(GameScripts.GameFramework.GameApplication app) : base(StateName.TUTORIAL_STATE, StateName.TUTORIAL_STATE, app)
    {
        m_pressEventDict = new Dictionary<int, int>();
        m_dragEventDict = new Dictionary<int, int>();
        m_NoteTypeDict = new Dictionary<Enum_NewGuideType, int>();
        m_soundSystem = m_mainApp.GetSystem<SoundSystem>();
        m_teachingSystem = m_mainApp.GetSystem<TeachingSystem>();
        m_resourceManager = m_mainApp.GetResourceManager();

        InitSongList();
        m_tempMusicData = null;
        PauseByChooseGuideUI = false;
    }

    #region BaseProcess
    public override void begin()
    {
        UnityDebugger.Debugger.Log("TutorialState.begin");
        base.begin();

        m_soundSystem.LoadGamePlaySound();
        m_pressEventDict.Clear();
        m_dragEventDict.Clear();        

        m_arcLineState = ArcLine.None; // 弧線狀態初始只有下面的先出現     
        m_uiTutorial = m_mainApp.GetGUIManager().GetGUI(typeof(UI_Tutorial).Name) as UI_Tutorial;
        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_audioController = m_mainApp.GetAudioController();

        m_musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();
        m_musicGameSys.NoteCreate.SetState(this);
        m_musicGameSys.ParticleCreate.SetState(this);

        m_musicGameSys.InitRuntimeData(GameDefine.DEFAULT_HITPOINT_DEPTH
            , Mathf.Max(m_uiTutorial.m_lowerArcTweener.GetComponent<UISprite>().material.renderQueue, m_uiTutorial.m_upperArcTweener.GetComponent<UISprite>().material.renderQueue));

        m_musicGameSys.MusicPlayData = null;
        bAllowUpdate = false;
        m_bFirstPlay = true;
        m_uiTutorial.InitializeUI();

        m_mainApp.MusicApp.StartCoroutine(BeginDelayLowerArcFadeOut());

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {  
        RealseRunTimeData();
        StopBGM();
        m_audioController.ClearBGM();
        m_soundSystem.UnloadGamePlaySound();
        m_musicGameSys.MusicPlayData = null;
        m_tempMusicData = null;
        m_uiTutorial = null;

        base.end();
        UnityDebugger.Debugger.Log("TutorialState.end");
    }
    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        m_tempMusicData = m_musicGameSys.MusicPlayData;
        PauseBGM();
        RemoveCallBack();
        base.suspend();
        UnityDebugger.Debugger.Log("TutorialState.suspend");
    }
    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        if (m_bReplay)
        { 
            m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(Replay));
            m_mainApp.MusicApp.StartCoroutine(BeginDelayLowerArcFadeOut());
        }
        else if (m_bResume)
            m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(ResumeBGM));

        if (m_uiTutorial != null)
        {
            m_musicGameSys.MusicPlayData = m_tempMusicData;
            m_musicGameSys.NoteCreate.SetState(this);
            m_musicGameSys.ParticleCreate.SetState(this);
            if (m_bIsSuspendByFullScreenState)
            {
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(ResumeBGM));
            }
            else
            {
                CheckActiveCommonGUI();
                ResumeBGM();
            }

            m_bIsSuspendByFullScreenState = false;
        }
        AddCallBack();
        base.resume();
        UnityDebugger.Debugger.Log("TutorialState.resume");
    }
    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        if (!bAllowUpdate)
            return;

        base.update();

        // 更新音樂播放時間
        if (m_bFirstPlay)
        {
            m_lastTime = DateTime.Now;
            m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
            m_bFirstPlay = false;
        }
        else
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan ts = currentTime - m_lastTime;
            m_fPlayTimeByDelta += (float)ts.TotalSeconds;
            m_lastTime = currentTime;
        }

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
            note.SyncUpdate(true);

            //自動戰鬥
            if (m_uiTutorial.AutoBattle)
                m_musicGameSys.AutoBattle(note, m_fPlayTimeByDelta);

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
        // 檢查更新弧線的出現時機
        UpdateArcLine();
        // 設定mark
        SetSubStartNoteMark();
        //hitpoint
        m_musicGameSys.TimeCheckCloseHitPoint(m_uiTutorial);
        // 檢查遊戲結束
        if ((m_musicGameSys.PlayTime >= (m_audioController.GetBGMLength()-0.1f)))         
        { 
            BattleGuideFinish();
        }
    }
    #endregion
    IEnumerator BeginDelayLowerArcFadeOut()
    {
        m_uiTutorial.m_spriteLowerArc.alpha = 0;
        //m_uiGamePlay.m_lowerArcTweener.enabled = false;
        m_arcLineState = ArcLine.None;
        m_arcLineState = ArcLine.Lower;
        yield return new WaitForSeconds(1.1f);
        if (!isPlaying)
            yield break;
        m_uiTutorial.LowerArcFadeIn();
    }

    #region Callback
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        for (int i = 0; i < m_uiTutorial.GameLineList.Count; i++)
        {
            UIEventListener.Get(m_uiTutorial.GameLineList[i].hitPoint).onPress = OnNotePress;
            UIEventListener.Get(m_uiTutorial.GameLineList[i].hitPoint).onDragOver = OnNoteDragOver;
        }
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        for (int i = 0; i < m_uiTutorial.GameLineList.Count; i++)
        {
            UIEventListener.Get(m_uiTutorial.GameLineList[i].hitPoint).onPress = null;
            UIEventListener.Get(m_uiTutorial.GameLineList[i].hitPoint).onDragOver = null;
        }
    }
    #endregion

    #region ButtonEvent
    public void OnNotePress(GameObject go, bool bPress)
    {
        GameLine gameLine = go.GetComponent<GameLineClickPoint>().m_gameLine;
        //UnityDebugger.Debugger.Log("OnNotePress " + gameLine.LineID + " " + bPress + " UICamera.currentTouchID " + UICamera.currentTouchID);
        if (bPress)
        {
            // 按下
            m_pressEventDict[UICamera.currentTouchID] = gameLine.LineID;
            m_dragEventDict[UICamera.currentTouchID] = gameLine.LineID; // 記錄按下時的線
            ControlSignal signal = new ControlSignal(ControlSignalType.ControllDown, UICamera.currentTouchID, gameLine.LineID, gameLine.LineID, gameLine.LineID);
            m_musicGameSys.SignalAllNotes(signal);
        }
        else
        {
            // 放開
            ControlSignal signal = new ControlSignal(ControlSignalType.ControllUp, UICamera.currentTouchID, gameLine.LineID, gameLine.LineID, gameLine.LineID);
            m_musicGameSys.SignalAllNotes(signal);
            m_pressEventDict.Remove(UICamera.currentTouchID);
            m_dragEventDict.Remove(UICamera.currentTouchID);
        }
    }
    //---------------------------------------------------------------------------------------------------
    public void OnNoteDragOver(GameObject go)
    {
        GameLine gameLine = go.GetComponent<GameLineClickPoint>().m_gameLine;
        // UnityDebugger.Debugger.Log("OnNoteDragOver " + gameLine.LineID + " UICamera.currentTouchID " + UICamera.currentTouchID);
        int iPointerID = UICamera.currentTouchID;
        if (m_dragEventDict.ContainsKey(iPointerID) && (m_dragEventDict[iPointerID] != gameLine.LineID))
        {
            int iFromLine = m_dragEventDict[iPointerID];
            int iToLine = gameLine.LineID;
            m_dragEventDict[iPointerID] = iToLine;

            ControlSignal signal = new ControlSignal(ControlSignalType.ControllSwipe, UICamera.currentTouchID, m_pressEventDict[iPointerID], iFromLine, iToLine);
            m_musicGameSys.SignalAllNotes(signal);
        }
        else
        {
            //僅供長按Freeze滑入判斷使用
            ControlSignal signal = new ControlSignal(ControlSignalType.ControllFreezeSwipe, UICamera.currentTouchID, gameLine.LineID, -1, -1);
            m_musicGameSys.SignalAllNotes(signal);
        }
    }
    #endregion

    #region MusicCtrl
    public void Replay()
    {
        StopBGM();
        m_bFirstPlay = true;
        PlayBGM();
        m_bReplay = false;
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        m_musicGameSys.PlayTime = m_fPlayTimeByDelta;
    }
    //---------------------------------------------------------------------------------------------------
    public void PlayBGM()
    {
        m_audioController.PlayBGM(null, false);        
    }
    //---------------------------------------------------------------------------------------------------
    public void ResumeBGM()
    {
        if (m_musicGameSys.MusicPlayData == null)
            return;
        PauseByChooseGuideUI = false;
        bAllowUpdate = true;
        m_bFirstPlay = true;
        PlayBGM();
        m_bResume = false;
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
    }
    //---------------------------------------------------------------------------------------------------
    public void PauseBGM()
    {
        bAllowUpdate = false;
        m_audioController.PauseBGM();        
    }
    //---------------------------------------------------------------------------------------------------
    public void StopBGM()
    {
        List<AbstractNote> destroyNote = new List<AbstractNote>(m_musicGameSys.Notes);
        for (int i = 0; i < destroyNote.Count; i++)
        {
            //ReleaseNote(destroyNote[i]);
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
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        m_musicGameSys.PlayTime = m_fPlayTimeByDelta;
    }
    //---------------------------------------------------------------------------------------------------
    public bool BGMIsPlay()
    {
        return m_audioController.BGMIsPlay();
    }
    #endregion
    
    #region NoteCreate
    //---------------------------------------------------------------------------------------------------
    public GameObject CreateNote(NoteType type)
    {
        GameObject go = m_uiTutorial.Dequeue(type);
        UISprite uiSprite = go.GetComponent<UISprite>();
        if (uiSprite != null)
        {
            uiSprite.color = Color.white;
            uiSprite.depth = GameDefine.DEFAULT_NOTE_DEPTH;
        }
        return go;
    }
    //---------------------------------------------------------------------------------------------------
    public void ReleaseNote(NoteType type, GameObject go)
    {
        if (m_uiTutorial != null)
            m_uiTutorial.Enqueue(type, go);
    }
    //---------------------------------------------------------------------------------------------------
    private void CreateNote()
    {
        MusicData musicData = m_musicGameSys.MusicPlayData;
        AbstractNote note = musicData.Notes[musicData.CurrentNoteIndex];

        // 賦與該Note GameObject給邏輯Note
        note.LineList = m_uiTutorial.GameLineList;
        note.CreateNote();
        // 將Note加到工作中的Notes        
        m_musicGameSys.Notes.Add(note);
        // 如果是多重Note
        if (note.Type == NoteType.Multi)
        {
            // 建立子Note物件
            MultiNote multiNote = note as MultiNote;
            foreach (AbstractNote subNote in multiNote.SubNotes)
            {
                m_musicGameSys.SetHitPointDepth(subNote);
                subNote.LineList = m_uiTutorial.GameLineList;

                LineRenderer lr = subNote.NoteObject.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    m_musicGameSys.SetLineMaterial(subNote, lr, subNote.Type, m_uiTutorial);
                    m_musicGameSys.SetShaderQueue(lr.material, 1);
                }

                //依LineOrder開啟對應HitPoint
                if (subNote.Type == NoteType.SubVerticalEnd)
                {
                    if ((subNote as SubVerticalNote).TrailType == NoteType.TrailDragVertical)
                        m_musicGameSys.OpenGameLineCollider((subNote as SubVerticalNote).TargetLine, m_uiTutorial);
                }
                else if (subNote.Type == NoteType.SubDragEnd || subNote.Type == NoteType.SubDragMiddle)
                    m_musicGameSys.OpenGameLineCollider((subNote as SubHorizontalNote).TargetLineOrder, m_uiTutorial);
                else
                    m_musicGameSys.OpenGameLineCollider(subNote.LineOrder, m_uiTutorial);
            }
        }
        else// 如果不是多重Note
        {
            m_musicGameSys.SetHitPointDepth(note);
            //依LineOrder開啟對應HitPoint
            m_musicGameSys.OpenGameLineCollider(note.LineOrder, m_uiTutorial);
        }
        musicData.CurrentNoteIndex++;
    }
    #endregion

    #region Particle
    //---------------------------------------------------------------------------------------------------
    public GameObject CreateParticleObject(EParticleType type)
    {
        GameObject go = m_uiTutorial.ParticleDequeue(type);
        return go;
    }
    //---------------------------------------------------------------------------------------------------
    public void ReleaseParticleObject(EParticleType type, GameObject go)
    {
        m_uiTutorial.ParticleEnqueue(type, go);
    }
    //---------------------------------------------------------------------------------------------------
    public void ShowParticle(EParticleType type, Transform tran)
    {
        GameObject go = m_uiTutorial.ParticleDequeue(type);
        go.transform.position = tran.position;
        ParticleInstance pi = go.GetComponent<ParticleInstance>();
        pi.initParticle();

        m_uiTutorial.StartCoroutine(ReleaseParticle(type, go));
    }
    //---------------------------------------------------------------------------------------------------
    public IEnumerator ReleaseParticle(EParticleType type, GameObject go, int time = 3)
    {
        yield return new WaitForSeconds(time);
        if (!isPlaying)
            yield break;

        m_uiTutorial.ParticleEnqueue(type, go);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    public void ShowHitJudge(HitJudgeType hitJudgeType, Transform tran)
    {
        HitJudgeIcon hitJudgeIcon = m_uiTutorial.CreateHitJudgeIcon(hitJudgeType);
        hitJudgeIcon.transform.position = new Vector3(tran.position.x, tran.position.y, hitJudgeIcon.transform.position.z);
        hitJudgeIcon.textureJudge.depth = GameDefine.DEFAULT_JudgeIcon_DEPTH;
        hitJudgeIcon.PlayJudgeIcon();
    }
    //---------------------------------------------------------------------------------------------------
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
    //---------------------------------------------------------------------------------------------------
    public void CreateDoubleFadeOutObj(Vector3 vecA, Vector3 vecB, int depth)
    {
        DoubleFadeOut fadeout = m_uiTutorial.CreateDoubleFadeOutObj(vecA, vecB, depth);
        m_musicGameSys.SetShaderQueue(fadeout.LineR.material, 1);
    }
    //---------------------------------------------------------------------------------------------------
    private void UpdateArcLine()
    {
        // 檢查生成的Note所屬上下弧線
        bool bHasLower = false;
        bool bHasUpper = false;
        for (int i = 0; i < m_musicGameSys.Notes.Count; i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];
            if (note.LineOrder <= "R3".ToLineOrder()) bHasLower = true;
            else bHasUpper = true;

            if (note.Type == NoteType.Multi)
            {
                // 是多重節點
                MultiNote multiNote = note as MultiNote;
                foreach (AbstractNote subNote in multiNote.SubNotes)
                {
                    if (subNote.LineOrder <= "R3".ToLineOrder()) bHasLower = true;
                    else bHasUpper = true;
                }
            }
        }
        ArcLine arcLineStatus;
        if (bHasLower && bHasUpper)
        {
            // 上下弧線都要出現
            arcLineStatus = ArcLine.Both;
        }
        else
        {
            if (bHasUpper)
            {
                // 僅出現上方弧線
                arcLineStatus = ArcLine.Upper;
            }
            else
            {
                // 預設只出現下方弧線
                arcLineStatus = ArcLine.Lower;
            }
        }

        switch (m_arcLineState)
        {
            case ArcLine.Lower:
                if (arcLineStatus == ArcLine.Both)
                {
                    m_uiTutorial.UpperArcFadeIn();
                    m_arcLineState = ArcLine.Both;
                }
                else if (arcLineStatus == ArcLine.Upper)
                {
                    m_uiTutorial.UpperArcFadeIn();
                    m_uiTutorial.LowerArcFadeOut();
                    m_arcLineState = ArcLine.Upper;
                }
                break;
            case ArcLine.Upper:
                if (arcLineStatus == ArcLine.Both)
                {
                    m_uiTutorial.LowerArcFadeIn();
                    m_arcLineState = ArcLine.Both;
                }
                else if (arcLineStatus == ArcLine.Lower)
                {
                    m_uiTutorial.LowerArcFadeIn();
                    m_uiTutorial.UpperArcFadeOut();
                    m_arcLineState = ArcLine.Lower;
                }
                break;
            case ArcLine.Both:
                if (arcLineStatus == ArcLine.Upper)
                {
                    m_uiTutorial.LowerArcFadeOut();
                    m_arcLineState = ArcLine.Upper;
                }
                else if (arcLineStatus == ArcLine.Lower)
                {
                    m_uiTutorial.UpperArcFadeOut();
                    m_arcLineState = ArcLine.Lower;
                }
                break;
            case ArcLine.None:
            default:
                break;
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void SetSubStartNoteMark()
    {
        if (m_arcLineState == ArcLine.Lower || m_arcLineState == ArcLine.None)
            return;

        for (int i = 0; i < m_musicGameSys.Notes.Count; i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];
            if (note.Type == NoteType.Multi)//多重note
            {
                // 是多重節點
                MultiNote multiNote = note as MultiNote;
                foreach (AbstractNote absNote in multiNote.SubNotes)
                {
                    SubNote subNote = absNote as SubNote;
                    subNote.SetNoteMark();
                }
            }
            else //單點NOTE
            {
                //SMALL太小先關閉
                if (note.Type == NoteType.Small)
                    continue;
                note.SetNoteMark();
            }
        }
    }
    //---------------------------------------------------------------------------------------------------
    void RealseRunTimeData()
    {
        m_iHitNoteDepth = 1;
        m_uiBarQueue = 0;
        m_bReplay = false;
        m_bResume = false;
        m_bFirstPlay = true;
        PauseByChooseGuideUI = false;
    }
    #region NewGuide

    //---------------------------------------------------------------------------------------------------
    void InitSongList()
    {
        m_NoteTypeDict.Add(Enum_NewGuideType.Single, 1001);
        m_NoteTypeDict.Add(Enum_NewGuideType.LongPressA, 1002);
        m_NoteTypeDict.Add(Enum_NewGuideType.LongPressB, 1003);
        m_NoteTypeDict.Add(Enum_NewGuideType.Slide, 1004);
        m_NoteTypeDict.Add(Enum_NewGuideType.Double, 1005);
        m_NoteTypeDict.Add(Enum_NewGuideType.CrossSlide, 1006);
        m_NoteTypeDict.Add(Enum_NewGuideType.PressSlideA, 1007);
        m_NoteTypeDict.Add(Enum_NewGuideType.PressSlideB, 1008);
        m_NoteTypeDict.Add(Enum_NewGuideType.AllBattleGuide, 1009);
    }
    //---------------------------------------------------------------------------------------------------
    void ReloadMusicDataInTutorial(int songGUID)
    {
        bAllowUpdate = false;
        PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_musicGameSys.PlaySpeed = dataSystem.GetSongNodeSpeed(songGUID);

        // 讀曲譜面
        TextAsset ta = m_resourceManager.GetResourceSync<TextAsset>(Enum_ResourcesType.Songs, dataSystem.GetTutorialSheetMusicDataPath(songGUID));
        if (ta == null)
        {
            UnityDebugger.Debugger.LogError("Load TextAsset FAILED");
        }
        else
        {
            // 解析譜面資料
            //UnityDebugger.Debugger.Log(ta.text);
            m_musicGameSys.LoadMusicData(ta.text);
        }

        // 讀取音樂
        Softstar.AudioController audioController = m_mainApp.GetAudioController();
        AudioClip audioClip = m_resourceManager.GetResourceSync<AudioClip>(Enum_ResourcesType.Songs, dataSystem.GetTutorialSongPath(songGUID));
        if (audioClip == null)
        {
            UnityDebugger.Debugger.LogError("Load AudioClip FAILED");
        }
        else
        {
            audioController.SetBGM(audioClip);
        }

        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        m_musicGameSys.InitializeGame();
        m_musicGameSys.PlayLength = m_audioController.GetBGMLength();

        if (ta != null && audioClip != null)
        {
            m_bFirstPlay = true;
            bAllowUpdate = true;
        }
    }
    //---------------------------------------------------------------------------------------------------
    public IEnumerator StartBattleGuide(Enum_NewGuideType type, bool isAuto)
    {
        StopBGM();

        yield return new WaitForSeconds(0.5f);

        ReloadMusicDataInTutorial(m_NoteTypeDict[type]);
        m_uiTutorial.AutoBattle = isAuto;
        PlayBGM();
        if (PauseByChooseGuideUI)
        {
            PauseBGM();
            PauseByChooseGuideUI = false;
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void BattleGuideFinish()
    {
        bAllowUpdate = false;
        int maxCombo = m_musicGameSys.ScoreCont.MaxCombo;
        StopBGM();

        if (m_teachingSystem.InquireGuideNextStepCondition() == Enum_GuideNextStepCondition.WaitForBattleGuideEnd)
            m_teachingSystem.NextStep(null);
        else if (m_teachingSystem.InquireGuideNextStepCondition() == Enum_GuideNextStepCondition.WaitForPlayerClick)
        {
            if (m_musicGameSys.ScoreCont.TotalNotes == maxCombo)
                m_teachingSystem.OnPlayerClickNoteSuccess();
            else
                m_teachingSystem.OnPlayerClickNoteFailed();
        }
    }
    #endregion

}
