using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarTower : SourceTower
{
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("power", collectNumber);
    }
}
