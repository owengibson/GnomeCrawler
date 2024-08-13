using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GnomeCrawler.Enemies
{

    public class EnemyStateManager : MonoBehaviour
    {

        private EnemyBaseState currentState;
        private EnemyStateFactory states;
        private GameObject _playerCharacter;
        private NavMeshAgent _enemyNavMeshAgent;
        private bool _isAttackFinised;
        private float _currentDistance;

        [SerializeField] private GameObject _currentEnemy;
        [SerializeField] private float _chasingDistance;
        [SerializeField] private float _attackingDistance;
        [SerializeField] private Animator _enemyAnimator;
        [SerializeField] private float _chaseSpeed;
        [SerializeField] private bool _isInAttackZone;
        [SerializeField] private float _chargeAttackRange = 60f;
        [SerializeField] private float _chargeAttackDeadzone = 15f;

        public UnityEvent _startAttack;


        public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
        public GameObject PlayerCharacter { get => _playerCharacter; set => _playerCharacter = value; }
        public GameObject CurrentEnemy { get => _currentEnemy; set => _currentEnemy = value; }
        public float ChasingDistance { get => _chasingDistance; set => _chasingDistance = value; }
        public float AttackingDistance { get => _attackingDistance; set => _attackingDistance = value; }
        public Animator EnemyAnimator { get => _enemyAnimator; set => _enemyAnimator = value; }
        public NavMeshAgent EnemyNavMeshAgent { get => _enemyNavMeshAgent; set => _enemyNavMeshAgent = value; }
        public float ChaseSpeed { get => _chaseSpeed; set => _chaseSpeed = value; }
        public bool IsAttackFinished { get => _isAttackFinised; set => _isAttackFinised = value; }
        public bool IsInAttackZone { get => _isInAttackZone; set => _isInAttackZone = value; }
        public float CurrentDistance { get => _currentDistance; set => _currentDistance = value; }
        public float ChargeAttackRange { get => _chargeAttackRange; set => _chargeAttackRange = value; }
        public float ChargeAttackDeadzone { get => _chargeAttackDeadzone; set => _chargeAttackDeadzone = value; }

        void Start()
        {
            _playerCharacter = GameObject.FindWithTag("Player");

            if (_playerCharacter != null)
            {
                _enemyNavMeshAgent = GetComponent<NavMeshAgent>();
                states = new EnemyStateFactory(this);
                currentState = states.IdleState();
                currentState.EnterState();
            }
            else
            {
                Debug.LogWarning("Player character not found. Check if the player exists in the scene.");
            }

        }

        void Update()
        {
            if (_playerCharacter == null)
                return;
            _currentDistance = Vector3.Distance(transform.position, PlayerCharacter.transform.position);
            currentState.UpdateState();
        }

        private void FixedUpdate()
        {
            if (_playerCharacter == null)
                return;
            currentState.FixedUpdateState();
        }

        public void EndOfAnimation(string aninName)
        {
            if (aninName == "Attack")
            {
                _isAttackFinised = true;
                _enemyAnimator.SetBool("inCombat", false);
            }
        }

    }
}