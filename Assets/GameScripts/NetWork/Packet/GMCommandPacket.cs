using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    class GMCommandPacket : UserPacket
    {
        public string command;

        public GMCommandPacket() : base()
        {
            cmd = PacketId.PACKET_GMCOMMAND;
            id = -1;
            command = string.Empty;
        }
    }

    [Serializable]
    public class GMCommandResPacket : Packet
    {
        public bool result;
        public string msg;
        public string command;

        public GMCommandResPacket() : base()
        {
            cmd = PacketId.PACKET_GMCOMMAND_RES;
            result = false;
            msg = string.Empty;
            command = string.Empty;
        }
    }
}
