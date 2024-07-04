using GnomeCrawler.Deckbuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class ExplosiveProjectile : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(3f, gameObject);
                }
            }
        }
    }
}
