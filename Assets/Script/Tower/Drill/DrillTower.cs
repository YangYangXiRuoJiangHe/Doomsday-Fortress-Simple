using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTower : SourceTower
{
    public override void ReduceResources()
    {
        //�����︳ֵ������start��ֵ��ԭ�򣬲�֪����ʲôӰ�죬�ڵ�����������Ĳ�ֱ������������������ʱ���У�ԭ��ֵ����createRequireResource�����㣬��������������ָֻ������ר����������и�ֵ��
        createRequiredResource = CreateSourceManager.instance.drillBitIron;
        base.ReduceResources();
    }
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("iron", collectNumber);
    }
}
