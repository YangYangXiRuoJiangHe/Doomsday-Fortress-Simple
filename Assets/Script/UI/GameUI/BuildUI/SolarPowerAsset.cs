public class SolarPowerAsset : BuildAsset
{
    private void Start()
    {
        currentRequireResource = CreateSourceManager.instance.scalePower;

        SetSource(currentRequireResource);
    }
}
