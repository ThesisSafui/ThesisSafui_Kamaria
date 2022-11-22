using System;
using System.Collections;
using DG.Tweening;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.UI.UIMainGame;
using Kamaria.Utilities.GameEvent;
using Kamaria.VFX_ALL;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kamaria.Player.Controller
{
    public sealed class PlayerControllerInput : MonoBehaviour
    {
        public event Action DashCooldownAction;
        public event Action PunchWaitTime;

        [SerializeField] private bool useFog = false;
        [SerializeField] private UIGamePlay uiGamePlay;
        [SerializeField] private Animator anim;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CharacterSkill characterSkill;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private GameObject uiInventory;
        [SerializeField] private GameObject uiMenuOption;
        [SerializeField] private GameObject uiMap;
        [SerializeField] private GameObject uiTips;
        [SerializeField] private Indicator indicatorCannonAttackRange;
        [SerializeField] private RectTransform uiQuest;
        [SerializeField] private GameObject uiFirstTime;

        #region GAME_EVENT

        [Tooltip("Right click hold Performed")]
        [SerializeField] private GameEventSO eventRightClickHoldPerformed;
        
        [Tooltip("Right click hold Canceled")]
        [SerializeField] private GameEventSO eventRightClickHoldCanceled;

        #endregion

        public bool isDashCooldown = false;
        private bool showQuest = true;

        
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private PlayerInput playerInput;

        #region PRIVATE_VARIABLE

        private Vector2 movementInput;
        
        #region VARIABLE_INPUT_ACTIONS

        private InputAction moveAction;
        private InputAction dashAction;
        private InputAction skillWeaponAction;
        private InputAction skillKeyStoneAction;
        private InputAction openMapAction;
        private InputAction openInventoryAction;
        private InputAction interactAction;
        private InputAction useEscape;
        private InputAction leftClickTapAction;
        private InputAction leftClickHoldAction;
        private InputAction rightClickTapAction;
        private InputAction rightClickHoldAction;
        private InputAction mousePositionAction;
        private InputAction changeWeaponSword;
        private InputAction changeWeaponKnuckle;
        private InputAction changeWeaponGun;
        private InputAction usePotion;
        private InputAction openQuest;
        private InputAction openTips;

        #endregion

        #endregion

        private void Awake()
        {
            GetInputAction();
            showQuest = true;
            uiQuest.gameObject.SetActive(showQuest);
        }

        private void OnEnable()
        {
            moveAction.performed += MoveInput;
            dashAction.started += DashInput;
            leftClickTapAction.performed += LeftClickTap;
            leftClickHoldAction.performed += LeftClickHold;
            rightClickTapAction.performed += RightClickTap;
            rightClickHoldAction.performed += RightClickHold;
            skillWeaponAction.started += SkillWeaponActionStarted;
            skillKeyStoneAction.started += SkillKeyStoneActionOnStarted;
            interactAction.started += Interact;
            openInventoryAction.started += OpenInventory;
            openMapAction.started += OpenMap;
            useEscape.started += EscapeOption;
            changeWeaponSword.started += ChangWeaponToSword;
            changeWeaponKnuckle.started += ChangWeaponToKnuckle;
            changeWeaponGun.started += ChangWeaponToGun;
            usePotion.started += UsePotion;
            openQuest.started += OpenQuest;
            openTips.started += OpenTips;

            moveAction.canceled += MoveStopInput;
            rightClickHoldAction.canceled += RightClickHold;
            leftClickHoldAction.canceled += LeftClickHold;
            interactAction.canceled += Interact;
        }
        
        private void MoveSfx()
        {
            playerEvent.SfxWalk();
        }
        
        private void NotMoveSfx()
        {
            playerEvent.SfxNotWalk();
        }

        private void OnDisable()
        {
            moveAction.performed -= MoveInput;
            dashAction.started -= DashInput;
            leftClickTapAction.performed -= LeftClickTap;
            leftClickHoldAction.performed -= LeftClickHold;
            rightClickTapAction.performed -= RightClickTap;
            rightClickHoldAction.performed -= RightClickHold;
            skillWeaponAction.started -= SkillWeaponActionStarted;
            skillKeyStoneAction.started -= SkillKeyStoneActionOnStarted;
            interactAction.started -= Interact;
            openInventoryAction.started -= OpenInventory;
            openMapAction.started -= OpenMap;
            useEscape.started -= EscapeOption;
            changeWeaponSword.started -= ChangWeaponToSword;
            changeWeaponKnuckle.started -= ChangWeaponToKnuckle;
            changeWeaponGun.started -= ChangWeaponToGun;
            usePotion.started -= UsePotion;
            openQuest.started -= OpenQuest;
            openTips.started -= OpenTips;
            
            moveAction.canceled -= MoveStopInput;
            rightClickHoldAction.canceled -= RightClickHold;
            leftClickHoldAction.canceled -= LeftClickHold;
            interactAction.canceled -= Interact;
        }
        
        private void OpenTips(InputAction.CallbackContext callback)
        {
            if (uiFirstTime != null)
            {
                if (playerData.Info.IsFirstTime && uiFirstTime.activeInHierarchy)
                {
                    playerData.Info.IsFirstTime = false;
                    uiTips.SetActive(true);
                    playerData.IsInteract = false;
                    uiFirstTime.SetActive(false);
                }
            }
            
            if (playerData.IsUsingUI || playerData.IsDead) return;
            uiTips.SetActive(true);
            playerData.IsInteract = false;
        }
        
        private void OpenQuest(InputAction.CallbackContext callback)
        {
            showQuest = !showQuest;
            uiQuest.gameObject.SetActive(showQuest);
        }

        private void UsePotion(InputAction.CallbackContext callback)
        {
            //Debug.Log("HHHH");
            characterSkill.UsePotion();
        }

        private void RightClickTap(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            
            AttackRight();
        }

        private void ChangWeaponToSword(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;
            
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (weapon.WeaponType == WeaponTypes.Sword || playerData.CharacterSkillData.IsChangeWeaponCooldown) return;
            
            characterSkill.ChangeWeaponTo(WeaponTypes.Sword);
        }
        private void ChangWeaponToKnuckle(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;
            
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (weapon.WeaponType == WeaponTypes.Knuckle || playerData.CharacterSkillData.IsChangeWeaponCooldown) return;
            
            characterSkill.ChangeWeaponTo(WeaponTypes.Knuckle);
        }
        private void ChangWeaponToGun(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;
            
            BaseItem weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (weapon.WeaponType == WeaponTypes.Gun || playerData.CharacterSkillData.IsChangeWeaponCooldown) return;

            characterSkill.ChangeWeaponTo(WeaponTypes.Gun);
        }

        private void EscapeOption(InputAction.CallbackContext callback)
        {
            if (playerData.IsUsingUI)
            {
                foreach (GameObject ui in playerData.UIsLinkEsc)
                {
                    ui.SetActive(false);
                }
            }
            else
            {
                if (playerData.IsDead) return;
                uiMenuOption.SetActive(true);
                playerData.IsInteract = false;
            }
            
            if (useFog)
            {
                RenderSettings.fog = true;
            }
        }

        private void OpenMap(InputAction.CallbackContext callback)
        {
            if (playerData.IsUsingUI || playerData.IsDead) return;
            uiMap.SetActive(true);
            if (useFog)
            {
                RenderSettings.fog = false;
            }
            playerData.IsInteract = false;
        }

        private void OpenInventory(InputAction.CallbackContext callback)
        {
            if (playerData.IsUsingUI || playerData.IsDead) return;
            uiInventory.SetActive(true);
            playerData.IsInteract = false;
        }

        private void Interact(InputAction.CallbackContext callback)
        {
            if (playerData.IsUsingUI) return;
            if (playerData.IsStop) return;
            if (!playerData.CanInteract) return;

            playerData.IsInteract = callback.action.IsPressed();

        }

        private void Update()
        {
            if (playerData.IsStop)
            {
                playerData.CharacterControllerData.MovementInput = Vector2.zero;
                CancelAim();
                
                if (playerData.CharacterSkillData.IsBowChargeAttacking && playerData.IsUsingUI)
                {
                    playerData.CharacterSkillData.IsBowChargeAttacking = false;
                    characterSkill.BowChargeAttack();
                    Time.timeScale = 0;
                }
                
                return;
            }

            if (movementInput != Vector2.zero && !playerData.PlayerAnimation.IsAnimationAttackNotMove && !playerData.CharacterControllerData.Aiming)
            {
                MoveSfx();
            }
            else
            {
                NotMoveSfx();
            }
            
            playerData.CharacterControllerData.MovementInput = playerData.CharacterControllerData.Aiming ? Vector2.zero : movementInput;

            if (playerData.CharacterControllerData.Dashing)
            {
                CancelAim();
            }
        }

        /// <summary>
        /// Get input action from input system.
        /// </summary>
        private void GetInputAction()
        {
            moveAction = playerInput.actions["Move"];
            dashAction = playerInput.actions["Dash"];
            skillWeaponAction = playerInput.actions["ActiveSkillWeapon"];
            skillKeyStoneAction = playerInput.actions["ActiveSkillKeyStone"];
            openMapAction = playerInput.actions["OpenMap"];
            openInventoryAction = playerInput.actions["OpenInventory"];
            interactAction = playerInput.actions["Interact"];
            useEscape = playerInput.actions["Escape"];
            leftClickTapAction = playerInput.actions["LeftClickTap"];
            leftClickHoldAction = playerInput.actions["LeftClickHold"];
            rightClickTapAction = playerInput.actions["RightClickTap"];
            rightClickHoldAction = playerInput.actions["RightClickHold"];
            mousePositionAction = playerInput.actions["MousePosition"];
            changeWeaponSword = playerInput.actions["ChangeWeapon1"];
            changeWeaponKnuckle = playerInput.actions["ChangeWeapon3"];
            changeWeaponGun = playerInput.actions["ChangeWeapon2"];
            usePotion = playerInput.actions["UsePotion"];
            openQuest = playerInput.actions["OpenQuest"];
            openTips = playerInput.actions["OpenTips"];
        }

        private void MoveInput(InputAction.CallbackContext callback)
        {
            movementInput = callback.ReadValue<Vector2>();
        }

        private void MoveStopInput(InputAction.CallbackContext callback)
        {
            movementInput = Vector2.zero;
        }

        private void LeftClickHold(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (weapon.WeaponType == WeaponTypes.Sword)
            {
                if (!playerData.CharacterSkillData.IsUseSkillPowerStone) return;
                
                playerData.CharacterSkillData.IsBowChargeAttacking = callback.action.IsPressed();
                
                characterSkill.BowChargeAttack();
            }
            else if (weapon.WeaponType == WeaponTypes.Gun)
            {
                
            }
            else if (weapon.WeaponType == WeaponTypes.Knuckle)
            {
                
            }
        }

        private void SkillWeaponActionStarted(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (weapon.WeaponType == WeaponTypes.Sword)
            {
                if (!weapon.WeaponComponentLevel[1].UpgradeComponent1) return;

                if (weapon.WeaponComponentLevel[2].UpgradeComponent1)
                {
                    characterSkill.SwordActiveSkillLv3Component1();
                }
                else
                {
                    characterSkill.SwordActiveSkillLv2Component1();
                }
            }
            else if (weapon.WeaponType == WeaponTypes.Gun)
            {
                if (!playerData.CharacterControllerData.Aiming) return;
                
                if (!weapon.WeaponComponentLevel[1].UpgradeComponent1) return;

                if (weapon.WeaponComponentLevel[2].UpgradeComponent1)
                {
                    characterSkill.GunActiveSkillLv3Component1();
                }
                else
                {
                    characterSkill.GunActiveSkillLv2Component1();
                }
            }
            else if (weapon.WeaponType == WeaponTypes.Knuckle)
            {
                if (!weapon.WeaponComponentLevel[1].UpgradeComponent1) return;

                if (weapon.WeaponComponentLevel[2].UpgradeComponent1)
                {
                    characterSkill.KnuckleActiveSkillLv3Component1();
                }
                else
                {
                    characterSkill.KnuckleActiveSkillLv2Component1();
                }
            }
        }
        
        private void SkillKeyStoneActionOnStarted(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            if (!playerData.CharacterSkillData.ChangeWeaponFinish) return;
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;

            characterSkill.StopBowCharging();

            RaycastDirection();
            
            if (weapon.WeaponType == WeaponTypes.Sword)
            {
                if (!weapon.WeaponComponentLevel[1].UpgradeComponent2) return;

                if (weapon.UsedKeyStone is KeyStones.WindStone or KeyStones.None) return;

                CancelAim();

                if (weapon.WeaponComponentLevel[2].UpgradeComponent2)
                {
                    characterSkill.SwordKeyStoneSkillLv3Component2();
                }
                else
                {
                    characterSkill.SwordKeyStoneSkillLv2Component2();
                }
            }
            else if (weapon.WeaponType == WeaponTypes.Gun)
            {
                if (!weapon.WeaponComponentLevel[1].UpgradeComponent2) return;
                
                if (weapon.WeaponComponentLevel[2].UpgradeComponent2)
                {
                    characterSkill.GunKeyStoneSkillLv3Component2();
                }
                else
                {
                    characterSkill.GunKeyStoneSkillLv2Component2();
                }
            }
            else if (weapon.WeaponType == WeaponTypes.Knuckle)
            {
                if (!weapon.WeaponComponentLevel[1].UpgradeComponent2) return;

                CancelAim();
                
                if (weapon.WeaponComponentLevel[2].UpgradeComponent2)
                {
                    characterSkill.KnuckleKeyStoneSkillLv3Component2();
                }
                else
                {
                    characterSkill.KnuckleKeyStoneSkillLv2Component2();
                }
            }
        }
        
        private void DashInput(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            if (isDashCooldown) return;
            
            //StartCoroutine(nameof(DashCooldown));
            DashCooldownAction?.Invoke();
            uiGamePlay.SetUICooldownDash();
            characterSkill.ResetCombo();
            characterController.Dash();
        }
        
        private void LeftClickTap(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
            
            if (playerData.CharacterSkillData.UsingGuard)
            {
                characterSkill.GuardCounter();
                return;
            }
            
            Attack();
        }
        
        private void RightClickHold(InputAction.CallbackContext callback)
        {
            if (playerData.IsStop) return;
           
            if (callback.action.IsPressed())
            {
                if (playerData.Info.DeviceCompartment.SlotWeapon.Info.TypeAttackRange == AttacksRange.RangeAttack)
                {
                    UsedAim();
                }
            }
            else
            {
                CancelAim();
            }
        }
        
#endif
        
        private void Attack()
        {
            if (!playerData.CharacterSkillData.IsChangeWeaponFinish) return;
            if (playerData.CharacterControllerData.Dashing || playerData.CharacterSkillData.IsUseSkillCanNotAttack) return;
            
            playerData.CharacterSkillData.RotationDashSequence.Pause();
            
            if (playerData.Info.DeviceCompartment.SlotWeapon.Info.WeaponType == WeaponTypes.Sword)
            {
                characterSkill.SwordNormalAttack();
            }
            else if (playerData.Info.DeviceCompartment.SlotWeapon.Info.WeaponType == WeaponTypes.Gun)
            {
                characterSkill.GunNormalAttack();
            }
            else if (playerData.Info.DeviceCompartment.SlotWeapon.Info.WeaponType == WeaponTypes.Knuckle)
            {
                if (playerData.PlayerAnimation.IsAttacking) return;
                if (!playerData.CharacterSkillData.IsUseSkillPowerStone)
                {
                    if (!playerData.CharacterSkillData.PunchFinish) return;
                    playerData.CharacterSkillData.PunchFinish = false;
                    
                    playerData.CharacterSkillData.KnuckleLeft = true;
                    playerData.CharacterSkillData.KnuckleRight = false;
                    playerData.CharacterSkillData.KnuckleHeavyPunch = false;
                }
                
                characterSkill.KnuckleNormalAttack();
            }
        }

        private void AttackRight()
        {
            if (!playerData.CharacterSkillData.IsChangeWeaponFinish) return;
            if (playerData.CharacterControllerData.Dashing || playerData.CharacterSkillData.IsUseSkillCanNotAttack) return;

            if (playerData.Info.DeviceCompartment.SlotWeapon.Info.WeaponType == WeaponTypes.Knuckle)
            {
                if (playerData.CharacterSkillData.IsUseSkillPowerStone) return;
                
                if (playerData.PlayerAnimation.IsAttacking) return;
                playerData.CharacterSkillData.RotationDashSequence.Pause();
                
                if (!playerData.CharacterSkillData.PunchFinish) return;
                playerData.CharacterSkillData.PunchFinish = false;

                if (playerData.CharacterSkillData.KnuckleLeft)
                {
                    playerData.CharacterSkillData.KnuckleLeft = false;
                    playerData.CharacterSkillData.KnuckleRight = false;
                    playerData.CharacterSkillData.KnuckleHeavyPunch = true;
                }
                else
                {
                    playerData.CharacterSkillData.KnuckleLeft = false;
                    playerData.CharacterSkillData.KnuckleRight = true;
                    playerData.CharacterSkillData.KnuckleHeavyPunch = false;
                }

                characterSkill.KnuckleNormalAttack();
            }
        }

        public void WaitNextComboKnuckle()
        {
            PunchWaitTime?.Invoke();
        }
        
        #region AIM

        private void UsedAim()
        {
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;

            if (weapon.WeaponType == WeaponTypes.Knuckle)
            {            
                playerEvent.ShowVfxLine(false);
                indicatorCannonAttackRange.SetSize(playerData.CharacterSkillData.LimitIndicatorCannonAttackAimSize);
                anim.SetBool(playerData.PlayerAnimation.AnimIsAim, true);
            }
            else if (weapon.WeaponType == WeaponTypes.Gun)
            {
                if (!weapon.WeaponComponentLevel[0].UpgradeComponent2) return;
                
                anim.SetBool(playerData.PlayerAnimation.AnimIsAim, true);
            }

            playerEvent.ShowVfxLine(true);
            eventRightClickHoldPerformed.TriggerEvent();
            playerData.CharacterControllerData.Aiming = true;
            StartCoroutine(nameof(MouseDirection));
        }
        
        public void CancelAim()
        {
            playerEvent.ShowVfxLine(false);
            if (indicatorCannonAttackRange.gameObject.activeInHierarchy)
            {
                indicatorCannonAttackRange.SetSize(playerData.CharacterSkillData.LimitIndicatorCannonAttackSize);
            }
            
            anim.SetBool(playerData.PlayerAnimation.AnimIsAim, false);
            eventRightClickHoldCanceled.TriggerEvent();
            playerData.CharacterControllerData.Aiming = false;
        }

        #endregion
        
        /*private IEnumerator DashCooldown()
        {
            isDashCooldown = true;
            yield return new WaitForSeconds(playerData.CharacterControllerData.DashCooldown);
            isDashCooldown = false;
        }*/

        private IEnumerator MouseDirection()
        {
            while (playerData.CharacterControllerData.Aiming)
            {
                RaycastDirection();
                characterController.AimRotate();
                yield return null;
            }
        }

        /// <summary>
        /// Mouse direction.
        /// </summary>
        public void RaycastDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, playerData.CharacterControllerData.LayersInput))
            {
                playerData.CharacterControllerData.MouseDirection = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }
        
        /// <summary>
        /// Mouse vfx direction.
        /// </summary>
        public void RaycastVfxDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, playerData.CharacterControllerData.LayersInput))
            {
                playerData.CharacterControllerData.MouseVfxDirection = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(playerData.CharacterControllerData.MouseDirection, 2f);
        }
    }
}