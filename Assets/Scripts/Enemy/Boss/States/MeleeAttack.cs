using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class MeleeAttack : Attack
    {
        private static readonly int MeleeAttackHash = Animator.StringToHash("MeleeAttack");
        private static readonly int MeleeAttackNumberHash = Animator.StringToHash("MeleeAttackNumber");
        public MeleeAttack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, int attackNumber) : base(boss, navMeshAgent, animator, attackNumber)
        {

        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log(_attackNumber);
            _animator.SetInteger(MeleeAttackNumberHash, _attackNumber);
            _animator.SetTrigger(MeleeAttackHash);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
