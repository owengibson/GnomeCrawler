using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class MushroomEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject _poisonCloudPrefab;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private GameObject _player;
        private Coroutine _poisonCloudCooldown;

        private bool _hasAggro = false;


        void Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            float currentDistance = Vector3.Distance(transform.position, _player.transform.position);

            if (currentDistance < 12) _hasAggro = true;
            else return;

            if (currentDistance > 2)
            {
                FollowPlayer();
            }
            else if (currentDistance <= 2)
            {
                Fart();
            }
        }

        private void FollowPlayer()
        {
            _navMeshAgent.destination = _player.transform.position;
        }

        private void Fart()
        {
            _navMeshAgent.destination = transform.position;
            if (_poisonCloudCooldown == null)
            {
                _poisonCloudCooldown = StartCoroutine(FartCooldown());
            }
        }

        private IEnumerator FartCooldown()
        {
            GameObject poisonCloud = Instantiate(_poisonCloudPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(4f);
            Destroy(poisonCloud);
            yield return new WaitForSeconds(1f);
            _poisonCloudCooldown = null;
        }
    }
}
