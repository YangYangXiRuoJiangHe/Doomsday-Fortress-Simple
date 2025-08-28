using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSourceManager : MonoBehaviour
{
    public static CreateSourceManager instance;
    public CreateRequiredResource scalePower;
    public CreateRequiredResource towerMachineGun;
    public CreateRequiredResource drillBitIron;
    public CreateRequiredResource technology;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
    }
}
