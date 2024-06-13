using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class BossCombatBrain : CombatBrain
    {
        public Action ReachedPhase2Threshold;
        public Action ReachedPhase3Threshold;

        [SerializeField] private float _phase2HealthThresholdPercentage = .65f;
        [SerializeField] private float _phase3HealthThresholdPercentage = .35f;

        private bool _phase2Activated = false;
        private bool _phase3Activated = false;

        private void Start()
        {
            base.InitialiseVariables();
        }

        private void Update()
        {
            base.InternalUpdate();
            if (CurrentHealth / _maxHealth * 100 < _phase2HealthThresholdPercentage && !_phase2Activated)
            {
                _phase2Activated = true;
                ReachedPhase2Threshold?.Invoke();
            }
            if (CurrentHealth / _maxHealth * 100 < _phase3HealthThresholdPercentage && !_phase3Activated)
            {
                _phase3Activated = true;
                ReachedPhase3Threshold?.Invoke();
            }

        }

        public override void Die()
        {
            base.Die();
        }

        
    }
}