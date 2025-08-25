using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGame_UI : MonoBehaviour
{
    [Header("资源UI")]
    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI woodValue;
    public TextMeshProUGUI foodValue;
    public TextMeshProUGUI ironValue;
    public TextMeshProUGUI corpseValue;
    public TextMeshProUGUI powerValue;
    public TextMeshProUGUI waterValue;
    public TextMeshProUGUI ammunitionValue;
    public TextMeshProUGUI missileValue;
    public TextMeshProUGUI nuclearWarheadValue;
    [Header("创建建筑UI")]
    public GameObject BuildUI;

    public void OnBuildUI()
    {
        MouseManager.instance.ShowMouseCursor();
        BuildUI.SetActive(true);
    }
    public void OffBuildUI()
    {
        MouseManager.instance.HideMouseCursor();
        BuildUI.SetActive(false);
    }


    public void UpdateHealthUI(int value)
    {
        healthValue.text = "" + value;
    }
    public void UpdateWoodUI(int value)
    {
        woodValue.text = "" + value;
    }
    public void UpdateFoodUI(int value)
    {
        foodValue.text = "" + value;
    }
    public void UpdateIronUI(int value)
    {
        ironValue.text = "" + value;
    }
    public void UpdateCorpseUI(int value)
    {
        corpseValue.text = "" + value;
    }
    public void UpdatePowerUI(int value)
    {
        powerValue.text = "" + value;
    }
    public void UpdateWaterUI(int value)
    {
        waterValue.text = "" + value;
    }
    public void UpdateAmmunitionUI(int value)
    {
        ammunitionValue.text = "" + value;
    }
    public void UpdateMissionUI(int value)
    {
        missileValue.text = "" + value;
    }
    public void UpdateNuclerWarheadUI(int value)
    {
        nuclearWarheadValue.text = "" + value;
    }
}
