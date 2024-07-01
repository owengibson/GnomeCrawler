using GnomeCrawler.Player;
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

        [SerializeField] private float _distanceForMeleeRange;

        private bool _canEnterPhase2 = false;
        private bool _canEnterPhase3 = false ;

        private StateMachine _stateMachine;
        private BossCombatBrain _combatBrain;
        private Dictionary<int, int> fleeChance = new Dictionary<int, int>
        {
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 50 },
            { 4, 65 },
            { 5, 80 },
            { 6, 100 },
        };

        private void Awake()
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var animator = GetComponent<Animator>();
            _combatBrain = GetComponent<BossCombatBrain>();

            _stateMachine = new StateMachine();

            //states
            var chase = new Chase(this, navMeshAgent, animator, _moveSpeed);
            var attack = new MeleeAttack(this, navMeshAgent, animator, 1);
            var attack2 = new MeleeAttack(this, navMeshAgent, animator, 2);
            var attack3 = new MeleeAttack(this, navMeshAgent, animator, 3);
            var rangedAttack = new RangedAttack(this, navMeshAgent, animator, 1);
            var idle = new Idle(this, navMeshAgent, animator);
            var flee = new Flee(this, navMeshAgent, animator);
            var adds = new Adds(this, animator);

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
            At(flee, rangedAttack, FleeIsFinished());
            At(rangedAttack, chase, AttackComplete("RangedAttack"));
            At(adds, chase, AddsPhaseOver());

            //maybe 2 adds states?


            //any transitions
            _stateMachine.AddAnyTransition(flee, () => WillFleeFromMelee()());
            _stateMachine.AddAnyTransition(adds, () => CanBossEnterPhase2()());
            _stateMachine.AddAnyTransition(adds, () => CanBossEnterPhase3()());

            _stateMachine.SetState(chase);

            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Aet(IState from, ref Action eventTrigger, IState to) => _stateMachine.AddTransition(from, ref eventTrigger, to);
            Func<bool> IsPlayerInMeleeRange() => () => Vector3.Distance(transform.position, Target.transform.position) < _distanceForMeleeRange;
            Func<bool> IsPlayerOutOfMeleeRange() => () => !IsPlayerInMeleeRange()();
            Func<bool> CheckSuccessByPercentage(int percentage) => () => Random.Range(1, 101) <= percentage;
            Func<bool> IsInMeleeRangeAndChoseAttackNo(int attackNumber) => () => IsPlayerInMeleeRange()() && Random.Range(1, 4) == attackNumber;
            Func<bool> AttackComplete(string stateName) => () => AnimatorIsFinished(animator, stateName);
            Func<bool> CooldownAfterAttackFinished() => () => idle.TimeInIdle > 3;
            Func<bool> ChoseFleeAfterCooldown() => () => CooldownAfterAttackFinished()() && CheckSuccessByPercentage(35)();
            Func<bool> ChoseChaseAfterCooldown() => () => CooldownAfterAttackFinished()() && !ChoseFleeAfterCooldown()();
            Func<bool> ChoseRangedAfterCooldown() => () => CooldownAfterAttackFinished()() && IsPlayerOutOfMeleeRange()();
            Func<bool> FleeIsFinished() => () => flee.FleeTimer > 3;
            Func<bool> WillFleeFromMelee() => () => InMeleePhase == true && CheckSuccessByPercentage(fleeChance[_bossHitNumberInMeleePhase])();
            Func<bool> AddsPhaseOver() => () => adds.AddsTestTimer > 3;

            Func<bool> CanBossEnterPhase2() => () => _canEnterPhase2 == true && !InMeleePhase;
            Func<bool> CanBossEnterPhase3() => () => _canEnterPhase3 == true && !InMeleePhase;
        }

        private void Update() => _stateMachine.Tick();

        private void OnEnable()
        {
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
