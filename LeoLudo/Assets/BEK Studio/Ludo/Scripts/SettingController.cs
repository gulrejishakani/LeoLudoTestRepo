using BEKStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public Toggle sfxToggle;
    public Toggle muiscToggle;

    void Start()
    {
        Invoke(nameof(InitSfxToggle), 0.5f);
        Invoke(nameof(InitMusicToggle), 0.3f);
    }

    private void InitSfxToggle()
    {
        sfxToggle.isOn = AudioController.Instance.isSFXEnable;
        sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
    }

    private void InitMusicToggle()
    {
        muiscToggle.isOn = AudioController.Instance.isMusicEnable;
        muiscToggle.onValueChanged.AddListener(OnMusicToggleChanged);
    }

    private void OnMusicToggleChanged(bool arg0)
    {
        AudioController.Instance.SetMusicEnable(arg0);
        //Debug.Log("Music Toggle Changed : " + arg0);
        AudioController.Instance.PlayBGMusic();
    }

    void OnSfxToggleChanged(bool isOn)
    {
        AudioController.Instance.PlayButtonSound();
        AudioController.Instance.SetSFXEnable(isOn);
    }

}
