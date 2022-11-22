using System;
using Kamaria.SteamManagerAPI;
using LootLocker.Requests;
using Steamworks;
using UnityEngine;

namespace Kamaria.Authentication
{
    internal static class AuthenticationHandler
    {
        #region EVENTS
        
        /// <summary>
        /// Event login success.
        /// Callback steam name.
        /// </summary>
        internal static event Action<string> OnLoginSuccess;
        
        /// <summary>
        /// Event login failed.
        /// Callback error.
        /// </summary>
        internal static event Action<string> OnLoginFailed;

        #endregion

        #region PRIVATE_VARIABLE

        private const string STEAM_NOT_INITIALIZED = "Steam not initialized";        // Callback Steam not initialized.
        private const string LOGIN_FAILED = "Login failed";                          // Callback Login failed.

        #endregion
        
        #region AUTHENTICATION_STEAM

        /// <summary>
        /// Login with steam.
        /// </summary>
        internal static void LoginSteam()
        {
            if (!SteamManager.Initialized)
            {
                Debug.Log($"{STEAM_NOT_INITIALIZED}");
                OnLoginFailed?.Invoke(STEAM_NOT_INITIALIZED);
                return;
            }

            byte[] ticket = new byte[1024];
            HAuthTicket newTicket = SteamUser.GetAuthSessionTicket(ticket, 1024, out uint ticketSize);

            string steamSessionTicket = LootLockerSDKManager.SteamSessionTicket(ref ticket, ticketSize);

            LootLockerSDKManager.VerifySteamID(steamSessionTicket, verifyResponse =>
            {
                if (verifyResponse.success)
                {
                    Debug.Log($"Successfully verified steam user");

                    CSteamID steamID = SteamUser.GetSteamID();

                    LootLockerSDKManager.StartSteamSession(steamID.ToString(), sessionResponse =>
                    {
                        if (sessionResponse.success)
                        {
                            Debug.Log($"Session started!");
                            string steamName = SteamFriends.GetFriendPersonaName(steamID);

                            OnLoginSuccess?.Invoke(steamName);
                        }
                        else
                        {
                            Debug.Log($"Error starting session: {sessionResponse.Error}");
                            OnLoginFailed?.Invoke($"{LOGIN_FAILED}\nError:{sessionResponse.Error}");
                        }
                    });
                }
                else
                {
                    Debug.Log($"Error verifying steam ID: {verifyResponse.Error}");
                    OnLoginFailed?.Invoke($"{LOGIN_FAILED}\nError:{verifyResponse.Error}");
                }
            });
        }

        #endregion
    }
}