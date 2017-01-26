using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class Slot_NumberPicker : MonoBehaviour
{
    public enum Enum_NumberPivot
    {
       Left,
       Right,
    }

    [Header("Value Setting")]
    public long m_lShowNumber;                                      //欲顯示的數字
    public Slot_ScrollNumber.Enum_ScrollOrientation Orientation;    //滾動方向
    public Enum_NumberPivot PositionPivot;                          //位置置左或置右
    public int MaxDigit;                                            //最大位數

    public float Duration;                                          //滾動時間
    public float Interval;                                          //每組數字滾動間隔
    public int HorizontalPadding;                                   //數字左右的間隔
    public int VerticalPadding;                                     //數字上下間隔
    public Color NoneColor;                                        //數字沒用到時顯示的顏色

    //Call back
    public EventDelegate.Callback OnPlayFinish;

    [Header("UI")]
    public UIPanel m_panelBase;
    public UIGrid m_gridAllNumber;
    public TweenScale m_tweenScale;
    public Slot_ScrollNumber m_slotScrollNumber;                    //複製用

    //RunTime Data
    private bool                    m_bIsPlaying;
    public bool IsPlaying { get { return m_bIsPlaying; } }
    private List<Slot_ScrollNumber> m_numberList;
    private List<int>               m_digitList;
    private Coroutine               m_playScrollCoroutine;
    private Coroutine               m_playScaleCoroutine;
    private Vector3                 m_originSlotPos;
    private long                    m_lLastNumber;
    private int                     m_iTweenScalePlayCount;
    //-------------------------------------------------------------------------------------------------
    private Slot_NumberPicker(){}
    //-------------------------------------------------------------------------------------------------
    public void Initialize()
    {
        m_numberList = new List<Slot_ScrollNumber>();
        m_digitList = new List<int>();
        m_slotScrollNumber.Hide();
        m_originSlotPos = this.transform.localPosition;
        m_lLastNumber = 0;

        CreateScrollNumber();
        AdjustPanelRange();
        OnPlayFinish += PlayFinish;
        EventDelegate.Add(m_tweenScale.onFinished, RecordTweenScalePlayCount);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>生成每組Scroll數字</summary>
    private void CreateScrollNumber()
    {
        for (int i = 0; i < MaxDigit; ++i)
        {
            GameObject go = NGUITools.AddChild(this.gameObject, m_slotScrollNumber.gameObject);
            Slot_ScrollNumber slot = go.GetComponent<Slot_ScrollNumber>();
            slot.name += "_" + i.ToString();
            slot.Initialize(VerticalPadding, Duration, Orientation, NoneColor);
            m_numberList.Add(slot);
            slot.Show();
        }
        m_numberList.Reverse();

        //排序
        m_gridAllNumber.pivot = (PositionPivot == Enum_NumberPivot.Left) ? UIWidget.Pivot.Left : UIWidget.Pivot.Right;
        m_gridAllNumber.cellWidth = m_slotScrollNumber.m_spriteBG.width + HorizontalPadding;
        m_gridAllNumber.enabled = true;
        m_gridAllNumber.Reposition();
        //After grid reposition, Slot localPosition would changed. So reset localPosition to original.
        this.transform.localPosition = m_originSlotPos;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>調整Panel範圍大小</summary>
    private void AdjustPanelRange()
    {
        float eachNumberWidth = m_slotScrollNumber.m_spriteBG.width + HorizontalPadding;
        //設定範圍大小
        float panelWidth = eachNumberWidth * MaxDigit;
        m_panelBase.SetRect(0, 0, panelWidth, m_slotScrollNumber.m_spriteBG.height);
        //設定中心點
        float centerX = (MaxDigit - 1) * eachNumberWidth / 2;
        centerX *= (PositionPivot == Enum_NumberPivot.Left) ?1:-1;
        Vector4 vec4 = m_panelBase.baseClipRegion;
        vec4.x = centerX;
        m_panelBase.baseClipRegion = vec4;
    }
    //-------------------------------------------------------------------------------------------------
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    //-------------------------------------------------------------------------------------------------
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetNumber(long number)
    {    
        if (number < 0)
            return;

        //若設定新數字則停止上一次的動畫
        if (m_bIsPlaying)
        {
            Stop(true);
            //UnityDebugger.Debugger.Log(this.name + " Scroll Stop By SetNumber!");
        }

        m_digitList.Clear();

        //0也算一位數
        if (number == 0 && m_lLastNumber != 0)
            m_digitList.Add((int)number); 
        else
            m_digitList = Softstar.Utility.SeparateNumberToDigit(number, true);

        //檢察欲顯示位數是否超過上限
        if (m_digitList.Count > MaxDigit)
        {
            UnityDebugger.Debugger.Log(this.name + " the digit of number=" + m_digitList.Count + " is bigger then MaxDigit=" + MaxDigit);
            return;
        }

        m_lShowNumber = number;

        AddNumberPlayEvent();

        //UnityDebugger.Debugger.Log(this.name+" Set number =" + m_lShowNumber + ", last number =" + m_lLastNumber);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>設定滾動完畢的監聽事件(顏色變化&結束旗標)</summary>
    private void AddNumberPlayEvent()
    {  
        for (int i = 0; i < m_numberList.Count; ++i)
        {
            if (i >= m_digitList.Count)
                m_numberList[i].OnPlayFinish += m_numberList[i].SetAllColorToNone;
        }
        if (m_lShowNumber == 0)
        {
            OnPlayFinish += m_numberList[0].SetAllColorToNone;
        }
    }
    //-------------------------------------------------------------------------------------------------
    private void RemoveNumberPlayEvent()
    {
        for (int i = 0; i < m_numberList.Count; ++i)
        {
            m_numberList[i].OnPlayFinish -= m_numberList[i].SetAllColorToNone;
        }
        OnPlayFinish -= m_numberList[0].SetAllColorToNone;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// 播放數字滾動動畫(若顯示數字跟上次一樣則不會播放)
    /// </summary>
    public void Play(bool isScale)
    {
        if (m_lShowNumber < 0)
            return;

        if (m_bIsPlaying)
        {
            Stop(false);
        }

        m_bIsPlaying = true;

        if (isScale)
            m_playScaleCoroutine = StartCoroutine(PlayScaleAndScroll());
        else
            m_playScrollCoroutine = StartCoroutine(PlayScroll());
    }
    //-------------------------------------------------------------------------------------------------
    public IEnumerator PlayScaleAndScroll()
    {
        EventDelegate.Add(m_tweenScale.onFinished, m_tweenScale.PlayReverse, true);
        m_tweenScale.PlayForward();
        //等待數字放大演出結束
        while (m_iTweenScalePlayCount < 2)
            yield return null;

        m_playScrollCoroutine = StartCoroutine(PlayScroll());
    }
    //-------------------------------------------------------------------------------------------------
    private IEnumerator PlayScroll()
    {
        int playDigit = CalPlayDigitcount();
        if (playDigit <= 0)
        {
            Stop(false);
            yield break;
        } 

        //滾動前重置所有數字顏色
        for (int m = 0; m < playDigit; ++m)
        {
            m_numberList[m].SetAllColorToWhite();
        }

        for (int i = 0; i < playDigit; ++i)
        {
            //設定滾動終點的數字
            int tweenNumber = (i >= m_digitList.Count) ? 0 : m_digitList[i];
            //開始滾動
            m_numberList[i].TweenToNumber(tweenNumber);
            yield return new WaitForSeconds(Interval);
        }

        //等待所有數字滾動結束
        for (int i = 0; i < m_numberList.Count; ++i)
        {
            while (m_numberList[i].m_iTweenCount > 0)
                yield return null;
        }
        if (OnPlayFinish != null)
            OnPlayFinish();
    }
    //-------------------------------------------------------------------------------------------------
    private void Stop(bool stopByNumber)
    {
        if (m_playScrollCoroutine != null)
            StopCoroutine(m_playScrollCoroutine);
        if (m_playScaleCoroutine != null)
            StopCoroutine(m_playScaleCoroutine);
        PlayFinish();
        /*
        string stopSource = (stopByNumber)?"SetNumber":"Play";
        UnityDebugger.Debugger.Log("-------------------"+this.name + " Scroll Stop! Stop By"+ stopSource);*/
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>計算要跑幾位數的動畫 </summary>
    public int CalPlayDigitcount()
    {
        int lastDigit = Softstar.Utility.SeparateNumberToDigit(m_lLastNumber, true).Count;
        int playDigit = (lastDigit > m_digitList.Count)?lastDigit: m_digitList.Count;

        return playDigit;
    }
    //-------------------------------------------------------------------------------------------------
    private void PlayFinish()
    {
        RemoveNumberPlayEvent();

        this.transform.localScale = Vector3.one;
        m_iTweenScalePlayCount = 0;
        m_playScrollCoroutine = null;
        m_lLastNumber = m_lShowNumber;
        m_bIsPlaying = false;

        //UnityDebugger.Debugger.Log(this.name + " Scroll Finish!");
    }
    //-------------------------------------------------------------------------------------------------
    private void RecordTweenScalePlayCount()
    {
        m_iTweenScalePlayCount++;
    }
    //-------------------------------------------------------------------------------------------------
    public void ResetUI()
    {
        for (int i = 0; i < m_numberList.Count; ++i)
        {
            GameObject.Destroy(m_numberList[i].gameObject);
        }
        m_numberList.Clear();
        m_digitList.Clear();
        m_slotScrollNumber.Hide();

        m_originSlotPos = this.transform.localPosition;
        this.transform.localScale = Vector3.one;
        m_iTweenScalePlayCount = 0;
        m_playScrollCoroutine = null;
        m_lLastNumber = 0;
        m_lShowNumber = 0;
        m_bIsPlaying = false;

        CreateScrollNumber();
        AdjustPanelRange();
    }
    //-------------------------------------------------------------------------------------------------
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (m_numberList != null)
            {
                foreach (var number in m_numberList)
                {
                    Destroy(number.gameObject);
                }
                m_numberList.Clear();
            }
            OnPlayFinish -= PlayFinish;
            Initialize();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            SetNumber(m_lShowNumber);
            Play(false);
        }
    }
}
