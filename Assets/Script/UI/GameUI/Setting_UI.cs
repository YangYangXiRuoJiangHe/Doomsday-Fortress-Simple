using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Setting_UI : MonoBehaviour
{
    [Header("”Œœ∑bgm”Î“Ù–ß")]
    public Slider bgmUI;
    public TextMeshProUGUI bgmPercentage;
    public Slider sfxUI;
    public TextMeshProUGUI sfxPercentage;


    private void Awake()
    {
        bgmUI = transform.Find("BGM_UI").GetComponent<Slider>();
        sfxUI = transform.Find("SFX_UI").GetComponent<Slider>();
    }
    private void Start()
    {
        bgmUI.value = PlayerPrefs.GetFloat("musicui",.8f);
        bgmPercentage.text = ((int)(bgmUI.value * 100)) + "%";
        sfxUI.value = PlayerPrefs.GetFloat("sfxui", .8f);
        sfxPercentage.text = ((int)(sfxUI.value * 100)) + "%";
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
}
