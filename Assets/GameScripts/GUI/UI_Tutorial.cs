using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using System;

public class UI_Tutorial : NGUIChildGUI, IGamePlayGUI
{
    Transform IGamePlayGUI.transform
    {
        get { return this.transform; }
    }
    public bool AutoBattle = false;
    [Header("Other UI")]
    public UISprite m_spriteLowerArc;
    public UILabel m_labelSongName;

    [Header("Game Play UI")]
    public UIPanel m_panelBase;

    public Dictionary<NoteType, GameObject> NoteBaseObjectMap { get; set; }

    public GameObject NotePool;
    public GameObject ActiveNotePool;
    public Dictionary<NoteType, Queue<GameObject>> NotePoolMap;
    public Dictionary<EParticleType, GameObject> ParticleBaseObjectMap { get; set; }

    public GameObject ComboBulletShot { get; set; }

    public GameObject ComboBulletEffect { get; set; }

    public GameObject ComboFinishShot { get; set; }

    public Dictionary<ComboEffectLevel, GameObject> ComboEffectMap { get; set; }

    public Dictionary<EParticleType, Queue<GameObject>> ParticlePoolMap;
    public Dictionary<HitJudgeType, GameObject> HitJudgeBaseObjectMap { get; set; }

    public Dictionary<NoteLineType, Material> NoteLineMaterialMap { get; set; }

    private List<GameLine> m_gameLineList;
    public List<GameLine> GameLineList { get { return m_gameLineList; } }

    public GameObject m_lowerArcPosition;
    public GameObject m_upperArcPosition;
    public UITweener m_lowerArcTweener;
    public UITweener m_upperArcTweener;

    public GameDataDB GameDB { set; get; }
    public PlayerDataSystem playerDataSystem { set; get; }

    public DoubleFadeOut doubleFadeOut;
    public float m_DragLineScaleOffset = 15f;
    public float DragLineScaleOffset { get { return m_DragLineScaleOffset; } }

