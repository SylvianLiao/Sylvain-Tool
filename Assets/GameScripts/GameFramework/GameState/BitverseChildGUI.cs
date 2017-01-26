
using UnityEngine;
using System.Collections.Generic;

    public abstract class BitverseChildGUI// : IChildGUI
    {

    //    /// <summary>
    //    /// 做配合視窗的類型
    //    /// </summary>
    //    protected enum FitType
    //    {
    //        None,
    //        Horizontal,
    //        Vertical,
    //        All
    //    }


    //    private string prefabPath;
    //    protected GameObject guiRoot;
    //    protected BitWindow window;

    //    //lobby裡面的所有子狀態
    //    private Dictionary<string, UiState> subUIStates = new Dictionary<string, UiState>();

    //    //目前的子狀態
    //    protected UiState currentSubUIState;

    //    public BitverseChildGUI(GameObject guiRoot, string prefabPath)
    //    {
    //        this.guiRoot = guiRoot;
    //        this.prefabPath = prefabPath;
    //    }
    //    public virtual void initialize()
    //    {
    //        window = loadGUIPrefab<BitWindow>(prefabPath);
    //        window.Visible = false;
    //    }
    //    public virtual void show()
    //    {
    //        window.Visible = true;
    //    }

    //    public virtual void hide()
    //    {
    //        window.Visible = false;
    //    }
    //    public virtual void update()
    //    {

    //    }

    //    public virtual void onGUI()
    //    {

    //    }

    //    /// <summary>
    //    /// 刪除的動作
    //    /// </summary>
    //    public virtual void destroy()
    //    {
    //        destroyWindow(window);
    //    }

    //    /// <summary>
    //    /// 刪掉視窗
    //    /// </summary>
    //    /// <param name="window"></param>
    //    protected void destroyWindow(BitWindow window)
    //    {
    //        if (window != null)
    //        {
    //            GameObject.Destroy(window.gameObject);
    //            window = null;
    //        }
    //    }

    //    /// <summary>
    //    /// 讀取prefab
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="path"></param>
    //    /// <returns></returns>
    //    protected T loadGUIPrefab<T>(string path) where T : BitContainer
    //    {
    //        GameObject gui = loadPrefab(path);
    //        T window = gui.GetComponent<T>();
    //        BitStage bitStage = guiRoot.GetComponent<BitStage>();
    //        window.Parent = bitStage;
    //        if (window == null)
    //        {
    //            Debug.LogError(gui + " is not a Windows Prefab!");
    //        }

    //        return window;
    //    }

    //    private GameObject loadPrefab(string path)
    //    {
    //        UnityEngine.Object prefab = Resources.Load(path);
    //        if (prefab == null)
    //        {
    //            Debug.LogError("Can not find " + path);
    //            return null;
    //        }

    //        GameObject gui = UnityEngine.Object.Instantiate(prefab) as GameObject;
    //        if (gui == null)
    //        {
    //            Debug.LogError(path + " prefab is not GameObject");
    //            return null;
    //        }

    //        if (guiRoot == null)
    //        {
    //            Debug.LogError(" UIState UIRoot is not found!");
    //            return null;
    //        }
    //        return gui;
    //    }

    //    /// <summary>
    //    /// 設定配合視窗縮放
    //    /// </summary>
    //    /// <param name="fitType"></param>
    //    protected void setFitSize(FitType fitType)
    //    {
    //        float width = window.Position.width;
    //        float height = window.Position.height;
    //        if (fitType == FitType.Horizontal && fitType == FitType.All)
    //        {
    //            width = Screen.width;
    //        }
    //        if (fitType == FitType.Horizontal && fitType == FitType.All)
    //        {
    //            height = Screen.height;
    //        }

    //        window.Position = new Rect(window.Position.x, window.Position.y, width, height);
    //    }

    //    /// <summary>
    //    /// 鎖住視窗
    //    /// </summary>
    //    /// <param name="islockWin"></param>
    //    public void lockGUI(bool islockWin)
    //    {
    //        if (window != null)
    //        {
    //            if (islockWin)
    //            {
    //                window.Disabled = true;
    //                window.MouseEnabled = false;
    //            }
    //            else
    //            {
    //                window.Disabled = false;
    //                window.Enabled = true;
    //                window.MouseEnabled = true;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 加入子state
    //    /// </summary>
    //    /// <param name="uiState"></param>
    //    protected void addSubUiState(UiState uiState)
    //    {
    //        if (subUIStates.ContainsKey(uiState.name))
    //        {
    //            Debug.LogWarning("lobbySubUIStates has the same id = " + uiState.name);
    //        }
    //        else
    //        {
    //            subUIStates.Add(uiState.name, uiState);
    //            uiState.initialize();
    //        }
    //    }

    //    /// <summary>
    //    /// 根據名稱取得子State
    //    /// </summary>
    //    /// <param name="uiStateName"></param>
    //    /// <returns></returns>
    //    protected UiState getSubUiState(string uiStateName)
    //    {
    //        if (subUIStates.ContainsKey(uiStateName))
    //        {
    //            return subUIStates[uiStateName];
    //        }
    //        return null;
    //    }
    //    /// <summary>
    //    /// 切換子state
    //    /// </summary>
    //    /// <param name="id"></param>
    //    public void switchSubState(string key)
    //    {
    //        if (currentSubUIState != null)
    //        {
    //            currentSubUIState.end();
    //        }
    //        if (subUIStates.ContainsKey(key))
    //        {
    //            currentSubUIState = subUIStates[key];
    //            currentSubUIState.begin();
    //        }
    //    }
    }
