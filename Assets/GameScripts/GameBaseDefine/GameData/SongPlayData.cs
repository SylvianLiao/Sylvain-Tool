using System;
using System.Collections.Generic;

public class SongPlayData : SongDifficultyData
{
    private Dictionary<ScoreType, int> m_playClickCountList = new Dictionary<ScoreType, int>();

    public SongPlayData() : base (){}

    public SongPlayData(Enum_SongDifficulty diff, 
        int score, 
        int combo, 
        Enum_SongRank rank,
        int star,
        int perfect,
        int good,
        int weak,
        int miss ) : base(diff, Enum_DifficultyLockStatus.TrueUnlock, score, combo, rank, star)
    {
        m_playClickCountList.Add(ScoreType.Fantastic, perfect);
        m_playClickCountList.Add(ScoreType.Great, good);
        m_playClickCountList.Add(ScoreType.Weak, weak);
        m_playClickCountList.Add(ScoreType.Lost, miss);
    }

    public void SetPlayClickCount(ScoreType status, int count)
    {
        m_playClickCountList[status] = count;
    }

    public int GetPlayClickCount(ScoreType status)
    {
        if (!m_playClickCountList.ContainsKey(status))
            return -1;

        return m_playClickCountList[status];
    }

    public Dictionary<ScoreType, int> GetPlayClickCountList()
    {
        return m_playClickCountList;
    }
}
