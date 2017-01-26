using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;

namespace Softstar
{
    public class GUIManager
    {
        private Dictionary<string, NGUIChildGUI> m_GUIList = new Dictionary<string, NGUIChildGUI>();

        private MonoBehaviour m_mono;
        private ResourceManager m_ResourceManager;
        public UICamera m_uiCamera;

        //Common UI
        public UI_Fade m_uiFade;
        public UI_PlayerInfo m_uiPlayerInfo;
        public UI_TopBar m_uiTopBar;
        public UI_CheckBox m_uiCheckBox;
        public UI_Loading m_uiLoading;
        public UI_Development m_uiDevelopment;

        public GUIManager(MainApplication mainApp)
        {
            m_uiCamera = GameObject.Find("UI Root/Camera(UI)").GetComponent<UICamera>(); ;
            m_mono = mainApp.MusicApp;
            m_ResourceManager = mainApp.GetResourceManager();
            CreateCommonUI(mainApp);
        }
        //-----------------------------------------------------------------------------------------------------------
        public void Initialize()
        {
            foreach (NGUIChildGUI gui in m_GUIList.Values)
            {
                if (gui.IsInitialize() == false)
                {
                    gui.Initialize();
                }
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        public T AddGUI<T>(string guiName) where T : NGUIChildGUI
        {
            if (m_GUIList.ContainsKey(guiName))
            {
                UnityDebugger.Debugger.Log("GUI: [" + guiName + "] Already Exist!");
                return m_GUIList[guiName] as T;
            }

            GameObject go = m_ResourceManager.GetResourceSync(Enum_ResourcesType.GUI, guiName);
            T gui = NGUITools.AddChild(m_uiCamera.gameObject, go).GetComponent<T>();
            if (gui != null && gui is NChildGUI)
            {
                gui.SetUIName(guiName);
                m_GUIList.Add(guiName, gui);
            }
            else
            {
                UnityDebugger.Debugger.Log("GUI Instantiate Failed! GUI = [" + gui + "]");
                return default(T);
            }

            gui.Hide();

            return gui;
        }
        //-----------------------------------------------------------------------------------------------------------
        public AsyncLoadOperation AddGUIAsync(string guiName, System.Type type)
        {
            AsyncLoadOperation operater;
            if (m_GUIList.ContainsKey(guiName))
            {
                UnityDebugger.Debugger.Log("GUI: [" + guiName + "] Already Exist!");
                operater = new AsyncLoadOperation(guiName, type);
                operater.m_assetObject = m_GUIList[guiName];
                operater.m_bIsDone = true;
                return operater;
            }

            operater = m_ResourceManager.GetResourceASync(Enum_ResourcesType.GUI, guiName, type, OnLoadGUIFinish);

            return operater;
        }
        //-----------------------------------------------------------------------------------------------------------
        public void OnLoadGUIFinish(AsyncLoadOperation load)
        {
            if (load.m_assetObject == null)
            {
                UnityDebugger.Debugger.Log("GUI Instantiate Failed!");
                return;
            }
            GameObject go = load.m_assetObject as GameObject;
            NGUIChildGUI gui = NGUITools.AddChild(m_uiCamera.gameObject, go).GetComponent<NGUIChildGUI>();

            //非同步物件讀取成功後要等待其他UI淡出，故先不顯示
            gui.Hide();
            gui.SetUIName(load.m_Type.Name);
            m_GUIList.Add(load.m_Type.Name, gui);
        }
        //-----------------------------------------------------------------------------------------------------------
        public NGUIChildGUI GetGUI(string guiName)
        {
            if (m_GUIList.ContainsKey(guiName) == false)
            {
                UnityDebugger.Debugger.Log("Get GUI Failed! GUI[" + guiName + "] Doesnt Exist!");
                return null;
            }

            NGUIChildGUI gui = m_GUIList[guiName];
            if (gui.IsInitialize() == false)
                gui.Initialize();

            return gui;
        }
        //-----------------------------------------------------------------------------------------------------------
        public bool DeleteGUI(string guiName)
        {
            if (m_GUIList.ContainsKey(guiName))
            {
                NGUIChildGUI gui = m_GUIList[guiName];
                gui.GUIDestroy();
                m_GUIList.Remove(guiName);
                return true;
            }
            else
            {
                UnityDebugger.Debugger.Log("Delete GUI Failed! GUI[" + guiName + "] Doesnt Exsit !");
                return false;
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        public bool FadeIn(string guiName)
        {
            if (m_GUIList.ContainsKey(guiName) == false)
            {
                UnityDebugger.Debugger.Log("FadeOutGUI GUI Failed! GUI[" + guiName + "] Doesnt Exsit !");
                return false;
            }

            NGUIChildGUI gui = m_GUIList[guiName];
            gui.FadeIn();

            return true;
        }
        //-----------------------------------------------------------------------------------------------------------
        public bool FadeOut(string guiName,bool isDestroy = false)
        {
            if (m_GUIList.ContainsKey(guiName) == false)
            {
                UnityDebugger.Debugger.Log("FadeOutGUI GUI Failed! GUI[" + guiName + "] Doesnt Exsit !");
                return false;
            }

            NGUIChildGUI gui = m_GUIList[guiName];
            if (isDestroy)
            {
                gui.OnFadeOutFinish.Add(gui.GUIDestroy);
                m_GUIList.Remove(guiName);
            }
            gui.FadeOut();
            return true;
        }
        //-----------------------------------------------------------------------------------------------------------
        public void DeleteAllGUI()
        {
            List<string> delList = new List<string>();
            foreach (string guiName in m_GUIList.Keys)
            {
                delList.Add(guiName);                
            }

            for (int i = 0; i < delList.Count; i++)
            {
                DeleteGUI(delList[i]);
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        public void Update()
        {
            foreach (NChildGUI gui in m_GUIList.Values)
            {
                gui.UiUpdate();
            }
        }
        //-------------------------------------------------------------------------------
        public void ShowGUI(string guiName)
        {
            if (m_GUIList.ContainsKey(guiName))
            {
                NChildGUI gui = m_GUIList[guiName];
                if (gui.IsInitialize() == false)
                    gui.Initialize();
                gui.Show();
            }
        }
        //-------------------------------------------------------------------------------
        public void HideGUI(string guiName)
        {
            if (m_GUIList.ContainsKey(guiName))
            {
                NChildGUI gui = m_GUIList[guiName];
                gui.Hide();
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        //生成常駐型UI
        public void CreateCommonUI(MainApplication mainApp)
        {
            m_uiPlayerInfo = AddGUI<UI_PlayerInfo>(typeof(UI_PlayerInfo).Name);
            m_uiTopBar = AddGUI<UI_TopBar>(typeof(UI_TopBar).Name);
            m_uiFade = AddGUI<UI_Fade>(typeof(UI_Fade).Name);
            m_uiCheckBox = AddGUI<UI_CheckBox>(typeof(UI_CheckBox).Name);
            m_uiLoading = AddGUI<UI_Loading>(typeof(UI_Loading).Name);
#if DEVELOP
            m_uiDevelopment = AddGUI<UI_Development>(typeof(UI_Development).Name);
            m_uiDevelopment.m_mainApp = mainApp;
#endif
            m_uiPlayerInfo.SetMainApp(mainApp);
            m_uiTopBar.SetMainApp(mainApp);

            Initialize();
        }
        //-----------------------------------------------------------------------------------------------------------
        public IEnumerator ScreenShot()
        {
            yield return m_mono.StartCoroutine(m_uiFade.ScreenShot());
        }
        //-----------------------------------------------------------------------------------------------------------
        public void FadeOutScreenShot ()
        {
            m_mono.StartCoroutine(m_uiFade.FadeOutScreenShot());
        }
        //-----------------------------------------------------------------------------------------------------------
        public void OnFadeOutScreenShot(List<EventDelegate.Callback> callback)
        {
            if (callback == null)
                return;

            m_uiFade.OnFadeOutFinish.AddRange(callback);
        }
        //-----------------------------------------------------------------------------------------------------------
        public void OnFadeOutScreenShot(EventDelegate.Callback callback)
        {
            if (callback == null)
                return;

            m_uiFade.OnFadeOutFinish.Add(callback);
        }
        //-----------------------------------------------------------------------------------------------------------
        public void ShowLoadingUI(bool isShow)
        {
            if (isShow)
                m_uiLoading.Show();
            else
                m_uiLoading.Hide();
        }
    }
}
