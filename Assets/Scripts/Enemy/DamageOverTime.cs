using GnomeCrawler.Player;
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

            PlayerCombat playerCombat = other.gameObject.GetComponent<PlayerCombat>();
            _poisionTickTime -= Time.deltaTime;

            if (_poisionTickTime <= 0)
            {
                playerCombat.TakeDamageNoStun(1, ParentGO);
                _poisionTickTime = _poisonResetTimer;
            }
        }
    }
}
