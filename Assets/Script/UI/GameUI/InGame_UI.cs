using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGame_UI : UI
{
    public List<UI_Response> uiResponse;

    private void Awake()
    {
        uiResponse = new List<UI_Response>();
        foreach(UI_Response ui in GetComponentsInChildren<UI_Response>(true))
        {
            uiResponse.Add(ui);
        }
    }
    [Header("资源UI")]
    public SourceShow_UI SourceShowUI;

    [Header("创建建筑UI")]
    public Build_UI BuildUI;
    [Header("塔的详细信息UI")]
    public Detail_UI DetailUI;
    public void OnBuildUI()
    {
        OffDetailUI();
        MouseManager.instance.ShowMouseCursor();
        BuildUI.gameObject.SetActive(true);
    }
    public void OffBuildUI()
    {
        MouseManager.instance.HideMouseCursor();
        BuildUI.gameObject.SetActive(false);
    }
    public void OnDetailUI(DetailDescribe towerDescribe,GameObject tower)
    {
        OffBuildUI();
        DetailUI.FindTower(tower);
        DetailUI.UpdateDescribeText(towerDescribe);
        DetailUI.gameObject.SetActive(true);
    }
    public void OffDetailUI()
    {
        DetailUI.gameObject.SetActive(false);
    }
    public void OnSourceShowUI() 
    {
        SourceShowUI.gameObject.SetActive(true);
    }
    public void OffSourceShowUI() 
    {
        SourceShowUI.gameObject.SetActive(false);

    }
}
