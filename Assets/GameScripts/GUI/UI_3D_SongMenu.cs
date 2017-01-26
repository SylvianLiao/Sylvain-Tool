using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using DG.Tweening;
using Softstar;

public class UI_3D_SongMenu : UI3DChildGUI
{
    public List<Slot_Song>              m_songObjectList;
    private Dictionary<int, Vector3>    m_songLocPosMap;
    private Dictionary<int, Vector3>    m_songLocRotateMap;

    [Header("Swipe Option")]
    public bool ShowTouchPosition;
    public float m_ResetTime;
    public float m_SwipeSpeed;
    public float m_WaitPlaySongTime;

    [HideInInspector] public int m_firstSongIndex;
    [HideInInspector] public int m_lastSongIndex;
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        m_songLocPosMap = new Dictionary<int, Vector3>();
        m_songLocRotateMap = new Dictionary<int, Vector3>();
        InitSongList();
    }
    //-------------------------------------------------------------------------------------------------
    private void InitSongList()
    {
        for (int i = 0, iCount = m_songObjectList.Count; i < iCount; ++i)
        {
            int index = i - (m_songObjectList.Count / 2);
            //Init Transform Data
            m_songLocPosMap.Add(index, m_songObjectList[i].transform.localPosition);
            m_songLocRotateMap.Add(index, m_songObjectList[i].transform.localEulerAngles);

            if (i == 0)
                m_firstSongIndex = index;
            else if (i == iCount-1)
                m_lastSongIndex = index;
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void SetSongListUI(ResourceManager rm, SwipeController controller, PlayerDataSystem datsSystem, SongData[] songDataArray)
    {
        for (int i = 0, iCount = m_songObjectList.Count; i < iCount; ++i)
        {
            m_songObjectList[i].gameObject.SetActive(false);
            int index = i - (m_songObjectList.Count / 2);
            //Init Data
            m_songObjectList[i].Initialize(this, controller, index, songDataArray[i].SongGroupID);
            //Init Material
            SongData songData = songDataArray[i];
            m_songObjectList[i].UpdateData(rm, datsSystem, songData);
            //Init transform
            m_songObjectList[i].transform.localPosition = m_songLocPosMap[index];
            m_songObjectList[i].transform.localEulerAngles = m_songLocRotateMap[index];
        }
    }
    #region Swipe Functions
    //-------------------------------------------------------------------------------------------------
    /// <summary>通知物件移動，除了發出移動通知的物件外</summary>
    public void NotifyMoving(Slot_Song callerSong, float ratio, bool isLeft)
    {
        foreach (var song in m_songObjectList)
        {
            if (song.Equals(callerSong))
            {
                continue;
            }
            song.NotCatchedMoving(ratio, isLeft);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>通知移動結束</summary>
    public void NotifyMovedEnd(Slot_Song callerSong, Enum_ClosedPos endPos)
    {
        foreach (var song in m_songObjectList)
        {
            if (song.Equals(callerSong))
            {
                continue;
            }
            song.NotCatchedMovedEnd(endPos);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得物件左邊的物件位置 </summary>
    public bool GetNextLeftPos(int index, out Vector3 pos)
    {
        int nextIdx = index - 1;

        bool canGet = m_songLocPosMap.TryGetValue(nextIdx, out pos);
        if (!canGet)
        {
            nextIdx = index * -1;
            canGet = m_songLocPosMap.TryGetValue(nextIdx, out pos);
        }
        return canGet;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得物件左邊的物件旋轉量 </summary>
    public bool GetNextLeftRotate(int index, out Vector3 pos)
    {
        int nextIdx = index - 1;

        bool canGet = m_songLocRotateMap.TryGetValue(nextIdx, out pos);
        if (!canGet)
        {
            nextIdx = index * -1;
            canGet = m_songLocRotateMap.TryGetValue(nextIdx, out pos);
        }
        return canGet;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得物件左邊的物件索引 </summary>
    public int GetNextLeftIndex(int index)
    {
        int nextIdx = index - 1;
        if (!m_songLocPosMap.ContainsKey(nextIdx))
        {
            nextIdx = index * -1;
        }
        return nextIdx;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得物件右邊的物件位置 </summary>
    public bool GetNextRightPos(int index, out Vector3 pos)
    {
        int nextIdx = index + 1;

        bool canGet = m_songLocPosMap.TryGetValue(nextIdx, out pos);
        if (!canGet)
        {
            nextIdx = index * -1;
            canGet = m_songLocPosMap.TryGetValue(nextIdx, out pos);
        }
        return canGet;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得物件右邊的物件旋轉量 </summary>
    public bool GetNextRightRotate(int index, out Vector3 pos)
    {
        int nextIdx = index + 1;

        bool canGet = m_songLocRotateMap.TryGetValue(nextIdx, out pos);
        if (!canGet)
        {
            nextIdx = index * -1;
            canGet = m_songLocRotateMap.TryGetValue(nextIdx, out pos);
        }
        return canGet;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>取得物件右邊的物件索引 </summary>
    public int GetNextRightIndex(int index)
    {
        int nextIdx = index + 1;
        if (!m_songLocPosMap.ContainsKey(nextIdx))
        {
            nextIdx = index * -1;
        }
        return nextIdx;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>根據索引取得物件位置 </summary>
    public bool GetSpecifiedIndexPos(int index, out Vector3 pos)
    { 
        return m_songLocPosMap.TryGetValue(index, out pos);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>根據索引取得物件旋轉量 </summary>
    public bool GetSpecifiedIndexRotate(int index, out Vector3 pos)
    {
        return m_songLocRotateMap.TryGetValue(index, out pos);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>根據索引取得歌曲GroupID </summary>
    public bool GetSpecifiedSlotSong(int index, out Slot_Song song)
    {
        song = null;
        foreach (Slot_Song obj in m_songObjectList)
        {
            if (index == obj.m_iCurrentIndex)
            {
                song = obj;
                return true;
            }
        }
        return false;
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    /// <summary>更新歌曲選單資料</summary>
    public void UpdateSongListData(PlayerDataSystem dataSystem, ResourceManager rm)
    {
        foreach (Slot_Song obj in m_songObjectList)
        {
            obj.UpdateDataAfterCathed(dataSystem, rm);
        }
    }
}
