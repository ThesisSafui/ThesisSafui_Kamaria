using System;
using System.Collections;
using DG.Tweening;
using Kamaria.Item;
using Kamaria.Player.Animation;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    internal sealed class CharacterController : MonoBehaviour
    {
        #region REFERENCE_VARIABLE

        [Header("Reference")] 
        [Tooltip("Info playerData as ScriptableObject")] 
        [SerializeField] private PlayerDataSO playerData;

        [Tooltip("Rigidbody component")] 
        [SerializeField] private Rigidbody controllerRb;

        [Tooltip("CapsuleCollider component")] 
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private CapsuleCollider colliderIFrame;

        #endregion

        #region PRIVATE_VARIABLE
        
        private Animator animator;
        private float speed;
        private Vector3 sphereGroundedPosition;
        private Transform cameraTransform;
        private RaycastHit slopeHit;
        private RaycastHit groundHit;
        private float currentDashTime;

        private float moveAttackTime;
        private float moveAttackAcceleration;
        
        #endregion

        private void OnEnable()
        {
            CharacterAwake();
        }

        private void FixedUpdate()
        {
            //MoveToGround();
            CheckSlope();
            Movement();
            GroundedCheck();
            ExtraGravity();
            ControlPhysicMaterial();
            //AimRotate();
        }

        private void CharacterAwake()
        {
            capsuleCollider.enabled = true;
            colliderIFrame.enabled = false;
            
            cameraTransform = Camera.main.transform;

            controllerRb.constraints = RigidbodyConstraints.FreezeRotation;
            controllerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            playerData.PlayerAnimation.Animator = GetComponent<Animator>();
            playerData.SetAnimator(out animator);
            playerData.CharacterControllerData.Dashing = false;
        }

        private void Movement()
        {
            if (playerData.IsDead)
            {
                return;
            }
            
            // Stop move if not on ground or dashing.
            if (!playerData.CharacterControllerData.IsGrounded || playerData.CharacterControllerData.Dashing) return;
            
            playerData.CharacterSkillData.RotationDashSequence.Pause();
            
            // Get speed.
            if (!playerData.PlayerAnimation.IsAnimationAttackNotMove)
            {
                speed = playerData.CharacterControllerData.WalkSpeed;
            }
            else
            {
                speed = 0;

                playerData.CharacterControllerData.SmoothInputVector = Vector2.Lerp(
                    playerData.CharacterControllerData.SmoothInputVector, Vector2.zero, 0.6f);
            }

            #region MOVE_DIRECTION_AND_SMOOTH_SPEED

            playerData.CharacterControllerData.SmoothInputVector =
                Vector2.SmoothDamp(playerData.CharacterControllerData.SmoothInputVector,
                    playerData.CharacterControllerData.MovementInput,
                    ref playerData.CharacterControllerData.SmoothInputVelocity,
                    playerData.CharacterControllerData.SmoothSpeed);

            playerData.CharacterControllerData.MoveDirection =
                new Vector3(playerData.CharacterControllerData.SmoothInputVector.x, 0,
                    playerData.CharacterControllerData.SmoothInputVector.y);

            playerData.CharacterControllerData.MoveDirection =
                playerData.CharacterControllerData.MoveDirection.x * cameraTransform.right.normalized +
                playerData.CharacterControllerData.MoveDirection.z * cameraTransform.forward.normalized;
            playerData.CharacterControllerData.MoveDirection.y = 0f;

            playerData.CharacterControllerData.TargetPosition =
                controllerRb.position +
                (playerData.CharacterControllerData.MoveDirection * (Time.deltaTime * speed));
            playerData.CharacterControllerData.TargetVelocity =
                (playerData.CharacterControllerData.TargetPosition - transform.position) / Time.deltaTime;
            playerData.CharacterControllerData.TargetVelocity.y = controllerRb.velocity.y;

            #endregion

            #region MOVE

            controllerRb.velocity = playerData.CharacterControllerData.OnSlope
                ? Vector3.ProjectOnPlane(playerData.CharacterControllerData.TargetVelocity, slopeHit.normal)
                : playerData.CharacterControllerData.TargetVelocity;

            #endregion
            
            animator.SetFloat(playerData.PlayerAnimation.AnimMove, Mathf.Abs(playerData.CharacterControllerData.SmoothInputVector.magnitude));
            
            CharacterRotate();
        }

        #region MOVE_SWORD_ATTACK
        
        /// <summary>
        /// Use with animation clip.
        /// </summary>
        /// <param name="acceleration"> Index sword move attacks. </param>
        public void SetMoveAttackAcceleration(int acceleration)
        {
            moveAttackAcceleration = playerData.CharacterSkillData.SwordMoveAttacks[acceleration].Acceleration;
            moveAttackTime = playerData.CharacterSkillData.SwordMoveAttacks[acceleration].Time;
        }
        
        /// <summary>
        /// Use with animation clip.
        /// </summary>
        public void MoveAttack()
        {
            StopCoroutine(nameof(MoveAttackTime));
            StartCoroutine(nameof(MoveAttackTime));
        }

        private IEnumerator MoveAttackTime()
        {
            while (moveAttackTime > 0)
            {
                if (!playerData.IsUsingUI)
                {
                    var acceleration = playerData.CharacterControllerData.OnSlope
                        ? Vector3.ProjectOnPlane(transform.forward.normalized *
                                                 moveAttackAcceleration, slopeHit.normal)
                        : transform.forward.normalized *
                          moveAttackAcceleration;

                    controllerRb.AddForce(acceleration, ForceMode.VelocityChange);

                    moveAttackTime -= Time.deltaTime;
                }
                yield return null;
            }
        }

        public void KnuckleDashAttack()
        {
            moveAttackAcceleration = playerData.CharacterSkillData.KnuckleMoveAttacks[Index.Start].Acceleration;
            moveAttackTime = playerData.CharacterSkillData.KnuckleMoveAttacks[Index.Start].Time;
            StartCoroutine(nameof(KnuckleMoveAttackTime));
        }
        
        private IEnumerator KnuckleMoveAttackTime()
        {
            capsuleCollider.enabled = false;
            colliderIFrame.enabled = true;
            
            while (moveAttackTime > 0)
            {
                if (!playerData.IsUsingUI)
                {
                    var acceleration = playerData.CharacterControllerData.OnSlope
                        ? Vector3.ProjectOnPlane(transform.forward.normalized *
                                                 moveAttackAcceleration, slopeHit.normal)
                        : transform.forward.normalized *
                          moveAttackAcceleration;

                    controllerRb.AddForce(acceleration, ForceMode.VelocityChange);

                    moveAttackTime -= Time.deltaTime;
                }
                yield return null;
            }
            
            capsuleCollider.enabled = true;
            colliderIFrame.enabled = false;
        }

        #endregion

        #region DASH

        internal void Dash()
        {
            if (playerData.CharacterControllerData.Dashing) return;
            
            GetDashDirection();
            StartCoroutine(nameof(Dashing));
            StartCoroutine(nameof(WaitTimeAttack));
        }

        private IEnumerator Dashing()
        {
            capsuleCollider.enabled = false;
            colliderIFrame.enabled = true;
            playerData.CharacterControllerData.Dashing = true;
            currentDashTime = playerData.CharacterControllerData.DashTime;

            playerData.CharacterSkillData.RotationAttackSequence.Pause();

            if (playerData.CharacterControllerData.MovementInput != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(playerData.CharacterControllerData.MovementInput.x,
                        playerData.CharacterControllerData.MovementInput.y) *
                    Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

                Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);

                playerData.CharacterSkillData.RotationDashSequence = DOTween.Sequence();
                playerData.CharacterSkillData.RotationDashSequence.Append
                    (transform.DORotate(rotation.eulerAngles, 0.01f).SetEase(Ease.Linear));

            }
            
            animator.SetBool(playerData.PlayerAnimation.AnimDash, true);
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);

            while (currentDashTime > 0)
            {
                controllerRb.velocity = playerData.CharacterControllerData.OnSlope ?
                    Vector3.ProjectOnPlane(playerData.CharacterControllerData.DashDirection.normalized *
                                           playerData.CharacterControllerData.DashAcceleration, slopeHit.normal)
                    : playerData.CharacterControllerData.DashDirection.normalized *
                      playerData.CharacterControllerData.DashAcceleration;
                
                currentDashTime -= Time.deltaTime;
                yield return null;
            }
            
            capsuleCollider.enabled = true;
            colliderIFrame.enabled = false;
            playerData.CharacterControllerData.Dashing = false;
            animator.SetBool(playerData.PlayerAnimation.AnimDash, false);
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, true);
        }

        private IEnumerator WaitTimeAttack()
        {
            playerData.PlayerAnimation.IsAttacking = true;
            float time = playerData.CharacterControllerData.DashTime + playerData.CharacterSkillData.DurationAttack;
            yield return new WaitForSeconds(time);
            playerData.PlayerAnimation.IsAttacking = false;
        }
        
        private void GetDashDirection()
        {
            if (playerData.CharacterControllerData.MovementInput == Vector2.zero)
            {
                playerData.CharacterControllerData.DashDirection = this.transform.forward;
            }
            else
            {
                //playerData.CharacterControllerData.DashDirection = playerData.CharacterControllerData.MoveDirection;
                playerData.CharacterControllerData.DashDirection =
                    new Vector3(playerData.CharacterControllerData.MovementInput.x, 0,
                        playerData.CharacterControllerData.MovementInput.y);
                playerData.CharacterControllerData.DashDirection =
                    playerData.CharacterControllerData.DashDirection.x * cameraTransform.right.normalized +
                    playerData.CharacterControllerData.DashDirection.z * cameraTransform.forward.normalized;
                playerData.CharacterControllerData.DashDirection.y = 0f;
            }
        }
        
        #endregion

        #region ROTATE

        private void CharacterRotate()
        {
            if (playerData.CharacterControllerData.MovementInput == Vector2.zero 
                || playerData.PlayerAnimation.IsAnimationAttackNotMove) return;

            float targetAngle = Mathf.Atan2(playerData.CharacterControllerData.MovementInput.x,
                    playerData.CharacterControllerData.MovementInput.y) *
                Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                ref playerData.CharacterControllerData.RotationVelocity,
                playerData.CharacterControllerData.RotationSmoothSpeed);

            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        public void AimRotate()
        {
            if (playerData.CharacterControllerData.Aiming)
            {
                Quaternion rotation =
                    Quaternion.LookRotation(new Vector3(playerData.CharacterControllerData.MouseDirection.x, 0,
                            playerData.CharacterControllerData.MouseDirection.z) - new Vector3(transform.position.x, 0,
                            transform.position.z),
                        Vector3.up);

                transform.rotation = Quaternion.Lerp(rotation, transform.rotation,
                    playerData.CharacterControllerData.AimDuration);
            }
        }

        #endregion

        #region GROUNDED_AND_GRAVITY_AND_PHYSIC_MATERIAL

        private void GroundedCheck()
        {
            #region CHECK_GROUNDED

            sphereGroundedPosition = new Vector3(transform.position.x,
                this.transform.position.y - playerData.CharacterControllerData.GroundedOffset,
                this.transform.position.z);
            playerData.CharacterControllerData.IsGrounded = Physics.CheckSphere(sphereGroundedPosition,
                playerData.CharacterControllerData.GroundedRadius, playerData.CharacterControllerData.GroundLayers,
                QueryTriggerInteraction.Ignore);

            #endregion

            #region CHECK_USE_DOUBLE_GRAVITY

            playerData.CharacterControllerData.UseDoubleExtraGravity = Physics.Raycast(sphereGroundedPosition,
                Vector3.down, playerData.CharacterControllerData.RayExtraGravity,
                playerData.CharacterControllerData.GroundLayers, QueryTriggerInteraction.Ignore);

            #endregion
        }
        
        private void MoveToGround()
        {
            if (!Physics.Raycast(transform.position, -Vector3.up, out groundHit,
                    playerData.CharacterControllerData.GroundLayers)) return;
            
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, groundHit.point.y, transform.position.z), 0.5f);
        }

        private void ExtraGravity()
        {
            if (playerData.CharacterControllerData.IsGrounded) return;
            
            //if (playerData.CharacterControllerData.UseDoubleExtraGravity) return;
            
            //UseExtraGravity(playerData.CharacterControllerData.UseDoubleExtraGravity ? 2 : 1);
            //UseExtraGravity(playerData.CharacterControllerData.UseDoubleExtraGravity ? 1 : 2);
            
            MoveToGround();
            UseExtraGravity(2);
        }
        
        private void CheckSlope()
        {
            if (Physics.Raycast(this.transform.position, Vector3.down, out slopeHit,
                    capsuleCollider.height / 2 + 0.5f, playerData.CharacterControllerData.GroundLayers))
            {
                playerData.CharacterControllerData.OnSlope = slopeHit.normal != Vector3.up;
            }
        }

        private void UseExtraGravity(int multiplied)
        {
            controllerRb.AddForce(
                transform.up * ((playerData.CharacterControllerData.ExtraGravity * multiplied) * Time.deltaTime),
                ForceMode.VelocityChange);
        }

        private void ControlPhysicMaterial()
        {
            capsuleCollider.material = playerData.CharacterControllerData.MovementInput == Vector2.zero ? 
                playerData.CharacterControllerData.NotMovingPhysicMaterial : 
                playerData.CharacterControllerData.MovingPhysicMaterial;
        }

        #endregion
    }
}