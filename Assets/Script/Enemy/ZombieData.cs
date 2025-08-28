using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieData : MonoBehaviour,IDamagable
{
    public float health = 15;
    public bool isDead = false;
    public moveAgent enemyAgent;
    public Animator anim;
    public int numberDeadAnimation;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    public float GetHealth() => health;

    public void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0 && !isDead)
        {
            Dead();
        }
    }
    public void Dead()
    {
        isDead = true;
        enemyAgent.SetAgentSpeed(0);
        anim.SetBool("Dead", isDead);
        int deadIndex = Random.Range(0,2);
        anim.SetFloat("DeadIndex", deadIndex);
        anim.applyRootMotion = true;
        SourceManager.instance.AddSourceData("corpse", numberDeadAnimation);
    }

}
