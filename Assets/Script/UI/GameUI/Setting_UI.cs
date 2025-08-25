using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Setting_UI : MonoBehaviour
{
    [Header("游戏bgm与音效")]
    public Slider bgmUI;
    public TextMeshProUGUI bgmPercentage;
    public Slider sfxUI;
    public TextMeshProUGUI sfxPercentage;
    [Header("游戏人物镜头灵敏度")]
    public Slider playerSensitivityUI;
    public TextMeshProUGUI playerSensitivityPercentage;
    public CharController_Motor playerController;
    [Header("输入系统")]
    public Player player;
    private InputActions inputActions;



    private void Awake()
    {
        bgmUI = transform.Find("BGM_UI").GetComponent<Slider>();
        sfxUI = transform.Find("SFX_UI").GetComponent<Slider>();
        playerSensitivityUI = transform.Find("PlayerSensitivity_UI").GetComponent<Slider>();
    }
    private void Start()
    {
        inputActions = player.inputActions;
        bgmUI.value = PlayerPrefs.GetFloat("musicui", .8f);
        bgmPercentage.text = ((int)(bgmUI.value * 100)) + "%";
        sfxUI.value = PlayerPrefs.GetFloat("sfxui", .8f);
        sfxPercentage.text = ((int)(sfxUI.value * 100)) + "%";
        playerSensitivityUI.value = PlayerPrefs.GetFloat("playerSensitivityui", .8f);
        playerSensitivityPercentage.text = (int)((playerSensitivityUI.value - playerSensitivityUI.minValue) / (playerSensitivityUI.maxValue - playerSensitivityUI.minValue) * 100f) + "%";
    
    }
    public void SaveBgm()
    {
        PlayerPrefs.SetFloat("musicui", bgmUI.value);
        bgmPercentage.text = ((int)(bgmUI.value * 100)) + "%";
        AudioManager.instance.SetBgmSoundSize(bgmUI.value);
    }
    public void SaveSfx()
    {
        PlayerPrefs.SetFloat("sfxui", sfxUI.value);
        sfxPercentage.text = ((int)(sfxUI.value * 100)) + "%";
        AudioManager.instance.SetSfxSoundSize(sfxUI.value);
    }
    public void SavePlayerSensitivity()
    {
        PlayerPrefs.SetFloat("playerSensitivityui", playerSensitivityUI.value);
        playerSensitivityPercentage.text = (int)((playerSensitivityUI.value - playerSensitivityUI.minValue) / (playerSensitivityUI.maxValue - playerSensitivityUI.minValue) * 100f) + "%";
        playerController.SetPlayerSensitivity(playerSensitivityUI.value);
    }
    private void OnEnable()
    {
        Time.timeScale = 0;
        player.SetPlayerInputIsActive(false);
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
