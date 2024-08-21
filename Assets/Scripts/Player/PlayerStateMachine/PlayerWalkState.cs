using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            Ctx.OnWalkStart?.Invoke();
        }

        public override void UpdateState()
        {
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
            Ctx.Animator.SetFloat(Ctx.SpeedHash, Ctx.CurrentMovementInput.magnitude, 0.1f, Time.deltaTime);
            CheckSwitchStates();
        }

        public override void ExitState() 
        {
            Ctx.OnWalkStop?.Invoke();
        }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsFlinching && !Ctx.IsInvincible)
            {
                SwitchState(Factory.Flinch());
            }
            else if (Ctx.IsAttackPressed && _currentSuperState == Factory.Grounded())
            {
                SwitchState(Factory.Attack());
            }
            else if (Ctx.IsDodgePressed && _currentSuperState == Factory.Grounded() && Ctx.CanDodge && Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Dodge());
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
