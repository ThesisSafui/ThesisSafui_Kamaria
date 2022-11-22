using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.UI.UIMainGame;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class CharacterSkill : MonoBehaviour
    {
        #region EVENT_ACTION

        public event Action CooldownSwordActiveSkillComponent1Action;
        public event Action CooldownSwordUltimateSkillAction;
        public event Action CooldownSwordPowerStoneSkillAction;
        public event Action CooldownChangeWeapon;
        public event Action CooldownKnuckleActiveSkillComponent1Action;
        public event Action CooldownKnuckleUltimateSkillAction;
        public event Action CooldownKnucklePowerStoneSkillAction;
        public event Action CooldownKnuckleCannonAttack;
        public event Action CooldownGunAttack;
        public event Action CooldownGunActiveSkillComponent1Action;
        public event Action CooldownGunUltimateSkillAction;
        public event Action CooldownGunPowerStoneSkillAction;
        public event Action CooldownPotion;

        #endregion

        [SerializeField] private GameManagerFarming gameManagerFarming;
        [SerializeField] private UIGamePlay uiGamePlay;
        [SerializeField] private FarmingManagerSO farmingManagerSo;
        [SerializeField] private PlayerCooldownManager cooldownManager;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private PlayerControllerInput playerControllerInput;
        [SerializeField] private Transform spawnGroundSlash;
        [SerializeField] private Transform spawnArrow;
        [SerializeField] private Indicator indicatorCannonAttackRange;
        [SerializeField] private Indicator indicatorCannonAttackPosition;
        [SerializeField] private Transform spawnBulletCannon;
        [SerializeField] private Transform spawnGrenade;
        [SerializeField] private Transform[] spawnBulletGun;
        [SerializeField] private GameObject[] vfxShootBulletGun;
        [SerializeField] private GameObject vfxBowFullCharge;
        [SerializeField] private GameObject vfxBowChargeShoot;
        [SerializeField] private GameObject laserLv1;
        [SerializeField] private GameObject laserLv2;
        [SerializeField] private float laserTime;
        [SerializeField] private GameObject vfxSpear;
        [SerializeField] private GameObject vfxHeal;

        private Animator anim;
        private AttacksRange tempWeaponAttacksRange;
        private BaseItem tempWeaponSword = new BaseItem();
        private BaseItem tempWeaponGun = new BaseItem();
        private BaseItem tempWeaponKnuckle = new BaseItem();
        private WeaponTypes weaponChange;
        private Vector3 positionFallBullet;
        private bool isRotation;

        public bool isSwordActiveSkillComponent1Cooldown = false;
        public bool isSwordUltimateSkillCooldown = false;

        public bool isKnuckleActiveSkillComponent1Cooldown = false;
        public bool isKnuckleUltimateSkillCooldown = false;
        public bool isCannonAttackCooldown = false;

        public bool isGunAttackCooldown = false;
        public bool isGunActiveSkillComponent1Cooldown = false;
        public bool isGunUltimateSkillCooldown = false;

        public bool isCooldownPotion = false;
        
        public bool UseSkillKnuckleRedStone { get; set; }
        public bool UseSkillGunRedStone { get; set; }

        private void OnEnable()
        {
            isRotation = false;
            indicatorCannonAttackRange.gameObject.SetActive(false);
            indicatorCannonAttackPosition.gameObject.SetActive(false);
            playerData.CharacterSkillData.Initialized();
            playerData.PlayerAnimation.Animator = GetComponent<Animator>();
            playerData.SetAnimator(out anim);
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, true);
        }

        private void OnDisable()
        {
        }
        
        public void UsePotion()
        {
            if (gameManagerFarming.Dungeon == Dungeons.None && gameManagerFarming.Map == Map.Lobby) return;
            
            if (farmingManagerSo.CurrentPotion - 1 < 0 || isCooldownPotion) return;
            CooldownPotion?.Invoke();
            farmingManagerSo.CurrentPotion--;
            
            uiGamePlay.SetUICooldownPotionFill();
            uiGamePlay.SetUIPotionCount(farmingManagerSo.CurrentPotion);
            
            int tempHeal = (int)((playerData.Info.Status.MaxHealth * farmingManagerSo.PercentHeal) / 100);
            
            if (playerData.Info.Status.CurrentHealth + tempHeal >= playerData.Info.Status.MaxHealth)
            {
                playerData.Info.Status.CurrentHealth = playerData.Info.Status.MaxHealth;
            }
            else
            {
                playerData.Info.Status.CurrentHealth += tempHeal;
            }

            vfxHeal.SetActive(true);
            uiGamePlay.UpdateUIHp();

            //Debug.Log($"HHHH CurrentHealth + {tempHeal}");
        }
        
        private void CloseLaser()
        {
            StopCoroutine(nameof(LaserTime));
            laserLv1.SetActive(false);
            laserLv2.SetActive(false);
            isRotation = false;
        }

        public void ChangeStatusWeaponToDefault()
        {
            InventoryWeapon(WeaponTypes.Sword).TypeAttackRange = AttacksRange.MeleeAttack;
            InventoryWeapon(WeaponTypes.Knuckle).TypeAttackRange = AttacksRange.MeleeAttack;
            InventoryWeapon(WeaponTypes.Gun).TypeAttackRange = AttacksRange.RangeAttack;
        }

        private void ChangeWeaponReset()
        {
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.IsChangeWeaponFinish = false;
            playerData.CharacterSkillData.IsBowAttackFinish = true;
            playerData.CharacterSkillData.IsBowAttacking = false;
            playerData.CharacterSkillData.IsBowCharging = false;
            playerData.CharacterSkillData.IsBowChargeAttacking = false;
            playerData.CharacterSkillData.CurrentCombo = 0;
            playerData.CharacterSkillData.UsingGuard = false;
            playerData.CharacterSkillData.IsUseSkillPowerStone =
                !playerData.CharacterSkillData.IsUseSkillPowerStone;
            anim.SetBool(playerData.PlayerAnimation.AnimIsChangeWeapon, true);
            anim.SetBool(playerData.PlayerAnimation.AnimIsBowCharge, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsGuard, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsAim, false);
        }
        
        private BaseItem InventoryWeapon(WeaponTypes weaponTypes)
        {
            return playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == weaponTypes);
        }
        
        #region SKILL_SWORD

        public void ShowVfxBowChargeShoot()
        {
            if (!playerData.IsBowChargeFull) return;
            
            vfxBowChargeShoot.SetActive(true);
        }
        
        public void ChangeWeapon()
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (playerData.CharacterSkillData.IsChangeWeaponKeyStone)
            {
                playerData.CharacterSkillData.IsChangeWeaponKeyStone = false;
                WeaponChangeStatus(weapon);
            }
            else
            {
                playerData.UseWeapon(weaponChange);
                playerEvent.OpenWeaponObj();
                playerData.CharacterSkillData.IsChangeWeaponFinish = true;
            }
            
            uiGamePlay.SetUIParentSkill();
            playerData.SetAnimator(out anim);
        }

        public void ChangeWeaponTo(WeaponTypes weaponTypes)
        {
            playerData.CharacterSkillData.IsUseSkillPowerStone = false;
            playerControllerInput.CancelAim();
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            WeaponChangeStatus(weapon);
            
            weaponChange = weaponTypes;
            CooldownChangeWeapon?.Invoke();
            ResetComboAttack();
            ResetColliderDamage();
            playerEvent.CloseVfx();
            playerEvent.CloseVfx();
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.IsChangeWeaponFinish = false;
            playerData.CharacterSkillData.IsBowAttackFinish = true;
            playerData.CharacterSkillData.IsBowAttacking = false;
            playerData.CharacterSkillData.IsBowCharging = false;
            playerData.CharacterSkillData.IsBowChargeAttacking = false;
            playerData.CharacterSkillData.CurrentCombo = 0;
            anim.SetBool(playerData.PlayerAnimation.AnimIsChangeWeapon, true);
            anim.SetBool(playerData.PlayerAnimation.AnimIsBowCharge, false);
        }

        private void WeaponChangeStatus(BaseItem weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponTypes.Sword:
                    ChangeWeaponSwordPowerStone();
                    break;
                case WeaponTypes.Knuckle:
                    ChangeWeaponKnucklePowerStone();
                    break;
                case WeaponTypes.Gun:
                    ChangeWeaponGunPowerStone();
                    break;
            }
        }

        private void ChangeWeaponSwordPowerStone()
        {
            if (playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                playerEvent.OpenWeaponPowerObj(WeaponTypes.Sword);
                //anim.SetBool(playerData.PlayerAnimation.AnimIsUsingBow, true);
                playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange = AttacksRange.RangeAttack;
            }
            else
            {
                playerEvent.OpenWeaponObj();
                //anim.SetBool(playerData.PlayerAnimation.AnimIsUsingBow, false);
                playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange = AttacksRange.MeleeAttack;
            }
            
            playerData.CharacterSkillData.IsChangeWeaponFinish = true;
        }
        
        private void ChangeWeaponKnucklePowerStone()
        {
            if (playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                indicatorCannonAttackRange.gameObject.SetActive(true);
                indicatorCannonAttackPosition.gameObject.SetActive(true);
                indicatorCannonAttackRange.SetSize(playerData.CharacterSkillData.LimitIndicatorCannonAttackSize);
                playerEvent.OpenWeaponPowerObj(WeaponTypes.Knuckle);
                //anim.SetBool(playerData.PlayerAnimation.AnimIsUsingBow, true);
                playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange = AttacksRange.RangeAttack;
            }
            else
            {
                indicatorCannonAttackRange.gameObject.SetActive(false);
                indicatorCannonAttackPosition.gameObject.SetActive(false);
                playerEvent.OpenWeaponObj();
                //anim.SetBool(playerData.PlayerAnimation.AnimIsUsingBow, false);
                playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange = AttacksRange.MeleeAttack;
            }
            
            playerData.CharacterSkillData.IsChangeWeaponFinish = true;
        }
        
        private void ChangeWeaponGunPowerStone()
        {
            if (playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                playerEvent.OpenWeaponPowerObj(WeaponTypes.Gun);
                //anim.SetBool(playerData.PlayerAnimation.AnimIsUsingBow, true);
                playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange = AttacksRange.MeleeAttack;
            }
            else
            {
                playerEvent.OpenWeaponObj();
                //anim.SetBool(playerData.PlayerAnimation.AnimIsUsingBow, false);
                playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange = AttacksRange.RangeAttack;
            }
            
            playerData.CharacterSkillData.IsChangeWeaponFinish = true;
        }

        internal void SwordNormalAttack()
        {
            if (playerData.PlayerAnimation.IsAttacking || playerData.CharacterSkillData.IsBowAttacking) return;
            
            playerControllerInput.RaycastDirection();

            if (playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                anim.SetBool(playerData.PlayerAnimation.AnimIsAttacking, true);
                playerData.CharacterSkillData.IsBowAttackFinish = false;
                playerData.CharacterSkillData.IsBowAttacking = true;
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                StartCoroutine(nameof(BowAttackDuration));
                RotationAttack();
            }
            else
            {
                SwordAttack();
            }
        }

        internal void BowChargeAttack()
        {
            if (playerData.CharacterSkillData.IsBowChargeAttacking)
            {
                if (playerData.PlayerAnimation.IsAttacking || playerData.CharacterSkillData.IsBowAttacking)
                {
                    playerData.CharacterSkillData.IsBowChargeAttacking = false;
                    return;
                }
                
                playerControllerInput.RaycastDirection();
                StartCoroutine(nameof(BowCharging));
                anim.SetBool(playerData.PlayerAnimation.AnimIsAttacking, true);
                playerData.CharacterSkillData.IsBowAttackFinish = false;
                playerData.CharacterSkillData.IsBowAttacking = true;
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                
                StartCoroutine(nameof(BowAttackDuration));
                RotationAttack();
            }
            else
            {
                Time.timeScale = 1;
                StopCoroutine(nameof(BowCharging));
                anim.SetBool(playerData.PlayerAnimation.AnimIsBowCharge, false);
            }
        }
        
        public void StopBowCharging()
        {
          StopCoroutine(nameof(BowCharging));
          playerData.IsBowChargeFull = false;
          Time.timeScale = 1;
        }
        
        private IEnumerator BowCharging()
        {
            yield return new WaitForSeconds(playerData.CharacterSkillData.BowChargeTime);
            vfxBowFullCharge.SetActive(true);
            playerData.IsBowChargeFull = true;
            Time.timeScale = 0.5f;
            Debug.Log("BowChargeFull");
        }
        
        private void BowAttack()
        {
            if (playerData.CharacterSkillData.IsBowChargeAttacking)
            {
                anim.SetBool(playerData.PlayerAnimation.AnimIsBowCharge, true);
            }
            else
            {
                anim.SetTrigger(playerData.PlayerAnimation.AnimBowAttack);
            }
        }

        private IEnumerator BowAttackDuration()
        {
            yield return new WaitUntil(() => playerData.CharacterSkillData.IsBowAttackFinish);
            yield return new WaitForSeconds(playerData.CharacterSkillData.BowAttackDuration.Cooldown[Index.Start]);
            playerData.CharacterSkillData.IsBowAttacking = false;
        }

        private void SwordAttack()
        {
            if (playerData.CharacterSkillData.CurrentCombo < playerData.Info.DeviceCompartment.SlotWeapon.Info.MaxCombo
                    [playerData.Info.DeviceCompartment.SlotWeapon.Info.LevelIndex])
            {
                anim.ResetTrigger(playerData.PlayerAnimation.AnimComboAttacks[playerData.CharacterSkillData.CurrentCombo]);
            }

            if (playerData.CharacterSkillData.CurrentCombo >= playerData.Info.DeviceCompartment.SlotWeapon.Info.MaxCombo
                    [playerData.Info.DeviceCompartment.SlotWeapon.Info.LevelIndex])
            {
                playerData.CharacterSkillData.CurrentCombo = 0;
            }
            
            RotationAttack();

            anim.SetTrigger(playerData.PlayerAnimation.AnimComboAttacks[playerData.CharacterSkillData.CurrentCombo]);

            playerData.CharacterSkillData.CurrentCombo++;
        }

        private void RotationAttack()
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            Quaternion rotation =
                Quaternion.LookRotation(new Vector3(playerData.CharacterControllerData.MouseDirection.x, 0,
                        playerData.CharacterControllerData.MouseDirection.z) - new Vector3(transform.position.x, 0,
                        transform.position.z),
                    Vector3.up);
            
            playerData.CharacterSkillData.RotationAttackSequence = DOTween.Sequence();
            if (!playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                switch (weapon.WeaponType)
                {
                    case WeaponTypes.Sword:
                    case WeaponTypes.Knuckle:
                        playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, 0.2f)
                            .SetEase(Ease.Linear));
                        break;
                    case WeaponTypes.Gun:
                        playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, 0.2f)
                            .SetEase(Ease.Linear)).OnComplete(GunShootBullet);
                        break;
                }
            }
            else
            {
                if (weapon.WeaponType == WeaponTypes.Sword)
                {
                    playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, playerData.CharacterSkillData.BowAttackRotationSpeed)
                        .SetEase(Ease.Linear)).OnComplete(BowAttack);
                }
                else if (weapon.WeaponType == WeaponTypes.Knuckle)
                {
                    playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, playerData.CharacterSkillData.CannonAttackRotationSpeed)
                        .SetEase(Ease.Linear));
                }
                else if (weapon.WeaponType == WeaponTypes.Gun)
                {
                    playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, 0.2f)
                        .SetEase(Ease.Linear));
                }
            }
        }

        #region USE_WITH_ANIMATION_CLIP
        
        public void NextCombo()
        {
            playerData.PlayerAnimation.IsAttacking = false;
        }

        public void SpawnGroundSlash()
        {
            var weapon = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword);
            
            PoolManager.PoolObjectType poolObj = weapon.WeaponComponentLevel[2].UpgradeComponent2 
                ? PoolManager.PoolObjectType.GroundSlashLv2 : PoolManager.PoolObjectType.GroundSlashLv1;

            var groundSlash = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (groundSlash)
            {
                groundSlash.transform.position = spawnGroundSlash.position;
                groundSlash.transform.rotation = Quaternion.identity;
                groundSlash.SetActive(true);
                groundSlash.GetComponent<GroundSlash>().Init(spawnGroundSlash.forward);
            }
        }
        
        public void SpawnArrow()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.Arrow;

            var arrow = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (arrow)
            {
                arrow.transform.position = spawnArrow.position;
                arrow.transform.rotation = Quaternion.identity;
                arrow.GetComponent<Arrow.Arrow>().Init(spawnArrow.forward, playerData.IsBowChargeFull);
                arrow.SetActive(true);
            }
        }

        #endregion
        
        public void ResetCombo()//TODO:RESET
        {
            playerData.IsInteract = false;
            if (!playerData.CharacterSkillData.IsChangeWeaponFinish)
            {
               ChangeWeapon();
            }

            StopBowCharging();
            
            playerData.CharacterSkillData.ChangeWeaponFinish = true;
            playerData.CharacterSkillData.IsBowAttackFinish = true;
            playerData.CharacterSkillData.IsUseActiveSkill = false;
            playerData.CharacterSkillData.IsUseUltimateSkill = false;
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.CurrentCombo = 0;
            playerData.CharacterSkillData.IsBowCharging = false;
            playerData.CharacterSkillData.PunchFinish = true;

            anim.SetBool(playerData.PlayerAnimation.AnimIsBowCharge, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsChangeWeapon, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsAttacking, false);
            anim.ResetTrigger(playerData.PlayerAnimation.AnimGetDamage);
            anim.SetBool(playerData.PlayerAnimation.AnimIsInteract, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsGuardCounter, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsAim, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsGrenadeToss, false);

            ResetComboAttack();
            ResetColliderDamage();
            
            if (playerData.CharacterSkillData.UsingGuard)
            {
                GuardFinish();
            }
            
            playerEvent.CloseVfx();
            playerEvent.CloseSpine();
        }

        public void ResetComboAttack()
        {
            for (int i = 0; i < playerData.PlayerAnimation.AnimComboAttacks.Length; i++)
            {
                anim.ResetTrigger(playerData.PlayerAnimation.AnimComboAttacks[i]);
            }
            
            anim.ResetTrigger(playerData.PlayerAnimation.AnimAttackHeavyPunch);
            anim.ResetTrigger(playerData.PlayerAnimation.AnimAttackKnuckleLeft);
            anim.ResetTrigger(playerData.PlayerAnimation.AnimAttackKnuckleRight);
            anim.ResetTrigger(playerData.PlayerAnimation.AnimCannonAttack);
            anim.ResetTrigger(playerData.PlayerAnimation.AnimGunAttack);
            
            CloseLaser();

            HideVfxSpear();
        }

        public void ResetColliderDamage()
        {
            foreach (var playerDamageCollier in playerEvent.PlayerDamageColliers)
            {
                playerDamageCollier.gameObject.SetActive(false);
            }
        }

        internal void SwordPowerNormalAttack()
        {
            
        }

        #region LV2

        internal void SwordActiveSkillLv2Component1()
        {
            SwordAttackSpin();
        }

        private void SwordAttackSpin()
        {
            if (playerData.CharacterSkillData.IsUseUltimateSkill || playerData.CharacterSkillData.IsUseSkillPowerStone) return;

            if (isSwordActiveSkillComponent1Cooldown) return;
            
            //StartCoroutine(nameof(CooldownSwordActiveSkillComponent1));
            CooldownSwordActiveSkillComponent1Action?.Invoke();
            playerData.CharacterSkillData.IsUseActiveSkill = true;
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = true;
            anim.SetBool(playerData.PlayerAnimation.AnimIsAttackSpin, true);
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
        }
        
        /*private IEnumerator CooldownSwordActiveSkillComponent1()
        {
            isSwordActiveSkillComponent1Cooldown = true;
            yield return new WaitForSeconds(
                playerData.CharacterSkillData.SkillSwordComponent1Cooldown.Cooldown
                [playerData.Info.DeviceCompartment.SlotWeapon.Info.LevelIndex - 1]);
            isSwordActiveSkillComponent1Cooldown = false;
        }*/
        
        internal void SwordKeyStoneSkillLv2Component2()
        {
            if (!IsCanUseSkillUltimate()) return;

            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            ResetComboAttack();
            ResetColliderDamage();
            
            if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                RotationDelta();
                //StartCoroutine(nameof(CooldownSwordUltimateSkill));
                CooldownSwordUltimateSkillAction?.Invoke();
               
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimSkillRedStone);
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            Debug.Log("SwordKeyStoneSkill (Lv2) Component2");
        }

        private void RotationDelta()
        {
            Quaternion rotation =
                Quaternion.LookRotation(new Vector3(playerData.CharacterControllerData.MouseDirection.x, 0,
                        playerData.CharacterControllerData.MouseDirection.z) - new Vector3(transform.position.x, 0,
                        transform.position.z),
                    Vector3.up);
            
            transform.DORotate(rotation.eulerAngles, 0.2f).SetEase(Ease.Linear);
        }

        private bool IsCanUseSkillUltimate()
        {
            if (playerData.CharacterControllerData.Dashing) return false;

            if (playerData.CharacterSkillData.IsUseActiveSkill ||
                playerData.CharacterSkillData.IsUseUltimateSkill) return false;

            if (isSwordUltimateSkillCooldown) return false;
            return true;
        }
        
        private bool IsCanUseSkillUltimateKnuckle()
        {
            if (playerData.CharacterControllerData.Dashing) return false;

            if (playerData.CharacterSkillData.IsUseActiveSkill ||
                playerData.CharacterSkillData.IsUseUltimateSkill) return false;

            if (isKnuckleUltimateSkillCooldown) return false;
            return true;
        }
        
        private bool IsCanUseSkillUltimateGun()
        {
            if (playerData.CharacterControllerData.Dashing) return false;

            if (playerData.CharacterSkillData.IsUseActiveSkill ||
                playerData.CharacterSkillData.IsUseUltimateSkill) return false;

            if (isGunUltimateSkillCooldown) return false;
            return true;
        }

        /*private IEnumerator CooldownSwordUltimateSkill()
        {
            isSwordUltimateSkillCooldown = true;
            yield return new WaitForSeconds(
                playerData.CharacterSkillData.SkillSwordRedStoneCooldown.Cooldown
                    [playerData.Info.DeviceCompartment.SlotWeapon.Info.LevelIndex - 1]);
            isSwordUltimateSkillCooldown = false;
        }*/
        
        /*private IEnumerator CooldownSwordPowerStoneSkill()
        {
            isSwordUltimateSkillCooldown = true;
            yield return new WaitForSeconds(
                playerData.CharacterSkillData.SkillSwordPowerStoneCooldown.Cooldown[Index.Start]);
            isSwordUltimateSkillCooldown = false;
        }*/

        internal void SwordPassiveSkillLv2Component3()
        {
            
        }

        #endregion
        
        #region LV3

        internal void SwordActiveSkillLv3Component1()
        {
            SwordAttackSpin();
        }

        internal void SwordKeyStoneSkillLv3Component2()
        {
            if (!IsCanUseSkillUltimate()) return;
            
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            ResetComboAttack();
            ResetColliderDamage();
            
            if (weapon.UsedKeyStone == KeyStones.PowerStone)
            {
                //StartCoroutine(nameof(CooldownSwordPowerStoneSkill));
                playerData.CharacterSkillData.IsChangeWeaponKeyStone = true;
                CooldownSwordPowerStoneSkillAction?.Invoke();
                ResetComboAttack();
                ResetColliderDamage();
                playerEvent.CloseVfx();

                ChangeWeaponReset();
                
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                RotationDelta();
                //StartCoroutine(nameof(CooldownSwordUltimateSkill));
                CooldownSwordUltimateSkillAction?.Invoke();
                
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimSkillRedStone);
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            Debug.Log("SwordKeyStoneSkill (Lv3) Component2");
        }
        
        internal void SwordPassiveSkillLv3Component3()
        {
            
        }

        #endregion

        #endregion

        #region SKILL_GUN
        //TODO:Doing
        internal void GunNormalAttack()
        {
            playerControllerInput.RaycastDirection();
            
            if (playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                if (playerData.PlayerAnimation.IsAttacking) return;
                
                RotationAttack();
                positionFallBullet = indicatorCannonAttackPosition.gameObject.transform.position;
                anim.SetTrigger(playerData.PlayerAnimation.AnimAttackSpear);
                anim.SetBool(playerData.PlayerAnimation.AnimIsAttacking, true);
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            }
            else
            {
                GunAttack();
            }
        }

        private void GunAttack()
        {
            if (isGunAttackCooldown) return;
            
            CooldownGunAttack?.Invoke();
            
            RotationAttack();
        }

        private void GunShootBullet()
        {
            anim.SetTrigger(playerData.PlayerAnimation.AnimGunAttack);
        }

        public void ShootBullet()
        {
            var weapon = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Gun);

            int spawnIndex = weapon.LevelIndex;
            vfxShootBulletGun[spawnIndex].SetActive(true);
            PoolManager.PoolObjectType poolObj;

            if (!weapon.WeaponComponentLevel[2].UpgradeComponent3)
            {
                poolObj = weapon.WeaponComponentLevel[1].UpgradeComponent3 
                    ? PoolManager.PoolObjectType.GunBulletLv2 : PoolManager.PoolObjectType.GunBulletLv1;
            }
            else
            {
                poolObj = PoolManager.PoolObjectType.GunBulletLv3;
            }

            var bullet = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (bullet)
            {
                bullet.transform.position = spawnBulletGun[spawnIndex].position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<GunBullet>().Init(spawnBulletGun[spawnIndex].forward, weapon.UsedKeyStone,
                    playerData.CharacterControllerData.Aiming);
                bullet.SetActive(true);
            }
        }
        
        #region LV2

        internal void GunActiveSkillLv2Component1()
        {
            GrenadeToss();
        }

        internal void GunKeyStoneSkillLv2Component2()
        {
            if (!IsCanUseSkillUltimateGun()) return;
            
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            ResetComboAttack();
            ResetColliderDamage();
            
            if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                UseSkillGunRedStone = false;
                RotationDelta();
                CooldownGunUltimateSkillAction?.Invoke();
                
                Quaternion rotation =
                    Quaternion.LookRotation(new Vector3(playerData.CharacterControllerData.MouseDirection.x, 0,
                            playerData.CharacterControllerData.MouseDirection.z) - new Vector3(transform.position.x, 0,
                            transform.position.z),
                        Vector3.up);
            
                playerData.CharacterSkillData.RotationAttackSequence = DOTween.Sequence();
                
                playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, 0.2f)
                    .SetEase(Ease.Linear)).OnComplete(GunGreenStoneAttack);
                
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                UseSkillGunRedStone = true;
                LaserAttack(1);
                //StartCoroutine(nameof(CooldownSwordUltimateSkill));
                CooldownGunUltimateSkillAction?.Invoke();
                playerData.PlayerAnimation.IsAttacking = true;
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                playerData.CharacterSkillData.IsUseSkillCanNotAttack = true;
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            Debug.Log("SwordKeyStoneSkill (Lv2) Component2");
        }

        private void GunGreenStoneAttack()
        {
            playerData.CharacterSkillData.IsUseUltimateSkill = true;
            anim.SetTrigger(playerData.PlayerAnimation.AnimSkillGreenStone);
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
        }

        private void LaserAttack(int lv)
        {
            anim.SetBool(playerData.PlayerAnimation.AnimIsAim, true);
            switch (lv)
            {
                case 1:
                    laserLv1.SetActive(true);
                    break;
                case 2:
                    laserLv2.SetActive(true);
                    break;
            }

            isRotation = true;
            StartCoroutine(nameof(LaserTime));
            StartCoroutine(nameof(Rotation));
        }

        private IEnumerator LaserTime()
        {
            playerEvent.SfxGunLaserAttack();
            yield return new WaitForSeconds(laserTime);
            playerEvent.SfxGunLaserAttackEnd();
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = false;
            isRotation = false;
            playerData.CharacterSkillData.IsUseUltimateSkill = false;
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, true);
            anim.SetBool(playerData.PlayerAnimation.AnimIsAim, playerData.CharacterControllerData.Aiming);

            ResetComboAttack();
        }

        private IEnumerator Rotation()
        {
            while (isRotation)
            {
                if (!playerData.IsStop)
                {
                    anim.SetBool(playerData.PlayerAnimation.AnimIsAim, true);
                    playerData.PlayerAnimation.IsAttacking = true;
                    playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                    playerData.CharacterSkillData.IsUseSkillCanNotAttack = true;
                    playerControllerInput.RaycastDirection();
                    RotationDelta();
                }
                yield return null;
            }
            
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = false;
            anim.SetBool(playerData.PlayerAnimation.AnimIsAim, playerData.CharacterControllerData.Aiming);
        }
        
        internal void GunPassiveSkillLv2Component3()
        {
            
        }

        #endregion

        #region LV3

        internal void GunActiveSkillLv3Component1()
        {
            GrenadeToss();
        }

        private void GrenadeToss()
        {
            if (playerData.CharacterSkillData.IsUseUltimateSkill || playerData.CharacterSkillData.IsUseSkillPowerStone) return;

            if (isGunActiveSkillComponent1Cooldown) return;
           
            playerControllerInput.RaycastDirection();
            positionFallBullet = playerData.CharacterControllerData.MouseDirection;
            RotationDelta();
            
            CooldownGunActiveSkillComponent1Action?.Invoke();
            playerData.CharacterSkillData.IsUseActiveSkill = true;
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = true;
            anim.SetBool(playerData.PlayerAnimation.AnimIsGrenadeToss, true);
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
        }

        public void SpawnGrenade()
        {
            var weapon = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Gun);
            
            PoolManager.PoolObjectType poolObj = weapon.WeaponComponentLevel[2].UpgradeComponent1 
                ? PoolManager.PoolObjectType.GrenadeLv2 : PoolManager.PoolObjectType.GrenadeLv1;

            var grenade = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (grenade)
            {
                Quaternion rotation = Quaternion.LookRotation(spawnGrenade.forward);
                grenade.transform.position = spawnGrenade.position;
                grenade.transform.rotation = Quaternion.Lerp(grenade.transform.rotation, rotation, 1);
                grenade.GetComponent<Grenade>().Init(positionFallBullet, spawnGrenade.position, poolObj);
                grenade.SetActive(true);
            }
        }

        internal void GunKeyStoneSkillLv3Component2()
        {
            if (!IsCanUseSkillUltimateGun()) return;
            
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            ResetComboAttack();
            ResetColliderDamage();
            
            if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                UseSkillGunRedStone = false;
                RotationDelta();
                CooldownGunUltimateSkillAction?.Invoke();
                
                Quaternion rotation =
                    Quaternion.LookRotation(new Vector3(playerData.CharacterControllerData.MouseDirection.x, 0,
                            playerData.CharacterControllerData.MouseDirection.z) - new Vector3(transform.position.x, 0,
                            transform.position.z), Vector3.up);
            
                playerData.CharacterSkillData.RotationAttackSequence = DOTween.Sequence();
                
                playerData.CharacterSkillData.RotationAttackSequence.Append(transform.DORotate(rotation.eulerAngles, 0.2f)
                    .SetEase(Ease.Linear)).OnComplete(GunGreenStoneAttack);
                
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.PowerStone)
            {
                if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;
                
                playerData.CharacterSkillData.IsChangeWeaponKeyStone = true;
                CooldownGunPowerStoneSkillAction?.Invoke();
                ResetComboAttack();
                ResetColliderDamage();
                playerData.CharacterSkillData.ResetComboKnuckle();
                
                playerEvent.CloseVfx();
                ChangeWeaponReset();
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                UseSkillGunRedStone = true;
                LaserAttack(2);
                //StartCoroutine(nameof(CooldownSwordUltimateSkill));
                CooldownGunUltimateSkillAction?.Invoke();
                playerData.PlayerAnimation.IsAttacking = true;
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                playerData.CharacterSkillData.IsUseSkillCanNotAttack = true;
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            Debug.Log("SwordKeyStoneSkill (Lv2) Component2");
        }

        public void ShootBulletGreenStone()
        {
            var weapon = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Gun);
            
            int spawnIndex = weapon.LevelIndex;
            vfxShootBulletGun[spawnIndex].SetActive(true);
            
            PoolManager.PoolObjectType poolObj = weapon.WeaponComponentLevel[2].UpgradeComponent2 
                ? PoolManager.PoolObjectType.BulletGunGreenStoneLv2 : PoolManager.PoolObjectType.BulletGunGreenStoneLv1;
            
            var bullet = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (bullet)
            {
                bullet.transform.position = spawnBulletGun[spawnIndex].position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<BulletGreenStone>().Init(spawnBulletGun[spawnIndex].forward, poolObj);
                bullet.SetActive(true);
            }
        }
        
        internal void GunPassiveSkillLv3Component3()
        {
            
        }

        #endregion
        
        #endregion
        
        #region SKILL_KNUCKLE
       
        internal void KnuckleNormalAttack()
        {
            playerControllerInput.RaycastDirection();
            
            if (playerData.CharacterSkillData.IsUseSkillPowerStone)
            {
                if (isCannonAttackCooldown) return;
                
                CooldownKnuckleCannonAttack?.Invoke();
                
                RotationAttack();
                positionFallBullet = indicatorCannonAttackPosition.gameObject.transform.position;
                anim.SetTrigger(playerData.PlayerAnimation.AnimCannonAttack);
                anim.SetBool(playerData.PlayerAnimation.AnimIsAttacking, true);
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                StartCoroutine(nameof(BowAttackDuration));
            }
            else
            {
                KnuckleAttack();
            }
        }

        private void KnuckleAttack()
        {
            cooldownManager.StopPunchWaitTime();
            
            RotationAttack();
            
            if (playerData.CharacterSkillData.KnuckleLeft)
            {
                anim.SetTrigger(playerData.PlayerAnimation.AnimAttackKnuckleLeft);
            }
            else if (playerData.CharacterSkillData.KnuckleRight)
            {
                anim.SetTrigger(playerData.PlayerAnimation.AnimAttackKnuckleRight);
            }
            else if (playerData.CharacterSkillData.KnuckleHeavyPunch)
            {
                anim.SetTrigger(playerData.PlayerAnimation.AnimAttackHeavyPunch);
            }
        }

        #region LV2
       
        private void KnuckleGuard()
        {
            if (playerData.CharacterSkillData.UsingGuard) return;
            
            if (playerData.CharacterSkillData.IsUseUltimateSkill || playerData.CharacterSkillData.IsUseSkillPowerStone) return;

            if (isKnuckleActiveSkillComponent1Cooldown) return;
            isKnuckleActiveSkillComponent1Cooldown = true;
            playerData.CharacterSkillData.ResetComboKnuckle();
            CooldownKnuckleActiveSkillComponent1Action?.Invoke();
            playerData.CharacterSkillData.UsingGuard = true;
            playerData.CharacterSkillData.IsUseActiveSkill = true;
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = true;
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
            anim.SetBool(playerData.PlayerAnimation.AnimIsGuard, true);
            StartCoroutine(nameof(GuardTime));
        }

        private IEnumerator GuardTime()
        {
            playerEvent.VfxShield.SetActive(true);
            playerEvent.SfxKnuckleGuardCounter();
            
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);
            
            float guardTime = weapon.WeaponComponentLevel[2].UpgradeComponent1
                ? playerData.CharacterSkillData.GuardTime.Last()
                : playerData.CharacterSkillData.GuardTime.First();

            yield return new WaitForSeconds(guardTime);
            
            GuardFinish();
        }

        internal void GuardCounter()
        {
            StopCoroutine(nameof(GuardTime));
            playerControllerInput.RaycastDirection();
            RotationAttack();
            anim.SetBool(playerData.PlayerAnimation.AnimIsGuardCounter, true);
            anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
            GuardFinish();
        }
        
        private void GuardFinish()
        {
            playerEvent.VfxShield.SetActive(false);
            playerData.CharacterSkillData.UsingGuard = false;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = false;
            anim.SetBool(playerData.PlayerAnimation.AnimIsGuard, false);
        }

        internal void KnuckleActiveSkillLv2Component1()
        {
            KnuckleGuard();
        }

        internal void KnuckleKeyStoneSkillLv2Component2()
        {
            if (!IsCanUseSkillUltimateKnuckle()) return;

            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            ResetComboAttack();
            ResetColliderDamage();
            
            if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                UseSkillKnuckleRedStone = true;
                RotationDelta();
                CooldownKnuckleUltimateSkillAction?.Invoke();
                
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimSkillRedStone);
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                UseSkillKnuckleRedStone = false;
                RotationDelta();
                CooldownKnuckleUltimateSkillAction?.Invoke();
                
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimSkillGreenStone);
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
        }
        
        internal void KnucklePassiveSkillLv2Component3()
        {
            
        }

        #endregion

        #region LV3

        internal void KnuckleActiveSkillLv3Component1()
        {
            KnuckleGuard();
        }

        internal void KnuckleKeyStoneSkillLv3Component2()
        {
            if (!IsCanUseSkillUltimateKnuckle()) return;
            Debug.Log("CANNON");
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            ResetComboAttack();
            ResetColliderDamage();
            
            if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                UseSkillKnuckleRedStone = true;
                RotationDelta();
                CooldownKnuckleUltimateSkillAction?.Invoke();
                
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimSkillRedStone);
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                UseSkillKnuckleRedStone = false;
                RotationDelta();
                CooldownKnuckleUltimateSkillAction?.Invoke();
                
                playerData.CharacterSkillData.IsUseUltimateSkill = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimSkillGreenStone);
                anim.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
            else if (weapon.UsedKeyStone == KeyStones.PowerStone)
            {
                if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;

                Debug.Log("CANNON2");
                playerData.CharacterSkillData.IsChangeWeaponKeyStone = true;
                CooldownKnucklePowerStoneSkillAction?.Invoke();
                ResetComboAttack();
                ResetColliderDamage();
                playerData.CharacterSkillData.ResetComboKnuckle();
                
                playerEvent.CloseVfx();
                ChangeWeaponReset();
                Debug.Log($"Use skill key stone = {weapon.UsedKeyStone}");
            }
        }

        internal void KnucklePassiveSkillLv3Component3()
        {
            
        }
        
        public void ShowVfxSpear()
        {
            vfxSpear.SetActive(true);
        }
        
        public void HideVfxSpear()
        {
            vfxSpear.SetActive(false);
        }

        public void FireBulletCannon()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.BulletCannon;
            
            var bulletCannon = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (bulletCannon)
            {
                Quaternion rotation = Quaternion.LookRotation(spawnBulletCannon.forward);
                bulletCannon.transform.position = spawnBulletCannon.position;
                bulletCannon.transform.rotation = Quaternion.Lerp(bulletCannon.transform.rotation, rotation, 1);
                bulletCannon.GetComponent<BulletCannon>()
                    .Init(positionFallBullet, spawnBulletCannon.position);
                bulletCannon.SetActive(true);
            }
        }
        
        #endregion
        
        #endregion
    }
}