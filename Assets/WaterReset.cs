using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class WaterReset : MonoBehaviour
    {
        [SerializeField] private Transform _resetTransform;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable component))
            {
                component.TakeDamage(1);
                other.enabled = false;
                other.transform.position = _resetTransform.position;
                other.enabled = true;
            }
        }
    }
}
