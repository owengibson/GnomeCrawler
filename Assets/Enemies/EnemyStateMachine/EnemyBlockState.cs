using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyBlockState : EnemyBaseState
    {
        public EnemyBlockState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            Debug.Log("Hello from the Block state");
        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void OnTriggerEnterState(Collider collision) { }

        public override void OnTriggerExitState(Collider collision) { }

        public override void CheckSwitchState() { }

        public override void ExitState() { }
    }
}
