using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class SignInPacket : Packet
    {
        public int id;
        public string pwd;

        public SignInPacket() : base()
        {
            cmd = PacketId.PACKET_SIGN_IN;
            id = 0;
            pwd = "12345";
        }
    }
}
