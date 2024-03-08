using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Enemy
{
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            // tool tip
            // Debug.Log("Chasing");
            ctx.EnemyAnimator.SetBool("isMoving", true);
            ctx.EnemyAnimator.SetBool("inCombat", false);
        }

        public override void UpdateState()
        {
            CheckSwitchState();
            //Debug.Log("Now Chasing");
            //ctx.CurrentEnemy.transform.LookAt(ctx.PlayerCharacter.transform.position);
        }
        public override void FixedUpdateState()
        {
            ApproachPlayer();
        }

        public override void CheckSwitchState() 
        {
            if (ctx.NeedsBlockState)
            {
                SwitchStates(factory.BlockState());
            }
            else if (ctx.IsInAttackZone)
            {
                SwitchStates(factory.AttackState());
            }
        }

        public override void ExitState() { }

        public void ApproachPlayer()
        {
            ctx.EnemyNavMeshAgent.speed = ctx.ChaseSpeed;
            ctx.EnemyNavMeshAgent.destination = ctx.PlayerCharacter.transform.position;
        }
    }
}