using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class GetPlayerDataPacket : UserPacket
    {
        public GetPlayerDataPacket() : base()
        {
            cmd = PacketId.PACKET_GET_PLAYER_DATA;
            id = -1;
        }
    }
}
