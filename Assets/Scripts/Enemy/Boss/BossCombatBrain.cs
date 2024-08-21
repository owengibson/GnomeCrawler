using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GnomeCrawler
{
    public class BossCombatBrain : CombatBrain
    {
        public Action ReachedPhase2Threshold;
        public Action ReachedPhase3Threshold;

        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Transform[] _weaponTransforms;
        [SerializeField] private float[] _weaponHitboxSizes;
        [SerializeField] private GameObject _projectileGO;
        [SerializeField] private GameObject _shockwaveGO;
        [SerializeField] private float _projectileSpeed;

        [SerializeField] private float _phase2HealthThresholdPercentage = .65f;
        [SerializeField] private float _phase3HealthThresholdPercentage = .35f;

        [SerializeField] private Slider _healthSlider;

        [SerializeField] private UnityEvent OnShockwave;
        [SerializeField] private UnityEvent OnBombCharge;
        [SerializeField] private UnityEvent OnBombHitGround;

        private bool _phase2Activated = false;
        private bool _phase3Activated = false;

        private List<Color> _originalColours = new List<Color>();
        private int _originalColorIndex;

        private void Start()
        {
            base.InitialiseVariables();
            _originTransform = _weaponTransforms[0];
            _weaponSize = _weaponHitboxSizes[0];

            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                _originalColours.Add(mat.GetColor("_MainColor"));
            }
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

        public void ChargeProjectile()
        {
            _projectileGO.transform.localScale = Vector3.zero;
            _projectileGO.SetActive(true);
            _projectileGO.transform.parent = _weaponTransforms[0].transform;
            _projectileGO.transform.localPosition = new Vector3(0,1,-1);
            StartCoroutine(ScaleProjectile(Vector3.one * 4, 1.0f));
            OnBombCharge?.Invoke();
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

            _projectileGO.transform.localScale = targetScale;
        }

        public void ShootProjectile()
        {
            _projectileGO.transform.parent = null;
            Vector3 playerPos = PlayerStateMachine.instance.transform.position;
            Vector3 dir = (PlayerStateMachine.instance.transform.position - _originTransform.position).normalized;

            RaycastHit hit;
            float distanceToPlayer = Vector3.Distance(_originTransform.position, playerPos);
            if (Physics.SphereCast(_originTransform.position, 2, dir, out hit, distanceToPlayer, LayerMask.GetMask("Obstacles")))
            {
                playerPos = hit.point;
            }

            StartCoroutine(ProjectileTravel(playerPos, _projectileSpeed));
        }

        private IEnumerator ProjectileTravel(Vector3 finalPosition, float speed)
        {
            while (Vector3.Distance(_projectileGO.transform.position, finalPosition) > 0.1f)
            {
                float step = speed * Time.deltaTime;

                _projectileGO.transform.position = Vector3.MoveTowards(_projectileGO.transform.position, finalPosition, step);

                yield return null;
            }
            _projectileGO.transform.position = finalPosition;

            OnBombHitGround?.Invoke();
            
            yield return StartCoroutine(ScaleProjectile(Vector3.one * 13.5f, 0.5f));

            _projectileGO.SetActive(false);

        }

        public void StartShockwave()
        {
            _shockwaveGO.SetActive(true);
            OnShockwave?.Invoke();
        }

        public override void TakeDamage(float amount, GameObject damager)
        {
            base.TakeDamage(amount, damager);

            _healthSlider.value = CurrentHealth;

            DamageFeedback();
        }

        private void DamageFeedback()
        {
            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                mat.SetColor("_MainColor", Color.black);
            }
            Invoke("ResetColour", .15f);
        }

        private void ResetColour()
        {
            foreach (Renderer renderer in _renderers)
            {
                Material mat = renderer.material;
                mat.SetColor("_MainColor", _originalColours[_originalColorIndex]);
                _originalColorIndex++;
            }
            _originalColorIndex = 0;
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
