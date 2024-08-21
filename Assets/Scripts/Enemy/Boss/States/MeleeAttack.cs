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
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Debug.Log(_attackNumber);
            _boss.InMeleePhase = true;

            StartRotating();

            _boss._combatBrain.ChangeWeaponOrigin(_attackNumber - 1);

            _animator.SetInteger(MeleeAttackNumberHash, _attackNumber);
            _animator.SetTrigger(MeleeAttackHash);

            switch (_attackNumber)
            {
                case 0:
                    break;
                case 1:
                    _boss.OnAttackOne?.Invoke();
                    break;
                case 2:
                    _boss.OnAttackTwo?.Invoke();
                    break;
                case 3:
                    _boss.OnAttackThree?.Invoke();
                    break;
            }
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
            _lookCoroutine = _boss.StartCoroutine(LookAt(0.5f));
        }

        private IEnumerator LookAt(float duration)
        {
            yield return new WaitForSeconds(0.1f);

            Quaternion startRotation = _boss.transform.rotation;
            Quaternion lookRotation = Quaternion.LookRotation(_boss.Target.transform.position - _boss.transform.position);

            float time = 0;

            while (time < duration)
            {
                _boss.transform.rotation = Quaternion.Slerp(startRotation, lookRotation, time / duration);

                time += Time.deltaTime;

                yield return null;
            }

            _boss.transform.rotation = lookRotation;
        }
    }
}
