namespace Kamaria.Item.Weapon
{
    public interface IBaseItemWeaponManager
    {
        /// <summary>
        /// Unlock skill component1.
        /// </summary>
        public void UnlockSkillComponent1();
        
        /// <summary>
        /// Unlock skill component2.
        /// </summary>
        public void UnlockSkillComponent2();
        
        /// <summary>
        /// Unlock skill component3.
        /// </summary>
        public void UnlockSkillComponent3();

        /// <summary>
        /// Evolution item.
        /// </summary>
        public void Evolution();
    }
}