using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class MushroomEnemy : MonoBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private GameObject _player;

        void Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            _navMeshAgent.destination = _player.transform.position;
        }
    }
}
