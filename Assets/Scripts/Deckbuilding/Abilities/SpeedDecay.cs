using DG.Tweening;
using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class SpeedDecay : Ability
    {

        [SerializeField] private ParticleSystem pSystem;
        [SerializeField] private Tween tween;
        private void OnEnable()
        {
            EventManager.OnEnemyKilled += DecaySpeed;
            DecaySpeed(gameObject);
        }

        private void OnDisable()
        {
            EventManager.OnEnemyKilled -= DecaySpeed;
            pSystem.Stop();
        }

        private void Update()
        {
            Card.ValidateStats();
            if (!tween.IsPlaying())
            {
                pSystem.Stop();
            }
        }

        private void DecaySpeed(GameObject gameObject)
        {
            pSystem.Play();
            Card.Value = Card.AbilityValues[0].value;
            tween = DOTween.To(() => Card.Value, x => Card.Value = x, 0, Card.AbilityValues[1].value);
        }
    }
}
