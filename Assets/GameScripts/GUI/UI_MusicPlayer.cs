using UnityEngine;
using System.Collections;

public class UI_MusicPlayer : MonoBehaviour
{
    [Header("Progress Bar")]
    public UIProgressBar m_barMusicProgress;
    public UILabel m_labelPlayTime;
    public UIButton m_buttonProgressLine;
    public UISprite m_spriteProgressLine;

    [Header("Music Control")]
    public UIButton m_buttonOneLoop;
    public UIButton m_buttonAllLoop;
    public UIButton m_buttonPreSong;
    public UIButton m_buttonPlay;
    public UIButton m_buttonPause;
    public UIButton m_buttonNextSong;
    public UIButton m_buttonRandom;

    //RunTimeData
    private Vector3 m_vecProgressLinePos;

    public UI_MusicPlayer()
    { }
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Initialize()
    {
        m_vecProgressLinePos = m_spriteProgressLine.transform.localPosition;
        SetPlayTime(0.0f);
    }
    /// <summary>透過音樂播放時間更新進度條</summary>
    public void SetProgressByMusic(float now, float max)
    {
        float ratio = now / max;
        if (Mathf.Approximately(max, 0.0f))
            ratio = 0.0f;

        //更新bar值
        float deltaRatio = ratio - m_barMusicProgress.value;
        m_barMusicProgress.value = ratio;
        //更新按鈕位置
        int totalWidth = m_barMusicProgress.foregroundWidget.width;
        float deltaDistance = (deltaRatio * totalWidth);
        Vector3 vect3 = m_spriteProgressLine.transform.localPosition;
        vect3.x += deltaDistance;
        vect3.x = Mathf.Clamp(vect3.x, m_vecProgressLinePos.x, (float)m_barMusicProgress.foregroundWidget.width);
        m_spriteProgressLine.transform.localPosition = vect3;
        //更新時間文字
        SetPlayTime(now);
    }
    /// <summary>透過進度條按鈕更新進度條</summary>
    public void SetProgressByButton(float deltaDistance, float bgmLength)
    {
        deltaDistance *= 1.3f;
        //更新按鈕位置
        float totalWidth = (float)m_barMusicProgress.foregroundWidget.width;
        Vector3 vect3 = m_spriteProgressLine.transform.localPosition;
        vect3.x += deltaDistance;
        vect3.x = Mathf.Clamp(vect3.x, m_vecProgressLinePos.x, totalWidth);
        m_spriteProgressLine.transform.localPosition = vect3;
        //更新bar值
        float ratio = GetProgressLineRatio();
        m_barMusicProgress.value = ratio;
        //更新時間文字
        SetPlayTime(ratio * bgmLength);
        //UnityDebugger.Debugger.Log("---------Music Play Ratio = "+ deltaRatio);
    }
    public float GetProgressLineRatio()
    {
        float lineDistance = m_spriteProgressLine.transform.localPosition.x - m_vecProgressLinePos.x;
        float ratio = lineDistance / m_barMusicProgress.foregroundWidget.width;
        ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);

        return ratio;
    }
    public void SetPlayTime(float time)
    {
        m_labelPlayTime.text = Softstar.Utility.GetShowTime(Enum_TimeFormat.Minute, time);
    }
    //-------------------------------------------------------------------------------------------------
    public void ChangeButtonOneLoop(int guid)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonOneLoop, guid);
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeButtonAllLoop(int guid)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonAllLoop, guid);
    }
    //-------------------------------------------------------------------------------------------------
    public void ChangeButtonRandom(int guid)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonRandom, guid);
    }
}
