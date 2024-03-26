using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Rendering;
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

            if (ctx.EnemyAnimator.GetBool("inCombat") && IsFacingPlayer())
            {
                ctx.EnemyAnimator.SetBool("inCombat", true);
            }


            else if (!ctx.EnemyAnimator.GetBool("inCombat") && !IsFacingPlayer())
            {
                RotateToFacePlayer();
            }
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState()
        {

            float currentDist = Vector3.Distance(ctx.transform.position, ctx.PlayerCharacter.transform.position);

            if (currentDist > ctx.AttackingDistance)
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

        private void RotateToFacePlayer()
        {
            Vector3 direction = (ctx.PlayerCharacter.transform.position - ctx.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 1f;
            ctx.transform.rotation = Quaternion.Lerp(ctx.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public bool IsFacingPlayer()
        {
            Vector3 directionToPlayer = (ctx.PlayerCharacter.transform.position - ctx.transform.position).normalized;

            float angle = Vector3.Angle(ctx.transform.forward, directionToPlayer);

            float thresholdAngle = 30f;

            if (angle <= thresholdAngle)
            {
                ctx.EnemyAnimator.SetBool("inCombat", true);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}