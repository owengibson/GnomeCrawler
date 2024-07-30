using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace GnomeCrawler
{
    public class CombatBrain : MonoBehaviour, IDamageable, IKillable
    {
        public Action DamageTaken;
        public Transform _lockOnTransform;

        #region health
        protected float _maxHealth;
        #endregion

        #region damage
        [SerializeField] protected float _weaponSize;
        [SerializeField] protected bool _canDealDamage;
        protected bool _hasDealtDamage;
        #endregion

        #region raycast
        [SerializeField] protected LayerMask _layerMask;
        [SerializeField] protected Transform _originTransform;
        #endregion

        [SerializeField] protected StatsSO _stats;
        public UnityEvent OnDamageConnected;
        [SerializeField] private UnityEvent OnDamaged;
        [SerializeField] private UnityEvent OnDeath;
        public float CurrentHealth { get; set; }
        public bool IsDead { get; set; }
        public bool IsLockable { get; set; }

        private void Start()
        {
            InitialiseVariables();
        }

        private void Update()
        {
            InternalUpdate();
        }

        protected void InternalUpdate()
        {
            if (_canDealDamage)
            {
                CheckForRaycastHit();
            }
        }

        protected virtual void InitialiseVariables()
        {
            _maxHealth = _stats.GetStat(Stat.Health);
            CurrentHealth = _maxHealth;

            _canDealDamage = false;
            _hasDealtDamage = false;
            IsDead = false;
            IsLockable = true;
        }

        protected virtual void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponSize, _layerMask))
            {

                if (hit.transform.TryGetComponent(out IDamageable damageable) && !_hasDealtDamage)
                {
                    damageable.TakeDamage(_stats.GetStat(Stat.Damage), gameObject);
                    _hasDealtDamage = true;
                }
            }
        }

        public virtual void StartDealDamage()
        {
            _canDealDamage = true;
            _hasDealtDamage = false;
        }

        public virtual void EndDealDamage()
        {
            _canDealDamage = false;
        }

        public virtual void TakeDamage(float amount, GameObject damager)
        {
            print(name + " has taken damage");
            CurrentHealth -= amount;
            DamageTaken?.Invoke();
            OnDamaged?.Invoke();

            if (CurrentHealth <= 0) Die();
        }

        public virtual void Die()
        {
            Debug.Log(gameObject + " killed");
            EventManager.OnEnemyKilled?.Invoke(gameObject);
            OnDeath?.Invoke();
            IsDead = true;
            Destroy(gameObject);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position - _originTransform.up * _weaponSize);
            Gizmos.DrawLine(_originTransform.position - _originTransform.up * _weaponSize, _originTransform.position);
        }
    }
}
