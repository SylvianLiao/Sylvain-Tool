using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class SignUpPacket : Packet
    {
        public string pwd;

        public SignUpPacket() : base()
        {
            cmd = PacketId.PACKET_SIGN_UP;
            pwd = "1234";
        }
    }
}
