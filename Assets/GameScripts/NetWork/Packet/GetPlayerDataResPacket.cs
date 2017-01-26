using System;
using Softstar.GameFramework.Network;
using System.Collections.Generic;

namespace Softstar.GamePacket
{
    [Serializable]
    public class GetPlayerDataResPacket : Packet
    {
        public bool result;
        public string msg;

        public int game_money;
        public int real_money;
        public int real_money_bygame;

        public List<StageData> stage_datas;

        public GetPlayerDataResPacket() : base()
        {
            cmd = PacketId.PACKET_GET_PLAYER_DATA_RES;
            result = false;
            msg = "";

            game_money = 0;
            real_money = 0;
            real_money_bygame = 0;
            stage_datas = new List<StageData>();
        }
    }

    [Serializable]
    public class StageData
    {
        //public int user_id;
        public int stage_group;

        public StageSlot easy;
        public StageSlot normal;
        public StageSlot hard;        

        public StageData()
        {
            //user_id = -1;
            stage_group = 0;

            easy = new StageSlot();
            normal = new StageSlot();
            hard = new StageSlot();            
        }
    }

    [Serializable]
    public class StageSlot
    {
        //0=未擁有, 1=擁有, 2=暫時擁有
        public int owned;
        public int score;
        public int combo;
        public int rank;

        public StageSlot()
        {
            owned = 0;
            score = 0;
            combo = 0;
            rank = 0;
        }
    }

}
