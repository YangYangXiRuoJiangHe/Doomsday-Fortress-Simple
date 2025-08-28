using UnityEngine;

public class SourceTower : Tower
{
    [Tooltip("�����ռ�һ��")]public float collectSpeed;
    [Tooltip("һ���ռ�����")]public int collectNumber;
    public bool canCollect;
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fountionPosition.position, createRadio);
    }
    public void SetCanCollect(bool can)
    {
        canCollect = can;
    }
    public void Update()
    {
        if (canCollect && !IsInvoking(nameof(AddPowerSource)))
        {
            OnCollect();
        }
        else if(!canCollect && IsInvoking(nameof(AddPowerSource)))
        {
            OffCollect();
        }
    }
    public override void ReduceResources()
    {
        //�����︳ֵ������start��ֵ��ԭ�򣬲�֪����ʲôӰ�죬�ڵ�����������Ĳ�ֱ������������������ʱ���У�ԭ��ֵ����createRequireResource�����㣬��������������ָֻ������ר����������и�ֵ��
        createRequiredResource = CreateSourceManager.instance.scalePower;
        base.ReduceResources();
    }
    public void OnCollect()
    {
        InvokeRepeating(nameof(AddPowerSource), 1, collectSpeed);
    }
    public void OffCollect()
    {
        CancelInvoke(nameof(AddPowerSource));
    }
    public virtual void AddPowerSource()
    {
    }
}
