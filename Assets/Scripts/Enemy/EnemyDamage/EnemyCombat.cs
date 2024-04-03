using GnomeCrawler.Deckbuilding;
using Sirenix.OdinInspector.Editor.Validation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyCombat : CombatBrain
    {
        private List<GameObject> _damagedGameObjects;
        [SerializeField] protected ProgressBar _healthBar;
        [SerializeField] protected Animator _enemyAnim;
        protected MeshRenderer _meshRenderer;
        Color _orginalColor;

        private void Start()
        {
            base.InitialiseVariables();
            _damagedGameObjects = new List<GameObject>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _orginalColor = _meshRenderer.material.color;
        }

        protected override void CheckForRaycastHit()
        {
            Collider[] hitColliders = Physics.OverlapSphere(_originTransform.position, _weaponSize, _layerMask);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out IDamageable damageable) && !_damagedGameObjects.Contains(hitCollider.transform.gameObject))
                {
                    print("hit " + hitCollider.transform.gameObject);
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage));
                    _damagedGameObjects.Add(hitCollider.transform.gameObject);
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
            _damagedGameObjects.Clear();
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
            _meshRenderer.material.color = Color.black;
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
             _meshRenderer.material.color = _orginalColor;
        }
    }
}
