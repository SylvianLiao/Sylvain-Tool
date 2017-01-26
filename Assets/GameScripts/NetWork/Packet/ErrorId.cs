
namespace Softstar.GamePacket
{
    public class ErrorId
    {
        public const int ERR_NONE = 0;              // 無錯誤
        public const int ERR_PWD = 1;               // 帳密錯誤
        public const int ERR_TOKEN_MISMATCH = 2;    // Token錯誤
        public const int ERR_TOKEN_EXPIRED = 3;     // Token過期
    }
}
