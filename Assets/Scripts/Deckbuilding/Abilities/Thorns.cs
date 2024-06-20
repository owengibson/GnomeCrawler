using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class Thorns : Ability
    {
        private void ApplyThorns(float damage, GameObject target)
        {
            if (target.TryGetComponent<IDamageable>(out var targetDamageable))
            {
                targetDamageable.TakeDamage(damage * Card.AbilityValues[0].value, gameObject);
            }

        }

        private void OnEnable()
        {
            EventManager.OnPlayerAttacked += ApplyThorns;
        }
        private void OnDisable()
        {
            EventManager.OnPlayerAttacked -= ApplyThorns;
        }
    }
}
