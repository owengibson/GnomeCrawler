using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Flee : IState
    {
        public float FleeTimer;

        private readonly Boss _boss;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;

        private static readonly int FleeHash = Animator.StringToHash("Flee");
        private float _cachedSpeed;
        private const float FLEE_SPEED = 15;
        public Flee(Boss boss, NavMeshAgent navMeshAgent, Animator animator)
        {
            _boss = boss;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
        }

        public void Tick()
        {
            FleeTimer += Time.deltaTime;
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
            FleeTimer = 0;
            _boss._bossHitNumberInMeleePhase = 0;
            _animator.SetTrigger(FleeHash);
            _cachedSpeed = _navMeshAgent.speed;
            _navMeshAgent.speed = FLEE_SPEED;
            _navMeshAgent.SetDestination(GetFleePosition());
        }

        public void OnExit()
        {
            _navMeshAgent.enabled = false;
            _navMeshAgent.speed = _cachedSpeed;
        }
    }
}
