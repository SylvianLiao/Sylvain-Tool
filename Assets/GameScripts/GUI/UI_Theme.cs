using UnityEngine;
using System.Collections;
using Softstar;


public class UI_Theme : NGUIChildGUI
{
    public UITexture    m_textureMainBG;
    public UIButton     m_buttonIntroduce;
    public UITexture    m_textureIcon;
    public UITexture    m_textureThemeName;
    public UILabel      m_labelNews;

    public UIButton     m_buttonEnter;

    //-------------------------------------------------------------------------------------------------
    private UI_Theme() : base()
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
    //UI Setting
    public void SetNewsLabel(string news)
    {
        m_labelNews.text = news;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetInforTextureAndIcon(PlayerDataSystem dataSystem)
    {
        int themeID = dataSystem.GetCurrentThemeID();
        Softstar.Utility.ChangeTexture(m_textureIcon, dataSystem.GetThemeInforIconPath(themeID));
        Softstar.Utility.ChangeTexture(m_textureThemeName, dataSystem.GetThemeInforTexturePath(themeID));
    }
}
