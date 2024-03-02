using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using UnityEngine;

namespace GnomeCrawler
{
    public class CombatBrain : MonoBehaviour, IDamageable, IKillable
    {
        #region health
        [SerializeField] protected float _maxHealth;
        [SerializeField] protected ProgressBar _healthBar;
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
            InitialiseVariables();
        }

        private void Update()
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
            print("start deal damage");
            _canDealDamage = true;
            _hasDealtDamage = false;
        }

        public void EndDealDamage()
        {
            print("end deal damage");
            _canDealDamage = false;
        }

        public void TakeDamage(float amount)
        {
            CurrentHealth -= amount;

            _healthBar.SetProgress(CurrentHealth / _maxHealth * 3);

            if (CurrentHealth <= 0) Die();
        }

        public virtual void Die()
        {
            EventManager.OnEnemyKilled?.Invoke(gameObject);
            Destroy(gameObject);
            Destroy(_healthBar);

        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position - _originTransform.up * _weaponLength);
        }

        public void SetUpHealthBar(Canvas canvas, Camera camera)
        {
            _healthBar.transform.SetParent(canvas.transform);
            if(_healthBar.TryGetComponent<FaceCameraScript>(out FaceCameraScript faceCamera))
            {
                faceCamera.FaceCamera = camera;
            }
        }
    }
}
