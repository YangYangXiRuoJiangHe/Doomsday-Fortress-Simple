using UnityEngine;

public class DefenseTower : Tower
{
    [Header("���Ļ�����Ϣ")]
    public float damage = 1;
    [Header("�����Ϣ")]
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
    [Header("�ӵ�Ԥ����")]
    public GameObject bulletPrefab;
    [Header("��ЧƬ��")]
    public AudioSource[] attackSFXs;
    // Start is called before the first frame update
    protected virtual void AttackEnemy()
    {
        //Ŀ��Ϊ�գ�����Ŀ��Ϊ�Ǽ��Ϊ����������ش�������������Ҳ�����������destroy���gameobject���ܻ�����α�ǿգ�����ʱ�����������󣩣�����������������
        if (target == null || !target.gameObject.activeSelf || target.GetComponent<ZombieData>() == null || target.GetComponent<ZombieData>().isDead)
        {
            target = FindTargetEnemy();
        }
        if (target != null && target.GetComponent<ZombieData>() != null)
        {
            Vector3 direction = (target.transform.position - rotationOffset - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            //ƽ�������٣�һ������camera������
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //���٣�ÿ֡��ת���ٶȣ�һ��������̨��
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
                shootTime = Time.time + shootSpeed; // �̶����
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
        shootPointPosition = (shootPointPosition + 1) % shootPoint.Length; //�����
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
