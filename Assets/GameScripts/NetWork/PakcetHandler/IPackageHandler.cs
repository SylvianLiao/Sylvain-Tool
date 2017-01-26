using UnityEngine;
using System.Collections;
using Softstar.GameFramework.Network;

public abstract class IPacketHandler
{
    protected GameScripts.GameFramework.GameApplication m_gameApplication;
    protected NetworkSystem m_networkSystem;

    public IPacketHandler(GameScripts.GameFramework.GameApplication app)
    {
        m_gameApplication = app;
        m_networkSystem = app.GetSystem<NetworkSystem>();
    }

    public abstract void RegisterPacketHandler();
}
