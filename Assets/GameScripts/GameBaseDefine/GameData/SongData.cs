using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class SongData
{
    //歌曲Group ID
    private int m_iSongGroup;       
    public int SongGroupID
    {
        get { return m_iSongGroup; }
        set { m_iSongGroup = value; }
    }
    //歌曲所屬的主題館ID
    private int m_iBelongThemeID;   
    public int BelongThemeID
    {
        get { return m_iBelongThemeID; }
        set { m_iBelongThemeID = value; }
    }
    //是否為新曲
    private bool m_bIsNewSong;     
    public bool IsNewSong
    {
        get { return m_bIsNewSong; }
        set { m_bIsNewSong = value; }
    }
    //是否為原曲
    private bool m_bIsOriginalSong;
    public bool IsOriginalSong
    {
        get { return m_bIsOriginalSong; }
        set { m_bIsOriginalSong = value; }
    }
    //上鎖狀態
    private Enum_SongLockStatus m_lockStatus;
    public Enum_SongLockStatus LockStatus
    {
        get { return m_lockStatus; }
        set { m_lockStatus = value; }
    }

    private Dictionary<Enum_SongDifficulty, SongDifficultyData> m_songDifficultyData;

    public SongData()
    {
        m_iSongGroup = 0;
        m_iBelongThemeID = 0;
        LockStatus = Enum_SongLockStatus.Lock;
        m_bIsNewSong = false;
        m_bIsOriginalSong = false;
        m_songDifficultyData = new Dictionary<Enum_SongDifficulty, SongDifficultyData>();
    }
    public SongData(int themeID, int groupID, Enum_SongLockStatus lockStatus, bool isNew, bool isOrigin)
    {
        m_iSongGroup = groupID;
        m_iBelongThemeID = themeID;
        LockStatus = lockStatus;
        m_bIsNewSong = isNew;
        m_bIsOriginalSong = isOrigin;
        m_songDifficultyData = new Dictionary<Enum_SongDifficulty, SongDifficultyData>();
    }
    //-----------------------------------------------------------------------------------------------------------
    //Get Data Methods
    //取得歌曲難度資料，沒有該難度資料則回傳null
    public SongDifficultyData GetSongDifficultyData(Enum_SongDifficulty difficulty)
    {
        if (m_songDifficultyData.ContainsKey(difficulty) == false)
        {
            return null;
        }
        return m_songDifficultyData[difficulty];
    }
    public Dictionary<Enum_SongDifficulty, SongDifficultyData> GetSongDifficultyPlayDataList()
    {
        return m_songDifficultyData;
    }
    //-----------------------------------------------------------------------------------------------------------
    //Set Data Methods
    public void SetSongDifficultyPlayData(SongDifficultyData difficultyData)
    {
        Enum_SongDifficulty difficulty = difficultyData.SongDifficulty;

        if (m_songDifficultyData.ContainsKey(difficulty))
            m_songDifficultyData[difficulty] = difficultyData;
        else
            m_songDifficultyData.Add(difficulty, difficultyData);
    }
    //-----------------------------------------------------------------------------------------------------------
    public bool CheckSongDifficulty(Enum_SongDifficulty difficulty)
    {
        return m_songDifficultyData.ContainsKey(difficulty);
    }
}

