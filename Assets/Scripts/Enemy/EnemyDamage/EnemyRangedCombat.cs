using System.Collections;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    //public class EnemyProjectile : EnemyCombat
    //{
    //    protected float _projectileDamage;
    //    public float DamageDealt { get; set; }


    //    void Start()
    //    {
    //        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //        //sphere.transform.localScale = Vector3.one;
    //        //sphere.transform.localPosition = Vector3.zero;
    //        _canDealDamage = true;
    //        // create sphere method
    //        StartCoroutine(DestroyProjectile());
    //    }

    //    //public EnemyProjectile()
    //    //{
    //    //    _projectileDamage = _stats.GetStat(Deckbuilding.Stat.Damage);
    //    //    DamageDealt = _projectileDamage;
    //    //}


    //    protected override void CheckForRaycastHit() { }

    //    private IEnumerator DestroyProjectile()
    //    {
    //        yield return new WaitForSeconds(4);
    //        _canDealDamage = false;
    //        Destroy(this);
    //    }
    //}

    public class EnemyRangedCombat : EnemyCombat
    {
        private EnemyProjectile _enemyProjectile;
        private Transform _playerTransform;
        private float _speed = 5f;
        [SerializeField] private Transform _handTransform;

        private void Start()
        {
            base.InitialiseVariables();

            foreach (Material mat in _meshRenderer.materials)
            {
                _originalColours.Add(mat.color);
            }
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            _healthBar.SetProgress(CurrentHealth / _maxHealth);
            /*hurtstate*/ DamageFeedback();
        }

        public override void Die()
        {
            base.Die();
            Destroy(_healthBar.gameObject);
        }

        public override void StartDealDamage()
        {
            _canDealDamage = true;
            CreateBullet();
            //GameObject sphere = new GameObject("EnemySphere");
            //EnemyProjectile clone = Instantiate(_enemyProjectile);
            //EnemyProjectile projectileComponent = sphere.AddComponent<EnemyProjectile>();

            /* 
             * 
             * instantiate the thing
             * with the above script
             * and it's own stats
             *  
             */
        }

        private void DamageFeedback() // move to hurtstate
        {
            _meshRenderer.material.color = Color.black;
            _enemyAnim.SetBool("isDamaged", true);
            Invoke("ResetColour", .15f);
        }

        private void EndHurtAnimation() // move to hurtstate
        {
            _enemyAnim.SetBool("isDamaged", false);
        }

        private void CreateBullet()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.localScale = Vector3.one;
            sphere.transform.localPosition = _handTransform.position;
            _originTransform = sphere.transform;
            sphere.layer = LayerMask.NameToLayer("Enemy");

            Vector3 direction = (_playerTransform.position - sphere.transform.position).normalized;

            Rigidbody rb = sphere.AddComponent<Rigidbody>();
            rb.useGravity = false; 
            rb.velocity = direction * _speed;

            Destroy(sphere, 2f);
        }

    }
}
