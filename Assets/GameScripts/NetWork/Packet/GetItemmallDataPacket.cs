using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class GetItemmallDataPacket : UserPacket
    {
        public GetItemmallDataPacket():base()
        {
            cmd = PacketId.PACKET_GET_ITEMMALL_DATA;
            id = -1;
        }
    }

    [Serializable]
    public class GetItemmallDataResPacket : Packet
    {
        public bool result;
        public ItemmallData[] itemmall_datas;
        public ShopData[] shop_datas;

        public GetItemmallDataResPacket() : base()
        {
            cmd = PacketId.PACKET_GET_ITEMMALL_DATA_RES;
            result = false;            
        }
    }

    [Serializable]
    public class ItemmallData
    {
        public int GUID;
        public int iName;
        public int iGoodsType;
        public int iCurrencyType;
        public int iOriginalGUID;
        public int iPrice;
        public float fDiscount;
        public int iTag;

        public ItemmallData()
        {

        }

        public int GetRealPrice()
        {
            return UnityEngine.Mathf.RoundToInt(iPrice * fDiscount);
        }
    }

    [Serializable]
    public class ShopData
    {
        public int GUID;
        public int iName;
        public int iGoodsType;

        public ShopData()
        {

        }
    }
}
