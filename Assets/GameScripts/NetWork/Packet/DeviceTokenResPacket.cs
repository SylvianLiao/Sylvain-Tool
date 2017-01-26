using System;
using Softstar.GameFramework.Network;

namespace Softstar.GamePacket
{
    [Serializable]
    public class DeviceTokenResPacket : Packet
    {
        public string token;
        public string expire;

        public DeviceTokenResPacket() : base()
        {
            cmd = PacketId.PACKET_DEVICE_TOKEN_RES;
            token = "";
            expire = "";
        }
    }
}
