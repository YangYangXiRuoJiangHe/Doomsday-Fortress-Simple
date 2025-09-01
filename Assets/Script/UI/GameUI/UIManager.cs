using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("UI")]
    public InGame_UI inGameUI;
    public Pause_UI pauseUI;
    public Setting_UI settingUI;
    public Main_UI mainUI;
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
        uis.Add(pauseUI.gameObject);
        uis.Add(settingUI.gameObject);
        uis.Add(mainUI.gameObject);
    }
    private void Start()
    {
        if (Scene_Manage.isStartGame)
        {
            ActiveInGameUI();
        }
        else
        {
            ActiveMainUI();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Scene_Manage.isStartGame)
        {
            if (pauseUI.gameObject.activeSelf)
            {
                ActiveInGameUI();
            }
            else
            {
                ActivePauseUI();
            }
        }
    }
    public void ActiveInGameUI()
    {
        ClosedAllUI();
        MouseManager.instance.HideMouseCursor();
        inGameUI.gameObject.SetActive(true);
        inGameUI.OffBuildUI();
    }
    public void ActivePauseUI()
    {
        ClosedAllUI();
        pauseUI.gameObject.SetActive(true);
    }
    public void ActiveSettingUI(UI returnUI)
    {
        ClosedAllUI();
        settingUI.SetActiveAndReturn(true, returnUI);
    }
    public void ActiveMainUI()
    {
        Scene_Manage.isStartGame = false;
        ClosedAllUI();
        mainUI.gameObject.SetActive(true);
    }
    public void ActiveUI(UI activeUI)
    {
        ClosedAllUI();
        activeUI.gameObject.SetActive(true);
    }
    public void ClosedAllUI()
    {
        MouseManager.instance.ShowMouseCursor();
        foreach (GameObject ui in uis)
        {
            ui.SetActive(false);
        }
    }
    public void NewGame()
    {
        Scene_Manage.isStartGame = true;
        Scene_Manage.instance.LoadScene(0);
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
