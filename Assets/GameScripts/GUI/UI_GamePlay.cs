using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Softstar;
using System;

public enum emArcLine
{
    LL, LM, LR,
    UL, UM, UR,
}

public class UI_GamePlay : NGUIChildGUI, IGamePlayGUI
{
    Transform IGamePlayGUI.transform
    {
        get { return this.transform; }
    }
    public bool AutoBattle = false;
    [Header("Other UI")]
    public UIPanel      m_panelBackground;
    public UIButton     m_buttonPause;
    public UILabel      m_labelSongName;
    public GameObject   m_comboContainer;
    public UITexture    m_textureCombo;
    public UIGrid       m_gridCombo;
    public UISprite[]   m_spriteComboValue;
    public Slot_NumberPicker      m_scoreNumberPicker;
    public UIProgressBar    m_barMusicProgress;
    public UISprite         m_spriteProgressLine;
    public UISprite     m_spriteLowerArc;
    public GameObject   m_startAnimation;

    [Header("Game Play UI")]
    public UIPanel m_panelBase;
    private List<GameLine> m_gameLineList;
    public List<GameLine> GameLineList { get { return m_gameLineList; } }

    public GameObject NotePool;
    public GameObject ActiveNotePool;
    public Dictionary<NoteType, GameObject> NoteBaseObjectMap { get; set; }
    public Dictionary<NoteType, Queue<GameObject>> NotePoolMap;
    public Dictionary<EParticleType, GameObject> ParticleBaseObjectMap { get; set; }

    public Dictionary<EParticleType, Queue<GameObject>> ParticlePoolMap;
    public Dictionary<HitJudgeType, GameObject> HitJudgeBaseObjectMap { get; set; } // Perfect, Good, Weak, Miss效果文字Base物件

    public UITweener m_lowerArcTweener;
    public UITweener m_upperArcTweener;

    public Dictionary<NoteLineType, Material> NoteLineMaterialMap { get; set; }

    public GameObject m_lowerArcPosition;
    public GameObject m_upperArcPosition;

    public GameObject ComboBulletShot { get; set; } // 從點擊往Combo數字移動特效，
    public GameObject ComboBulletEffect { get; set; } // 移動到Combo數字後顯示的特效
    public Dictionary<ComboEffectLevel, GameObject> ComboEffectMap { get; set; }

    public GameObject ComboFinishShot { get; set; } // Combo中斷後移動效果

    public GameObject CurrentComboEffect;
    public GameObject ComboEndShotPosition; // Combo結束的子彈射擊結束點(空的GameObject，定位用)

    public const int NOTE_BASE_AMOUNT = 5;

    public GameDataDB GameDB { set; get; }
    public PlayerDataSystem playerDataSystem { set; get; }    

    public DoubleFadeOut doubleFadeOut;
    public float m_DragLineScaleOffset = 15f;
    public float DragLineScaleOffset { get { return m_DragLineScaleOffset; } }

    public UISprite m_spResumeNumer = null;

    public GameObject m_objFullCombo = null;
    public Animation m_animFullCombo = null;

    [Header("Arc Setting")]
    public float m_Arcduration = 1.0f;
    public Ease m_ArcEaseType = Ease.Linear;
    public UISprite[] m_ArcArray = new UISprite[6]; // 照emArcLine順序設定

    public float m_ArcMoveduration = 1.0f;
    public Ease m_ArcMoveEaseType = Ease.Linear;
    public UISprite[] m_ArcMoveArray = new UISprite[6]; // 照emArcLine順序設定

    public GameObject m_objUpArcAnim = null;
    public GameObject m_objLowArcAnim = null;

    Dictionary<emArcLine, Tweener> dictArcTweener = new Dictionary<emArcLine, Tweener>();
    Dictionary<emArcLine, Tweener> dictArcMoveTweener = new Dictionary<emArcLine, Tweener>();
    bool[] bArcTweenerSwitch = new bool[6];
    bool bOriLower = false;
    bool bOriUpper = false;

    //RunTimeData
    private Vector3 m_vecProgressLinePos;

    //ComboTween
    private Vector3 m_OriginTweenToV3    = new Vector3();
    private Vector3 m_SmallTweenToV3     = new Vector3();        
    

