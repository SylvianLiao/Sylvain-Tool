using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class StartStagePacket : UserPacket
    {
        public int stage_id;

        public StartStagePacket() : base()
        {
            cmd = PacketId.PACKET_START_STAGE;
            id = -1;
            stage_id = 0;
        }
    }
}