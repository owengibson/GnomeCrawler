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
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _projectileLinger = 3f;
        [SerializeField] private float _projectileCooldown = 2f;

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

            if (currentDistance < 10 || _hasAggro) _hasAggro = true;
            else return;

            if (currentDistance > 12 && _fartLineCooldown == null)
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
            Vector3 enemyPosition = transform.position;
            _fartLine = Instantiate(_poisonCloudLinePrefab, transform.position, Quaternion.LookRotation(playerPosition - enemyPosition));
            CapsuleCollider capsuleCollider = _fartLine.GetComponent<CapsuleCollider>();
            ParticleSystem particleSystem = _fartLine.GetComponentInChildren<ParticleSystem>();
            var shapeModule = particleSystem.shape;
           

            float distanceCovered = 0f;
            float journeyLength = Vector3.Distance(enemyPosition, playerPosition);

            while (distanceCovered < journeyLength)
            {

                distanceCovered += _projectileSpeed * Time.deltaTime;
                float fracJourney = distanceCovered / journeyLength;
                _fartLine.transform.position = Vector3.Lerp(enemyPosition, playerPosition, fracJourney);
                capsuleCollider.height = Mathf.Lerp(1f, journeyLength, fracJourney);
                shapeModule.scale = new Vector3(0,0,capsuleCollider.height);
                capsuleCollider.center = new Vector3(0 ,0 ,-capsuleCollider.height / 2 + 0.5f);
                shapeModule.position = capsuleCollider.center;
                yield return null;
            }
            yield return new WaitForSeconds(_projectileLinger);
            Destroy(_fartLine);
            yield return new WaitForSeconds(_projectileCooldown);
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
            if (_fartLine != null) Destroy(_fartLine);
        }
    }
}
