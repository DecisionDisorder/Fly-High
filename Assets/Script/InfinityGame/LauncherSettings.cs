using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LauncherSettings : MonoBehaviour
{
    private enum SelectMode { Angle, Power }
    private IEnumerator selectionMode;
    public SettingPart[] settingPart;
    private bool angleSelected = false;
    public GameObject decide_button;
    public bool isSettingCompleted = false;

    public bool quickLaunchMode = false; // forTesting

    public Image powerGageInside_img;
    public AudioSource beforeLaunch_audio;
    public AudioSource buttonAudio;
    IEnumerator audioSectionRepeat;

    public Slider power_slider;
    public Launch launch;
    public Settings settings;
    public WeaponManager weaponManager;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (!quickLaunchMode)
            SelectAngle();
        else
        {
            RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.launcherAxis.transform);
            RocketSet.instance.launcherAxis.transform.rotation = Quaternion.Euler(0, 0, -30);
            power_slider.value = 1;
            isSettingCompleted = true;
            decide_button.SetActive(false);
        }
    }

    private void SelectAngle()
    {
        RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.launcherAxis.transform);
        selectionMode = SeletionMode(settingPart[(int)SelectMode.Angle].min, true, SelectMode.Angle);

        StartCoroutine(selectionMode);
    }

    private void CompleteAngle(bool isReplace)
    {
        if (!launch.isStart)
        {
            if(selectionMode != null)
                StopCoroutine(selectionMode);
            RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.transform);
            angleSelected = true;
            SelectPower(isReplace);
        }
    }
    private void SelectPower(bool isReplace)
    {
        if(!isReplace)
            StartRepeatAudio();
        power_slider.interactable = false;
        selectionMode = SeletionMode(settingPart[(int)SelectMode.Power].min, true, SelectMode.Power);
        StartCoroutine(selectionMode);
    }

    private void CompletePower()
    {
        if (!launch.isStart)
        {
            if (selectionMode != null)
                StopCoroutine(selectionMode);
            StopCoroutine(selectionMode);
            launch.realtimePower *= power_slider.value;
            isSettingCompleted = true;
        }
    }

    public void CompleteControl(bool isRocketReplace = false)
    {
        /* 결정 버튼을 누른 상태에서 DecideButtonMode가 아니면 스킵 */
        if (!isRocketReplace && !settings.GetDecideButtonMode())
            return;

        if(!angleSelected)
        {
            CompleteAngle(isRocketReplace);
        }
        else
        {
            CompletePower();
            decide_button.SetActive(false);
        }
        if(!isRocketReplace)
            buttonAudio.Play();
    }

    public void ETCompleteControl()
    {
        if (!settings.GetDecideButtonMode())
        {
            if (!angleSelected)
            {
                CompleteAngle(false);
            }
            else
            {
                CompletePower();
                decide_button.SetActive(false);
            }
            buttonAudio.Play();
        }
    }

    public void StartRepeatAudio()
    {
        if (RocketSet.instance.rocketData.rocketType != RocketType.BottleRocket)
        {
            beforeLaunch_audio.Play();
            audioSectionRepeat = AudioSectionRepeat(beforeLaunch_audio, 1.5f, 2.0f);
            StartCoroutine(audioSectionRepeat);
        }
    }

    public void StopRepeatAudio()
    {
        if(audioSectionRepeat != null)
            StopCoroutine(audioSectionRepeat);
    }

    IEnumerator AudioSectionRepeat(AudioSource audio, float startTime, float endTime)
    {
        yield return new WaitForSeconds(0.1f);

        if (audio.time >= endTime)
            audio.time = startTime;

        audioSectionRepeat = AudioSectionRepeat(audio, startTime, endTime);
        StartCoroutine(audioSectionRepeat);
    }
   
    public void SyncPowerGage(Slider slider)
    {
        powerGageInside_img.fillAmount = slider.value;
    }

    IEnumerator SeletionMode(float sample, bool isIncrease, SelectMode selectMode)
    {
        yield return new WaitForEndOfFrame();

        if (isIncrease)
        {
            sample += settingPart[(int)selectMode].speed * Time.deltaTime;
            if (sample > settingPart[(int)selectMode].max)
            {
                sample = settingPart[(int)selectMode].max;
                isIncrease = false;
            }
        }
        else
        {
            sample -= settingPart[(int)selectMode].speed * Time.deltaTime;
            if (sample < settingPart[(int)selectMode].min)
            {
                sample = settingPart[(int)selectMode].min;
                isIncrease = true;
            }
        }

        if (selectMode.Equals(SelectMode.Angle))
        {
            RocketSet.instance.launcherAxis.transform.rotation = Quaternion.Euler(0, 0, -sample);
            if(weaponManager.weapon != null)
                weaponManager.weapon.DrawShotLine();
        }
        else if (selectMode.Equals(SelectMode.Power))
        {
            power_slider.value = sample;
        }

        selectionMode = SeletionMode(sample, isIncrease, selectMode);

        StartCoroutine(selectionMode);
    }
}

[System.Serializable]
public struct SettingPart
{
    public string name;
    public float min;
    public float max;
    public float speed;
}