using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerFlinchState : PlayerBaseState
    {
        public PlayerFlinchState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            Debug.Log("start flinch");
            Ctx.IsFlinchFinished = false;
            Ctx.Animator.SetTrigger(Ctx.FlinchHash);
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() 
        {
            Ctx.IsFlinching = false;
        }

        public override void InitialiseSubState() { }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsFlinchFinished)
            {
                SwitchState(Factory.Idle());
            }
        }
    }
}
