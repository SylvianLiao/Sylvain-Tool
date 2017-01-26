using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class DeviceChangePacket : Packet
    {
        public string ott;
        public DeviceChangePacket() : base()
        {
            cmd = PacketId.PACKET_DEVICE_CHANGE;
            ott = "";
        }
    }
}
