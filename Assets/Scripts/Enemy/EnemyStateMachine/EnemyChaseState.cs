using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            ctx.EnemyAnimator.SetBool("isMoving", true);
            ctx.EnemyAnimator.SetFloat("Speed", 1f);
            ctx.EnemyAnimator.SetBool("inCombat", false);
            ctx.EnemyNavMeshAgent.speed = ctx.ChaseSpeed;
        }

        public override void UpdateState()
        {
            CheckSwitchState();
            Vector3 playerPos = ctx.PlayerCharacter.transform.position;
            playerPos.y = ctx.transform.position.y;
            ctx.EnemyNavMeshAgent.destination = playerPos;
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState() 
        {
            if (ctx.CurrentDistance < ctx.AttackingDistance)
            {
                SwitchStates(factory.AttackState());
                ctx.IsInAttackZone = true; 
            }
        }

        public override void ExitState() { }
    }
}