using UnityEngine;
using System.Collections;
using Softstar;

public class UI_CheckBox : NGUIChildGUI
{

    [Header("System Info")]
    public GameObject m_containerSystemType;
    public UILabel  m_labelSystemTitle;
    public UILabel  m_labelSystemContent;
    public UIButton m_buttonSystemOK;
    public UILabel  m_labelSystemOK;
    public UIButton m_buttonSystemCancel;

    [Header("Other Info")]
    public GameObject m_containerOtherType;
    public UISprite m_spriteOtherIcon;
    public UILabel m_labelOtherTitle;
    public UILabel m_labelOtherContentTitle;
    public UILabel m_labelOtherContent;
    public UIButton m_buttonOtherOK;
    public UILabel m_labelOtherOK;
    public UIButton m_buttonOtherCancel;
    //-------------------------------------------------------------------------------------------------
    private UI_CheckBox() : base(){}
    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(Hashtable table, int iconID)
    {
        if (iconID <= 0)
        {
            m_labelSystemTitle.text = table[GameDefine.CHECK_BOX_TITLE_KEY] as string;
            m_labelSystemTitle.gameObject.SetActive(!string.IsNullOrEmpty(m_labelSystemTitle.text));
            m_labelSystemContent.text = table[GameDefine.CHECK_BOX_CONTENT_KEY] as string;
            m_labelSystemOK.text = table[GameDefine.CHECK_BOX_OK_KEY] as string;
            m_buttonSystemOK.gameObject.SetActive(!string.IsNullOrEmpty(m_labelSystemOK.text)); 
        }
        else
        {
            Softstar.Utility.ChangeAtlasSprite(m_spriteOtherIcon, iconID);
            m_labelOtherTitle.text = table[GameDefine.CHECK_BOX_TITLE_KEY] as string;
            m_labelOtherTitle.gameObject.SetActive(!string.IsNullOrEmpty(m_labelOtherTitle.text));
            m_labelOtherContentTitle.text = table[GameDefine.CHECK_BOX_CONTENT_TITLE_KEY] as string;
            m_labelOtherContentTitle.gameObject.SetActive(!string.IsNullOrEmpty(m_labelOtherContentTitle.text));
            m_labelOtherContent.text = table[GameDefine.CHECK_BOX_CONTENT_KEY] as string;
            m_labelOtherOK.text = table[GameDefine.CHECK_BOX_OK_KEY] as string;
            //m_buttonOtherOK.gameObject.SetActive(!string.IsNullOrEmpty(m_labelOtherOK.text));
        }
        m_containerSystemType.SetActive(iconID <= 0);
        m_containerOtherType.SetActive(iconID > 0);
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
}
