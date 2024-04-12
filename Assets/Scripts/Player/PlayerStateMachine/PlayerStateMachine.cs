using Cinemachine;
using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GnomeCrawler.Player
{
    // State Machine Heavily Inspired By IHeartGameDev https://www.youtube.com/c/iHeartGameDev/videos
    public class PlayerStateMachine : MonoBehaviour
    {
        public Analytics analyticsScript;

        #region constants
        const float _rotationFactorPerFrame = 15.0f;
        const float _runMultiplier = 1.2f;
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
        [SerializeField] CinemachineFreeLook followCam;
        [SerializeField] Animator camAnimator;
        [SerializeField] LayerMask _targetLayers;
        [SerializeField] LayerMask _environmentLayers;
        [SerializeField] Transform _playerLockTransform;
        [SerializeField] GameObject _lockOnImage;

        List<CombatBrain> _avaliableTargets = new List<CombatBrain>();
        CombatBrain _currentLockOnTarget;
        CombatBrain _nearestLockOnTarget;
        CombatBrain _leftLockOnTarget;
        CombatBrain _rightLockOnTarget;
        float _lockOnRadius = 25.0f;
        float _minimumViewableAngle = -60.0f;
        float _maximumViewableAngle = 60.0f;
        bool _isLockedOn = false;
        Coroutine _lockOnCoroutine;
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
        float _maxJumpHeight = 2f;
        float _maxJumpTime = .75f;
        bool _requireNewJumpPress = false;
        #endregion

        #region dodging
        float _dodgeForce = 1.5f;
        float _dodgeVelocity = 1f;
        float _dodgeDuration = 0.5f;
        float _dodgeCooldown = 0.75f;
        float _miniDodgeCooldown = 0.1f;
        int _dodgeNumber = 0;
        bool _isDodgePressed;
        bool _isDodging = false;
        bool _canDodge = true;
        Coroutine _resetDodgeCoroutine;
        #endregion

        #region combat
        bool _isAttackPressed;
        bool _isAttackFinished = true;
        bool _canMoveWhileAttacking = false;
        int _chainAttackNumber = 0;
        Coroutine _resetChainAttackCoroutine;
        bool _isFlinching = false;
        bool _isFlinchFinished = true;
        bool _isInvincible = false;
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
        int _attackNumberHash;
        int _flinchHash;
        #endregion

        // gravity
        float _gravity = -9.8f;

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
        public int AttackNumberHash { get => _attackNumberHash; set => _attackNumberHash = value; }
        public int FlinchHash { get => _flinchHash; set => _flinchHash = value; }
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
        public Vector3 CameraRelativeMovement { get => _cameraRelativeMovement; set => _cameraRelativeMovement = value; }
        public bool CanMoveWhileAttacking { get => _canMoveWhileAttacking; set => _canMoveWhileAttacking = value; }
        public int ChainAttackNumber { get => _chainAttackNumber; set => _chainAttackNumber = value; }
        public Coroutine ResetChainAttackCoroutine { get => _resetChainAttackCoroutine; set => _resetChainAttackCoroutine = value; }
        public int DodgeNumber { get => _dodgeNumber; set => _dodgeNumber = value; }
        public float MiniDodgeCooldown { get => _miniDodgeCooldown; set => _miniDodgeCooldown = value; }
        public Coroutine ResetDodgeCoroutine { get => _resetDodgeCoroutine; set => _resetDodgeCoroutine = value; }
        public bool IsFlinching { get => _isFlinching; set => _isFlinching = value; }
        public bool IsInvincible { get => _isInvincible; set => _isInvincible = value; }
        public bool IsFlinchFinished { get => _isFlinchFinished; set => _isFlinchFinished = value; }
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
            _attackNumberHash = Animator.StringToHash("attackNumber");
            _flinchHash = Animator.StringToHash("flinch");

            // set player input callbacks
            _playerInput.Player.Move.started += OnMovementInput;
            _playerInput.Player.Move.canceled += OnMovementInput;
            _playerInput.Player.Move.performed += OnMovementInput;
            //_playerInput.Player.Jump.performed += OnJump;
            //_playerInput.Player.Jump.canceled += OnJump;
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
            transform.parent = transform.root;
            _characterController.Move(_appliedMovement * _playerStats.GetStat(Stat.MoveSpeed) * Time.deltaTime);
        }

        private void Update()
        {
            //Debug.Log(Animator.GetCurrentAnimatorStateInfo(0).IsName("Flinch"));
            HandleLockOnStatus();
            HandleRotation();
            _currentState.UpdateStates();
            //print(_currentState);
            //print(_currentState._currentSubState);


            _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
            _cameraRelativeMovement.x = _cameraRelativeMovement.x * _playerStats.GetStat(Stat.MoveSpeed) * _dodgeVelocity;
            _cameraRelativeMovement.z = _cameraRelativeMovement.z * _playerStats.GetStat(Stat.MoveSpeed) * _dodgeVelocity;
            _characterController.Move(_cameraRelativeMovement * Time.deltaTime);
        }

        private void HandleLockOnStatus()
        {
            if (_isLockedOn)
            {
                if (_currentLockOnTarget == null)
                {
                    SetLockOnStatus(false);
                    return;
                }

                if (_currentLockOnTarget.IsDead)
                {
                    SetLockOnStatus(false);
                    return;
                }

                _lockOnImage.SetActive(true);
                _lockOnImage.transform.position = _mainCam.WorldToScreenPoint(_currentLockOnTarget._lockOnTransform.transform.position);
            }
            else
            {
                _lockOnImage.SetActive(false);
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
            if (!_isAttackFinished && !_canMoveWhileAttacking) return;

            Vector3 positionToLookAt;
            Quaternion currentRotation = transform.rotation;

            if (_isLockedOn && !_isDodging && !_isRunPressed)
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

            Collider[] colliders = Physics.OverlapSphere(transform.position, _lockOnRadius, _targetLayers);

            for (int i = 0; i < colliders.Length; i++)
            {
                CombatBrain lockOnTarget = colliders[i]?.GetComponent<CombatBrain>();

                if (lockOnTarget != null)
                {
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - transform.position;
                    float distanceFromTarget = Vector3.Distance(transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, _mainCam.transform.forward);

                    if (lockOnTarget.IsDead)
                    {
                        continue;
                    }

                    if (lockOnTarget.transform == transform)
                    {
                        continue;
                    }

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
                    float distanceFromTarget = Vector3.Distance(transform.position, _avaliableTargets[k].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        _nearestLockOnTarget = _avaliableTargets[k];
                    }

                    if (_isLockedOn)
                    {
                        Vector3 relativeEnemyPosition = transform.InverseTransformPoint(_avaliableTargets[k].transform.position);

                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        if (_avaliableTargets[k] == _currentLockOnTarget)
                        {
                            print(_avaliableTargets[k] + " is current target");
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
                    SetLockOnStatus(false);
                }
            }
        }

        public void ClearLockOnTargets()
        {
            print("clear lockon tagets");
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
            string buttonName = context.action.name;
            analyticsScript.TrackButtonPress(buttonName);
        }
        private void OnDodge(InputAction.CallbackContext context)
        {
            _isDodgePressed = context.ReadValueAsButton();
        }

        private void CameraLockOn(InputAction.CallbackContext context)
        {
            if (_isLockedOn)
            {
                ClearLockOnTargets();
                SetLockOnStatus(false);
                return;
            }
            if (!_isLockedOn)
            {
                HandleLocatingLockOnTargets();

                if (_nearestLockOnTarget != null)
                {
                    _currentLockOnTarget = _nearestLockOnTarget;
                    SetLockOnStatus(true);
                } 
            }
        }

        private void RightLockOnSwap(InputAction.CallbackContext context)
        {
            if (_isLockedOn)
            {
                ClearLockOnTargets();
                HandleLocatingLockOnTargets();

                if (_rightLockOnTarget != null)
                {
                    print("right target exists");
                    _currentLockOnTarget = _rightLockOnTarget;
                    lockOnCam.LookAt = _currentLockOnTarget._lockOnTransform;
                }
            }
        }

        private void LeftLockOnSwap(InputAction.CallbackContext context)
        {
            if (_isLockedOn)
            {
                ClearLockOnTargets();
                HandleLocatingLockOnTargets();

                if (_leftLockOnTarget != null)
                {
                    print("left target exists");
                    _currentLockOnTarget = _leftLockOnTarget;
                    lockOnCam.LookAt = _currentLockOnTarget._lockOnTransform;
                }
            }
        }

        private void SetLockOnStatus(bool isLocked)
        {
            if (isLocked)
            {
                followCam.gameObject.SetActive(false);

                lockOnCam.LookAt = _currentLockOnTarget._lockOnTransform;
                //followCam.LookAt = _currentLockOnTarget._lockOnTransform;
                camAnimator.Play("LockCam");
                _isLockedOn = true;
            }
            else if (!isLocked)
            {
                followCam.transform.position = lockOnCam.transform.position;
                followCam.gameObject.SetActive(true);
                //followCam.LookAt = _playerLockTransform;
                camAnimator.Play("FollowCam");
                _isLockedOn = false;
            }
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

            if (animName == "Attack")
            {
                _isAttackFinished = true;
                ResetChainAttackCoroutine = StartCoroutine(ResetChainAttack());
            }
            if (animName == "Flinch")
            {
                _isFlinchFinished = true;
            }
        }
        public IEnumerator WaitThenFindNewTarget()
        {
            while (!_currentLockOnTarget.IsDead)
            {
                yield return null;
            }

            ClearLockOnTargets();
            HandleLocatingLockOnTargets();

            if (_nearestLockOnTarget != null)
            {
                _currentLockOnTarget = _nearestLockOnTarget;
                SetLockOnStatus(true);
            }

            yield return null;
        }

        IEnumerator ResetChainAttack()
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("chain attack reset");
            _chainAttackNumber = 0;
        }

        public IEnumerator ResetDodge()
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("dodge reset");
            _dodgeNumber = 0;
        }

    }

}
