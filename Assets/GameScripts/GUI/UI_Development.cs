using UnityEngine;
using System.Collections;
using Softstar;

public class UI_Development : NGUIChildGUI
{
    public UILabel m_labelMode;
    public UIButton m_buttonGMCommand;

    public Softstar.MainApplication m_mainApp;
    //-------------------------------------------------------------------------------------------------
    private UI_Development() : base()
    {
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        UIEventListener.Get(m_buttonGMCommand.gameObject).onClick = OnBtnGMCommandClick;
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
    //-------------------------------------------------------------------------------------------------
    public void OnBtnGMCommandClick(GameObject go)
    {
        if (m_mainApp.GetCurrentGameState().name != StateName.GM_COMMAND_STATE)
        {
            m_mainApp.PushState(StateName.GM_COMMAND_STATE);
        }
        else
            m_mainApp.PopState();
    }
}
