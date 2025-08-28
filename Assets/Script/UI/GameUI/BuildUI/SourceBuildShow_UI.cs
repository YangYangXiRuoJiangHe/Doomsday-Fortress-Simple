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
                //ʹ����button.interactable,��ΪtrueʱʹͼƬ��Ҳ�ʹ�����Ч
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
