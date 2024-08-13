using GnomeCrawler.Deckbuilding;
using System;
using GnomeCrawler.Systems;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DinoFracture;
using UnityEngine.Events;

namespace GnomeCrawler.Enemies
{
    public class EnemyCombat : CombatBrain
    {
        [SerializeField] private bool _isPotPlant = false;
        [SerializeField] private PreFracturedGeometry _potFractureGeo;
        [SerializeField] private GameObject _fraturedPot;
        [SerializeField] protected ProgressBar _healthBar;
        [SerializeField] protected Animator _enemyAnim;
        [SerializeField] protected Renderer _meshRenderer;
        [SerializeField] protected List<ParticleSystem> _particleSystems = new List<ParticleSystem>();

        [SerializeField] private UnityEvent _finishAttack;

        protected List<Color> _originalColours = new List<Color>();
        protected int _originalColorIndex;

        private bool _isUnflinchable = false;

  
        private void Start()
        {
            base.InitialiseVariables();

            foreach (Material mat in _meshRenderer.materials)
            {
                _originalColours.Add(mat.GetColor("_MainColor"));
            }
        }

        protected override void CheckForRaycastHit()
        {

            Collider[] hitColliders = Physics.OverlapSphere(_originTransform.position, _weaponSize, _layerMask);

            foreach (var hitCollider in hitColliders)
            {

                if (hitCollider.transform.TryGetComponent(out IDamageable damageable) && !_hasDealtDamage)
                {
                    OnDamageConnected?.Invoke();
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage), gameObject);
                    _hasDealtDamage = true;
                }
            }
        }

        public override void TakeDamage(float amount, GameObject damager)
        {
            base.TakeDamage(amount, damager);
            _healthBar.SetProgress(CurrentHealth / _maxHealth);
            DamageFeedback();
        }

        public override void StartDealDamage()
        {
            base.StartDealDamage();
        }

        public override void EndDealDamage()
        {
            _canDealDamage = false;
        }

        public void HitGround()
        {
            _particleSystems[0].Play();
            _finishAttack?.Invoke();
        }

        public override void Die()
        {
            if (_isPotPlant)
            {
                _fraturedPot.transform.parent = null;
                _potFractureGeo.FractureAndForget();
            }

            base.Die();
            Destroy(_healthBar.gameObject);
        }

        public void SetUpHealthBar(Canvas canvas, Camera camera)
        {
            _healthBar.transform.SetParent(canvas.transform);
            if (_healthBar.TryGetComponent<FaceCameraScript>(out FaceCameraScript faceCamera))
            {
                faceCamera.FaceCamera = camera;
            }
        }

        protected virtual void DamageFeedback()
        {
            foreach(Material mat in _meshRenderer.materials)
            {
                mat.SetColor("_MainColor", Color.black);
            }
            Invoke("ResetColour", .15f);

            if (_isUnflinchable) return;
            _enemyAnim.SetBool("isDamaged", true);
        }

        private void EndHurtAnimation()
        {
            _enemyAnim.SetBool("isDamaged", false);
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_originTransform.position, _weaponSize);
        }

        protected virtual void ResetColour()
        {
            foreach (Material mat in _meshRenderer.materials)
            {
                mat.SetColor("_MainColor", _originalColours[_originalColorIndex]);
                _originalColorIndex++;
            }
            _originalColorIndex = 0;
        }

        private void ToggleUnflinchability(bool unflinchability)
        {
            _isUnflinchable = unflinchability;
        }

        private void OnEnable()
        {
            EventManager.OnAttackAbilityToggle += ToggleUnflinchability;
        }

        private void OnDisable()
        {
            EventManager.OnAttackAbilityToggle -= ToggleUnflinchability;
        }
    }
}
