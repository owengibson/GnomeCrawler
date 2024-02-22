using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class PlayerWeaponHitBox : MonoBehaviour
    {

        [SerializeField] private int _weaponDamage;
        [SerializeField] private EnemyStateManager enemyStateManager;


        private bool _canDealDamage = true;

        private void Start()
        {
            
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!_canDealDamage) return;

            if (other.gameObject.tag == "Enemy")
            {
                enemyStateManager.TakeDamage(_weaponDamage);
                Debug.Log("Deal Damage");
                // add the health system and damage here
            }
        }

        public void StartDealDamage()
        {
            // _canDealDamage = true;
            GetComponent<BoxCollider>().enabled = false;
        }

        public void StopDealDamage()
        {
            //_canDealDamage = false;
            GetComponent<BoxCollider>().enabled = true;
        }

    }
}
