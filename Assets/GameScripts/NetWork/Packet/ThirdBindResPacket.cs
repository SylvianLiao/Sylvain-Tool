using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class ThirdBindResPacket : Packet
    {
        public bool result;

        public ThirdBindResPacket() : base()
        {
            cmd = PacketId.PACKET_THIRD_BIND_RES;
            result = false;
        }
    }
}
