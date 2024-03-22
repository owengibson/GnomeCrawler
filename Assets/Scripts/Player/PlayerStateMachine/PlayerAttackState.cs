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
            yield return new WaitForSeconds(0.2f);
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
            Ctx.CanMoveWhileAttacking = false;
        }

        IEnumerator ChainAttackCooldown()
        {
            Ctx.ChainAttackNumber = 0;
            yield return new WaitForSeconds(0.3f);
        }

        public override void EnterState()
        {
            Ctx.Animator.SetBool(Ctx.IsAttackingHash, true);
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
            Ctx.IsAttackFinished = false;
            Ctx.CanMoveWhileAttacking = true;
            Ctx.StartCoroutine(AttackMovement());

            Ctx.ChainAttackNumber++;
            Ctx.Animator.SetInteger(Ctx.AttackNumberHash, Ctx.ChainAttackNumber);
            if (Ctx.ResetChainAttackCoroutine != null) Ctx.StopCoroutine(Ctx.ResetChainAttackCoroutine);

        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() 
        {
            Ctx.Animator.SetBool(Ctx.IsAttackingHash, false);

            if (Ctx.ChainAttackNumber >= 4)
            {
                Ctx.StartCoroutine(ChainAttackCooldown());
            }
        }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (!Ctx.IsAttackFinished) return;


            if (Ctx.IsAttackPressed && Ctx.ChainAttackNumber < 4)
            {
                SwitchState(Factory.Attack());
            }
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
