using System;
using UnityEngine;

namespace Kamaria.Utilities
{
    public enum SoundTypes
    {
        BGM, SFX, UI
    }

    public enum SoundCharacter
    {
        Player, Enemy
    }
    
    [Serializable]
    public sealed class SoundClip
    {
        public SoundCharacter soundCharacter;
        public Sound sound;
        public AudioClip audioClip;
        [Range(0, 1)] public float soundVolume;
        public bool loop = false;
        public bool mute;
        public bool untilTheEnd;
        public bool use3D = false;
        [Range(0, 1)] public float spatialBlend;
        [Range(0, 100)] public float min3D;
        [Range(0, 100)] public float max3D;
        public AudioRolloffMode rolloffMode;
        public AudioSource audioSource;

        public enum Sound
        {
            //BGM
            BGMBattle, BGMLobby,
            
            //UI
            Click, 
            
            //PlayerSound
            PlayerWalk, PlayerDead, PlayerDash, PlayerRun,
            
            PlayerSwordAttack1, PlayerSwordAttack2, PlayerSwordAttack3, PlayerSwordAttack4Start, PlayerSwordAttack4End, PlayerSwordQ, 
            PlayerSwordGroundSlash, PlayerBowChargeHold, PlayerBowChargeEnd,
            
            PlayerGloveAttack1, PlayerGloveAttack2, PlayerGloveCounterAttack, PlayerGloveFireStone, PlayerGloveWindStone,
            PlayerGloveSkillGuardCounterStart, PlayerGloveBlueStoneStart, PlayerGloveBlueStoneEnd,
            
            PlayerRifleAim, PlayerRifleGrenadeStart, PlayerRifleGrenadeEnd,
            PlayerFireLaserAttack, PlayerRifleWindAttackStart, PlayerRifleWindAttackEnd, PlayerBlueSpearAttack,
            
            //EnemyMouseSound
            MouseNormalAttack, MouseCombat1, MouseCombat2, 
            //EnemyFishSound
            FishGrenadeStart, FishGrenadeEnd,
            //EnemyGolemSound
            GolemNormalAttack, GoloemSpinAttack,
            //EnemySharkSound
            SharkNormalAttackStart, SharkNormalAttackEnd, SharkJumpStart, SharkJumpEnd, SharkDash, SharkSummon,
            //EnemySkeletonSound
            SkeletonNormalAttack, SkeletonPistolAttack, SkeletonMeteor, SkeletonSummon,
        }
    }
}
