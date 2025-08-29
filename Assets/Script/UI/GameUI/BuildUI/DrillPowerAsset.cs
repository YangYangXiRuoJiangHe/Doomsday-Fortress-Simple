using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillPowerAsset : BuildAsset
{
    private void Start()
    {
        currentRequireResource = CreateSourceManager.instance.drillBitIron;
        SetSource(currentRequireResource);
    }
}
