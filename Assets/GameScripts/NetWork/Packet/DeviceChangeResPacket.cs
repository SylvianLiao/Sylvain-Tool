using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class DeviceChangeResPacket : Packet
    {
        public bool result;
        public int uid;
        public string pwd;

        public DeviceChangeResPacket() : base()
        {
            cmd = PacketId.PACKET_DEVICE_TOKEN_RES;
            result = false;
            uid = -1;
            pwd = "";
        }
    }
}
