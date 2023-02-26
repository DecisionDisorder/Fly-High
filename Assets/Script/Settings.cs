using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 설정 관리 클래스
/// </summary>
public class Settings : MonoBehaviour
{
    /// <summary>
    /// 설정 데이터
    /// </summary>
    private SettingData settingData;
    /// <summary>
    /// 선택한 FPS 제한 버튼 이미지
    /// </summary>
    public Image[] frame_imgs = new Image[4];
    /// <summary>
    /// 선택한 옵션을 나타내는 색상
    /// </summary>
    public Color selectedColor;

    /// <summary>
    /// 배경음악 아이콘 이미지
    /// </summary>
    public Image bgIcon_img;
    /// <summary>
    /// 효과음 아이콘 이미지
    /// </summary>
    public Image effectIcon_img;
    /// <summary>
    /// 배경음악 아이콘 스프라이트 2종 (음소거/평시)
    /// </summary>
    public Sprite[] bgIcons = new Sprite[2];
    /// <summary>
    /// 효과음 아이콘 스프라이트 2종 (음소거/평시)
    /// </summary>
    public Sprite[] effectIcons = new Sprite[2];
    /// <summary>
    /// 배경음악 볼륨 조절 슬라이더
    /// </summary>
    public Slider bgVolume_slider;
    /// <summary>
    /// 효과음 볼륨 조절 슬라이더
    /// </summary>
    public Slider effectVolume_slider;
    /// <summary>
    /// 배경음악 오디오 소스
    /// </summary>
    public AudioSource backgroundSound_audio;
    /// <summary>
    /// 배경음악 볼륨 표시 텍스트
    /// </summary>
    public Text bgVolume_text;
    /// <summary>
    /// 효과음 볼륨 표시 텍스트
    /// </summary>
    public Text effectVolume_text;

    /// <summary>
    /// 언어 설정 버튼
    /// </summary>
    public Button[] lang_buttons;

    /// <summary>
    /// 버튼 반응 설정 버튼
    /// </summary>
    public Button[] buttonMode_buttons;

    /// <summary>
    /// 실시간 프레임 표시 텍스트
    /// </summary>
    public Text frame_text;
    /// <summary>
    /// 설정한 최대 프레임 표시 텍스트
    /// </summary>
    public Text maxFrame_text;
    /// <summary>
    /// FPS 표시 여부 텍스트
    /// </summary>
    public Text fpsDisplay_text;
    /// <summary>
    /// 1 프레임동안 흐른 시간
    /// </summary>
    private float deltaTime = 0.0f;
    /// <summary>
    /// 프레임 표시 여부
    /// </summary>
    private bool showFPS = true;

    /// <summary>
    /// 효과음 오디오 소스 모음
    /// </summary>
    public AudioSource[] effectSounds_audio;

    public WeaponManager weaponManager;
    public Launch launch;
    public LocalizationManager localizationManager;

    private void Start()
    {
        // FPS 표시 여부에 따라 표시 시작
        if (showFPS)
            StartCoroutine(FPS());
        else
            enabled = false;
    }
    private void Update()
    {
        // 프레임 간격 시간 계산
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    /// <summary>
    /// 0.5초마다 FPS를 표기해주는 코루틴
    /// </summary>
    IEnumerator FPS()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // FPS 계산
        int fps = (int)(1.0f / deltaTime);
        if (fps > 9999)
            fps = 9999;
        // FPS 표기
        frame_text.text = "(" + fps + " fps)";

        if (showFPS)
            StartCoroutine(FPS());
    }

    /// <summary>
    /// FPS 표기 여부 설정 버튼 리스너
    /// </summary>
    public void ShowFPS()
    {
        bool set = !showFPS;
        ShowFPS(set);
    }
    /// <summary>
    /// FPS 표기 여부에 따라 설정 UI를 업데이트
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
    /// 최대 FPS 설정
    /// </summary>
    /// <param name="frame">최대 FPS</param>
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
    /// 버튼 클릭 모드 설정 버튼 리스너
    /// </summary>
    /// <param name="mode">버튼 모드</param>
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
    /// 버튼 모드 반환
    /// </summary>
    /// <returns>버튼 모드</returns>
    public bool GetDecideButtonMode()
    {
        return settingData.decideButtonMode;
    }

    /// <summary>
    /// 배경음악 볼륨 조절 (OnChangeListener)
    /// </summary>
    public void SetBGSoundVolume()
    {
        // 슬라이더 값에 따라 볼륨 조절
        settingData.bgVolume = bgVolume_slider.value;
        if (backgroundSound_audio != null)
            backgroundSound_audio.volume = settingData.bgVolume;

        // 0일 때 음소거 아이콘 활성화
        if (settingData.bgVolume == 0)
            bgIcon_img.sprite = bgIcons[1];
        else
            bgIcon_img.sprite = bgIcons[0];
        bgVolume_text.text = string.Format("{0:0}%", settingData.bgVolume * 100);
    }
    /// <summary>
    ///  효과음 볼륨 조절 (OnChangeListener)
    /// </summary>
    public void SetEffectSoundVolume()
    {
        // 슬라이더 값에 따라 볼륨 조절
        settingData.effectVolume = effectVolume_slider.value;
        VolumeSet();

        // 0일 때 음소거 아이콘 활성화
        if (settingData.effectVolume == 0)
            effectIcon_img.sprite = effectIcons[1];
        else
            effectIcon_img.sprite = effectIcons[0];


        effectVolume_text.text = string.Format("{0:0}%", settingData.effectVolume * 100);
    }
    /// <summary>
    /// 효과음 볼륨 반환
    /// </summary>
    /// <returns>효과음 볼륨</returns>
    public float GetEffectVolume()
    {
        return settingData.effectVolume;
    }

    /// <summary>
    /// 효과음 볼륨 적용
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
    /// 언어 설정
    /// </summary>
    /// <param name="fileName">언어 파일 이름</param>
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
    /// 현재 설정된 언어 종류 얻어오기
    /// </summary>
    /// <returns>설정된 언어</returns>
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
    /// 설정 데이터 저장
    /// </summary>
    /// <param name="data">저장될 데이터</param>
    public void SaveData(ref Data data)
    {
        data.settingData = settingData;
    }
    /// <summary>
    /// 설정 데이터 불러오기
    /// </summary>
    /// <param name="data">불러온 데이터</param>
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

        // 불러온 데이터에 맞게 UI 업데이트 및 설정 값 적용
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
/// 설정 데이터
/// </summary>
[System.Serializable]
public class SettingData
{
    /// <summary>
    /// 최대 프레임
    /// </summary>
    public int frame = 60;
    /// <summary>
    /// 배경음악 볼륨 비율
    /// </summary>
    public float bgVolume = 1;
    /// <summary>
    /// 효과음 볼륨 비율
    /// </summary>
    public float effectVolume = 1;
    /// <summary>
    /// 설정된 언어
    /// </summary>
    public string lang;
    /// <summary>
    /// 버튼 모드
    /// </summary>
    public bool decideButtonMode = true;
}
