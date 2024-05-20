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
        }

        public override void ExitState() { }
    }
}