using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    private SettingData settingData;
    public Image[] frame_imgs = new Image[4];
    public Color selectedColor;

    public Image bgIcon_img;
    public Image effectIcon_img;
    public Sprite[] bgIcons = new Sprite[2];
    public Sprite[] effectIcons = new Sprite[2];
    public Slider bgVolume_slider;
    public Slider effectVolume_slider;
    public AudioSource backgroundSound_audio;
    public Text bgVolume_text;
    public Text effectVolume_text;

    public Button[] lang_buttons;

    public Button[] buttonMode_buttons;

    public Text frame_text;
    public Text maxFrame_text;
    public Text fpsDisplay_text;
    private float deltaTime = 0.0f;
    private bool showFPS = true;

    public AudioSource[] effectSounds_audio;
    public WeaponManager weaponManager;
    public Launch launch;
    public LocalizationManager localizationManager;

    private void Start()
    {
        if (showFPS)
            StartCoroutine(FPS());
        else
            enabled = false;
    }
    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    IEnumerator FPS()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        int fps = (int)(1.0f / deltaTime);
        if (fps > 9999)
            fps = 9999;

        frame_text.text = "(" + fps + " fps)";

        if (showFPS)
            StartCoroutine(FPS());
    }

    public void ShowFPS()
    {
        bool set = !showFPS;
        ShowFPS(set);
    }
    public void ShowFPS(bool set)
    {
        showFPS = set;
        if (set)
        {
            frame_text.gameObject.SetActive(true);
            fpsDisplay_text.text = LocalizationManager.instance.GetLocalizedValue("fps_display") + " On";
            enabled = true;
            StartCoroutine(FPS());
        }
        else
        {
            fpsDisplay_text.text = LocalizationManager.instance.GetLocalizedValue("fps_display") + " Off";
            enabled = false;
            frame_text.gameObject.SetActive(false);
        }
    }

    public void SetFrame(int frame)
    {
        Application.targetFrameRate = frame;
        settingData.frame = frame;
        for(int i = 0; i < frame_imgs.Length; i++)
            frame_imgs[i].color = Color.white;
        frame_imgs[frame / 30 - 1].color = selectedColor;
        maxFrame_text.text = frame.ToString();
    }

    public void SetDecideButtonMode(bool mode)
    {
        settingData.decideButtonMode = mode;
        if(mode)
        {
            buttonMode_buttons[0].image.color = Color.gray;
            buttonMode_buttons[1].image.color = Color.white;
        }
        else
        {
            buttonMode_buttons[1].image.color = Color.gray;
            buttonMode_buttons[0].image.color = Color.white;
        }
    }

    public bool GetDecideButtonMode()
    {
        return settingData.decideButtonMode;
    }

    public void SetBGSoundVolume()
    {
        settingData.bgVolume = bgVolume_slider.value;
        if (backgroundSound_audio != null)
            backgroundSound_audio.volume = settingData.bgVolume;

        if (settingData.bgVolume == 0)
            bgIcon_img.sprite = bgIcons[1];
        else
            bgIcon_img.sprite = bgIcons[0];
        bgVolume_text.text = string.Format("{0:0}%", settingData.bgVolume * 100);
    }
    public void SetEffectSoundVolume()
    {
        settingData.effectVolume = effectVolume_slider.value;
        VolumeSet();

        if (settingData.effectVolume == 0)
            effectIcon_img.sprite = effectIcons[1];
        else
            effectIcon_img.sprite = effectIcons[0];


        effectVolume_text.text = string.Format("{0:0}%", settingData.effectVolume * 100);
    }
    public float GetEffectVolume()
    {
        return settingData.effectVolume;
    }

    public void VolumeSet()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
            effectSounds_audio[0].volume = launch.rocketFireAudioVolumeRatio * settingData.effectVolume;
        for (int i = 1; i < effectSounds_audio.Length; i++)
        {
            effectSounds_audio[i].volume = settingData.effectVolume;
        }
    }

    public void SetLanguage(string fileName)
    {
        settingData.lang = fileName;
        localizationManager.ReloadTexts(fileName);
        if(fileName.Equals("kor"))
        {
            lang_buttons[0].image.color = Color.gray;
            lang_buttons[1].image.color = Color.white;
        }
        else
        {
            lang_buttons[1].image.color = Color.gray;
            lang_buttons[0].image.color = Color.white;
        }
    }

    public string GetLanguage()
    {
        if (settingData.lang == null)
        {
            if (Application.systemLanguage.Equals(SystemLanguage.Korean))
                settingData.lang = "kor";
            else
                settingData.lang = "eng";
        }

        return settingData.lang;
    }

    public void SaveData(ref Data data)
    {
        data.settingData = settingData;
    }
    public void LoadData(Data data)
    {
        if(data.settingData != null)
        {
            settingData = data.settingData;
        }
        else
        {
            settingData = new SettingData();
        }

        bgVolume_slider.value = settingData.bgVolume;
        effectVolume_slider.value = settingData.effectVolume;
        SetBGSoundVolume();
        SetEffectSoundVolume();
        SetFrame(settingData.frame);
        Screen.SetResolution(Screen.width, Screen.height, true);
        SetDecideButtonMode(settingData.decideButtonMode);
        localizationManager.LoadLocalizedText(GetLanguage());
    }
}
[System.Serializable]
public class SettingData
{
    public int frame = 60;
    public float bgVolume = 1;
    public float effectVolume = 1;
    public string lang;
    public bool decideButtonMode = true;
}
