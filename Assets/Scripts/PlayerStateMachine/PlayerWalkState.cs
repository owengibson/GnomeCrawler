using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
            Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        }

        public override void UpdateState()
        {
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * (Ctx.RunMultiplier/ 2);
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * (Ctx.RunMultiplier / 2);
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (!Ctx.IsMovementPressed)
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
