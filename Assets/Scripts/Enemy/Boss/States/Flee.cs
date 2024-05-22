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
        private readonly float _moveSpeed;

        private static readonly int Speed = Animator.StringToHash("Speed");
        public Flee(Boss boss, NavMeshAgent navMeshAgent, Animator animator, float moveSpeed)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _moveSpeed = moveSpeed;
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
