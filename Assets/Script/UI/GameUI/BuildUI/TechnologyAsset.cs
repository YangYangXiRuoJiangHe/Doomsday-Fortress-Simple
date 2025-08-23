using UnityEngine.UI;

public class TechnologyAsset : BuildAsset
{
    private void Awake()
    {
        image = GetComponent<Image>();
        orinalColor = image.color;
        button = GetComponent<Button>();
    }
    private void Start()
    {
            SetSource(CreateSourceManager.instance.technology.wood, CreateSourceManager.instance.technology.food, CreateSourceManager.instance.technology.iron, CreateSourceManager.instance.technology.corpse, CreateSourceManager.instance.technology.power, CreateSourceManager.instance.technology.water);

    }
    public override bool CanBuildTower()
    {
        return base.CanBuildTower();
    }
}
