using GnomeCrawler.Systems;
using System;
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
        [SerializeField] private Renderer[] _renderers;
        private bool _chargingAttack = false;
        private GameObject _playerCharacter;


        // Teleport Variables
        private float _playersCurrentDistance;
        private bool _canTele = true; 
        [SerializeField] private float _needToTeleportRadius;
        [SerializeField] private float _teleTimer = 1f;
        private Vector3 _teleRadius;
        [SerializeField] private float _teleRange = 5f;


        private void Start() 
        {
            base.InitialiseVariables();

            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                _originalColours.Add(mat.GetColor("_MainColor"));
            }

            _playerCharacter = GameObject.FindGameObjectWithTag("Player");

        }

        private void Update()
        {
            bool? nullableIsPlayerTargetable = EventManager.IsPlayerTargetable?.Invoke();
            bool isPlayerTargetable = nullableIsPlayerTargetable == true || nullableIsPlayerTargetable == null;
            if (!isPlayerTargetable) return;

            if (_chargingAttack == true)
            {
                FacePlayer();
            }

            // suppress create bullet while teleporting

            _playersCurrentDistance = Vector3.Distance(transform.position, _playerCharacter.transform.position);

            if(_playersCurrentDistance < _needToTeleportRadius)
            {
                Invoke("TeleportAway", 1);
            }
        }

        public override void TakeDamage(float amount, GameObject damager)
        {
            base.TakeDamage(amount, damager);
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

        protected override void DamageFeedback()
        {
            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                mat.SetColor("_MainColor", Color.black);
            }
            Invoke("ResetColour", .15f);
            _enemyAnim.SetBool("isDamaged", true);
        }

        protected override void ResetColour()
        {
            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                mat.SetColor("_MainColor", _originalColours[_originalColorIndex]);
                _originalColorIndex++;
            }
            _originalColorIndex = 0;
        }


        private void EndHurtAnimation() 
        {
            _enemyAnim.SetBool("isDamaged", false);
        }


        private void TeleportAway()
        {
            Vector3 point;

            if (IsPositionOnNavMesh(transform.position, _teleRange, out point) && _canTele)
            {
                _canTele = false;
                transform.position = point;
                StartCoroutine(TeleportCoolDown());
            }
        }

        private IEnumerator TeleportCoolDown()
        {
            yield return new WaitForSeconds(_teleTimer);
            _canTele = true; 
        }

        private bool IsPositionOnNavMesh(Vector3 position, float range, out Vector3 result)
        {
            _teleRadius = position + UnityEngine.Random.insideUnitSphere * range;

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
            projectile.GetComponent<EnemyProjectile>().Parent = gameObject;

            //Vector3 direction = (playerTransform.position - _handTransform.position).normalized;
            Vector3 direction = transform.forward;
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

        private void FacePlayer()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            transform.LookAt(playerTransform);
        }

        public void EndOfAnimation(string aninName)
        {
            if (aninName == "Attack")
            {
                _enemyAnim.SetBool("inCombat", false);
            }
            Invoke("SetCombatToTrue", 1);
        }

        private void SetCombatToTrue()
        {
            _enemyAnim.SetBool("inCombat", true);
        }
    }
}
