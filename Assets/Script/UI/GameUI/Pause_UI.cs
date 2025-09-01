using UnityEngine;

public class Pause_UI : UI
{
    public GameObject pauseUI;
    public GameObject settingUI;
    public GameObject quitUI;
    [Header("����ϵͳ")]
    public Player player;
    public GodViewMove godViewMove;
    private void OnEnable()
    {
        Time.timeScale = 0;
        if (player.gameObject.activeSelf)
        {
            player.SetPlayerInputIsActive(false);
        }
        if (godViewMove.gameObject.activeSelf)
        {
            godViewMove.SetCameraInputIsActive(false);
        }
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
        //�ڽ�����Ϸʱ����player��godviewMove��Pause_UI�����٣���׼���޸Ľű�ִ��˳��
        if (player != null? player.gameObject.activeSelf : false)
        {
            player.SetPlayerInputIsActive(true);
        }
        if (godViewMove != null ? godViewMove.gameObject.activeSelf : false)
        {
            godViewMove.SetCameraInputIsActive(true);
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

}
