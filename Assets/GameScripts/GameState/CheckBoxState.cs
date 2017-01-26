using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class CheckBoxState : CustomBehaviorState
{
    private UI_CheckBox m_uiCheckBox;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;

    public delegate void CheckBoxEvent();
    public CheckBoxEvent OnCheckBoxOKClick;
    public CheckBoxEvent OnCheckBoxCancelClick;

    public delegate void CheckBoxEvent_Param(System.Object obj);
    public CheckBoxEvent_Param OnCheckBoxOKClick_Param;
    public CheckBoxEvent_Param OnCheckBoxCancelClick_Param;

    private bool m_bIsAutoPop;

    public CheckBoxState(GameScripts.GameFramework.GameApplication app) : base(StateName.CHECK_BOX_STATE, StateName.CHECK_BOX_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_bIsAutoPop = true;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("CheckBoxState begin");

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_CheckBox));

        base.begin();

        if (m_bIsAync == false)
        {
            m_uiCheckBox = m_guiManager.GetGUI(typeof(UI_CheckBox).Name) as UI_CheckBox;
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        m_uiCheckBox = m_guiManager.GetGUI(typeof(UI_CheckBox).Name) as UI_CheckBox;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        int iconID = (userData.ContainsKey(GameDefine.CHECK_BOX_ICON_ID_KEY)) ? (int)userData[GameDefine.CHECK_BOX_ICON_ID_KEY] :0;
        m_uiCheckBox.InitializeUI(userData, iconID);

        //Custom delegate
        OnCheckBoxOKClick = (CheckBoxEvent)userData[GameDefine.CHECK_BOX_OK_EVENT_KEY];
        OnCheckBoxOKClick_Param = (CheckBoxEvent_Param)userData[GameDefine.CHECK_BOX_OK_EVENT_PARAM_KEY];
        OnCheckBoxCancelClick = (CheckBoxEvent)userData[GameDefine.CHECK_BOX_CANCEL_EVENT_KEY];
        OnCheckBoxCancelClick_Param = (CheckBoxEvent_Param)userData[GameDefine.CHECK_BOX_CANCEL_EVENT_PARAM_KEY];

        m_bIsAutoPop = (bool)userData[GameDefine.CHECK_BOX_IS_AUTO_POP_KEY];

        m_uiCheckBox.StopFade();
       
        m_uiCheckBox.OnFadeInFinish.Add(AddCallBack);
        m_guiManager.FadeIn(typeof(UI_CheckBox).Name);
    }

    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        RemoveCallBack();

        m_uiCheckBox.FadeOut();
        m_uiCheckBox = null;

        m_bIsAutoPop = true;
        base.end();
        UnityDebugger.Debugger.Log("CheckBoxState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        RemoveCallBack();
        base.suspend();
        UnityDebugger.Debugger.Log("CheckBoxState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        AddCallBack();
        base.resume();
        UnityDebugger.Debugger.Log("CheckBoxState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        UIEventListener.Get(m_uiCheckBox.m_buttonSystemOK.gameObject).onClick = OnButtonOkClick;
        UIEventListener.Get(m_uiCheckBox.m_buttonSystemCancel.gameObject).onClick = OnButtonCancelClick;
        UIEventListener.Get(m_uiCheckBox.m_buttonOtherOK.gameObject).onClick = OnButtonOkClick;
        UIEventListener.Get(m_uiCheckBox.m_buttonOtherCancel.gameObject).onClick = OnButtonCancelClick;
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        UIEventListener.Get(m_uiCheckBox.m_buttonSystemOK.gameObject).onClick = null;
        UIEventListener.Get(m_uiCheckBox.m_buttonSystemCancel.gameObject).onClick = null;
        UIEventListener.Get(m_uiCheckBox.m_buttonSystemOK.gameObject).onClick = null;
        UIEventListener.Get(m_uiCheckBox.m_buttonSystemCancel.gameObject).onClick = null;
    }
    #region ButtonEvents
    private void OnButtonOkClick(GameObject go)
    {
        if (!isPlaying)
            return;

        System.Object obj = (userData.ContainsKey(GameDefine.CHECK_BOX_PARAM_KEY)) ? userData[GameDefine.CHECK_BOX_PARAM_KEY] : null;

        if (m_bIsAutoPop)
            m_mainApp.PopState();

        if (OnCheckBoxOKClick != null)
            OnCheckBoxOKClick();
        else if (OnCheckBoxOKClick_Param != null)
            OnCheckBoxOKClick_Param(obj);

        OnCheckBoxOKClick = null;
        OnCheckBoxOKClick_Param = null;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonCancelClick(GameObject go)
    {
        if (!isPlaying)
            return;

        System.Object obj = (userData.ContainsKey(GameDefine.CHECK_BOX_PARAM_KEY)) ? userData[GameDefine.CHECK_BOX_PARAM_KEY] : null;

        if (m_bIsAutoPop)
            m_mainApp.PopState();

        if (OnCheckBoxCancelClick != null)
            OnCheckBoxCancelClick();
        else if (OnCheckBoxCancelClick_Param != null)
            OnCheckBoxCancelClick_Param(obj);

        OnCheckBoxCancelClick = null;
        OnCheckBoxCancelClick_Param = null;
    }
    #endregion
}

