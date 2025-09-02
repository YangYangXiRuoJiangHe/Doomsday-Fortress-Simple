public class SolarPowerAsset : BuildAsset
{
    private void Start()
    {
        currentRequireResource = CreateSourceManager.instance.solarPower;
        SetSource(currentRequireResource);
    }
}
