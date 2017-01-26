using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class GamePlayState : CustomBehaviorState, IGamePlayNoteCreater, IGamePlayParticleCreater
{    
    private UI_GamePlay m_uiGamePlay;
    private UI_3D_BattleBG m_ui3DBattleBG;
    private Softstar.AudioController m_audioController;
    private MusicGameSystem m_musicGameSys;
    private SoundSystem m_soundSystem;

    private Dictionary<int, int> m_pressEventDict;
    private Dictionary<int, int> m_dragEventDict;

    private ArcLine m_arcLineState = ArcLine.None;
    private float m_fLastCheckHitpointTime;
    
    //RuntimeData
    private int m_iTempCombo = 0;
    private int m_iTempScore = 0;
    private int m_iHitNoteDepth = 1;
    private int m_uiBarQueue = 0;
    public bool m_bReplay;
    public bool m_bResume;
    public bool m_bStopByEditor;
    private CdTimer m_updateScoreTimer;
    private MusicData m_tempMusicData;
    public bool m_bFullComboAnim;

    private float m_fPlayTimeByDelta; // 利用Time.deltaTime自行計算播放時間
    private DateTime m_lastTime;

    private ComboEffectLevel m_comboEffectLevel;
    private bool m_bInitialCheck = false; // Combo結束往分數射擊，一開始Combo為0會發出一次，因此另外做個旗標跳過第一次設擊
    
    //CDSystem
    private string m_cdKey_LastNote = "cdLastNote";
    private CdTimer m_cdTime;

    private bool m_bFirstPlay = true;

    //==========================================================================
    // Fade out結束歌曲功能
    private bool m_bFadeOutEnd = false; // 是否以Fade out結束歌曲
    private bool m_bFadeOutFlag = false; // Fade out完成旗標
    private float m_fFadeOutTime = 60.0f; // Fade out時間點
    private float m_fFadeOutLen = 5.0f; // Fade out長度
    //==========================================================================

    public GamePlayState(GameScripts.GameFramework.GameApplication app) : base(StateName.GAME_PLAY_STATE, StateName.GAME_PLAY_STATE, app)
    {
        m_pressEventDict = new Dictionary<int, int>();
        m_dragEventDict = new Dictionary<int, int>();
        m_soundSystem = m_mainApp.GetSystem<SoundSystem>();
        m_tempMusicData = null;
    }

    public override void begin()
    {
        UnityDebugger.Debugger.Log("GamePlayState.begin");

        bool isTutorial = false;
        if(userData != null)        
            isTutorial = userData.ContainsKey(Enum_StateParam.Tutorial);
              
        base.SetGUIType((isTutorial) ? typeof(UI_Tutorial) : typeof(UI_GamePlay));

        base.begin();

        m_soundSystem.LoadGamePlaySound();
        m_bFirstPlay = true;
        m_pressEventDict.Clear();
        m_dragEventDict.Clear();

        m_arcLineState = ArcLine.None; // 弧線狀態初始只有下面的先出現        

        m_uiGamePlay = m_mainApp.GetGUIManager().GetGUI(typeof(UI_GamePlay).Name) as UI_GamePlay;
        m_ui3DBattleBG = m_mainApp.GetGUI3DManager().GetGUI(typeof(UI_3D_BattleBG).Name) as UI_3D_BattleBG;

        PlayerDataSystem playerDataSys = m_mainApp.GetSystem<PlayerDataSystem>();  
        
        m_audioController = m_mainApp.GetAudioController();

        m_comboEffectLevel = ComboEffectLevel.None;
        m_bInitialCheck = false;        

        m_musicGameSys = m_mainApp.GetSystem<MusicGameSystem>();
        m_musicGameSys.NoteCreate.SetState(this);
        m_musicGameSys.ParticleCreate.SetState(this);

        m_bFadeOutFlag = false;
        m_bFullComboAnim = false;

        /*
        m_iHitNoteDepth = GameDefine.DEFAULT_HITPOINT_DEPTH;
        m_uiBarQueue = Mathf.Max(m_uiGamePlay.m_lowerArcTweener.GetComponent<UISprite>().material.renderQueue
                                        , m_uiGamePlay.m_upperArcTweener.GetComponent<UISprite>().material.renderQueue);
        */
        m_musicGameSys.InitRuntimeData(GameDefine.DEFAULT_HITPOINT_DEPTH
            , Mathf.Max(m_uiGamePlay.m_ArcArray[0].material.renderQueue, m_uiGamePlay.m_ArcArray[3].material.renderQueue));

        if (m_mainApp.EnvironmentType == Enum_EnvironmentType.NormalEnvironment)
        {
            m_uiGamePlay.InitializeUI();
            m_uiGamePlay.m_comboContainer.SetActive(true);
            m_uiGamePlay.m_scoreNumberPicker.gameObject.SetActive(true);

            m_musicGameSys.InitializeGame();
            m_musicGameSys.PlayLength = m_audioController.GetBGMLength();

            //Set Rank Score
            S_Songs_Tmp songTmp = playerDataSys.GetSongTmp(playerDataSys.GetCurrentSongData(), playerDataSys.GetCurrentSongPlayData().SongDifficulty);
            if (songTmp != null)
            {
                m_musicGameSys.ScoreCont.ScoreGenius = songTmp.iScore_G;
                m_musicGameSys.ScoreCont.ScoreS = songTmp.iScore_S;
                m_musicGameSys.ScoreCont.ScoreA = songTmp.iScore_A;
                m_musicGameSys.ScoreCont.ScoreB = songTmp.iScore_B;
                m_musicGameSys.ScoreCont.ScoreC = songTmp.iScore_C;
            }

            m_bStopByEditor = false;
            //m_mainApp.MusicApp.StartCoroutine(BeginDelayLowerArcFadeOut());                       

            BattleStart();
        }
        else
        {
            m_uiGamePlay.m_buttonPause.gameObject.SetActive(false);
            m_uiGamePlay.m_comboContainer.SetActive(false);
            m_uiGamePlay.m_scoreNumberPicker.gameObject.SetActive(false);
            //m_mainApp.MusicApp.StartCoroutine(BeginDelayLowerArcFadeOut());
            m_bStopByEditor = true;
        }

        m_cdTime = null;

        AddCallBack();
    }

    private void BattleStart()
    {
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        //Change 3DUI Battle Background Performance
        m_ui3DBattleBG.SwitchGameStart(true);
        m_uiGamePlay.PlayStartAnimation();

        m_updateScoreTimer = m_mainApp.GetSystem<CountDownSystem>().StartCountDown(GameDefine.TICEKT_UPDATE_SCORE, m_uiGamePlay.m_scoreNumberPicker.Duration + 0.2f, UpdateScore);
        PlayBGM();
    }

    public override void end()
    {
        RemoveCallBack();
        CloseCDLastNoteProcess();
        RealseRunTimeData();
        StopBGM();
        m_audioController.ClearBGM();
        m_ui3DBattleBG.SwitchGameStart(false);

        m_uiGamePlay = null;
        m_musicGameSys.MusicPlayData = null;
        m_soundSystem.UnloadGamePlaySound();
        base.end();
    }

    public override void ApplicationPause()
    {
        base.ApplicationPause();
        if (m_mainApp.EnvironmentType == Enum_EnvironmentType.NormalEnvironment)
            OnButtonPauseClick(null);        
    }

    public override void ApplicationResume()
    {
        base.ApplicationResume();
        if (m_mainApp.EnvironmentType == Enum_EnvironmentType.SMEEnvironment)
            m_lastTime = DateTime.Now;
    }

    public override void onGUI()
    {
        base.onGUI();
    }

    public override void update()
    {
        base.update();
#if UNITY_EDITOR
        CheckKeyEvent();
#endif

        // 更新音樂播放時間
        if(!m_bStopByEditor)
        {
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
        }

        // 更新給Note的計算時間
        m_musicGameSys.MusicPlayData.PlayTime = m_musicGameSys.PlayTime;

        //更新UI上方播放進度
        m_uiGamePlay.SetProgressBarValue(m_musicGameSys.PlayTime, m_musicGameSys.PlayLength);

        // 檢查並建立Note
        while(m_musicGameSys.CheckCreateNote())
        {
            // 建立Note
            CreateNote();
        }

        List<AbstractNote> destroyList = new List<AbstractNote>();
        for(int i=0;i<m_musicGameSys.Notes.Count;i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];
            // 更新每個Note位置
            note.SyncUpdate(true);

            //自動戰鬥
            if(m_uiGamePlay.AutoBattle)
                m_musicGameSys.AutoBattle(note, m_fPlayTimeByDelta);

            if (note.State == NoteState.Destroyed)
            {
                destroyList.Add(note);
            }
        }
        // 釋放Note
        if (destroyList.Count > 0)
        {
            for(int i=0;i<destroyList.Count;i++)
            {
                AbstractNote note = destroyList[i];
                m_musicGameSys.Notes.Remove(note);
                //
                LastNoteComboProcess(note);                
            }
        }

        // 檢查更新弧線的出現時機
        //UpdateArcLine();
        UpdateArcLine2();                
        // 設定mark
        SetSubStartNoteMark();
        //hitpoint
        m_musicGameSys.TimeCheckCloseHitPoint(m_uiGamePlay);

#if DEVELOP
        //Test
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            m_fPlayTimeByDelta = m_musicGameSys.PlayLength-1.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            OnButtonPauseClick(null);
        }
