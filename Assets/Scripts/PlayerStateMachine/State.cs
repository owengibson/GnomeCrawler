using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

namespace GnomeCrawler
{
    public class State
    {
        public ThirdPersonController character;
        public StateMachine stateMachine;

        protected Vector3 gravityVelocity;
        protected Vector3 velocity;
        protected Vector2 input;

        public InputAction moveAction;
        public InputAction lookAction;
        public InputAction jumpAction;
        public InputAction attackAction;
        public InputAction blockAction;
        public InputAction dodgeAction;
        public InputAction lockOnAction;
        public InputAction sprintAction;

        public State(ThirdPersonController _character, StateMachine _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;

            moveAction = character.playerInput.actions["Move"];
            lookAction = character.playerInput.actions["Look"];
            jumpAction = character.playerInput.actions["Jump"];
            attackAction = character.playerInput.actions["Attack"];
            blockAction = character.playerInput.actions["Block"];
            dodgeAction = character.playerInput.actions["Dodge"];
            lockOnAction = character.playerInput.actions["LockOn"];
            sprintAction = character.playerInput.actions["Sprint"];

        }

        public virtual void Enter()
        {
            //StateUI.instance.SetStateText(this.ToString());
            Debug.Log("Enter State: " + this.ToString());
        }

        public virtual void HandleInput()
        {
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void Exit()
        {
        }
    }
}
