using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ���� Ŭ����
/// </summary>
public class Settings : MonoBehaviour
{
    /// <summary>
    /// ���� ������
    /// </summary>
    private SettingData settingData;
    /// <summary>
    /// ������ FPS ���� ��ư �̹���
    /// </summary>
    public Image[] frame_imgs = new Image[4];
    /// <summary>
    /// ������ �ɼ��� ��Ÿ���� ����
    /// </summary>
    public Color selectedColor;

    /// <summary>
    /// ������� ������ �̹���
    /// </summary>
    public Image bgIcon_img;
    /// <summary>
    /// ȿ���� ������ �̹���
    /// </summary>
    public Image effectIcon_img;
    /// <summary>
    /// ������� ������ ��������Ʈ 2�� (���Ұ�/���)
    /// </summary>
    public Sprite[] bgIcons = new Sprite[2];
    /// <summary>
    /// ȿ���� ������ ��������Ʈ 2�� (���Ұ�/���)
    /// </summary>
    public Sprite[] effectIcons = new Sprite[2];
    /// <summary>
    /// ������� ���� ���� �����̴�
    /// </summary>
    public Slider bgVolume_slider;
    /// <summary>
    /// ȿ���� ���� ���� �����̴�
    /// </summary>
    public Slider effectVolume_slider;
    /// <summary>
    /// ������� ����� �ҽ�
    /// </summary>
    public AudioSource backgroundSound_audio;
    /// <summary>
    /// ������� ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text bgVolume_text;
    /// <summary>
    /// ȿ���� ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text effectVolume_text;

    /// <summary>
    /// ��� ���� ��ư
    /// </summary>
    public Button[] lang_buttons;

    /// <summary>
    /// ��ư ���� ���� ��ư
    /// </summary>
    public Button[] buttonMode_buttons;

    /// <summary>
    /// �ǽð� ������ ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text frame_text;
    /// <summary>
    /// ������ �ִ� ������ ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text maxFrame_text;
    /// <summary>
    /// FPS ǥ�� ���� �ؽ�Ʈ
    /// </summary>
    public Text fpsDisplay_text;
    /// <summary>
    /// 1 �����ӵ��� �帥 �ð�
    /// </summary>
    private float deltaTime = 0.0f;
    /// <summary>
    /// ������ ǥ�� ����
    /// </summary>
    private bool showFPS = true;

    /// <summary>
    /// ȿ���� ����� �ҽ� ����
    /// </summary>
    public AudioSource[] effectSounds_audio;

    public WeaponManager weaponManager;
    public Launch launch;
    public LocalizationManager localizationManager;

    private void Start()
    {
        // FPS ǥ�� ���ο� ���� ǥ�� ����
        if (showFPS)
            StartCoroutine(FPS());
        else
            enabled = false;
    }
    private void Update()
    {
        // ������ ���� �ð� ���
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    /// <summary>
    /// 0.5�ʸ��� FPS�� ǥ�����ִ� �ڷ�ƾ
    /// </summary>
    IEnumerator FPS()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // FPS ���
        int fps = (int)(1.0f / deltaTime);
        if (fps > 9999)
            fps = 9999;
        // FPS ǥ��
        frame_text.text = "(" + fps + " fps)";

        if (showFPS)
            StartCoroutine(FPS());
    }

    /// <summary>
    /// FPS ǥ�� ���� ���� ��ư ������
    /// </summary>
    public void ShowFPS()
    {
        bool set = !showFPS;
        ShowFPS(set);
    }
    /// <summary>
    /// FPS ǥ�� ���ο� ���� ���� UI�� ������Ʈ
    /// </summary>
    /// <param name="set"></param>
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

    /// <summary>
    /// �ִ� FPS ����
    /// </summary>
    /// <param name="frame">�ִ� FPS</param>
    public void SetFrame(int frame)
    {
        Application.targetFrameRate = frame;
        settingData.frame = frame;
        for(int i = 0; i < frame_imgs.Length; i++)
            frame_imgs[i].color = Color.white;
        frame_imgs[frame / 30 - 1].color = selectedColor;
        maxFrame_text.text = frame.ToString();
    }

    /// <summary>
    /// ��ư Ŭ�� ��� ���� ��ư ������
    /// </summary>
    /// <param name="mode">��ư ���</param>
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

    /// <summary>
    /// ��ư ��� ��ȯ
    /// </summary>
    /// <returns>��ư ���</returns>
    public bool GetDecideButtonMode()
    {
        return settingData.decideButtonMode;
    }

    /// <summary>
    /// ������� ���� ���� (OnChangeListener)
    /// </summary>
    public void SetBGSoundVolume()
    {
        // �����̴� ���� ���� ���� ����
        settingData.bgVolume = bgVolume_slider.value;
        if (backgroundSound_audio != null)
            backgroundSound_audio.volume = settingData.bgVolume;

        // 0�� �� ���Ұ� ������ Ȱ��ȭ
        if (settingData.bgVolume == 0)
            bgIcon_img.sprite = bgIcons[1];
        else
            bgIcon_img.sprite = bgIcons[0];
        bgVolume_text.text = string.Format("{0:0}%", settingData.bgVolume * 100);
    }
    /// <summary>
    ///  ȿ���� ���� ���� (OnChangeListener)
    /// </summary>
    public void SetEffectSoundVolume()
    {
        // �����̴� ���� ���� ���� ����
        settingData.effectVolume = effectVolume_slider.value;
        VolumeSet();

        // 0�� �� ���Ұ� ������ Ȱ��ȭ
        if (settingData.effectVolume == 0)
            effectIcon_img.sprite = effectIcons[1];
        else
            effectIcon_img.sprite = effectIcons[0];


        effectVolume_text.text = string.Format("{0:0}%", settingData.effectVolume * 100);
    }
    /// <summary>
    /// ȿ���� ���� ��ȯ
    /// </summary>
    /// <returns>ȿ���� ����</returns>
    public float GetEffectVolume()
    {
        return settingData.effectVolume;
    }

    /// <summary>
    /// ȿ���� ���� ����
    /// </summary>
    public void VolumeSet()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
            effectSounds_audio[0].volume = launch.rocketFireAudioVolumeRatio * settingData.effectVolume;
        for (int i = 1; i < effectSounds_audio.Length; i++)
        {
            effectSounds_audio[i].volume = settingData.effectVolume;
        }
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    /// <param name="fileName">��� ���� �̸�</param>
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

    /// <summary>
    /// ���� ������ ��� ���� ������
    /// </summary>
    /// <returns>������ ���</returns>
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

    /// <summary>
    /// ���� ������ ����
    /// </summary>
    /// <param name="data">����� ������</param>
    public void SaveData(ref Data data)
    {
        data.settingData = settingData;
    }
    /// <summary>
    /// ���� ������ �ҷ�����
    /// </summary>
    /// <param name="data">�ҷ��� ������</param>
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

        // �ҷ��� �����Ϳ� �°� UI ������Ʈ �� ���� �� ����
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
/// <summary>
/// ���� ������
/// </summary>
[System.Serializable]
public class SettingData
{
    /// <summary>
    /// �ִ� ������
    /// </summary>
    public int frame = 60;
    /// <summary>
    /// ������� ���� ����
    /// </summary>
    public float bgVolume = 1;
    /// <summary>
    /// ȿ���� ���� ����
    /// </summary>
    public float effectVolume = 1;
    /// <summary>
    /// ������ ���
    /// </summary>
    public string lang;
    /// <summary>
    /// ��ư ���
    /// </summary>
    public bool decideButtonMode = true;
}