#endif

        if (m_mainApp.EnvironmentType == Enum_EnvironmentType.NormalEnvironment)
        {
            // 更新數值
            UpdateScoreAndCombo();
            //更新音樂播放時間給動態背景
            m_ui3DBattleBG.SetMusicProgress(m_musicGameSys.PlayTime / m_musicGameSys.PlayLength);
        }

        if(m_bFadeOutEnd)
        {
            if(m_musicGameSys.PlayTime >= m_fFadeOutTime)
            {
                float fVolume = m_musicGameSys.PlayTime - m_fFadeOutTime;
                fVolume = 1.0f - Mathf.Clamp01(fVolume / m_fFadeOutLen);
                m_audioController.SetBGMVolume(fVolume);
            }
            if(m_musicGameSys.PlayTime >= (m_fFadeOutTime + m_fFadeOutLen))
            {
                m_bFadeOutFlag = true;
            }
        }
        // 檢查遊戲結束
        if ((m_musicGameSys.PlayTime >= (m_musicGameSys.PlayLength - 0.1f)) || // 音樂結束前的0.1秒結束遊戲 或
            m_bFadeOutFlag) // Fade out 完成而結束遊戲
        {
            //樂譜編輯模式不跳結算畫面
            if (m_mainApp.EnvironmentType != Enum_EnvironmentType.SMEEnvironment)
            {       
                //FullCombo時待動畫結束才跳轉狀態         
                if(!m_bFullComboAnim)
                    BattleFinish();
            }                
            else
                StopBGMByEditor();
        }    
    }

    public override void suspend()
    {
        m_tempMusicData = m_musicGameSys.MusicPlayData;
        PauseBGM();
        RemoveCallBack();
        base.suspend();
    }

    public override void resume()
    {
        if (m_bReplay)
        {
            m_uiGamePlay.ResetUIForRestart();
            m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(Replay));
        }            
        else if (m_bResume)
            m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(ResumeGame));

        if (m_uiGamePlay != null)
        {
            m_musicGameSys.MusicPlayData = m_tempMusicData;
            if (m_bIsSuspendByFullScreenState)
            {
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(ResumeGame));
            }
            else
            {
                CheckActiveCommonGUI();
                ResumeGame();
            }

            m_bIsSuspendByFullScreenState = false;
        }

        AddCallBack();
        base.resume();
    }

    //==========================================================================

    private void UpdateScoreAndCombo()
    {
        Softstar.ScoreController scoreController = m_musicGameSys.ScoreCont;
       

        if (m_iTempCombo != scoreController.ComboCount)
        {
            UpdateComboEffect();
            m_uiGamePlay.SetComboValue(scoreController.ComboCount);
            PlayComboDynamicBG(scoreController.ComboCount);
            m_iTempCombo = scoreController.ComboCount;
        }
    }

    private void UpdateScore(string ticket)
    {
        Softstar.ScoreController scoreController = m_musicGameSys.ScoreCont;
        if (m_iTempScore != (int)scoreController.Score)
        {
            m_uiGamePlay.SetScoreValue((int)scoreController.Score);
            if (scoreController.ComboCount != 0)
                m_uiGamePlay.m_scoreNumberPicker.Play(false);

            m_iTempScore = (int)scoreController.Score;
        }    
    }

    private void UpdateComboEffect()
    {
        int iCombo = m_musicGameSys.ScoreCont.ComboCount;
        if (iCombo == 0)
        {
            // Combo歸零，清除特效
            m_comboEffectLevel = ComboEffectLevel.None;
            m_uiGamePlay.SetComboEffect(m_comboEffectLevel);
            if (m_bInitialCheck)
            {
                m_uiGamePlay.ComboEndShot();
                m_uiGamePlay.m_scoreNumberPicker.Play(true);
            }
            else
            {
                // 避開遊戲剛開始會先發射一次的問題
                m_bInitialCheck = true;
            }
        }
        else
        {
            switch (m_comboEffectLevel)
            {
                case ComboEffectLevel.None:
                    if(iCombo >= GameDefine.COMBO_EFFECT_CONDITION_1)
                    {
                        // 建立Lv1特效
                        m_comboEffectLevel = ComboEffectLevel.Lv1;
                        m_uiGamePlay.SetComboEffect(m_comboEffectLevel);
                    }
                    break;
                case ComboEffectLevel.Lv1:
                    if (iCombo >= GameDefine.COMBO_EFFECT_CONDITION_2)
                    {
                        // 建立Lv2特效
                        m_comboEffectLevel = ComboEffectLevel.Lv2;
                        m_uiGamePlay.SetComboEffect(m_comboEffectLevel);
                    }
                    break;
                case ComboEffectLevel.Lv2:
                    if (iCombo >= GameDefine.COMBO_EFFECT_CONDITION_3)
                    {
                        // 建立Lv3特效
                        m_comboEffectLevel = ComboEffectLevel.Lv3;
                        m_uiGamePlay.SetComboEffect(m_comboEffectLevel);
                    }
                    break;
                case ComboEffectLevel.Lv3:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// For ParticleCreater use
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject CreateParticleObject(EParticleType type)
    {
        GameObject go = m_uiGamePlay.ParticleDequeue(type);
        return go;
    }
    /// <summary>
    /// For ParticleCreater use
    /// </summary>
    /// <param name="type"></param>
    /// <param name="go"></param>
    public void ReleaseParticleObject(EParticleType type, GameObject go)
    {
        m_uiGamePlay.ParticleEnqueue(type, go);
    }

    /// <summary>
    /// For NoteCreater use
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject CreateNote(NoteType type)
    {
        GameObject go = m_uiGamePlay.Dequeue(type);
        UISprite uiSprite = go.GetComponent<UISprite>();
        if(uiSprite != null)
        {
            uiSprite.color = Color.white;
            uiSprite.depth = GameDefine.DEFAULT_NOTE_DEPTH;
            m_musicGameSys.SetShaderQueue(uiSprite.material, 3);
        }
        return go;
    }
    /// <summary>
    /// For NoteCreater use
    /// </summary>
    /// <param name="type"></param>
    /// <param name="go"></param>
    public void ReleaseNote(NoteType type, GameObject go)
    {
        m_uiGamePlay.Enqueue(type, go);             
    }

    private void CreateNote()
    {
        MusicData musicData = m_musicGameSys.MusicPlayData;
        AbstractNote note = musicData.Notes[musicData.CurrentNoteIndex];

        // 賦與該Note GameObject給邏輯Note
        note.LineList = m_uiGamePlay.GameLineList;
        note.CreateNote();
        // 將Note加到工作中的Notes        
        m_musicGameSys.Notes.Add(note);        
        // 如果是多重Note
        if (note.Type == NoteType.Multi)
        {
            // 建立子Note物件
            MultiNote multiNote = note as MultiNote;
            foreach(AbstractNote subNote in multiNote.SubNotes)
            {
                m_musicGameSys.SetHitPointDepth(subNote);
                subNote.LineList = m_uiGamePlay.GameLineList;

                LineRenderer lr = subNote.NoteObject.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    m_musicGameSys.SetLineMaterial(subNote,lr, subNote.Type, m_uiGamePlay);                    
                    m_musicGameSys.SetShaderQueue(lr.material, 2);
                }
                    
                //依LineOrder開啟對應HitPoint
                if (subNote.Type == NoteType.SubVerticalEnd)
                {
                    if((subNote as SubVerticalNote).TrailType == NoteType.TrailDragVertical)
                        m_musicGameSys.OpenGameLineCollider((subNote as SubVerticalNote).TargetLine, m_uiGamePlay);
                }                                                    
                else if(subNote.Type == NoteType.SubDragEnd || subNote.Type == NoteType.SubDragMiddle)
                    m_musicGameSys.OpenGameLineCollider((subNote as SubHorizontalNote).TargetLineOrder, m_uiGamePlay);
                else
                    m_musicGameSys.OpenGameLineCollider(subNote.LineOrder, m_uiGamePlay);
            }
        }
        else// 如果不是多重Note
        {
            m_musicGameSys.SetHitPointDepth(note);
            //依LineOrder開啟對應HitPoint
            m_musicGameSys.OpenGameLineCollider(note.LineOrder, m_uiGamePlay);
        }
        musicData.CurrentNoteIndex++;
    }

    public void CreateDoubleFadeOutObj(Vector3 vecA,Vector3 vecB,int depth)
    {
        DoubleFadeOut fadeout = m_uiGamePlay.CreateDoubleFadeOutObj(vecA, vecB,depth);
        m_musicGameSys.SetShaderQueue(fadeout.LineR.material, 1);
    }
    
    IEnumerator BeginDelayLowerArcFadeOut()
    {
        m_uiGamePlay.m_spriteLowerArc.alpha = 0;
        //m_uiGamePlay.m_lowerArcTweener.enabled = false;
        m_arcLineState = ArcLine.None;
        m_arcLineState = ArcLine.Lower;
        yield return new WaitForSeconds(1.1f);        
        m_uiGamePlay.LowerArcFadeIn();
    }
    //--------------------------------------------------------------------------------------------------
    private void UpdateArcLine2()
    {
        m_uiGamePlay.InitArcSwitch();

        for (int i = 0; i < m_musicGameSys.Notes.Count; i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];

            if (note.Type == NoteType.Multi)
            {
                // 是多重節點
                MultiNote multiNote = note as MultiNote;
                foreach (AbstractNote subNote in multiNote.SubNotes)
                {
                    if (subNote.State == NoteState.Freeze)
                        continue;

                    //依LineOrder開啟對應HitPoint
                    if (subNote.Type == NoteType.SubVerticalEnd)
                    {
                        if ((subNote as SubVerticalNote).TrailType == NoteType.TrailDragVertical)
                        {
                            if (subNote.NormalizeTime >= subNote.FadeInTime /*&& subNote.NormalizeTime <= GameDefine.NOTE_FADE_OUT_TIME*/)
                            {
                                SetLine2Arc((subNote as SubVerticalNote).TargetLine);

                                if (subNote.Type == NoteType.SubVerticalEnd)
                                    SetLine2Arc((subNote as SubVerticalNote).PreviousNote.LineOrder);
                            }                                
                        }                         
                        else
                        {
                            if (subNote.NormalizeTime >= subNote.FadeInTime/* && subNote.NormalizeTime <= GameDefine.NOTE_FADE_OUT_TIME*/)
                                SetLine2Arc((subNote as SubVerticalNote).LineOrder);
                        }                            
                    }
                    else if (subNote.Type == NoteType.SubDragEnd || subNote.Type == NoteType.SubDragMiddle)
                    {
                        if (subNote.NormalizeTime >= subNote.FadeInTime && subNote.NormalizeTime <= GameDefine.NOTE_FADE_OUT_TIME)
                            SetLine2Arc((subNote as SubHorizontalNote).TargetLineOrder);
                    }                                            
                    else
                        if (subNote.NormalizeTime >= subNote.FadeInTime && subNote.NormalizeTime <= GameDefine.NOTE_FADE_OUT_TIME)
                            SetLine2Arc(subNote.LineOrder);                        
                }
            }
            else
            {
                if(m_musicGameSys.Notes[i].NormalizeTime >= m_musicGameSys.Notes[i].FadeInTime && m_musicGameSys.Notes[i].NormalizeTime <= GameDefine.NOTE_FADE_OUT_TIME)
                    SetLine2Arc(m_musicGameSys.Notes[i].LineOrder);                
            }
        }

        m_uiGamePlay.FadeInArcProcess();
    }

    void SetLine2Arc(int lineID)
    {
        if(lineID >= (int)emLineOrder.S_L3)
        {
            if(lineID <= (int)emLineOrder.S_L2)
                m_uiGamePlay.SetArcSwitchToggle(emArcLine.UL);
            else if(lineID <= (int)emLineOrder.S_R1a)
                m_uiGamePlay.SetArcSwitchToggle(emArcLine.UM);
            else
                m_uiGamePlay.SetArcSwitchToggle(emArcLine.UR);
        }
        else
        {
            if (lineID <= (int)emLineOrder.L2)
                m_uiGamePlay.SetArcSwitchToggle(emArcLine.LL);
            else if (lineID <= (int)emLineOrder.R1a)
                m_uiGamePlay.SetArcSwitchToggle(emArcLine.LM);
            else
                m_uiGamePlay.SetArcSwitchToggle(emArcLine.LR);
        }
    }

    //--------------------------------------------------------------------------------------------------
    private void UpdateArcLine()
    {
        // 檢查生成的Note所屬上下弧線
        bool bHasLower = false;
        bool bHasUpper = false;
        for (int i=0;i<m_musicGameSys.Notes.Count;i++)
        {
            AbstractNote note = m_musicGameSys.Notes[i];
            if (note.LineOrder <= "R3".ToLineOrder()) bHasLower = true;
            else bHasUpper = true;

            if(note.Type == NoteType.Multi)
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
        if(bHasLower && bHasUpper)
        {
            // 上下弧線都要出現
            arcLineStatus = ArcLine.Both;
        }
        else
        {
            if(bHasUpper)
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

        switch(m_arcLineState)
        {
            case ArcLine.Lower:
                if(arcLineStatus==ArcLine.Both)
                {
                    m_uiGamePlay.UpperArcFadeIn();
                    m_arcLineState = ArcLine.Both;
                }
                else if(arcLineStatus==ArcLine.Upper)
                {
                    m_uiGamePlay.UpperArcFadeIn();
                    m_uiGamePlay.LowerArcFadeOut();
                    m_arcLineState = ArcLine.Upper;
                }
                break;
            case ArcLine.Upper:
                if (arcLineStatus == ArcLine.Both)
                {
                    m_uiGamePlay.LowerArcFadeIn();
                    m_arcLineState = ArcLine.Both;
                }
                else if (arcLineStatus == ArcLine.Lower)
                {
                    m_uiGamePlay.LowerArcFadeIn();
                    m_uiGamePlay.UpperArcFadeOut();
                    m_arcLineState = ArcLine.Lower;
                }
                break;
            case ArcLine.Both:
                if (arcLineStatus == ArcLine.Upper)
                {
                    m_uiGamePlay.LowerArcFadeOut();
                    m_arcLineState = ArcLine.Upper;
                }
                else if (arcLineStatus == ArcLine.Lower)
                {
                    m_uiGamePlay.UpperArcFadeOut();
                    m_arcLineState = ArcLine.Lower;
                }
                break;
            case ArcLine.None:
            default:
                break;
        }
    }

    private void SetSubStartNoteMark()
    {
        if (!m_uiGamePlay.NeedShowNoteMark())
            return;
        /*if (m_arcLineState == ArcLine.Lower || m_arcLineState == ArcLine.None)
            return;*/

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

    public void ShowParticle(EParticleType type, Transform tran)
    {
        if (m_musicGameSys.ScoreCont.ComboCount > 0)
        {
            switch (type)
            {
                case EParticleType.Fantastic:
                case EParticleType.Great:
                    // 換成Combo的Particle
                    if (m_musicGameSys.ScoreCont.ComboCount % GameDefine.COMBO_MOD == 0)
                    {
                        type = EParticleType.Combo;
                    }
                    m_uiGamePlay.ComboShot(tran.position);
                    break;
                case EParticleType.Double:
                case EParticleType.SlideMove:
                    m_uiGamePlay.ComboShot(tran.position);
                    break;
                default: break;
            }
        }
        GameObject go = m_uiGamePlay.ParticleDequeue(type);
        go.transform.position = tran.position;
        ParticleInstance pi = go.GetComponent<ParticleInstance>();
        pi.initParticle();

        m_uiGamePlay.StartCoroutine(ReleaseParticle(type, go));
    }
    public void ShowHitJudge(HitJudgeType hitJudgeType, Transform tran)
    {
        HitJudgeIcon hitJudgeIcon = m_uiGamePlay.CreateHitJudgeIcon(hitJudgeType);
        hitJudgeIcon.transform.position = new Vector3(tran.position.x, tran.position.y, hitJudgeIcon.transform.position.z);
        hitJudgeIcon.textureJudge.depth = GameDefine.DEFAULT_JudgeIcon_DEPTH;
        hitJudgeIcon.PlayJudgeIcon();
    }
    public IEnumerator ReleaseParticle(EParticleType type, GameObject go, int time=3)
    {
        yield return new WaitForSeconds(time);
        if (!isPlaying)
            yield break;

        m_uiGamePlay.ParticleEnqueue(type, go);
    }

    public void Replay()
    {
        if (m_iTempCombo != 0)
        {
            m_bInitialCheck = false;
        }
        StopBGM();
        m_bFirstPlay = true;
        m_bReplay = false;
        m_musicGameSys.PlayTime = m_fPlayTimeByDelta;
        BattleStart();
    }

    public void PlayBGM()
    {
        m_uiGamePlay.m_spResumeNumer.enabled = false;
        m_audioController.PlayBGM(null, false);
        m_audioController.SetBGMLoop(false);
        ChangeButtonPauseSprite();
        m_bStopByEditor = false;
    }

    public void PlayBGMByEditor()
    {
        m_lastTime = DateTime.Now;
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        PlayBGM();             
    }

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
        m_musicGameSys.MusicPlayData.CurrentNoteIndex = 0;
        m_musicGameSys.InitializeGame();
        m_musicGameSys.PlayLength = m_audioController.GetBGMLength();
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        m_musicGameSys.PlayTime = m_fPlayTimeByDelta;
        m_bStopByEditor = true;
    }

    public void StopBGMByEditor()
    {
        StopBGM();
        //m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        //m_musicGameSys.PlayTime = m_fPlayTimeByDelta;
        m_bFirstPlay = true;        
    }

    public void ResumeGame()
    {
        m_mainApp.MusicApp.StartCoroutine(ResumeProcess());
    }

    IEnumerator ResumeProcess()
    {
        TweenScale ts = m_uiGamePlay.m_spResumeNumer.GetComponent<TweenScale>();
        m_uiGamePlay.m_spResumeNumer.enabled = true;
        Softstar.Utility.ChangeScoreNumberSprite(m_uiGamePlay.m_spResumeNumer, 3);     
        ts.PlayForward();
        ts.ResetToBeginning();
        yield return new WaitForSeconds(1f);
        Softstar.Utility.ChangeScoreNumberSprite(m_uiGamePlay.m_spResumeNumer, 2);
        yield return new WaitForSeconds(1f);
        Softstar.Utility.ChangeScoreNumberSprite(m_uiGamePlay.m_spResumeNumer, 1);
        yield return new WaitForSeconds(1f);
        m_uiGamePlay.m_spResumeNumer.enabled = false;
        ResumeBGM();
        ts.enabled = false;
    }

    public void ResumeBGM()
    {
        m_ui3DBattleBG.SwitchResume(true);                
        m_bResume = false;
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        m_lastTime = DateTime.Now;
        PlayBGM();
    }

    public void PauseBGM()
    {
        m_audioController.PauseBGM();
        m_bStopByEditor = true;
        ChangeButtonPauseSprite();        
    }

    public void PauseBGMByEditor()
    {
        PauseBGM();        
    }

    public void PlayBGMInLocal(float ATime,float BTime)
    {
        for (int i = 0; i < m_musicGameSys.MusicPlayData.Notes.Count; i++)
        {
            if (m_musicGameSys.MusicPlayData.Notes[i].NotePerfectHitTime > ATime)
            {
                m_musicGameSys.MusicPlayData.CurrentNoteIndex = i;
                break;
            }
        }
        m_musicGameSys.PlayLength = BTime;
        m_audioController.PlayBGM();
        m_audioController.SetBGMPlayTime(ATime);
        m_fPlayTimeByDelta = m_audioController.GetBGMPlayTime();
        m_bStopByEditor = false;
    }

    public bool BGMIsPlay()
    {
        return m_audioController.BGMIsPlay();
    }

    public void ResetSheetMusicData(MusicData _musicData,AudioClip _audioClip)
    {
        // 讀曲譜面       
        m_musicGameSys.MusicPlayData = _musicData;
        // 讀取音樂             
        m_audioController.SetBGM(_audioClip);
    }

    // 根據類型播放點擊音效
    public void PlayNoteSoundEffect(EParticleType type)
    {
        PlayerDataSystem dataSys = m_mainApp.GetSystem<PlayerDataSystem>();
        bool useLowVolume = dataSys.InquireSongIsUseLowVolume(dataSys.GetCurrentSongData().SongGroupID);
        switch (type)
        {
            case EParticleType.Miss:
                break;
            case EParticleType.Double:
                m_soundSystem.PlayDoubleClickNoteSound(useLowVolume);
                break;
            case EParticleType.SlideMove:
                m_soundSystem.PlaySmallNoteSlideSound(useLowVolume);
                break;
            case EParticleType.DragMove:
                m_soundSystem.PlayDragNoteSound(useLowVolume);
                break;
            default:
                m_soundSystem.PlayOneClickNoteSound(useLowVolume);
                break;
        }
    }
    //==========================================================================

#if UNITY_EDITOR
    private static Dictionary<KeyCode, int> m_keyEventCodeMapping;
    public void CheckKeyEvent()
    {
        if(m_keyEventCodeMapping==null)
        {
            m_keyEventCodeMapping = new Dictionary<KeyCode, int>();
            m_keyEventCodeMapping[KeyCode.A] = 0;
            m_keyEventCodeMapping[KeyCode.S] = 1;
            m_keyEventCodeMapping[KeyCode.D] = 2;
            m_keyEventCodeMapping[KeyCode.F] = 3;
            m_keyEventCodeMapping[KeyCode.G] = 4;
            m_keyEventCodeMapping[KeyCode.H] = 5;
            m_keyEventCodeMapping[KeyCode.J] = 6;
            m_keyEventCodeMapping[KeyCode.K] = 7;
            m_keyEventCodeMapping[KeyCode.L] = 8;
            m_keyEventCodeMapping[KeyCode.Semicolon] = 9;
            m_keyEventCodeMapping[KeyCode.DoubleQuote] = 10;
            m_keyEventCodeMapping[KeyCode.Quote] = 10;

            m_keyEventCodeMapping[KeyCode.Q] = 11;
            m_keyEventCodeMapping[KeyCode.W] = 12;
            m_keyEventCodeMapping[KeyCode.E] = 13;
            m_keyEventCodeMapping[KeyCode.R] = 14;
            m_keyEventCodeMapping[KeyCode.T] = 15;
            m_keyEventCodeMapping[KeyCode.Y] = 16;
            m_keyEventCodeMapping[KeyCode.U] = 17;
            m_keyEventCodeMapping[KeyCode.I] = 18;
            m_keyEventCodeMapping[KeyCode.O] = 19;
            m_keyEventCodeMapping[KeyCode.P] = 20;
            m_keyEventCodeMapping[KeyCode.LeftBracket] = 21;
        }
        foreach (KeyValuePair<KeyCode, int> kv in m_keyEventCodeMapping)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                GameObject go = m_uiGamePlay.GameLineList[kv.Value].hitPoint;
                OnNotePress(go, true);
            }

            if (Input.GetKeyUp(kv.Key))
            {
                GameObject go = m_uiGamePlay.GameLineList[kv.Value].hitPoint;
                OnNotePress(go, false);
            }
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            m_uiGamePlay.StartCoroutine(PlayFullComboAnim());
        }
    }
#endif
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    { 
        UIEventListener.Get(m_uiGamePlay.m_buttonPause.gameObject).onClick = OnButtonPauseClick;

        for (int i = 0; i < m_uiGamePlay.GameLineList.Count; i++)
        {
            UIEventListener.Get(m_uiGamePlay.GameLineList[i].hitPoint).onPress = OnNotePress;
            UIEventListener.Get(m_uiGamePlay.GameLineList[i].hitPoint).onDragOver = OnNoteDragOver;         
        }
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        UIEventListener.Get(m_uiGamePlay.m_buttonPause.gameObject).onClick = null;

        for (int i = 0; i < m_uiGamePlay.GameLineList.Count; i++)
        {
            UIEventListener.Get(m_uiGamePlay.GameLineList[i].hitPoint).onPress = null;
            UIEventListener.Get(m_uiGamePlay.GameLineList[i].hitPoint).onDragOver = null;
        }
    }
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
    //---------------------------------------------------------------------------------------------------
    private void OnButtonPauseClick(GameObject go)
    {
        if (m_mainApp.EnvironmentType == Enum_EnvironmentType.NormalEnvironment)
            m_ui3DBattleBG.SwitchGamePause(true);

        m_mainApp.PushState(StateName.GAME_PAUSE_STATE);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    private void ChangeButtonPauseSprite()
    {
        m_uiGamePlay.m_buttonPause.gameObject.SetActive(BGMIsPlay());
    }
    //---------------------------------------------------------------------------------------------------
    private void SetSongPlayData()
    {
        PlayerDataSystem dataSys = m_mainApp.GetSystem<PlayerDataSystem>();
        SongPlayData playData = dataSys.GetCurrentSongPlayData();
        Softstar.ScoreController scoreController = m_musicGameSys.ScoreCont;

        //P.S 移置最後一個NOTE結束後執行
        //計算最後一批Combo的分數
        //scoreController.CountComboScore(scoreController.ComboCount);

        playData.Score = (int)scoreController.Score;
        playData.Rank = scoreController.Rank;
        playData.Combo = scoreController.MaxCombo;
        playData.SetPlayClickCount(ScoreType.Fantastic, scoreController.FantasicCount);
        playData.SetPlayClickCount(ScoreType.Great, scoreController.GreatCount);
        playData.SetPlayClickCount(ScoreType.Weak, scoreController.WeakCount);
        playData.SetPlayClickCount(ScoreType.Lost, scoreController.LostCount);
    }
    //---------------------------------------------------------------------------------------------------
    private void BattleFinish()
    {        
        SetSongPlayData();
        StopBGM();

        m_uiGamePlay.Hide();

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, false);
        m_mainApp.PushStateByScreenShot(StateName.BATTLE_RESULT_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void PlayComboDynamicBG(int combo)
    {
        switch (combo)
        {
            case GameDefine.COMBO_EFFECT_CONDITION_3:
                m_ui3DBattleBG.m_animator.Play(GameDefine.COMBO_ANIMATION_STATE_NAME_3);
                break;
            case GameDefine.COMBO_EFFECT_CONDITION_2:
                m_ui3DBattleBG.m_animator.Play(GameDefine.COMBO_ANIMATION_STATE_NAME_2);
                break;
            case GameDefine.COMBO_EFFECT_CONDITION_1:
                m_ui3DBattleBG.m_animator.Play(GameDefine.COMBO_ANIMATION_STATE_NAME_1);
                break;
            default:
                break;
        }
    }                 
    //---------------------------------------------------------------------------------------------------
    void LastNoteComboProcess(AbstractNote note)
    {
        if (note == m_musicGameSys.MusicPlayData.Notes[m_musicGameSys.MusicPlayData.Notes.Count - 1])
        {     
            //計算最後一批Combo的分數  
            Softstar.ScoreController scoreController = m_musicGameSys.ScoreCont;                 
            scoreController.CountComboScore(scoreController.ComboCount);
            scoreController.ComboCount = 0;
            //停止分數自動更新，所以手動更新
            CloseUpadteScoreTimer();
            m_uiGamePlay.SetScoreValue((int)m_musicGameSys.ScoreCont.Score);
            //FullCombo時播放動畫
            if (scoreController.MaxCombo >= m_musicGameSys.MusicPlayData.NotesCount)
                m_uiGamePlay.StartCoroutine(PlayFullComboAnim());
                                        
            //目前不使用延遲效果
            //CountDownSystem cdSys = m_mainApp.GetSystem<CountDownSystem>();
            //m_cdTime = cdSys.StartCountDown("m_cdKey_LastNote", 1f, CDLastNoteProcess);       
        }
    }
    //---------------------------------------------------------------------------------------------------
    void CDLastNoteProcess(string Key)
    {
        //計算最後一批Combo的分數  
        Softstar.ScoreController scoreController = m_musicGameSys.ScoreCont;
        scoreController.CountComboScore(scoreController.ComboCount);
        scoreController.ComboCount = 0;
        CloseCDLastNoteProcess();
    }
    //---------------------------------------------------------------------------------------------------
    void CloseCDLastNoteProcess()
    {
        if(m_cdTime != null)
        {
            CountDownSystem cdSys = m_mainApp.GetSystem<CountDownSystem>();
            cdSys.CloseCountDown(m_cdKey_LastNote);
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void RealseRunTimeData()
    {
        m_iTempCombo = 0;
        m_iTempScore = 0;
        m_iHitNoteDepth = 1;
        m_uiBarQueue = 0;
        m_bReplay = false;
        m_bResume = false;
        m_bStopByEditor = false;
        m_tempMusicData = null;
        m_bFirstPlay = true;
        m_bFullComboAnim = false;
        CloseUpadteScoreTimer();
    }
    //---------------------------------------------------------------------------------------------------
    private void CloseUpadteScoreTimer()
    {
        if (m_updateScoreTimer != null)
        {
            m_mainApp.GetSystem<CountDownSystem>().CloseCountDown(GameDefine.TICEKT_UPDATE_SCORE);
            m_updateScoreTimer = null;
        }
    }
    //---------------------------------------------------------------------------------------------------
    //播放FullCombo動畫
    IEnumerator PlayFullComboAnim()
    {
        m_bFullComboAnim = true;
        m_uiGamePlay.m_objFullCombo.SetActive(true);
        do
        {
            yield return null;
        } while (m_uiGamePlay.m_animFullCombo.isPlaying);

        m_uiGamePlay.m_objFullCombo.SetActive(false);
        m_bFullComboAnim = false;

        if ((m_musicGameSys.PlayTime >= (m_musicGameSys.PlayLength - 0.1f)) || // 音樂結束前的0.1秒結束遊戲 或
            m_bFadeOutFlag) // Fade out 完成而結束遊戲
        {
            BattleFinish();
        }            
    }
}
