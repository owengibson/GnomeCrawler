using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

namespace GnomeCrawler.Enemy
{
    public class EnemyAttackState : EnemyBaseState
    {
        public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            ctx.EnemyNavMeshAgent.speed = 0;
            ctx.IsAttackFinished = false;
            ctx.EnemyAnimator.SetBool("inCombat", true);
        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState() 
        {

            float currentDist = Vector3.Distance(ctx.transform.position, ctx.PlayerCharacter.transform.position);

            if (currentDist > ctx.AttackingDistance)
            {
                ctx.IsInAttackZone = false;
            }

            if (ctx.IsAttackFinished && !ctx.IsInAttackZone)
            {
                SwitchStates(factory.ChaseState());
            }
        }

        public override void ExitState() 
        {
            ctx.EnemyAnimator.SetBool("inCombat", false);
        }
    }
}


