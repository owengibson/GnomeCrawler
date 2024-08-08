using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyAttackState : EnemyBaseState
    {
        public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            ctx.EnemyNavMeshAgent.speed = 0;
            ctx.IsAttackFinished = false;
            ctx.EnemyAnimator.SetBool("inCombat", true);
        }

        public override void UpdateState()
        {
            CheckSwitchState();

            Debug.Log("animator bool is set to " + ctx.EnemyAnimator.GetBool("inCombat"));

            //if (!ctx.EnemyAnimator.GetBool("inCombat") && !IsFacingPlayer())
            //{
            //    RotateToFacePlayer();
            //}
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState()
        {
            if (ctx.CurrentDistance > ctx.AttackingDistance)
            {
                ctx.IsInAttackZone = false;
            }

            if (ctx.IsAttackFinished && !ctx.IsInAttackZone)
            {
                SwitchStates(factory.ChaseState());
            }
        }

        public override void ExitState()
        {
            ctx.EnemyAnimator.SetBool("inCombat", false);
        }

        //private void RotateToFacePlayer()
        //{
        //    return;
        //    //ctx.transform.LookAt(ctx.PlayerCharacter.transform);
        //}


        public bool IsFacingPlayer()
        {
            Vector3 directionToPlayer = (ctx.PlayerCharacter.transform.position - ctx.transform.position).normalized;

            float angle = Vector3.Angle(ctx.transform.forward, directionToPlayer);

            float thresholdAngle = 0f;

            if (angle <= thresholdAngle)
            {
                ctx.EnemyAnimator.SetBool("inCombat", true);
                Debug.Log("Facing Player is true");

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}