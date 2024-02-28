using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GnomeCrawler.Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        #region constants
        const float _rotationFactorPerFrame = 15.0f;
        const float _runMultiplier = 1.5f;
        const int _zero = 0;
        #endregion

        #region classes
        CharacterController _characterController;
        Animator _animator;
        PlayerControls _playerInput;
        Camera _mainCam;
        #endregion

        #region movement
        float _moveSpeed = 2.5f;
        Vector2 _currentMovementInput;
        Vector3 _currentMovement;
        Vector3 _appliedMovement;
        Vector3 _cameraRelativeMovement;
        bool _isMovementPressed;
        bool _isRunPressed;
        #endregion

        #region jumping
        bool _isJumpPressed = false;
        float _initialJumpVelocity;
        float _initialGravity;
        float _maxJumpHeight = 1.0f;
        float _maxJumpTime = .75f;
        bool _isJumping = false;
        bool _requireNewJumpPress = false;
        #endregion

        #region combat
        bool _isAttackPressed;
        bool _isAttackFinished = true;
        #endregion

        #region state variables
        PlayerBaseState _currentState;
        PlayerStateFactory _states;
        #endregion

        #region hash
        int _speedHash;
        int _isFallingHash;
        int _isJumpingHash;
        int _isAttackingHash;
        #endregion

        // gravity
        float _gravity = -9.8f;

        // temp
        public int health = 10;

        #region getters and setters
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public Animator Animator { get { return _animator; } }
        public CharacterController CharacterController { get { return _characterController; } }
        public Camera MainCam { get { return _mainCam; } }
        public int SpeedHash { get { return _speedHash; } }
        public int IsFallingHash { get { return _isFallingHash; } }
        public int IsJumpingHash { get { return _isJumpingHash; } }
        public int IsAttackingHash { get { return _isAttackingHash; } }
        public bool IsMovementPressed { get { return _isMovementPressed; } }
        public bool IsRunPressed { get { return _isRunPressed; } }
        public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
        public bool IsJumping { set { _isJumping = value; } }
        public bool IsJumpPressed { get { return _isJumpPressed; } }
        public bool IsAttackPressed { get { return _isAttackPressed; } }
        public bool IsAttackFinished { get { return _isAttackFinished; } set { _isAttackFinished = value; } }
        public float InitialJumpVelocity { get { return _initialJumpVelocity; } set { _initialJumpVelocity = value; } }
        public float InitialGravity { get { return _initialGravity; } set { _initialGravity = value; } }
        public float Gravity { get { return _gravity; } }
        public float MoveSpeed { get { return _moveSpeed; } }
        public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
        public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
        public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
        public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
        public float RunMultiplier { get { return _runMultiplier; } }
        public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
        #endregion

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
            _speedHash = Animator.StringToHash("speed");
            _isFallingHash = Animator.StringToHash("isFalling");
            _isJumpingHash = Animator.StringToHash("isJumping");
            _isAttackingHash = Animator.StringToHash("isAttacking");

            // set player input callbacks
            _playerInput.Player.Move.started += OnMovementInput;
            _playerInput.Player.Move.canceled += OnMovementInput;
            _playerInput.Player.Move.performed += OnMovementInput;
            _playerInput.Player.Jump.performed += OnJump;
            _playerInput.Player.Jump.canceled += OnJump;
            _playerInput.Player.Sprint.started += OnRun;
            _playerInput.Player.Sprint.canceled += OnRun;
            _playerInput.Player.Attack.started += OnAttack;
            _playerInput.Player.Attack.canceled += OnAttack;

            SetupJumpVariables();
        }

        void SetupJumpVariables()
        {
            float timeToApex = _maxJumpTime / 2;
            _initialGravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        }

        private void Start()
        {
            _characterController.Move(_appliedMovement * _moveSpeed * Time.deltaTime);
        }

        private void Update()
        {
            HandleRotation();
            _currentState.UpdateStates();
            /*print(_currentState);
            print(_currentState._currentSubState);*/

            _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
            _characterController.Move(_cameraRelativeMovement * _moveSpeed * Time.deltaTime);

            if (health <= 0)
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
            if (!_isAttackFinished) return;

            Vector3 positionToLookAt;

            positionToLookAt.x = _cameraRelativeMovement.x;
            positionToLookAt.y = _zero;
            positionToLookAt.z = _cameraRelativeMovement.z;

            Quaternion currentRotation = transform.rotation;

            if (_isMovementPressed)
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
        private void OnAttack(InputAction.CallbackContext context)
        {
            //StartCoroutine(Attack());
            _isAttackPressed = context.ReadValueAsButton();
        }

        private void OnEnable()
        {
            _playerInput.Player.Enable();
        }

        private void OnDisable()
        {
            _playerInput.Player.Disable();
        }

        public void AnimationFinished(string animName)
        {
            print (animName + " animation finished");

            if (animName == "Attack") _isAttackFinished = true;
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
        }
        /*
        private IEnumerator Attack()
        {
            GetComponentInChildren<PlayerWeaponHitBox>().StartDealDamage();
            yield return new WaitForSeconds(1);
            GetComponentInChildren<PlayerWeaponHitBox>().StopDealDamage();
        }*/
    }

}
