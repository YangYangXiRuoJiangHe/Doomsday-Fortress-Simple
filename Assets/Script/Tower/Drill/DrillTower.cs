using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTower : SourceTower
{
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("iron", collectNumber);
    }
}
