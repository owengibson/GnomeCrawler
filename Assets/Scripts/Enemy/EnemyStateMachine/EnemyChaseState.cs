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
            //Debug.Log("Now Chasing");
            ctx.CurrentEnemy.transform.LookAt(ctx.PlayerCharacter.transform.position);
        }
        public override void FixedUpdateState()
        {
            ApproachPlayer();
        }

        public override void OnTriggerEnterState(Collider collision)
        {
            if (ctx.AttackingZone && collision.gameObject.tag == "Player")
            {
                if(ctx.NeedsBlockState)
                {
                    SwitchStates(factory.BlockState());
                }
                else
                {
                    SwitchStates(factory.AttackState());
                }
            }
        }
        public override void OnTriggerExitState(Collider collision) { }

        public override void CheckSwitchState() { }

        public override void ExitState() { }

        public void ApproachPlayer()
        {
            ctx.EnemyNavMeshAgent.speed = ctx.ChaseSpeed;
            ctx.EnemyNavMeshAgent.destination = ctx.PlayerCharacter.transform.position;
        }
    }
}