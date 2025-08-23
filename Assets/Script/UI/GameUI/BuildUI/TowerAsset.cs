using UnityEngine.UI;

public class TowerAsset : BuildAsset
{
    private void Awake()
    {
        image = GetComponent<Image>();
        orinalColor = image.color;
        button = GetComponent<Button>();
    }
    private void Start()
    {
        SetSource(CreateSourceManager.instance.towerMachineGun.wood, CreateSourceManager.instance.towerMachineGun.food, CreateSourceManager.instance.towerMachineGun.iron, CreateSourceManager.instance.towerMachineGun.corpse, CreateSourceManager.instance.towerMachineGun.power, CreateSourceManager.instance.towerMachineGun.water);

    }
    public override bool CanBuildTower()
    {
        return base.CanBuildTower();
    }
}
