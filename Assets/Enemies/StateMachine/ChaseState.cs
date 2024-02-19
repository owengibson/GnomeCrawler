using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(StateManager stateManager, StateFactory stateFactory)
        : base(stateManager, stateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Chasing");
    }

    public override void UpdateState()
    {

        CheckSwitchState();
    }

    public override void CheckSwitchState()
    {
        float currentDist = Vector3.Distance(ctx.CurrentEnemy.transform.position, ctx.PlayerCharacter.transform.position); // check how close the player is 

        if (currentDist < ctx.DistanceToPlayer)
        {
            SwitchStates(factory.AttackState());
        }
    }

    public override void ExitState()
    {

    }
}
