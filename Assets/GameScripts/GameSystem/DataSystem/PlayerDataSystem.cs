using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

/// <summary>玩家資料系統</summary>
public class PlayerDataSystem : BaseSystem
{
    private GameDataDB m_gameDataDB;
    private T_GameDB<S_Themes_Tmp> m_themeDB;
    private StringTable m_stringTable;

    private PlayerData m_playerData;                //玩家基本資料

    private List<ThemeData> m_allThemeDataList;     //所有主題館&歌曲資料

    private ThemeData m_currentThemeData;           //玩家目前所在的主題館資料+主題館解鎖的歌曲資料清單(歌曲清單的順序即為選歌UI顯示順序)

    private SongData m_currentSongData;             //玩家目前所在的歌曲資料+該歌曲的難度資料

    private SongPlayData m_currentSongPlayData;     //玩家遊玩中的資料(注意! 此資料與SongDifficultyData不同! SongDifficultyData為歌曲難度資料)

    public PlayerDataSystem(GameScripts.GameFramework.GameApplication app) : base(app) {}

    //---------------------------------------------------------------------------------------------------
    public override void Initialize()
    {
        base.Initialize();

        m_playerData = new PlayerData();
        m_currentThemeData = new ThemeData();
        m_currentSongData = new SongData();
        m_currentSongPlayData = new SongPlayData();
        m_allThemeDataList = new List<ThemeData>();

        MainApplication mainApp = gameApplication as MainApplication;
        m_gameDataDB = mainApp.GetGameDataDB();
        m_themeDB = m_gameDataDB.GetGameDB<S_Themes_Tmp>();
        m_stringTable = mainApp.GetStringTable();

        SetDefaultThemeAndSong(true);
    }
    //---------------------------------------------------------------------------------------------------
    public void RealseAllData()
    {
        m_playerData = new PlayerData();
        m_currentThemeData = new ThemeData();
        m_currentSongData = new SongData();
        m_currentSongPlayData = new SongPlayData();
        m_allThemeDataList = new List<ThemeData>();
    }
    /// <summary>預設選擇第一個主題館的第一首歌曲，若之後接收到玩家資料會覆蓋此資料</summary>
    public void SetDefaultThemeAndSong(bool isInit = false)
    {
        m_themeDB.ResetByOrder();
        S_Themes_Tmp themeTmp = m_themeDB.GetDataByOrder();
        m_currentThemeData.GUID = themeTmp.GUID;

        //有第一首歌的資料則填入
        if (!isInit)
        {
            SetCurrentTheme(m_allThemeDataList[0]);
            List<SongData> songDataList = m_allThemeDataList[0].GetSongDataList();
            SetCurrentSong(songDataList[0]);
        }    
        //沒有則只設定歌曲群組與所屬主題館編號
        else
        {
            m_currentSongData.BelongThemeID = themeTmp.GUID;
            m_currentSongData.SongGroupID = themeTmp.m_iSongsGroupList[0];
        }
      
    }
    //---------------------------------------------------------------------------------------------------
    public void SetPlayerData(PlayerData data)
    {
        m_playerData = data;
    }
    public PlayerData GetPlayerData()
    {
        return m_playerData;
    }
    public void SetPlayerDiamond(int diamond)
    {
        m_playerData.Diamond = diamond;
    }
    public int GetPlayerDiamond()
    {
        return m_playerData.Diamond;
    }
    //---------------------------------------------------------------------------------------------------
    public void SetCurrentTheme(ThemeData themeData)
    {
        m_currentThemeData = themeData;
    }
    public ThemeData GetCurrentThemeData()
    {
        return m_currentThemeData;
    }
    public int GetCurrentThemeID()
    {
        return m_currentThemeData.GUID;
    }
    //---------------------------------------------------------------------------------------------------
    public void SetCurrentSong(SongData songData)
    {
        m_currentSongData = songData;
    }
    public SongData GetCurrentSongData()
    {
        return m_currentSongData;
    }
    //---------------------------------------------------------------------------------------------------
    public void SetCurrentSongPlayData(SongPlayData playData)
    {
        m_currentSongPlayData = playData;
    }
    public SongPlayData GetCurrentSongPlayData()
    {
        return m_currentSongPlayData;
    }
    /// <summary>設定所有主題館資料</summary>
    public void SetAllThemeDataList(List<ThemeData> list)
    {
        if (list == null)
            return;

        m_allThemeDataList = list;
    }
    /// <summary>設定所有主題館資料</summary>
    public List<ThemeData> GetAllThemeDataList()
    {
        return m_allThemeDataList;
    }
    /// <summary>新增主題館資料</summary>
    public void AddThemeData(ThemeData data)
    {
        m_allThemeDataList.Add(data);
    }
    /// <summary>取得主題館資料</summary>
    public ThemeData GetThemeDataByGUID(int themeID)
    {
        for (int i = 0, iCount = m_allThemeDataList.Count; i < iCount; ++i)
        {
            if (m_allThemeDataList[i].GUID == themeID)
                return m_allThemeDataList[i];
        }
        return null;
    }
    /// <summary>取得歌曲資料</summary>
    public SongData GetSongDataByGroupID(int groupID)
    {
        if (groupID <= 0)
            return null;

        for (int i = 0, iCount = m_allThemeDataList.Count; i < iCount; ++i)
        {
            List<SongData> songList = m_allThemeDataList[i].GetSongDataList();
            for (int m = 0, mCount = songList.Count; m < mCount; ++m)
            {
                if (songList[m].SongGroupID == groupID)
                    return songList[m];
            }
        }
        return null;
    }
    /// <summary>取得歌曲資料</summary>
    public SongData GetSongDataBySongGUID(int guid)
    {
        if (guid <= 0)
            return null;

        for (int i = 0, iCount = m_allThemeDataList.Count; i < iCount; ++i)
        {
            List<SongData> songList = m_allThemeDataList[i].GetSongDataList();
            for (int m = 0, mCount = songList.Count; m < mCount; ++m)
            {
                List<S_Songs_Tmp> songTmpList = m_gameDataDB.m_songTmpDict[songList[m].SongGroupID];
                foreach (var data in songTmpList)
                {
                    if (data.GUID == guid)
                        return songList[m];
                }
            }
        }
        return null;
    }
    /// <summary>取得歌曲難度資料</summary>
    public SongDifficultyData GetSongDifficultyDataBySongGUID(int guid)
    {
        if (guid <= 0)
            return null;

        S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(guid);
        SongData songData = GetSongDataByGroupID(songTmp.iGroupID);
        SongDifficultyData diffData = songData.GetSongDifficultyData((Enum_SongDifficulty)songTmp.iDifficulty);
        return diffData;
    }
    #region Theme Methods

