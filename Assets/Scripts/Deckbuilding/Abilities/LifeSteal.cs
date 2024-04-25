using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    public class LifeSteal : Ability
    {

        private void OnEnable()
        {
            EventManager.OnPlayerHit += StealLife;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerHit -= StealLife;
        }

        private void StealLife(float amount)
        {
            EventManager.OnPlayerLifeSteal?.Invoke(amount * Card.AbilityValues[0].value);
        }
    }
}
