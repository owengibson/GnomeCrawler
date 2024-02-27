using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private float _distanceToPlayer;
        [SerializeField] private Animator _enemyAnimator;

        public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
        public GameObject PlayerCharacter { get => _playerCharacter; set => _playerCharacter = value; }
        public GameObject CurrentEnemy { get => _currentEnemy; set => _currentEnemy = value; }
        public float DistanceToPlayer { get => _distanceToPlayer; set => _distanceToPlayer = value; }
        public Animator EnemyAnimator { get => _enemyAnimator; set => _enemyAnimator = value; }
        public EnemyWeaponHitBox HitBox { get => hitBox; }

        void Start()
        {
            states = new EnemyStateFactory(this);
            currentState = states.IdleState();
            currentState.EnterState();
        }

        void Update()
        {
            currentState.UpdateState();
            //Debug.Log(currentState);

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