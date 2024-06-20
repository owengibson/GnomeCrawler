using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class DamageOverTime : MonoBehaviour
    {
        [HideInInspector] public GameObject ParentGO;

        private float _poisonResetTimer = 1;
        private float _poisionTickTime;
        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            IDamageable iDawmagabwe = other.gameObject.GetComponent<IDamageable>();
            _poisionTickTime -= Time.deltaTime;

            if (_poisionTickTime <= 0)
            {
                iDawmagabwe.TakeDamage(1, ParentGO);
                _poisionTickTime = _poisonResetTimer;
            }
        }
    }
}
