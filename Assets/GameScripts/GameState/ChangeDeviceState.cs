using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class ChangeDeviceState : CustomBehaviorState
{
    private UI_ChangeDevice m_uiChangeDevice;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private PacketHandler_Login m_packetLogin;

    private string m_token = string.Empty;
    private DateTime m_tokenDeadTime = default(DateTime);

    private string strTokenKey = "ChangeDeviceTokenKey";

    private CdTimer m_cdToken = null;

    public ChangeDeviceState(GameScripts.GameFramework.GameApplication app) : base(StateName.CHANGE_DEVICE_STATE, StateName.CHANGE_DEVICE_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_packetLogin = m_mainApp.GetSystem<NetworkSystem>().GetPaketHandler(typeof(PacketHandler_Login).Name) as PacketHandler_Login;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("ChangeDeviceState begin");

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_ChangeDevice));

        base.begin();

        if (m_bIsAync == false)
        {
            m_uiChangeDevice = m_guiManager.AddGUI<UI_ChangeDevice>(typeof(UI_ChangeDevice).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        m_uiChangeDevice = m_guiManager.GetGUI(typeof(UI_ChangeDevice).Name) as UI_ChangeDevice;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_uiChangeDevice.InitializeUI(m_mainApp.GetStringTable());

        SetTokenData();

        //設定預設開啟頁面
        if (userData.ContainsKey(Enum_StateParam.EnterOTP))
        {
            m_uiChangeDevice.SwitchEnterPWPage(true);

        }
        else if (userData.ContainsKey(Enum_StateParam.GetOTP))
        {
            m_uiChangeDevice.SwitchGetPWPage(true);
            OnUIShowFinish += SendRequestToGetDeviceToken;
        }

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        m_uiChangeDevice = null;
        OnUIShowFinish = null;

        m_guiManager.DeleteGUI(typeof(UI_ChangeDevice).Name);
        base.end();
        UnityDebugger.Debugger.Log("ChangeDeviceState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        if (m_bIsSuspendByFullScreenState && m_uiChangeDevice != null)
        {
            CheckActiveCommonGUI();
            m_uiChangeDevice.Hide();
        }
        RemoveCallBack();
        base.suspend();
        UnityDebugger.Debugger.Log("ChangeDeviceState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        if (m_uiChangeDevice != null)
        {
            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                CheckActiveCommonGUI();
                AddCallBack();
            }
            m_bIsSuspendByFullScreenState = false;
        }
        base.resume();
        UnityDebugger.Debugger.Log("ChangeDeviceState resume");
    }
    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();

        if(m_cdToken != null && m_uiChangeDevice != null)
        {
           m_uiChangeDevice.m_labelCoolDown.text = Softstar.Utility.GetShowTime(Enum_TimeFormat.Minute, m_cdToken.m_CountDownTime);
        }
    }
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        //UIEventListener.Get(m_uiChangeDevice.m_buttonGetPW.gameObject).onClick = OnButtonGetPWClick;
        //UIEventListener.Get(m_uiChangeDevice.m_buttonEnterPW.gameObject).onClick = OnButtonEnterPWClick;
        //UIEventListener.Get(m_uiChangeDevice.m_buttonChooseClose.gameObject).onClick = OnButtonCloseClick;
        UIEventListener.Get(m_uiChangeDevice.m_buttonEnterOK.gameObject).onClick = OnButtonEnterOKClick;
        UIEventListener.Get(m_uiChangeDevice.m_buttonClose.gameObject).onClick = OnButtonCloseClick;
    }
    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        //UIEventListener.Get(m_uiChangeDevice.m_buttonGetPW.gameObject).onClick = null;
        //UIEventListener.Get(m_uiChangeDevice.m_buttonEnterPW.gameObject).onClick = null;
        //UIEventListener.Get(m_uiChangeDevice.m_buttonChooseClose.gameObject).onClick = null;
        UIEventListener.Get(m_uiChangeDevice.m_buttonEnterOK.gameObject).onClick = null;
        UIEventListener.Get(m_uiChangeDevice.m_buttonClose.gameObject).onClick = null;
    }
    //---------------------------------------------------------------------------------------------------
    public void HandleGetPW(Softstar.GamePacket.DeviceTokenResPacket pk)
    {
        try
        {
            m_tokenDeadTime = Convert.ToDateTime(pk.expire);
        }
        catch(FormatException e)
        {
            UnityDebugger.Debugger.Log(e + ": 不是格式正確的日期和時間字串!");
        }

        m_token = pk.token;

        CountDownSystem sys = m_mainApp.GetSystem<CountDownSystem>();
        sys.SyncServerTime(DateTime.Now);
        float f = (float)m_tokenDeadTime.Subtract(sys.GetAdjustedClientTime()).TotalSeconds;
        m_cdToken = sys.StartCountDown(strTokenKey, f, TokenTimeEnd);

        SetTokenData();
    }
    //-------------------------------------------------------------------------------------------------
    public void TokenTimeEnd(string key)
    {
        CountDownSystem sys = m_mainApp.GetSystem<CountDownSystem>();
        sys.CloseCountDown(key);
        m_token = string.Empty;
        m_cdToken = null;
        SetTokenData();
    }
    //-------------------------------------------------------------------------------------------------
    public void SetTokenData()
    {        
        m_uiChangeDevice.m_labelPassword.text = m_token;
    }
    //-------------------------------------------------------------------------------------------------
    private void SendRequestToGetDeviceToken()
    {
        //TODO: 送取得OTP要求給Server並倒數計時
        if (m_cdToken == null)
            m_packetLogin.SendPacket_DeviceToken();
    }
    //-------------------------------------------------------------------------------------------------
    #region ButtonEvents
    //-------------------------------------------------------------------------------------------------
    public void OnButtonCloseClick(GameObject go)
    {
        if (!isPlaying)
            return;

        m_mainApp.PopStateByScreenShot();
    }
    /*
    //-------------------------------------------------------------------------------------------------
    public void OnButtonGetPWClick(GameObject go)
    {
        m_uiChangeDevice.SwitchGetPWPage(true);

        //TODO: 送取得OTP要求給Server並倒數計時
        SendRequestToGetDeviceToken();
    }
    //-------------------------------------------------------------------------------------------------
    public void OnButtonEnterPWClick(GameObject go)
    {
        m_uiChangeDevice.SwitchEnterPWPage(true);
    }
    */
    //-------------------------------------------------------------------------------------------------
    public void OnButtonEnterOKClick(GameObject go)
    {
        if (string.IsNullOrEmpty(m_uiChangeDevice.m_inputField.text))
            return;     

        //TODO: 換機成功後，重新開始遊戲
        m_packetLogin.SendPacket_DeviceChange(m_uiChangeDevice.m_inputField.text);
    }
    #endregion
}

