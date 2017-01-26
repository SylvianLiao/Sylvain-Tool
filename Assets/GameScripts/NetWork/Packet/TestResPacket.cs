using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class TestResPacket : Packet
    {
        public int data;

        public TestResPacket() : base()
        {
            cmd = PacketId.PACKET_TEST_RES;
            data = 0;
        }
    }
}
