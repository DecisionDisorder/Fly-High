using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 발사 전 설정 시스템 관리 클래스
/// </summary>
public class LauncherSettings : MonoBehaviour
{
    /// <summary>
    /// 설정 모드 열거형 (각도, 출력량)
    /// </summary>
    private enum SelectMode { Angle, Power }
    /// <summary>
    /// 선택 모드 코루틴
    /// </summary>
    private IEnumerator selectionMode;
    /// <summary>
    /// 사전 설정 값 (각도, 출력량)
    /// </summary>
    public SettingPart[] settingPart;
    /// <summary>
    /// 각도 설정이 완료되었는지 여부
    /// </summary>
    private bool angleSelected = false;
    /// <summary>
    /// 결정 버튼
    /// </summary>
    public GameObject decide_button;
    /// <summary>
    /// 설정이 완료되었는지 여부
    /// </summary>
    public bool isSettingCompleted = false;

    /// <summary>
    /// (테스트용) 즉시 설정 모드 여부
    /// </summary>
    public bool quickLaunchMode = false; // forTesting

    /// <summary>
    /// 파워 게이지 내부 fill 이미지
    /// </summary>
    public Image powerGageInside_img;
    /// <summary>
    /// 발사 직전 효과음
    /// </summary>
    public AudioSource beforeLaunch_audio;
    /// <summary>
    /// 버튼 효과음
    /// </summary>
    public AudioSource buttonAudio;
    /// <summary>
    /// 오디오 반복 코루틴
    /// </summary>
    IEnumerator audioSectionRepeat;

    /// <summary>
    /// 출력량 조절 슬라이더
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
    /// 로켓 및 설정 초기화 작업
    /// </summary>
    public void Initialize()
    {
        // 각도 선택모드 시작
        if (!quickLaunchMode)
            SelectAngle();
        // 테스트용 빠른 설정
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
    /// 각도 선택 모드 활성화
    /// </summary>
    private void SelectAngle()
    {
        // 발사대 축으로 hierarchy 상의 부모를 변경하고 각도 설정 모드 코루틴 시작
        RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.launcherAxis.transform);
        selectionMode = SeletionMode(settingPart[(int)SelectMode.Angle].min, true, SelectMode.Angle);

        StartCoroutine(selectionMode);
    }

    /// <summary>
    /// 각도 설정 완료 처리
    /// </summary>
    /// <param name="isReplace">중간에 로켓이 교체되었는지 여부</param>
    private void CompleteAngle(bool isReplace)
    {
        // 발사된 것이 아닌지 확인
        if (!launch.isStart)
        {
            // 기존 선택 모드 코루틴 비활성화
            if(selectionMode != null)
                StopCoroutine(selectionMode);
            // 로켓 hierarchy 상의 부모 변경 및 각도설정 완료 처리
            RocketSet.instance.rocket.transform.SetParent(RocketSet.instance.transform);
            angleSelected = true;
            // 출력량 설정 시작
            SelectPower(isReplace);
        }
    }
    /// <summary>
    /// 출력량 선택 모드 활성화
    /// </summary>
    /// <param name="isReplace">로켓 교체 여부</param>
    private void SelectPower(bool isReplace)
    {
        // 로켓이 중간에 교체되지 않았으면 오디오 재생
        if(!isReplace)
            StartRepeatAudio();
        // 파워 슬라이더 직접 조작 막기
        power_slider.interactable = false;
        // 파워 설정 코루틴 시작
        selectionMode = SeletionMode(settingPart[(int)SelectMode.Power].min, true, SelectMode.Power);
        StartCoroutine(selectionMode);
    }

    /// <summary>
    /// 출력량 설정 완료 처리
    /// </summary>
    private void CompletePower()
    {
        // 발사되지 않았는지 확인
        if (!launch.isStart)
        {
            // 기존 설정 코루틴 중단
            if (selectionMode != null)
                StopCoroutine(selectionMode);
            StopCoroutine(selectionMode);
            // 실시간 파워 계산 및 설정 완료 처리
            launch.realtimePower *= power_slider.value;
            isSettingCompleted = true;
        }
    }

