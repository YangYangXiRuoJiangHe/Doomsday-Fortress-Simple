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
            //启动创建倒计时，倒计时结束启用tower脚本
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
        countdownSprite.CreateOrDismantleVision(Color.red, dismantleTimer);
        yield return new WaitForSeconds(dismantleTimer);
        ReturnResources(SourceManager.instance.returnSourceMultiplier);
        SetFountionEmpty();
        Destroy(transform.parent.transform.parent.gameObject);
    }
}
