using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Player;
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
        [SerializeField] private GameObject _projectileGO;
        [SerializeField] private GameObject _shockwaveGO;
        [SerializeField] private float _projectileSpeed;

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

        public void ChargeProjectile()
        {
            _projectileGO.SetActive(true);
            _projectileGO.transform.parent = _weaponTransforms[0].transform;
            _projectileGO.transform.localPosition = new Vector3(0,1,-1);
            _projectileGO.transform.localScale = Vector3.zero;
            StartCoroutine(ScaleProjectile(Vector3.one * 3, 1.0f));
        }

        private IEnumerator ScaleProjectile(Vector3 targetScale, float duration)
        {
            Vector3 initialScale = _projectileGO.transform.localScale;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                _projectileGO.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final scale is exactly the target scale
            _projectileGO.transform.localScale = targetScale;
        }

        public void ShootProjectile()
        {
            _projectileGO.transform.parent = null;
            StartCoroutine(ProjectileTravel(PlayerStateMachine.instance.transform.position, _projectileSpeed));
        }

        private IEnumerator ProjectileTravel(Vector3 finalPosition, float speed)
        {
            Vector3 direction = finalPosition - _projectileGO.transform.position;
            direction.Normalize();

            while (Vector3.Distance(_projectileGO.transform.position, finalPosition) > 0.1f)
            {
                float step = speed * Time.deltaTime;

                _projectileGO.transform.position = Vector3.MoveTowards(_projectileGO.transform.position, finalPosition, step);

                //Debug.Log($"Moving to {finalPosition}, current position: {_projectileGO.transform.position}");

                yield return null;
            }
            _projectileGO.transform.position = finalPosition;
            
            yield return StartCoroutine(ScaleProjectile(Vector3.one * 10, 0.5f));
            _projectileGO.SetActive(false);
            Debug.Log("Projectile reached the final position");
        }

        public void StartShockwave()
        {
            _shockwaveGO.SetActive(true);
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
