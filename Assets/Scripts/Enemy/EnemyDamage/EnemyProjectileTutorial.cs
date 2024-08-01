using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyProjectileTutorial : MonoBehaviour
    {
        [SerializeField] private StatsSO _stats;
        public float _lifespan = 7f;

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

            CharacterController charController = other.gameObject.GetComponent<CharacterController>();
            charController.enabled = false;
            other.transform.position = new Vector3(-22, 12, -80);
            charController.enabled = true;
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
