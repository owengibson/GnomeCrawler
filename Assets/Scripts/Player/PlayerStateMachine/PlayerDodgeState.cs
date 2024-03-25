using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerDodgeState : PlayerBaseState
    {
        public PlayerDodgeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        IEnumerator DodgeTimer()
        {
            yield return new WaitForSeconds(Ctx.DodgeDuration);
            Ctx.DodgeVelocity = 1f;
            Ctx.IsDodging = false;
            if (Ctx.DodgeNumber >= Ctx.PlayerStats.GetStat(Deckbuilding.Stat.NumberOfRolls))
            {
                Ctx.DodgeNumber = 0;
                Ctx.StartCoroutine(DodgeCooldownTimer(Ctx.DodgeCooldown));
            }
            else
            {
                Ctx.StartCoroutine(DodgeCooldownTimer(Ctx.MiniDodgeCooldown));
                Ctx.ResetDodgeCoroutine = Ctx.StartCoroutine(Ctx.ResetDodge());
            }
        }

        IEnumerator DodgeCooldownTimer(float cooldownTime)
        {
            yield return new WaitForSeconds(cooldownTime);
            Ctx.CanDodge = true;
        }

        public override void EnterState()
        {
            HandleDodge();
            Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
            Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() 
        {
            Ctx.Animator.SetBool(Ctx.IsDodgingHash, false);
            Ctx.ChainAttackNumber = 0;
        }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsFlinching && !Ctx.IsInvincible)
            {
                SwitchState(Factory.Flinch());
            }

            if (Ctx.IsDodging) return;
            else if (Ctx.IsDodgePressed && Ctx.CanDodge && Ctx.DodgeNumber < Ctx.PlayerStats.GetStat(Deckbuilding.Stat.NumberOfRolls))
            {
                SwitchState(Factory.Dodge());
            }
            else if (Ctx.IsAttackPressed && _currentSuperState == Factory.Grounded())
            {
                SwitchState(Factory.Attack());
            }
            else if (!Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Idle());
            }
            else if (Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Walk());
            }
            else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
            {
                SwitchState(Factory.Run());
            }
        }

        private void HandleDodge()
        {
            if (Ctx.ResetDodgeCoroutine != null) Ctx.StopCoroutine(Ctx.ResetDodgeCoroutine);
            Ctx.Animator.SetBool(Ctx.IsDodgingHash, true);
            Ctx.DodgeVelocity = Ctx.DodgeForce;
            Ctx.IsDodging = true;
            Ctx.CanDodge = false;
            Ctx.DodgeNumber++;
            Ctx.StartCoroutine(DodgeTimer());
        }
    }
}
