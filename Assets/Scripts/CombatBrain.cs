using GnomeCrawler.Deckbuilding;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class CombatBrain : MonoBehaviour, IDamageable
    {
        #region health
        [SerializeField] private float _maxHealth;
        #endregion

        #region damage
        [SerializeField] protected float _weaponLength;
        [SerializeField] protected bool _canDealDamage;
        protected bool _hasDealtDamage;
        #endregion

        #region raycast
        [SerializeField] protected LayerMask _layerMask;
        [SerializeField] protected Transform _originTransform;
        #endregion

        [SerializeField] protected StatsSO _stats;

        public float CurrentHealth { get; set; }

        private void Start()
        {
            _maxHealth = _stats.GetStat(Stat.Health);
            CurrentHealth = _maxHealth;

            _canDealDamage = false;
            _hasDealtDamage = false;
        }

        private void Update()
        {
            if (_canDealDamage)
            {
                CheckForRaycastHit();
            }
        }

        protected virtual void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponLength, _layerMask))
            {

                if (hit.transform.TryGetComponent(out IDamageable damageable) && !_hasDealtDamage)
                {
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage));
                    _hasDealtDamage = true;
                }
            }
        }

        public virtual void StartDealDamage()
        {
            _canDealDamage = true;
            _hasDealtDamage = false;
        }

        public void EndDealDamage()
        {
            _canDealDamage = false;
        }

        public void TakeDamage(float amount)
        {
            CurrentHealth -= amount;
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position - _originTransform.up * _weaponLength);
        }
    }
}
