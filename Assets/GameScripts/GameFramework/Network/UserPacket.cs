using System;

namespace Softstar.GameFramework.Network
{
    [Serializable]
    public class UserPacket : Packet
    {
        public int id;
        public string token;
    }
}
