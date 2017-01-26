using UnityEngine;
using System.Collections;
using Softstar;

public class UI_GamePause : NGUIChildGUI
{
    public UIPanel m_panelBase;
    public UIButton m_buttonResume;
    public UIButton m_buttonChooseSong;
    public UIButton m_buttonChooseDifficulty;
    public UIButton m_buttonRestart;
    public UILabel m_labelResume;
    public UILabel m_labelChooseSong;
    public UILabel m_labelChooseDifficulty;
    public UILabel m_labelRestart;

    //-------------------------------------------------------------------------------------------------
    private UI_GamePause() : base()
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
        m_labelResume.text = st.GetString(68);              //"回到遊戲"
        m_labelChooseSong.text = st.GetString(71);          //"歌曲選單"
        m_labelChooseDifficulty.text = st.GetString(70);    //"選擇難度"
        m_labelRestart.text = st.GetString(69);             //"重新開始"
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
