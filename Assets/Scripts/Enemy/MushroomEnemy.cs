using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        private GameObject fartCloud;
        private Coroutine _fartCloudCooldown;

        private bool _hasAggro = false;


        void Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            if (_player == null) return;
            float currentDistance = Vector3.Distance(transform.position, _player.transform.position);

            if (currentDistance < 12 || _hasAggro) _hasAggro = true;
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
            _animator.SetBool("isChasing", true);
            _animator.SetBool("isFarting", false);
            _navMeshAgent.destination = _player.transform.position;
        }

        private void Fart()
        {
            _navMeshAgent.destination = transform.position;
            if (_fartCloudCooldown == null && fartCloud == null)
            {
                _animator.SetBool("isChasing", false);
                _animator.SetBool("isFarting", true);
                _fartCloudCooldown = StartCoroutine(CreateFart());
            }
        }

        private IEnumerator CreateFart()
        {
            yield return new WaitForSeconds(1f);
            fartCloud = Instantiate(_poisonCloudPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(4f);
            Destroy(fartCloud);
            yield return new WaitForSeconds(1f);
            _fartCloudCooldown = null;
        }

        private IEnumerator CoolDown()
        {
            _animator.SetBool("isChasing", true);
            _animator.SetBool("isFarting", false);
            yield return new WaitForSeconds(0.5f);
            _fartCloudCooldown = null;
        }

        public void StopFart()
        {
            if (_fartCloudCooldown != null)
            {
                StopCoroutine(_fartCloudCooldown);
                StartCoroutine(CoolDown());
            }
        }

        private void OnDestroy()
        {
            if (fartCloud != null) Destroy(fartCloud);
        }
    }
}
