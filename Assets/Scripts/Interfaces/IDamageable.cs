using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }

    void Damage(float damageAmount);
    void Die();
}
