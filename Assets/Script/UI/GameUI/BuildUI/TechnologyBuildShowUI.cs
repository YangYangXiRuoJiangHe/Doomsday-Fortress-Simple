using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyBuildShowUI : MonoBehaviour
{
    public BuildAsset TechonlogyBuildUI;
    public List<BuildAsset> buildAssets = new List<BuildAsset>();
    private void Awake()
    {
        TechonlogyBuildUI = transform.Find("TechonlogyUI").GetComponent<BuildAsset>();
        buildAssets.Add(TechonlogyBuildUI);
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
