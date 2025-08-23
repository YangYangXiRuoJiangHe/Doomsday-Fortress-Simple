using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShowUI : MonoBehaviour
{
    public BuildAsset towerBuildUI;
    public List<BuildAsset> buildAssets = new List<BuildAsset>();
    private void Awake()
    {
        towerBuildUI = transform.Find("TowerUI").GetComponent<BuildAsset>();
        buildAssets.Add(towerBuildUI);
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
