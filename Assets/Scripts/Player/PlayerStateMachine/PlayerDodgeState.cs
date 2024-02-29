using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerDodgeState : PlayerBaseState
    {
        public PlayerDodgeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * 10f;
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * 10f;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsAttackPressed && _currentSuperState == Factory.Grounded())
            {
                SwitchState(Factory.Attack());
            }
            else if (!Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Idle());
            }
            else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
            {
                SwitchState(Factory.Run());
            }
        }
    }
}
