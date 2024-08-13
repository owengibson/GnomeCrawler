using DG.Tweening;
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

        [SerializeField] private ParticleSystem _particleSystem;

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

        private void Update()
        {
            if (_particleSystem == null) return;
            if (!_particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }

        public void DestroyParticles()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
