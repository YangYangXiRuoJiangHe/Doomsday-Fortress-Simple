using UnityEngine;

public class SourceTower : Tower
{
    [Tooltip("几秒收集一次")]public float collectSpeed;
    [Tooltip("一次收集多少")]public int collectNumber;
    public bool canCollect;
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fountionPosition.position, createRadio);
    }
    public override void DismantleTower()
    {
        base.DismantleTower();
        //直接使用函数而不是SetCanCollect，SetCanCollest是塔还激活的状态下管理，一旦删除塔，那在塔的删除时间段中塔处于非激活，所有直接用OffCollest();
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
