using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace GnomeCrawler
{
    public class LandingState : State
    {
        float timePassed;
        float landingTime;

        public LandingState(ThirdPersonController _character, StateMachine _stateMachine) : base(_character, _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            timePassed = 0f;
            character.animator.SetTrigger("land");
            landingTime = 0.5f;
        }

        public override void LogicUpdate()
        {

            base.LogicUpdate();
            if (timePassed > landingTime)
            {
                character.animator.SetTrigger("move");
                stateMachine.ChangeState(character.standing);
            }
            timePassed += Time.deltaTime;
        }
    }
}