    /// <summary>取得主題館背景Prefab路徑</summary>
    public string GetThemeBgPrefabName(int themeID)
    {
        S_Themes_Tmp themeTmp = m_themeDB.GetData(themeID);
        string prefabPath = "Theme_" + themeTmp.GUID.ToString() + "/" + themeTmp.strBgPrefab;
        return prefabPath;
    }
    /// <summary>取得主題館音樂路徑</summary>
    public string GetThemeMusicName(int themeID)
    {
        S_Themes_Tmp themeTmp = m_themeDB.GetData(themeID);
        string musicPath = "Theme_" + themeTmp.GUID.ToString() + "/" + themeTmp.strMusicName;
        return musicPath;
    }
    /// <summary>取得主題館介紹Icon資源路徑</summary>
    public string GetThemeInforIconPath(int themeID)
    {
        S_Themes_Tmp themeTmp = m_themeDB.GetData(themeID);
        string iconPath = "Theme_" + themeTmp.GUID.ToString() + "/" + themeTmp.strInforIcon;
        return iconPath;
    }
    /// <summary>取得主題館介紹貼圖資源路徑</summary>
    public string GetThemeInforTexturePath(int themeID)
    {
        S_Themes_Tmp themeTmp = m_themeDB.GetData(themeID);
        string texturePath = "Theme_" + themeTmp.GUID.ToString() + "/" + themeTmp.strInforTexture;
        return texturePath;
    }
    /// <summary>取得歌單</summary>
    public List<SongData> GetSongDataList(int themeID = int.MaxValue)
    {
        ThemeData themeData = (themeID == int.MaxValue)?m_currentThemeData:GetThemeDataByGUID(themeID);
        return themeData.GetSongDataList();
    }
    /// <summary>根據歌曲位於歌單中的Index取得歌曲資料</summary>
    public SongData GetSongDataByIndex(int index, int themeID = int.MaxValue)
    {
        if (themeID <= 0)
            return null;

        List<SongData> songList = GetSongDataList(themeID);
        if (index < 0 || index >= songList.Count)
            return null;

        return songList[index];
    }
    /// <summary>取得該歌曲於歌單中的Index</summary>
    public int GetSongIndex(int songGroupID, int themeID = int.MaxValue)
    {
        if (songGroupID <= 0)
            return -1;

        List<SongData> songList = GetSongDataList(themeID);
        for (int i = 0, iCount = songList.Count; i < iCount; ++i)
        {
            if (songList[i].SongGroupID == songGroupID)
                return i;
        }
        return -1;
    }
    /// <summary>取得上一首或下一首歌曲資料</summary>
    private SongData GetNextOrPreSongData(bool isNext, int songGroupID, int themeID = int.MaxValue)
    {
        if (songGroupID <= 0)
            return null;

        List<SongData> songList = GetSongDataList(themeID);
        for (int i = 0, iCount = songList.Count; i < iCount; ++i)
        {
            if (songList[i].SongGroupID != songGroupID)
                continue;

            int songIndex = (isNext) ? i + 1 : i - 1;
            songIndex = (songIndex + iCount) % iCount;
            return songList[songIndex];
        }

        return null;
    }
    /// <summary>取得下一首歌曲資料</summary>
    public SongData GetNextSongData(int songGroupID, int themeID = int.MaxValue)
    {
        return GetNextOrPreSongData(true, songGroupID, themeID);
    }
    /// <summary>取得上一首歌曲資料</summary>
    public SongData GetPreSongData(int songGroupID, int themeID = int.MaxValue)
    {
        return GetNextOrPreSongData(false, songGroupID, themeID);
    }
    #endregion

