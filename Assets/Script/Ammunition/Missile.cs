using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed;
    public float damage;
    public Vector3 target;
    public float damageAreaDistance;
    public LayerMask isEnemy;
    private void Start()
    {
        Destroy(gameObject, 15f);
    }
    public void SetupProjectile(Vector3 newTarget, float newDamage, float newSpeed, float newDamageAreaDistance)
    {
        speed = newSpeed;
        damage = newDamage;
        target = newTarget;
        damageAreaDistance = newDamageAreaDistance;
    }
    private void OnTriggerEnter(Collider other)
    {
        MissileExplode();
    }

    private void MissileExplode()
    {
        Collider[] enemys = Physics.OverlapSphere(transform.position, damageAreaDistance, isEnemy);
        IDamagable damagable = null;
        foreach (Collider enemy in enemys)
        {
            if (enemy.GetComponent<ZombieData>().isDead)
            {
                continue;
            }
            damagable = enemy.GetComponent<ZombieData>();
            damagable?.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= .01f)
        {
            MissileExplode();
        }
    }
}
