using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Deal Damage");
            // add the health system and damage here
        }
    }
}
