using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceBuildShow_UI : MonoBehaviour
{
    public BuildAsset powerBuildUI;
    public BuildAsset drillBuildUI;
    public List<BuildAsset> buildAssets = new List<BuildAsset>();
    private void Awake()
    {
        powerBuildUI = transform.Find("PowerUI").GetComponent<BuildAsset>();
        drillBuildUI = transform.Find("DrillUI").GetComponent<BuildAsset>();
        buildAssets.Add(powerBuildUI);
        buildAssets.Add(drillBuildUI);
    }
    private void Update()
    {
        foreach(BuildAsset ui in buildAssets)
        {
            if (ui.CanBuildTower())
            {
                ui.SetCanBuild(true);
                //使用了button.interactable,当为true时使图片变灰并使点击无效
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
