using UnityEditor.Rendering;
using UnityEngine;

namespace GnomeCrawler
{/// <summary>
/// the enemy should attack the player
/// every x seconds, they will attack the player / the speed of anim is ok for now
/// at a certain point in the anim the enemy will instantiate an object, which will damage the player
/// it should happen each time
/// </summary>
    public class EnemyAttackState : EnemyBaseState
    {

        public EnemyAttackState(EnemyStateManager stateManager, EnemyStateFactory stateFactory)
            : base(stateManager, stateFactory) { }

        public override void EnterState()
        {
            //Debug.Log("Entered Attack State");
            ctx.EnemyAnimator.SetBool("inCombat", true);
        }

        public override void UpdateState()
        {
            //Debug.Log("Now attacking");
            CheckSwitchState();
        }

        public override void FixedUpdateState()
        {
            // attack player
            // ctx.EnemyAnimator.Play("Base Layer.Combat");
            
        }

        public override void OnTriggerEnterState(Collider collision)
        {
            if(collision.gameObject.name == "PlayerCharacter")
            {
                Debug.Log("Damage Dealt");
            }
            // logic for doing damage
            // gonna need the stats/ health system first 
            // set the animation of the punch to play on attack - not when you're in the state
        }
        public override void OnTriggerExitState(Collider collision)
        {
            SwitchStates(factory.ChaseState());
        }

        public override void CheckSwitchState() { }

        public override void ExitState() 
        {

        }

    }
}

