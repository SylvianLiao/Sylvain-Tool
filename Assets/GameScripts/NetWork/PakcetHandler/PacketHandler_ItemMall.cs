using System;
using System.Collections;
using UnityEngine;
using Softstar.GamePacket;
using Softstar;

public class PacketHandler_ItemMall : IPacketHandler
{
    private MainApplication m_mainApp;

    public PacketHandler_ItemMall(GameScripts.GameFramework.GameApplication app) : base(app)
    {
        m_mainApp = app as MainApplication;
    }

    public override void RegisterPacketHandler()
    {
        m_networkSystem.RegisterCallback(PacketId.PACKET_BUY_GOODS_RES, HandlerPacket_BuyGoods);
        m_networkSystem.RegisterCallback(PacketId.PACKET_GET_ITEMMALL_DATA_RES, HandlerPacket_GetItemmallData);        
    }

    /// <summary>要求購買商品封包</summary>
    public void SendPacket_BuyGoods(int goodsGUID)
    {
        BuyGoodsPacket pk = new BuyGoodsPacket();       
        pk.goods_id = goodsGUID;

        m_networkSystem.Send(pk);
    }

    private void HandlerPacket_BuyGoods(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_BuyGoods : " + strResponse);

        BuyGoodsResPacket pk = JsonUtility.FromJson<BuyGoodsResPacket>(strResponse);
        if (pk.result)
        {
            ShopState shopState = m_mainApp.GetGameStateByName(StateName.SHOP_STATE) as ShopState;
            if (shopState != null)
            {
                //m_mainApp.MusicApp.StartCoroutine(shopState.BuyGoodsResult(pk));
                shopState.BuyGoodsResult(pk);
            }
            UnityDebugger.Debugger.Log("Buy Goods Success!");
        }
        else
            UnityDebugger.Debugger.Log("Buy Goods Fail! : " + pk.msg);
    }

    /// <summary>取得商城物品資料封包</summary>
    public void SendPacket_GetItemmallData()
    {
        GetItemmallDataPacket pk = new GetItemmallDataPacket();
        m_networkSystem.Send(pk);
    }

    private void HandlerPacket_GetItemmallData(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_GetItemmallData : " + strResponse);
        GetItemmallDataResPacket pk = JsonUtility.FromJson<GetItemmallDataResPacket>(strResponse);
        if (pk.result)
        {
            ShopState shopState = m_mainApp.GetGameStateByName(StateName.SHOP_STATE) as ShopState;
            if (shopState != null)
            {
                shopState.StateInitAfterDataGot(pk);
            }
            UnityDebugger.Debugger.Log("GetItemmallData Success!");
        }
        else
        {
            UnityDebugger.Debugger.Log("GetItemmallData Fail!");
        }
    }
}
