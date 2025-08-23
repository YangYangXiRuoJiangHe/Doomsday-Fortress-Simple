using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainScene_SettingUI : MonoBehaviour
{
    [Header("游戏bgm与音效")]
    public Slider bgmUI;
    public TextMeshProUGUI bgmPercentage;
    public Slider sfxUI;
    public TextMeshProUGUI sfxPercentage;
    [Header("声音管理器")]
    public AudioMixer audioMixer;
    public string bgmMixerName;
    public string sfxMixerName;
    public float mixerMultiplier = 25;

    private void Awake()
    {
        bgmUI = transform.Find("BGM_Slider").GetComponent<Slider>();
        sfxUI = transform.Find("SFX_Slider").GetComponent<Slider>();
    }
    private void Start()
    {
        bgmUI.value = PlayerPrefs.GetFloat("musicui", .8f);
        bgmPercentage.text = ((int)(bgmUI.value * 100)) + "%";
        audioMixer.SetFloat(bgmMixerName, Mathf.Log10(bgmUI.value) * mixerMultiplier);
        sfxUI.value = PlayerPrefs.GetFloat("sfxui", .8f);
        sfxPercentage.text = ((int)(sfxUI.value * 100)) + "%";
        audioMixer.SetFloat(sfxMixerName, Mathf.Log10(sfxUI.value) * mixerMultiplier);
    }
    public void SaveBgm()
    {
        PlayerPrefs.SetFloat("musicui", bgmUI.value);
        bgmPercentage.text = ((int)(bgmUI.value * 100)) + "%";
        audioMixer.SetFloat(bgmMixerName, Mathf.Log10(bgmUI.value) * mixerMultiplier);
    }
    public void SaveSfx()
    {
        PlayerPrefs.SetFloat("sfxui", sfxUI.value);
        sfxPercentage.text = ((int)(sfxUI.value * 100)) + "%";
        audioMixer.SetFloat(sfxMixerName, Mathf.Log10(sfxUI.value) * mixerMultiplier);
    }
}
