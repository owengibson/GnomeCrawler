using GnomeCrawler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHitBox : MonoBehaviour
{

    [SerializeField] private int _weaponDamage;
    [SerializeField] private PlayerStateMachine playerStateMachine;


    private bool _canDealDamage = true;

    private void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(!_canDealDamage) return;
        
        if(other.gameObject.tag == "Player")
        {
            playerStateMachine.TakeDamage(_weaponDamage);
            //Debug.Log("Deal Damage");
            // add the health system and damage here
        }
    }

    public void StartDealDamage()
    {
        _canDealDamage = true;
    }

    public void StopDealDamage()
    {
        _canDealDamage = false;
    }

}
