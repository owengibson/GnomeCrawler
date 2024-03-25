using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerCombat : CombatBrain
    {
        private List<GameObject> _damagedGameObjects;

        private void Start()
        {
            base.InitialiseVariables();
            _damagedGameObjects = new List<GameObject>();
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

        public override void StartDealDamage()
        {
            _damagedGameObjects.Clear();
            base.StartDealDamage();
        }

        private void AddHandToStats(List<CardSO> hand)
        {
            foreach (CardSO card in hand)
            {
                _stats.AddCard(card);
                if (card.UpgradedStat.Key == Stat.Health)
                {
                    CurrentHealth += _stats.GetStat(Stat.Health) - _maxHealth;
                    _maxHealth = _stats.GetStat(Stat.Health);
                }
            }

            EventManager.OnHandDrawn?.Invoke();
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
            EventManager.OnHandApproved += AddHandToStats;
        }

        private void OnDisable()
        {
            EventManager.OnHandApproved -= AddHandToStats;
        }
    }
}
