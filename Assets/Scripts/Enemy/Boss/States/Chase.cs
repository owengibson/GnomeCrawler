using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Chase : IState
    {
        private readonly Boss _boss;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        private readonly float _moveSpeed;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        public Chase(Boss boss, NavMeshAgent navMeshAgent, Animator animator, float moveSpeed)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _moveSpeed = moveSpeed;
        }

        public void Tick()
        {
            _navMeshAgent.SetDestination(_boss.Target.transform.position);
            _animator.SetLookAtPosition(_boss.Target.transform.position);
        }

        public void OnEnter()
        { 
            _navMeshAgent.enabled = true;
            _animator.SetFloat(SpeedHash, _moveSpeed);
        }

        public void OnExit()
        {
            _navMeshAgent.enabled = false;
            _animator.SetFloat(SpeedHash, 0f);
        }
    }
}
