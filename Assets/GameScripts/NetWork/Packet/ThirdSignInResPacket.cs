using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class ThirdSignInResPacket : Packet
    {
        public bool result;
        public int id;
        public string pwd;

        public ThirdSignInResPacket() : base()
        {
            cmd = PacketId.PACKET_THIRD_SIGN_IN_RES;
            result = true;
            id = 0;
            pwd = "";
        }
    }
}
