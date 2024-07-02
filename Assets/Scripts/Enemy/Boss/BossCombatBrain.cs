using GnomeCrawler.Deckbuilding;
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

        [SerializeField] private Transform[] _weaponTransforms;
        [SerializeField] private float[] _weaponHitboxSizes;

        [SerializeField] private float _phase2HealthThresholdPercentage = .65f;
        [SerializeField] private float _phase3HealthThresholdPercentage = .35f;

        private bool _phase2Activated = false;
        private bool _phase3Activated = false;

        private void Start()
        {
            base.InitialiseVariables();
            _originTransform = _weaponTransforms[0];
            _weaponSize = _weaponHitboxSizes[0];
        }

        protected override void CheckForRaycastHit()
        {
            Collider[] hitColliders = Physics.OverlapSphere(_originTransform.position, _weaponSize, _layerMask);

            if (hitColliders.Length <= 0) return;

            foreach (var collider in hitColliders)
            {
                if (collider.transform.TryGetComponent(out IDamageable damageable) && !_hasDealtDamage)
                {
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage), gameObject);
                    _hasDealtDamage = true;
                }
            }
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

        public void ChangeWeaponOrigin(int index)
        {
            _originTransform = _weaponTransforms[index];
            _weaponSize = _weaponHitboxSizes[index];
        }

        public void ExpandOverlapSphere()
        {
            _weaponSize += 0.5f * Time.deltaTime;
        }

        public override void Die()
        {
            base.Die();
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_originTransform.position, _weaponSize);
        }
    }
}
