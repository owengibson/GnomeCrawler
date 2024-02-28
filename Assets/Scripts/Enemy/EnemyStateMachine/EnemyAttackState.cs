using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

namespace GnomeCrawler.Enemy
{/// <summary>
/// the enemy should attack the player
/// every x seconds, they will attack the player / the speed of anim is ok for now
/// at a certain point in the anim the enemy will instantiate an object, which will damage the player
/// it should happen each time
/// </summary>
    public class EnemyAttackState : EnemyBaseState
    {
        private bool timerIsRunning;
        private float elapsedTime = 0f;
        private float attackChance;

        public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            attackChance = Random.Range(ctx.MinAttackChance, ctx.MaxAttackChance);
            timerIsRunning = true;
        }

        public override void UpdateState()
        {
            Debug.Log(timerIsRunning);

            if (!timerIsRunning)
            {
                attackChance = Random.Range(ctx.MinAttackChance, ctx.MaxAttackChance);
                timerIsRunning = true;
            }

            else if (timerIsRunning)
            {
                Debug.Log(timerIsRunning);
                elapsedTime += Time.deltaTime; 

                if (elapsedTime >= attackChance)
                {
                    AttackPlayer();
                    Debug.Log("Timer finished!");
                }
            }
        }

        public override void FixedUpdateState()
        {
            // attack player
            // ctx.EnemyAnimator.Play("Base Layer.Combat");
        }

        public override void OnTriggerEnterState(Collider collision)
        {
            if(collision.gameObject.layer == 6)
            {
                Debug.Log("Damage Dealt");
            }
            // logic for doing damage
            // gonna need the stats/ health system first 
            // set the animation of the punch to play on attack - not when you're in the state
        }
        public override void OnTriggerExitState(Collider collision) { }

        public override void CheckSwitchState() { }

        public override void ExitState() { }

        private void AttackPlayer()
        {
            timerIsRunning = false;
            elapsedTime = 0f;
            ctx.EnemyAnimator.Play("Base Layer.Combat");
            SwitchStates(factory.ChaseState());
        }
    }
}


