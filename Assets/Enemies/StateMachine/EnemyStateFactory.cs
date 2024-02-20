
public class EnemyStateFactory
{ 
    EnemyStateManager context;

    public EnemyStateFactory(EnemyStateManager currentContext)
    {
        context = currentContext;
    }

    public EnemyBaseState IdleState()
    {
        return new EnemyIdleState(context, this);
    }

    public EnemyBaseState AttackState()
    {
        return new EnemyAttackState(context, this);
    }

    public EnemyBaseState ChaseState()
    {
        return new EnemyChaseState(context, this);
    }
}
