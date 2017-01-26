using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_SongIntroduce : NGUIChildGUI
{
    public UITexture m_textureSongName;
    public UITexture m_textureOriginalSong;
    public UILabel m_labelFullComposer;

    public UIButton m_buttonCloseIntroduce;
    public UILabel m_labelShortIntroduce;
    public UILabel m_labelFullIntroduce;

    //-------------------------------------------------------------------------------------------------
    private UI_SongIntroduce() : base(){}

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(PlayerDataSystem dataSystem, GameDataDB gameDB)
    {
        SongData songData = dataSystem.GetCurrentSongData();
        //歌曲介紹
        m_labelShortIntroduce.text = dataSystem.GetSongIntroduce(songData);
        m_labelFullIntroduce.text = dataSystem.GetSongFullIntroduce(songData);
        //歌曲作者
        m_labelFullComposer.text = dataSystem.GetSongFullComposerName(songData);
        //歌曲名稱
        Softstar.Utility.ChangeTexture(m_textureSongName, dataSystem.GetSongNameTexture(songData));
        //原曲字樣貼圖
        S_TextureTable_Tmp textureTmp = gameDB.GetGameDB<S_TextureTable_Tmp>().GetData(55);   
        Softstar.Utility.ChangeTexture(m_textureOriginalSong, textureTmp.strTextureName);
        m_textureOriginalSong.gameObject.SetActive(songData.IsOriginalSong);
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
}
