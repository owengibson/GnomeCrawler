using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerAttackState : PlayerBaseState
    {
        public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            Ctx.Animator.SetBool(Ctx.IsAttackingHash, true);
            Ctx.IsAttackFinished = false;
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
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

            else if (Ctx.IsDodgePressed && _currentSuperState == Factory.Grounded() && Ctx.CanDodge && Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Dodge());
            }
            else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
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
