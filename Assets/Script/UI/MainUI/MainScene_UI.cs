using System.Collections.Generic;
using UnityEngine;

public class MainScene_UI : MonoBehaviour
{
    public MainScene_MainUI mainUI;
    public MainScene_SettingUI settingUI;
    public List<GameObject> uis = new List<GameObject>();

    private void Awake()
    {
        uis.Add(mainUI.gameObject);
        uis.Add(settingUI.gameObject);
    }
    private void Start()
    {
        ActiveMainUII();
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
        foreach (GameObject ui in uis)
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
