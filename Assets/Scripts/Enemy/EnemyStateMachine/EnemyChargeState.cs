using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyChargeState : EnemyBaseState
    {
        public EnemyChargeState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            ctx.EnemyNavMeshAgent.speed = 0;
            ctx.IsAttackFinished = false;
            ctx.EnemyAnimator.SetBool("isMoving", true);
            ctx.EnemyAnimator.SetBool("inCombat", false);

            StartCharge();
        }

        public override void UpdateState()
        {
            CheckSwitchState();

            Vector3 playerPos = ctx.PlayerCharacter.transform.position;
            playerPos.y = ctx.transform.position.y;
            ctx.EnemyNavMeshAgent.destination = playerPos;
        }

        public override void FixedUpdateState()
        {

        }

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

        private void StartCharge()
        {
            ctx.StartCoroutine(Charge());
        }

        private IEnumerator Charge()
        {
            ctx.EnemyNavMeshAgent.angularSpeed = 10000f;
            ctx.EnemyNavMeshAgent.speed = 1f;
            ctx.EnemyAnimator.SetFloat("Speed", 1f);

            float counter = 0f;
            while (counter < 3f)
            {
                ctx.EnemyNavMeshAgent.speed *= 1.01f;

                float animSpeed = ctx.EnemyAnimator.GetFloat("Speed");
                animSpeed = Mathf.Clamp(ctx.EnemyNavMeshAgent.speed, 0f, 5f);
                ctx.EnemyAnimator.SetFloat("Speed", animSpeed);

                counter += Time.deltaTime;
            }
            yield return null;
        }
    }
}