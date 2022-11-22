using System;
using System.Collections;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Animation;
using Kamaria.Player.Controller;
using Kamaria.UI.UIMainGame;
using Kamaria.UIDamage;
using Kamaria.Utilities;
using Kamaria.Utilities.PoolingPattern;
using Kamaria.VFX_ALL;
using UnityEngine;

namespace Kamaria.Player.Data
{
    public sealed class PlayerEvent : MonoBehaviour, IDamageable, IInteractable
    {
        [SerializeField] private CapsuleCollider colliderInteract;
        [SerializeField] private RectTransform uiInteract;
        [SerializeField] private RectTransform uiInteractTreasure;
        [SerializeField] private UIGamePlay uiGamePlay;
        [SerializeField] private BlinkEffect blinkEffect;
        [SerializeField] private PlayerCooldownManager cooldownManager;
        [SerializeField] private GameManagerFarming gameManagerFarming;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private CharacterSkill characterSkill;
        [SerializeField] private Rigidbody controllerRb;
        [SerializeField] private List<PlayerDamageCollier> playerDamageColliers;
        [SerializeField] private float deadTime = 1.8f;
        [Space] 
        [SerializeField] private GameObject vfxDash;
        [SerializeField] private GameObject vfxChangeWeapon;
        [SerializeField] private GameObject vfxLine;
        [SerializeField] private GameObject vfxStun;
        [SerializeField] private GameObject vfxSpawn;

        [Header("VFX Sword")] 
        [SerializeField] private GameObject[] vfxSlash;
        [SerializeField] private GameObject[] vfxSlashRedStone;
        [SerializeField] private GameObject[] vfxSlashGreenStoneLV1;
        [SerializeField] private GameObject[] vfxSlashGreenStoneLV2;
        [SerializeField] private GameObject vfxSpine;
        [SerializeField] private GameObject vfxSpineRedStone;
        [SerializeField] private GameObject vfxSpineGreenStoneLV1;
        [SerializeField] private GameObject vfxSpineGreenStoneLV2;
        [Header("VFX Knuckle")] 
        [SerializeField] private GameObject[] vfxPunch;
        [SerializeField] private GameObject[] vfxPunchRed;
        [SerializeField] private GameObject[] vfxPunchGreen;
        [SerializeField] private GameObject vfxPunchGreenStone;
        [SerializeField] private Transform spawnKnuckleSkillRedStone;
        [SerializeField] private GameObject vfxShield;
        [Space] 
        [SerializeField] private List<WeaponObject> weaponObjects = new List<WeaponObject>();

        private Animator anim;
        private float stunTime;
        private PlayerDamageCollier playerDamageCollier;

        public GameObject VfxShield => vfxShield;
        public List<PlayerDamageCollier> PlayerDamageColliers => playerDamageColliers;
        public List<WeaponObject> WeaponObjects => weaponObjects;

        private void OnEnable()
        {
            StartCoroutine(CheckPauseSound());
            playerData.CharacterControllerData.MovementInput = Vector2.zero;
            
            colliderInteract.enabled = true;
            uiInteract.gameObject.SetActive(false);
            uiInteractTreasure.gameObject.SetActive(false);
            if (!playerData.TelePort)
            {
                Debug.Log($"UIInitialize = {playerData.TelePort}");
                uiGamePlay.UIInitialize();
            }
            playerData.PlayerAnimation.Animator = GetComponent<Animator>();
            playerData.SetAnimator(out anim);
            vfxSpawn.SetActive(true);
            playerData.Initialized();
            controllerRb.isKinematic = false;
            OpenWeaponObj();
            playerData.PlayerDamageColliers = playerDamageColliers;
        }

        private IEnumerator CheckPauseSound()
        {
            while (true)
            {
                yield return new WaitUntil((() => Time.timeScale == 0));
                SoundHandler.Instance.AllSoundPause();
                yield return new WaitUntil((() => Time.timeScale == 1));
                SoundHandler.Instance.AllSoundUnPause();
            }
        }

        private void OnDisable()
        {
            colliderInteract.enabled = false;
            if (!playerData.TelePort)
            {
                playerData.Info.DeviceCompartment.SlotWeapon.Remove(playerData.Info.DeviceCompartment.SlotWeapon.Info,
                    playerData);
            }
        }