    //-------------------------------------------------------------------------------------------------
    public UI_Tutorial() : base()
    {
    }
    //-------------------------------------------------------------------------------------------------
    void Awake()
    {
        m_gameLineList = new List<GameLine>();

        NoteBaseObjectMap = new Dictionary<NoteType, GameObject>();
        NotePoolMap = new Dictionary<NoteType, Queue<GameObject>>();

        ParticleBaseObjectMap = new Dictionary<EParticleType, GameObject>();
        ParticlePoolMap = new Dictionary<EParticleType, Queue<GameObject>>();

        HitJudgeBaseObjectMap = new Dictionary<HitJudgeType, GameObject>();

        ComboEffectMap = new Dictionary<ComboEffectLevel, GameObject>();
        NoteLineMaterialMap = new Dictionary<NoteLineType, Material>();
    }
    //-------------------------------------------------------------------------------------------------    
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Hide()
    {
        base.Hide();
    }
    //-------------------------------------------------------------------------------------------------
    public override void UiUpdate()
    {
        base.UiUpdate();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI()
    {
        AutoBattle = false;
        m_labelSongName.text = playerDataSystem.GetSongName(playerDataSystem.GetCurrentSongData());
        S_TextureTable_Tmp textureTmp = GameDB.GetGameDB<S_TextureTable_Tmp>().GetData(56);   //Combo字樣貼圖  
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeHitPoint()
    {
        for (int i = 0; i < GameLineList.Count; i++)
        {
            GameLineList[i].hitPoint.SetActive(false);
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeArcPosition()
    {
        int iLowerIndex = "LR".ToLineOrder();
        int iUpperIndex = "SLR".ToLineOrder();
        Vector3 lowerPos = GameLineList[iLowerIndex].hitPoint.transform.position;
        Vector3 upperPos = GameLineList[iUpperIndex].hitPoint.transform.position;
        // 上下弧線Sprite放置於LowerArcPosition及UpperArcPosition空GameObject之內
        // 並且將此GameObject定位在點擊點的位置，藉此對齊點集點與弧線
        m_lowerArcPosition.transform.position = lowerPos;
        m_upperArcPosition.transform.position = upperPos;
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeNoteObject()
    {
        foreach (KeyValuePair<NoteType, GameObject> kv in NoteBaseObjectMap)
        {
            for (int i = 0; i < UI_GamePlay.NOTE_BASE_AMOUNT; i++)
            {
                if (NotePoolMap.ContainsKey(kv.Key) == false)
                {
                    NotePoolMap[kv.Key] = new Queue<GameObject>();
                }
                CreateAndEnqueue(kv.Key);
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void CreateAndEnqueue(NoteType noteType)
    {
        GameObject go = GameObject.Instantiate<GameObject>(NoteBaseObjectMap[noteType]);
        Enqueue(noteType, go);
    }
    //-------------------------------------------------------------------------------------------------
    public void Enqueue(NoteType noteType, GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(NotePool.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        NotePoolMap[noteType].Enqueue(go);
    }
    //-------------------------------------------------------------------------------------------------
    public GameObject Dequeue(NoteType noteType)
    {
        if (NotePoolMap[noteType].Count <= 0)
        {
            CreateAndEnqueue(noteType);
        }
        GameObject go = NotePoolMap[noteType].Dequeue();
        go.transform.SetParent(ActiveNotePool.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = new Vector3(99999, 99999, 0); // 初始在螢幕外面
        go.SetActive(true);
        return go;
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeParticleObject()
    {
        foreach (KeyValuePair<EParticleType, GameObject> kv in ParticleBaseObjectMap)
        {
            if (ParticlePoolMap.ContainsKey(kv.Key) == false)
            {
                ParticlePoolMap[kv.Key] = new Queue<GameObject>();
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void LowerArcFadeIn()
    {
        m_lowerArcTweener.enabled = true;
        m_lowerArcTweener.PlayForward();
        m_lowerArcTweener.ResetToBeginning();
    }
    //-------------------------------------------------------------------------------------------------
    public void LowerArcFadeOut()
    {
        m_lowerArcTweener.enabled = true;
        m_lowerArcTweener.PlayReverse();
        m_lowerArcTweener.ResetToBeginning();
    }
    //-------------------------------------------------------------------------------------------------
    public void UpperArcFadeIn()
    {
        m_upperArcTweener.enabled = true;
        m_upperArcTweener.PlayForward();
        m_upperArcTweener.ResetToBeginning();
    }
    //-------------------------------------------------------------------------------------------------
    public void UpperArcFadeOut()
    {
        m_upperArcTweener.enabled = true;
        m_upperArcTweener.PlayReverse();
        m_upperArcTweener.ResetToBeginning();
    }
    //-------------------------------------------------------------------------------------------------
    public DoubleFadeOut CreateDoubleFadeOutObj(Vector3 vecA, Vector3 vecB, int depth)
    {
        GameObject obj = GameObject.Instantiate(doubleFadeOut.gameObject);
        obj.transform.parent = ActiveNotePool.transform;
        obj.transform.localScale = Vector3.one;
        DoubleFadeOut fadeout = obj.GetComponent<DoubleFadeOut>();
        fadeout.Init();
        fadeout.StartUp(vecA, vecB, GameLineList[5].hitPoint.transform.position, depth);
        return fadeout;
    }
    //-------------------------------------------------------------------------------------------------
    public void ParticleCreateEnqueue(EParticleType type)
    {
        GameObject go = GameObject.Instantiate<GameObject>(ParticleBaseObjectMap[type]);
        ParticleEnqueue(type, go);
    }
    //-------------------------------------------------------------------------------------------------
    public void ParticleEnqueue(EParticleType type, GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(NotePool.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        ParticlePoolMap[type].Enqueue(go);
    }
    //-------------------------------------------------------------------------------------------------
    public GameObject ParticleDequeue(EParticleType type)
    {
        if (ParticlePoolMap[type].Count <= 0)
        {
            ParticleCreateEnqueue(type);
        }
        GameObject go = ParticlePoolMap[type].Dequeue();
        go.transform.SetParent(ActiveNotePool.transform);
        go.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;
        go.transform.localPosition = new Vector3(99999, 99999, 0); // 初始在螢幕外面
        go.SetActive(true);
        return go;
    }
    //-------------------------------------------------------------------------------------------------
    public HitJudgeIcon CreateHitJudgeIcon(HitJudgeType type)
    {
        GameObject go = GameObject.Instantiate<GameObject>(HitJudgeBaseObjectMap[type]);
        go.transform.SetParent(NotePool.transform);
        go.transform.localScale = Vector3.one;
        return go.GetComponent<HitJudgeIcon>();
    }
}
