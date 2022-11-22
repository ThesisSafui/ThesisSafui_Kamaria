namespace Kamaria.Player.Data
{
    public interface IPlayerData
    {
        /// <summary>
        /// Initialized data.
        /// </summary>
        void Initialized();
        
        /// <summary>
        /// Get player data.
        /// </summary>
        /// <param name="playerData"> Data player </param>
        void GetData(PlayerData playerData);
    }
}