using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class PlayerAttackState : PlayerBaseState
    {
        public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            Ctx.Animator.SetBool(Ctx.IsAttackingHash, true);
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() 
        {
            Ctx.Animator.SetBool(Ctx.IsAttackingHash, false);
        }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (!Ctx.IsAttackFinished) return;
            Ctx.IsAttackFinished = false;

            if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
            {
                SwitchState(Factory.Run());
            }
            else if (Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Walk());
            }
            else 
            { 
                SwitchState(Factory.Idle()); 
            }
        }


    }
}
