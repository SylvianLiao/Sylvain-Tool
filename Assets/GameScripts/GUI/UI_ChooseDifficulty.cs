using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using Softstar;

public class UI_ChooseDifficulty : NGUIChildGUI
{
    [Header("Song UI")]
    public UITexture m_textureSongName;
    public UITexture m_textureOriginalSong;
    public UILabel m_labelComposer;
    public UILabel m_labelComposerName;
    public UIPlayTween m_playTweenRank;
    public UISprite m_spriteRank;
    public UISprite m_spriteRankBG;
    public UIButton m_buttonStartGame;
    public UITexture m_textureStartGame;
    public UIButton m_buttonRanking;

    [Header("Left UI")]
    public UILabel m_labelBestScore;
    public Slot_NumberPicker m_scoreNumberPicker;
    public UILabel m_labelBestCombo;
    public Slot_NumberPicker m_comboNumberPicker;

    [Header("Bottom UI")]
    public TweenPosition m_tweenDifficulty;
    public List<UITexture> m_texturesdifficulty;
    public UIButton m_buttonPreDifficulty;
    public UIButton m_buttonNextDifficulty;
    public List<UISprite> m_spirteStarList = new List<UISprite>();
    public UIButton m_buttonIntroduce;
    public UILabel m_labelShortIntroduce;

    [Header("Develop Test")]
    public UILabel m_labelNodeSpeed;
    public UIButton m_buttonSpeedUp;
    public UIButton m_buttonSpeedDown;
    public UILabel  m_labelTotleNote;

    //-------------------------------------------------------------------------------------------------
    private UI_ChooseDifficulty() : base()
    {
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        m_playTweenRank.resetOnPlay = true;

#if DEVELOP
        m_labelNodeSpeed.gameObject.SetActive(true);
        m_buttonSpeedUp.gameObject.SetActive(true);
        m_buttonSpeedDown.gameObject.SetActive(true);
        m_labelTotleNote.gameObject.SetActive(true);
#else
        m_labelNodeSpeed.gameObject.SetActive(false);
        m_buttonSpeedUp.gameObject.SetActive(false);
        m_buttonSpeedDown.gameObject.SetActive(false);
        m_labelTotleNote.gameObject.SetActive(false);
#endif
    }
    public void InitializeUI(PlayerDataSystem dataSystem, GameDataDB gameDB, StringTable st)
    {
        SongData songData = dataSystem.GetCurrentSongData();
        //字串UI
        m_labelBestScore.text = st.GetString(22);   //"最高分數"
        m_labelBestCombo.text = st.GetString(23);   //"最高連擊"
        //歌曲名稱
        Softstar.Utility.ChangeTexture(m_textureSongName, dataSystem.GetSongNameTexture(songData));
        //作者名稱
        m_labelComposer.text = st.GetString(25);  //"作曲"
        m_labelComposerName.text = dataSystem.GetSongComposerName(songData);
        //文字貼圖轉換
        S_TextureTable_Tmp textureTmp = gameDB.GetGameDB<S_TextureTable_Tmp>().GetData(55);   //原曲字樣貼圖
        Softstar.Utility.ChangeTexture(m_textureOriginalSong, textureTmp.strTextureName);
        m_textureOriginalSong.gameObject.SetActive(songData.IsOriginalSong);
        textureTmp = gameDB.GetGameDB<S_TextureTable_Tmp>().GetData(57);   //開始字樣貼圖
        Softstar.Utility.ChangeTexture(m_textureStartGame, textureTmp.strTextureName);
        //歌曲簡介
        //m_labelShortIntroduce.text = dataSystem.GetSongIntroduce(songData);

        m_scoreNumberPicker.Initialize();
        m_comboNumberPicker.Initialize();
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
    #region Setting UI

    public void SetDifficultyTexture()
    {
        for (int i = m_texturesdifficulty.Count-1; i >= 0; --i)
        {
            Softstar.Utility.ChangeSongDifficultyTexture(m_texturesdifficulty[i], (Enum_SongDifficulty)i);
        }       
    }
    public void TweenDifficultyTexture(Enum_SongDifficulty difficulty)
    {
        m_tweenDifficulty.from = m_tweenDifficulty.transform.localPosition;
        m_tweenDifficulty.to = m_texturesdifficulty[(int)difficulty].transform.localPosition;
        m_tweenDifficulty.ResetToBeginning();
        m_tweenDifficulty.PlayForward();
    }
    public void SetDifficultyTexturePos(Enum_SongDifficulty difficulty)
    {
        m_tweenDifficulty.transform.localPosition = m_texturesdifficulty[(int)difficulty].transform.localPosition;
    }
    public void SetRankSprite(string rankSpriteName, string bgSpriteName)
    {
        Softstar.Utility.ChangeAtlasSprite(m_spriteRank, rankSpriteName);
        Softstar.Utility.ChangeAtlasSprite(m_spriteRankBG, bgSpriteName);
    }
    public void SetBestScore(int score)
    {
        m_scoreNumberPicker.SetNumber(score);
    }
    public void SetBestCombo(int combo)
    {
        m_comboNumberPicker.SetNumber(combo);
    }
    public void SetStar(int starCount)
    {
        int uiStarCount = starCount / 2;
        if (uiStarCount > m_spirteStarList.Count)
            return;

        for (int i = 0, iCount = m_spirteStarList.Count; i < iCount; ++i)
        {
            Enum_StarStatus star;
            if (i < uiStarCount)
                star = Enum_StarStatus.Full;
            else if (i == uiStarCount && ((starCount % 2) > 0))
                star = Enum_StarStatus.Half;
            else
                star = Enum_StarStatus.None;

            Softstar.Utility.ChangeStarSprite(m_spirteStarList[i], star);
        }
    }
    #endregion
    public void PlayNumberPicker()
    {
        m_scoreNumberPicker.Play(false);
        m_comboNumberPicker.Play(false);
    }
    //-------------------------------------------------------------------------------------------------
    public void PlayTweenRank()
    {
        m_playTweenRank.Play(true);
    }
  
    //Test---------------------------------------------------------------------------------------------------
    public void SetLabelNodeSpeed(float speed)
    {
        speed = Mathf.Round(speed * 10) / 10;
        m_labelNodeSpeed.text = "NodeSpeed: "+ speed.ToString();
    }
}
