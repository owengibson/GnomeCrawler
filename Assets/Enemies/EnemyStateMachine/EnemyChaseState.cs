using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            //Debug.Log("Chasing");
            ctx.EnemyAnimator.SetBool("isMoving", true);
            ctx.EnemyAnimator.SetBool("inCombat", false);

        }

        public override void UpdateState()
        {
            //Debug.Log("Now Chasing");
            ctx.CurrentEnemy.transform.LookAt(ctx.PlayerCharacter.transform.position);
        }
        public override void FixedUpdateState()
        {
            ApproachPlayer();
        }

        public override void OnTriggerEnterState(Collider collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                SwitchStates(factory.AttackState());
            }
        }
        public override void OnTriggerExitState(Collider collision) { }

        public override void CheckSwitchState() { }

        public override void ExitState() { }

        public void ApproachPlayer()
        {
            float speed = 2f;
            float step = speed * Time.deltaTime;
            ctx.CurrentEnemy.transform.position = Vector3.MoveTowards(ctx.CurrentEnemy.transform.position, ctx.PlayerCharacter.transform.position, step);
        }
    }
}