
namespace Softstar.ThirdParty
{
    public abstract class AbstractThirdParty
    {
        public delegate void SignInDelegate(bool bSuccess, string strThirdName, string strThirdId, string strThirdToken);

        protected SignInDelegate m_signInDelegate;
        //======================================================================
        // Abstract method
        public abstract void Initialize();
        public abstract void SignIn(SignInDelegate signInCallback);
        public abstract bool Authenticated();
    }

    public class ThirdPartyName
    {
        public static string FB = "fb";
        public static string GOOGLE = "google";
    }
}
