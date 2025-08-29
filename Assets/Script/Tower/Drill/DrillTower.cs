using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTower : SourceTower
{
    public override void ReduceResources()
    {
        //在这里赋值而不在start赋值的原因，不知道受什么影响，在调用这个函数的并直到这个函数结束的这段时间中，原赋值过的createRequireResource会置零，这个函数结束后又恢复，因此专门在这里进行赋值。
        createRequiredResource = CreateSourceManager.instance.drillBitIron;
        base.ReduceResources();
    }
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("iron", collectNumber);
    }
}
