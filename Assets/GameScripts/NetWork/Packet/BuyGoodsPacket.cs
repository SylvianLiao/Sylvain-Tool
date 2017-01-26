using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    class BuyGoodsPacket : UserPacket
    {
        public int goods_id;

        public BuyGoodsPacket():base()
        {
            cmd = PacketId.PACKET_BUY_GOODS;
            id = -1;
            goods_id = 0;
        }
    }

    [Serializable]
    public class BuyGoodsResPacket : Packet
    {
        public bool result;
        public string msg;
        public int sync_money_type;
        public int sync_now_money;

        public BuyGoodsResPacket() : base()
        {
            cmd = PacketId.PACKET_BUY_GOODS_RES;
            result = false;
            msg = string.Empty;
            sync_money_type = -1;
            sync_now_money = 0;
        }
    }
}
