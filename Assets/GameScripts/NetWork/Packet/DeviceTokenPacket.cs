using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class DeviceTokenPacket : UserPacket
    {
        public DeviceTokenPacket() : base()
        {
            cmd = PacketId.PACKET_DEVICE_TOKEN;
        }
    }
}