        public void OpenWeaponPowerObj(WeaponTypes weaponTypes)
        {
            CloseWeaponObj();
            
            var weaponsObj = weaponObjects.Find(x => x.WeaponType == weaponTypes);

            for (int i = 0; i < weaponsObj.WeaponsPower.Count; i++)
            {
                weaponsObj.WeaponsPower[i].SetActive(true);
            }
            
            switch (weaponTypes)
            {
                case WeaponTypes.Sword:
                    playerData.PlayerAnimation.UsingAnimWeapon(PlayerAnimations.isUsingBow);
                    break;
                case WeaponTypes.Gun:
                    playerData.PlayerAnimation.UsingAnimWeapon(PlayerAnimations.isUsingSpear);
                    break;
                case WeaponTypes.Knuckle:
                    playerData.PlayerAnimation.UsingAnimWeapon(PlayerAnimations.isUsingCannon);
                    break;
            }
        }
        
        public void OpenWeaponObj()
        {
            CloseWeaponObj();
            
            var weaponData = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            var weaponsObj = weaponObjects.Find(x => x.WeaponType == weaponData.WeaponType);

            weaponsObj.WeaponsLv[weaponData.LevelIndex].SetActive(true);

            switch (weaponData.WeaponType)
            {
                case WeaponTypes.Sword:
                    playerData.PlayerAnimation.UsingAnimWeapon(PlayerAnimations.isUsingSword);
                    break;
                case WeaponTypes.Gun:
                    playerData.PlayerAnimation.UsingAnimWeapon(PlayerAnimations.isUsingGun);
                    break;
                case WeaponTypes.Knuckle:
                    playerData.PlayerAnimation.UsingAnimWeapon(PlayerAnimations.isUsingKnuckle);
                    break;
            }
        }

        private void CloseWeaponObj()
        {
            for (int i = 0; i < weaponObjects.Count; i++)
            {
                for (int j = 0; j < weaponObjects[i].WeaponsLv.Count; j++)
                {
                    weaponObjects[i].WeaponsLv[j].SetActive(false);
                }

                for (int k = 0; k < weaponObjects[i].WeaponsPower.Count; k++)
                {
                    weaponObjects[i].WeaponsPower[k].SetActive(false);
                }
                
            }
        }

        private void Start()
        {
            anim = playerData.PlayerAnimation.Animator;
        }

        public void TakeDamage(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime, WeaponTypes weaponTypes, KeyStones keyStones)
        {
            if (playerData.CharacterControllerData.Dashing || playerData.IsDead || playerData.CharacterSkillData.UsingGuard) return;
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            Debug.Log($"Get effectAttack = {effectAttack}");
            int tempDamage = damage;
            
            if (weapon.WeaponType == WeaponTypes.Knuckle)
            {
                if (weapon.WeaponComponentLevel[0].UpgradeComponent2)
                {
                    tempDamage -= (int)((damage * weapon.ReductionDamage[Index.Start]) / 100);
                }
            }
            
            playerData.Info.Status.CurrentHealth -= tempDamage;
            
            blinkEffect.Blink();
            ShowTextDamage(tempDamage);
            uiGamePlay.UpdateUIHp();
            
            if (playerData.Info.Status.CurrentHealth <= 0)
            {
                SfxNotWalk();
                Dead();
                return;
            }

            TakeDamageBehavior(effectAttack, explosionPos, powerKnockback, radiusKnockback, isAOE, stunTime);
        }

        private void Dead()
        {
            uiInteract.gameObject.SetActive(false);
            uiInteractTreasure.gameObject.SetActive(false);
            colliderInteract.enabled = false;
            characterSkill.StopBowCharging();
            playerData.IsDead = true;
            characterSkill.ResetCombo();
            ShowVfxStun(false);

            anim.SetBool(playerData.PlayerAnimation.AnimIsDead, true);
            controllerRb.isKinematic = true;
            StartCoroutine(DeadTime());
            playerData.TelePort = false;
        }

        private IEnumerator DeadTime()
        {
            yield return new WaitForSeconds(deadTime);
            cooldownManager.CancelCooldown();
            gameManagerFarming.ResetGame();//TODO
            gameObject.SetActive(false);
        }

