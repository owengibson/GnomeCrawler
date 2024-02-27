using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

namespace GnomeCrawler
{
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        #region health
        [SerializeField] private float _maxHealth;
        #endregion

        #region damage
        [SerializeField] private float _weaponLength;
        [SerializeField] private float _weaponDamage;
        [SerializeField] private bool _canDealDamage;
        List<GameObject> _hasDealtDamage;
        #endregion

        #region raycast
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Transform _originTransform;
        #endregion

        public float CurrentHealth { get; set; }

        private void Start()
        {
            CurrentHealth = _maxHealth;

            _canDealDamage = false;
            _hasDealtDamage = new List<GameObject>();
        }

        private void Update()
        {
            if (_canDealDamage)
            {
                CheckForRaycastHit();
            }
        }

        private void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponLength, _layerMask))
            {

                if (hit.transform.TryGetComponent(out IDamageable damageable) && !_hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    damageable.TakeDamage(_weaponDamage);
                    _hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }

        public void StartDealDamage()
        {
            _canDealDamage = true;
            _hasDealtDamage.Clear();
        }

        public void EndDealDamage()
        {
            _canDealDamage = false;
        }

        public void TakeDamage(float amount)
        {
            CurrentHealth -= amount;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position -  _originTransform.up * _weaponLength);
        }
    }
}
