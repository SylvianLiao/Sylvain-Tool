using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_Setting : NGUIChildGUI
{
    [Header("Setting Content")]
    public TweenAlpha m_tweenPageSetting;
    public UILabel m_labelSound;
    public UILabel m_labelAdjust;
    public UILabel m_labelLocalization;
    public UIButton m_buttonAdjust;
    public UILabel m_labelBtnAdjust;
    public UILabel m_labelBtnAdjustValue;
    public UIButton m_buttonSoundOn;
    public UIButton m_buttonSoundOff;
    public TweenPosition m_tweenLanguage;
    public List<UILabel> m_LanguageLabels = new List<UILabel>();
    public UIButton m_buttonPreLanguage;
    public UIButton m_buttonNextLanguage;
    public UIButton m_buttonConfirmChange;

    [Header("ID Content")]
    public TweenAlpha m_tweenPageID;
    public UILabel m_labelCreateID;
    public UILabel m_labelPlayerID;
    public UIButton m_buttonConnectFacebook;
    public UILabel m_labelConnectFacebook;
    public UIButton m_buttonConnectGoogle;
    public UILabel m_labelConnectGoogle;
    public UIButton m_buttonEnterOTP;
    public UILabel m_labelEnterOTP;
    public UIButton m_buttonGetOTP;
    public UILabel m_labelGetOTP;

    private UI_Setting() : base(){ }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(StringTable st)
    {
        m_labelSound.text = st.GetString(29);          //音效
        m_labelAdjust.text = st.GetString(30);         //校正
        m_labelLocalization.text = st.GetString(31);   //語言
        m_labelBtnAdjust.text = st.GetString(30);      //校正
        m_LanguageLabels[0].text = st.GetString(38);   //English
        m_LanguageLabels[1].text = st.GetString(37);   //中文(简体)
        m_LanguageLabels[2].text = st.GetString(36);   //中文(繁體)
        m_labelConnectFacebook.text = st.GetString(301); //"連接帳號"
        m_labelConnectGoogle.text = st.GetString(301);   //"連接帳號"
        m_labelEnterOTP.text = st.GetString(291);        //"輸入換機密碼"
        m_labelGetOTP.text = st.GetString(292);          //"取得換機密碼"
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
    public void SetAdjustNumber(float variable)
    {
        //取小數點第一位
        variable = Mathf.RoundToInt(variable * 100) / 100.0f;
        string sign = (variable >= 0) ? "+" : "";
        m_labelBtnAdjustValue.text = sign + string.Format("{0:0.00}", variable).ToString();
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchSoundButton(bool isON)
    {
        m_buttonSoundOn.gameObject.SetActive(isON);
        m_buttonSoundOff.gameObject.SetActive(!isON);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetLanguageConfirmButton(bool isUse)
    {
        Softstar.Utility.ChangeButtonSprite(m_buttonConfirmChange, (isUse)?100:101);
        m_buttonConfirmChange.enabled = !isUse;
    }
    //-------------------------------------------------------------------------------------------------
    public void TweenLanguageLabel(ENUM_LanguageType language)
    {
        m_tweenLanguage.from = m_tweenLanguage.transform.localPosition;
        m_tweenLanguage.to = m_LanguageLabels[(int)language].transform.localPosition;
        m_tweenLanguage.ResetToBeginning();
        m_tweenLanguage.PlayForward();
    }
    //-------------------------------------------------------------------------------------------------
    public void SetLanguageLabelPos(ENUM_LanguageType language)
    {
        m_tweenLanguage.transform.localPosition = m_LanguageLabels[(int)language].transform.localPosition;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>True = 切換至設定頁面，False = 切換至ID頁面</summary>
    public void SwitchToSettingPage(bool changeToSetting, EventDelegate.Callback callback)
    {
        m_tweenPageSetting.value = 0;
        m_tweenPageID.value = 0;
        if (changeToSetting)
        {
            EventDelegate.Add(m_tweenPageID.onFinished, m_tweenPageSetting.PlayForward, true);
            EventDelegate.Add(m_tweenPageSetting.onFinished, callback, true);
            m_tweenPageID.PlayReverse();
        }
        else
        {
            EventDelegate.Add(m_tweenPageSetting.onFinished, m_tweenPageID.PlayForward, true);
            EventDelegate.Add(m_tweenPageID.onFinished, callback, true);
            m_tweenPageSetting.PlayReverse();
        }
    }
  
}

