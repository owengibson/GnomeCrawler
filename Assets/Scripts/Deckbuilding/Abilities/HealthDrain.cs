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
            //StartCoroutine(DrainHealthCO());
            InvokeRepeating("DrainHealth", 0, Card.AbilityValues[1].value);
        }

        private void OnDisable()
        {
            //StopCoroutine(DrainHealthCO());
            CancelInvoke();
        }

        private IEnumerator DrainHealthCO()
        {
            yield return new WaitForSeconds(Card.AbilityValues[1].value);
            EventManager.OnPlayerHurtFromAbility?.Invoke(Card.AbilityValues[0].value);
            StartCoroutine(DrainHealthCO());
        }

        private void DrainHealth()
        {
            EventManager.OnPlayerHurtFromAbility?.Invoke(Card.AbilityValues[0].value);
        }
    }
} 
