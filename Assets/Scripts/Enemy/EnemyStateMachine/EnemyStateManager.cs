using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler.Enemies
{

    public class EnemyStateManager : MonoBehaviour
    {

        EnemyBaseState currentState;
        EnemyStateFactory states;

        private GameObject _playerCharacter;
        [SerializeField] private GameObject _currentEnemy;
        [SerializeField] private float _chasingDistance;
        [SerializeField] private float _attackingDistance;
        [SerializeField] private Animator _enemyAnimator;
        [SerializeField] private float _chaseSpeed;
        private NavMeshAgent _enemyNavMeshAgent;
        [SerializeField] private bool needsBlockState;
        private bool _isAttackFinised;
        [SerializeField] private bool _isInAttackZone;
        private Camera _camera;
        private Canvas _healthBarCanvas;


        public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
        public GameObject PlayerCharacter { get => _playerCharacter; set => _playerCharacter = value; }
        public GameObject CurrentEnemy { get => _currentEnemy; set => _currentEnemy = value; }
        public float ChasingDistance { get => _chasingDistance; set => _chasingDistance = value; }
        public float AttackingDistance { get => _attackingDistance; set => _attackingDistance = value; }
        public Animator EnemyAnimator { get => _enemyAnimator; set => _enemyAnimator = value; }
        public NavMeshAgent EnemyNavMeshAgent { get => _enemyNavMeshAgent; set => _enemyNavMeshAgent = value; }
        public float ChaseSpeed { get => _chaseSpeed; set => _chaseSpeed = value; }
        public bool NeedsBlockState { get => needsBlockState; set => needsBlockState = value; }
        public bool IsAttackFinished { get => _isAttackFinised; set => _isAttackFinised = value; }
        public bool IsInAttackZone { get => _isInAttackZone; set => _isInAttackZone = value; }

        void Start()
        {
            _playerCharacter = GameObject.FindWithTag("Player");

            if (_playerCharacter != null)
            {
                _enemyNavMeshAgent = GetComponent<NavMeshAgent>();
                _camera = Camera.main;
                _healthBarCanvas = GameObject.Find("Enemy Canvas").GetComponent<Canvas>();
                gameObject.GetComponent<EnemyCombat>().SetUpHealthBar(_healthBarCanvas, _camera);
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
            currentState.UpdateState();
        }

        private void FixedUpdate()
        {
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