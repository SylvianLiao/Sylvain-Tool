using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class ThirdSignInPacket : Packet
    {
        public string third;
        public string id;
        public string token;
        public string platform;

        public ThirdSignInPacket() : base()
        {
            cmd = PacketId.PACKET_THIRD_SIGN_IN;
            third = "third";
            id = "0";
            token = "12345";
            platform = "";
        }
    }
}
