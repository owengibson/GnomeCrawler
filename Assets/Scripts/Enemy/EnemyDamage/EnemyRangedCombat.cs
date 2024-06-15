using System.Collections;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyRangedCombat : EnemyCombat
    {
        [SerializeField] private GameObject _enemyProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private Transform _handTransform;
        [SerializeField] private Renderer[] _renderers;

        private void Start()
        {
            base.InitialiseVariables();

            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                _originalColours.Add(mat.GetColor("_MainColor"));
            }
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

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            _healthBar.SetProgress(CurrentHealth / _maxHealth);
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

        private void EndHurtAnimation() // move to hurtstate
        {
            _enemyAnim.SetBool("isDamaged", false);
        }

        private void CreateBullet()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            GameObject projectile = Instantiate(_enemyProjectilePrefab, _handTransform.position, Quaternion.identity);

            Vector3 direction = (playerTransform.position - _handTransform.position).normalized;
            Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), direction);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = direction * _projectileSpeed;
        }
    }
}
