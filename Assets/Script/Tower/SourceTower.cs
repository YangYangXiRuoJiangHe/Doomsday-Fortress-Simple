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
    public override void ReduceResources()
    {
        //在这里赋值而不在start赋值的原因，不知道受什么影响，在调用这个函数的并直到这个函数结束的这段时间中，原赋值过的createRequireResource会置零，这个函数结束后又恢复，因此专门在这里进行赋值。
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
