using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyIdleState : EnemyBaseState
    {
        public EnemyIdleState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {

        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState()
        {
            if (ctx.CurrentDistance < ctx.ChasingDistance)
            {
                SwitchStates(factory.ChaseState());
            }

            else if (ctx.CurrentDistance < ctx.ChargeAttackRange && ctx.CurrentDistance > ctx.ChargeAttackDeadzone && IsFacingPlayer())
            {
                SwitchStates(factory.ChargeState());
            }
        }

        public override void ExitState() { }

        public bool IsFacingPlayer()
        {
            Vector3 directionToPlayer = (ctx.PlayerCharacter.transform.position - ctx.transform.position).normalized;

            float angle = Vector3.Angle(ctx.transform.forward, directionToPlayer);

            float thresholdAngle = 30f;

            if (angle <= thresholdAngle)
            {
                ctx.EnemyAnimator.SetBool("inCombat", true);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}