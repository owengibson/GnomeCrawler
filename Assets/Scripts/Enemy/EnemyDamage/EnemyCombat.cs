using GnomeCrawler.Deckbuilding;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemyCombat : CombatBrain
    {
        [SerializeField] protected ProgressBar _healthBar;
        [SerializeField] protected Animator _enemyAnim;
        [SerializeField] protected Renderer _meshRenderer;
        protected List<Color> _originalColours = new List<Color>();
        protected int _originalColorIndex;

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
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage));
                    _hasDealtDamage = true;
                }
            }
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
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

        public override void Die()
        {
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

        private void DamageFeedback()
        {
            foreach(Material mat in _meshRenderer.materials)
            {
                mat.SetColor("_MainColor", Color.black);
            }
            _enemyAnim.SetBool("isDamaged", true);
            Invoke("ResetColour", .15f);
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

        private void ResetColour()
        {
            foreach (Material mat in _meshRenderer.materials)
            {
                mat.SetColor("_MainColor", _originalColours[_originalColorIndex]);
                _originalColorIndex++;
            }
            _originalColorIndex = 0;
        }
    }
}
