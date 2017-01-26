using UnityEngine;
using System.Collections;
using Softstar;

public class UI_PlayerInfo : NGUIChildGUI
{
    public UIButton     m_buttonAvatar;
    public UILabel      m_labelPlayerName;

    public GameObject   m_containerButton;
    public UIButton     m_buttonListen;
    public UIButton     m_buttonShop;
    public UIButton     m_buttonSetting;
    public UIButton     m_buttonTeach;

    public GameObject   m_containerMoney;
    public UILabel      m_labelMoney;
    public UILabel      m_labelMallMoney;

    public EventDelegate.Callback OnButtonSettingClickEvent;
    public EventDelegate.Callback OnButtonListeningClickEvent;
    public EventDelegate.Callback OnButtonMallClickEvent;
    public EventDelegate.Callback OnButtonTeachClickEvent;

    private MainApplication m_mainApp;

    private UI_PlayerInfo() : base(){ }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    public void InitializeUI(string name, int mallMoney)
    {
        SetPlayerName(name);
        SetDiamondUI(mallMoney);
    }
    public void SetMainApp(MainApplication app)
    {
        m_mainApp = app;
    }
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
        ShowButton(true);
        ShowAvatar(true);
        ShowMoney(true);
        CheckShowTeachButton();
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
        UIEventListener.Get(m_buttonListen.gameObject).onClick = OnButtonListenClick;
        UIEventListener.Get(m_buttonShop.gameObject).onClick = OnButtonShopClick;
        UIEventListener.Get(m_buttonSetting.gameObject).onClick = OnButtonSettingClick;
        UIEventListener.Get(m_buttonTeach.gameObject).onClick = OnButtonTeachClick;
    }
    //---------------------------------------------------------------------------------------------------
    public void RemoveCallBack()
    {
        UIEventListener.Get(m_buttonListen.gameObject).onClick = null;
        UIEventListener.Get(m_buttonShop.gameObject).onClick = null;
        UIEventListener.Get(m_buttonSetting.gameObject).onClick = null;
        UIEventListener.Get(m_buttonTeach.gameObject).onClick = null;
    }
    //-------------------------------------------------------------------------------------------------
    //UI Setting
    public void SetPlayerAvatarUI(string spriteName)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonAvatar, spriteName, spriteName, spriteName, spriteName);
    }
    public void SetPlayerName(string name)
    {
        m_labelPlayerName.text = name;
    }
    public void SetDiamondUI(int mallMoney)
    {
        m_labelMallMoney.text = mallMoney.ToString();
    }
    #region ButtonEvent
    //---------------------------------------------------------------------------------------------------
    private void OnButtonListenClick(GameObject go)
    {
        if (OnButtonListeningClickEvent != null)
            OnButtonListeningClickEvent();

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        m_mainApp.PushStateByScreenShot(StateName.MUSIC_LISTENING_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonShopClick(GameObject go)
    {
        if (OnButtonMallClickEvent != null)
            OnButtonMallClickEvent();

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        m_mainApp.PushStateByScreenShot(StateName.SHOP_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonSettingClick(GameObject go)
    {
        if (OnButtonSettingClickEvent != null)
            OnButtonSettingClickEvent();

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        m_mainApp.PushStateByScreenShot(StateName.SETTING_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonTeachClick(GameObject go)
    {
        if (OnButtonTeachClickEvent != null)
            OnButtonTeachClickEvent();
        TeachingSystem teachSys = m_mainApp.GetSystem<TeachingSystem>();
        teachSys.NewGuideStart();
        teachSys.SetGuideEventToState(false);

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        m_mainApp.PushStateByScreenShot(StateName.LOAD_GAME_STATE, table);
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
    /// <summary>顯示[聆聽]、[商城]、[設定]、[教學]按鈕與否</summary>
    public void ShowButton(bool bShow)
    {
        m_containerButton.SetActive(bShow);
    }
    //---------------------------------------------------------------------------------------------------
    public void ShowAvatar(bool bShow)
    {
        m_buttonAvatar.gameObject.SetActive(bShow);
    }
    //---------------------------------------------------------------------------------------------------
    public void ShowMoney(bool bShow)
    {
        m_containerMoney.gameObject.SetActive(bShow);
    }
    //---------------------------------------------------------------------------------------------------
    private void CheckShowTeachButton()
    {
        TeachingSystem teachSys = m_mainApp.GetSystem<TeachingSystem>();
        m_buttonTeach.gameObject.SetActive(teachSys.CheckIsGuideFinished());
    }
}
