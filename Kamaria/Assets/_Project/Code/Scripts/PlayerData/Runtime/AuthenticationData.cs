using System;
using Kamaria.Utilities;

namespace Kamaria.Player.Data
{
    [Serializable]
    public sealed class AuthenticationData : IPlayerData
    {
        public bool FirstLogin = true;
        public int Age = 0;
        public PlayerGenders Gender = PlayerGenders.None;
        
        public void Initialized()
        {
            FirstLogin = true;
            Age = 0;
            Gender = PlayerGenders.None;
        }

        public void GetData(PlayerData playerData)
        {
            FirstLogin = playerData.Authentication.FirstLogin;
            Age = playerData.Authentication.Age;
            Gender = playerData.Authentication.Gender;
        }
    }
}