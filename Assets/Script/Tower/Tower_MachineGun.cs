using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class Tower_MachineGun : DefenseTower
{
    private void Update()
    {
        if (canAttack)
        {
            AttackEnemy();
        }
    }
    public override void ReduceResources()
    {
        createRequiredResource = CreateSourceManager.instance.towerMachineGun;
        base.ReduceResources();
    }
    private void OnEnable()
    {
        canAttack = true;
    }
}
