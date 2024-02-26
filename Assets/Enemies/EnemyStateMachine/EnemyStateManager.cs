using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{

    public class EnemyStateManager : MonoBehaviour
    {

        EnemyBaseState currentState;
        EnemyStateFactory states;
        EnemyWeaponHitBox hitBox;

        public int enemyHeatlh;

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

        public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
        public GameObject PlayerCharacter { get => _playerCharacter; set => _playerCharacter = value; }
        public GameObject CurrentEnemy { get => _currentEnemy; set => _currentEnemy = value; }
        public float ChasingZone { get => _chasingZone; set => _chasingZone = value; }
        public Collider AttackingZone { get => _attackingZone; set => _attackingZone = value; }
        public Animator EnemyAnimator { get => _enemyAnimator; set => _enemyAnimator = value; }
        public EnemyWeaponHitBox HitBox { get => hitBox; }
        public NavMeshAgent EnemyNavMeshAgent { get => _enemyNavMeshAgent; set => _enemyNavMeshAgent = value; }
        public float ChaseSpeed { get => _chaseSpeed; set => _chaseSpeed = value; }
        public float MinAttackChance { get => minAttackChance; set => minAttackChance = value; }
        public float MaxAttackChance { get => maxAttackChance; set => maxAttackChance = value; }
        public bool NeedsBlockState { get => needsBlockState; set => needsBlockState = value; }

        void Start()
        {
            _enemyNavMeshAgent = GetComponent<NavMeshAgent>();
            states = new EnemyStateFactory(this);
            currentState = states.IdleState();
            currentState.EnterState();
        }

        void Update()
        {
            currentState.UpdateState();
            Debug.Log(currentState);

            if(enemyHeatlh <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        private void FixedUpdate()
        {
            currentState.FixedUpdateState();
        }
        private void OnTriggerEnter(Collider other)
        {
            currentState.OnTriggerEnterState(other);
        }
        private void OnTriggerExit(Collider other)
        {
            currentState.OnTriggerExitState(other);
        }

        public void TakeDamage(int amount)
        {
            enemyHeatlh -= amount;
        }


    }
}