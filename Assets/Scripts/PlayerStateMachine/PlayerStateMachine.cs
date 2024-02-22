using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GnomeCrawler
{
    public class PlayerStateMachine : MonoBehaviour
    {
        CharacterController _characterController;
        Animator _animator;
        PlayerControls _playerInput;
        Camera _mainCam;

        Vector2 _currentMovementInput;
        Vector3 _currentMovement;
        Vector3 _appliedMovement;
        Vector3 _cameraRelativeMovement;
        bool _isMovementPressed;
        bool _isRunPressed;

        float _rotationFactorPerFrame = 15.0f;
        float _runMultiplier = 4.0f;
        int  _zero = 0;

        bool _isJumpPressed = false;
        float _initialJumpVelocity;
        float _initialGravity;
        float _maxJumpHeight = 1.0f;
        float _maxJumpTime = .75f;
        bool _isJumping = false;
        int _isJumpingHash;
        int _jumpCountHash;
        bool _requireNewJumpPress = false;
        int _jumpCount = 0;
        //Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
        //Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
        //Coroutine _currentJumpResetRoutine = null;

        // state variables
        PlayerBaseState _currentState;
        PlayerStateFactory _states;

        // hash
        int _isWalkingHash;
        int _isRunningHash;
        int _isFallingHash;

        // gravity
        float _gravity = -9.8f;

        // temp
        public int health = 10;

        // getters and setters
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public Animator Animator { get { return _animator; } }
        public CharacterController CharacterController { get { return _characterController; } }
        public Camera MainCam { get { return _mainCam; } }
        //public Coroutine CurrentJumpResetRoutine { get { return _currentJumpResetRoutine; } set { _currentJumpResetRoutine = value; } }
        //public Dictionary<int, float> InitialJumpVelocities { get { return _initialJumpVelocities; } }
        //public Dictionary<int, float> JumpGravities { get { return _jumpGravities; } }
        public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }
        public int IsWalkingHash { get { return _isWalkingHash; } }
        public int IsRunningHash { get { return _isRunningHash; } }
        public int IsFallingHash { get { return _isFallingHash; } }
        public int IsJumpingHash { get { return _isJumpingHash; } }
        public int JumpCountHash { get { return _jumpCountHash; } }
        public bool IsMovementPressed { get { return _isMovementPressed; } }
        public bool IsRunPressed { get { return _isRunPressed; } }
        public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
        public bool IsJumping { set { _isJumping = value; } }
        public bool IsJumpPressed { get { return _isJumpPressed; } }
        public float InitialJumpVelocity { get { return _initialJumpVelocity; } set { _initialJumpVelocity = value; } }
        public float InitialGravity { get { return _initialGravity; } set { _initialGravity = value; } }
        public float Gravity { get { return _gravity; } }
        public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
        public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
        public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
        public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
        public float RunMultiplier { get { return _runMultiplier; } }
        public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }

        private void Awake()
        {
            // initialised reference variables
            _playerInput = new PlayerControls();
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _mainCam = Camera.main;

            // Setup States
            _states = new PlayerStateFactory(this);
            _currentState = _states.Grounded();
            _currentState.EnterState();

            // set parameter hash references
            _isWalkingHash = Animator.StringToHash("isWalking");
            _isRunningHash = Animator.StringToHash("isRunning");
            _isFallingHash = Animator.StringToHash("isFalling");
            _isJumpingHash = Animator.StringToHash("isJumping");
            _jumpCountHash = Animator.StringToHash("jumpCount");

            // set player input callbacks
            _playerInput.Player.Move.started += OnMovementInput;
            _playerInput.Player.Move.canceled += OnMovementInput;
            _playerInput.Player.Move.performed += OnMovementInput;
            _playerInput.Player.Jump.performed += OnJump;
            _playerInput.Player.Jump.canceled += OnJump;
            _playerInput.Player.Sprint.started += OnRun;
            _playerInput.Player.Sprint.canceled += OnRun;

            //temp
            _playerInput.Player.Attack.started += OnAttack;

            SetupJumpVariables();
        }

        void SetupJumpVariables()
        {
            float timeToApex = _maxJumpTime / 2;
            _initialGravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
            //float secondJumpGravity = (-2 * (_maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
            //float secondJumpInitialVelocity = (2 * (_maxJumpHeight + 2)) / (timeToApex * 1.25f);
            //float thirdJumpGravity = (-2 * (_maxJumpHeight + 4)) / Mathf.Pow((timeToApex * 1.5f), 2);
            //float thirdJumpInitialVelocity = (2 * (_maxJumpHeight + 4)) / (timeToApex * 1.5f);

            //_initialJumpVelocities.Add(1, _initialJumpVelocity);
            //_initialJumpVelocities.Add(2, secondJumpInitialVelocity);
            //_initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

            //_jumpGravities.Add(0, initialGravity);
            //_jumpGravities.Add(1, initialGravity);
            //_jumpGravities.Add(2, secondJumpGravity);
            //_jumpGravities.Add(3, thirdJumpGravity);
        }

        private void Start()
        {
            _characterController.Move(_appliedMovement * Time.deltaTime);
        }

        private void Update()
        {
            HandleRotation();
            _currentState.UpdateStates();

            _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
            _characterController.Move(_cameraRelativeMovement * Time.deltaTime);

            if(health <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
        {
            float currentYValue = vectorToRotate.y;

            Vector3 cameraForward = _mainCam.transform.forward;
            Vector3 cameraRight = _mainCam.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward = cameraForward.normalized;
            cameraRight = cameraRight.normalized;

            Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
            Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

            Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
            vectorRotatedToCameraSpace.y = currentYValue;
            return vectorRotatedToCameraSpace;
        }

        void HandleRotation()
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = _cameraRelativeMovement.x;
            positionToLookAt.y = _zero;
            positionToLookAt.z = _cameraRelativeMovement.z;

            Quaternion currentRotation = transform.rotation;

            if (_isMovementPressed )
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
            }
        }

        void OnMovementInput(InputAction.CallbackContext context)
        {
            _currentMovementInput = context.ReadValue<Vector2>();
            _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }

        void OnJump(InputAction.CallbackContext context)
        {
            _isJumpPressed = context.ReadValueAsButton();
            _requireNewJumpPress = false;
        }

        void OnRun(InputAction.CallbackContext context)
        {
            _isRunPressed = context.ReadValueAsButton();
        }

        private void OnEnable()
        {
            _playerInput.Player.Enable();
        }

        private void OnDisable()
        {
            _playerInput.Player.Disable();
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            StartCoroutine(Attack());
            
        }

        private IEnumerator Attack()
        {
            GetComponentInChildren<PlayerWeaponHitBox>().StartDealDamage();
            yield return new WaitForSeconds(1);
            GetComponentInChildren<PlayerWeaponHitBox>().StopDealDamage();
        }
    }

}
