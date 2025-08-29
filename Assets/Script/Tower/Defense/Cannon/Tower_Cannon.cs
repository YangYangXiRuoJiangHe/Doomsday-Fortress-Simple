using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Cannon : DefenseTower
{
    [SerializeField] private Transform CannonBase;
    [SerializeField] private Transform CannonGun;
    [SerializeField] private float explodeAreaDistince;
    private void Update()
    {
        if (canAttack)
        {
            AttackEnemy();
        }
    }
    //���Ǵ�����ͻ���ٵ���Դ
    public override void ReduceResources()
    {
        createRequiredResource = CreateSourceManager.instance.towerCannon;
        base.ReduceResources();
    }
    private void OnEnable()
    {
        canAttack = true;
    }
    protected override void AttackEnemy()
    {
        //Ŀ��Ϊ�գ�����Ŀ��Ϊ�Ǽ��Ϊ����������ش�������������Ҳ�����������destroy���gameobject���ܻ�����α�ǿգ�����ʱ�����������󣩣�����������������
        if (target == null || !target.gameObject.activeSelf || target.GetComponent<ZombieData>() == null || target.GetComponent<ZombieData>().isDead)
        {
            target = FindTargetEnemy();
        }
        if (target != null && target.GetComponent<ZombieData>() != null)
        {
            float remainingAngleBase = RotationCannonBase();
            float remainingAngleGun = RotationCannonGun();
            RotationCannonGun();
            float angle = remainingAngleBase + remainingAngleGun;
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
    private float  RotationCannonGun()
    {
        Vector3 attackDirection = (target.transform.position - rotationOffset - CannonGun.position).normalized;
        Quaternion lookRotationGun = Quaternion.LookRotation(attackDirection);
        //���Բ�ֵ�������ͣ�������
        //Vector3 rotation = Quaternion.Lerp(CannonGun.rotation, lookRotationGun, rotationSpeed * Time.deltaTime).eulerAngles;
        //�㶨���ٶȣ������е�
        Vector3 rotation = Quaternion.RotateTowards(CannonGun.rotation, lookRotationGun, rotationSpeed * Time.deltaTime).eulerAngles;
        Vector3 targetRotation =new Vector3 (rotation.x, CannonGun.eulerAngles.y, 0);
        CannonGun.rotation = Quaternion.Euler(targetRotation);
        return Quaternion.Angle(CannonGun.rotation, lookRotationGun);
    }
    private float RotationCannonBase()
    {
        Vector3 directionToEnemy = (target.transform.position - transform.position).normalized;
        directionToEnemy.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        //�������Բ�ֵ��������
        //CannonBase.rotation = Quaternion.Slerp(CannonBase.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        CannonBase.rotation = Quaternion.RotateTowards(CannonBase.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        return Quaternion.Angle(CannonBase.rotation, lookRotation);
    }
    protected override void Shoot(Transform target, IDamagable damagable)
    {
        int i = shootPointPosition % shootPoint.Length;
        if (damagable == null)
        {
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, shootPoint[i].position, shootPoint[i].rotation);
        bullet.GetComponent<Missile>().SetupProjectile(target.position, damage, bulletSpeed, explodeAreaDistince);
        shootPointPosition = (shootPointPosition + 1) % shootPoint.Length; //�����
    }
}
