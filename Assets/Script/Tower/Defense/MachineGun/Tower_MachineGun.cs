using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class Tower_MachineGun : DefenseTower
{
    private void Update()
    {
        if (canAttack)
        {
            AttackEnemy();
        }
    }
    public override void ReduceResources()
    {
        createRequiredResource = CreateSourceManager.instance.towerMachineGun;
        base.ReduceResources();
    }
    private void OnEnable()
    {
        canAttack = true;
    }
    public override void DismantleTower()
    {
        //��ֹ��ǰ�Ĺ������ռ���Դ�ȶ���
        this.enabled = false;
        //��ֹ��ǰ���Ӿ�������ת֮���
        this.GetComponentInChildren<TowerVision>()?.SetEnable(false);
        //��ֹcollider��mesh�����������
        //this.gameObject.SetActive(false);
        StartCoroutine(FinishDismantleTower(createRequiredResource.dismantleBuildTime));
    }
    protected override IEnumerator FinishDismantleTower(float dismantleTimer)
    {
        yield return new WaitForSeconds(dismantleTimer);
        ReturnResources(SourceManager.instance.returnSourceMultiplier);
        SetFountionEmpty();
        Destroy(transform.parent.transform.parent.gameObject);
    }
}