    public UI_GamePlay() : base() { }
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
        m_vecProgressLinePos = m_spriteProgressLine.transform.localPosition;
    }
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
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
        Softstar.Utility.ChangeTexture(m_textureCombo, textureTmp.strTextureName);

        if(m_spriteComboValue.Length > 0)
        {
            m_OriginTweenToV3 = m_spriteComboValue[0].GetComponent<TweenScale>().to;
            m_SmallTweenToV3 = m_OriginTweenToV3 * 0.5f;
        }

        ComboFinishShot.transform.position = m_comboContainer.transform.position;

        m_scoreNumberPicker.Initialize();

        m_spResumeNumer.depth = GameDefine.DEFAULT_NOTE_DEPTH + 1;
        m_objUpArcAnim.GetComponent<TweenPosition>().AddOnFinished(objUpArcAnimEnd);
        m_objLowArcAnim.GetComponent<TweenPosition>().AddOnFinished(objLowArcAnimEnd);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>重新開始遊戲需要重置的UI</summary>
    public void ResetUIForRestart()
    {
        m_scoreNumberPicker.ResetUI();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeNoteObject()
    {
        foreach(KeyValuePair<NoteType, GameObject> kv in NoteBaseObjectMap)
        {
            for(int i=0;i<UI_GamePlay.NOTE_BASE_AMOUNT;i++)
            {
                if(NotePoolMap.ContainsKey(kv.Key) == false)
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
        foreach(KeyValuePair<EParticleType, GameObject> kv in ParticleBaseObjectMap)
        {
            if(ParticlePoolMap.ContainsKey(kv.Key) == false)
            {
                ParticlePoolMap[kv.Key] = new Queue<GameObject>();
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeHitPoint()
    {
        for (int i = 0; i < GameLineList.Count; i++)
        {
            GameLineList[i].hitPoint.SetActive(false);
        }
    }
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
        if(NotePoolMap[noteType].Count <= 0)
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
        if(ParticlePoolMap[type].Count <= 0)
        {
            ParticleCreateEnqueue(type);
        }
        GameObject go = ParticlePoolMap[type].Dequeue();
        go.transform.SetParent(ActiveNotePool.transform);
        go.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;
        go.transform.localPosition = new Vector3(99999, 99999, 0); // 初始在螢幕外面
        go.SetActive(true);
        /*
        //Settnig particle render queue
        ParticleInstance pi = go.GetComponent<ParticleInstance>();
        if (pi != null)
            pi.SetRenderQueue(m_spriteLowerArc);
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
    public void SetComboEffect(ComboEffectLevel level)
    {
        if (CurrentComboEffect != null)
        {
            CurrentComboEffect.SetActive(false);
            Destroy(CurrentComboEffect);
            CurrentComboEffect = null;
        }

        if (ComboEffectMap.ContainsKey(level))
        {
            GameObject go = GameObject.Instantiate<GameObject>(ComboEffectMap[level]);
            go.transform.SetParent(m_comboContainer.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;

            CurrentComboEffect = go;
            CurrentComboEffect.SetActive(true);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// 以世界座標為起點
    /// </summary>
    /// <param name="startPos"></param>
    public void ComboShot(Vector3 startPos)
    {
        StartCoroutine(ComboShotCoroutine(startPos));
    }
    private IEnumerator ComboShotCoroutine(Vector3 startPos)
    {
        GameObject comboBullet = Instantiate<GameObject>(ComboBulletShot);
        comboBullet.transform.SetParent(m_comboContainer.transform);
        comboBullet.transform.position = startPos;
        comboBullet.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;
        comboBullet.SetActive(true);
        Tween tween = comboBullet.transform.DOMove(m_comboContainer.transform.position, 0.3f);
        yield return tween.WaitForCompletion();

        GameObject comboBulletEffect = Instantiate<GameObject>(ComboBulletEffect);
        comboBulletEffect.transform.SetParent(m_comboContainer.transform);
        comboBulletEffect.transform.localPosition = Vector3.zero;
        comboBulletEffect.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;
        comboBulletEffect.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        comboBullet.SetActive(false);
        Destroy(comboBullet);

        yield return new WaitForSeconds(2.0f);
        comboBulletEffect.SetActive(false);
        Destroy(comboBulletEffect);
        comboBulletEffect = null;

        yield return null;
    }
    //-------------------------------------------------------------------------------------------------
    public void ComboEndShot()
    {
        StartCoroutine(ComboEndShotCoroutine());
    }
    private IEnumerator ComboEndShotCoroutine()
    {
        GameObject comboBullet = Instantiate<GameObject>(ComboFinishShot);
        comboBullet.transform.SetParent(m_comboContainer.transform);
        comboBullet.transform.position = m_comboContainer.transform.position;
        comboBullet.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;
        comboBullet.SetActive(true);
        Tween tween = comboBullet.transform.DOMove(ComboEndShotPosition.transform.position, 0.3f);
        yield return tween.WaitForCompletion();

        GameObject comboBulletEffect = Instantiate<GameObject>(ComboBulletEffect);
        comboBulletEffect.transform.SetParent(ComboEndShotPosition.transform);
        comboBulletEffect.transform.localPosition = Vector3.zero;
        comboBulletEffect.transform.localScale = Vector3.one * GameDefine.PARTICLE_MULTIPLY;
        comboBulletEffect.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        comboBullet.SetActive(false);
        Destroy(comboBullet);

        yield return new WaitForSeconds(2.0f);
        comboBulletEffect.SetActive(false);
        Destroy(comboBulletEffect);
        comboBulletEffect = null;

        yield return null;
    }
    //-------------------------------------------------------------------------------------------------
    #region Setting UI
    public void SetBtnPauseSprite(string atlasName, string spriteName)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonPause, spriteName, spriteName, spriteName, spriteName, atlasName);
    }
    public void SetScoreValue(int score)
    {
        m_scoreNumberPicker.SetNumber(score);
    }
    public void SetComboValue(int combo)
    {
        m_textureCombo.gameObject.SetActive(combo > 0);

        List<int> digitNumberList = Softstar.Utility.SeparateNumberToDigit(combo, false);
        if (digitNumberList.Count <= m_spriteComboValue.Length)
        {   
            int digit = digitNumberList.Count;
            int digitIndex = 0;
            for (int i = 0, iCount = m_spriteComboValue.Length; i < iCount; ++i)
            {
                //關閉用不到的位數圖
                if (i < iCount - digit)
                {
                    m_spriteComboValue[i].gameObject.SetActive(false);
                }
                else
                {
                    S_TextureTable_Tmp textureTmp = GameDB.GetGameDB<S_TextureTable_Tmp>().GetData(41+ digitNumberList[digitIndex]);
                    if (m_spriteComboValue[i].spriteName != textureTmp.strSpriteName)
                    {
                        Softstar.Utility.ChangeAtlasSprite(m_spriteComboValue[i], textureTmp.strSpriteName);
                    }
                    m_spriteComboValue[i].gameObject.SetActive(true);
                    digitIndex++;
                }   
            }
            if (combo >= GameDefine.COMBO_MOD)
            {
                TweenComboValue((combo % GameDefine.COMBO_MOD == 0)? m_OriginTweenToV3 : m_SmallTweenToV3);
            }
            m_gridCombo.Reposition();
        }
    }
    public void SetProgressBarValue(float now, float max)
    {
        m_barMusicProgress.value = (max - now) / max;
        SetCurrentProgressLine(now / max);   
    }
    private void SetCurrentProgressLine(float ratio)
    {
        int totalWidth = m_barMusicProgress.foregroundWidget.width;
        int transformDistance = Mathf.RoundToInt(ratio * totalWidth);
        Vector3 vect3 = m_vecProgressLinePos;
        vect3.x += transformDistance;
        m_spriteProgressLine.transform.localPosition = vect3;
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    private void TweenComboValue(Vector3 tweenTo)
    {
        TweenScale tc = null;
        for (int i = 0, iCount = m_spriteComboValue.Length; i < iCount; ++i)
        {
            if (!m_spriteComboValue[i].gameObject.activeInHierarchy)
                continue;
            tc = m_spriteComboValue[i].GetComponent<TweenScale>();
            tc.to = tweenTo;
            tc.ResetToBeginning();
            tc.PlayForward();            
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void PlayStartAnimation()
    {
        m_startAnimation.SetActive(false);
        m_startAnimation.SetActive(true);
    }

    //-------------------------------------------------------------------------------------------------
    public DoubleFadeOut CreateDoubleFadeOutObj(Vector3 vecA, Vector3 vecB,int depth)
    {
        GameObject obj = GameObject.Instantiate(doubleFadeOut.gameObject);        
        obj.transform.parent = ActiveNotePool.transform;
        obj.transform.localScale = Vector3.one;
        DoubleFadeOut fadeout = obj.GetComponent<DoubleFadeOut>();
        fadeout.Init();
        fadeout.StartUp(vecA, vecB, GameLineList[5].hitPoint.transform.position, depth);
        return fadeout;
    }

    #region New ArcRule
    //-------------------------------------------------------------------------------------------------
    //Arc淡入消失
    public void FadeInArc(emArcLine em)
    {
        if (!dictArcTweener.ContainsKey(em))
            return;

        dictArcTweener[em].PlayBackwards();
    }
    //-------------------------------------------------------------------------------------------------
    //Arc淡出出現
    public void FadeOutArc(emArcLine em)
    {
        //線斷出現前的殘影動畫
        PlayArcMove(em);

        if (!dictArcTweener.ContainsKey(em))
            CreateArcTweener(em);

        dictArcTweener[em].PlayForward();
    }
    //-------------------------------------------------------------------------------------------------
    //播放Arc殘影動畫
    public void PlayArcMove(emArcLine em)
    {
        //該線斷必須接近透明才播放
        if (m_ArcArray[(int)em].alpha >= 0.2f)
            return;

        if (!dictArcMoveTweener.ContainsKey(em))
            CreateArcMoveTweener(em);
                
        dictArcMoveTweener[em].Play();
    }
    //-------------------------------------------------------------------------------------------------    
    //產生ArcTween物件
    void CreateArcTweener(emArcLine em)
    {
        UISprite sp = m_ArcArray[(int)em];        
        if (sp != null)
        {
            Tweener t = DOTween.ToAlpha(() => sp.color, x => sp.color = x, 1, m_Arcduration).SetEase(m_ArcEaseType)
            .SetAutoKill(false);
            dictArcTweener.Add(em, t);
        }
    }
    //-------------------------------------------------------------------------------------------------
    //產生Arc殘影Tween物件
    void CreateArcMoveTweener(emArcLine em)
    {
        UISprite sp = m_ArcMoveArray[(int)em];
        if (sp != null)
        {
            Tweener t = DOTween.ToAlpha(() => sp.color, x => sp.color = x, 1, m_ArcMoveduration).SetEase(m_ArcMoveEaseType)
            .SetLoops(2, LoopType.Yoyo)
            .SetAutoKill(false)
            .OnComplete(() => ArcMoveComplete(em));

            dictArcMoveTweener.Add(em, t);            
        }        
    }
    //-------------------------------------------------------------------------------------------------
    //Arc殘影Tween物件完成事件
    void ArcMoveComplete(emArcLine em)
    {
        //重置
        dictArcMoveTweener[em].Rewind();
    }
    //-------------------------------------------------------------------------------------------------
    //淡出/入 流程
    public void FadeInArcProcess()
    {
        //規則 左右兩段存在中間必須存在
        if (bArcTweenerSwitch[(int)emArcLine.LL] && bArcTweenerSwitch[(int)emArcLine.LR])
            bArcTweenerSwitch[(int)emArcLine.LM] = true;

        if (bArcTweenerSwitch[(int)emArcLine.UL] && bArcTweenerSwitch[(int)emArcLine.UR])
            bArcTweenerSwitch[(int)emArcLine.UM] = true;

        for (int i = 0; i < bArcTweenerSwitch.Length; i++)
        {
            if (bArcTweenerSwitch[i])
                FadeOutArc((emArcLine)i);
            else
                FadeInArc((emArcLine)i);
        }                               
    }
    //-------------------------------------------------------------------------------------------------
    //重置淡出/入開關
    public void InitArcSwitch()
    {        
        for (int i = 0; i < bArcTweenerSwitch.Length; i++)
        {
            bArcTweenerSwitch[i] = false;
        }
    }
    //-------------------------------------------------------------------------------------------------
    //設定淡出/入開關
    public void SetArcSwitchToggle(emArcLine em)
    {
        bArcTweenerSwitch[(int)em] = true;
    }
    //-------------------------------------------------------------------------------------------------
    //檢查是否需要顯示 上下線段提示(包含Arc上下線段進場動畫播放檢查)
    public bool NeedShowNoteMark()
    {
        bool U = false;
        bool L = false;
        for (int i = (int)emArcLine.LL; i <= (int)emArcLine.LR; i++)
        {
            if (bArcTweenerSwitch[i])
                L = true;
        }

        for (int j = (int)emArcLine.UL; j <= (int)emArcLine.UR; j++)
        {
            if (bArcTweenerSwitch[j])
                U = true;
        }

        //上下線段進場動畫播放
        PlayArcAnimCheck(U, L);

        if ((L && !U) || (!L && !U))
            return false;
        else
            return true;   
    }
    //-------------------------------------------------------------------------------------------------
    public void PlayArcAnimCheck(bool U, bool L)
    {        
        float alpha = 0;
        if (!bOriLower && L && bOriUpper) //規則->原本線段透明 & 現在線段需顯示 & 另一個線段存在
        {
            //本線段需接近透明
            alpha = m_ArcArray[0].alpha + m_ArcArray[1].alpha + m_ArcArray[2].alpha;
            if (alpha < 0.2f)
                PlayArcLowerAnim();
        }

        if (!bOriUpper && U && bOriLower) //規則同上
        {
            alpha = m_ArcArray[3].alpha + m_ArcArray[4].alpha + m_ArcArray[5].alpha;
            if (alpha < 0.2f)
                PlayArcUpperAnim();
        }

        //記錄本次線段資訊
        bOriLower = L;
        bOriUpper = U;
    }
    //-------------------------------------------------------------------------------------------------
    //Arc進場動畫播放
    public void PlayArcLowerAnim()
    {
        m_objLowArcAnim.SetActive(true);
        TweenScale ts = m_objLowArcAnim.GetComponent<TweenScale>();
        TweenAlpha ta = m_objLowArcAnim.GetComponent<TweenAlpha>();
        TweenPosition tp = m_objLowArcAnim.GetComponent<TweenPosition>();

        TweenPlay(ts);
        TweenPlay(ta);
        TweenPlay(tp);
    }
    //-------------------------------------------------------------------------------------------------
    //Arc進場動畫播放
    public void PlayArcUpperAnim()
    {
        m_objUpArcAnim.SetActive(true);
        TweenScale ts = m_objUpArcAnim.GetComponent<TweenScale>();
        TweenAlpha ta = m_objUpArcAnim.GetComponent<TweenAlpha>();
        TweenPosition tp = m_objUpArcAnim.GetComponent<TweenPosition>();

        TweenPlay(ts);
        TweenPlay(ta);
        TweenPlay(tp);
    }
    //-------------------------------------------------------------------------------------------------
    void TweenPlay(UITweener t, bool bReverse = false)
    {
        t.enabled = true;
           
        if(!bReverse)
            t.PlayForward();
        else
            t.PlayReverse();

        t.ResetToBeginning();
    }
    //-------------------------------------------------------------------------------------------------
    //Arc動畫進場結束事件(註冊於tweenPosition)
    public void objLowArcAnimEnd()
    {
        m_objLowArcAnim.SetActive(false);
    }
    //-------------------------------------------------------------------------------------------------
    //Arc動畫進場結束事件(註冊於tweenPosition)
    public void objUpArcAnimEnd()
    {
        m_objUpArcAnim.SetActive(false);
    }

    public void SetFullComboAnim(GameObject obj)
    {
        m_objFullCombo = GameObject.Instantiate<GameObject>(obj);
        m_objFullCombo.transform.parent = this.transform;
        m_objFullCombo.transform.position = Vector3.zero;
        m_objFullCombo.transform.localScale = Vector3.one;
        m_animFullCombo = m_objFullCombo.GetComponentInChildren<Animation>();
        m_objFullCombo.SetActive(false);
    }

    #endregion
}
