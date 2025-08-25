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
    private InputActions inputActions;

    private void Awake()
    {
        pauseUI = transform.Find("Pause_UI").gameObject;
        pauseUI = transform.Find("Setting_UI").gameObject;
        pauseUI = transform.Find("Quit_UI").gameObject;
    }
    private void Start()
    {
        inputActions = player.inputActions;
    }
    private void OnEnable()
    {
        Time.timeScale = 0;
        player.SetPlayerInputIsActive(false);

    }
    private void OnDisable()
    {
        Time.timeScale = 1;
        player.SetPlayerInputIsActive(true) ;

    }
    public void PauseGame()
    {
        Time.timeScale = 1;
        player.SetPlayerInputIsActive(true);
        gameObject.SetActive(false);
    }

}
