using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;

public class SwipeController
{
    private SoundSystem m_SoundSystem;
    public delegate void Action<T>(T obj);
    //世界座標每移動一單位距離，滑鼠要移動多少
    private float m_WorldUnitPerMouseMove = 0;
    //通知物件被抓取
    private Dictionary<string, Action<bool>> m_notifyCatched;
    //通知物件移動
    private Dictionary<string, Action<Vector3>> m_notifyMoving;
    //Data
    private float m_fSwipeBound = 5.0f;
    private Vector2 m_startPosition;
    private string m_LastCatchedObjName;
    public bool m_isSongSwiping;

    //Main Song Object
    private Slot_Song m_MainSong;

    //SwipeZone
    private List<UIButton> m_SwipeZone;
    //---------------------------------------------------------------------------------------------------
    public SwipeController(SoundSystem soundSystem, List<UIButton> swipeZone)
    {
        m_SoundSystem = soundSystem;
        m_notifyCatched = new Dictionary<string, Action<bool>>();
        m_notifyMoving = new Dictionary<string, Action<Vector3>>();
        m_isSongSwiping = false;
        m_SwipeZone = swipeZone;
    }
    //---------------------------------------------------------------------------------------------------
    public void OnSwipeStart(Gesture gesture)
    {
        //檢查滑移偵測範圍
        GameObject target = gesture.GetCurrentPickedObject();
        if (!CheckInSwipeZone(target))
            return;

        if (m_isSongSwiping)
            return;

        //計算單位位移量
        if (m_WorldUnitPerMouseMove == 0)
        {
            Vector3 worldObj2SP = Camera.main.WorldToScreenPoint(m_MainSong.transform.position);
            Vector3 addWorldObj2SP = Camera.main.WorldToScreenPoint(m_MainSong.transform.position + new Vector3(1f, 0, 0));
            m_WorldUnitPerMouseMove = Mathf.Abs(addWorldObj2SP.x - worldObj2SP.x);
        }

        Action<bool> notifyCatched;
        if (m_notifyCatched.TryGetValue(m_MainSong.name, out notifyCatched))
        {
            //通知上一個抓取事件結束
            if (!string.IsNullOrEmpty(m_LastCatchedObjName))
                m_notifyCatched[m_LastCatchedObjName](false);

            m_LastCatchedObjName = m_MainSong.name;
            //通知當前被抓取
            notifyCatched(true);
        }
    }
    //---------------------------------------------------------------------------------------------------
    public void OnSwipe(Gesture gesture)
    {
        //設定小範圍內不滑移，避免與點擊偵測搞混
        Vector3 tmpVec = gesture.position - gesture.startPosition;
        if (Mathf.Abs(tmpVec.x) <= m_fSwipeBound)
            return;

        if (string.IsNullOrEmpty(m_LastCatchedObjName))
            return;

        if (m_startPosition == Vector2.zero)
            m_startPosition = gesture.position;

        m_isSongSwiping = true;

        //只會通知被抓取的物件
        Action<Vector3> notifyMoving;
        if (m_notifyMoving.TryGetValue(m_LastCatchedObjName, out notifyMoving))
        {
            tmpVec = gesture.position - m_startPosition;           
            Vector3 moveX = new Vector3(tmpVec.x / m_WorldUnitPerMouseMove, 0, 0);
            //UnityDebugger.Debugger.Log("2D Move Distance = " + tmpVec.x+ ", 3D Move Distance = " + moveX);
            notifyMoving(moveX);
        }
    }
    //---------------------------------------------------------------------------------------------------
    public void OnSwipeEnd(Gesture gesture)
    {
        if (string.IsNullOrEmpty(m_LastCatchedObjName))
            return;

        m_startPosition = Vector2.zero;
        //通知抓取結束
        m_notifyCatched[m_LastCatchedObjName](false);

        //UnityDebugger.Debugger.Log("Swipe End--------------");
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>設定中央歌曲</summary>
    public void SetMainSong(Slot_Song song)
    {
        m_MainSong = song;
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>檢查偵測滑移區域</summary>
    public bool CheckInSwipeZone(GameObject target)
    {
        foreach (var go in m_SwipeZone)
        {
            if (target.Equals(go.gameObject))
                return true;
        }
        return false;
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>註冊通知抓取 </summary>
    public void RegisterNotifyCatched(string objName, Action<bool> callback)
    {
        if (!m_notifyCatched.ContainsKey(objName))
        {
            m_notifyCatched.Add(objName, callback);
        }
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>註冊通知移動</summary>
    public void RegisterNotifyMoving(string objName, Action<Vector3> callback)
    {
        if (!m_notifyMoving.ContainsKey(objName))
        {
            m_notifyMoving.Add(objName, callback);
        }
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>釋放資料</summary>
    public void ReleaseData()
    {
        m_notifyMoving.Clear();
        m_notifyMoving = null;
        m_notifyCatched.Clear();
        m_notifyCatched = null;

        m_LastCatchedObjName = null;
        m_isSongSwiping = false;
    }
}
