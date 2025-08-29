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
