using DG.Tweening;
using GnomeCrawler.Player;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GnomeCrawler
{
    public class MushroomEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject _poisonCloudPrefab;
        [SerializeField] private GameObject _poisonCloudLinePrefab;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _projectileLinger = 3f;
        [SerializeField] private float _projectileCooldown = 2f;

        [SerializeField] private UnityEvent OnChargeFart;
        [SerializeField] private UnityEvent OnFart;
        [SerializeField] private UnityEvent OnRangedAttack;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private GameObject _player;
        private GameObject fartCloud;
        private Coroutine _fartCloudCO;
        private GameObject _fartLine;
        private Coroutine _fartLineCO;

        private bool _hasAggro = false;


        void Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = PlayerStateMachine.instance.gameObject;
        }

        void Update()
        {
            if (_player == null) return;

            float currentDistance = Vector3.Distance(transform.position, _player.transform.position);

            bool? nullableIsPlayerTargetable = EventManager.IsPlayerTargetable?.Invoke();
            bool isPlayerTargetable = nullableIsPlayerTargetable == true || nullableIsPlayerTargetable == null;
            if (!isPlayerTargetable) return;

            if (currentDistance < 12 || _hasAggro) _hasAggro = true;
            else return;

            if (currentDistance > 8 && _fartLineCO == null)
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
            _fartLineCO = StartCoroutine(CreateLineFart());
        }

        private void Fart()
        {
            _navMeshAgent.destination = transform.position;
            if (_fartCloudCO == null && fartCloud == null)
            {
                _animator.SetTrigger("isFarting");
                _fartCloudCO = StartCoroutine(CreateFart());
            }
        }

        private IEnumerator CreateFart()
        {
            OnChargeFart?.Invoke();
            yield return new WaitForSeconds(1f);
            OnFart?.Invoke();
            fartCloud = Instantiate(_poisonCloudPrefab, transform.position, Quaternion.identity);
            fartCloud.GetComponent<DamageOverTime>().ParentGO = gameObject;
            yield return new WaitForSeconds(4f);
            fartCloud.GetComponent<DamageOverTime>().DestroyParticles();
            yield return new WaitForSeconds(1f);
            _fartCloudCO = null;
        }

        private IEnumerator CreateLineFart()
        {
            OnRangedAttack?.Invoke();
            yield return new WaitForSeconds(0.5f);
            Vector3 playerPosition = _player.transform.position;
            Vector3 enemyPosition = transform.position;
            _fartLine = Instantiate(_poisonCloudLinePrefab, transform.position, Quaternion.LookRotation(playerPosition - enemyPosition));
            _fartLine.GetComponent<DamageOverTime>().ParentGO = gameObject;
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
            _fartLine.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(_fartLine));
            yield return new WaitForSeconds(_projectileCooldown);
            _fartLineCO = null;
        }

        private IEnumerator CoolDown()
        {
            _animator.SetTrigger("isChasing");
            yield return new WaitForSeconds(0.5f);
            _fartCloudCO = null;
        }

        public void StopFart()
        {
            if (_fartCloudCO != null)
            {
                StopCoroutine(_fartCloudCO);
                StartCoroutine(CoolDown());
            }
        }

        private void OnDestroy()
        {
            if (fartCloud != null) fartCloud.GetComponent<DamageOverTime>().DestroyParticles();
            if (_fartLine != null) Destroy(_fartLine);
        }
    }
}
