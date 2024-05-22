using GnomeCrawler.Player;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GnomeCrawler
{
    public class Boss : MonoBehaviour
    {
        public GameObject Target;
        public float _moveSpeed;

        [SerializeField] private float _distanceForMeleeRange;

        private StateMachine _stateMachine;

        private void Awake()
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var animator = GetComponent<Animator>();

            _stateMachine = new StateMachine();

            //states
            var chase = new Chase(this, navMeshAgent, animator, _moveSpeed);
            var attack = new MeleeAttack(this, navMeshAgent, animator, 1);
            var attack2 = new MeleeAttack(this, navMeshAgent, animator, 2);
            var attack3 = new MeleeAttack(this, navMeshAgent, animator, 3);
            var rangedAttack = new RangedAttack(this, navMeshAgent, animator, 1);
            var idle = new Idle(this, navMeshAgent, animator);
            var flee = new Flee(this, navMeshAgent, animator);

            //transitions
            At(chase, attack, IsInMeleeRangeAndChoseAttack1());
            At(chase, attack2 , IsInMeleeRangeAndChoseAttack2());
            At(chase, attack3 , IsInMeleeRangeAndChoseAttack3());
            At(attack, attack2, Attack1Complete());
            At(attack2, attack3, Attack2Complete());
            At(attack3, idle, Attack3Complete());

            //any transitions
            //_stateMachine.AddAnyTransition(flee, () => );

            _stateMachine.SetState(chase);

            void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
            Func<bool> IsInMeleeRangeAndChoseAttack1() => () => Vector3.Distance(transform.position, Target.transform.position) < _distanceForMeleeRange && Random.Range(1, 4) == 1;
            Func<bool> IsInMeleeRangeAndChoseAttack2() => () => Vector3.Distance(transform.position, Target.transform.position) < _distanceForMeleeRange && Random.Range(1, 4) == 2;
            Func<bool> IsInMeleeRangeAndChoseAttack3() => () => Vector3.Distance(transform.position, Target.transform.position) < _distanceForMeleeRange && Random.Range(1, 4) == 3;
            Func<bool> Attack1Complete() => () => AnimatorIsPlaying(animator, "Attack01") == false;
            Func<bool> Attack2Complete() => () => AnimatorIsPlaying(animator, "Attack02") == false;
            Func<bool> Attack3Complete() => () => AnimatorIsPlaying(animator, "Attack03") == false;
        }

        private void Update() => _stateMachine.Tick();

        private bool AnimatorIsPlaying(Animator _animator){
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
        }

        private bool AnimatorIsPlaying(Animator _animator, string stateName) { 
            return AnimatorIsPlaying(_animator) && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}
