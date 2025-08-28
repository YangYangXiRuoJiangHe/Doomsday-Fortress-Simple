using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_UI : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject settingUI;
    public GameObject quitUI;
    [Header(" ‰»ÎœµÕ≥")]
    public Player player;
    public GodViewMove godViewMove;

    private void Awake()
    {
        pauseUI = transform.Find("Pause_UI").gameObject;
        pauseUI = transform.Find("Setting_UI").gameObject;
        pauseUI = transform.Find("Quit_UI").gameObject;
    }
    private void OnEnable()
    {
        Time.timeScale = 0;
        player.SetPlayerInputIsActive(false);
        godViewMove.SetCameraInputIsActive(false);

    }
    private void OnDisable()
    {
        Time.timeScale = 1;
        player.SetPlayerInputIsActive(true) ;
        godViewMove.SetCameraInputIsActive(true);
    }
    public void PauseGame()
    {
        Time.timeScale = 1;
        player.SetPlayerInputIsActive(true);
        godViewMove.SetCameraInputIsActive(true);
        gameObject.SetActive(false);
    }

}
