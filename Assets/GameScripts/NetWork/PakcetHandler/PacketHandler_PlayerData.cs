using UnityEngine;
using System.Collections;
using Softstar.GamePacket;
using Softstar;

public class PacketHandler_PlayerData : IPacketHandler
{
    private MainApplication m_mainApp;
    public PacketHandler_PlayerData(GameScripts.GameFramework.GameApplication app) : base(app)
    {
        m_mainApp = app as MainApplication;
    }

    public override void RegisterPacketHandler()
    {
        m_networkSystem.RegisterCallback(PacketId.PACKET_GMCOMMAND_RES, HandlerPacket_GMCommand);
        m_networkSystem.RegisterCallback(PacketId.PACKET_GET_PLAYER_DATA_RES, HandlerPacket_GetPlayerData);
    }

    public void SendPacket_GMCommand(string command)
    {
        GMCommandPacket pk = new GMCommandPacket();
        pk.command = command;
        m_networkSystem.Send(pk);
    }

    private void HandlerPacket_GMCommand(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_GetPlayerData : " + strResponse);
        GMCommandResPacket pk = JsonUtility.FromJson<GMCommandResPacket>(strResponse);


        GMCommandState GMState = m_mainApp.GetGameStateByName(StateName.GM_COMMAND_STATE) as GMCommandState;
        if(GMState != null)
        {
            GMState.CommandResultProcess(pk);
        }        
    }


    /// <summary>取得玩家資料封包</summary>
    public void SendPacket_GetPlayerData()
    {
        GetPlayerDataPacket pk = new GetPlayerDataPacket();
        m_networkSystem.Send(pk);
    }

    private void HandlerPacket_GetPlayerData(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_GetPlayerData : " + strResponse);
        GetPlayerDataResPacket pk = JsonUtility.FromJson<GetPlayerDataResPacket>(strResponse);

        m_mainApp.InitAllData(pk);
        TitleState ts = m_mainApp.GetGameStateByName(StateName.TITLE_STATE) as TitleState;
        ts.LoginUISetting((m_mainApp.IsLogin()) ? TitleState.Enum_TitleLoginStatus.Online : TitleState.Enum_TitleLoginStatus.Offline);
    }
}
