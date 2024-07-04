using GnomeCrawler.Systems;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GnomeCrawler
{
    public class Boss : MonoBehaviour
    {
        public GameObject Target;
        public float _moveSpeed;
        public bool InMeleePhase;
        public int _bossHitNumberInMeleePhase;
        public Transform[] _addsSpawnPoints;
        [HideInInspector] public BossCombatBrain _combatBrain;
        [HideInInspector] public float _currentEnemiesNo;

        [SerializeField] private float _distanceForMeleeRange;

        [HideInInspector] public bool _canEnterPhase2 = false;
        [HideInInspector] public bool _canEnterPhase3 = false ;

        private StateMachine _stateMachine;
        private Dictionary<int, int> fleeChance = new Dictionary<int, int>
        {
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 10 },
            { 4, 30 },
            { 5, 65 },
            { 6, 80 },
            { 7, 100 }
        };

        private void Awake()
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var animator = GetComponent<Animator>();
            var collider = GetComponent<Collider>();
            _combatBrain = GetComponent<BossCombatBrain>();

            _stateMachine = new StateMachine();

            //states
            var chase = new Chase(this, navMeshAgent, animator, _moveSpeed);
            var attack = new MeleeAttack(this, navMeshAgent, animator, 1);
            var attack2 = new MeleeAttack(this, navMeshAgent, animator, 2);
            var attack3 = new MeleeAttack(this, navMeshAgent, animator, 3);
            var rangedAttack = new RangedAttack(this, navMeshAgent, animator, 1);
            var idle = new Idle(this, navMeshAgent, animator);
            var flee = new Flee(this, navMeshAgent, animator, collider);
            var adds = new Adds(this, animator, collider);

            //transitions
            At(chase, attack, IsInMeleeRangeAndChoseAttackNo(1));
            //At(chase, attack2 , IsInMeleeRangeAndChoseAttackNo(2));
            //At(chase, attack3 , IsInMeleeRangeAndChoseAttackNo(3));
            At(attack, attack2, AttackComplete("Attack01"));
            At(attack2, attack3, AttackComplete("Attack02"));
            At(attack3, idle, AttackComplete("Attack03"));
            At(idle, chase, ChoseChaseAfterCooldown());
            At(idle, flee, ChoseFleeAfterCooldown());
            At(idle, rangedAttack, ChoseRangedAfterCooldown());
            At(flee, rangedAttack, AttackComplete("Flee"));
            At(rangedAttack, chase, AttackComplete("RangedAttack"));
            At(adds, chase, AddsPhaseOver());

            //maybe 2 adds states?


            //any transitions
            _stateMachine.AddAnyTransition(flee, () => WillFleeFromMelee()());
            _stateMachine.AddAnyTransition(adds, () => CanBossEnterPhase2()());
            _stateMachine.AddAnyTransition(adds, () => CanBossEnterPhase3()());

            _stateMachine.SetState(chase);

            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            //void Aet(IState from, ref Action eventTrigger, IState to) => _stateMachine.AddTransition(from, ref eventTrigger, to);
            Func<bool> IsPlayerInMeleeRange() => () => Vector3.Distance(transform.position, Target.transform.position) < _distanceForMeleeRange;
            Func<bool> IsPlayerOutOfMeleeRange() => () => !IsPlayerInMeleeRange()();
            Func<bool> CheckSuccessByPercentage(int percentage) => () => Random.Range(1, 101) <= percentage;
            Func<bool> IsInMeleeRangeAndChoseAttackNo(int attackNumber) => () => IsPlayerInMeleeRange()() && Random.Range(1, 4) == attackNumber;
            Func<bool> AttackComplete(string stateName) => () => AnimatorIsFinished(animator, stateName);
            Func<bool> CooldownAfterAttackFinished() => () => idle.TimeInIdle > 1.5f;
            Func<bool> ChoseFleeAfterCooldown() => () => CooldownAfterAttackFinished()() && CheckSuccessByPercentage(35)();
            Func<bool> ChoseChaseAfterCooldown() => () => CooldownAfterAttackFinished()() && !ChoseFleeAfterCooldown()();
            Func<bool> ChoseRangedAfterCooldown() => () => CooldownAfterAttackFinished()() && IsPlayerOutOfMeleeRange()();
            Func<bool> WillFleeFromMelee() => () => InMeleePhase == true && CheckSuccessByPercentage(fleeChance[_bossHitNumberInMeleePhase])();
            Func<bool> AddsPhaseOver() => () => _currentEnemiesNo == 0;

            Func<bool> CanBossEnterPhase2() => () => _canEnterPhase2 == true && !InMeleePhase;
            Func<bool> CanBossEnterPhase3() => () => _canEnterPhase3 == true && !InMeleePhase;


            _currentEnemiesNo = -1;
        }

        private void Update() => _stateMachine.Tick();

        private void OnEnable()
        {
            EventManager.OnEnemyKilled += EnemyKilled;

            _combatBrain.DamageTaken += BossHasTakenHit;
            _combatBrain.ReachedPhase2Threshold += BossEnteredPhase2;
            _combatBrain.ReachedPhase3Threshold += BossEnteredPhase3;
        }

        private void OnDisable()
        {
            _combatBrain.DamageTaken -= BossHasTakenHit;
            _combatBrain.ReachedPhase2Threshold -= BossEnteredPhase2;
            _combatBrain.ReachedPhase3Threshold -= BossEnteredPhase3;
        }

        private void EnemyKilled(GameObject obj)
        {
            _currentEnemiesNo--;
        }

        private void BossHasTakenHit()
        {
            if (InMeleePhase)
            {
                _bossHitNumberInMeleePhase += 1;
            }
        }

        private void BossEnteredPhase2() => _canEnterPhase2 = true;
        private void BossEnteredPhase3() => _canEnterPhase3 = true;

        private bool AnimatorIsFinished(Animator _animator){
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        }

        private bool AnimatorIsFinished(Animator _animator, string stateName) { 
            return AnimatorIsFinished(_animator) && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}
