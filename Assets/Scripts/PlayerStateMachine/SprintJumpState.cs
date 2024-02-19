using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace GnomeCrawler
{
    public class SprintJumpState : State
    {
        float timePassed;
        float jumpTime;

        public SprintJumpState(ThirdPersonController _character, StateMachine _stateMachine) : base(_character, _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            character.animator.applyRootMotion = true;
            timePassed = 0f;
            character.animator.SetTrigger("sprintJump");

            jumpTime = 1f;
        }

        public override void Exit()
        {
            base.Exit();
            character.animator.applyRootMotion = false;
        }

        public override void LogicUpdate()
        {

            base.LogicUpdate();
            if (timePassed > jumpTime)
            {
                character.animator.SetTrigger("move");
                stateMachine.ChangeState(character.sprinting);
            }
            timePassed += Time.deltaTime;
        }
    }
}
