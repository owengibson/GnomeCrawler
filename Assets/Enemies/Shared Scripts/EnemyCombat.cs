using GnomeCrawler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour, IDamageable
{

    [SerializeField] private int _weaponDamage;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;


    private bool _canDealDamage = true;

    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!_canDealDamage) return;
        
        if(other.gameObject.tag == "Player")
        {
            //playerStateMachine.TakeDamage(_weaponDamage);
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

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
    }
}
