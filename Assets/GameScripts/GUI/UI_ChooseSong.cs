using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using HedgehogTeam.EasyTouch;

public class UI_ChooseSong : NGUIChildGUI
{
    [Header("Develop UI")]
    public UIButton m_tempButtonSong;
    public UILabel m_labelSongGroupID;

    [Header("Song UI")]
    public UITexture m_textureNewSong;
    public UITexture m_textureOriginalSong;
    public UITexture m_textureSongName;
    public UILabel m_labelComposer;
    public UILabel m_labelComposerName;

    [Header("Rank")]
    public UISprite m_spriteRankEasy;
    public UISprite m_spriteRankNormal;
    public UISprite m_spriteRankHard;

    [Header("Limpid Buttons")]
    public UIButton m_buttonLimpid;
    public UIButton m_buttonWaitForUnlock;
    public List<UIButton> m_SwipeZone;
    //-------------------------------------------------------------------------------------------------
    private UI_ChooseSong() : base()
    {
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();

#if DEVELOP
        m_tempButtonSong.gameObject.SetActive(true);
        m_labelSongGroupID.gameObject.SetActive(true);
#else
        m_tempButtonSong.gameObject.SetActive(false);
         m_labelSongGroupID.gameObject.SetActive(false);
#endif
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(StringTable st, GameDataDB gameDB)
    {
        m_labelComposer.text = st.GetString(25);    //"作曲"

        S_TextureTable_Tmp textureTmp = gameDB.GetGameDB<S_TextureTable_Tmp>().GetData(55);   //原曲字樣貼圖
        SetOriginalTexture(textureTmp.strTextureName);
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
    //Song Setting
    public void SetSongComposer(string composerName)
    {
        m_labelComposerName.text = composerName;
    }
    public void SetSongName(string songNameTexture)
    {
        Softstar.Utility.ChangeTexture(m_textureSongName, songNameTexture);
    }
    public void ShowSongRank(SongData songData)
    {
        UISprite sprite = null;
        for (int i = 0, iCount = (int)Enum_SongDifficulty.Max; i < iCount; ++i)
        {
            Enum_SongDifficulty difficulty = (Enum_SongDifficulty)i;
            switch (difficulty)
            {
                case Enum_SongDifficulty.Easy:
                    sprite = m_spriteRankEasy;
                    break;
                case Enum_SongDifficulty.Normal:
                    sprite = m_spriteRankNormal;
                    break;
                case Enum_SongDifficulty.Hard:
                    sprite = m_spriteRankHard;
                    break;
                default:
                    break;
            }
            if (!songData.CheckSongDifficulty(difficulty))
            {
                Softstar.Utility.ChangeSongRankSprite(sprite, Enum_SongRank.New);
                continue;
            }

            SongDifficultyData diffData = songData.GetSongDifficultyData(difficulty);
            if (diffData.LockStatus != Enum_DifficultyLockStatus.Lock)
            {
                Enum_SongRank rank = songData.GetSongDifficultyData(difficulty).Rank;
                Softstar.Utility.ChangeSongRankSprite(sprite, rank);
            }
            else
            {
                Softstar.Utility.ChangeAtlasSprite(sprite, 102);    //白色上鎖圖
            }
        }
    }
    public void SetOriginalTexture(string texture)
    {
        Softstar.Utility.ChangeTexture(m_textureOriginalSong, texture);
    }
    public void SetSongGroupID(int id)
    {
        m_labelSongGroupID.text = "SongGroupID: "+id.ToString();
    }
    //-------------------------------------------------------------------------------------------------
    //UI Switch
    public void SwitchNewSong(bool bSwitch)
    {
        m_textureNewSong.gameObject.SetActive(bSwitch);
    }
    public void SwitchOriginalSong(bool bSwitch)
    {
        m_textureOriginalSong.gameObject.SetActive(bSwitch);
    }
}
