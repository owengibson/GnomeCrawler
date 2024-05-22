using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Attack : IState
    {
        private readonly Boss _boss;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;

        private readonly int AttackHash;

        public Attack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, string attackTransitionName)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            AttackHash = Animator.StringToHash(attackTransitionName);
        }

        public virtual void Tick()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }
    }
}
