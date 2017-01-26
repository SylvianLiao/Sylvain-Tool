using UnityEngine;
using System.Collections;
using Softstar;

public enum Enum_ThemeIntroduceStyle
{
    LabelStyle,
    TextureStyle
}

public class UI_ThemeIntroduce : NGUIChildGUI
{
    public UILabel m_labelTitle;
    public UIButton m_buttonBackToTheme;
    public UIScrollView m_scrollView;
    public UITable m_tabelRoot;
    public UILabel m_labelExample;
    public UITexture m_textureExample;
    //-------------------------------------------------------------------------------------------------
    private UI_ThemeIntroduce() : base()
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
    //-------------------------------------------------------------------------------------------------
    public void SetIntroduceTitle(string title)
    {
        m_labelTitle.text = title;
    }
    // 增加一組字串
    public void AddLabel(string text)
    {
        GameObject newLabel = NGUITools.AddChild(m_tabelRoot.gameObject, m_labelExample.gameObject);
        newLabel.SetActive(true);

        UILabel uiLabel = newLabel.GetComponent<UILabel>();
        if (uiLabel)
            uiLabel.text = text;

        RepositionContent();
    }
    //-------------------------------------------------------------------------------------------------
    // 增加一組貼圖
    public void AddTexture(string texturePath)
    {
        GameObject newTexture = NGUITools.AddChild(m_tabelRoot.gameObject, m_textureExample.gameObject);
        newTexture.SetActive(true);
        UITexture uiTexture = newTexture.GetComponent<UITexture>();
        if (uiTexture)
        {
            if (Softstar.Utility.ChangeTexture(uiTexture, texturePath))
            {
                uiTexture.MakePixelPerfect();
                RepositionContent();
            }
            else
            {
                GameObject.Destroy(newTexture);
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void RepositionContent()
    {
        m_tabelRoot.Reposition();
    }
}
