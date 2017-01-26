using System;
using System.Collections.Generic;
using Facebook.Unity;

using GameScripts.GameFramework;

namespace Softstar.ThirdParty
{
    public class Facebook : AbstractThirdParty
    {
        private GameApplication m_app;

        public Facebook(GameApplication app)
        {
            m_app = app;
        }

        //======================================================================
        // Interface

        public override void Initialize()
        {
            if (FB.IsInitialized == false)
            {
                FB.Init(OnInitComplete, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
        }

        public override void SignIn(SignInDelegate signInCallback)
        {
            UnityDebugger.Debugger.Log("Facebook.SignIn called");
            List<string> perms = new List<string>();
            perms.Add("public_profile");
            perms.Add("email");
            perms.Add("user_friends");
            FB.LogInWithReadPermissions(perms, AuthCallback);

            m_signInDelegate = signInCallback;
        }

        public override bool Authenticated()
        {
            return FB.IsLoggedIn;
        }

        //======================================================================
        // FB Callback

        private void OnInitComplete()
        {
            UnityDebugger.Debugger.Log("Facebook.OnInitComplete called");
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                UnityDebugger.Debugger.LogError("Facebook initialize FAILED");
            }
        }

        private void OnHideUnity(bool isShown)
        {
            UnityDebugger.Debugger.Log("Facebook.OnHideUnity called, isShown: " + isShown);
        }

        private void AuthCallback(ILoginResult result)
        {
            UnityDebugger.Debugger.Log("Facebook.AuthCallback called");
            if (FB.IsLoggedIn)
            {
                AccessToken token = AccessToken.CurrentAccessToken;
                UnityDebugger.Debugger.Log("FacebookAuth.Callback logged in: " + token.UserId);

                if(m_signInDelegate != null)
                {
                    m_signInDelegate(true, ThirdPartyName.FB, token.UserId, token.TokenString);
                }
            }
            else
            {
                UnityDebugger.Debugger.Log("FacebookAuth.Callback not log in");
                if (m_signInDelegate != null)
                {
                    m_signInDelegate(false, ThirdPartyName.FB, null, null);
                }
            }
        }
    }
}
