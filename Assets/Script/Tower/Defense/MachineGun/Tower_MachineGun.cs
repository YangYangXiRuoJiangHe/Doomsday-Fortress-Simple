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
        //禁止当前的攻击，收集资源等动作
        this.enabled = false;
        //禁止当前的视觉，如旋转之类的
        this.GetComponentInChildren<TowerVision>()?.SetEnable(false);
        //禁止collider，mesh，等其他组件
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
