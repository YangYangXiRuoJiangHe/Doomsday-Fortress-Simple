using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailDescribe : MonoBehaviour
{
    //��Ϊ�м��������ݣ���Ϊ��Ҫʹ����Ϸ����������ű����̳�����ű�����������˼̳���MonoBehaviour
    public TowerDescribe towerDescribe;
    public virtual void DetailDescribeShow(string name, string describe)
    {
        towerDescribe.name = name;
        towerDescribe.desctibe = describe;
    }
}
