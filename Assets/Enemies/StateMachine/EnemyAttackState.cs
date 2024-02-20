using UnityEditor.Rendering;
using UnityEngine;
public class EnemyAttackState : EnemyBaseState
{

    public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
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
