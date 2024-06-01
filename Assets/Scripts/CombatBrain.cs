using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System;
using UnityEngine;

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
        public float CurrentHealth { get; set; }
        public bool IsDead { get; set; }

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
        }

        protected virtual void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponSize, _layerMask))
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

        public virtual void EndDealDamage()
        {
            _canDealDamage = false;
        }

        public virtual void TakeDamage(float amount)
        {
            print(name + " has taken damage");
            CurrentHealth -= amount;
            DamageTaken?.Invoke();

            if (CurrentHealth <= 0) Die();
        }

        public virtual void Die()
        {
            Debug.Log(gameObject + " killed");
            EventManager.OnEnemyKilled?.Invoke(gameObject);
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
