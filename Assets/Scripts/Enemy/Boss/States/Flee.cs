using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Flee : IState
    {
        private readonly Boss _boss;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;

        private static readonly int FleeHash = Animator.StringToHash("Flee");
        public Flee(Boss boss, NavMeshAgent navMeshAgent, Animator animator)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
        }

        public void Tick()
        {

        }

        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }
    }
}
