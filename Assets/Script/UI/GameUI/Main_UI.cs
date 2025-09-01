using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_UI : UI
{
    [Header("����ϵͳ")]
    public Player player;
    //���ؼ���UI
    private UI returnUI;
    private void OnEnable()
    {
        Time.timeScale = 0;
        player.SetPlayerInputIsActive(false);
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void SetActiveAndReturn(bool actived, UI returnUI)
    {
        this.gameObject.SetActive(actived);
        this.returnUI = returnUI;
    }
    public void ReturnUI()
    {
        UIManager.instance.ActiveUI(returnUI);
    }
}
