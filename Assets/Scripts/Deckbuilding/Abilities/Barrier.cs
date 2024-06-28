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
        [Space]

        [SerializeField] private MeshRenderer _barrierRenderer;

        private StatsSO _playerStats;
        private float _barrierMaxHealth;
        private float _barrierCurrentHealth;

        private float _barrierDamage;

        private Color _barrierColor;
        private Color _barrierDamageColor = new Color32(14, 13, 7, 255);

        private void OnEnable()
        {
            EventManager.IsShieldActive += IsShieldActive;
            EventManager.OnShieldHit += TakeDamage;

            _playerStats = EventManager.GetPlayerStats?.Invoke();
            _barrierMaxHealth = _playerStats.GetStat(Stat.Health) * Card.AbilityValues[0].value;
            _barrierCurrentHealth = _barrierMaxHealth;

            _barrierDamage = _playerStats.GetStat(Stat.Damage) * Card.AbilityValues[2].value;

            _barrierColor = _barrierRenderer.material.color;

            _barrier.SetActive(true);
            //_shieldText.gameObject.SetActive(true);
            //_shieldText.text = $"BARRIER: {_barrierCurrentHealth}/{_barrierMaxHealth}";
        }

        private void TakeDamage(float damage)
        {
            _barrierCurrentHealth -= damage;
            StartCoroutine(DamageFeedback());

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
                _fracturedBarrier.transform.parent = null;
                _barrier.GetComponent<PreFracturedGeometry>().FractureAndForget();
            }

        }

        private IEnumerator DamageFeedback()
        {
            _barrierRenderer.material.color = _barrierDamageColor;
            yield return new WaitForSeconds(0.25f);
            _barrierRenderer.material.color = _barrierColor;
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
