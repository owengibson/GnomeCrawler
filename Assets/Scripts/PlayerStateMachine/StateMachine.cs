using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class StateMachine
    {
        public State currentState;

        public void Initialise(State startingState)
        {
            currentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(State newState)
        {
            currentState.Exit();

            currentState = newState;
            newState.Enter();
        }
    }
}