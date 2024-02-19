
public abstract class BaseState
{
    protected StateManager ctx;
    protected StateFactory factory;

    public BaseState(StateManager currentContext, StateFactory stateFactory)
    {
        ctx = currentContext;
        factory = stateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

    protected void SwitchStates(BaseState newState)
    {
        ExitState();

        newState.EnterState();


    }

}
