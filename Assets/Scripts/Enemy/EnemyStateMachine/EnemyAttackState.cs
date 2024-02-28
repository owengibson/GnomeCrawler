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
        private bool _timerIsRunning;
        private float _elapsedTime = 0f;
        private float _attackChance;
        private bool _canAttack;

        public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            ctx.EnemyNavMeshAgent.speed = 0;
            ctx.IsAttackFinished = false;
            _attackChance = Random.Range(ctx.MinAttackChance, ctx.MaxAttackChance);
            ctx.EnemyAnimator.SetBool("inCombat", true);
            /*AttackPlayer();*/
        }

        public override void UpdateState()
        {
            CheckSwitchState();

           /* if (ctx.IsAttackFinished)
            {
                AttackPlayer();
            }*/

            /*if(_canAttack)
            {
                _timerIsRunning = true;
            }

            if (!_timerIsRunning)
            {
                _attackChance = Random.Range(ctx.MinAttackChance, ctx.MaxAttackChance);
                _timerIsRunning = true;
            }

            else if (_timerIsRunning)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime >= _attackChance)
                {
                    AttackPlayer();
                    //Debug.Log("Timer finished!");
                }
            }*/
        }

        public override void FixedUpdateState() { }

        public override void CheckSwitchState() 
        {
           /* if (!ctx.IsAttackFinished)
            {
                _timerIsRunning = false;
                return;
            }*/

            if (ctx.IsAttackFinished && !ctx.IsInAttackZone)
            {
                SwitchStates(factory.ChaseState());
            }
        }

        public override void ExitState() 
        {
            ctx.EnemyAnimator.SetBool("inCombat", false);
        }

       /* private void AttackPlayer()
        {
            ctx.IsAttackFinished = false;
            _elapsedTime = 0f;
            ctx.EnemyAnimator.Play("Base Layer.Combat");
        }*/
    }
}


