using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GnomeCrawler.Player
{
    public class PlayerCombat : CombatBrain
    {
        private List<GameObject> _damagedGameObjects;
        [SerializeField] private Slider _healthbarSlider;

        private void Start()
        {
            base.InitialiseVariables();
            _damagedGameObjects = new List<GameObject>();
            _healthbarSlider.maxValue = _maxHealth;
            _healthbarSlider.value = CurrentHealth;
        }

        protected override void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponLength, _layerMask))
            {
                if (hit.transform.TryGetComponent(out IDamageable damageable) && !_damagedGameObjects.Contains(hit.transform.gameObject))
                {
                    print("hit " + hit.transform.gameObject);
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage));
                    _damagedGameObjects.Add(hit.transform.gameObject);
                }
            }
        }

        [Button]
        private void DealOneDamage()
        {
            TakeDamage(1);
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            _healthbarSlider.value = CurrentHealth;
        }

        public override void StartDealDamage()
        {
            _damagedGameObjects.Clear();
            base.StartDealDamage();
        }

        private void AddCardToStats(CardSO card)
        {
            _stats.AddCard(card);

            if (card.UpgradedStat.Key == Stat.Health)
            {
                CurrentHealth += _stats.GetStat(Stat.Health) - _maxHealth;
                _maxHealth = _stats.GetStat(Stat.Health);
            }
        }

        private void OnApplicationQuit()
        {
            _stats.ResetCards();
        }

        public override void Die()
        {
            base.Die();
        }

        private void OnEnable()
        {
            EventManager.OnCardChosen += AddCardToStats;
        }

        private void OnDisable()
        {
            EventManager.OnCardChosen -= AddCardToStats;
        }
    }
}
