using UnityEngine;

public enum TowerType
{
    Tower_MachineGun_Type,
    Tower_Castle_Type,
    Tower_SolarPower_Type,
    Tower_DrillBitIron_Type
}
public class Tower : MonoBehaviour
{
    [Header("旋转信息(攻击时的旋转)")]
    public float rotationSpeed = 1;
    public Vector3 rotationOffset;
    [Header("创建信息")]
    public Vector3 buildOffset;
    public TowerType towerType;
    public Transform fountionPosition;
    public LayerMask isFountion;
    public float createRadio = 2;
    [Header("资源信息")]
    public CreateRequiredResource createRequiredResource;
    public Vector3 GetBuildOffset()
    {
        return buildOffset;
    }
    public bool EnableBuilding()
    {
        Collider[] fountions = Physics.OverlapSphere(fountionPosition.position, createRadio, isFountion);
        foreach(Collider hit in fountions)
        {
            if (!hit.GetComponent<CreateBasicFound>().isEmpty)
            {
                return false;
            }
        }
        return true;
    }
    public void SetFountionNotEmpty()
    {
        Collider[] fountions = Physics.OverlapSphere(fountionPosition.position, createRadio, isFountion);
        foreach (Collider hit in fountions)
        {
            hit.GetComponent<CreateBasicFound>().isEmpty = false;
        }
    }
    public void SetFountionEmpty()
    {
        Collider[] fountions = Physics.OverlapSphere(fountionPosition.position, createRadio, isFountion);
        foreach (Collider hit in fountions)
        {
            hit.GetComponent<CreateBasicFound>().isEmpty = true;
        }
    }
    public virtual void ReduceResources()
    {
        SourceManager.instance.AddSourceData("wood", -(createRequiredResource.wood));
        SourceManager.instance.AddSourceData("food", -(createRequiredResource.food));
        SourceManager.instance.AddSourceData("iron", -(createRequiredResource.iron));
        SourceManager.instance.AddSourceData("corpse", -(createRequiredResource.corpse));
        SourceManager.instance.AddSourceData("power", -(createRequiredResource.power));
        SourceManager.instance.AddSourceData("water", -(createRequiredResource.water));
    }
    public virtual void ReturnResources(float multipilier)
    {
        SourceManager.instance.AddSourceData("wood", +(int)(createRequiredResource.wood * multipilier));
        SourceManager.instance.AddSourceData("food", +(int)(createRequiredResource.food * multipilier));
        SourceManager.instance.AddSourceData("iron", +(int)(createRequiredResource.iron * multipilier));
        SourceManager.instance.AddSourceData("corpse", +(int)(createRequiredResource.corpse * multipilier));
        SourceManager.instance.AddSourceData("power", +(int)(createRequiredResource.power * multipilier));
        SourceManager.instance.AddSourceData("water", +(int)(createRequiredResource.water * multipilier));
    }
}
