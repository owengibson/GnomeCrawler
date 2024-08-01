using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Enemies;
using GnomeCrawler.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyProjectileTutorial : EnemyProjectile
    {
        [SerializeField] private StatsSO _stats;
      

      

        private void Awake()
        {
            _lifespan = 7f;
            Invoke("DestroyProjectile", _lifespan);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            if (other.GetComponent<PlayerCombat>()._isInvincible)
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
