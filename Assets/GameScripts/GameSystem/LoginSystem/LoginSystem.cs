using UnityEngine;
using System.Security.Cryptography;
using System;
using Softstar.GamePacket;
using Softstar.GameFramework.Network;
using Softstar;

class LoginSystem : BaseSystem
{
    private MainApplication m_mainApp;
    private RecordSystem m_recordSystem;
    private PacketHandler_Login m_packetLogin;

    public PlayerLoginData m_playerLoginData;

    public bool IsLogin;

    public LoginSystem(GameScripts.GameFramework.GameApplication gameApplication): base(gameApplication)
    {
        m_mainApp = gameApplication as MainApplication;
    }
    //-------------------------------------------------------------------------------------------------
    private void SetDebugData()
    {
        string strSsmgapp = "{ \"UserId\":2,\"Password\":\"VKmtXkg+7eQ/Rs14\"}"; // ssmgapp.softstar.com.tw        
        //string strSsmgapp = "{ \"UserId\":8,\"Password\":\"paUv6lkvrDzjQcRt\"}"; // localhost
        m_recordSystem.SetPlayerLoginData(strSsmgapp);
    }
    //-------------------------------------------------------------------------------------------------
    public override void Initialize()
    {
        base.Initialize();

        MainApplication app = gameApplication as MainApplication;
        m_recordSystem = app.GetSystem<RecordSystem>();
       
        // ssmgapp
        //SetDebugData();
        IsLogin = false;
        string strUserData = m_recordSystem.GetPlayerLoginData();
        m_playerLoginData = JsonUtility.FromJson<PlayerLoginData>(strUserData);
        UnityDebugger.Debugger.Log("Login user data : " + strUserData);  
    }
    //-------------------------------------------------------------------------------------------------
    public override void Update()
    {
        base.Update();
    }
    //-------------------------------------------------------------------------------------------------
    public void SetPacketLogin(PacketHandler_Login packet)
    {
        m_packetLogin = packet;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetUserID(int id)
    {
        m_playerLoginData.UserId = id;
        string strJson = JsonUtility.ToJson(m_playerLoginData);
        UnityDebugger.Debugger.Log("SignUp OK, id=" + id);
        UnityDebugger.Debugger.Log("Save to json : " + strJson);
        m_recordSystem.SetPlayerLoginData(strJson);
    }
    public void SetUserToken(string token)
    {
        m_playerLoginData.Token = token;
        string strJson = JsonUtility.ToJson(m_playerLoginData);
        UnityDebugger.Debugger.Log("SignUp OK, Token=" + token);
        UnityDebugger.Debugger.Log("Save to json : " + strJson);
        m_recordSystem.SetPlayerLoginData(strJson);
    }
    public void SetLoginData(int id, string pwd)
    {
        m_playerLoginData.UserId = id;
        m_playerLoginData.Password = pwd;
        m_playerLoginData.Token = "";
        string strJson = JsonUtility.ToJson(m_playerLoginData);
        m_recordSystem.SetPlayerLoginData(strJson);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetUserID()
    {
        return m_playerLoginData.UserId;
    }
    //-------------------------------------------------------------------------------------------------
    public bool IsSignUp()
    {
        bool bRet = false;
        if(m_playerLoginData != null)
        {
            if(m_playerLoginData.UserId >= 0)
            {
                bRet = true;
            }
        }
        return bRet;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>快速登入</summary>
    public void SignUp()
    {
        IsLogin = false;
        // Generate random password
        RNGCryptoServiceProvider rcsp = new RNGCryptoServiceProvider();
        byte[] rndByteAry = new byte[12];
        rcsp.GetBytes(rndByteAry);
        m_playerLoginData.Password = Convert.ToBase64String(rndByteAry);
        UnityDebugger.Debugger.Log("Random Password : " + m_playerLoginData.Password);

        m_packetLogin.SendPacket_SignUp(m_playerLoginData.Password);
    }

    //-------------------------------------------------------------------------------------------------
    public void SignIn()
    {
        m_packetLogin.SendPacket_SignIn(m_playerLoginData);  
    }
    public void ThirdSignIn(string strThird, string strId, string strToken, string strPlatform="")
    {
        m_packetLogin.SendPacket_ThirdSignIn(strThird, strId, strToken, strPlatform);
    }
    public void ThirdBind(string strThird, string strId, string strToken, string strPlatform="")
    {
        m_packetLogin.SendPacket_ThirdBind(strThird, strId, strToken, strPlatform);
    }
}
