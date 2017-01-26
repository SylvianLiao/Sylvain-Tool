using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class NewState : CustomBehaviorState
{
    private UI_New m_uiNew;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;

    public NewState(GameScripts.GameFramework.GameApplication app) : base(StateName.THEME_STATE, StateName.THEME_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("NewState begin");

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_New));

        base.begin();

        if (m_bIsAync == false)
        {
            m_uiNew = m_guiManager.AddGUI<UI_New>(typeof(UI_New).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        m_uiNew = m_guiManager.GetGUI(typeof(UI_New).Name) as UI_New;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }

    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        m_uiNew = null;

        m_guiManager.DeleteGUI(typeof(UI_New).Name);
        base.end();
        UnityDebugger.Debugger.Log("NewState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        RemoveCallBack();
        base.suspend();
        UnityDebugger.Debugger.Log("NewState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        AddCallBack();
        base.resume();
        UnityDebugger.Debugger.Log("NewState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {

    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {

    }
}

