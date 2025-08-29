public class TechnologyAsset : BuildAsset
{

    private void Start()
    {
        currentRequireResource = CreateSourceManager.instance.technology;
        SetSource(currentRequireResource);
    }
}
