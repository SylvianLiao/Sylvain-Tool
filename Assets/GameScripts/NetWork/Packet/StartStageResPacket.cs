using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class StartStageResPacket : Packet
    {
        public bool result;
        public string msg;

        public StartStageResPacket() : base()
        {
            cmd = PacketId.PACKET_START_STAGE_RES;
            result = false;
            msg = string.Empty;
        }
    }
}