using UnityEngine;

public class TowerCannonAsset : BuildAsset
{
    private void Start()
    {
        currentRequireResource = CreateSourceManager.instance.towerCannon;

        SetSource(currentRequireResource);
    }
}
