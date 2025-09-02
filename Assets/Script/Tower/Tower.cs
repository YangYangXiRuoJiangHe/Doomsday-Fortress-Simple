using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public enum TowerType
{
    Tower_MachineGun_Type,
    Tower_Castle_Type,
    Tower_SolarPower_Type,
    Tower_DrillBitIron_Type,
    Tower_Cannon_Type
}
public class Tower : MonoBehaviour
{
    [Header("��ת��Ϣ(����ʱ����ת)")]
    public float rotationSpeed = 1;
    public Vector3 rotationOffset;
    [Header("������Ϣ")]
    public Vector3 buildOffset;
    public TowerType towerType;
    public Transform fountionPosition;
    public LayerMask isFountion;
    public float createRadio = 2;
    [Header("��Դ��Ϣ")]
    //���㿴�ģ�ûʲô��
    public CreateRequiredResource createRequiredResource = new CreateRequiredResource();
    //�ý������󶨵Ĵ���UI��ֵ������
    public GameObject buildAssetUI;
    [Header("������ɾ����Ҫ���ص���ģ��")]
    public List<MeshRenderer> CreateOrDismantleModel = new List<MeshRenderer>();
    [Header("������ɾ���еĵ���ʱ�Ӿ�")]
    public Countdown countdownSprite;
    //�ڴ�����ʱ��û����ģ�ͣ�����ֱ���ô��нű���������������������ֻ�д����˲��ܼ�����
    protected bool isReduceRequireResource = false;
    public void SetIsReduce(bool isReduce) 
    {
        isReduceRequireResource = isReduce;
    }
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
    public virtual void DismantleTower()
    {
        foreach (MeshRenderer model in CreateOrDismantleModel)
        {
            model.enabled = false;
        }
        TowerActive(false);
        StartCoroutine(FinishDismantleTower(createRequiredResource.dismantleBuildTime));
    }
    public virtual void CreateTower()
    {
        TowerActive(false);
        StartCoroutine(FinishCreateBuildTower(createRequiredResource.createBuildTime));

    }
    private void TowerActive(bool active)
    {
        Debug.Log(active);
        //��ֹ��ǰ�Ĺ������ռ���Դ�ȶ���
        this.enabled = active;
        //��ֹ��ǰ���Ӿ�������ת֮���
        this.GetComponentInChildren<TowerVision>()?.SetEnable(active);
        //��ֹcollider��mesh�����������
        //this.gameObject.SetActive(active);
    }
    protected virtual IEnumerator FinishCreateBuildTower(float createBuildTimer)
    {
        countdownSprite.CreateOrDismantleVision(Color.green, createBuildTimer);
        yield return new WaitForSeconds(createBuildTimer);
        TowerActive(true);
    }

    protected virtual IEnumerator FinishDismantleTower(float dismantleTimer)
    {
        countdownSprite.CreateOrDismantleVision(Color.red, dismantleTimer);
        yield return new WaitForSeconds(dismantleTimer);
        ReturnResources(SourceManager.instance.returnSourceMultiplier);
        SetFountionEmpty();
        Destroy(this.gameObject);
    }
    public void SetFountionEmpty()
    {
        Collider[] fountions = Physics.OverlapSphere(fountionPosition.position, createRadio, isFountion);
        foreach (Collider hit in fountions)
        {
            hit.GetComponent<CreateBasicFound>().isEmpty = true;
        }
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
    public virtual bool CanBuildTower()
    {
        if (createRequiredResource.wood <= SourceManager.instance.GetSourceData("wood") && createRequiredResource.food <= SourceManager.instance.GetSourceData("food") && createRequiredResource.iron <= SourceManager.instance.GetSourceData("iron") && createRequiredResource.corpse <= SourceManager.instance.GetSourceData("corpse") && createRequiredResource.power <= SourceManager.instance.GetSourceData("power") && createRequiredResource.water <= SourceManager.instance.GetSourceData("water"))
        {
            return true;
        }
        return false;
    }
}
