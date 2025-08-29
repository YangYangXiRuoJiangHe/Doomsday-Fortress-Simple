public class TowerMachineAsset : BuildAsset
{
    private void Start()
    {
        currentRequireResource = CreateSourceManager.instance.towerMachineGun;
        SetSource(currentRequireResource);
    }
}
