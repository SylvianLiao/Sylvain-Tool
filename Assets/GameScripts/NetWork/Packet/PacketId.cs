
namespace Softstar.GamePacket
{
    public class PacketId
    {
        public const string PACKET_ERROR = "error";

        public const string PACKET_TEST = "test";
        public const string PACKET_TEST_RES = "test_res";

        // 快速登入建立帳號
        public const string PACKET_SIGN_UP = "signup";
        public const string PACKET_SIGN_UP_RES = "signup_res";

        // 登入帳號
        public const string PACKET_SIGN_IN = "signin";
        public const string PACKET_SIGN_IN_RES = "signin_res";

        // 第三方登入
        public const string PACKET_THIRD_SIGN_IN = "third_signin";
        public const string PACKET_THIRD_SIGN_IN_RES = "third_signin_res";

        // 第三方綁定
        public const string PACKET_THIRD_BIND = "third_bind";
        public const string PACKET_THIRD_BIND_RES = "third_bind_res";

        // 輸入換機密碼
        public const string PACKET_DEVICE_CHANGE = "device_change";
        public const string PACKET_DEVICE_CHANGE_RES = "device_change_res";

        // 換機密碼
        public const string PACKET_DEVICE_TOKEN = "device_token";
        public const string PACKET_DEVICE_TOKEN_RES = "device_token_res";

        // 開啟關卡
        public const string PACKET_START_STAGE = "startstage";
        public const string PACKET_START_STAGE_RES = "startstage_res";

        // 關卡結算
        public const string PACKET_BATTLE_RESULT = "battleresult";
        public const string PACKET_BATTLE_RESULT_RES = "battleresult_res";

        // 取得關卡資料
        public const string PACKET_GET_PLAYER_DATA = "getplayerdata";
        public const string PACKET_GET_PLAYER_DATA_RES = "getplayerdata_res";

        // 購買商城物品
        public const string PACKET_BUY_GOODS = "buygoods";
        public const string PACKET_BUY_GOODS_RES = "buygoods_res";

        // 取得商城資料
        public const string PACKET_GET_ITEMMALL_DATA = "getitemmalldata";
        public const string PACKET_GET_ITEMMALL_DATA_RES = "getitemmalldata_res";

        // GM指令
        public const string PACKET_GMCOMMAND = "gmcommand";
        public const string PACKET_GMCOMMAND_RES = "gmcommand_res";
    }
}
