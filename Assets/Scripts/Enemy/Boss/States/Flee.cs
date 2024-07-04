using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Flee : IState
    {
        private readonly Boss _boss;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        private readonly Collider _collider;

        private Coroutine _lookCoroutine;

        private static readonly int FleeHash = Animator.StringToHash("Flee");
        private float _cachedSpeed;
        private float _cachedRotation;
        private const float FLEE_SPEED = 20;
        private const float FLEE_ROTATION = 0;
        public Flee(Boss boss, NavMeshAgent navMeshAgent, Animator animator, Collider collider)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _collider = collider;
        }

        public void Tick()
        {

        }

        private Vector3 GetFleePosition()
        {
            Vector3 fleeDirection = _boss.transform.position - _boss.Target.transform.position;
            fleeDirection.y = 0;
            fleeDirection.Normalize();

            Vector3 endPoint = _boss.transform.position + (fleeDirection * FLEE_SPEED);
            Vector3 endPointBehindPlayer = _boss.transform.position - (fleeDirection * FLEE_SPEED * 1.5f);
            if (NavMesh.SamplePosition(endPoint, out var hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            else if (NavMesh.SamplePosition(endPointBehindPlayer, out hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return _boss.transform.position;
        }

        public void OnEnter()
        {
            _navMeshAgent.enabled = true;
            _collider.enabled = false;
            _boss._bossHitNumberInMeleePhase = 0;
            _animator.SetTrigger(FleeHash);
            _cachedSpeed = _navMeshAgent.speed;
            _cachedRotation = _navMeshAgent.angularSpeed;
            _navMeshAgent.speed = FLEE_SPEED;
            _navMeshAgent.angularSpeed = FLEE_ROTATION;
            _navMeshAgent.SetDestination(GetFleePosition());
            StartRotating();
        }

        public void OnExit()
        {
            _navMeshAgent.enabled = false;
            _collider.enabled = true;
            _navMeshAgent.speed = _cachedSpeed;
            _navMeshAgent.angularSpeed = _cachedRotation;
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
