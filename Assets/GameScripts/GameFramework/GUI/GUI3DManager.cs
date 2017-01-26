using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;

namespace Softstar
{
    public class GUI3DManager
    {
        private Dictionary<string, UI3DChildGUI> m_3DUIList = new Dictionary<string, UI3DChildGUI>();

        private MonoBehaviour m_mono;
        private ResourceManager m_resourceManager;
        public GameObject m_uiCameraGO;

        public GUI3DManager(MainApplication mainApp)
        {
            m_uiCameraGO = GameObject.Find("MusicApplication/Camera(3DUI)");
            m_mono = mainApp.MusicApp;
            m_resourceManager = mainApp.GetResourceManager();
        }
        //-----------------------------------------------------------------------------------------------------------
        public void Initialize()
        {
            foreach (UI3DChildGUI gui in m_3DUIList.Values)
            {
                if (gui.IsInitialize() == false)
                    gui.Initialize();
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        public T AddGUI<T>(string guiPath) where T : UI3DChildGUI
        {
            if (m_3DUIList.ContainsKey(typeof(T).Name))
            {
                UnityDebugger.Debugger.Log("GUI: [" + typeof(T).Name + "] Already Exist!");
                return m_3DUIList[typeof(T).Name] as T;
            }

            GameObject go = m_resourceManager.GetResourceSync(Enum_ResourcesType.GUI, guiPath);
            T gui = GameObject.Instantiate(go).GetComponent<T>();
            if (gui != null && gui is NChildGUI)
            {
                gui.SetUIName(typeof(T).Name);
                m_3DUIList.Add(typeof(T).Name, gui);
            }
            else
            {
                UnityDebugger.Debugger.Log("3DGUI Instantiate Failed! GUI = [" + gui + "]");
                return default(T);
            }

            gui.Hide();

            return gui;
        }
        //-----------------------------------------------------------------------------------------------------------
        public AsyncLoadOperation AddGUIAsync(string guiPath, System.Type type)
        {
            string guiName = type.Name;

            AsyncLoadOperation operater;
            if (m_3DUIList.ContainsKey(guiName))
            {
                UnityDebugger.Debugger.Log("GUI: [" + guiName + "] Already Exist!");
                operater = new AsyncLoadOperation(guiName, type);
                operater.m_assetObject = m_3DUIList[guiName];
                operater.m_bIsDone = true;
                return operater;
            }

            operater = m_resourceManager.GetResourceASync(Enum_ResourcesType.GUI, guiPath, type, OnLoadGUIFinish);

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
            UI3DChildGUI gui = GameObject.Instantiate(go).GetComponent<UI3DChildGUI>();

            //非同步物件讀取成功後要等待其他UI淡出，故先不顯示
            gui.Hide();
            gui.SetUIName(load.m_Type.Name);
            m_3DUIList.Add(load.m_Type.Name, gui);
        }
        //-----------------------------------------------------------------------------------------------------------
        public UI3DChildGUI GetGUI(string guiName)
        {
            if (m_3DUIList.ContainsKey(guiName) == false)
            {
                UnityDebugger.Debugger.Log("Get GUI Failed! GUI[" + guiName + "] Doesnt Exist!");
                return null;
            }

            UI3DChildGUI gui = m_3DUIList[guiName];
            if (gui.IsInitialize() == false)
                gui.Initialize();

            return gui;
        }
        //-----------------------------------------------------------------------------------------------------------
        public bool DeleteGUI(string guiName)
        {
            if (m_3DUIList.ContainsKey(guiName))
            {
                UI3DChildGUI gui = m_3DUIList[guiName];
                gui.GUIDestroy();
                m_3DUIList.Remove(guiName);
                return true;
            }
            else
            {
                UnityDebugger.Debugger.Log("Delete GUI Failed! GUI[" + guiName + "] Doesnt Exsit !");
                return false;
            }
        }  
        //-----------------------------------------------------------------------------------------------------------
        public void DeleteAllGUI()
        {
            List<string> delList = new List<string>();            
            foreach (string guiName in m_3DUIList.Keys)
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
            foreach (NChildGUI gui in m_3DUIList.Values)
            {
                gui.UiUpdate();
            }
        }
        //-------------------------------------------------------------------------------
        public void ShowGUI(string guiName)
        {
            if (m_3DUIList.ContainsKey(guiName))
            {
                NChildGUI gui = m_3DUIList[guiName];
                if (gui.IsInitialize() == false)
                    gui.Initialize();
                gui.Show();
            }
        }
        //-------------------------------------------------------------------------------
        public void HideGUI(string guiName)
        {
            if (m_3DUIList.ContainsKey(guiName))
            {
                NChildGUI gui = m_3DUIList[guiName];
                gui.Hide();
            }
        }    
    }
}
