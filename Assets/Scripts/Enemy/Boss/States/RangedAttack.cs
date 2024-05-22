using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class RangedAttack : Attack
    {
        public RangedAttack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, string attackAnimName) : base(boss, navMeshAgent, animator, attackAnimName)
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
