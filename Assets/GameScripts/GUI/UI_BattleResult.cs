using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_BattleResult : NGUIChildGUI
{
    [Header("Song UI")]
    public UITexture m_textureSongName;
    public UITexture m_textureOriginalSong;
    public UILabel m_labelComposer;
    public UILabel m_labelComposerName;
    public GameObject m_newRank;

    [Header("Left UI")]
    public List<UISprite> m_spirteStarList = new List<UISprite>();
    public UITexture m_textureDifficulty;
    public GameObject m_newScore;
    public UITexture m_textureScore;
    public Slot_NumberPicker m_scoreNumberPicker;
    public GameObject m_newCombo;
    public UITexture m_textureCombo;
    public Slot_NumberPicker m_comboNumberPicker;

    [Header("Click Count UI")]
    public UILabel m_labelPerfect;
    public UILabel m_labelGood;
    public UILabel m_labelWeak;
    public UILabel m_labelMiss;

    [Header("Button UI")]
    public UIButton m_buttonPlayAgain;
    public UIButton m_buttonRank;

    private Dictionary<ScoreType, UILabel> m_labelClickCount;
    //-------------------------------------------------------------------------------------------------
    private UI_BattleResult() : base(){}
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        InitializeData();

        m_scoreNumberPicker.Initialize();
        m_comboNumberPicker.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    private void InitializeData()
    {
        m_labelClickCount = new Dictionary<ScoreType, UILabel>();
        m_labelClickCount.Add(ScoreType.Lost, m_labelMiss);
        m_labelClickCount.Add(ScoreType.Weak, m_labelWeak);
        m_labelClickCount.Add(ScoreType.Great, m_labelGood);
        m_labelClickCount.Add(ScoreType.Fantastic, m_labelPerfect);
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(PlayerDataSystem playerDataSys ,GameDataDB gameDB, StringTable st)
    {
        T_GameDB<S_TextureTable_Tmp> textureDB = gameDB.GetGameDB<S_TextureTable_Tmp>();
        //最高分數貼圖
        string texturePath = textureDB.GetData(69).strTextureName;
        Softstar.Utility.ChangeTexture(m_textureScore, texturePath);
        //最高連擊貼圖
        texturePath = textureDB.GetData(70).strTextureName;
        Softstar.Utility.ChangeTexture(m_textureCombo, texturePath);
        //歌曲名稱貼圖
        Softstar.Utility.ChangeTexture(m_textureSongName, playerDataSys.GetSongNameTexture(playerDataSys.GetCurrentSongData()));
        m_textureSongName.MakePixelPerfect();
        //原曲字樣貼圖
        texturePath = textureDB.GetData(18).strTextureName;
        Softstar.Utility.ChangeTexture(m_textureOriginalSong, texturePath);
        SwitchOriginalSong(playerDataSys.GetCurrentSongData().IsOriginalSong);
        //作者字串
        m_labelComposer.text = st.GetString(25);  //"作曲"
        m_labelComposerName.text = playerDataSys.GetSongComposerName(playerDataSys.GetCurrentSongData());

        //設定歌曲遊玩資料
        SongPlayData playData = playerDataSys.GetCurrentSongPlayData();
        SetDifficultyTexture(playData.SongDifficulty);
        SetCombo(playData.Combo);
        SetScore(playData.Score);
        SetStar(playData.Star);

        //設定點擊次數
        Dictionary<ScoreType, int> clickList = playData.GetPlayClickCountList();
        foreach (KeyValuePair<ScoreType, int> data in clickList)
        {
            SetClickCount(data.Key, data.Value);
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
    #region Setting UI
    public void SetDifficultyTexture(Enum_SongDifficulty difficulty)
    {
        Softstar.Utility.ChangeSongDifficultyTexture(m_textureDifficulty, difficulty);
    }
    public void SetScore(int score)
    {
        m_scoreNumberPicker.SetNumber(score);
    }
    public void SetCombo(int combo)
    {
        m_comboNumberPicker.SetNumber(combo);    
    }
    public void SetNewUI(bool isScoreNew, bool isComboNew, bool isRankNew)
    {
        m_newScore.SetActive(isScoreNew);
        m_newCombo.SetActive(isComboNew);
        m_newRank.SetActive(isRankNew);
    }
    public void SetClickCount(ScoreType status, int count)
    {
        m_labelClickCount[status].text = count.ToString();
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
    //-------------------------------------------------------------------------------------------------
    public void SwitchOriginalSong(bool bSwitch)
    {
        m_textureOriginalSong.enabled = bSwitch;
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchNewScore(bool bSwitch)
    {
        m_newScore.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchNewCombo(bool bSwitch)
    {
        m_newCombo.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchNewRank(bool bSwitch)
    {
        m_newRank.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void PlayNumberPicker()
    {
        m_scoreNumberPicker.Play(false);
        m_comboNumberPicker.Play(false);
    }
}
