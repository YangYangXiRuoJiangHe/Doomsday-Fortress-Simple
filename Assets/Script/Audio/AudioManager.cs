using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource[] bgms;
    public bool playBGM;
    public int currentMusicIndex;
    [Header("声音管理器")]
    public AudioMixer audioMixer;
    public string bgmMixerName;
    public string sfxMixerName;
    public float mixerMultiplier = 25;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        SetBgmSoundSize(PlayerPrefs.GetFloat("musicui", .8f));
        SetSfxSoundSize(PlayerPrefs.GetFloat("sfxui", .8f));
        InvokeRepeating(nameof(PlayerBgmIfNeeded), 0, 2);
    }
    private void Update()
    {
        audioMixer.GetFloat(bgmMixerName,out float value);
    }
    public void SetBgmSoundSize(float bgmValue)
    {
        audioMixer.SetFloat(bgmMixerName, Mathf.Log10(bgmValue) * mixerMultiplier);
    }
    public void SetSfxSoundSize(float sfxValue)
    {
        audioMixer.SetFloat(sfxMixerName, Mathf.Log10(sfxValue) * mixerMultiplier);
    }


    public void PlayerBgmIfNeeded()
    {
        if (bgms[currentMusicIndex].isPlaying) return;
        RandomPlayBGM();
    }
    [ContextMenu("RandomPlayBGM")]
    private void RandomPlayBGM()
    {
        int index = Random.Range(0, bgms.Length);
        int i = 0;
        while(currentMusicIndex == index)
        {
            index = Random.Range(0, bgms.Length);
            i++;
            if (i > 10) return;
        }
        PlayerBgm(index);
    }

    public void PlayerBgm(int index)
    {
        if (!playBGM)
        {
            return;
        }
        if (index >= bgms.Length)
        {
            Debug.Log("此索引超出了BGM的范围索引");
            return;
        }
        StopAllBGM();
        bgms[index].Play();
        currentMusicIndex = index;
    }
    [ContextMenu("StopAllBGM")]
    private void StopAllBGM()
    {
        for (int i = 0; i < bgms.Length; i++)
        {
            bgms[i].Stop();
        }
    }
    public void PlaySFX(AudioSource sfx)
    {
        if(sfx.clip == null)
        {
            Debug.Log("没有" + sfx.gameObject.name + "对应的音频，请检查该物体是否具有音效");
            return;
        }
        if (sfx.isPlaying)
        {
            return;
        }
        sfx.Play();
    }

}
