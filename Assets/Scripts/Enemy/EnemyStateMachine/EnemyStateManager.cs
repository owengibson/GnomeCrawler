using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler.Enemy
{

    public class EnemyStateManager : MonoBehaviour
    {

        EnemyBaseState currentState;
        EnemyStateFactory states;

        public int EnemyHeatlh;

        [SerializeField] private GameObject _playerCharacter;
        [SerializeField] private GameObject _currentEnemy;
        [SerializeField] private float _chasingZone;
        [SerializeField] private Collider _attackingZone;
        [SerializeField] private Animator _enemyAnimator;
        [SerializeField] private float _chaseSpeed;
        private NavMeshAgent _enemyNavMeshAgent;
        [SerializeField] private float minAttackChance;
        [SerializeField] private float maxAttackChance;
        [SerializeField] private bool needsBlockState;
        private bool _isAttackFinised;
        [SerializeField] private bool _isInAttackZone;
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _healthBarCanvas;   


        public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
        public GameObject PlayerCharacter { get => _playerCharacter; set => _playerCharacter = value; }
        public GameObject CurrentEnemy { get => _currentEnemy; set => _currentEnemy = value; }
        public float ChasingZone { get => _chasingZone; set => _chasingZone = value; }
        public Collider AttackingZone { get => _attackingZone; set => _attackingZone = value; }
        public Animator EnemyAnimator { get => _enemyAnimator; set => _enemyAnimator = value; }
        public NavMeshAgent EnemyNavMeshAgent { get => _enemyNavMeshAgent; set => _enemyNavMeshAgent = value; }
        public float ChaseSpeed { get => _chaseSpeed; set => _chaseSpeed = value; }
        public float MinAttackChance { get => minAttackChance; set => minAttackChance = value; }
        public float MaxAttackChance { get => maxAttackChance; set => maxAttackChance = value; }
        public bool NeedsBlockState { get => needsBlockState; set => needsBlockState = value; }
        public bool IsAttackFinished { get => _isAttackFinised; set => _isAttackFinised = value; }
        public bool IsInAttackZone { get => _isInAttackZone; set => _isInAttackZone = value; }

        void Start()
        {
            _enemyNavMeshAgent = GetComponent<NavMeshAgent>();
            gameObject.GetComponent<CombatBrain>().SetUpHealthBar(_healthBarCanvas, _camera);
            states = new EnemyStateFactory(this);
            currentState = states.IdleState();
            currentState.EnterState();
        }

        void Update()
        {
            currentState.UpdateState();
            //Debug.Log(currentState);

            //if(EnemyHeatlh <= 0)
            //{
            //    Destroy(this.gameObject);
            //}
        }

        private void FixedUpdate()
        {
            currentState.FixedUpdateState();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (AttackingZone && other.gameObject.tag == "Player")
            {
                IsInAttackZone = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (AttackingZone && other.gameObject.tag == "Player")
            {
                IsInAttackZone = false;
            }
        }

        //public void TakeDamage(int amount)
        //{
        //    EnemyHeatlh -= amount;
        //}

        public void EndOfAnimation(string aninName)
        {
            if (aninName == "Attack") _isAttackFinised = true;
        }
    }
}