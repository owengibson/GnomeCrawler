using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(StateManager stateManager, StateFactory stateFactory)
        : base(stateManager, stateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Idle");
    }

    public override void UpdateState()
    {

        CheckSwitchState();
    }

    public override void CheckSwitchState()
    {
        float currentDist = Vector3.Distance(ctx.CurrentEnemy.transform.position, ctx.PlayerCharacter.transform.position); // check how close the player is 

        if(currentDist < ctx.DistanceToPlayer)
        {
            SwitchStates(factory.ChaseState());
        }
    }

    public override void ExitState()
    {

    }
}
