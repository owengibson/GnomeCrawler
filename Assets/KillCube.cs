using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class KillCube : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable component))
            {
                component.TakeDamage(1, gameObject);
            }
        }
    }
}
