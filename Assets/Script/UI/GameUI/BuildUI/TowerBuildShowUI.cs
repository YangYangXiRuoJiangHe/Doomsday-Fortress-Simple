using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuildShowUI : MonoBehaviour
{
    [SerializeField] private List<BuildAsset> buildAssets = new List<BuildAsset>();
    private void Awake()
    {
        foreach (BuildAsset buildAsset in GetComponentsInChildren<BuildAsset>())
        {
            buildAssets.Add(buildAsset);
        }
    }
    private void Update()
    {
        foreach (BuildAsset ui in buildAssets)
        {
            if (ui.CanBuildTower())
            {
                ui.SetCanBuild(true);
                //ui.SetColorOrinal();
            }
            else
            {
                ui.SetCanBuild(false);
                //ui.SetColorGray();
            }
        }
    }
}
