using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Softstar.GameFramework.Network;
using Softstar.GamePacket;
using Softstar;
public class NetworkSystem : BaseSystem
{
    public delegate void ResponseCallback(string strPacket);

    public int UserID { set; get; }
    public string Token { set; get; }

    public string ServerURL { set; get; }

    private Queue<string> m_resPacketQueue;
    private Dictionary<string, ResponseCallback> m_responseCallback;
    private Dictionary<string, IPacketHandler> m_packetHandlerMap;

	public NetworkSystem(GameScripts.GameFramework.GameApplication app) : base(app)
	{
        m_resPacketQueue = new Queue<string>();
        m_responseCallback = new Dictionary<string, ResponseCallback>();
        m_packetHandlerMap = new Dictionary<string, IPacketHandler>();
    }

    public void SetServerURL(string url, int port)
    {
        ServerURL = string.Format("http://{0}:{1}/", url, port);
        UnityDebugger.Debugger.Log("Server URL: [" + ServerURL + "]");
    }

    public void SetIdToken(int id, string token)
    {
        UserID = id;
        Token = token;
    }

    public override void Initialize()
    {
        base.Initialize();

        ServerURL = "http://127.0.0.1:8080/";

        // FOR TEST
        RegisterCallback(PacketId.PACKET_TEST_RES, TestResPacketProcess);

        // Handle Error
        RegisterCallback(PacketId.PACKET_ERROR, HandleNetworkError);
    }

    public override void Update()
    {
        base.Update();
        while (m_resPacketQueue.Count > 0)
        {
            string strRes = m_resPacketQueue.Dequeue();
            //UnityDebugger.Debugger.Log("Packet dequeue: "+strRes);
            Softstar.GameFramework.Network.Packet pk = UnityEngine.JsonUtility.FromJson<Softstar.GameFramework.Network.Packet>(strRes);
            //UnityDebugger.Debugger.Log(pk.cmd);

            if(m_responseCallback.ContainsKey(pk.cmd))
            {
                m_responseCallback[pk.cmd](strRes);
            }
            else
            {
                UnityDebugger.Debugger.LogError("Unknown response packet : " + strRes);
            }
        }
    }

    //=========================================================================

    private void TestResPacketProcess(string strPacket)
    {
        Softstar.GamePacket.TestResPacket pk = UnityEngine.JsonUtility.FromJson<Softstar.GamePacket.TestResPacket>(strPacket);
        UnityDebugger.Debugger.Log("cmd : " + pk.cmd);
        UnityDebugger.Debugger.Log("data : " + pk.data);
    }

    public void RegisterCallback(string strResPacketId, ResponseCallback cb)
    {
        m_responseCallback[strResPacketId] = cb;
    }

    public void Send(Packet packet)
    {
        string strJson = JsonUtility.ToJson(packet);
        Send(strJson);
    }
    public void Send(UserPacket packet)
    {
        packet.id = UserID;
        packet.token = Token;
        Send((Packet)packet);
    }

    public void Send(string strJson)
    {
        string strBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(strJson));
        //UnityDebugger.Debugger.Log(strBase64);

        string strUri = ServerURL + "cmd/p";
        UnityDebugger.Debugger.Log(strUri);
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["s"] = strBase64;

        MusicApplication musicApp = ((Softstar.MainApplication)gameApplication).MusicApp;
        musicApp.StartCoroutine(SendHttpRequest(strUri, parameters));
    }

    public IEnumerator SendHttpRequest(string uri, Dictionary<string, string> parameters=null, string method="POST")
    {
        UnityWebRequest request;
        if(method=="POST")
        {
            request = UnityWebRequest.Post(uri, parameters);
        }
        else
        {
            string strUri = uri;
            if(parameters!=null && parameters.Count>0)
            {
                string strParameters = "";
                int count = 0;
                foreach(KeyValuePair<string, string> kv in parameters)
                {
                    if(count > 0)
                    {
                        strParameters += "&";
                    }
                    strParameters += (kv.Key + "=" + kv.Value);
                }
                UnityDebugger.Debugger.Log("Get Parameter: " + strParameters);

                strUri += "?" + strParameters;
            }
            request = UnityWebRequest.Get(strUri);
        }

        yield return request.Send();

        if(request.isError)
        {
            UnityDebugger.Debugger.LogError(request.error);
            ErrorPacket pk = new ErrorPacket();
            Dictionary<string, string> responseHeaders = request.GetResponseHeaders();
            if (responseHeaders != null)
            {
                //pk.status = int.Parse(responseHeaders["Status"]); // 取得http的status code
            }
            pk.msg = request.error;
            m_resPacketQueue.Enqueue(JsonUtility.ToJson(pk));
        }
        else if(request.responseCode == HttpStatus.OK)
        {
            UnityDebugger.Debugger.Log(request.downloadHandler.text);
            m_resPacketQueue.Enqueue(request.downloadHandler.text);
        }
        else
        {
            UnityDebugger.Debugger.LogError("ERROR status code: " + request.responseCode);
            ErrorPacket pk = new ErrorPacket();
            pk.msg = request.error;
            m_resPacketQueue.Enqueue(JsonUtility.ToJson(pk));
        }
        yield return null;
    }

    /// <summary>無法連到Server的錯誤狀況(無論是否有連網都會回傳該錯誤)</summary>
    public void HandleNetworkError(string strPacket)
    {
        ErrorPacket pk = JsonUtility.FromJson<ErrorPacket>(strPacket);
        UnityDebugger.Debugger.LogError(string.Format("HandleNetworkError msg[{0}]", pk.msg));

        //離線模式
        Softstar.MainApplication mainApp = gameApplication as Softstar.MainApplication;
        mainApp.IninAllData_Offline();
        TitleState ts = gameApplication.GetGameStateByName(StateName.TITLE_STATE) as TitleState;
        ts.LoginUISetting(TitleState.Enum_TitleLoginStatus.Offline);

#if DEVELOP
        UI_Development ui_icon = mainApp.GetGUIManager().GetGUI(typeof(UI_Development).Name) as UI_Development;
        ui_icon.m_labelMode.text = "OfflineMode";
        ui_icon.Show();
#endif
    }

    #region PacketHandler
    public bool AddPaketHandler(string key, IPacketHandler ph)
    {
        if (m_packetHandlerMap.ContainsKey(key))
            return false;

        m_packetHandlerMap.Add(key, ph);
        ph.RegisterPacketHandler();
        return true;
    }
    public bool RemovePaketHandler(string key)
    {
        if (!m_packetHandlerMap.ContainsKey(key))
            return false;

        m_packetHandlerMap.Remove(key);
        return true;
    }
    public IPacketHandler GetPaketHandler(string key)
    {
        if (!m_packetHandlerMap.ContainsKey(key))
            return null;

        return m_packetHandlerMap[key];
    }
    public T GetPacketHandler<T>() where T : IPacketHandler
    {
        string key = typeof(T).Name;
        if (!m_packetHandlerMap.ContainsKey(key))
            return null;

        return m_packetHandlerMap[key] as T;
    }
    #endregion
}