    #region Song Methods
    /// <summary>取得歌曲樣板資料</summary>
    public S_Songs_Tmp GetSongTmp(SongData songData, Enum_SongDifficulty diff)
    {
        if (m_gameDataDB.m_songTmpDict.ContainsKey(songData.SongGroupID) == false)
        {
            return null;
        }

        List<S_Songs_Tmp> songTmpList = m_gameDataDB.m_songTmpDict[songData.SongGroupID];
        foreach (S_Songs_Tmp tmp in songTmpList)
        {
            if (tmp.iDifficulty == (int)diff)
                return tmp;
        }

        return null;
    }
    /// <summary>取得歌曲名稱</summary>///     
    public string GetSongName(int groupID)
    {
        if (m_gameDataDB.m_songTmpDict.ContainsKey(groupID) == false)
        {
            return "";
        }
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[groupID][0];
        string songName = m_stringTable.GetString(songTmp.iShowName);
        return songName;
    }
    public string GetSongName(SongData songData)
    {
        return GetSongName(songData.SongGroupID);
    }
    /// <summary>取得歌曲名稱圖示</summary>
    public string GetSongNameTexture(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string songPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strNameTexture;
        return songPath;
    }
    /// <summary>取得歌曲作者名稱</summary>
    public string GetSongComposerName(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        string composerName = m_stringTable.GetString(songTmp.iComposer);
        return composerName;
    }
    /// <summary>取得歌曲完整作者</summary>
    public string GetSongFullComposerName(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        string composerName = m_stringTable.GetString(songTmp.iFullComposer);
        return composerName;
    }
    /// <summary>取得歌曲簡介</summary>
    public string GetSongIntroduce(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        string songintroduce = m_stringTable.GetString(songTmp.iIntroduce);
        return songintroduce;
    }
    /// <summary>取得歌曲完整介紹</summary>
    public string GetSongFullIntroduce(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        string songintroduce = m_stringTable.GetString(songTmp.iFullIntroduce);
        return songintroduce;
    }
    /// <summary>取得歌曲音樂資源路徑</summary>
    public string GetSongPath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string songPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strAssetName;
        return songPath;
    }
    public string GetSongPath(int themeGUID, int songGUID)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(songGUID);
        string songPath = "Theme_" + themeGUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strAssetName;
        return songPath;
    }
    /// <summary>取得歌曲短版音樂資源路徑</summary>
    public string GetShortSongPath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string shortSongPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strShortAssetName;
        return shortSongPath;
    }
    /// <summary>取得歌曲音樂貼圖資源路徑</summary>
    public string GetSongTexturePath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string texturePath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strTexture;
        return texturePath;
    }
    /// <summary>取得歌曲背景材質資源路徑</summary>
    public string GetSongBgResourcePath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string materialPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strBgMaterial;
        return materialPath;
    }
    /// <summary>取得歌曲戰鬥背景物件資源路徑</summary>
    public string GetSongBattleBgResourcePath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string prefabPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strBattleBG;
        return prefabPath;
    }
    /// <summary>取得歌曲選單材質資源路徑</summary>
    public string GetSongMaterialResourcePath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string materialPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strMenuMaterial;
        return materialPath;
    }
    /// <summary>取得歌曲音樂譜面資料路徑</summary>
    public string GetSheetMusicDataPath(SongData songData, Enum_SongDifficulty diff)
    {
        S_Songs_Tmp songTmp = null;
        foreach (var tmp in m_gameDataDB.m_songTmpDict[songData.SongGroupID])
        {
            if (tmp.iDifficulty == (int)diff)
            {
                songTmp = tmp;
                break;
            }
        }
        S_Themes_Tmp themeTmp = m_gameDataDB.GetGameDB<S_Themes_Tmp>().GetData(songData.BelongThemeID);
        string sheetMusicPath = "Theme_" + themeTmp.GUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strSheetMusicName;
        return sheetMusicPath;
    }
    public string GetSheetMusicDataPath(int themeGUID, int songGUID)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(songGUID);
        string sheetMusicPath = "Theme_" + themeGUID.ToString() + "/" + "SongGroup_" + songTmp.iGroupID.ToString() + "/" + songTmp.strSheetMusicName;
        return sheetMusicPath;
    }
    /// <summary>取得歌曲節點下降速度</summary>
    public float GetSongNodeSpeed(SongData songData, Enum_SongDifficulty diff)
    {
        if (m_gameDataDB.m_songTmpDict.ContainsKey(songData.SongGroupID) == false)
        {
            return 1.0f;
        }

        foreach (var tmp in m_gameDataDB.m_songTmpDict[songData.SongGroupID])
        {
            if (tmp.iDifficulty == (int)diff)
            {
                return tmp.fNodeSpeed;
            }
        }
        UnityDebugger.Debugger.Log("GetSongNodeSpeed Failed!! Song Group ID = " + songData.SongGroupID + ", Difficulty = " + diff.ToString());
        return 1.0f;
    }
    public float GetSongNodeSpeed(int songGUID)
    {
        S_Songs_Tmp tmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(songGUID);
        return tmp.fNodeSpeed;
    }
    //Test-------------------------------------------------------------
    public float SetSongNodeSpeed(SongData songData, Enum_SongDifficulty diff, float plus)
    {
        if (m_gameDataDB.m_songTmpDict.ContainsKey(songData.SongGroupID) == false)
        {
            return 1.0f;
        }

        foreach (var tmp in m_gameDataDB.m_songTmpDict[songData.SongGroupID])
        {
            if (tmp.iDifficulty == (int)diff)
            {
                tmp.fNodeSpeed += plus;
                if (tmp.fNodeSpeed <= 0) tmp.fNodeSpeed = 0.1f;
                return tmp.fNodeSpeed;
            }
        }
        UnityDebugger.Debugger.Log("SetSongNodeSpeed Failed!! Song Group ID = " + songData.SongGroupID + ", Difficulty = " + diff.ToString());
        return 1.0f;
    }
    //Test-----------------------------------------------------------------------------------------------------------
    #endregion

    #region 校正用歌曲資料
    /// <summary>取得校正用歌曲音樂資源路徑</summary>
    public string GetAdjustSongPath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        string songPath = "Adjust" + "/" + songTmp.strAssetName;
        return songPath;
    }
    /// <summary>取得校正用歌曲音樂譜面資料路徑</summary>
    public string GetAdjustSheetMusicDataPath(SongData songData)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.m_songTmpDict[songData.SongGroupID][0];
        string sheetMusicPath = "Adjust" + "/" + songTmp.strSheetMusicName;
        return sheetMusicPath;
    }
    #endregion
    #region 教學用歌曲資料
    /// <summary>取得教學用歌曲音樂資源路徑</summary>
    public string GetTutorialSongPath(int songGUID)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(songGUID);
        string songPath = "Tutorial" + "/" + songTmp.strAssetName;
        return songPath;
    }
    /// <summary>取得教學用歌曲音樂譜面資料路徑</summary>
    public string GetTutorialSheetMusicDataPath(int songGUID)
    {
        S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(songGUID);
        string sheetMusicPath = "Tutorial" + "/" + songTmp.strSheetMusicName;
        return sheetMusicPath;
    }
    #endregion
    #region Inquire Function
    /// <summary>查詢分數可以達到特定歌曲的評價</summary>
    public Enum_SongRank InquireSongRankByScore(int groupID, Enum_SongDifficulty diff, int score)
    {
        if (!m_gameDataDB.m_songTmpDict.ContainsKey(groupID) || score <= 0)
            return Enum_SongRank.New;

        Enum_SongRank rank;
        List<S_Songs_Tmp> songTmpList = m_gameDataDB.m_songTmpDict[groupID];
        for (int i = 0, iCount = songTmpList.Count; i < iCount; ++i)
        {
            if (songTmpList[i].iDifficulty == (int)diff)
            {
                if (score >= songTmpList[i].iScore_G)
                    rank = Enum_SongRank.G;
                else if (score >= songTmpList[i].iScore_S)
                    rank = Enum_SongRank.S;
                else if (score >= songTmpList[i].iScore_A)
                    rank = Enum_SongRank.A;
                else if (score >= songTmpList[i].iScore_B)
                    rank = Enum_SongRank.B;
                else if (score >= songTmpList[i].iScore_C)
                    rank = Enum_SongRank.C;
                else
                    rank = Enum_SongRank.D;
                return rank;
            }
        }
        return Enum_SongRank.New;
    }
    /// <summary>查詢該歌曲是否使用低音版戰鬥音效</summary>
    public bool InquireSongIsUseLowVolume(int groupID)
    {
        List<S_Songs_Tmp> songTmpList = m_gameDataDB.m_songTmpDict[groupID];
        for (int i = 0, iCount = songTmpList.Count; i < iCount; ++i)
        {
            if ((int)m_currentSongPlayData.SongDifficulty == songTmpList[i].iDifficulty && songTmpList[i].bUseLowVolume)
                return true;
        }
        return false;
    }  
    /// <summary>玩家是否遊玩過該歌曲</summary>
    public bool InquireSongIsPlayed(int songGroupID)
    {
        SongData songData = GetSongDataByGroupID(songGroupID);
        foreach (SongDifficultyData data in songData.GetSongDifficultyPlayDataList().Values)
        {
            if (data.Score > 0)
                return true;
        }

        return false;
    }
    /// <summary>是否刷新分數</summary>
    public bool InquireScoreIsHighest(int groupID, Enum_SongDifficulty diff, int score)
    {
        SongData songData = GetSongDataByGroupID(groupID);
        foreach (SongDifficultyData data in songData.GetSongDifficultyPlayDataList().Values)
        {
            if (diff == data.SongDifficulty)
            {
                return score > data.Score;
            }     
        }
        return false;
    }
    /// <summary>是否刷新連擊數</summary>
    public bool InquireComboIsHighest(int groupID, Enum_SongDifficulty diff, int combo)
    {
        SongData songData = GetSongDataByGroupID(groupID);
        foreach (SongDifficultyData data in songData.GetSongDifficultyPlayDataList().Values)
        {
            if (diff == data.SongDifficulty)
            {
                return combo > data.Combo;
            }
        }
        return false;
    }
    /// <summary>是否刷新評價</summary>
    public bool InquireRankIsHighest(int groupID, Enum_SongDifficulty diff, Enum_SongRank rank)
    {
        SongData songData = GetSongDataByGroupID(groupID);
        foreach (SongDifficultyData data in songData.GetSongDifficultyPlayDataList().Values)
        {
            if (diff == data.SongDifficulty)
            {
                return (int)rank > (int)data.Rank;
            }
        }
        return false;
    }
    #endregion
}