        private void TakeDamageBehavior(EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime)
        {
            if (effectAttack == EffectAttack.KnockBack)
            {
                playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
                anim.SetTrigger(playerData.PlayerAnimation.AnimGetDamage);
                
                if (isAOE)
                {
                    controllerRb.AddExplosionForce(powerKnockback, explosionPos, radiusKnockback, 0.0f,
                        ForceMode.VelocityChange);
                }
                else
                {
                    Vector3 direction = this.transform.position - explosionPos;
                    direction.y = 0;

                    controllerRb.AddForce(direction * powerKnockback, ForceMode.VelocityChange);
                }
            }
            else if (effectAttack == EffectAttack.Stun)
            {
                characterSkill.StopBowCharging();
                Debug.Log("STUN");
                this.stunTime = stunTime;
                StopCoroutine(nameof(StunTime));
                StartCoroutine(nameof(StunTime));
            }
        }

        private IEnumerator StunTime()
        {
            Time.timeScale = 1;
            characterSkill.ResetCombo();
            playerData.IsStun = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            anim.SetBool(playerData.PlayerAnimation.AnimIsStun, true);
            ShowVfxStun(true);

            yield return new WaitForSeconds(stunTime);
            
            characterSkill.ResetCombo();
            playerData.IsStun = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            anim.SetBool(playerData.PlayerAnimation.AnimIsStun, false);
            ShowVfxStun(false);
        }
        
        #region USE_WITH_ANIMATION

        public void PlayerOpenDamageCollider(int damageCollider)
        {
            SetDamageCollider(damageCollider, true);
        }

        public void PlayerCloseDamageCollider(int damageCollider)
        {
            SetDamageCollider(damageCollider, false);
        }

        #endregion
        
        private void SetDamageCollider(int damageCollider, bool isOpen)
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            switch (damageCollider)
            {
                case 0:
                    ColliderDamageSwordCombo(weapon, CollidersDamage.BladeColliderCombo1,
                        CollidersDamage.BladeColliderCombo1GreenStoneLV1,
                        CollidersDamage.BladeColliderCombo1GreenStoneLV2);
                    break;
                case 1:
                    ColliderDamageSwordCombo(weapon, CollidersDamage.BladeColliderCombo2,
                        CollidersDamage.BladeColliderCombo2GreenStoneLV1,
                        CollidersDamage.BladeColliderCombo2GreenStoneLV2);
                    break;
                case 2:
                    ColliderDamageSwordCombo(weapon, CollidersDamage.BladeColliderCombo3,
                        CollidersDamage.BladeColliderCombo3GreenStoneLV1,
                        CollidersDamage.BladeColliderCombo3GreenStoneLV2);
                    break;
                case 3:
                    playerDamageCollier = playerDamageColliers.Find
                        (x => x.PlayerDamage.CollidersDamage == ColliderDamageSwordCombo4(weapon));
                    break;
                case 4:
                    playerDamageCollier = playerDamageColliers.Find
                        (x => x.PlayerDamage.CollidersDamage == ColliderDamageSwordSpin(weapon));
                    break;
                case 5:
                    ColliderKnuckleCombo(weapon, CollidersDamage.KnuckleComboLeft);
                    break;
                case 6:
                    ColliderKnuckleCombo(weapon, CollidersDamage.KnuckleComboRight);
                    break;
                case 7:
                    ColliderKnuckleCombo(weapon, CollidersDamage.KnuckleComboHeavyPunchNormal);
                    break;
                case 8:
                    ColliderKnuckleCombo(weapon, CollidersDamage.KnuckleGuardCounter);
                    break;
                case 9:
                    ColliderKnuckleSkillKeyStone(weapon, CollidersDamage.KnuckleSkillRedStoneLV1,
                        CollidersDamage.KnuckleSkillRedStoneLV2);
                    break;
                case 10:
                    ColliderKnuckleSkillKeyStone(weapon, CollidersDamage.KnuckleSkillGreenStoneLV1,
                        CollidersDamage.KnuckleSkillGreenStoneLV2);
                    break;
                case 11:
                    playerDamageCollier = playerDamageColliers.Find
                        (x => x.PlayerDamage.CollidersDamage == CollidersDamage.Spear);
                    break;
            }

            playerData.SetDataAttack(isOpen, playerDamageCollier, out playerDamageCollier.damage,
                out playerDamageCollier.effectAttack, out playerDamageCollier.weaponTypes, out playerDamageCollier.keyStones);
        }
        
        #region SWORD
        
        #region COLLIDER

