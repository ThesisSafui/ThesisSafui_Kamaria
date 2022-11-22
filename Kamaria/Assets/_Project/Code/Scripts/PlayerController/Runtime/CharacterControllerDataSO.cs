using UnityEngine;

namespace Kamaria.Player.Controller
{
    [CreateAssetMenu(fileName = "New CharacterControllerData", menuName = "ThesisSafui/Data/CharacterController")]
    public sealed class CharacterControllerDataSO : ScriptableObject
    {
        #region MOVEMENT

        public float WalkSpeed { get; set; }
        public float SmoothSpeed { get; set; }
        public float RotationSmoothSpeed { get; set; }
        
        [Header("Move")] 
        [Tooltip("Walk speed of the character.")] 
        [SerializeField] private float walkSpeed = 0;
        
        [Tooltip("Smooth speed of the character.")] 
        [SerializeField] private float smoothSpeed = 0;

        [Tooltip("Rotation smooth speed of the character")] 
        [SerializeField] private float rotationSmoothSpeed;

        #endregion

        #region GROUNDED_AND_EXTRA_GRAVITY_AND_SLOPE

        public float ExtraGravity { get; set; }
        public float GroundedOffset { get; set; }
        public float GroundedRadius { get; set; }
        public LayerMask GroundLayers { get; set; }
        public LayerMask LayersInput { get; set; }
        public Color ColorGizmosGrounded { get; set; }
        public Color ColorGizmosNotGrounded { get; set; }

        [Header("Grounded")] 
        [Tooltip("Character is grounded or not.")] 
        public bool IsGrounded = false;
        
        [Tooltip("Character is use double extra gravity or not.")] 
        public bool UseDoubleExtraGravity = false;
        
        [Tooltip("Character is on slope or not.")] 
        public bool OnSlope = false;

        [Tooltip("What layers the character used as ground.")] 
        [SerializeField] private LayerMask groundLayers = default;
        
        [Tooltip("What layers the character input.")] 
        [SerializeField] private LayerMask layersInput = default;

        [Tooltip("Grounded position Y")] 
        [SerializeField] private float groundedOffset = 0;

        [Tooltip("The radius of the grounded check.")]
        [SerializeField] private float groundedRadius = 0;
        
        [Tooltip("Color gizmos if character grounded.")] 
        [SerializeField] private Color colorGizmosGrounded = Color.green;

        [Tooltip("Color gizmos if character not grounded.")] 
        [SerializeField] private Color colorGizmosNotGrounded = Color.red;

        [Header("Gravity")]
        [Tooltip("Apply extra gravity when the character is not grounded")] 
        [SerializeField] private float extraGravity;
        
        #endregion

        #region PHYSICS_RAYCAST
        
        public float RayExtraGravity { get; set; }
        public Color ColorGizmosDoubleExtraGravity { get; set; }
        
        [Header("Physics Raycast")]
        [Tooltip("Raycast check use air control.")]
        [SerializeField] private float rayExtraGravity = 1.8f;

        [Tooltip("Color gizmos if extra gravity.")]
        [SerializeField] private Color colorGizmosDoubleExtraGravity = Color.black;

        #endregion

        #region PHYSIC_MATERIAL

        public PhysicMaterial MovingPhysicMaterial { get; set; }
        public PhysicMaterial NotMovingPhysicMaterial { get; set; }
        
        [Header("Physic Material")]
        [Tooltip("Physic material when moving.")]
        [SerializeField] private PhysicMaterial movingPhysicMaterial;
        
        [Tooltip("Physic material when not moving.")]
        [SerializeField] private PhysicMaterial notMovingPhysicMaterial;

        #endregion
        
        #region READ_ONLY_INSPECTOR_VARIABLE
        
        #region MOVEMENT_VARIABLE

        [Header("Movement value")]
        public Vector2 MovementInput;
        public Vector3 MoveDirection;
        public Vector3 DashDirection;
        public Vector3 TargetPosition;
        public Vector3 TargetVelocity;

        #endregion

        #region SMOOTH_VARIABLE
        
        [Header("Smooth value")]
        public Vector2 SmoothInputVector;
        public Vector2 SmoothInputVelocity;
        public float RotationVelocity;

        #endregion

        #endregion

        #region DASH

        public float DashTime { get; set; }
        public float DashAcceleration { get; set; }
        public float DashCooldown { get; set; }
        
        [Header("Dash Setting")] 
        
        [Tooltip("Player dashing or not.")]
        public bool Dashing = false;

        [Tooltip("Dash time.")]
        [SerializeField] private float dashTime;
        
        [Tooltip("Dash acceleration.")]
        [SerializeField] private float dashAcceleration;
        
        [Tooltip("Dash acceleration.")]
        [SerializeField] private float dashCooldown;
        
        #endregion

        #region MOUSE

        public float AimDuration { get; set; }
        
        [Header("Mouse")]
        [Tooltip("Mouse position")] 
        public Vector3 MouseDirection;
        
        public Vector3 MouseVfxDirection;

        [Tooltip("Player aiming")]
        public bool Aiming;

        [Tooltip("Aim duration")] 
        [SerializeField] [Range(0, 0.99f)] private float aimDuration;

        #endregion

        private void OnEnable()
        {
            GetData();
        }
        
#if UNITY_EDITOR
        // Use edit in Inspector
        private void OnValidate()
        {
            GetData();
        }
#endif
        
        private void GetData()
        {
            WalkSpeed = walkSpeed;
            SmoothSpeed = smoothSpeed;
            RotationSmoothSpeed = rotationSmoothSpeed;

            ExtraGravity = extraGravity;
            GroundedOffset = groundedOffset;
            GroundedRadius = groundedRadius;
            GroundLayers = groundLayers;
            LayersInput = layersInput;
            ColorGizmosGrounded = colorGizmosGrounded;
            ColorGizmosNotGrounded = colorGizmosNotGrounded;
            
            RayExtraGravity = rayExtraGravity;
            ColorGizmosDoubleExtraGravity = colorGizmosDoubleExtraGravity;

            MovingPhysicMaterial = movingPhysicMaterial;
            NotMovingPhysicMaterial = notMovingPhysicMaterial;

            DashTime = dashTime;
            DashAcceleration = dashAcceleration;
            DashCooldown = dashCooldown;

            AimDuration = aimDuration;
        }
    }
}