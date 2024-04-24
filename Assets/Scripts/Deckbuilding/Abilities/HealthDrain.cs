using Cinemachine;
using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class HealthDrain : Ability
    {
        private void OnEnable()
        {
            StartCoroutine(DrainHealth());
        }

        private void OnDisable()
        {
            StopCoroutine(DrainHealth());
        }

        private IEnumerator DrainHealth()
        {
            yield return new WaitForSeconds(Card.AbilityValues[1].value);
            EventManager.OnPlayerHurtFromAbility?.Invoke(Card.AbilityValues[0].value);
            StartCoroutine(DrainHealth());
        }
    }
}
