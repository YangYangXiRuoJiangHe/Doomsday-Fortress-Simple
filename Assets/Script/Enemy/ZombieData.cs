using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieData : MonoBehaviour,IDamagable
{
    public float health = 15;
    public bool isDead = false;
    public float GetHealth() => health;

    public void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}
