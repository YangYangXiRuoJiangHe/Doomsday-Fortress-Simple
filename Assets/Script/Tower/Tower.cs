using UnityEngine;

public enum TowerType
{
    Tower_MachineGun_Type,
    Tower_Castle_Type,
    Tower_SolarPower_Type
}
public class Tower : MonoBehaviour
{
    [Header("��ת��Ϣ")]
    public float rotationSpeed = 1;
    public Vector3 rotationOffset;
    [Header("������Ϣ")]
    public Vector3 buildOffset;
    public TowerType towerType;
    public Transform fountionPosition;
    public LayerMask isFountion;
    public float createRadio = 2;
    [Header("��Դ��Ϣ")]
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
    public virtual void ReduceResources()
    {
        SourceManager.instance.AddSourceData("wood", -(createRequiredResource.wood));
        SourceManager.instance.AddSourceData("food", -(createRequiredResource.food));
        SourceManager.instance.AddSourceData("iron", -(createRequiredResource.iron));
        SourceManager.instance.AddSourceData("corpse", -(createRequiredResource.corpse));
        SourceManager.instance.AddSourceData("power", -(createRequiredResource.power));
        SourceManager.instance.AddSourceData("water", -(createRequiredResource.water));
    }
}
