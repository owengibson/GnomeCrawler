using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private StatsSO _stats;
        [SerializeField] private float _lifespan = 4f;

        private IDamageable _damageable;

        private void Awake()
        {
            Invoke("DestroyProjectile", _lifespan);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player"))
                return;

            if (collision.gameObject.TryGetComponent(out _damageable))
            {
                _damageable.TakeDamage(_stats.GetStat(Stat.Damage));
            }
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
