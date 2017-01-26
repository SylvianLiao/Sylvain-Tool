using UnityEngine;
using System.Collections;
using Softstar;

public class UI_Fade : NGUIChildGUI
{
    public UITexture m_textureScreenShot;

    private int m_iUIWidth;
    private int m_iUIHeight;
    //-------------------------------------------------------------------------------------------------
    private UI_Fade() : base()
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
    public IEnumerator ScreenShot()
    {
        Show();

        UIRoot root = GameObject.FindObjectOfType<UIRoot>();

        m_iUIWidth = Screen.width;
        m_iUIHeight = Screen.height;

        //在擷取畫面之前請等到所有的Camera都Render完
        yield return new WaitForEndOfFrame();

        Texture2D texture = new Texture2D(m_iUIWidth, m_iUIHeight, TextureFormat.RGB24 , false , false);
        //擷取全部畫面的資訊
        texture.ReadPixels(new Rect(0, 0, m_iUIWidth, m_iUIHeight), 0, 0, false);
        texture.Apply();

        m_textureScreenShot.height = Mathf.RoundToInt((float)root.manualWidth / m_iUIWidth * m_iUIHeight);
        m_textureScreenShot.width = root.manualWidth;

        m_textureScreenShot.mainTexture = texture;
    }
    //-------------------------------------------------------------------------------------------------
    public IEnumerator FadeOutScreenShot()
    {
        yield return new WaitForSeconds(0.1f);

        OnFadeOutFinish.Add(ResetFadeUI);
        FadeOut();
    }
    //-------------------------------------------------------------------------------------------------
    private void ResetFadeUI()
    {
        GameObject.DestroyImmediate(m_textureScreenShot.mainTexture);
        m_textureScreenShot.mainTexture = null;

        foreach (var panel in m_allPanelList)
        {
            panel.alpha = 1.0f;
        }
    }
}
