using GnomeCrawler.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class RangedCombatTutorial : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject _enemyProjectilePrefab;
        [SerializeField] private Transform _handTransform;
        [SerializeField] private float _projectileSpeed;

        private float _currentTime;
        private float _maxTime;

        private void Start()
        {
            animator.SetBool("isMoving", true);
            InvokeRepeating("CreateBullet", 1, 2);
        }

        public void CreateBullet()
        {
            GameObject projectile = Instantiate(_enemyProjectilePrefab, _handTransform.position, Quaternion.identity);
            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
            enemyProjectile.CancelInvoke();
            enemyProjectile.Invoke("DestroyProjectile", 7);
            enemyProjectile.Parent = gameObject;

            Vector3 direction = transform.forward;
            Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), direction);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = direction * _projectileSpeed;
        }
    }
}