    /// <summary>
    /// 설정 모드 완료 처리
    /// </summary>
    /// <param name="isRocketReplace">로켓 교체 여부</param>
    public void CompleteControl(bool isRocketReplace)
    {
        /* 결정 버튼을 누른 상태에서 DecideButtonMode가 아니면 스킵 */
        if (!isRocketReplace && !settings.GetDecideButtonMode())
            return;

        // 각도 - 출력량 순서대로 설정
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
    /// 이벤트 트리거 모드에서의 버튼 완료 처리
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
    /// 발사 전 효과음 반복 재생
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
    /// 발사 전 효과음 반복 재생 종료
    /// </summary>
    public void StopRepeatAudio()
    {
        if(audioSectionRepeat != null)
            StopCoroutine(audioSectionRepeat);
    }
    
    /// <summary>
    /// 효과음 구간 반복
    /// </summary>
    /// <param name="audio">구간 반복할 오디오 소스</param>
    /// <param name="startTime">시작 시간</param>
    /// <param name="endTime">종료 시간</param>
    IEnumerator AudioSectionRepeat(AudioSource audio, float startTime, float endTime)
    {
        yield return new WaitForSeconds(0.1f);

        if (audio.time >= endTime)
            audio.time = startTime;

        audioSectionRepeat = AudioSectionRepeat(audio, startTime, endTime);
        StartCoroutine(audioSectionRepeat);
    }
   
    /// <summary>
    /// 출력량 게이지 동기화
    /// </summary>
    /// <param name="slider">슬라이더</param>
    public void SyncPowerGage(Slider slider)
    {
        powerGageInside_img.fillAmount = slider.value;
    }

    /// <summary>
    /// 특정 숫자 범위에서 왔다 갔다하는 선택 모드 반복 코루틴
    /// </summary>
    /// <param name="sample">샘플 값</param>
    /// <param name="isIncrease">증가하는 차례인지 여부</param>
    /// <param name="selectMode">선택 모드</param>
    /// <returns></returns>
    IEnumerator SeletionMode(float sample, bool isIncrease, SelectMode selectMode)
    {
        yield return new WaitForEndOfFrame();

        // 증가 모드 일 때, 정해진 속도로 숫자를 더하고, 최대치만큼 도달하면 감소 모드로 변경
        if (isIncrease)
        {
            sample += settingPart[(int)selectMode].speed * Time.deltaTime;
            if (sample > settingPart[(int)selectMode].max)
            {
                sample = settingPart[(int)selectMode].max;
                isIncrease = false;
            }
        }
        // 감소 모드일 때, 정해진 속도로 숫자를 빼고, 최소치 만큼 도달하면 증가 모드로 변경
        else
        {
            sample -= settingPart[(int)selectMode].speed * Time.deltaTime;
            if (sample < settingPart[(int)selectMode].min)
            {
                sample = settingPart[(int)selectMode].min;
                isIncrease = true;
            }
        }

        // 각도 선택 모드일 때 샘플 값을 발사대 회전 각도로 적용
        if (selectMode.Equals(SelectMode.Angle))
        {
            RocketSet.instance.launcherAxis.transform.rotation = Quaternion.Euler(0, 0, -sample);
            if(weaponManager.weapon != null)
                weaponManager.weapon.DrawShotLine();
        }
        // 출력량 선택 모드일 때, 출력량 게이지 값에 샘플 값 적용
        else if (selectMode.Equals(SelectMode.Power))
        {
            power_slider.value = sample;
        }

        selectionMode = SeletionMode(sample, isIncrease, selectMode);

        StartCoroutine(selectionMode);
    }
}

/// <summary>
/// 사전 설정 값 데이터 클래스
/// </summary>
[System.Serializable]
public struct SettingPart
{
    /// <summary>
    /// 설정 값 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 설정 값 최소 수치
    /// </summary>
    public float min;
    /// <summary>
    /// 설정 값 최대 수치
    /// </summary>
    public float max;
    /// <summary>
    /// 설정 값 변화 속도
    /// </summary>
    public float speed;
}