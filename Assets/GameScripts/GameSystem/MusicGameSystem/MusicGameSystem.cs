using UnityEngine;
using System.Collections.Generic;
using Softstar;

public class MusicGameSystem : BaseSystem
{
    private MainApplication m_MainApp;

    private MusicData m_musicData;
    public MusicData MusicPlayData
    {
        set { m_musicData = value; }
        get { return m_musicData; }
    }

    public Softstar.ScoreController ScoreCont{ private set; get; }
    public NoteCreater NoteCreate { private set; get; }
    public ParticleCreater ParticleCreate { private set; get; }

    public List<AbstractNote> Notes;

    public float PlaySpeed { set; get; }

    public float PlayTime { set; get; }
    public float PlayLength { set; get; }

    private float m_NoteTimeOffset;
    public float NoteTimeOffset
    {
        set { m_NoteTimeOffset = value; }
        get { return m_NoteTimeOffset; }      
    }

    //RuntimeData
    private int m_iTempCombo = -1;
    private int m_iHitNoteDepth = 1;
    private int m_uiBarQueue = 0;
    private float m_fLastCheckHitpointTime = 0;

    //-----------------------------------------------------------------------------------------------------------
    public MusicGameSystem(GameScripts.GameFramework.GameApplication app) : base(app)
    {
        m_MainApp = app as MainApplication;
        Notes = new List<AbstractNote>();
        ScoreCont = new Softstar.ScoreController();
        NoteCreate = new NoteCreater(m_MainApp);
        ParticleCreate = new ParticleCreater();
    }

    public override void Initialize()
    {
        base.Initialize();
        //取得本地端記錄的校正值
        RecordSystem recordSys = m_MainApp.GetSystem<RecordSystem>();
        NoteTimeOffset = recordSys.GetNoteAdjust();
    }

    //-----------------------------------------------------------------------------------------------------------
    public void InitializeGame()
    {
        ScoreCont.initialize();
        Notes.Clear();

        // 設定總Note數，以計算每個Note與Combo的分數
        ScoreCont.TotalNotes = m_musicData.NotesCount;                
    }

    public void LoadMusicData(string musicDataText)
    {
        m_musicData = new MusicData();
        m_musicData.ScoreCont = ScoreCont;
        m_musicData.NoteCreate = NoteCreate;
        m_musicData.ParticleCreate = ParticleCreate;
        m_musicData.FromString(musicDataText);
        m_musicData.PlaySpeed = GameDefine.DEFAULT_PLAY_SPEED * PlaySpeed;
        SetMusicNoteTimeOffset();
    }

    public bool CheckCreateNote()
    {
        if (m_musicData.CurrentNoteIndex < m_musicData.Notes.Count)
        {
            if (PlayTime >= m_musicData.Notes[m_musicData.CurrentNoteIndex].NoteCreateTime)
            {
                return true;
            }
        }
        return false;
    }

   public void SetMusicNoteTimeOffset()
    {
        if(m_musicData != null)
        {
            foreach (AbstractNote n in m_musicData.Notes)
                n.TimeOffset = m_NoteTimeOffset;
        }
    }

    public void SaveNoteOffset()
    {
        RecordSystem recordSys = m_MainApp.GetSystem<RecordSystem>();
        recordSys.SetNoteAdjust(NoteTimeOffset);
    }

