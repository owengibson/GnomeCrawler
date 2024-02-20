using UnityEditor.Rendering;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyAttackState : EnemyBaseState
    {

        public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            Debug.Log("Entered Attack State");
            ctx.EnemyAnimator.SetBool("isMoving", false);
            ctx.EnemyAnimator.SetBool("inCombat", true);
        }

        public override void UpdateState()
        {
            Debug.Log("Now attacking");
            CheckSwitchState();
        }

        public override void FixedUpdateState()
        {

        }

        public override void OnTriggerEnterState(Collider collision)
        {
            // logic for doing damage
            // gonna need the stats/ health system first 
        }
        public override void OnTriggerExitState(Collider collision)
        {
            SwitchStates(factory.ChaseState());
        }

        public override void CheckSwitchState() { }

        public override void ExitState() { }

    }
}