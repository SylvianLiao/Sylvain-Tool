using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class BattleResultResPacket : Packet
    {
        public bool result;
        public string msg;
        public int[] unlock_list;
        public int update_score;
        public int update_combo;
        public int stage_id;

        public BattleResultResPacket() : base()
        {
            cmd = PacketId.PACKET_BATTLE_RESULT_RES;
            result = false;
            msg = string.Empty;
            update_score = 0;
            update_combo = 0;
            stage_id = -1;
        }
    }
}