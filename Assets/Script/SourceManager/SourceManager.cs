using System.Collections.Generic;
using UnityEngine;

public class SourceManager : MonoBehaviour
{
    public static SourceManager instance;
    //生命
    [SerializeField] private int health;
    //木材
    [SerializeField] private int wood;
    //食物
    [SerializeField] private int food;
    //金属
    [SerializeField] private int iron;
    //尸块
    [SerializeField] private int corpse;
    //电力
    [SerializeField] private int power;
    //水
    [SerializeField] private int water;
    //弹药
    [SerializeField] private int ammunition;
    //导弹
    [SerializeField] private int missile;
    //核弹头
    [SerializeField] private int nuclearWarhead;
    InGame_UI inGameUI;
    public Dictionary<string, int> sourceData;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            sourceData = new Dictionary<string, int>
            {
                 {"health",instance.health },
                 {"wood",instance.wood },
                 {"food",instance.food },
                 {"iron",instance.iron },
                 {"corpse",instance.corpse },
                 {"power",instance.power },
                 {"water",instance.water },
                 {"ammunition",instance.ammunition },
                 {"missile",instance.missile },
                 {"nuclearWarhead",instance.nuclearWarhead },
            };
        }
        else
        {
            Destroy(this);
        }

    }
    private void Start()
    {
        inGameUI = FindFirstObjectByType<InGame_UI>();
    }

    public void UpdateHp()
    {
        inGameUI.UpdateHealthUI(GetSourceData("health"));
        inGameUI.UpdateWoodUI(GetSourceData("wood"));
        inGameUI.UpdateFoodUI(GetSourceData("food"));
        inGameUI.UpdateIronUI(GetSourceData("iron"));
        inGameUI.UpdateCorpseUI(GetSourceData("corpse"));
        inGameUI.UpdatePowerUI(GetSourceData("power"));
        inGameUI.UpdateWaterUI(GetSourceData("water"));
        inGameUI.UpdateAmmunitionUI(GetSourceData("ammunition"));
        inGameUI.UpdateMissionUI(GetSourceData("missile"));
        inGameUI.UpdateNuclerWarheadUI(GetSourceData("nuclearWarhead"));
    }
    private void Update()
    {
        health = GetSourceData("health");
        wood = GetSourceData("wood");
        food = GetSourceData("food");
        iron = GetSourceData("iron");
        corpse = GetSourceData("corpse");
        power = GetSourceData("power");
        water = GetSourceData("water");
        ammunition = GetSourceData("ammunition");
        missile = GetSourceData("missile");
        nuclearWarhead = GetSourceData("nuclearWarhead");

        //测试
        UpdateHp();
    }
    public int GetSourceData(string name)
    {
        if (sourceData.ContainsKey(name))
        {
            return sourceData[name];
        }
        else
        {
            Debug.Log("没有对应的资源存在");
            return 0;
        }
    }
    public void AddSourceData(string name, int quantity)
    {
        if (sourceData.ContainsKey(name))
        {
            sourceData[name] += quantity;
        }
        else
        {
            Debug.Log("没有对应的资源存在");
        }
    }
}
