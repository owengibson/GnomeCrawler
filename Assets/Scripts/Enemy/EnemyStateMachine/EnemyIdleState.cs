using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Enemy
{
    public class EnemyIdleState : EnemyBaseState
    {
        public EnemyIdleState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            //Debug.Log("Idle");
        }

        public override void UpdateState()
        {
            //Debug.Log("Idling");
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void OnTriggerEnterState(Collider collision) { }

        public override void OnTriggerExitState(Collider collision) { }

        public override void CheckSwitchState()
        {
            float currentDist = Vector3.Distance(ctx.CurrentEnemy.transform.position, ctx.PlayerCharacter.transform.position); 

            if (currentDist < ctx.ChasingZone)
            {
                SwitchStates(factory.ChaseState());
            }
        }

        public override void ExitState() { }
    }
}