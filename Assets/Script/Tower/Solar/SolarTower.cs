using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarTower : SourceTower
{
    public override void ReduceResources()
    {
        //�����︳ֵ������start��ֵ��ԭ�򣬲�֪����ʲôӰ�죬�ڵ�����������Ĳ�ֱ������������������ʱ���У�ԭ��ֵ����createRequireResource�����㣬��������������ָֻ������ר����������и�ֵ��
        createRequiredResource = CreateSourceManager.instance.scalePower;
        base.ReduceResources();
    }
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("power", collectNumber);
    }
}
