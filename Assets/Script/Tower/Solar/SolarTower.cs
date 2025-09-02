using UnityEngine;

public class SolarTower : SourceTower
{
    private void Start()
    {
        if (isReduceRequireResource)
        {
            createRequiredResource = CreateSourceManager.instance.solarPower;
            //������Դ
            ReduceResources();
            //������������ʱ������ʱ��������tower�ű�
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
