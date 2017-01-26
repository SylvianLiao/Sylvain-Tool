using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class ThemeData
{
    private int m_iGUID;
    public int GUID
    {
        get { return m_iGUID; }
        set { m_iGUID = value; }
    }
    //清單順序即為顯示順序
    private List<SongData> m_songDataList;

    public ThemeData()
    {
        m_songDataList = new List<SongData>();
        m_iGUID = 0;
    }
    public ThemeData(int themeID)
    {
        m_songDataList = new List<SongData>();

        m_iGUID = themeID;
    }
    //---------------------------------------------------------------------------------
    //Set Data Methods
    public void SetSongDataList(List<SongData> songDataList)
    {
        m_songDataList = songDataList;
    }
    //設定歌曲資料清單 (離線模式用)
    public void SetSongDataListByGameDB(GameDataDB gameDB, RecordSystem recordSys)
    {
        S_Themes_Tmp themeTmp = gameDB.GetGameDB<S_Themes_Tmp>().GetData(m_iGUID);
        List<int> songList = themeTmp.m_iSongsGroupList;
        for (int i = 0, iCount = songList.Count; i < iCount; ++i)
        {
            int groupID = songList[i];
            //直接將DBF的歌曲資料寫入
            if (gameDB.m_songTmpDict.ContainsKey(groupID))
            {
                //解鎖資料於解鎖系統設定
                SongData songData = new SongData();
                songData.BelongThemeID = themeTmp.GUID;
                songData.SongGroupID = groupID;

                List<S_Songs_Tmp> songTmpList = gameDB.m_songTmpDict[groupID];
                songData.IsOriginalSong = songTmpList[0].bIsOriginal;
                foreach (S_Songs_Tmp data in songTmpList)
                {
                    Enum_SongDifficulty diff = (Enum_SongDifficulty)data.iDifficulty;

                    //測試資料
                    Enum_SongRank rank = (Enum_SongRank)recordSys.GetRank(songData, diff);
                    int score = recordSys.GetMaxScore(songData, diff);
                    int combo = recordSys.GetMaxCombo(songData, diff);
                    songData.SetSongDifficultyPlayData(new SongDifficultyData(diff, Enum_DifficultyLockStatus.Lock, score, combo, rank, data.iStar));
                }
                m_songDataList.Add(songData);
            }
        }
    }
    //---------------------------------------------------------------------------------
    //Get Data Methods
    public List<SongData> GetSongDataList()
    {
        return m_songDataList;
    }
}
