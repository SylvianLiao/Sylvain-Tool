using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class TestPacket : Packet
    {
        public int num;

        public TestPacket():base()
        {
            cmd = PacketId.PACKET_TEST;
            num = 0;
        }
    }
}
