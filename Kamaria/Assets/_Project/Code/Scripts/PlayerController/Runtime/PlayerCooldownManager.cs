using System;
using System.Collections;
using System.Linq;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.UI.UIMainGame;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class PlayerCooldownManager : MonoBehaviour
    {
        [SerializeField] private UIGamePlay uiGamePlay;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private PlayerControllerInput playerControllerInput;
        [SerializeField] private CharacterSkill characterSkill;
        
        private void OnEnable()
        {
            playerControllerInput.DashCooldownAction += DashCooldown;
            playerControllerInput.PunchWaitTime += PlayerControllerInputOnPunchWaitTime;
            characterSkill.CooldownSwordActiveSkillComponent1Action += CharacterSkillOnCooldownSwordActiveSkillComponent1;
            characterSkill.CooldownSwordUltimateSkillAction += CharacterSkillOnCooldownSwordUltimateSkill;
            characterSkill.CooldownSwordPowerStoneSkillAction += CharacterSkillOnCooldownSwordPowerStoneSkill;
            characterSkill.CooldownChangeWeapon += CharacterSkillOnCooldownChangeWeapon;
            
            characterSkill.CooldownKnuckleActiveSkillComponent1Action += CharacterSkillOnCooldownKnuckleActiveSkillComponent1;
            characterSkill.CooldownKnuckleUltimateSkillAction += CharacterSkillOnCooldownKnuckleUltimate;
            characterSkill.CooldownKnucklePowerStoneSkillAction += CharacterSkillOnCooldownKnucklePowerStoneSkill;
            characterSkill.CooldownKnuckleCannonAttack += CharacterSkillOnCooldownKnuckleCannonAttack;
            
            characterSkill.CooldownGunAttack += CharacterSkillOnCooldownGunAttack;
            characterSkill.CooldownGunActiveSkillComponent1Action += CharacterSkillOnCooldownGunActiveSkillComponent1Action;
            characterSkill.CooldownGunUltimateSkillAction += CharacterSkillOnCooldownGunUltimateSkillAction;
            characterSkill.CooldownGunPowerStoneSkillAction += CharacterSkillOnCooldownGunPowerStoneSkillAction;
            
            characterSkill.CooldownPotion += CharacterSkillOnCooldownPotion;
        }

        private void OnDisable()
        {
            playerControllerInput.DashCooldownAction -= DashCooldown;
            playerControllerInput.PunchWaitTime -= PlayerControllerInputOnPunchWaitTime;
            characterSkill.CooldownSwordActiveSkillComponent1Action -= CharacterSkillOnCooldownSwordActiveSkillComponent1;
            characterSkill.CooldownSwordUltimateSkillAction -= CharacterSkillOnCooldownSwordUltimateSkill;
            characterSkill.CooldownSwordPowerStoneSkillAction -= CharacterSkillOnCooldownSwordPowerStoneSkill;
            characterSkill.CooldownChangeWeapon -= CharacterSkillOnCooldownChangeWeapon;
            
            characterSkill.CooldownKnuckleActiveSkillComponent1Action -= CharacterSkillOnCooldownKnuckleActiveSkillComponent1;
            characterSkill.CooldownKnuckleUltimateSkillAction -= CharacterSkillOnCooldownKnuckleUltimate;
            characterSkill.CooldownKnucklePowerStoneSkillAction -= CharacterSkillOnCooldownKnucklePowerStoneSkill;
            characterSkill.CooldownKnuckleCannonAttack -= CharacterSkillOnCooldownKnuckleCannonAttack;
            
            characterSkill.CooldownGunAttack -= CharacterSkillOnCooldownGunAttack;
            characterSkill.CooldownGunActiveSkillComponent1Action -= CharacterSkillOnCooldownGunActiveSkillComponent1Action;
            characterSkill.CooldownGunUltimateSkillAction -= CharacterSkillOnCooldownGunUltimateSkillAction;
            characterSkill.CooldownGunPowerStoneSkillAction -= CharacterSkillOnCooldownGunPowerStoneSkillAction;
            
            characterSkill.CooldownPotion -= CharacterSkillOnCooldownPotion;
            CancelCooldown();
        }
        
        private void CharacterSkillOnCooldownPotion()
        {
            StartCoroutine(nameof(CooldownPotion));
        }

        private IEnumerator CooldownPotion()
        {
            characterSkill.isCooldownPotion = true;
            yield return new WaitForSeconds(playerData.CharacterSkillData.CooldownPotion);
            characterSkill.isCooldownPotion = false;
        }
        
        private void CharacterSkillOnCooldownGunPowerStoneSkillAction()
        {
            StartCoroutine(nameof(CooldownGunPowerStoneSkill));
        }

        private IEnumerator CooldownGunPowerStoneSkill()
        {
            characterSkill.isGunUltimateSkillCooldown = true;
            uiGamePlay.SetUICooldownUltimateSkillBlueStoneGun(
                playerData.CharacterSkillData.SkillGunPowerStoneCooldown.Cooldown[Index.Start]);
            yield return new WaitForSeconds(
                playerData.CharacterSkillData.SkillGunPowerStoneCooldown.Cooldown[Index.Start]);
            characterSkill.isGunUltimateSkillCooldown = false;
        }
        
        private void CharacterSkillOnCooldownGunUltimateSkillAction()
        {
            StartCoroutine(nameof(CooldownGunUltimateSkill));
        }

        private IEnumerator CooldownGunUltimateSkill()
        {
            characterSkill.isGunUltimateSkillCooldown = true;
            
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);

            float cooldown = characterSkill.UseSkillGunRedStone
                ? weapon.WeaponComponentLevel[2].UpgradeComponent2
                    ? playerData.CharacterSkillData.SkillGunRedStoneCooldown.Cooldown.Last()
                    : playerData.CharacterSkillData.SkillGunRedStoneCooldown.Cooldown.First()
                : weapon.WeaponComponentLevel[2].UpgradeComponent2
                    ? playerData.CharacterSkillData.SkillGunGreenStoneCooldown.Cooldown.Last()
                    : playerData.CharacterSkillData.SkillGunGreenStoneCooldown.Cooldown.First();

            if (characterSkill.UseSkillGunRedStone)
            {
                uiGamePlay.SetUICooldownUltimateSkillRedStoneGun(cooldown);
            }
            else
            {
                uiGamePlay.SetUICooldownUltimateSkillGreenStoneGun(cooldown);
            }
            
            yield return new WaitForSeconds(cooldown);
            
            characterSkill.isGunUltimateSkillCooldown = false;
        }
        
        private void CharacterSkillOnCooldownGunActiveSkillComponent1Action()
        {
            StartCoroutine(nameof(CooldownGunActiveSkillComponent1Time));
        }

        private IEnumerator CooldownGunActiveSkillComponent1Time()
        {
            characterSkill.isGunActiveSkillComponent1Cooldown = true;

            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Gun);
            
            float cooldown = weapon.WeaponComponentLevel[2].UpgradeComponent1
                ? playerData.CharacterSkillData.SkillGunComponent1Cooldown.Cooldown.Last()
                : playerData.CharacterSkillData.SkillGunComponent1Cooldown.Cooldown.First();

            uiGamePlay.SetUICooldownActiveSkillGun(cooldown);
            yield return new WaitForSeconds(cooldown);
            
            characterSkill.isGunActiveSkillComponent1Cooldown = false;
        }
        
        private void CharacterSkillOnCooldownGunAttack()
        {
            StartCoroutine(nameof(CooldownGunAttack));
        }

        private IEnumerator CooldownGunAttack()
        {
            characterSkill.isGunAttackCooldown = true;
            yield return new WaitForSeconds(playerData.CharacterSkillData.GunAttackCooldown.Cooldown[Index.Start]);
            characterSkill.isGunAttackCooldown = false;
        }
        
        private void CharacterSkillOnCooldownKnuckleCannonAttack()
        {
            StartCoroutine(nameof(CooldownCannonAttack));
        }

        private IEnumerator CooldownCannonAttack()
        {
            characterSkill.isCannonAttackCooldown = true;
            yield return new WaitForSeconds(playerData.CharacterSkillData.CannonAttackCooldown.Cooldown[Index.Start]);
            characterSkill.isCannonAttackCooldown = false;
        }
        
        private void CharacterSkillOnCooldownKnucklePowerStoneSkill()
        {
            StartCoroutine(nameof(CooldownKnucklePowerStoneSkill));
        }
        
        private IEnumerator CooldownKnucklePowerStoneSkill()
        {
            characterSkill.isKnuckleUltimateSkillCooldown = true;
            uiGamePlay.SetUICooldownUltimateSkillBlueStoneKnuckle(
                playerData.CharacterSkillData.SkillKnucklePowerStoneCooldown.Cooldown[Index.Start]);
            yield return new WaitForSeconds(
                playerData.CharacterSkillData.SkillKnucklePowerStoneCooldown.Cooldown[Index.Start]);
            characterSkill.isKnuckleUltimateSkillCooldown = false;
        }

        private void CharacterSkillOnCooldownKnuckleUltimate()
        {
            StartCoroutine(nameof(CooldownKnuckleUltimateSkill));
        }

        private IEnumerator CooldownKnuckleUltimateSkill()
        {
            characterSkill.isKnuckleUltimateSkillCooldown = true;
            
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);

            float cooldown = characterSkill.UseSkillKnuckleRedStone
                ? weapon.WeaponComponentLevel[2].UpgradeComponent2
                    ? playerData.CharacterSkillData.SkillKnuckleRedStoneCooldown.Cooldown.Last()
                    : playerData.CharacterSkillData.SkillKnuckleRedStoneCooldown.Cooldown.First()
                : weapon.WeaponComponentLevel[2].UpgradeComponent2
                    ? playerData.CharacterSkillData.SkillKnuckleGreenStoneCooldown.Cooldown.Last()
                    : playerData.CharacterSkillData.SkillKnuckleGreenStoneCooldown.Cooldown.First();

            if (characterSkill.UseSkillKnuckleRedStone)
            {
                uiGamePlay.SetUICooldownUltimateSkillRedStoneKnuckle(cooldown);
            }
            else
            {
                uiGamePlay.SetUICooldownUltimateSkillGreenStoneKnuckle(cooldown);
            }
            
            yield return new WaitForSeconds(cooldown);
            
            characterSkill.isKnuckleUltimateSkillCooldown = false;
        }
        
        private void CharacterSkillOnCooldownKnuckleActiveSkillComponent1()
        {
            StartCoroutine(nameof(CooldownKnuckleActiveSkillComponent1Time));
        }

        private IEnumerator CooldownKnuckleActiveSkillComponent1Time()
        {
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);
            
            float cooldown = weapon.WeaponComponentLevel[2].UpgradeComponent1
                ? playerData.CharacterSkillData.SkillKnuckleComponent1Cooldown.Cooldown.Last()
                : playerData.CharacterSkillData.SkillKnuckleComponent1Cooldown.Cooldown.First();
            uiGamePlay.SetUICooldownActiveSkillKnuckle(cooldown);
            yield return new WaitForSeconds(cooldown);
            characterSkill.isKnuckleActiveSkillComponent1Cooldown = false;
        }
        
        private void PlayerControllerInputOnPunchWaitTime()
        {
            StartCoroutine(nameof(WaitNextComboKnuckle));
        }

        public void StopPunchWaitTime()
        {
            StopCoroutine(nameof(WaitNextComboKnuckle));
        }
        
        private IEnumerator WaitNextComboKnuckle()
        {
            yield return new WaitForSeconds(playerData.CharacterSkillData.WaitNextComboKnuckleTime);
            
            playerData.CharacterSkillData.ResetComboKnuckle();
        }

        private void CharacterSkillOnCooldownChangeWeapon()
        {
            StartCoroutine(nameof(CooldownChangeWeapon));
        }

        private IEnumerator CooldownChangeWeapon()
        {
            playerData.CharacterSkillData.IsChangeWeaponCooldown = true;
            yield return new WaitForSeconds(playerData.CharacterSkillData.CooldownChangeWeapon);
            playerData.CharacterSkillData.IsChangeWeaponCooldown = false;
        }
        
        public void CancelCooldown()
        {
            StopAllCoroutines();
            playerControllerInput.isDashCooldown = false;
            
            characterSkill.isSwordActiveSkillComponent1Cooldown = false;
            characterSkill.isSwordUltimateSkillCooldown = false;
            
            playerData.CharacterSkillData.IsChangeWeaponCooldown = false;
            
            characterSkill.isKnuckleActiveSkillComponent1Cooldown = false;
            characterSkill.isKnuckleUltimateSkillCooldown = false;
            characterSkill.isCannonAttackCooldown = false;
            playerData.CharacterSkillData.ResetComboKnuckle();
            
            characterSkill.isGunAttackCooldown = false;
            characterSkill.isGunActiveSkillComponent1Cooldown = false;
            characterSkill.isGunUltimateSkillCooldown = false;
            
            characterSkill.isCooldownPotion = false;
        }

        private void DashCooldown()
        {
            StartCoroutine(DashCooldownTime());
        }
        
        
        private IEnumerator DashCooldownTime()
        {
            playerControllerInput.isDashCooldown = true;
            yield return new WaitForSeconds(playerData.CharacterControllerData.DashCooldown);
            playerControllerInput.isDashCooldown = false;
        }
        
        private void CharacterSkillOnCooldownSwordActiveSkillComponent1()
        {
            StartCoroutine(CooldownSwordActiveSkillComponent1Time());
        }
        
        private IEnumerator CooldownSwordActiveSkillComponent1Time()
        {
            characterSkill.isSwordActiveSkillComponent1Cooldown = true;
            
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword);
            
            float cooldown = weapon.WeaponComponentLevel[2].UpgradeComponent1
                ? playerData.CharacterSkillData.SkillSwordComponent1Cooldown.Cooldown.Last()
                : playerData.CharacterSkillData.SkillSwordComponent1Cooldown.Cooldown.First();
            uiGamePlay.SetUICooldownActiveSkillSword(cooldown);
            yield return new WaitForSeconds(cooldown);
            characterSkill.isSwordActiveSkillComponent1Cooldown = false;
        }
        
        private void CharacterSkillOnCooldownSwordUltimateSkill()
        {
            StartCoroutine(CooldownSwordUltimateSkill());
        }
        
        private IEnumerator CooldownSwordUltimateSkill()
        {
            characterSkill.isSwordUltimateSkillCooldown = true;
            
            BaseItem weapon =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword);
            
            float cooldown = weapon.WeaponComponentLevel[2].UpgradeComponent2
                ? playerData.CharacterSkillData.SkillSwordRedStoneCooldown.Cooldown.Last()
                : playerData.CharacterSkillData.SkillSwordRedStoneCooldown.Cooldown.First();
            uiGamePlay.SetUICooldownUltimateSkillRedStoneSword(cooldown);
            yield return new WaitForSeconds(cooldown);
            
            characterSkill.isSwordUltimateSkillCooldown = false;
        }
        
        private void CharacterSkillOnCooldownSwordPowerStoneSkill()
        {
            StartCoroutine(CooldownSwordPowerStoneSkill());
        }
        
        private IEnumerator CooldownSwordPowerStoneSkill()
        {
            characterSkill.isSwordUltimateSkillCooldown = true;
            uiGamePlay.SetUICooldownUltimateSkillBlueStoneSword(
                playerData.CharacterSkillData.SkillSwordPowerStoneCooldown.Cooldown[Index.Start]);
            yield return new WaitForSeconds(
                playerData.CharacterSkillData.SkillSwordPowerStoneCooldown.Cooldown[Index.Start]);
            characterSkill.isSwordUltimateSkillCooldown = false;
        }
    }
}