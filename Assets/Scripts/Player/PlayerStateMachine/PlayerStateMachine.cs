using Cinemachine;
using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GnomeCrawler.Player
{
    // State Machine Heavily Inspired By IHeartGameDev https://www.youtube.com/c/iHeartGameDev/videos
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
        [SerializeField] private StatsSO _playerStats;
        #endregion

        #region lock on
        [Header("Lock On")]
        [SerializeField] CinemachineVirtualCamera lockOnCam;
        [SerializeField] Animator camAnimator;
        [SerializeField] LayerMask _targetLayers;
        [SerializeField] LayerMask _environmentLayers;
        [SerializeField] Transform _playerLockTransform;

        List<CombatBrain> _avaliableTargets = new List<CombatBrain>();
        CombatBrain _currentLockOnTarget;
        CombatBrain _nearestLockOnTarget;
        CombatBrain _leftLockOnTarget;
        CombatBrain _rightLockOnTarget;
        float _lockOnRadius = 30.0f;
        float _minimumViewableAngle = -50.0f;
        float _maximumViewableAngle = 50.0f;
        bool _isLockedOn = false;
        #endregion

        #region movement
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
        float _maxJumpHeight = .75f;
        float _maxJumpTime = .75f;
        bool _requireNewJumpPress = false;
        #endregion

        #region dodging
        float _dodgeForce = 2f;
        float _dodgeVelocity = 1f;
        float _dodgeDuration = 0.5f;
        float _dodgeCooldown = 0.75f;
        bool _isDodgePressed;
        bool _isDodging = false;
        bool _canDodge = true;
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
        int _isDodgingHash;
        #endregion

        // gravity
        float _gravity = -4.9f;

        #region getters and setters
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public Animator Animator { get { return _animator; } }
        public CharacterController CharacterController { get { return _characterController; } }
        public Camera MainCam { get { return _mainCam; } }
        public StatsSO PlayerStats { get { return _playerStats; } }
        public int SpeedHash { get { return _speedHash; } }
        public int IsFallingHash { get { return _isFallingHash; } }
        public int IsJumpingHash { get { return _isJumpingHash; } }
        public int IsAttackingHash { get { return _isAttackingHash; } }
        public int IsDodgingHash { get { return _isDodgingHash; } }
        public bool IsMovementPressed { get { return _isMovementPressed; } }
        public bool IsRunPressed { get { return _isRunPressed; } }
        public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
        public bool IsJumpPressed { get { return _isJumpPressed; } }
        public bool IsAttackPressed { get { return _isAttackPressed; } }
        public bool IsDodgePressed { get { return _isDodgePressed; } }
        public bool IsAttackFinished { get { return _isAttackFinished; } set { _isAttackFinished = value; } }
        public float InitialJumpVelocity { get { return _initialJumpVelocity; } set { _initialJumpVelocity = value; } }
        public float InitialGravity { get { return _initialGravity; } set { _initialGravity = value; } }
        public float Gravity { get { return _gravity; } }
        public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
        public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
        public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
        public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
        public float RunMultiplier { get { return _runMultiplier; } }
        public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
        public float DodgeVelocity { get { return _dodgeVelocity; } set { _dodgeVelocity = value; } }
        public float DodgeDuration { get => _dodgeDuration; }
        public float DodgeCooldown { get => _dodgeCooldown; }
        public float DodgeForce { get => _dodgeForce; }
        public bool IsDodging { get => _isDodging; set => _isDodging = value; }
        public bool CanDodge { get => _canDodge; set => _canDodge = value; }
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
            _isDodgingHash = Animator.StringToHash("isDodging");

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
            _playerInput.Player.Dodge.started += OnDodge;
            _playerInput.Player.Dodge.canceled += OnDodge;

            _playerInput.Player.LockOn.performed += CameraLockOn;
            _playerInput.Player.SeekLeftLockOnTagret.performed += LeftLockOnSwap;
            _playerInput.Player.SeekRightLockOnTagret.performed += RightLockOnSwap;

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
            _characterController.Move(_appliedMovement * _playerStats.GetStat(Stat.MoveSpeed) * Time.deltaTime);
        }

        private void Update()
        {
            HandleRotation();
            _currentState.UpdateStates();

            _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
            _characterController.Move(_cameraRelativeMovement * _playerStats.GetStat(Stat.MoveSpeed) * _dodgeVelocity * Time.deltaTime);
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
            Quaternion currentRotation = transform.rotation;

            if (_isLockedOn && !_isDodging)
            {
                positionToLookAt = _currentLockOnTarget.transform.position - transform.position;
                positionToLookAt.Normalize();
                positionToLookAt.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);

            }
            else
            {
                positionToLookAt.x = _cameraRelativeMovement.x;
                positionToLookAt.y = _zero;
                positionToLookAt.z = _cameraRelativeMovement.z;

                if (_isMovementPressed)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                    transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
                }
            }
        }

        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(_playerLockTransform.position, _lockOnRadius, _targetLayers);

            for (int i = 0; i < colliders.Length; i++)
            {
                CombatBrain lockOnTarget = colliders[i]?.GetComponent<CombatBrain>();

                if (lockOnTarget != null)
                {
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - _playerLockTransform.position;
                    float distanceFromPlayer = Vector3.Distance(_playerLockTransform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, _mainCam.transform.forward);

                    if (viewableAngle > _minimumViewableAngle && viewableAngle < _maximumViewableAngle)
                    {
                        RaycastHit hit;

                        if (Physics.Linecast(_playerLockTransform.position, lockOnTarget._lockOnTransform.position, out hit, _environmentLayers))
                        {
                            continue;
                        }
                        else
                        {
                            _avaliableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            for (int k = 0; k < _avaliableTargets.Count; k++)
            {
                if (_avaliableTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(_playerLockTransform.position, _avaliableTargets[k].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        _nearestLockOnTarget = _avaliableTargets[k];
                    }

                    if (_isLockedOn)
                    {
                        Vector3 relativeEnemyPosition = transform.InverseTransformPoint(_avaliableTargets[k].transform.position);

                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.y;

                        if (_avaliableTargets[k] == _currentLockOnTarget)
                        {
                            continue;
                        }

                        if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            _leftLockOnTarget = _avaliableTargets[k];
                        }
                        else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            _rightLockOnTarget = _avaliableTargets[k];
                        }
                    }
                }

                else
                {
                    ClearLockOnTargets();
                    _isLockedOn = false;
                }
            }
        }

        private void HandleLockOnSwitchTarget(CombatBrain directionOfLockOn)
        {
            if (_isLockedOn)
            {
                HandleLocatingLockOnTargets();

                if (directionOfLockOn != null)
                {
                    _currentLockOnTarget = directionOfLockOn;
                    lockOnCam.LookAt = _currentLockOnTarget._lockOnTransform;
                }
            }
        }

        public void ClearLockOnTargets()
        {
            _currentLockOnTarget = null;
            _nearestLockOnTarget = null;
            _leftLockOnTarget = null;
            _rightLockOnTarget= null;
            _avaliableTargets.Clear();
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
            _isAttackPressed = context.ReadValueAsButton();
        }
        private void OnDodge(InputAction.CallbackContext context)
        {
            _isDodgePressed = context.ReadValueAsButton();
        }

        private void CameraLockOn(InputAction.CallbackContext context)
        {
/*            HandleLocatingLockOnTargets();

            if (_nearestLockOnTarget != null)
            {
                _currentLockOnTarget = _nearestLockOnTarget;
                _isLockedOn = true;
                lockOnCam.LookAt = _currentLockOnTarget._lockOnTransform;
                camAnimator.Play("LockCam");
            }
            else
            {
                camAnimator.Play("FollowCam");
            }
*/
            if (!_isLockedOn)
            {
                HandleLocatingLockOnTargets();
                if (_nearestLockOnTarget == null) return;
                _currentLockOnTarget = _nearestLockOnTarget;
                _isLockedOn = true;
                lockOnCam.LookAt = _currentLockOnTarget._lockOnTransform;
                camAnimator.Play("LockCam");
            }
            else
            {
                HandleLocatingLockOnTargets();
                _isLockedOn = false;
                camAnimator.Play("FollowCam");
            }
        }

        private void RightLockOnSwap(InputAction.CallbackContext context)
        {
            HandleLockOnSwitchTarget(_rightLockOnTarget);
        }

        private void LeftLockOnSwap(InputAction.CallbackContext context)
        {
            HandleLockOnSwitchTarget(_leftLockOnTarget);
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
    }

}
