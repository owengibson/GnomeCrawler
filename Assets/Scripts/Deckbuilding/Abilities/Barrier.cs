using DinoFracture;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace GnomeCrawler.Deckbuilding
{
    public class Barrier : Ability
    {
        [SerializeField] private TextMeshProUGUI _shieldText;
        [SerializeField] private GameObject _barrier;
        [SerializeField] private GameObject _fracturedBarrier;

        private StatsSO _playerStats;
        private float _barrierMaxHealth;
        private float _barrierCurrentHealth;

        private float _barrierDamage;

        private void OnEnable()
        {
            EventManager.IsShieldActive += IsShieldActive;
            EventManager.OnShieldHit += TakeDamage;

            _playerStats = EventManager.GetPlayerStats?.Invoke();
            _barrierMaxHealth = _playerStats.GetStat(Stat.Health) * Card.AbilityValues[0].value;
            _barrierCurrentHealth = _barrierMaxHealth;

            _barrierDamage = _playerStats.GetStat(Stat.Damage) * Card.AbilityValues[2].value;

            _barrier.SetActive(true);
            //_shieldText.gameObject.SetActive(true);
            //_shieldText.text = $"SHIELD: {_barrierCurrentHealth}/{_barrierMaxHealth}";
        }

        private void TakeDamage(float damage)
        {
            _barrierCurrentHealth -= damage;
            //_shieldText.text = $"BARRIER: {_barrierCurrentHealth}/{_barrierMaxHealth}";
            if ( _barrierCurrentHealth <= 0 )
            {
                // aoe damage
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, Card.AbilityValues[1].value, Vector3.up, Card.AbilityValues[1].value, LayerMask.GetMask("Enemy"));
                if ( hits.Length > 0 )
                {
                    foreach(var hit in hits)
                    {
                        IDamageable target;
                        if (hit.collider.gameObject.TryGetComponent(out target))
                        {
                            target.TakeDamage(_barrierDamage, gameObject);
                        }
                    }
                }
                _barrier.GetComponent<PreFracturedGeometry>().FractureAndForget();
                _fracturedBarrier.transform.parent = null;
            }

        }

        private bool IsShieldActive()
        {
            if (_barrierCurrentHealth > 0) return true;
            else return false;
        }

        private void OnDisable()
        {
            //_shieldText.gameObject?.SetActive(false);
            

            EventManager.IsShieldActive -= IsShieldActive;
            EventManager.OnShieldHit -= TakeDamage;
        }
    }
}
