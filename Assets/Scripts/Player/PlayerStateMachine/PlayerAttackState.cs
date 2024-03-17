using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerAttackState : PlayerBaseState
    {
        public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        IEnumerator AttackMovement()
        {
            yield return new WaitForSeconds(0.3f);
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
        }

        public override void EnterState()
        {
            Ctx.Animator.SetBool(Ctx.IsAttackingHash, true);
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
            //Debug.Log(Ctx.AppliedMovementX + ", " + Ctx.AppliedMovementZ);
            Ctx.IsAttackFinished = false;
            Ctx.StartCoroutine(AttackMovement());
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
