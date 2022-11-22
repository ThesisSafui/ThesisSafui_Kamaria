using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Player.Animation
{ 
    public enum PlayerAnimations
    {
        move,
        attack1, attack2, attack3, attack4,
        isAttacking,
        isDash,
        isCanMove,
        isAttackSpin,
        skillRedStone,
        isUsingBow, isUsingSword, isUsingGun, isUsingSpear, isUsingKnuckle, isUsingCannon,
        isChangeWeapon,
        bowAttack,
        isBowCharge,
        isDead,
        getDamage,
        isStun,
        combo1Speed, combo2Speed, combo3Speed, combo4Speed,
        isInteract,
        attackKnuckleLeft, attackKnuckleRight, attackHeavyPunch, isGuard, skillGreenStone, isGuardCounter,
        isAim, cannonAttack, isGrenadeToss, gunAttack, gunSpeed, attackSpear
    }
    
    [Serializable]
    public sealed class PlayerAnimationData
    {
        public Animator Animator;
        public bool IsAttacking;
        public bool IsAnimationAttackNotMove;

        [SerializeField] private RuntimeAnimatorController animatorControllers;
        [SerializeField] private List<SpeedAnimation> speedAnimations = new List<SpeedAnimation>();

        #region SetAnimationValue
        
        public int AnimMove { get; } = Animator.StringToHash(PlayerAnimations.move.ToString());
        public int[] AnimComboAttacks { get; } = new[]
        {
            Animator.StringToHash(PlayerAnimations.attack1.ToString()),
            Animator.StringToHash(PlayerAnimations.attack2.ToString()),
            Animator.StringToHash(PlayerAnimations.attack3.ToString()),
            Animator.StringToHash(PlayerAnimations.attack4.ToString()),
        };
        public int AnimIsAttacking { get; } = Animator.StringToHash(PlayerAnimations.isAttacking.ToString());
        public int AnimDash { get; } = Animator.StringToHash(PlayerAnimations.isDash.ToString());
        public int AnimIsCanMove { get; } = Animator.StringToHash(PlayerAnimations.isCanMove.ToString());
        public int AnimIsAttackSpin { get; } = Animator.StringToHash(PlayerAnimations.isAttackSpin.ToString());
        public int AnimSkillRedStone { get; } = Animator.StringToHash(PlayerAnimations.skillRedStone.ToString());
        public int AnimIsUsingBow { get; } = Animator.StringToHash(PlayerAnimations.isUsingBow.ToString());
        public int AnimIsUsingSword { get; } = Animator.StringToHash(PlayerAnimations.isUsingSword.ToString());
        public int AnimIsUsingGun { get; } = Animator.StringToHash(PlayerAnimations.isUsingGun.ToString());
        public int AnimIsUsingSpear { get; } = Animator.StringToHash(PlayerAnimations.isUsingSpear.ToString());
        public int AnimIsUsingKnuckle { get; } = Animator.StringToHash(PlayerAnimations.isUsingKnuckle.ToString());
        public int AnimIsUsingCannon { get; } = Animator.StringToHash(PlayerAnimations.isUsingCannon.ToString());
        public int AnimIsChangeWeapon { get; } = Animator.StringToHash(PlayerAnimations.isChangeWeapon.ToString());
        public int AnimBowAttack { get; } = Animator.StringToHash(PlayerAnimations.bowAttack.ToString());
        public int AnimIsBowCharge { get; } = Animator.StringToHash(PlayerAnimations.isBowCharge.ToString());
        public int AnimIsDead { get; } = Animator.StringToHash(PlayerAnimations.isDead.ToString());
        public int AnimGetDamage { get; } = Animator.StringToHash(PlayerAnimations.getDamage.ToString());
        public int AnimIsStun { get; } = Animator.StringToHash(PlayerAnimations.isStun.ToString());
        public int AnimCombo1Speed { get; } = Animator.StringToHash(PlayerAnimations.combo1Speed.ToString());
        public int AnimCombo2Speed { get; } = Animator.StringToHash(PlayerAnimations.combo2Speed.ToString());
        public int AnimCombo3Speed { get; } = Animator.StringToHash(PlayerAnimations.combo3Speed.ToString());
        public int AnimCombo4Speed { get; } = Animator.StringToHash(PlayerAnimations.combo4Speed.ToString());
        public int AnimIsInteract { get; } = Animator.StringToHash(PlayerAnimations.isInteract.ToString());
        public int AnimAttackKnuckleLeft { get; } = Animator.StringToHash(PlayerAnimations.attackKnuckleLeft.ToString());
        public int AnimAttackKnuckleRight { get; } = Animator.StringToHash(PlayerAnimations.attackKnuckleRight.ToString());
        public int AnimAttackHeavyPunch { get; } = Animator.StringToHash(PlayerAnimations.attackHeavyPunch.ToString());
        public int AnimIsGuard { get; } = Animator.StringToHash(PlayerAnimations.isGuard.ToString());
        public int AnimSkillGreenStone { get; } = Animator.StringToHash(PlayerAnimations.skillGreenStone.ToString());
        public int AnimIsGuardCounter { get; } = Animator.StringToHash(PlayerAnimations.isGuardCounter.ToString());
        public int AnimIsAim { get; } = Animator.StringToHash(PlayerAnimations.isAim.ToString());
        public int AnimCannonAttack { get; } = Animator.StringToHash(PlayerAnimations.cannonAttack.ToString());
        public int AnimIsGrenadeToss { get; } = Animator.StringToHash(PlayerAnimations.isGrenadeToss.ToString());
        public int AnimGunAttack { get; } = Animator.StringToHash(PlayerAnimations.gunAttack.ToString());
        public int AnimGunSpeed { get; } = Animator.StringToHash(PlayerAnimations.gunSpeed.ToString());
        public int AnimAttackSpear { get; } = Animator.StringToHash(PlayerAnimations.attackSpear.ToString());

        #endregion

        public RuntimeAnimatorController AnimatorControllers => animatorControllers;
        public List<SpeedAnimation> SpeedAnimations => speedAnimations;

        public void UsingAnimWeapon(PlayerAnimations usingWeapon)
        {
            CloseAnimWeapon();
            
            int animWeapon = Animator.StringToHash(usingWeapon.ToString());
            Animator.SetBool(animWeapon, true);
        }

        public void CloseAnimWeapon()
        {
            Animator.SetBool(AnimIsUsingSword, false);
            Animator.SetBool(AnimIsUsingBow, false);
            Animator.SetBool(AnimIsUsingGun, false);
            Animator.SetBool(AnimIsUsingSpear, false);
            Animator.SetBool(AnimIsUsingKnuckle, false);
            Animator.SetBool(AnimIsUsingCannon, false);
        }
    }
}