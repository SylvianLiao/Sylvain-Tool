using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class Slot_ScrollNumber : MonoBehaviour
{
    public enum Enum_ScrollOrientation
    {
        Up,
        Down,
    }

    [Header("UI")]
    public UIPlayTween m_playTween;
    public UISprite m_spriteBG;
    public List<Slot_Number> m_slotNumberList;

    //Scroll Value
    private int m_iNumberSize;
    private float m_fDuration;
    private bool m_bIsUP;
    private Color m_noneColor;
    //Call back
    public EventDelegate.Callback OnPlayFinish;
    //RunTime Data
    public int m_iTweenCount;
    //-------------------------------------------------------------------------------------------------
    private Slot_ScrollNumber(){}
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Initialize(int padding, float duration, Enum_ScrollOrientation orientation, Color noUseColor)
    {
        m_iNumberSize = m_spriteBG.height + padding;
        m_fDuration = duration;
        m_bIsUP = orientation == Enum_ScrollOrientation.Up;
        m_noneColor = noUseColor;

        SettingPlayTween();
        SettingSlotNumber();
    }
    //-------------------------------------------------------------------------------------------------
    public void SettingPlayTween()
    {
        //delegate
        if (m_playTween.onFinished.Count == 0)
            EventDelegate.Add(m_playTween.onFinished, UpdateScrollNumber);

        //reset state when play
        m_playTween.resetOnPlay = true;
    }
    //-------------------------------------------------------------------------------------------------
    private void SettingSlotNumber()
    {
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        {
            //重置位置
            Vector3 vec3 = m_slotNumberList[i].transform.localPosition;
            vec3.y = m_iNumberSize - (i * m_iNumberSize);
            m_slotNumberList[i].transform.localPosition = vec3;

            m_slotNumberList[i].Initialize(i);
            float toY = (m_bIsUP) ? m_iNumberSize : m_iNumberSize * -1;
            m_slotNumberList[i].SetTweenPos(toY);
        }
        SetAllColorToNone();
        //設定下一個要顯示的數字
        m_slotNumberList[(m_bIsUP) ? 2 : 0].SetNumber(1);
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
    public void TweenToNumber(int number)
    {
        if (number > 9 || number < 0 || GetSlotNumberByIndex(1).CheckNumberIsTheSame(number))
            return;

        int currentNumber = GetSlotNumberByIndex(1).GetNumber();
        m_iTweenCount = (number > currentNumber) ? number-currentNumber: number - currentNumber +10;

        float eachDuration = m_fDuration / m_iTweenCount;
        //duration
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        {
            m_slotNumberList[i].SetTweenDuration(eachDuration);
        }
        m_playTween.Play(true);     
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>每次Tween一個數字時的更新事件</summary>
    private void UpdateScrollNumber()
    {
        int offset = (m_bIsUP)?-1:1;
        //先處理index，以便更新數字
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        {
            float y = m_slotNumberList[i].transform.localPosition.y;
            int index = m_slotNumberList[i].m_iIndex;
            index += offset;
            if (index > 2) index = 0;
            else if (index < 0) index = 2;

            m_slotNumberList[i].m_iIndex = index;
        }
        //更新數字&位置
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        {
            Transform numTrans = m_slotNumberList[i].transform;
            int index = m_slotNumberList[i].m_iIndex;
            //處理超出滾動範圍的數字
            if ((m_bIsUP && index == 2) || (!m_bIsUP && index == 0))
            {
                //更新數字
                int preIndex = m_slotNumberList[i].m_iIndex + offset;
                preIndex = Mathf.Clamp(preIndex, 0, 2);
                Slot_Number preSN = GetSlotNumberByIndex(preIndex);
                int nowNumber = preSN.GetNumber() + 1;
                if (nowNumber > 9) nowNumber = 0;
                m_slotNumberList[i].SetNumber(nowNumber);
            }
            //更新位置
            Vector3 vec3 = numTrans.localPosition;
            switch (index)
            {
                case 0:
                    vec3.y = m_iNumberSize;
                    break;
                case 1:
                    vec3.y = 0.0f;
                    break;
                case 2:
                    vec3.y = m_iNumberSize * -1;
                    break;
            } 
            numTrans.localPosition = vec3;

            //設定下一次Tween的目標
            float moveY = (m_bIsUP) ? m_iNumberSize : m_iNumberSize * -1;
            m_slotNumberList[i].SetTweenPos(moveY);

            if (Mathf.Abs(numTrans.localPosition.y) > m_iNumberSize*2)
                UnityDebugger.Debugger.LogError("Number index is error! [" + this.name + ", " + m_slotNumberList[i].name + ", Y= " + numTrans.localPosition.y.ToString() + ", index = " + m_slotNumberList[i].m_iIndex + "]");
        }
        m_iTweenCount--;

        if (m_iTweenCount <= 0)
        {
            if (OnPlayFinish != null)
                OnPlayFinish();
            return;
        }
            
        m_playTween.Play(true);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetAllColorToWhite()
    {
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        { 
            m_slotNumberList[i].SetColor(Color.white);
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void SetAllColorToNone()
    {
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        {
            m_slotNumberList[i].SetColor(m_noneColor);
        }
    }
    //-------------------------------------------------------------------------------------------------
    private Slot_Number GetSlotNumberByIndex(int index)
    {
        for (int i = 0, iCount = m_slotNumberList.Count; i < iCount; ++i)
        {
            if (m_slotNumberList[i].m_iIndex == index)
                return m_slotNumberList[i];
        }
        return null;
    }
}
