using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �߻� �� ���� �ý��� ���� Ŭ����
/// </summary>
public class LauncherSettings : MonoBehaviour
{
    /// <summary>
    /// ���� ��� ������ (����, ��·�)
    /// </summary>
    private enum SelectMode { Angle, Power }
    /// <summary>
    /// ���� ��� �ڷ�ƾ
    /// </summary>
    private IEnumerator selectionMode;
    /// <summary>
    /// ���� ���� �� (����, ��·�)
    /// </summary>
    public SettingPart[] settingPart;
    /// <summary>
    /// ���� ������ �Ϸ�Ǿ����� ����
    /// </summary>
    private bool angleSelected = false;
    /// <summary>
    /// ���� ��ư
    /// </summary>
    public GameObject decide_button;
    /// <summary>
    /// ������ �Ϸ�Ǿ����� ����
    /// </summary>
    public bool isSettingCompleted = false;

    /// <summary>
    /// (�׽�Ʈ��) ��� ���� ��� ����
    /// </summary>
    public bool quickLaunchMode = false; // forTesting

    /// <summary>
    /// �Ŀ� ������ ���� fill �̹���
    /// </summary>
    public Image powerGageInside_img;
    /// <summary>
    /// �߻� ���� ȿ����
    /// </summary>
    public AudioSource beforeLaunch_audio;
    /// <summary>
    /// ��ư ȿ����
    /// </summary>
    public AudioSource buttonAudio;
    /// <summary>
    /// ����� �ݺ� �ڷ�ƾ
    /// </summary>
    IEnumerator audioSectionRepeat;

    /// <summary>
    /// ��·� ���� �����̴�
    /// </summary>
    public Slider power_slider;

