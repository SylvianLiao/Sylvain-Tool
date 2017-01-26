using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using WellFired;

public class UI_BattleResultAnim : NGUIChildGUI
{
    public Animator m_animator;
    public USSequencer m_sequencer;
    public BattleResultEvent m_sequencerEvent_PlayScore;
    public BattleResultEvent m_sequencerEvent_PlayCombo;

    [Header("Develop")]
    public UIButton m_buttonSkip;

    [Header("Song")]
    public UITexture m_textureSongName;
    public UITexture m_textureOriginalSong;
    public UITexture m_textureDifficulty;

    [Header("Click Count")]
    public UILabel m_labelPerfect;
    public UILabel m_labelGood;
    public UILabel m_labelWeak;
    public UILabel m_labelMiss;

    [Header("Score And Combo")]
    public GameObject m_newScore;
    public UITexture m_textureScore;
    public Slot_NumberPicker m_scoreNumberPicker;
    public GameObject m_newCombo;
    public UITexture m_textureCombo;
    public Slot_NumberPicker m_comboNumberPicker;

    private Dictionary<ScoreType, UILabel> m_labelClickCount;
    public delegate void AnimPlayFinishEvent();
    public AnimPlayFinishEvent OnAnimPlayFinish;
    //-------------------------------------------------------------------------------------------------
    private UI_BattleResultAnim() : base() { }
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        InitializeData();
        m_scoreNumberPicker.Initialize();
        m_comboNumberPicker.Initialize();

        m_buttonSkip.gameObject.SetActive(false);
#if DEVELOP
        m_buttonSkip.gameObject.SetActive(true);
#endif
    }
    //-------------------------------------------------------------------------------------------------
    public void InitSequencer(SoundSystem soundSys)
    {
        m_sequencer.UseRunTimeFixed = true;
        m_sequencerEvent_PlayScore.m_soundSystem = soundSys;
        m_sequencerEvent_PlayCombo.m_soundSystem = soundSys;
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
    public void InitializeUI(PlayerDataSystem dataSystem, GameDataDB gameDB)
    {
        T_GameDB<S_TextureTable_Tmp> textureDB = gameDB.GetGameDB<S_TextureTable_Tmp>();
        //最高分數貼圖
        string texturePath = textureDB.GetData(69).strTextureName;
        Softstar.Utility.ChangeTexture(m_textureScore, texturePath);
        //最高連擊貼圖
        texturePath = textureDB.GetData(70).strTextureName;
        Softstar.Utility.ChangeTexture(m_textureCombo, texturePath);
        //歌曲名稱貼圖
        Softstar.Utility.ChangeTexture(m_textureSongName, dataSystem.GetSongNameTexture(dataSystem.GetCurrentSongData()));
        m_textureSongName.MakePixelPerfect();
        //原曲字樣貼圖
        texturePath = textureDB.GetData(18).strTextureName;
        Softstar.Utility.ChangeTexture(m_textureOriginalSong, texturePath);
        SwitchOriginalSong(dataSystem.GetCurrentSongData().IsOriginalSong);

        //設定歌曲遊玩資料
        SongPlayData playData = dataSystem.GetCurrentSongPlayData();
        SetDifficultyTexture(playData.SongDifficulty);
        SetCombo(playData.Combo);
        SetScore(playData.Score);

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
        m_sequencerEvent_PlayScore.m_soundSystem.PlaySound(21); //結算動畫第一段音效
        m_sequencer.Play();
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
    public void SetClickCount(ScoreType status, int count)
    {
        m_labelClickCount[status].text = count.ToString();
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
    public void PlayScoreNumberPicker()
    {
        m_scoreNumberPicker.Play(false);
    }
    //-------------------------------------------------------------------------------------------------
    public void PlayComboNumberPicker()
    {
        m_comboNumberPicker.Play(false);
    }
}
