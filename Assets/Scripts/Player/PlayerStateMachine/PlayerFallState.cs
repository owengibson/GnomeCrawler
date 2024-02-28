using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerFallState : PlayerBaseState, IRootState
    {
        public PlayerFallState(PlayerStateMachine currentContext,
        PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            InitialiseSubState();
            Ctx.Animator.SetBool(Ctx.IsFallingHash, true);
        }

        public override void UpdateState()
        {
            HandleGravity();
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            Ctx.Animator.SetBool(Ctx.IsFallingHash, false);
        }

        public void HandleGravity()
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + Ctx.Gravity * Time.deltaTime;
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.CharacterController.isGrounded)
            {
                SwitchState(Factory.Grounded());
            }
        }

        public override void InitialiseSubState()
        {
            if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
            {
                SetSubState(Factory.Idle());
            }
            else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
            {
                SetSubState(Factory.Walk());
            }
            else
            {
                SetSubState(Factory.Run());
            }
        }
    }
}
