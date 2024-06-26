using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class MeleeAttack : Attack
    {
        private static readonly int MeleeAttackHash = Animator.StringToHash("MeleeAttack");
        private static readonly int MeleeAttackNumberHash = Animator.StringToHash("MeleeAttackNumber");

        private Coroutine _lookCoroutine;
        public MeleeAttack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, int attackNumber) : base(boss, navMeshAgent, animator, attackNumber)
        {

        }

        public override void Tick()
        {
            base.Tick();

            if (_attackNumber == 3)
            {
                _boss._combatBrain.ExpandOverlapSphere();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Debug.Log(_attackNumber);
            _boss.InMeleePhase = true;

            _boss._combatBrain.ChangeWeaponOrigin(_attackNumber - 1);

            _animator.SetInteger(MeleeAttackNumberHash, _attackNumber);
            _animator.SetTrigger(MeleeAttackHash);

            StartRotating();
        }

        public override void OnExit()
        {
            base.OnExit();
            _boss.InMeleePhase = false;
        }

        private void StartRotating()
        {
            if (_lookCoroutine != null)
            {
                _boss.StopCoroutine( _lookCoroutine );
            }
            _lookCoroutine = _boss.StartCoroutine(LookAt());
        }

        private IEnumerator LookAt()
        {
            yield return new WaitForSeconds(0.5f);

            Quaternion lookRotation = Quaternion.LookRotation(_boss.Target.transform.position - _boss.transform.position);

            float time = 0;

            while (time < 1)
            {
                _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, time);

                time += Time.deltaTime * 1f;

                yield return null;
            }
        }
    }
}
