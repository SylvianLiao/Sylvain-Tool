using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_TopBar : NGUIChildGUI
{
    public UIButton m_buttonBack;

    private MainApplication m_mainApp;

    //Delegate
    public EventDelegate.Callback m_onButtonBackClick;

    private UI_TopBar() : base(){ }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void SetMainApp(MainApplication app)
    {
        m_mainApp = app;
    } 
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Hide()
    {
        base.Hide();
    }
    //-------------------------------------------------------------------------------------------------
    public override void UiUpdate()
    {
        base.UiUpdate();
    }
    //---------------------------------------------------------------------------------------------------
    public void AddCallBack()
    {
        UIEventListener.Get(m_buttonBack.gameObject).onClick = OnButtonBackClick;
    }
    //---------------------------------------------------------------------------------------------------
    public void RemoveCallBack()
    {
        UIEventListener.Get(m_buttonBack.gameObject).onClick = null;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick(GameObject go)
    {
        if (m_onButtonBackClick != null)
        {
            m_onButtonBackClick();
        }
        else
        {
            m_mainApp.PopState();
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonPagesClick(GameObject go)
    {
        
    }
}

