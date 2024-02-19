using UnityEngine;
public class AttackState : BaseState
{
    public AttackState(StateManager stateManager, StateFactory stateFactory)
        : base(stateManager, stateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Now attacking");
    }

    public override void UpdateState()
    {

        CheckSwitchState();
    }

    public override void CheckSwitchState()
    {
        
    }

    public override void ExitState()
    {

    }
}
