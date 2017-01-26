using System;
using System.Collections.Generic;

public class SongDifficultyData
{
    private Enum_SongDifficulty m_SongDifficulty;
    public Enum_SongDifficulty SongDifficulty
    {
        get { return m_SongDifficulty; }
        set { m_SongDifficulty = value; }
    }

    private int m_iScore;
    public int Score
    {
        get { return m_iScore; }
        set { m_iScore = value; }
    }
    private Enum_SongRank m_Rank;
    public Enum_SongRank Rank
    {
        get { return m_Rank; }
        set { m_Rank = value; }
    }
    private int m_iCombo;
    public int Combo
    {
        get { return m_iCombo; }
        set { m_iCombo = value; }
    }
    private int m_iStar;
    public int Star
    {
        get { return m_iStar; }
        set { m_iStar = value; }
    }

    private Enum_DifficultyLockStatus m_lockStatus;
    public Enum_DifficultyLockStatus LockStatus
    {
        get { return m_lockStatus; }
        set { m_lockStatus = value; }
    }

    public SongDifficultyData()
    {
        m_SongDifficulty = Enum_SongDifficulty.Normal;
        m_iScore = 0;
        m_Rank = Enum_SongRank.New;
        m_iCombo = 0;
        m_iStar = 0;
    }
    /// <summary>給非玩家擁有歌曲設定用</summary>
    public SongDifficultyData(Enum_SongDifficulty diff, Enum_DifficultyLockStatus lockStatus, int star)
    {
        m_SongDifficulty = diff;
        m_lockStatus = lockStatus;
        m_iStar = star;
        m_iScore = 0;
        m_Rank = Enum_SongRank.New;
        m_iCombo = 0;
    }
    /// <summary>給玩家擁有歌曲設定用</summary>
    public SongDifficultyData(Enum_SongDifficulty diff, Enum_DifficultyLockStatus lockStatus, int score, int combo, Enum_SongRank rank, int star)
    {
        m_SongDifficulty = diff;
        m_lockStatus = lockStatus;
        m_iScore = score;
        m_Rank = rank;
        m_iCombo = combo;
        m_iStar = star;
    }
    /// <summary>更新遊玩記錄</summary>
    public void UpdateData(int score, int combo, Enum_SongRank rank)
    {
        m_iScore = score;
        m_Rank = rank;
        m_iCombo = combo;
    }
}
