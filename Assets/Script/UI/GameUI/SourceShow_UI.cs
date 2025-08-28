using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SourceShow_UI : UI_Response
{
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
