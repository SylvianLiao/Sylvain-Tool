using UnityEngine;
using System.Collections;
using Softstar;

public class UI_ChangeDevice : NGUIChildGUI
{
    /*
    [Header("Choose")]
    public GameObject m_containerChoose;
    public UILabel m_labelCDTitle;
    public UILabel m_labelCDContent;
    public UIButton m_buttonGetPW;
    public UILabel m_labelGetPW;
    public UIButton m_buttonEnterPW;
    public UILabel m_labelEnterPW;
    public UIButton m_buttonChooseClose;
    */
    public UIButton m_buttonClose;

    [Header("Get Password")]
    public GameObject m_containerGetPW;
    public UILabel m_labelGetTitle;
    public UILabel m_labelGetContent;
    public UILabel m_labelPassword;
    public UILabel m_labelCoolDown;

    [Header("Enter Password")]
    public GameObject m_containerEnterPW;
    public UILabel m_labelEnterTitle;
    public UILabel m_labelEnterContent;
    public UILabel m_inputField;
    public UIButton m_buttonEnterOK;
    public UILabel m_labelEnterOK;

    //-------------------------------------------------------------------------------------------------
    private UI_ChangeDevice() : base()
    {
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(StringTable st)
    {
        /*
        //選擇頁面
        m_labelCDTitle.text = st.GetString(302);        //"切換裝置"
        m_labelCDContent.text = st.GetString(306);      //"切換裝置說明內容"
        m_labelGetPW.text = st.GetString(304);          //"取得換機密碼"
        m_labelEnterPW.text = st.GetString(305);        //"輸入換機密碼"
        */
        //取得換機密碼頁面
        //m_labelGetTitle.text = st.GetString(291);       //"取得換機密碼"
        //m_labelGetContent.text = st.GetString(307);     //"取得換機密碼說明內容"

        //輸入換機密碼頁面
        //m_labelEnterTitle.text = st.GetString(292);     //"輸入換機密碼"
        //m_labelEnterContent.text = st.GetString(308);   //"輸入換機密碼說明內容"
        m_labelEnterOK.text = st.GetString(291);         //"輸入換機密碼"
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
    public void SetGetPWCoolDown(string time)
    {
        m_labelCoolDown.text = time;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetOTP(string pw)
    {
        m_labelPassword.text = pw;
    }
    /*
    //-------------------------------------------------------------------------------------------------
    public void SwitchChoosePage(bool bSwtich)
    {
        m_containerChoose.SetActive(bSwtich);
        m_containerGetPW.SetActive(!bSwtich);
        m_containerEnterPW.SetActive(!bSwtich);
    }
    */
    //-------------------------------------------------------------------------------------------------
    public void SwitchGetPWPage(bool bSwtich)
    {
        m_containerGetPW.SetActive(bSwtich);
        m_containerEnterPW.SetActive(!bSwtich);
        //m_containerChoose.SetActive(!bSwtich);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchEnterPWPage(bool bSwtich)
    {
        m_containerEnterPW.SetActive(bSwtich);
        m_containerGetPW.SetActive(!bSwtich);
        //m_containerChoose.SetActive(!bSwtich);
    }
}
