using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class BattleResultPacket : UserPacket
    {
        public int stage_id;
        public int score;
        public int combo;

        public BattleResultPacket() : base()
        {
            cmd = PacketId.PACKET_BATTLE_RESULT;
            id = -1;
            stage_id = -1;
            score = 0;
            combo = 0;
        }
    }
}
