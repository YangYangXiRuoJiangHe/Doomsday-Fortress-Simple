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
            //������������ʱ������ʱ��������tower�ű�
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
