using UnityEngine;

public class SolarTower : SourceTower
{
    private void Start()
    {
        if (isReduceRequireResource)
        {
            createRequiredResource = CreateSourceManager.instance.solarPower;
            //减少资源
            ReduceResources();
            //启动创建倒计时，倒计时结束启用tower脚本
            CreateTower();
        }
    }

    public override void ReduceResources()
    {
        base.ReduceResources();
    }
    public override void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("power", collectNumber);
    }
}
