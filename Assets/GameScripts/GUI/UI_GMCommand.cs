using UnityEngine;
using System.Collections;
using Softstar;

public class UI_GMCommand : NGUIChildGUI
{    
    public UIButton m_buttonSend;
    public GameObject m_objGMCommand;
    public UIInput m_gmInput;
    public UILabel m_lbGMRes;

    //-------------------------------------------------------------------------------------------------
    private UI_GMCommand() : base()
    {        
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
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

    public void SetGMResultLabel(string msg)
    {
        m_lbGMRes.text = msg;
    }
}
