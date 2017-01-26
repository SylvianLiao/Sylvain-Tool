using UnityEngine;
using System.Security.Cryptography;
using System;
using Softstar.GamePacket;
using Softstar;

class PacketHandler_Login : IPacketHandler
{
    private LoginSystem m_loginSystem;

    public PacketHandler_Login(GameScripts.GameFramework.GameApplication app) : base(app)
    {
        m_loginSystem = app.GetSystem<LoginSystem>();
    }

    public override void RegisterPacketHandler()
    {
        m_networkSystem.RegisterCallback(PacketId.PACKET_SIGN_UP_RES, HandlePacket_SignUp);
        m_networkSystem.RegisterCallback(PacketId.PACKET_SIGN_IN_RES, HandlePacket_SignIn);
        m_networkSystem.RegisterCallback(PacketId.PACKET_THIRD_SIGN_IN_RES, HandlePacket_ThirdSignIn);
        m_networkSystem.RegisterCallback(PacketId.PACKET_THIRD_BIND_RES, HandlePacket_ThirdBind);
        m_networkSystem.RegisterCallback(PacketId.PACKET_DEVICE_TOKEN_RES, HandlePacket_DeviceToken);
        m_networkSystem.RegisterCallback(PacketId.PACKET_DEVICE_CHANGE_RES, HandlePacket_DeviceChange);
    }

    /// <summary>註冊封包</summary>
    public void SendPacket_SignUp(string pw)
    {
        SignUpPacket pk = new SignUpPacket();
        pk.pwd = pw;

        m_networkSystem.Send(pk);
    }
    public void HandlePacket_SignUp(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandleSignUpResponse : " + strResponse);
        SignUpResPacket pk = JsonUtility.FromJson<SignUpResPacket>(strResponse);

        if (pk.result)
        {
            m_loginSystem.SetUserID(pk.id);

            m_loginSystem.SignIn();
        }
    }

    /// <summary>登入封包</summary>
    public void SendPacket_SignIn(PlayerLoginData data)
    {
        SignInPacket pk = new SignInPacket();
        pk.id = data.UserId;
        pk.pwd = data.Password;

        m_networkSystem.Send(pk);
    }
    public void HandlePacket_SignIn(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_SignIn : " + strResponse);
        SignInResPacket pk = JsonUtility.FromJson<SignInResPacket>(strResponse);

        m_loginSystem.IsLogin = pk.result;
        //連線模式
        if (pk.result)
        {
            m_networkSystem.SetIdToken(m_loginSystem.GetUserID(), pk.token);
            m_loginSystem.SetUserToken(pk.token);
            PacketHandler_PlayerData packetPlayerData = m_networkSystem.GetPaketHandler(typeof(PacketHandler_PlayerData).Name) as PacketHandler_PlayerData;
            packetPlayerData.SendPacket_GetPlayerData();

#if DEVELOP
            MainApplication mainApp = m_gameApplication as MainApplication;
            UI_Development ui_icon = mainApp.GetGUIManager().GetGUI(typeof(UI_Development).Name) as UI_Development;
            ui_icon.m_labelMode.text = "OnlineMode UserID: "+m_loginSystem.GetUserID().ToString();
            ui_icon.Show();
#endif

            UnityDebugger.Debugger.Log(string.Format("SignIn OK, msg=[{0}] token[{1}]", pk.msg, pk.token));
        }
        else
        {
            m_loginSystem.SignUp();
            UnityDebugger.Debugger.LogError("SignIn Failed, msg=" + pk.msg);
        }
    }

    public void SendPacket_ThirdSignIn(string strThird, string strId, string strToken, string strPlatform)
    {
        ThirdSignInPacket pk = new ThirdSignInPacket();
        pk.third = strThird;
        pk.id = strId;
        pk.token = strToken;
        pk.platform = strPlatform;

        m_networkSystem.Send(pk);
    }

    public void HandlePacket_ThirdSignIn(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlePacket_ThirdSignIn " + strResponse);
        ThirdSignInResPacket pk = JsonUtility.FromJson<ThirdSignInResPacket>(strResponse);
        if(pk.result)
        {
            // 使用第三方登入後獲得的id與pwd登入
            m_loginSystem.SetLoginData(pk.id, pk.pwd);
            m_loginSystem.SignIn();
        }
        else
        {
            UnityDebugger.Debugger.LogError("Third sign in ERROR");
        }
    }

    public void SendPacket_ThirdBind(string strThirdName, string strThirdId, string strThirdToken, string strPlatform)
    {
        ThirdBindPacket pk = new ThirdBindPacket();
        pk.third_name = strThirdName;
        pk.third_id = strThirdId;
        pk.third_token = strThirdToken;
        pk.platform = strPlatform;

        m_networkSystem.Send(pk);
    }

    public void HandlePacket_ThirdBind(string strResponse)
    {
        MainApplication mainapp = m_gameApplication as MainApplication;
        UnityDebugger.Debugger.Log("HandlePacket_ThirdBind " + strResponse);
        ThirdBindResPacket pk = JsonUtility.FromJson<ThirdBindResPacket>(strResponse);
        if (pk.result)
        {
            // 第三方綁定成功
            UnityDebugger.Debugger.Log("Third bind success");
            
            mainapp.PushSystemCheckBox("", "帳號綁定成功!!", "確認", null, null);
        }
        else
        {
            UnityDebugger.Debugger.LogError("Third bind ERROR");
            mainapp.PushSystemCheckBox("", "帳號綁定失敗!!", "確認", null, null);
        }
    }

    public void SendPacket_DeviceToken()
    {
        DeviceTokenPacket pk = new DeviceTokenPacket();

        m_networkSystem.Send(pk);
    }

    public void HandlePacket_DeviceToken(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlePacket_DeviceToken " + strResponse);
        DeviceTokenResPacket pk = JsonUtility.FromJson<DeviceTokenResPacket>(strResponse);
        ChangeDeviceState state = m_gameApplication.GetGameStateByName(StateName.CHANGE_DEVICE_STATE) as ChangeDeviceState;
        state.HandleGetPW(pk);
    }

    public void SendPacket_DeviceChange(string ott)
    {
        DeviceChangePacket pk = new DeviceChangePacket();
        pk.ott = ott;
        m_networkSystem.Send(pk);
    }

    public void HandlePacket_DeviceChange(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlePacket_DeviceChange " + strResponse);
        DeviceChangeResPacket pk = JsonUtility.FromJson<DeviceChangeResPacket>(strResponse);

        MainApplication mainapp = m_gameApplication as MainApplication;
        if (pk.result)
        {
            mainapp.SetGamePause(true);
            mainapp.InitializeGameByRestart();
            mainapp.SetGamePause(false);
            
            m_loginSystem.SetLoginData(pk.uid, pk.pwd);
            /*LoginSystem loginSys = mainapp.GetSystem<LoginSystem>();
            NetworkSystem netSys = mainapp.GetSystem<NetworkSystem>();
            netSys.SetIdToken(pk.uid, loginSys.m_playerLoginData.Token);*/

            mainapp.ChangeState(StateName.TITLE_STATE);

            UnityDebugger.Debugger.Log(string.Format("DeviceChange to ID : {0}", pk.uid));
        }
        else
        {
            mainapp.PushSystemCheckBox(0, 293, 0, null, null);    //"換機密碼錯誤"
            UnityDebugger.Debugger.LogError("HandlePacket_DeviceChange ERROR: " + strResponse);
        }
    }
}
