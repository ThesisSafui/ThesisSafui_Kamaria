using System;
using System.Collections;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.UIMainGame
{
    [Serializable]
    public sealed class UISkill
    {
        [SerializeField] private WeaponTypes weaponTypes;
        [SerializeField] private RectTransform parentSkill;
        [SerializeField] private List<ChildrenSkill> childrenSkills;

        public WeaponTypes WeaponTypes => weaponTypes;
        public RectTransform ParentSkill => parentSkill;
        public List<ChildrenSkill> ChildrenSkills => childrenSkills;
    }

    [Serializable]
    public sealed class ChildrenSkill
    {
        [SerializeField] private SkillTypes skillTypes;
        [SerializeField] private KeyStones skillKeyStones;
        [SerializeField] private bool useCooldownFill;
        [SerializeField] private RectTransform uiSkill;
        [SerializeField] private Image cooldownFill;
        
        public SkillTypes SkillTypes => skillTypes;
        public KeyStones SkillKeyStones => skillKeyStones;
        public bool UseCooldownFill => useCooldownFill;
        public RectTransform UiSkill => uiSkill;
        public Image CooldownFill => cooldownFill;
    }

    public enum SkillTypes
    {
        ActiveSkill,
        UltimateSkill
    }
    
    public sealed class UIGamePlay : MonoBehaviour
    {
        [SerializeField] private GameManagerFarming gameManagerFarming;
        [SerializeField] private FarmingManagerSO farmingManagerSO;
        [SerializeField] private PlayerDataSO playerData;

        [Header("UI")] 
        [SerializeField] private UIQuest uiQuest;
        [Header("Potion")]
        [SerializeField] private TextMeshProUGUI potionCount;
        [SerializeField] private Image cooldownPotionFill;
        [Header("Dash")]
        [SerializeField] private Image cooldownDashFill;
        [Header("Weapon")] 
        [SerializeField] private RectTransform frameSelectSword;
        [SerializeField] private RectTransform frameSelectGun;
        [SerializeField] private RectTransform frameSelectKnuckle;
        [Header("UI Skill Weapon")] 
        [SerializeField] private List<UISkill> uiSkills;
        [Header("HP")] 
        [SerializeField] private Image hpFill;

        private const int CAN_NOT_USE = 1;
        private const int CAN_USE = 0;

        #region IMAGE_FILL

        private Image cooldownFillActiveSkillSword;
        private Image cooldownFillUltimateSkillRedStoneSword;
        private Image cooldownFillUltimateSkillBlueStoneSword;
        
        private Image cooldownFillActiveSkillKnuckle;
        private Image cooldownFillUltimateSkillRedStoneKnuckle;
        private Image cooldownFillUltimateSkillGreenStoneKnuckle;
        private Image cooldownFillUltimateSkillBlueStoneKnuckle;
        
        private Image cooldownFillActiveSkillGun;
        private Image cooldownFillUltimateSkillRedStoneGun;
        private Image cooldownFillUltimateSkillGreenStoneGun;
        private Image cooldownFillUltimateSkillBlueStoneGun;
        
        #endregion
        
        #region COROUTINE

        private IEnumerator coroutineDash;
        private IEnumerator coroutineActiveSkillSword;
        private IEnumerator coroutineUltimateSkillRedStoneSword;
        //private IEnumerator coroutineUltimateSkillGreenStoneSword;
        private IEnumerator coroutineUltimateSkillBlueStoneSword;
        private IEnumerator coroutineActiveSkillKnuckle;
        private IEnumerator coroutineUltimateSkillRedStoneKnuckle;
        private IEnumerator coroutineUltimateSkillGreenStoneKnuckle;
        private IEnumerator coroutineUltimateSkillBlueStoneKnuckle;
        private IEnumerator coroutineActiveSkillGun;
        private IEnumerator coroutineUltimateSkillRedStoneGun;
        private IEnumerator coroutineUltimateSkillGreenStoneGun;
        private IEnumerator coroutineUltimateSkillBlueStoneGun;

        #endregion

        private void GetCooldownFillSkill()
        {
            #region SWORD

            cooldownFillActiveSkillSword = FillActive(WeaponTypes.Sword);
            cooldownFillUltimateSkillRedStoneSword = FillUltimate(WeaponTypes.Sword, KeyStones.FireStone);
            cooldownFillUltimateSkillBlueStoneSword = FillUltimate(WeaponTypes.Sword, KeyStones.PowerStone);

            #endregion

            #region KUNCKLE

            cooldownFillActiveSkillKnuckle = FillActive(WeaponTypes.Knuckle);
            cooldownFillUltimateSkillRedStoneKnuckle = FillUltimate(WeaponTypes.Knuckle, KeyStones.FireStone);
            cooldownFillUltimateSkillGreenStoneKnuckle = FillUltimate(WeaponTypes.Knuckle, KeyStones.WindStone);
            cooldownFillUltimateSkillBlueStoneKnuckle = FillUltimate(WeaponTypes.Knuckle, KeyStones.PowerStone);

            #endregion
            
            #region GUN

            cooldownFillActiveSkillGun = FillActive(WeaponTypes.Gun);
            cooldownFillUltimateSkillRedStoneGun = FillUltimate(WeaponTypes.Gun, KeyStones.FireStone);
            cooldownFillUltimateSkillGreenStoneGun = FillUltimate(WeaponTypes.Gun, KeyStones.WindStone);
            cooldownFillUltimateSkillBlueStoneGun = FillUltimate(WeaponTypes.Gun, KeyStones.PowerStone);

            #endregion
        }

        public void SetUIParentSkill()
        {
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;

            for (int i = 0; i < uiSkills.Count; i++)
            {
                uiSkills[i].ParentSkill.gameObject.SetActive(false);
                
                for (int j = 0; j < uiSkills[i].ChildrenSkills.Count; j++)
                {
                    uiSkills[i].ChildrenSkills[j].UiSkill.gameObject.SetActive(false);
                }
            }

            uiSkills.Find(x => x.WeaponTypes == weapon.WeaponType).ParentSkill.gameObject.SetActive(true);
            
            if (weapon.WeaponComponentLevel[1].UpgradeComponent1)
            {
                uiSkills.Find(x => x.WeaponTypes == weapon.WeaponType).ChildrenSkills
                    .Find(x => x.SkillTypes == SkillTypes.ActiveSkill).UiSkill.gameObject.SetActive(true);
            }
            
            if (weapon.WeaponComponentLevel[1].UpgradeComponent2 && weapon.UsedKeyStone != KeyStones.None)
            {
                uiSkills.Find(x => x.WeaponTypes == weapon.WeaponType).ChildrenSkills
                    .Find(x => x.SkillTypes == SkillTypes.UltimateSkill && x.SkillKeyStones == weapon.UsedKeyStone)
                    .UiSkill.gameObject.SetActive(true);
            }

            FrameSelect(weapon.WeaponType);
        }

        private void FrameSelect(WeaponTypes weaponTypes)
        {
            frameSelectSword.gameObject.SetActive(false);
            frameSelectGun.gameObject.SetActive(false);
            frameSelectKnuckle.gameObject.SetActive(false);
            
            switch (weaponTypes)
            {
                case WeaponTypes.Sword:
                    frameSelectSword.gameObject.SetActive(true);
                    break;
                case WeaponTypes.Gun:
                    frameSelectGun.gameObject.SetActive(true);
                    break;
                case WeaponTypes.Knuckle:
                    frameSelectKnuckle.gameObject.SetActive(true);
                    break;
            }
        }
        
        private Image FillActive(WeaponTypes weaponTypes)
        {
            return uiSkills.Find(x => x.WeaponTypes == weaponTypes).ChildrenSkills.Find
                (x => x.SkillTypes == SkillTypes.ActiveSkill).CooldownFill;
        }
        
        private Image FillUltimate(WeaponTypes weaponTypes, KeyStones keyStones)
        {
            return uiSkills.Find(x => x.WeaponTypes == weaponTypes).ChildrenSkills.Find
                (x => x.SkillTypes == SkillTypes.UltimateSkill && x.SkillKeyStones == keyStones).CooldownFill;
        }

        public void UIInitialize()
        {
            GetCooldownFillSkill();
            StopAllCoroutines();
            SetUIPotionCount(farmingManagerSO.MaxPotion);
            SetUIParentSkill();
            cooldownPotionFill.fillAmount = farmingManagerSO.CurrentPotion > 0 ? CAN_USE : CAN_NOT_USE;
            cooldownDashFill.fillAmount = CAN_USE;
            cooldownFillActiveSkillSword.fillAmount = CAN_USE;
            cooldownFillUltimateSkillRedStoneSword.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillBlueStoneSword.fillAmount = CAN_USE;;
        
            cooldownFillActiveSkillKnuckle.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillRedStoneKnuckle.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillGreenStoneKnuckle.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillBlueStoneKnuckle.fillAmount = CAN_USE;;
        
            cooldownFillActiveSkillGun.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillRedStoneGun.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillGreenStoneGun.fillAmount = CAN_USE;;
            cooldownFillUltimateSkillBlueStoneGun.fillAmount = CAN_USE;;
            
            hpFill.fillAmount = 1;

            if (gameManagerFarming.Dungeon == Dungeons.None && gameManagerFarming.Map == Map.Lobby)
            {
                cooldownPotionFill.fillAmount = CAN_NOT_USE;
            }

            uiQuest.gameObject.SetActive(false);
            uiQuest.gameObject.SetActive(true);
        }

        public void UpdateUIHp()
        {
            float current = hpFill.fillAmount;
            float temp = (float)playerData.Info.Status.CurrentHealth / playerData.Info.Status.MaxHealth;

            Debug.Log("UpdateUIHp");
            StartCoroutine(HpUpdate(current, temp));
        }
        
        private IEnumerator HpUpdate(float current, float temp)
        {
            float cooldownTime = 1;
            float con = 0;
            while (cooldownTime > 0)
            {
                if (!playerData.IsUsingUI)
                {
                    float hpAmount = Mathf.Lerp(current, temp, con);
                    hpFill.fillAmount = Mathf.Clamp(hpAmount, 0, 1);
                    con += Time.deltaTime;
                    cooldownTime -= Time.deltaTime;
                }

                yield return null;
            }
        }

        #region SKILL

        public void SetUICooldownDash()
        {
            cooldownDashFill.fillAmount = CAN_NOT_USE;
            coroutineDash = Cooldown(playerData.CharacterControllerData.DashCooldown, cooldownDashFill);
            StartCoroutine(coroutineDash);
        }

        #region SWORD

        public void SetUICooldownActiveSkillSword(float cooldown)
        {
            cooldownFillActiveSkillSword.fillAmount = CAN_NOT_USE;
            coroutineActiveSkillSword = Cooldown(cooldown, cooldownFillActiveSkillSword);
            StartCoroutine(coroutineActiveSkillSword);
        }
        
        public void SetUICooldownUltimateSkillRedStoneSword(float cooldown)
        {
            cooldownFillUltimateSkillRedStoneSword.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillRedStoneSword = Cooldown(cooldown, cooldownFillUltimateSkillRedStoneSword);
            StartCoroutine(coroutineUltimateSkillRedStoneSword);
        }
        
        public void SetUICooldownUltimateSkillBlueStoneSword(float cooldown)
        {
            cooldownFillUltimateSkillBlueStoneSword.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillBlueStoneSword = Cooldown(cooldown, cooldownFillUltimateSkillBlueStoneSword);
            StartCoroutine(coroutineUltimateSkillBlueStoneSword);
        }

        #endregion

        #region KNUCKLE

        public void SetUICooldownActiveSkillKnuckle(float cooldown)
        {
            cooldownFillActiveSkillKnuckle.fillAmount = CAN_NOT_USE;
            coroutineActiveSkillKnuckle = Cooldown(cooldown, cooldownFillActiveSkillKnuckle);
            StartCoroutine(coroutineActiveSkillKnuckle);
        }
        
        public void SetUICooldownUltimateSkillRedStoneKnuckle(float cooldown)
        {
            cooldownFillUltimateSkillRedStoneKnuckle.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillRedStoneKnuckle = Cooldown(cooldown, cooldownFillUltimateSkillRedStoneKnuckle);
            StartCoroutine(coroutineUltimateSkillRedStoneKnuckle);
        }
        
        public void SetUICooldownUltimateSkillGreenStoneKnuckle(float cooldown)
        {
            cooldownFillUltimateSkillGreenStoneKnuckle.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillGreenStoneKnuckle = Cooldown(cooldown, cooldownFillUltimateSkillGreenStoneKnuckle);
            StartCoroutine(coroutineUltimateSkillGreenStoneKnuckle);
        }
        
        public void SetUICooldownUltimateSkillBlueStoneKnuckle(float cooldown)
        {
            cooldownFillUltimateSkillBlueStoneKnuckle.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillBlueStoneKnuckle = Cooldown(cooldown, cooldownFillUltimateSkillBlueStoneKnuckle);
            StartCoroutine(coroutineUltimateSkillBlueStoneKnuckle);
        }

        #endregion

        #region GUN

        public void SetUICooldownActiveSkillGun(float cooldown)
        {
            cooldownFillActiveSkillGun.fillAmount = CAN_NOT_USE;
            coroutineActiveSkillGun = Cooldown(cooldown, cooldownFillActiveSkillGun);
            StartCoroutine(coroutineActiveSkillGun);
        }
        
        public void SetUICooldownUltimateSkillRedStoneGun(float cooldown)
        {
            cooldownFillUltimateSkillRedStoneGun.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillRedStoneGun = Cooldown(cooldown, cooldownFillUltimateSkillRedStoneGun);
            StartCoroutine(coroutineUltimateSkillRedStoneGun);
        }
        
        public void SetUICooldownUltimateSkillGreenStoneGun(float cooldown)
        {
            cooldownFillUltimateSkillGreenStoneGun.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillGreenStoneGun = Cooldown(cooldown, cooldownFillUltimateSkillGreenStoneGun);
            StartCoroutine(coroutineUltimateSkillGreenStoneGun);
        }
        
        public void SetUICooldownUltimateSkillBlueStoneGun(float cooldown)
        {
            cooldownFillUltimateSkillBlueStoneGun.fillAmount = CAN_NOT_USE;
            coroutineUltimateSkillBlueStoneGun = Cooldown(cooldown, cooldownFillUltimateSkillBlueStoneGun);
            StartCoroutine(coroutineUltimateSkillBlueStoneGun);
        }

        #endregion
        
        #endregion
        
        private IEnumerator Cooldown(float cooldown, Image cooldownFill)
        {
            float cooldownTime = cooldown;

            while (cooldownTime > 0)
            {
                if (!playerData.IsUsingUI)
                {
                    cooldownFill.fillAmount = Mathf.Clamp((float)cooldownTime / cooldown, 0, 1);
                    cooldownTime -= Time.deltaTime;
                }

                yield return null;
            }
        }
        
        #region POTION

        public void SetUIPotionCount(int count)
        {
            potionCount.text = count.ToString();
        }

        public void SetUICooldownPotionFill()
        {
            cooldownPotionFill.fillAmount = 1;
            StartCoroutine(nameof(CooldownPotion));
        }

        private IEnumerator CooldownPotion()
        {
            float cooldownTime = playerData.CharacterSkillData.CooldownPotion;

            if (farmingManagerSO.CurrentPotion <= 0) yield break;
            
            while (cooldownTime > 0)
            {
                if (!playerData.IsUsingUI)
                {
                    cooldownPotionFill.fillAmount = Mathf.Clamp
                        ((float)cooldownTime / playerData.CharacterSkillData.CooldownPotion, 0, 1);
                    cooldownTime -= Time.deltaTime;
                }

                yield return null;
            }
        }

        #endregion
    }
}