using GnomeCrawler.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Boss : MonoBehaviour
    {
        public PlayerStateMachine Target;
        public float _moveSpeed;

        [SerializeField] private float _distanceForAttackOne;

        private StateMachine _stateMachine;

        private void Awake()
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var animator = GetComponent<Animator>();

            _stateMachine = new StateMachine();

            //states
            var chase = new Chase(this, navMeshAgent, animator, _moveSpeed);
            var attack = new MeleeAttack(this, navMeshAgent, animator, "Attack01");
            var attack2 = new MeleeAttack(this, navMeshAgent, animator, "Attack02");
            var attack3 = new MeleeAttack(this, navMeshAgent, animator, "Attack03");
            var rangedAttack = new RangedAttack(this, navMeshAgent, animator, "RangedAttack01");
            var charge = new Charge(this, navMeshAgent, animator, _moveSpeed);
            var flee = new Flee(this, navMeshAgent, animator, _moveSpeed);

            //transitions
            At(chase, attack, ReachedPlayer());

            //any transitions

            _stateMachine.SetState(chase);

            void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
            Func<bool> HasTarget() => () => Target != null;
            Func<bool> ReachedPlayer() => () => Target != null && Vector3.Distance(transform.position, Target.transform.position) > _distanceForAttackOne;
        }
    }
}
