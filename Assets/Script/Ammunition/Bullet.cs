using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private IDamagable damgable;
    public float speed;
    public float damage;
    public Vector3 target;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
    public void SetupProjectile(Vector3 newTarget, float newDamage, float newSpeed,IDamagable newDamagable)
    {
        speed = newSpeed;
        damage = newDamage;
        target = newTarget;
        damgable = newDamagable;
    }
    private void OnTriggerEnter(Collider other)
    {
        other.transform.GetComponent<ZombieData>()?.TakeDamage(damage);
        Destroy(this.gameObject);
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position,target) <= .01f)
        {
            damgable?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
