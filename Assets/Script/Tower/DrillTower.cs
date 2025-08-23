using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTower : Tower
{
    [Tooltip("�����ռ�һ��")] public float collectSpeed;
    [Tooltip("һ���ռ�����")] public int collectNumber;
    public bool canCollect;
    public void SetCanCollect(bool can)
    {
        canCollect = can;
    }
    private void Update()
    {
        //transform.position = orinalTransform + buildOffset;
        if (canCollect && !IsInvoking(nameof(AddPowerSource)))
        {
            OnCollect();
        }
        else if (!canCollect && IsInvoking(nameof(AddPowerSource)))
        {
            OffCollect();
        }
    }
    public override void ReduceResources()
    {
        //�����︳ֵ������start��ֵ��ԭ�򣬲�֪����ʲôӰ�죬�ڵ�����������Ĳ�ֱ������������������ʱ���У�ԭ��ֵ����createRequireResource�����㣬��������������ָֻ������ר����������и�ֵ��
        createRequiredResource = CreateSourceManager.instance.drillBitIron;
        base.ReduceResources();
    }
    public void OnCollect()
    {
        InvokeRepeating(nameof(AddPowerSource), 1,collectSpeed);
    }
    public void OffCollect()
    {
        CancelInvoke(nameof(AddPowerSource));
    }
    public void AddPowerSource()
    {
        SourceManager.instance.AddSourceData("iron", collectNumber);
    }
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fountionPosition.position, createRadio);
    }
}
