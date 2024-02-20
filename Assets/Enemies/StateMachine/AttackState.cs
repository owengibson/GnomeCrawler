using UnityEditor.Rendering;
using UnityEngine;
public class AttackState : BaseState
{

    public AttackState(StateManager stateManager, StateFactory stateFactory)
        : base(stateManager, stateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Entered Attack State");
    }

    public override void UpdateState()
    {
        Debug.Log("Now attacking");

        ctx.EnemyMovement.ApproachPlayer();
        CheckSwitchState();
    }

    public override void CheckSwitchState()
    {

    }

    public override void ExitState()
    {

    }

}
