using UnityEngine;
using System.Collections;
using Softstar;

public class UI_Title : NGUIChildGUI
{
    public UILabel m_labelVersion;
    public UIButton m_buttonEnterGame;
    public UILabel m_labelEnterGame;
    public GameObject m_loginBackground;

    [Header("Login Buttons")]
    public UIButton m_buttonQuickLogin;
    public UILabel m_labelQuickLogin;
    public UIButton m_buttonConntectFacebook;
    public UILabel m_labelConntectFacebook;
    public UIButton m_buttonConntectGoogle;
    public UILabel m_labelConntectGoogle;
    public UIButton m_buttonChangeDevice;
    public UILabel m_labelChangeDevice;

    [Header("Member Rule")]
    public UILabel m_labelRuleTitle;
    public UILabel m_labelRuleContent;
    public UIButton m_buttonRuleOK;
    public UILabel m_labelRuleOK;
    public UIButton m_buttonRuleCancel;
    public UILabel m_labelRuleCancel;

    [Header("Develop")]
    public UILabel m_labelLoginLog;
    public UIButton m_buttonForceSignUp;
    public UIButton m_buttonClearData;

    private UI_Title() : base()
    {
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        m_buttonEnterGame.gameObject.SetActive(false);
        SwitchDevelopUI(false);
#if DEVELOP
        SwitchDevelopUI(true);
#endif
    }
    public void InitializeUI(StringTable st)
    {
        m_labelEnterGame.text = st.GetString(21);       //"請點擊進入遊戲"
        m_labelQuickLogin.text = st.GetString(35);      //"快速登入"
        m_labelConntectFacebook.text = st.GetString(301);//"連接帳號"(要換成FB)
        m_labelConntectGoogle.text = st.GetString(301); //"連接帳號"(要換成Google)
        m_labelChangeDevice.text = st.GetString(302);   //"登入其他帳號"

        m_labelRuleTitle.text = st.GetString(303);      //"會員規範"
        m_labelRuleOK.text = st.GetString(41);          //"確定"
        m_labelRuleCancel.text = st.GetString(42);      //"取消"
        m_labelRuleContent.text = st.GetString(300);    //"會員規範內容"
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
    public void SwitchLoginButtons(bool bSwitch)
    {
        m_buttonQuickLogin.gameObject.SetActive(bSwitch);
        m_buttonConntectFacebook.gameObject.SetActive(bSwitch);
        m_buttonConntectGoogle.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetLabelVersion(string str)
    {
        m_labelVersion.text = str;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetLoginLog(string log)
    {
        m_labelLoginLog.text = log;
        m_labelLoginLog.gameObject.SetActive(true);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchDevelopUI(bool bSwitch)
    {
        m_buttonForceSignUp.gameObject.SetActive(bSwitch);
        m_buttonClearData.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchMemberRuleUI(bool bSwitch)
    {
        m_allPanelList[1].gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchLoginBackground(bool bSwitch)
    {
        m_loginBackground.SetActive(bSwitch);
    }
}
