using UnityEngine;

public class DefenseTower : Tower
{
    [Header("塔的基本信息")]
    public float damage = 1;
    [Header("射击信息")]
    public Transform[] shootPoint;
    public int shootPointPosition = 0;
    public LayerMask isEnemy;
    public float targetDistance = float.MaxValue;
    public float shootSpeed = 1;
    public float shootTime = 0;
    public float checkRadiu;
    public GameObject target;
    public bool canAttack;
    public float bulletSpeed = 200;
    [Header("子弹预制体")]
    public GameObject bulletPrefab;
    [Header("音效片段")]
    public AudioSource[] attackSFXs;
    // Start is called before the first frame update
    protected virtual void AttackEnemy()
    {
        //目标为空，或者目标为非激活（为后期做对象池打基础），或者找不到组件（如果destroy这个gameobject可能会陷入伪非空，但此时访问组件会错误），或者死亡则重新找
        if (target == null || !target.gameObject.activeSelf || target.GetComponent<ZombieData>() == null || target.GetComponent<ZombieData>().isDead)
        {
            target = FindTargetEnemy();
        }
        if (target != null && target.GetComponent<ZombieData>() != null)
        {
            Vector3 direction = (target.transform.position - rotationOffset - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            //平滑不恒速，一般用于camera等物体
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //恒速，每帧旋转多少度，一般用于炮台等
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (Time.time >= shootTime && angle < 3f)
            {
                Shoot(target.transform, target.GetComponent<ZombieData>());
                foreach (AudioSource sfx in attackSFXs)
                {
                    if (!sfx.isPlaying)
                    {
                        AudioManager.instance.PlaySFX(sfx);
                        break;
                    }
                }
                shootTime = Time.time + shootSpeed; // 固定间隔
            }
        }
    }
    protected GameObject FindTargetEnemy()
    {
        Collider[] Enemys = Physics.OverlapSphere(transform.position, checkRadiu, isEnemy);
        foreach (Collider enemy in Enemys)
        {
            if (enemy.GetComponent<ZombieData>().isDead)
            {
                continue;
            }
            float distanct = Vector3.Distance(enemy.transform.position, transform.position);
            if (distanct < targetDistance)
            {
                target = enemy.gameObject;
                targetDistance = distanct;
            }
        }
        targetDistance = float.MaxValue;
        return target;
    }
    protected virtual void Shoot(Transform target, IDamagable damagable)
    {
        int i = shootPointPosition % shootPoint.Length;
        if (damagable == null)
        {
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, shootPoint[i].position, shootPoint[i].rotation);
        bullet.GetComponent<Bullet>().SetupProjectile(target.position, damage, bulletSpeed, damagable);
        shootPointPosition = (shootPointPosition + 1) % shootPoint.Length; //防溢出
    }
    public void setCanAttack(bool isCanAttack)
    {
        canAttack = isCanAttack;

    }
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadiu);
        Gizmos.DrawWireSphere(fountionPosition.position, createRadio);
    }
}
