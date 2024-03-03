using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Enemy
{
    public class EnemyBlockState : EnemyBaseState
    {
        public EnemyBlockState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            //Debug.Log("Hello from the Block state");
        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState() { }

        public override void ExitState() { }
    }
}