        private void ColliderKnuckleSkillKeyStone(BaseItem weapon, CollidersDamage lv1, CollidersDamage lv2)
        {
            if (weapon.WeaponType != WeaponTypes.Knuckle)
            {
                Debug.Log($"Weapon not Knuckle! : {weapon.WeaponType}!");
                return;
            }

            playerDamageCollier = weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                ? playerDamageColliers.Find(x => x.PlayerDamage.CollidersDamage == lv2)
                : playerDamageColliers.Find(x => x.PlayerDamage.CollidersDamage == lv1);
        }
        
        private void ColliderKnuckleCombo(BaseItem weapon, CollidersDamage collidersDamage)
        {
            if (weapon.WeaponType != WeaponTypes.Knuckle)
            {
                Debug.Log($"Weapon not Knuckle! : {weapon.WeaponType}!");
                return;
            }

            if (collidersDamage is CollidersDamage.KnuckleComboHeavyPunchNormal or CollidersDamage.KnuckleComboHeavyPunchStun )
            {
                if (weapon.WeaponComponentLevel[0].UpgradeComponent1)
                {
                    collidersDamage = CollidersDamage.KnuckleComboHeavyPunchStun;
                }
            }

            playerDamageCollier = playerDamageColliers.Find(x =>
                x.PlayerDamage.CollidersDamage == collidersDamage);
        }
        
        private CollidersDamage ColliderDamageSwordSpin(BaseItem weapon)
        {
            CollidersDamage result = CollidersDamage.BladeColliderSpinLV1;
            
            if (weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent1)
            {
                result = weapon.UsedKeyStone == KeyStones.WindStone
                    ? weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                        ? CollidersDamage.BladeColliderSpinLV2GreenStoneLV2
                        : CollidersDamage.BladeColliderSpinLV2GreenStoneLV1
                    : CollidersDamage.BladeColliderSpinLV2;
            }
            else
            {
                result = weapon.UsedKeyStone == KeyStones.WindStone
                    ? weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                        ? CollidersDamage.BladeColliderSpinLV1GreenStoneLV2
                        : CollidersDamage.BladeColliderSpinLV1GreenStoneLV1
                    : CollidersDamage.BladeColliderSpinLV1;
            }


            return result;
        }
        
        private CollidersDamage ColliderDamageSwordCombo4(BaseItem weapon)
        {
            CollidersDamage result = CollidersDamage.BladeColliderCombo4LV1;
            
            if (weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent3)
            {
                result = weapon.UsedKeyStone == KeyStones.WindStone
                    ? weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                        ? CollidersDamage.BladeColliderCombo4LV2GreenStoneLV2
                        : CollidersDamage.BladeColliderCombo4LV2GreenStoneLV1
                    : CollidersDamage.BladeColliderCombo4LV2;
            }
            else
            {
                result = weapon.UsedKeyStone == KeyStones.WindStone
                    ? weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                        ? CollidersDamage.BladeColliderCombo4LV1GreenStoneLV2
                        : CollidersDamage.BladeColliderCombo4LV1GreenStoneLV1
                    : CollidersDamage.BladeColliderCombo4LV1;
            }


            return result;
        }
        
        private void ColliderDamageSwordCombo(BaseItem weapon , CollidersDamage collidersDamage,
            CollidersDamage collidersDamageGreenLV1, CollidersDamage collidersDamageGreenLV2)
        {
            if (weapon.WeaponType != WeaponTypes.Sword)
            {
                Debug.Log($"Weapon not sword! : {weapon.WeaponType}!");
                return;
            }
            
            playerDamageCollier = weapon.UsedKeyStone == KeyStones.WindStone
                ? weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                    ? playerDamageColliers.Find(x =>
                        x.PlayerDamage.CollidersDamage == collidersDamageGreenLV2)
                    : playerDamageColliers.Find(x =>
                        x.PlayerDamage.CollidersDamage == collidersDamageGreenLV1)
                : playerDamageColliers.Find(x =>
                    x.PlayerDamage.CollidersDamage == collidersDamage);
        }

        #endregion

        #region VFX

        public void ShowCombo4()
        {
            PoolVfxCombo4();
        }

        public void ShowSlash(int index)
        {
            ActiveSwordSlash(index, true);
        }

        public void HideSlash(int index)
        {
            ActiveSwordSlash(index, false);
        }

        private void PoolVfxCombo4()
        {
            var spawnPoint = playerDamageCollier;
            
            PoolManager.PoolObjectType poolObj = ShockWave();
            
            var shockWave = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (shockWave)
            {
                var position = spawnPoint.gameObject.transform.position;
                shockWave.transform.position = position;
                shockWave.transform.rotation = Quaternion.identity;
                shockWave.GetComponent<VfxShockWave>().Init(position);
                shockWave.SetActive(true);
            }
        }

