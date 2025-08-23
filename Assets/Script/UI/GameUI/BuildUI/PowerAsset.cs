using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerAsset : BuildAsset
{
    private void Awake()
    {
        image = GetComponent<Image>();
        orinalColor = image.color;
        button = GetComponent<Button>();
    }
    private void Start()
    {
        SetSource(CreateSourceManager.instance.scalePower.wood, CreateSourceManager.instance.scalePower.food, CreateSourceManager.instance.scalePower.iron, CreateSourceManager.instance.scalePower.corpse , CreateSourceManager.instance.scalePower.power,CreateSourceManager.instance.scalePower.water);
    }
    public override bool CanBuildTower()
    {
        return base.CanBuildTower();
    }

}
