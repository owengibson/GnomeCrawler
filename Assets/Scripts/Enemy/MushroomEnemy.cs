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
        [SerializeField] private GameObject _poisonCloudLinePrefab;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private GameObject _player;
        private GameObject fartCloud;
        private Coroutine _fartCloudCooldown;
        private GameObject _fartLine;
        private Coroutine _fartLineCooldown;

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

            if (currentDistance < 15 || _hasAggro) _hasAggro = true;
            else return;

            if (currentDistance > 15 && _fartLineCooldown == null)
            {
                ThrowFartLine();
            }
            else if (currentDistance <= 3)
            {
                Fart();
            }
            else
            {
                FollowPlayer();
            }
        }

        private void FollowPlayer() 
        {
            _animator.SetTrigger("isChasing");
            _navMeshAgent.destination = _player.transform.position;
        }

        private void ThrowFartLine()
        {
            _navMeshAgent.destination = transform.position;
            _animator.SetTrigger("isShootingLine");
            _fartLineCooldown = StartCoroutine(CreateLineFart());
        }

        private void Fart()
        {
            _navMeshAgent.destination = transform.position;
            if (_fartCloudCooldown == null && fartCloud == null)
            {
                _animator.SetTrigger("isFarting");
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

        private IEnumerator CreateLineFart()
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 playerPosition = _player.transform.position;
            _fartLine = Instantiate(_poisonCloudLinePrefab, transform.position, Quaternion.identity);
            while (_fartLine.transform.position != playerPosition)
            {
                _fartLine.transform.position = Vector3.MoveTowards(_fartLine.transform.position, playerPosition, Time.deltaTime * 20);
                yield return null;
            }
            Destroy(_fartLine);
            _fartLineCooldown = null;
        }

        private IEnumerator CoolDown()
        {
            _animator.SetTrigger("isChasing");
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
