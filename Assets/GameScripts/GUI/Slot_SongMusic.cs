using UnityEngine;
using System.Collections;
using Softstar;

public class Slot_SongMusic : MonoBehaviour
{
    public int m_songGroupID = -1;

    public UIButton m_buttonSong;
    public UIButton m_buttonStar;
    public UILabel m_labelSongName;
    public UILabel m_labelTag;

    //RunTimeData
    public SongData m_songData;
    //-------------------------------------------------------------------------------------------------
    private Slot_SongMusic(){}
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI()
    {
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
    public void SetData(SongData data, PlayerDataSystem dataSystem, string starSpriteName, string bgSpriteName)
    {
        m_songData = data;
        m_labelSongName.text = dataSystem.GetSongName(data)+"_"+ data.SongGroupID.ToString();
        m_labelTag.text = dataSystem.GetSongComposerName(data);
        SetStar(starSpriteName);
        SetBackground(bgSpriteName);

        m_songGroupID = data.SongGroupID;
    }
    //-------------------------------------------------------------------------------------------------
    public void ClearData()
    {
        m_songData = null;
        m_labelSongName.text = "";
        m_labelTag.text = "";
        m_songGroupID = -1;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetStar(string starSpriteName)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonStar, starSpriteName, starSpriteName, starSpriteName, starSpriteName);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetBackground(string bgSpriteName)
    {
        m_buttonSong.normalSprite = bgSpriteName;
    }
}
