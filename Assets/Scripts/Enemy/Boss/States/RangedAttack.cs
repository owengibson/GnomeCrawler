using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class RangedAttack : Attack
    {
        private static readonly int RangedAttackHash = Animator.StringToHash("RangedAttack");
        public RangedAttack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, int attackNumber) : base(boss, navMeshAgent, animator, attackNumber)
        {
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
