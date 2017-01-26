using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class ErrorPacket : Packet
    {
        public int err_id;
        public string msg;

        public ErrorPacket() : base()
        {
            cmd = PacketId.PACKET_ERROR;
            err_id = 0;
            msg = "";
        }
    }
}
