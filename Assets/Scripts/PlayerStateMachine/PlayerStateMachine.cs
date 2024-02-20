using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class PlayerStateMachine : MonoBehaviour
    {
        PlayerBaseState currentState;
        PlayerStateFactory factory;

        private void Start()
        {
            factory = new PlayerStateFactory(this);
            //currentState = factory.IdleState();
            currentState.EnterState();
        }

        private void Update()
        {
            currentState.UpdateState();
            Debug.Log(currentState.ToString());
        }

        private void FixedUpdate()
        {
            currentState.PhysicsUpdateState();
        }
    }
}
