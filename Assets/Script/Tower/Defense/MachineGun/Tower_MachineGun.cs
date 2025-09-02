using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class Tower_MachineGun : DefenseTower
{
    private void Start()
    {
        if (isReduceRequireResource)
        {
            createRequiredResource = CreateSourceManager.instance.towerMachineGun;
            ReduceResources();
            //������������ʱ������ʱ��������tower�ű�
            CreateTower();
        }
    }
    private void Update()
    {
        if (canAttack)
        {
            AttackEnemy();
        }
    }
    public override void ReduceResources()
    {
        base.ReduceResources();
    }
    private void OnEnable()
    {
        canAttack = true;
    }
    public override void DismantleTower()
    {
        foreach (MeshRenderer model in CreateOrDismantleModel)
        {
            model.enabled = false;
        }
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
        countdownSprite.CreateOrDismantleVision(Color.red, dismantleTimer);
        yield return new WaitForSeconds(dismantleTimer);
        ReturnResources(SourceManager.instance.returnSourceMultiplier);
        SetFountionEmpty();
        Destroy(transform.parent.transform.parent.gameObject);
    }
}
