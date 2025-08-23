using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_UI : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject settingUI;
    public GameObject quitUI;

    private void Awake()
    {
        pauseUI = transform.Find("Pause_UI").gameObject;
        pauseUI = transform.Find("Setting_UI").gameObject;
        pauseUI = transform.Find("Quit_UI").gameObject;
    }
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

}
