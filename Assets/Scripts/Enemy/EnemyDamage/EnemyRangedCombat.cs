using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler.Enemies
{
    public class EnemyRangedCombat : EnemyCombat
    {
        [SerializeField] private GameObject _enemyProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private Transform _handTransform;
        private bool _chargingAttack = false;
        private GameObject _playerCharacter;

        // Teleport Variables
        private float _playersCurrentDistance;
        private bool _currentlyTeleporting = false; 
        [SerializeField] private float _needToTeleportRadius;
        [SerializeField] private float _teleTimer = 1f;
        private Vector3 _teleRadius;
        [SerializeField] private float _teleRange = 5f;


        private void Start() 
        {
            base.InitialiseVariables();

            foreach (Material mat in _meshRenderer.materials)
            {
                _originalColours.Add(mat.GetColor("_MainColor"));
            }

            _playerCharacter = GameObject.FindGameObjectWithTag("Player");

        }

        private void Update()
        {
            if (_chargingAttack == true)
            {
                FacePlayer();
            }

            // suppress create bullet while teleporting

            _playersCurrentDistance = Vector3.Distance(transform.position, _playerCharacter.transform.position);

            if(_playersCurrentDistance < _needToTeleportRadius)
            {
                Invoke("TeleportAway", _teleTimer);
            }
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            _healthBar.SetProgress(CurrentHealth / _maxHealth);
            /*hurtstate*/ DamageFeedback();
        }

        public override void Die()
        {
            base.Die();
            Destroy(_healthBar.gameObject);
        }

        public override void StartDealDamage()
        {
            _canDealDamage = true;
            CreateBullet();
        }

        private void DamageFeedback() 
        {
            _meshRenderer.material.color = Color.black;
            _enemyAnim.SetBool("isDamaged", true);
            Invoke("ResetColour", .15f);
        }

        private void EndHurtAnimation() 
        {
            _enemyAnim.SetBool("isDamaged", false);
        }


        private void TeleportAway()
        {
            _currentlyTeleporting = true;

            Vector3 point;

            if (IsPositionOnNavMesh(transform.position, _teleRange, out point))
            {
                transform.position = point;
            }
            else 
            {
                TeleportWithRetry(transform.position);
            }
        }

        private void TeleportWithRetry(Vector3 currentPosition)
        {
            int maxRetries = 5; // Set a maximum number of retries
            int retryCount = 0;
            Vector3 newTeleportPosition = currentPosition;

            while (retryCount < maxRetries)
            {
                Vector3 randomDestination = Random.insideUnitSphere * _teleRange;

                if (IsPositionOnNavMesh(randomDestination, _teleRange, out newTeleportPosition))
                {
                    transform.position = newTeleportPosition;
                    break;
                }
                retryCount++;
            }
        }


        private bool IsPositionOnNavMesh(Vector3 position, float range, out Vector3 result)
        {
            //_teleRadius = position + new Vector3(Random.Range(_minTeleArea, _maxTeleArea), y: 0f, Random.Range(_minTeleArea, _maxTeleArea)) * range;
            _teleRadius = position + Random.insideUnitSphere * range;

            NavMeshHit hit;
            
            if (NavMesh.SamplePosition(_teleRadius, out hit, 0.1f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        private void CreateBullet()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // COULD USE REFACTORING  
            GameObject projectile = Instantiate(_enemyProjectilePrefab, _handTransform.position, Quaternion.identity);

            Vector3 direction = (playerTransform.position - _handTransform.position).normalized;
            Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), direction);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = direction * _projectileSpeed;

        }

        private void AttackCharged()
        {
            //Debug.Log("charged");
            _chargingAttack = false;
        }

        private void ChargingAttack()
        {
            _chargingAttack = true;
        }

        private void ResetColour()
        {
            foreach (Material mat in _meshRenderer.materials)
            {
                mat.SetColor("_MainColor", _originalColours[_originalColorIndex]);
                _originalColorIndex++;
            }
            _originalColorIndex = 0;
        }

        private void FacePlayer()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            transform.LookAt(playerTransform);
        }


    }
}
