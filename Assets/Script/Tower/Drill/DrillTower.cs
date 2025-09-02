using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTower : SourceTower
{
    private void Start()
    {
        if (isReduceRequireResource)
        {
            createRequiredResource = CreateSourceManager.instance.drillBitIron;
            ReduceResources();
            //启动创建倒计时，倒计时结束启用tower脚本
            CreateTower();
        }
    }
    public override void ReduceResources()
    {

        base.ReduceResources();
    }
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("iron", collectNumber);
    }
}
