using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class SignUpResPacket : Packet
    {
        public bool result;
        public int id;

        public SignUpResPacket() : base()
        {
            cmd = PacketId.PACKET_SIGN_UP_RES;
            result = true;
            id = 0;
        }
    }
}
