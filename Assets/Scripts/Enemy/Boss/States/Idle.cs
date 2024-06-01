using DunGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Idle : IState
    {
        public float TimeInIdle;

        private readonly Boss _boss;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;

        private static readonly int IdleHash = Animator.StringToHash("Idle");

        public Idle(Boss boss, NavMeshAgent navMeshAgent, Animator animator)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
        }

        public void Tick()
        {
            TimeInIdle += Time.deltaTime;
        }

        public void OnEnter()
        {
            _navMeshAgent.enabled = false;
            _animator.SetTrigger(IdleHash);
            TimeInIdle = 0;
            _boss._bossHitNumberInMeleePhase = 0;
        }

        public void OnExit()
        {
            TimeInIdle = 0;
        }
    }
}
