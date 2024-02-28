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
            ctx.IsAttackFinished = false;
            _attackChance = Random.Range(ctx.MinAttackChance, ctx.MaxAttackChance);
        }

        public override void UpdateState()
        {
            if(_canAttack)
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
                    Debug.Log("Timer finished!");
                }
            }
        }

        public override void FixedUpdateState() { }

        public override void OnTriggerEnterState(Collider collision) { }
        public override void OnTriggerExitState(Collider collision) 
        {
            _canAttack = false;
        }

        public override void CheckSwitchState() 
        {
            if (!ctx.IsAttackFinished)
            {
                _timerIsRunning = false;
                return;
            }

            else
            {
                SwitchStates(factory.ChaseState());
            }
        }

        public override void ExitState() { }

        private void AttackPlayer()
        {
            _elapsedTime = 0f;
            ctx.EnemyAnimator.Play("Base Layer.Combat");
            CheckSwitchState();
        }
    }
}