        private PoolManager.PoolObjectType ShockWave()
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;

            PoolManager.PoolObjectType result = weapon.UsedKeyStone switch
            {
                KeyStones.WindStone => weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2
                    ? PoolManager.PoolObjectType.ShockWavePlayerGreenStoneLV2
                    : PoolManager.PoolObjectType.ShockWavePlayerGreenStoneLV1,
                KeyStones.FireStone => PoolManager.PoolObjectType.ShockWavePlayerRedStone,
                _ => PoolManager.PoolObjectType.ShockWavePlayer
            };

            return result;
        }

        private void ActiveSwordSlash(int index,bool isActive)
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;

            if (weapon.WeaponType != WeaponTypes.Sword)
            {
                Debug.Log($"Weapon not sword! : {weapon.WeaponType}!");
                return;
            }

            if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                if (weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2)
                {
                    vfxSlashGreenStoneLV2[index].SetActive(isActive);
                }
                else
                {
                    vfxSlashGreenStoneLV1[index].SetActive(isActive);
                }
            }
            else if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                vfxSlashRedStone[index].SetActive(isActive);
            }
            else
            {
                vfxSlash[index].SetActive(isActive);
            }
        }

        public void CloseVfx()
        {
            foreach (GameObject slash in vfxSlash)
            {
                slash.SetActive(false);
            }

            foreach (GameObject slashRedStone in vfxSlashRedStone)
            {
                slashRedStone.SetActive(false);
            }
            
            foreach (GameObject slashGreenStone in vfxSlashGreenStoneLV1)
            {
                slashGreenStone.SetActive(false);
            }
            
            foreach (GameObject slashGreenStone in vfxSlashGreenStoneLV2)
            {
                slashGreenStone.SetActive(false);
            }

            foreach (GameObject punch in vfxPunch)
            {
                punch.SetActive(false);
            }
            
            foreach (GameObject punchRed in vfxPunchRed)
            {
                punchRed.SetActive(false);
            }
            
            foreach (GameObject punchGreen in vfxPunchGreen)
            {
                punchGreen.SetActive(false);
            }

            vfxShield.SetActive(false);
            vfxPunchGreenStone.SetActive(false);
        }

        public void ShowVfxLine(bool isShow)
        {
            vfxLine.SetActive(isShow);
        }
        
        public void ShowVfxStun(bool isShow)
        {
            vfxStun.SetActive(isShow);
        }

        public void ShowVfxChangeWeapon()
        {
            vfxChangeWeapon.SetActive(false);
            vfxChangeWeapon.SetActive(true);
        }
        
        public void ShowVfxDash()
        {
            vfxDash.SetActive(true);
        }
        
        public void HideVfxDash()
        {
            vfxDash.SetActive(false);
        }
        
        public void ShowVfxPunchGreenStone()
        {
            vfxPunchGreenStone.SetActive(true);
        }
        
        public void HideVfxPunchGreenStone()
        {
            vfxPunchGreenStone.SetActive(false);
        }

        public void ShowVfxKnuckleSkillRedStone()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.KnuckleSkillRedStone;
            
            var fire = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (fire)
            {
                Quaternion rotation = Quaternion.LookRotation(spawnKnuckleSkillRedStone.forward);
                fire.transform.position = spawnKnuckleSkillRedStone.position;
                fire.transform.rotation = Quaternion.Lerp(fire.transform.rotation, rotation, 1);
                fire.SetActive(true);
            }
        }

        public void ShowVfxPunch(int index)
        {
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);
            
            if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                vfxPunchRed[index].SetActive(true);
            }
            else if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                vfxPunchGreen[index].SetActive(true);
            }
            else
            {
                vfxPunch[index].SetActive(true);
            }
        }
        
        public void HideVfxPunch(int index)
        {
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);
            
            if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                vfxPunchRed[index].SetActive(false);
            }
            else if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                vfxPunchGreen[index].SetActive(false);
            }
            else
            {
                vfxPunch[index].SetActive(false);
            }
        }

        public void ShowSpine()
        {
            ActiveSpine(true);
        }
        
        public void HideSpine()
        {
            ActiveSpine(false);
        }
        
        private void ActiveSpine(bool isActive)
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (weapon.WeaponType != WeaponTypes.Sword)
            {
                Debug.Log($"Weapon not sword! : {weapon.WeaponType}!");
                return;
            }
            
            if (weapon.UsedKeyStone == KeyStones.WindStone)
            {
                if (weapon.WeaponComponentLevel[weapon.MaxLevel - 1].UpgradeComponent2)
                {
                    vfxSpineGreenStoneLV2.SetActive(isActive);
                }
                else
                {
                    vfxSpineGreenStoneLV1.SetActive(isActive);
                }
            }
            else if (weapon.UsedKeyStone == KeyStones.FireStone)
            {
                vfxSpineRedStone.SetActive(isActive);
            }
            else
            {
                vfxSpine.SetActive(isActive);
            }
        }
        
        public void CloseSpine()
        {
            vfxSpine.SetActive(false);
            vfxSpineRedStone.SetActive(false);
            vfxSpineGreenStoneLV1.SetActive(false);
            vfxSpineGreenStoneLV2.SetActive(false);
        }

        #endregion

        #endregion

        public void GetItem(ItemsName itemName, int count, out bool isAdd)
        {
            var item = playerData.Info.Inventory.InventoryGeneral.Items.Find(x
                => x.Name[Index.Start] == itemName);

            playerData.Info.Inventory.InventoryGeneral.Add(item, count, out isAdd, playerData.Info);
        }

        public void OpenUI(GameObject ui, bool isOpen)
        {
            ui.SetActive(isOpen);
        }

        private void ShowTextDamage(int damage)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.TextDamage;

            var textDamage = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (textDamage)
            {
                textDamage.transform.position = transform.position;
                textDamage.transform.rotation = Quaternion.identity;
                textDamage.SetActive(true);
                textDamage.GetComponent<TextDamage>().Init(damage, true);
            }
        }

        #region SOUND

        #region SWORD

        public void SfxSwordAttack(int combo)
        {
            switch (combo)
            {
                case 1:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordAttack1);
                    break;
                case 2:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordAttack2);
                    break;
                case 3:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordAttack3);
                    break;
                case 4:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordAttack4Start);
                    break;
                case 5:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordAttack4End);
                    break;
            }
        }

        public void SfxSpin()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordQ);
        }
        
        public void SfxGroundSlash()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerSwordGroundSlash);
        }
        
        public void SfxBowChargeHold()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerBowChargeHold);
        }
        
        public void SfxBowChargeEnd()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerBowChargeEnd);
        }

        #endregion

        #region KNUCKLE
        
        public void SfxKnuckleAttack(int combo)
        {
            switch (combo)
            {
                case 1:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveAttack1);
                    break;
                case 2:
                    SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveAttack2);
                    break;
            }
        }

        public void SfxKnuckleCounterAttack()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveCounterAttack);
        }
        
        public void SfxKnuckleGuardCounter()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveSkillGuardCounterStart);
        }
        
        public void SfxKnuckleFireStone()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveFireStone);
        }
        
        public void SfxKnuckleWindStone()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveWindStone);
        }
        
        public void SfxKnuckleBlueStoneAttack(int combo)
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerGloveBlueStoneStart);
        }

        #endregion
        
        #region GUN

        public void SfxGun()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerRifleAim);
        }
       
        public void SfxGrenade()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerRifleGrenadeStart);
        }
        
        public void SfxGunWindStone()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerRifleWindAttackStart);
        }
        
        public void SfxGunLaserAttack()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerFireLaserAttack);
        }
        
        public void SfxGunLaserAttackEnd()
        {
            SoundHandler.Instance.StopSFX(SoundClip.Sound.PlayerFireLaserAttack);
        }

        public void SfxDead()
        {
            SoundHandler.Instance.PlaySFXDead();
        }
        
        public void SfxGunSpearAttack()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerBlueSpearAttack);
        }
        
        public void SfxGunSpearAttackEnd()
        {
            SoundHandler.Instance.StopSFX(SoundClip.Sound.PlayerBlueSpearAttack);
        }
        

        #endregion
        
        #region GENERAL
        
        public void SfxDash()
        {
            SoundHandler.Instance.PlaySFXPlayer(SoundClip.Sound.PlayerDash);
        }

        public void SfxWalk()
        {
            SoundHandler.Instance.PlaySFXWalk();
        }

        public void SfxNotWalk()
        {
            SoundHandler.Instance.StopSFX(SoundClip.Sound.PlayerRun);
        }
        
        #endregion

        #endregion
    }
}