using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateMachine stateMachine;
        protected PlayerStateFactory factory;

        public PlayerBaseState(PlayerStateMachine currentStateMachine, PlayerStateFactory stateFactory)
        {
            stateMachine = currentStateMachine;
            factory = stateFactory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void PhysicsUpdateState();
        public abstract void ExitState();
        public abstract void OnCollisionEnter();
        public abstract void OnCollisionExit();
        public abstract void CheckSwitchState();

        protected void SwitchStates(PlayerBaseState newState)
        {
            ExitState();

            newState.EnterState();
        }

    }
}
