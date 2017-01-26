using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class ThirdBindPacket : UserPacket
    {
        public string third_name;
        public string third_id;
        public string third_token;
        public string platform;

        public ThirdBindPacket() : base()
        {
            cmd = PacketId.PACKET_THIRD_BIND;
            third_name = "";
            third_id = "";
            third_token = "";
            platform = "";
        }
    }
}
