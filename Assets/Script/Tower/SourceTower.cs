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
    public override void DismantleTower()
    {
        base.DismantleTower();
        //ֱ��ʹ�ú���������SetCanCollect��SetCanCollest�����������״̬�¹���һ��ɾ��������������ɾ��ʱ����������ڷǼ������ֱ����OffCollest();
        OffCollect();
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

    public void OnCollect()
    {
        if (IsInvoking(nameof(AddPowerSource)))
        {
            return;
        }
        InvokeRepeating(nameof(AddPowerSource), 1, collectSpeed);
    }
    public void OffCollect()
    {
        if (!IsInvoking(nameof(AddPowerSource)))
        {
            return;
        }
        CancelInvoke(nameof(AddPowerSource));
    }
    public virtual void AddPowerSource()
    {
    }
}
