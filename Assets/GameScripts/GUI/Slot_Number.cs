using UnityEngine;
using System.Collections;

public class Slot_Number : MonoBehaviour
{
    private UISprite m_spriteNumber;
    private TweenPosition m_tweenPos;
    private int m_iNumber;
    public int m_iIndex;
    //-------------------------------------------------------------------------------------------------
    public Slot_Number() {}
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Initialize(int index)
    {
        m_spriteNumber = this.GetComponent<UISprite>();
        m_tweenPos = this.GetComponent<TweenPosition>();
        m_iIndex = index;
        m_iNumber = 0;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetNumber(int num)
    {
        m_iNumber = num;
        Softstar.Utility.ChangeScoreNumberSprite(m_spriteNumber, num);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetNumber()
    {
        return m_iNumber;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetTweenPos(float toY)
    {
        m_tweenPos.from = this.transform.localPosition;
        m_tweenPos.to = new Vector3(m_tweenPos.from.x, m_tweenPos.from.y + toY, m_tweenPos.from.z);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetTweenDuration(float time)
    {
        m_tweenPos.duration = time;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetColor(Color color)
    {
        m_spriteNumber.color = color;
    }
    //-------------------------------------------------------------------------------------------------
    public bool CheckNumberIsTheSame(int number)
    {
        return m_iNumber == number;
    }
}