    public Launch launch;
    public Settings settings;
    public WeaponManager weaponManager;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// ���� �� ���� �ʱ�ȭ �۾�
    /// </summary>
    public void Initialize()
    {
        // ���� ���ø�� ����
        if (!quickLaunchMode)
            SelectAngle();
        // �׽�Ʈ�� ���� ����
        else
        {
            RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.launcherAxis.transform);
            RocketSet.instance.launcherAxis.transform.rotation = Quaternion.Euler(0, 0, -30);
            power_slider.value = 1;
            isSettingCompleted = true;
            decide_button.SetActive(false);
        }
    }
    /// <summary>
    /// ���� ���� ��� Ȱ��ȭ
    /// </summary>
    private void SelectAngle()
    {
        // �߻�� ������ hierarchy ���� �θ� �����ϰ� ���� ���� ��� �ڷ�ƾ ����
        RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.launcherAxis.transform);
        selectionMode = SeletionMode(settingPart[(int)SelectMode.Angle].min, true, SelectMode.Angle);

        StartCoroutine(selectionMode);
    }

    /// <summary>
    /// ���� ���� �Ϸ� ó��
    /// </summary>
    /// <param name="isReplace">�߰��� ������ ��ü�Ǿ����� ����</param>
    private void CompleteAngle(bool isReplace)
    {
        // �߻�� ���� �ƴ��� Ȯ��
        if (!launch.isStart)
        {
            // ���� ���� ��� �ڷ�ƾ ��Ȱ��ȭ
            if(selectionMode != null)
                StopCoroutine(selectionMode);
            // ���� hierarchy ���� �θ� ���� �� �������� �Ϸ� ó��
            RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.transform);
            angleSelected = true;
            // ��·� ���� ����
            SelectPower(isReplace);
        }
    }
    /// <summary>
    /// ��·� ���� ��� Ȱ��ȭ
    /// </summary>
    /// <param name="isReplace">���� ��ü ����</param>
    private void SelectPower(bool isReplace)
    {
        // ������ �߰��� ��ü���� �ʾ����� ����� ���
        if(!isReplace)
            StartRepeatAudio();
        // �Ŀ� �����̴� ���� ���� ����
        power_slider.interactable = false;
        // �Ŀ� ���� �ڷ�ƾ ����
        selectionMode = SeletionMode(settingPart[(int)SelectMode.Power].min, true, SelectMode.Power);
        StartCoroutine(selectionMode);
    }

    /// <summary>
    /// ��·� ���� �Ϸ� ó��
    /// </summary>
    private void CompletePower()
    {
        // �߻���� �ʾҴ��� Ȯ��
        if (!launch.isStart)
        {
            // ���� ���� �ڷ�ƾ �ߴ�
            if (selectionMode != null)
                StopCoroutine(selectionMode);
            StopCoroutine(selectionMode);
            // �ǽð� �Ŀ� ��� �� ���� �Ϸ� ó��
            launch.realtimePower *= power_slider.value;
            isSettingCompleted = true;
        }
    }

    /// <summary>
    /// ���� ��� �Ϸ� ó��
    /// </summary>
    /// <param name="isRocketReplace">���� ��ü ����</param>
    public void CompleteControl(bool isRocketReplace)
    {
        /* ���� ��ư�� ���� ���¿��� DecideButtonMode�� �ƴϸ� ��ŵ */
        if (!isRocketReplace && !settings.GetDecideButtonMode())
            return;

        // ���� - ��·� ������� ����
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

    /// <summary>
    /// �̺�Ʈ Ʈ���� ��忡���� ��ư �Ϸ� ó��
    /// </summary>
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

    /// <summary>
    /// �߻� �� ȿ���� �ݺ� ���
    /// </summary>
    public void StartRepeatAudio()
    {
        if (RocketSet.instance.rocketData.rocketType != RocketType.BottleRocket)
        {
            beforeLaunch_audio.Play();
            audioSectionRepeat = AudioSectionRepeat(beforeLaunch_audio, 1.5f, 2.0f);
            StartCoroutine(audioSectionRepeat);
        }
    }

    /// <summary>
    /// �߻� �� ȿ���� �ݺ� ��� ����
    /// </summary>
    public void StopRepeatAudio()
    {
        if(audioSectionRepeat != null)
            StopCoroutine(audioSectionRepeat);
    }
    
    /// <summary>
    /// ȿ���� ���� �ݺ�
    /// </summary>
    /// <param name="audio">���� �ݺ��� ����� �ҽ�</param>
    /// <param name="startTime">���� �ð�</param>
    /// <param name="endTime">���� �ð�</param>
    IEnumerator AudioSectionRepeat(AudioSource audio, float startTime, float endTime)
    {
        yield return new WaitForSeconds(0.1f);

        if (audio.time >= endTime)
            audio.time = startTime;

        audioSectionRepeat = AudioSectionRepeat(audio, startTime, endTime);
        StartCoroutine(audioSectionRepeat);
    }
   
    /// <summary>
    /// ��·� ������ ����ȭ
    /// </summary>
    /// <param name="slider">�����̴�</param>
    public void SyncPowerGage(Slider slider)
    {
        powerGageInside_img.fillAmount = slider.value;
    }

    /// <summary>
    /// Ư�� ���� �������� �Դ� �����ϴ� ���� ��� �ݺ� �ڷ�ƾ
    /// </summary>
    /// <param name="sample">���� ��</param>
    /// <param name="isIncrease">�����ϴ� �������� ����</param>
    /// <param name="selectMode">���� ���</param>
    /// <returns></returns>
    IEnumerator SeletionMode(float sample, bool isIncrease, SelectMode selectMode)
    {
        yield return new WaitForEndOfFrame();

        // ���� ��� �� ��, ������ �ӵ��� ���ڸ� ���ϰ�, �ִ�ġ��ŭ �����ϸ� ���� ���� ����
        if (isIncrease)
        {
            sample += settingPart[(int)selectMode].speed * Time.deltaTime;
            if (sample > settingPart[(int)selectMode].max)
            {
                sample = settingPart[(int)selectMode].max;
                isIncrease = false;
            }
        }
        // ���� ����� ��, ������ �ӵ��� ���ڸ� ����, �ּ�ġ ��ŭ �����ϸ� ���� ���� ����
        else
        {
            sample -= settingPart[(int)selectMode].speed * Time.deltaTime;
            if (sample < settingPart[(int)selectMode].min)
            {
                sample = settingPart[(int)selectMode].min;
                isIncrease = true;
            }
        }

        // ���� ���� ����� �� ���� ���� �߻�� ȸ�� ������ ����
        if (selectMode.Equals(SelectMode.Angle))
        {
            RocketSet.instance.launcherAxis.transform.rotation = Quaternion.Euler(0, 0, -sample);
            if(weaponManager.weapon != null)
                weaponManager.weapon.DrawShotLine();
        }
        // ��·� ���� ����� ��, ��·� ������ ���� ���� �� ����
        else if (selectMode.Equals(SelectMode.Power))
        {
            power_slider.value = sample;
        }

        selectionMode = SeletionMode(sample, isIncrease, selectMode);

        StartCoroutine(selectionMode);
    }
}

/// <summary>
/// ���� ���� �� ������ Ŭ����
/// </summary>
[System.Serializable]
public struct SettingPart
{
    /// <summary>
    /// ���� �� �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// ���� �� �ּ� ��ġ
    /// </summary>
    public float min;
    /// <summary>
    /// ���� �� �ִ� ��ġ
    /// </summary>
    public float max;
    /// <summary>
    /// ���� �� ��ȭ �ӵ�
    /// </summary>
    public float speed;
}