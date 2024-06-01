using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Attack : IState
    {
        protected readonly Boss _boss;
        protected readonly NavMeshAgent _navMeshAgent;
        protected readonly Animator _animator;
        protected readonly int _attackNumber;

        public Attack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, int attackNumber)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _attackNumber = attackNumber;
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
