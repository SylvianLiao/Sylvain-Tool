using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class SignInResPacket : Packet
    {
        public bool result;
        public string token;
        public string msg;

        public SignInResPacket() : base()
        {
            cmd = PacketId.PACKET_SIGN_IN_RES;
            result = true;
            token = "";
            msg = "";
        }
    }
}
