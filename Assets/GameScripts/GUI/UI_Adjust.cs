using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_Adjust : NGUIChildGUI
{
    public UILabel m_LabelAdjustNumber;
    public UILabel m_LabelSecond;
    public UIButton m_ButtonIncrease;
    public UIButton m_ButtonDecrease;
    public UIWidget m_BackGround;

    public GameObject NotePool;
    public GameObject ActiveNotePool;
    public Dictionary<NoteType, GameObject> NoteBaseObjectMap;
    public Dictionary<NoteType, Queue<GameObject>> NotePoolMap;

    public Dictionary<EParticleType, GameObject> ParticleBaseObjectMap;
    public Dictionary<EParticleType, Queue<GameObject>> ParticlePoolMap;
    public Dictionary<HitJudgeType, GameObject> HitJudgeBaseObjectMap; // Perfect, Good, Weak, Miss效果文字Base物件
    public Transform v0;
    public Transform v1;
    public Transform v2;
    public Transform v3;

    public Vector3 HitPointSize;

    public int TambourineClipIndex = 0;
    public List<AudioClip> TambourineClipList;

    private List<GameLine> m_gameLineList;
    public List<GameLine> GameLineList { get { return m_gameLineList; } }

    //-------------------------------------------------------------------------------------------------
    private UI_Adjust() : base()
    {
    }
    //-------------------------------------------------------------------------------------------------
    void Awake()
    {
    }
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(StringTable stringTable)
    {
        m_LabelSecond.text = stringTable.GetString(32);  //秒

        m_gameLineList = new List<GameLine>();

        NoteBaseObjectMap = new Dictionary<NoteType, GameObject>();
        NotePoolMap = new Dictionary<NoteType, Queue<GameObject>>();

        ParticleBaseObjectMap = new Dictionary<EParticleType, GameObject>();
        ParticlePoolMap = new Dictionary<EParticleType, Queue<GameObject>>();

        //HitJudgeBaseObjectMap = new Dictionary<HitJudgeType, GameObject>();

        TambourineClipList = new List<AudioClip>();
        HitJudgeBaseObjectMap = new Dictionary<HitJudgeType, GameObject>();
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
    public void SetAdjustNumber(float variable)
    {
        //取小數點第一位
        variable = Mathf.RoundToInt(variable * 100) / 100.0f;
        string sign = (variable >= 0) ? "+" : "";
        m_LabelAdjustNumber.text = sign + string.Format("{0:0.00}", variable).ToString();
    }

    //-------------------------------------------------------------------------------------------------
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
    public void CreateAndEnqueue(NoteType noteType)
    {
        GameObject go = GameObject.Instantiate<GameObject>(NoteBaseObjectMap[noteType]);
        Enqueue(noteType, go);
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
        go.transform.localScale = Vector3.one * 80;
        go.transform.localPosition = new Vector3(99999, 99999, 0); // 初始在螢幕外面
        go.SetActive(true);
        /*
        ParticleInstance pi = go.GetComponent<ParticleInstance>();
        if (pi != null)
            pi.SetRenderQueue(m_BackGround);
        */
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
