using System.Collections;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyRangedCombat : EnemyCombat
    {
        [SerializeField] private GameObject _enemyProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private Transform _handTransform;

        private void Start()
        {
            base.InitialiseVariables();

            foreach (Material mat in _meshRenderer.materials)
            {
                _originalColours.Add(mat.GetColor("_MainColor"));
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

        private void DamageFeedback() // move to hurtstate
        {
            _meshRenderer.material.color = Color.black;
            _enemyAnim.SetBool("isDamaged", true);
            Invoke("ResetColour", .15f);
        }

        private void EndHurtAnimation() // move to hurtstate
        {
            _enemyAnim.SetBool("isDamaged", false);
        }

        private void CreateBullet()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            GameObject projectile = Instantiate(_enemyProjectilePrefab, _handTransform.position, Quaternion.identity);

            Vector3 direction = (playerTransform.position - _handTransform.position).normalized;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = direction * _projectileSpeed;
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
    }
}