    #region 遊戲資源載入
    //---------------------------------------------------------------------------------------------------
    public void LoadGameResource(IGamePlayGUI ui)
    {
        ResourceManager resManager = m_MainApp.GetResourceManager();
        // 建立Notes
        ui.NoteBaseObjectMap[NoteType.Single] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "SingleNote");
        ui.NoteBaseObjectMap[NoteType.Multi] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "MultiNote");
        ui.NoteBaseObjectMap[NoteType.Small] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "SmallNote");
        ui.NoteBaseObjectMap[NoteType.SubVerticalStart] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "SubNote");
        ui.NoteBaseObjectMap[NoteType.SubVerticalEnd] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "LineNote");
        ui.NoteBaseObjectMap[NoteType.SubDouble] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "DoubleNote");
        ui.NoteBaseObjectMap[NoteType.SubDragStart] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "SubNote");
        ui.NoteBaseObjectMap[NoteType.SubDragMiddle] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "LineNote");
        ui.NoteBaseObjectMap[NoteType.SubDragEnd] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "LineNote");
        // 初始化Note物件，預設各個Note各五個
        ui.InitializeNoteObject();

        // 建立Particle
        ui.ParticleBaseObjectMap[EParticleType.Fantastic] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Fantastic");
        ui.ParticleBaseObjectMap[EParticleType.Great] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Great");
        ui.ParticleBaseObjectMap[EParticleType.Weak] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Weak");
        ui.ParticleBaseObjectMap[EParticleType.Miss] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Miss");
        ui.ParticleBaseObjectMap[EParticleType.OneSegment] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/OneSegament");
        ui.ParticleBaseObjectMap[EParticleType.MoreSegment] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/MoreSegament");
        ui.ParticleBaseObjectMap[EParticleType.Swipe] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Swipe");
        ui.ParticleBaseObjectMap[EParticleType.Combo] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/Combo");
        ui.ParticleBaseObjectMap[EParticleType.MoreSegmentSlide] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/MoreSegament_02");
        ui.ParticleBaseObjectMap[EParticleType.SlideMove] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/slidemove");
        ui.ParticleBaseObjectMap[EParticleType.Double] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/doubleclick");
        ui.InitializeParticleObject();

        // Combo特效
        ui.ComboBulletShot = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/combo_bullet");
        ui.ComboBulletEffect = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/combo_0");
        ui.ComboEffectMap[ComboEffectLevel.Lv1] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/combo_1");
        ui.ComboEffectMap[ComboEffectLevel.Lv2] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/combo_2");
        ui.ComboEffectMap[ComboEffectLevel.Lv3] = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/combo_3");
        ui.ComboFinishShot = resManager.GetResourceSync(Enum_ResourcesType.Particle, "Effect/combo_f");

        // 建立效果文字
        ui.HitJudgeBaseObjectMap[HitJudgeType.Miss] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconMiss");
        ui.HitJudgeBaseObjectMap[HitJudgeType.Weak] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconWeak");
        ui.HitJudgeBaseObjectMap[HitJudgeType.Good] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconGood");
        ui.HitJudgeBaseObjectMap[HitJudgeType.Perfect] = resManager.GetResourceSync(Enum_ResourcesType.Notes, "TextureIconPerfect");

        // 讀取Material
        ui.NoteLineMaterialMap[NoteLineType.VerticalNote] = resManager.GetResourceSync<Material>(Enum_ResourcesType.Material, "NoteLine/note_5_l");
        ui.NoteLineMaterialMap[NoteLineType.DoubleNote] = resManager.GetResourceSync<Material>(Enum_ResourcesType.Material, "NoteLine/note_2_l");
        ui.NoteLineMaterialMap[NoteLineType.DrageNote] = resManager.GetResourceSync<Material>(Enum_ResourcesType.Material, "NoteLine/note_4_l");
        ui.NoteLineMaterialMap[NoteLineType.VerticalNotePressB] = resManager.GetResourceSync<Material>(Enum_ResourcesType.Material, "NoteLine/note_6_l");

        // TODO: 修改成從譜面初始化來設定線一開始的型式
        LinePointGenerater lpg = new LinePointGenerater();
        List<LineCurve> lineList = lpg.Generate(
            new Vector2(GameDefine.LINE_ROTATE_X, GameDefine.LINE_ROTATE_Y),
            new Vector2(GameDefine.LINE_COMMON_START_X, GameDefine.LINE_COMMON_START_Y),
            GameDefine.LINE_LOWER_PART_LEN, GameDefine.LINE_UPPER_PART_LEN, GameDefine.LINE_RADIAN_DIFF);

        GameObject gameLineObj = resManager.GetResourceSync(Enum_ResourcesType.Notes, "GameLine");
        for (int i = 0; i < lineList.Count; i++) // TODO: 暫訂6條線，須修改MusicData格式以讀入本次遊戲線數
        {
            LineCurve lc = lineList[i];
            Vector2 v0 = lc.GetPointAtRandianTime(1.0f, 0.0f);
            Vector2 v1 = lc.GetPointAtRandianTime(1.0f, 0.3f);
            Vector2 v2 = lc.GetPointAtRandianTime(1.0f, 0.7f);
            Vector2 v3 = lc.GetPointAtRandianTime(1.0f, 1.0f);

            GameObject go = Object.Instantiate<GameObject>(gameLineObj);
            go.transform.SetParent(ui.transform);
            go.transform.localScale = Vector3.one;
            GameLine gameLine = go.GetComponent<GameLine>();
            gameLine.v0.transform.position = new Vector3(v0.x, v0.y, -1.5f);
            gameLine.v1.transform.position = new Vector3(v1.x, v1.y, -1.5f);
            gameLine.v2.transform.position = new Vector3(v2.x, v2.y, -1.5f);
            gameLine.v3.transform.position = new Vector3(v3.x, v3.y, -1.5f);
            gameLine.RefreshPosition();
            gameLine.LineID = i;
            HitPointHandle(gameLine, i);
            ui.GameLineList.Add(gameLine);
        }
        ui.InitializeHitPoint();
        ui.InitializeArcPosition();
    }
    //---------------------------------------------------------------------------------------------------
    //觸碰區大小及角度調整
    void HitPointHandle(GameLine gameLine, int lineID)
    {
        //只調整下排大小
        if (lineID < (int)emLineOrder.S_L3)
        {
            if (lineID % 2 == 0)
                gameLine.hitPoint.GetComponent<BoxCollider>().size = new Vector3(120, 300, 1);
            else
                gameLine.hitPoint.GetComponent<BoxCollider>().size = new Vector3(80, 300, 1);

            if (lineID == 0 || lineID == 10)
                gameLine.hitPoint.GetComponent<BoxCollider>().size = new Vector3(160, 300, 1);

            lineID -= (int)emLineOrder.LR;
        }
        else
        {
            if (lineID % 2 != 0)
                gameLine.hitPoint.GetComponent<BoxCollider>().size = new Vector3(120, 300, 1);
            else
                gameLine.hitPoint.GetComponent<BoxCollider>().size = new Vector3(80, 300, 1);
            lineID -= (int)emLineOrder.S_LR;
        }

        gameLine.hitPoint.transform.Rotate(0, 0, lineID * GameDefine.LINE_RADIAN);
    }
    #endregion

    #region GamePlayTool
    //---------------------------------------------------------------------------------------------------
    public void InitRuntimeData(int HitNoteDepth, int uiBarQueue)
    {
        m_iHitNoteDepth = HitNoteDepth;
        m_uiBarQueue = uiBarQueue;
    }
    //---------------------------------------------------------------------------------------------------
    //設定UI深度(NGUI)
    public void SetHitPointDepth(AbstractNote note)
    {
        note.LinePointDepth = m_iHitNoteDepth;
        --m_iHitNoteDepth;
    }
    //---------------------------------------------------------------------------------------------------
    //設定shader深度
    public void SetShaderQueue(Material m, int offset)
    {
        if (m != null)
            m.renderQueue = m_uiBarQueue + offset;
    }
    //---------------------------------------------------------------------------------------------------
    //設定線段材質
    public void SetLineMaterial(AbstractNote note, LineRenderer lr, NoteType type,IGamePlayGUI ui)
    {
        switch (type)
        {
            //case NoteType.SubDragStart:
            case NoteType.SubDragMiddle:
            case NoteType.SubDragEnd:
                //case NoteType.TrailDragHorizontal:
                SubHorizontalNote subNote = note as SubHorizontalNote;
                Vector3 srcPos = ui.GameLineList[subNote.PreviousNote.LineOrder].hitPoint.transform.position;
                if (subNote.PreviousNote.GetType() == typeof(SubHorizontalNote))
                {
                    srcPos = ui.GameLineList[((SubHorizontalNote)subNote.PreviousNote).TargetLineOrder].hitPoint.transform.position;
                }
                Vector3 dstPos = ui.GameLineList[subNote.TargetLineOrder].hitPoint.transform.position;
                float m = Vector3.Distance(dstPos, srcPos);
                //UnityDebugger.Debugger.Log("Vector3.Distance: " + m);
                m *= ui.DragLineScaleOffset;
                lr.material = ui.NoteLineMaterialMap[NoteLineType.DrageNote];
                lr.material.mainTextureScale = new Vector2(m, 1);
                break;
            case NoteType.SubVerticalEnd:
                SubVerticalNote subV = note as SubVerticalNote;
                if (subV.pressType == PressType.TypeA)
                    lr.material = ui.NoteLineMaterialMap[NoteLineType.VerticalNote];
                else
                    lr.material = ui.NoteLineMaterialMap[NoteLineType.VerticalNotePressB];

                lr.material.mainTextureScale = Vector2.one;
                break;
            case NoteType.SubDouble:
                lr.material = ui.NoteLineMaterialMap[NoteLineType.DoubleNote];
                lr.material.mainTextureScale = Vector2.one;
                break;
            case NoteType.SubVerticalStart:
            default:
                break;
        }
    }
    //---------------------------------------------------------------------------------------------------
    public void CheckCloseGameLineCollider(int iLineID,IGamePlayGUI ui)
    {
        if (ui.GameLineList[iLineID].hitPoint.activeSelf == false)
            return;

        for (int i = 0; i < Notes.Count; i++)
        {
            if (Notes[i].Type == NoteType.Multi)
            {
                MultiNote mu = Notes[i] as MultiNote;
                foreach (SubNote subNote in mu.SubNotes)
                {
                    //依LineOrder開啟對應HitPoint
                    if (subNote.Type == NoteType.SubVerticalEnd)
                    {
                        if ((subNote as SubVerticalNote).TrailType == NoteType.TrailDragVertical)
                            if ((subNote as SubVerticalNote).TargetLine == iLineID)
                                return;
                    }
                    else if (subNote.Type == NoteType.SubDragEnd || subNote.Type == NoteType.SubDragMiddle)
                    {
                        if ((subNote as SubHorizontalNote).TargetLineOrder == iLineID)
                            return;
                    }
                    else
                        if (subNote.LineOrder == iLineID)
                        return;
                }
            }
            else
            {
                if (Notes[i].LineOrder == iLineID)
                    return;
            }
        }

        ui.GameLineList[iLineID].hitPoint.SetActive(false);
        //UnityDebugger.Debugger.Log("HitPoint:" + iLineID + " is close");
    }
    //---------------------------------------------------------------------------------------------------
    public void OpenGameLineCollider(int iLineID, IGamePlayGUI ui)
    {
        ui.GameLineList[iLineID].hitPoint.SetActive(true);
        //UnityDebugger.Debugger.Log("HitPoint:" + iLineID + " is open");
    }
    //---------------------------------------------------------------------------------------------------
    public void TimeCheckCloseHitPoint(IGamePlayGUI ui)
    {
        int count = ui.GameLineList.Count;
        float nowTime = Time.time;
        if (nowTime - m_fLastCheckHitpointTime > 0.5f)
        {
            for (int i = 0; i < count; i++)
            {
                CheckCloseGameLineCollider(i, ui);
            }
            m_fLastCheckHitpointTime = nowTime;
        }
    }
    //---------------------------------------------------------------------------------------------------
    //Note事件處理
    public void SignalAllNotes(ControlSignal signal)
    {
        for (int i = 0; i < Notes.Count; i++)
        {
            AbstractNote note = Notes[i];
            NoteHandleReturn ret = note.HandleControl(signal);
            if (ret != NoteHandleReturn.NotHandle)
            {
                if (ret == NoteHandleReturn.HandleAndDestroy)
                {
                    
                }
                break;
            }
        }
    }
    //---------------------------------------------------------------------------------------------------
    //自動戰鬥流程
    public void AutoBattle(AbstractNote note, float fplaytime)
    {
        if (note.Type == NoteType.Multi)
        {
            MultiNote mu = note as MultiNote;
            foreach (AbstractNote subnote in mu.SubNotes)
            {
                if (subnote.State == NoteState.Alive)
                {
                    switch (subnote.Type)
                    {
                        case NoteType.SubDragStart:
                        case NoteType.SubVerticalStart:
                            if (subnote.CheckOnTrigger() && !subnote.bIsHandle)
                            {
                                ControlSignal c = new ControlSignal(ControlSignalType.ControllDown, -1, subnote.LineOrder, subnote.LineOrder, subnote.LineOrder);
                                SignalAllNotes(c);
                            }

                            break;
                        case NoteType.SubVerticalEnd:
                            {
                                SubVerticalNote vnote = subnote as SubVerticalNote;
                                if (vnote.TrailType == NoteType.TrailDragVertical)
                                {
                                    if (vnote.pressType == PressType.TypeA)
                                    {
                                        if (fplaytime >= vnote.NotePerfectEndTime - 0.1f)
                                        {
                                            int src = vnote.PreviousNote.LineOrder;
                                            ControlSignal c = new ControlSignal(ControlSignalType.ControllSwipe, -1, src, src, vnote.TargetLine);
                                            SignalAllNotes(c);
                                        }
                                    }
                                    else
                                    {
                                        if (fplaytime >= vnote.NotePerfectEndTime)
                                        {
                                            int src = vnote.PreviousNote.LineOrder;
                                            ControlSignal c = new ControlSignal(ControlSignalType.ControllSwipe, -1, src, src, vnote.TargetLine);
                                            SignalAllNotes(c);
                                            c = new ControlSignal(ControlSignalType.ControllUp, -1, src, src, vnote.TargetLine);
                                            SignalAllNotes(c);
                                        }
                                    }
                                }
                                else
                                {
                                    if (fplaytime >= vnote.NotePerfectEndTime)
                                    {
                                        ControlSignal c = new ControlSignal(ControlSignalType.ControllUp, -1, vnote.LineOrder, vnote.LineOrder, vnote.LineOrder);
                                        SignalAllNotes(c);
                                    }
                                }
                            }
                            break;
                        case NoteType.SubDragMiddle:
                        case NoteType.SubDragEnd:
                            {
                                SubHorizontalNote hnote = subnote as SubHorizontalNote;
                                if (fplaytime >= hnote.NotePerfectEndTime)
                                {
                                    int src = mu.SubNotes.First.Value.LineOrder;
                                    int from = -1;
                                    if (hnote.PreviousNote.Type == NoteType.SubDragStart)
                                        from = hnote.PreviousNote.LineOrder;
                                    else
                                        from = (hnote.PreviousNote as SubHorizontalNote).TargetLineOrder;
                                    ControlSignal c = new ControlSignal(ControlSignalType.ControllSwipe, -1, src, from, hnote.TargetLineOrder);
                                    SignalAllNotes(c);
                                }
                            }
                            break;
                        case NoteType.SubDouble:
                            if (subnote.CheckOnTrigger())
                            {
                                ControlSignal c = new ControlSignal(ControlSignalType.ControllDown, -1, subnote.LineOrder, subnote.LineOrder, subnote.LineOrder);
                                SignalAllNotes(c);
                            }                                
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            if (note.CheckOnTrigger())
            {
                ControlSignal c = new ControlSignal(ControlSignalType.ControllDown, -1, note.LineOrder, note.LineOrder, note.LineOrder);
                SignalAllNotes(c);
            }
        }
    }    
    #endregion
}
