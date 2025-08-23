using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;
    public InGame_UI inGameUI;
    public Main_UI mainUI;
    public Setting_UI settingUI;
    public List<GameObject> uis = new List<GameObject>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        uis.Add(inGameUI.gameObject);
        uis.Add(mainUI.gameObject);
        uis.Add(settingUI.gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainUI.gameObject.activeSelf)
            {
                ActiveInGameUI();
            }
            else
            {
                ActiveMainUII();
            }
        }
    }

    public void ActiveInGameUI()
    {
        ClosedAllUI();
        inGameUI.gameObject.SetActive(true);
    }   
    public void ActiveMainUII()
    {
        ClosedAllUI();
        mainUI.gameObject.SetActive(true);
    }
    public void ActiveSettingUI()
    {
        ClosedAllUI();
        settingUI.gameObject.SetActive(true);
    }

    public void ClosedAllUI()
    {
        foreach(GameObject ui in uis)
        {
            ui.SetActive(false);
        }
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
