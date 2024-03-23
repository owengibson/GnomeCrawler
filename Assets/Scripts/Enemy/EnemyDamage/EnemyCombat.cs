using GnomeCrawler.Deckbuilding;
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


        private void Start()
        {
            base.InitialiseVariables();
            _damagedGameObjects = new List<GameObject>();
        }

        protected override void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponLength, _layerMask))
            {
                if (hit.transform.TryGetComponent(out IDamageable damageable) && !_damagedGameObjects.Contains(hit.transform.gameObject))
                {
                    print("hit " + hit.transform.gameObject);
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage));
                    _damagedGameObjects.Add(hit.transform.gameObject);
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
            _enemyAnim.SetBool("isDamaged", true);
        }

        private void EndHurtAnimation()
        {
            _enemyAnim.SetBool("isDamaged", false);
        }

    }
}
