using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class SongUnlockSystem : BaseSystem
{
    private GameDataDB m_gameDataDB;
    private T_GameDB<S_SongUnlock_Tmp> m_unlockDB;
    private Dictionary<int, List<S_SongUnlock_Tmp>> m_songUnlockDict;   //解鎖條件歌曲GUID, 解鎖DB資料
    private PlayerDataSystem m_dataSystem;

    public SongUnlockSystem(GameScripts.GameFramework.GameApplication app) : base(app) { }

    public override void Initialize()
    {
        base.Initialize();
        MainApplication mainApp = gameApplication as MainApplication;
        m_dataSystem = mainApp.GetSystem<PlayerDataSystem>();
        m_gameDataDB = mainApp.GetGameDataDB();
        m_unlockDB = m_gameDataDB.GetGameDB<S_SongUnlock_Tmp>();
        m_songUnlockDict = new Dictionary<int, List<S_SongUnlock_Tmp>>();

        SortSongUnlockList();
    }
    //-----------------------------------------------------------------------------------------
    public override void Update()
    {
    }
    //-----------------------------------------------------------------------------------------
    private void SortSongUnlockList()
    {
        m_unlockDB.ResetByOrder();
        for (int i = 0; i < m_unlockDB.GetDataSize(); ++i)
        {
            S_SongUnlock_Tmp unlockTmp = m_unlockDB.GetDataByOrder();
            if (!m_songUnlockDict.ContainsKey(unlockTmp.iConditionSong))
                m_songUnlockDict.Add(unlockTmp.iConditionSong, new List<S_SongUnlock_Tmp>());
            m_songUnlockDict[unlockTmp.iConditionSong].Add(unlockTmp);
        }
    }
    //-----------------------------------------------------------------------------------------
    //解鎖歌曲難度
    public bool UnlockSongDifficulty(int songGUID)
    {
        SongData songData = m_dataSystem.GetSongDataBySongGUID(songGUID);
        if (songData == null)
            return false;
        SongDifficultyData diffData = m_dataSystem.GetSongDifficultyDataBySongGUID(songGUID);
        if (diffData == null)
            return false;

        diffData.LockStatus = Enum_DifficultyLockStatus.TrueUnlock;
        if (songData.LockStatus == Enum_SongLockStatus.Lock)
        {
            songData.IsNewSong = true;
            songData.LockStatus = Enum_SongLockStatus.WaitForUnlock;
        }

        return true;
    }
    //-----------------------------------------------------------------------------------------
    /// <summary>查詢歌曲難度於DB設定中是否解鎖</summary>
    public bool InquireSongDiffcultyIsUnlock(int songGUID)
    {
        S_SongUnlock_Tmp unlocktmp = GetUnlockCondition(songGUID);
        if (unlocktmp == null)
            return false;

        return unlocktmp.IsUnlock;
    }
    //-----------------------------------------------------------------------------------------
    //取得特定歌曲難度的解鎖條件資料
    public S_SongUnlock_Tmp GetUnlockCondition(int songGUID)
    {
        m_unlockDB.ResetByOrder();
        for (int i = 0, iCount = m_unlockDB.GetDataSize(); i < iCount; ++i)
        {
            S_SongUnlock_Tmp unlockTmp = m_unlockDB.GetDataByOrder();
            if (unlockTmp.iSongGUID == songGUID)
                return unlockTmp;
        }
        return null;
    }

    #region Offline Function

    //-----------------------------------------------------------------------------------------
    /// <summary> 設定歌曲的顯示與解鎖(離線模式用) </summary>
    public void InitSongShowAndUnlock_Offline()
    {
        //設定歌曲預設的顯示與解鎖
        T_GameDB<S_Songs_Tmp> songDB = m_gameDataDB.GetGameDB<S_Songs_Tmp>();
        m_unlockDB.ResetByOrder();
        for (int i = 0; i < m_unlockDB.GetDataSize(); ++i)
        {
            S_SongUnlock_Tmp unlockTmp = m_unlockDB.GetDataByOrder();
            S_Songs_Tmp songTmp = songDB.GetData(unlockTmp.iSongGUID);
            SongData songData = m_dataSystem.GetSongDataByGroupID(songTmp.iGroupID);
            SongDifficultyData diffData = songData.GetSongDifficultyData((Enum_SongDifficulty)songTmp.iDifficulty);
            diffData.LockStatus = (unlockTmp.IsUnlock) ? Enum_DifficultyLockStatus.TrueUnlock : Enum_DifficultyLockStatus.Lock;
            songData.SetSongDifficultyPlayData(diffData);
            songData.LockStatus = (unlockTmp.IsUnlock) ? Enum_SongLockStatus.Unlock : Enum_SongLockStatus.Lock;
        }

        //檢查歌曲是否符合解鎖條件並設定
        foreach (KeyValuePair<int, List<S_SongUnlock_Tmp>> data in m_songUnlockDict)
        {
            if (data.Key <= 0)
                continue;

            UnlockNextSong_Offline(data.Key);
        }
    }
    //-----------------------------------------------------------------------------------------
    /// <summary>
    /// 解鎖下一首歌曲，會回傳解鎖成功與否(離線模式用)
    /// </summary>
    /// <param name="songData"></param>
    /// <param name="unlockSongGUID">解鎖成功的歌曲ID</param>
    /// <returns></returns>
    public bool UnlockNextSong_Offline(int songGUID)
    {
        int[] unlockSongGUID;
        return UnlockNextSong_Offline(songGUID, out unlockSongGUID);
    }
    public bool UnlockNextSong_Offline(int songGUID, out int[] unlockSongGUID)
    {
        unlockSongGUID = null;
        if (!m_songUnlockDict.ContainsKey(songGUID))
            return false;

        T_GameDB<S_Songs_Tmp> songDB = m_gameDataDB.GetGameDB<S_Songs_Tmp>();
        bool unlockSuccess = false;
        List<S_SongUnlock_Tmp> unlockTmpList = m_songUnlockDict[songGUID];
        List<int> unlockSongGUIDList = new List<int>();
        foreach (S_SongUnlock_Tmp unlockTmp in unlockTmpList)
        {
            //取得被解鎖歌曲的難度資料
            SongDifficultyData unlockDiffData = m_dataSystem.GetSongDifficultyDataBySongGUID(unlockTmp.iSongGUID);
            //過濾已解鎖難度
            if (unlockDiffData.LockStatus == Enum_DifficultyLockStatus.TrueUnlock)
                continue;
            //取得解鎖條件歌曲的難度資料
            SongDifficultyData conditionDiffData = m_dataSystem.GetSongDifficultyDataBySongGUID(songGUID);
            //檢查評價是否達標
            if ((int)conditionDiffData.Rank < (int)unlockTmp.iConditionRank)
                continue;

            //記錄解鎖成功的歌曲GUID
            unlockSongGUIDList.Add(unlockTmp.iSongGUID);
            //設定歌曲解鎖狀態
            UnlockSongDifficulty(unlockTmp.iSongGUID);
            /*
            unlockDiffData.LockStatus = Enum_DifficultyLockStatus.TrueUnlock;
            S_Songs_Tmp unlockSongTmp = songDB.GetData(unlockTmp.iSongGUID);
            SongData unlockSongData = m_dataSystem.GetSongDataByGroupID(unlockSongTmp.iGroupID);
            unlockSongData.LockStatus = (m_dataSystem.InquireSongIsPlayed(unlockSongData.SongGroupID)) ? Enum_SongLockStatus.Unlock : Enum_SongLockStatus.WaitForUnlock;
            */
            unlockSuccess = true;
        }
        unlockSongGUID = (unlockSongGUIDList.Count == 0) ? null : unlockSongGUIDList.ToArray();
        return unlockSuccess;
    }
    //解鎖所有歌曲(離線模式功能)-----------------------------------------------------------------------------------------
    public void UnlockAndShowAllSong_Offline()
    {
        m_unlockDB.ResetByOrder();
        for (int i = 0; i < m_unlockDB.GetDataSize(); ++i)
        {
            S_SongUnlock_Tmp unlockTmp = m_unlockDB.GetDataByOrder();
            UnlockSongDifficulty(unlockTmp.iSongGUID);
            /*
            SongData songData = m_dataSystem.GetSongDataByGroupID(unlockTmp.iSongGUID);
            songData.LockStatus = Enum_SongLockStatus.Unlock;
            Dictionary<Enum_SongDifficulty, SongDifficultyData> songDifficultyData = songData.GetSongDifficultyPlayDataList();
            foreach (var data in songDifficultyData)
            {
                data.Value.LockStatus = Enum_DifficultyLockStatus.TrueUnlock;
            }*/
        }
    }
    #endregion
}
