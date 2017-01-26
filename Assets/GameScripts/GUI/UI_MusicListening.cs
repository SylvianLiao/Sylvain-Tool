using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_MusicListening : NGUIChildGUI
{
    [Header("Left UI")]
    public UITexture m_textureSong;
    public UITexture m_textureSongName;
    public UILabel m_labelComposer;
    public UILabel m_labelComposerName;
    public GameObject m_loadingUI;

    [Header("Music Player")]
    public UI_MusicPlayer m_uiMusicPlayer;

    [Header("Right UI")]
    public UIScrollView m_scrollSongMenu;
    public UIWrapContentEX m_wcSongMenu;
    public UISprite m_spriteHighLight;

    //Const Value
    public const int m_iEachPageSongCount = 9;

    //RunTime Data
    public List<Slot_SongMusic> m_slotSongObjList;
    private float m_fWCPanelPosY;
    private float m_fWCPanelOffsetY;

    private UI_MusicListening() : base(){ }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        m_slotSongObjList = new List<Slot_SongMusic>();
        m_spriteHighLight.gameObject.SetActive(false);
        m_fWCPanelPosY = m_wcSongMenu.GetComponent<UIPanel>().transform.localPosition.y;
        m_fWCPanelOffsetY = m_wcSongMenu.GetComponent<UIPanel>().clipOffset.y;
    }
    //-------------------------------------------------------------------------------------------------
    public void InitWrapContentValue(int songDataCount)
    {
        m_wcSongMenu.minIndex = (songDataCount - 1) * (-1);
        m_wcSongMenu.maxIndex = 0;

        m_scrollSongMenu.enabled = songDataCount > m_iEachPageSongCount;
    }
    //-------------------------------------------------------------------------------------------------
    public void CreateSlotSongMusic(GameObject obj)
    {
        if (obj == null)
            return;

        for (int i = 0, iCount = (m_iEachPageSongCount+2); i < iCount; ++i)
        {
            GameObject go = NGUITools.AddChild(m_allPanelList[1].gameObject, obj);
            go.name = typeof(Slot_SongMusic).Name + "_" + i.ToString();
            Slot_SongMusic slot = go.GetComponent<Slot_SongMusic>();
            UIDragScrollView dsv = go.GetComponent<UIDragScrollView>();
            if (dsv != null)
                dsv.scrollView = m_scrollSongMenu;

            m_slotSongObjList.Add(slot);
        }
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
    #region UI Setting
    //-------------------------------------------------------------------------------------------------
    public void SetSongComposer(string composerName)
    {
        m_labelComposerName.text = composerName;
    }
    public void SetSongName(string songNameTexture)
    {
        Softstar.Utility.ChangeTexture(m_textureSongName, songNameTexture);
    }
    public void SetSongTexture(Texture texture)
    {
        m_textureSong.mainTexture = texture;
    }
    
    #endregion

    //-------------------------------------------------------------------------------------------------
    public void SwitchLoadingUI(bool bSwitch)
    {
        m_loadingUI.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SortWrapContent()
    {
        m_wcSongMenu.ResetChildPositionsEX();
    }

    //-------------------------------------------------------------------------------------------------
    public Slot_SongMusic GetSlotSongMusicByGroupID(int id)
    {
        for (int i = 0, iCount = m_slotSongObjList.Count; i < iCount; ++i)
        {
            Slot_SongMusic slotSong = m_slotSongObjList[i];
            if (slotSong.m_songGroupID == id)
            {
                return slotSong;
            }
        }
        return null;
    }
    //-------------------------------------------------------------------------------------------------
    public bool IsOwnHighLightSongObj(GameObject slotObj)
    {
        return (m_spriteHighLight.transform.parent.gameObject).Equals(slotObj);
    }
    //-------------------------------------------------------------------------------------------------
    public void MoveWrapContentTo(int realIndex)
    {
        m_wcSongMenu.MoveTo(realIndex, m_fWCPanelPosY, m_fWCPanelOffsetY);
    }

}
