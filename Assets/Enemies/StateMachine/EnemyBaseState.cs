
public abstract class EnemyBaseState
{
    protected EnemyStateManager ctx;
    protected EnemyStateFactory factory;

    public EnemyBaseState(EnemyStateManager currentContext, EnemyStateFactory stateFactory)
    {
        ctx = currentContext;
        factory = stateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

    protected void SwitchStates(EnemyBaseState newState)
    {
        ExitState();

        newState.EnterState();


    }

}
