using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class RangedAttack : Attack
    {
        public Coroutine _lookCoroutine;

        private static readonly int RangedAttackHash = Animator.StringToHash("RangedAttack");
        public RangedAttack(Boss boss, NavMeshAgent navMeshAgent, Animator animator, int attackNumber) : base(boss, navMeshAgent, animator, attackNumber)
        {
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _animator.SetTrigger(RangedAttackHash);
            StartRotating();
            _boss.OnRangedAttack?.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void StartRotating()
        {
            if (_lookCoroutine != null)
            {
                _boss.StopCoroutine(_lookCoroutine);
            }
            _lookCoroutine = _boss.StartCoroutine(LookAt(1f));
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
