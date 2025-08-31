using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct CreateRequiredResource
{
    //木材
    public int wood;
    //食物
    public int food;
    //金属
    public int iron;
    //尸块
    public int corpse;
    //电力
    public int power;
    //水
    public int water;
    //创建所需时间
    public float createBuildTime;
    //拆除所需时间
    public float dismantleBuildTime;
}
