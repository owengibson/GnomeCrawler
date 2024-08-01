using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyProjectile : MonoBehaviour
    {
        public StatsSO _stats;
        public float _lifespan = 4f;

        private IDamageable _damageable;

        public GameObject Parent;

        private void Awake()
        {
            Invoke("DestroyProjectile", _lifespan);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            if (other.gameObject.TryGetComponent(out _damageable))
            {
                _damageable.TakeDamage(_stats.GetStat(Stat.Damage), Parent);
            }
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
