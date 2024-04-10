using System.Collections;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    //public class EnemyProjectile : EnemyCombat
    //{
    //    void Start()
    //    {
    //        base.InitialiseVariables();
    //        _canDealDamage = true;
    //        StartCoroutine(DestroyProjectile());
    //    }

    //    protected override void CheckForRaycastHit() { }

    //    private IEnumerator DestroyProjectile()
    //    {
    //        yield return new WaitForSeconds(4);
    //        _canDealDamage = false;
    //        Destroy(this);
    //    }

    //}

    public class EnemyRangedCombat : CombatBrain
    {
        //public EnemyProjectile Projectile;

        [SerializeField] protected ProgressBar _healthBar;
        [SerializeField] protected Animator _enemyAnim;
        [SerializeField] protected Renderer _meshRenderer;
        Color _orginalColor;

        private void Start()
        {
            base.InitialiseVariables();
            _orginalColor = _meshRenderer.material.color;
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

        public void SetUpHealthBar(Canvas canvas, Camera camera)
        {
            _healthBar.transform.SetParent(canvas.transform);
            if (_healthBar.TryGetComponent<FaceCameraScript>(out FaceCameraScript faceCamera))
            {
                faceCamera.FaceCamera = camera;
            }
        }
        public override void StartDealDamage()
        {
            //EnemyProjectile clone = Instantiate(Projectile);

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

    }
}
