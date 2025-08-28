using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailDescribe : MonoBehaviour
{
    //作为中间类来传递，因为需要使用游戏物体获得这个脚本（继承这个脚本的塔），因此继承于MonoBehaviour
    public TowerDescribe towerDescribe;
    public virtual void DetailDescribeShow(string name, string describe)
    {
        towerDescribe.name = name;
        towerDescribe.desctibe = describe;
    }
}
