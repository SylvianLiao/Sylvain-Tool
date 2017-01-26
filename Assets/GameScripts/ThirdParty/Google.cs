using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.SocialPlatforms;

using GameScripts.GameFramework;
using System;

namespace Softstar.ThirdParty
{
    public class Google : AbstractThirdParty
    {
        public const string PLATFORM_IOS = "google_ios";

        private GameApplication m_app;

        public Google(GameApplication app)
        {
            m_app = app;
        }

        //======================================================================
        // Interface

        public override void Initialize()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                //.EnableSavedGames()
                //.WithInvitationDelegate(InvitationReceived)
                //.WithMatchDelegate(MatchReceived)
                //.RequireGooglePlus()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        public override void SignIn(SignInDelegate signInCallback)
        {
            UnityDebugger.Debugger.Log("Google.SignIn");
            Social.localUser.Authenticate(AuthenticateResult);

            m_signInDelegate = signInCallback;
        }

        public override bool Authenticated()
        {
            return Social.localUser.authenticated;
        }

        //======================================================================
        // Google Callback

        /*
        private void InvitationReceived(Invitation invitation, bool shouldAutoAccept)
        {
            UnityDebugger.Debugger.Log("Google.InvitationReceived");
        }

        private void MatchReceived(TurnBasedMatch match, bool shouldAutoLaunch)
        {
            UnityDebugger.Debugger.Log("Google.MatchReceived");
        }
        */

        private void AuthenticateResult(bool success)
        {
            UnityDebugger.Debugger.Log("AuthenticateResult " + success);
            if (success)
            {
                // 登入成功，分iOS版及Android版
                UnityDebugger.Debugger.Log(string.Format("GoogleAuthenticateResult [{0}][{1}]", Social.localUser.id, Social.localUser.userName));
                #if UNITY_IOS
                // iOS版直接取得Access Token
                string strToken = PlayGamesPlatform.Instance.GetAccessToken();
                if(m_signInDelegate != null)
                {
                    m_signInDelegate(success, ThirdPartyName.GOOGLE, Social.localUser.id, strToken);
                }
                #else
                // Android版取得Server AuthCode交給Server以便讓Server跟Google換取Access Token
                PlayGamesPlatform.Instance.GetServerAuthCode(GetServerAuthCodeResult);
                #endif
            }
            else
            {
                UnityDebugger.Debugger.LogError("GoogleAuthenticateResult Error");
                if (m_signInDelegate != null)
                {
                    m_signInDelegate(success, ThirdPartyName.GOOGLE, null, null);
                }
            }
        }

        private void GetServerAuthCodeResult(CommonStatusCodes code, string data)
        {
            UnityDebugger.Debugger.Log(string.Format("GetServerAuthCodeResult [{0}][{1}]", code, data));

            if (code == CommonStatusCodes.Success)
            {
                if (m_signInDelegate != null)
                {
                    m_signInDelegate(true, ThirdPartyName.GOOGLE, Social.localUser.id, data);
                }
            }
            else
            {
                UnityDebugger.Debugger.LogError("CommonStatusCodes: " + code);
            }
        }
    }
}
