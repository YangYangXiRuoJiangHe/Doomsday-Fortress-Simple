using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    public GameObject[] BuildingUIs;
    public void OnSourceShowUI()
    {
        OffAllBuildingUI();
        BuildingUIs[0].SetActive(true);
    }
    public void OnTowerShowUI()
    {
        OffAllBuildingUI();
        BuildingUIs[1].SetActive(true);
    }
    public void OnTechnologyShowUI()
    {
        OffAllBuildingUI();
        BuildingUIs[2].SetActive(true);
    }
    public void OffAllBuildingUI()
    {
        foreach(GameObject buildingUi in BuildingUIs)
        {
            buildingUi.SetActive(false);
        }
    }
    public GameObject GetBuildingUIs(int i)
    {
        if (i < BuildingUIs.Length)
        {
            return BuildingUIs[i];
        }
        else
        {
            return null;
        }
    }
}
