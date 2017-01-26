using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_CommonBackground : NGUIChildGUI
{
    public UILabel m_labelTitle;
    public GameObject m_contanierPages;
    public UIButton[] m_buttonPages;    //由使用者自行註冊監聽
    public UILabel[] m_labelPages;
    //-------------------------------------------------------------------------------------------------
    private UI_CommonBackground() : base()
    {
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(string strTitle, List<string> strPages, bool setDefaultPage = true)
    {
        m_labelTitle.text = strTitle;
        if (strPages != null && strPages.Count > 0)
        {
            int lastIndex = m_labelPages.Length - 1;
            for (int i = 0, iCount = m_labelPages.Length; i < iCount; ++i)
            {
                if (i >= strPages.Count)
                {
                    m_buttonPages[i].gameObject.SetActive(false);
                    if (lastIndex > i)
                        lastIndex = i;
                    continue;
                }
                m_buttonPages[i].userData = i;
                m_labelPages[i].text = strPages[i];
                m_buttonPages[i].gameObject.SetActive(true);
            }
            //設定顯示預設頁面
            if (setDefaultPage)
                SwitchPageButton(lastIndex - 1);

            m_contanierPages.SetActive(true);
        }
        else
            m_contanierPages.SetActive(false);
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
        for (int i = 0, iCount = m_buttonPages.Length; i < iCount; ++i)
        {
            UIEventListener.Get(m_buttonPages[i].gameObject).onClick = OnButtonPagesClick;
        } 
    }
    //---------------------------------------------------------------------------------------------------
    public void RemoveCallBack()
    {
        for (int i = 0, iCount = m_buttonPages.Length; i < iCount; ++i)
        {
            UIEventListener.Get(m_buttonPages[i].gameObject).onClick = null;
        }
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonPagesClick(GameObject go)
    {
        UIButton btn = go.GetComponent<UIButton>();
        if (btn == null)
            return;
        int pageIndex = (int)btn.userData;
        SwitchPageButton(pageIndex);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchPageButton(int pageIndex)
    {
        for (int i = 0, iCount = m_buttonPages.Length; i < iCount; ++i)
        {
            Softstar.Utility.ChangeButtonSprite(m_buttonPages[i], (pageIndex == i) ? 77 : 78);
            m_buttonPages[i].isEnabled = (pageIndex != i);
        }
    }
}
