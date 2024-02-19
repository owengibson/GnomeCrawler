
public class StateFactory
{ 
    StateManager context;

    public StateFactory(StateManager currentContext)
    {
        context = currentContext;
    }

    public BaseState IdleState()
    {
        return new IdleState(context, this);
    }

    public BaseState AttackState()
    {
        return new AttackState(context, this);
    }

    public BaseState ChaseState()
    {
        return new ChaseState(context, this);
    }
}
