using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Softstar;

/// <summary>
/// 處理State中自定義的共同狀況
/// </summary>
public abstract class CustomBehaviorState : SceneState
{
    protected MainApplication m_mainApp;
    protected GUIManager m_guiManager;
    protected GUI3DManager m_gui3DManager;

    protected UI_PlayerInfo m_uiPlayerInfo;
    protected UI_TopBar m_uiTopBar;

    private Dictionary<System.Type, bool> m_guiType; //UI名稱, 是否為3DUI

    protected bool m_bIsAync;
    protected bool m_bUseLoadingUI;
    protected bool m_bIsSuspendByFullScreenState;  //用於指定下一個狀態是全螢幕UI與否

    //Async Load Operator
    protected List<AsyncLoadOperation> m_guiLoadOperator; //非同步讀取UI用的Loader

    //設定於狀態初始化結束 & UI顯示後的監聽事件
    public EventDelegate.Callback OnUIShowFinish;

    public CustomBehaviorState(string name, string sceneName, GameScripts.GameFramework.GameApplication gameApplication)
        : base(name, sceneName, gameApplication)
    {
        m_mainApp = gameApplication as MainApplication;
        m_guiManager = m_mainApp.GetGUIManager();
        m_gui3DManager = m_mainApp.GetGUI3DManager();

        m_guiType = new Dictionary<System.Type, bool>();
        m_guiLoadOperator = new List<AsyncLoadOperation>();
        m_bIsSuspendByFullScreenState = false;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        if (userData == null)
            userData = new Hashtable();

        //Get Common UI
        m_uiPlayerInfo = m_guiManager.m_uiPlayerInfo;
        m_uiTopBar = m_guiManager.m_uiTopBar;

        if (m_guiType.Count == 0)
        {
            UnityDebugger.Debugger.Log("UIState doesn't assign GUI Name!!");
            return;
        }

        //Check load GUI by Async or Sync
        m_bIsAync = userData.ContainsKey(Enum_StateParam.LoadGUIAsync);
        if (m_bIsAync)
        {
            if ((bool)userData[Enum_StateParam.LoadGUIAsync])
                m_bUseLoadingUI = true;
            foreach (KeyValuePair<System.Type, bool> data in m_guiType)
            {
                string guiPath = GetGUIPath(data.Key, m_mainApp.GetSystem<PlayerDataSystem>());
                AsyncLoadOperation load = (data.Value)
                    ? m_gui3DManager.AddGUIAsync(guiPath, data.Key)
                    : m_guiManager.AddGUIAsync(guiPath, data.Key);
                m_guiLoadOperator.Add(load);
            }
            m_mainApp.MusicApp.StartCoroutine(StateInitAsync());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected virtual IEnumerator StateInitAsync()
    {
        if (m_bUseLoadingUI)
            m_guiManager.ShowLoadingUI(true);

        foreach (var op in m_guiLoadOperator)
        {
            yield return m_mainApp.MusicApp.StartCoroutine(op);

            GetGUIAsync(op);
        }

        if (m_bUseLoadingUI)
            m_guiManager.ShowLoadingUI(false);

        yield return m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
    }
    //---------------------------------------------------------------------------------------------------
    protected virtual void GetGUIAsync(AsyncLoadOperation operater)
    {

    }
    //---------------------------------------------------------------------------------------------------
    protected virtual void StateInit()
    {
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>處理UI的顯示與否，包含共同UI、淡入淡出用UI、透過SetGUIType設定的UI </summary>
    public virtual IEnumerator CheckAndActiveGUI(EventDelegate.Callback callback)
    {
        CheckActiveCommonGUI();
        ShowBeSetGUI();

        if (userData.ContainsKey(Enum_StateParam.ChangeByScreenShot))
        {
            yield return null;

            m_guiManager.OnFadeOutScreenShot(callback);
            if (OnUIShowFinish != null)
                m_guiManager.OnFadeOutScreenShot(OnUIShowFinish);
            m_guiManager.FadeOutScreenShot();
        }
        else
        {
            if (callback != null)
                callback();
            if (OnUIShowFinish != null)
                OnUIShowFinish();
        }
    }
    //---------------------------------------------------------------------------------------------------
    public override void update()
    {

    }
    //---------------------------------------------------------------------------------------------------
    public override void onGUI()
    {

    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        if (m_bIsAync)
        {
            for (int i = 0, iCount = m_guiLoadOperator.Count; i < iCount; ++i)
            {
                m_guiLoadOperator[i].CancelLoad();
                m_guiLoadOperator[i] = null;
            }
            m_guiLoadOperator.Clear();
        }

        m_guiType.Clear();

        userData.Clear();
    }
    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        if (userData != null && userData.ContainsKey(Enum_StateParam.ChangeByScreenShot))
            userData.Remove(Enum_StateParam.ChangeByScreenShot);
    }
    //---------------------------------------------------------------------------------------------------
    public override void resume() { }
    //---------------------------------------------------------------------------------------------------
    public virtual void ApplicationResume() { }
    //---------------------------------------------------------------------------------------------------
    public virtual void ApplicationPause() { }
    //---------------------------------------------------------------------------------------------------
    public virtual void AddCallBack() { }
    //---------------------------------------------------------------------------------------------------
    public virtual void RemoveCallBack() { }
    //---------------------------------------------------------------------------------------------------
    /// <summary>Set the name of GUI witch is this state want to use.</summary>
    /// <param name="type"></param>
    /// <param name="is3DUI"></param>
    protected void SetGUIType(System.Type type, bool is3DUI = false)
    {
        m_guiType.Add(type, is3DUI);
    }
    //---------------------------------------------------------------------------------------------------
    protected void RemoveGUIType(System.Type type)
    {
        if (m_guiType.ContainsKey(type))
            m_guiType.Remove(type);
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>顯示被設定於GUI TYPE容器中的UI</summary>
    protected void ShowBeSetGUI()
    {
        foreach (KeyValuePair<System.Type, bool> data in m_guiType)
        {
            if (data.Value) m_gui3DManager.GetGUI(data.Key.Name).Show();
            else m_guiManager.GetGUI(data.Key.Name).Show();
        }
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>隱藏被設定於GUI TYPE容器中的UI</summary>
    protected void HideBeSetGUI()
    {
        foreach (KeyValuePair<System.Type, bool> data in m_guiType)
        {
            if (data.Value) m_gui3DManager.GetGUI(data.Key.Name).Hide();
            else m_guiManager.GetGUI(data.Key.Name).Hide();
        }
    }
    //---------------------------------------------------------------------------------------------------
    public void CheckActiveCommonGUI()
    {
        if (userData.ContainsKey(Enum_StateParam.UseTopBarUI) == false) m_uiTopBar.Hide();
        else m_uiTopBar.Show();
        if (userData.ContainsKey(Enum_StateParam.UsePlayerInfoUI) == false) m_uiPlayerInfo.Hide();
        else m_uiPlayerInfo.Show();
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>設定State Suspend時是否隱藏UI</summary>
    public void SetIsSuspendByFullScreenState()
    {
        m_bIsSuspendByFullScreenState = true;
    }
    //-------------------------------------------------------------------------------
    /// <summary>取得不在GUI資料夾第一層的UI路徑</summary>
    protected string GetGUIPath(System.Type type, PlayerDataSystem dataSystem)
    {
        string guiPath = type.Name;
        SongData songData = dataSystem.GetCurrentSongData();
        if (type.Equals(typeof(UI_3D_BattleBG)))
        {
            guiPath = dataSystem.GetSongBattleBgResourcePath(songData);
        }
        else if (type.Equals(typeof(UI_3D_Opening)))
        {
            guiPath = dataSystem.GetThemeBgPrefabName(songData.BelongThemeID);
        }
        return guiPath;
    }
    //---------------------------------------------------------------------------------------------------
    private void DelayDeleteGUI()
    {
        if (userData.ContainsKey(Enum_StateParam.DelayDeleteGUIName))
        {
            string[] deleteGUIName = userData[Enum_StateParam.DelayDeleteGUIName] as string[];
            foreach (var name in deleteGUIName)
            {
                if (!m_guiManager.DeleteGUI(name))
                    m_gui3DManager.DeleteGUI(name);
            }
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected IEnumerator CheckScreenShotBeforeInit()
    {
        if (userData.ContainsKey(Enum_StateParam.ChangeByScreenShot))
        {
            yield return m_mainApp.MusicApp.StartCoroutine(m_guiManager.ScreenShot());
        }

        DelayDeleteGUI();

        StateInit();
    }
}


