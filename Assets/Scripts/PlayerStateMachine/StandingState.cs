using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;

namespace GnomeCrawler
{
    public class StandingState : State
    {
        float gravityValue;
        bool jump;
        Vector3 currentVelocity;
        bool grounded;
        bool sprint;
        float playerSpeed;

        Vector3 cVelocity;

        public StandingState(ThirdPersonController _character, StateMachine _stateMachine) : base(_character, _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            jump = false;
            sprint = false;
            input = Vector2.zero;

            currentVelocity = Vector3.zero;
            gravityVelocity.y = 0;

            velocity = character.playerVelocity;
            playerSpeed = character.playerSpeed;
            grounded = character.controller.isGrounded;
            gravityValue = character.gravityValue;
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (jumpAction.triggered)
            {
                jump = true;
            }
            if (sprintAction.triggered)
            {
                sprint = true;
            }

            input = moveAction.ReadValue<Vector2>();
            velocity = new Vector3(input.x, 0, input.y);
            velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
            velocity.y = 0f;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

            if (sprint)
            {
                stateMachine.ChangeState(character.sprinting);
            }
            if (jump)
            {
                stateMachine.ChangeState(character.jumping);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            gravityVelocity.y += gravityValue * Time.deltaTime;
            grounded = character.controller.isGrounded;

            if (grounded && gravityVelocity.y < 0)
            {
                gravityVelocity.y = 0f;
            }

            currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);
            character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);
            if (velocity.sqrMagnitude > 0)
            {
                character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
            }
        }
        public override void Exit()
        {
            base.Exit();
            gravityVelocity.y = 0f;
            character.playerVelocity = new Vector3(input.x, 0, input.y);

            if (velocity.sqrMagnitude > 0)
            {
                character.transform.rotation = Quaternion.LookRotation(velocity);
            }
        }
    }
}
