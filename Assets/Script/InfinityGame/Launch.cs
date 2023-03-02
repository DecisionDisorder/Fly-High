using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로켓 발사 시스템 관리 클래스
/// </summary>
public class Launch : MonoBehaviour
{
    /// <summary>
    /// 발사 시스템의 싱글톤 인스턴스
    /// </summary>
    public static Launch instance;
    /// <summary>
    /// 따라올 카메라 제어 오브젝트
    /// </summary>
    public CameraFollow cameraFollow;

    public StatManager statManager;
    public LauncherSettings launcherSettings;
    public ItemManager itemManager;
    public Settings settings;
    public WeaponManager weaponManager;
    
    /// <summary>
    /// 연료 게이지 회전 중심 축
    /// </summary>
    public GameObject fuelGage_axis;
    /// <summary>
    /// 연료 아이콘 이미지
    /// </summary>
    public Image fuelMark;
    /// <summary>
    /// 연료 낮음 상태 색상
    /// </summary>
    public Color lowColor;
    /// <summary>
    /// 연료 소진 상태 색상
    /// </summary>
    public Color emptyColor;
    /// <summary>
    /// 연료 보충 상태 색상
    /// </summary>
    public Color supplyColor;
    /// <summary>
    /// 연료 보충 속도
    /// </summary>
    public float supplyingSpeed = 0.4f;
    /// <summary>
    /// 연료 보충 중인지 여부
    /// </summary>
    private bool isSupplying = false;
    /// <summary>
    /// 연료 단계 정보 텍스트
    /// </summary>
    public Text fuelStage_text;
    /// <summary>
    /// 출력 효율 스탯 수치
    /// </summary>
    public float powerEfficiency = 1f; //연비향상 레벨 상승으로 조절

    /// <summary>
    /// 일시정지 버튼
    /// </summary>
    public GameObject pause_button;
    /// <summary>
    /// 가속 레버 버튼
    /// </summary>
    public Button acceleratorButton;
    /// <summary>
    /// 가속 레버가 내려가있는지 여부
    /// </summary>
    private bool onAccButton = false;
    /// <summary>
    /// 로켓이 가속 상태인지 여부
    /// </summary>
    public bool isAccerlate = false;
    /// <summary>
    /// 발사대에서 출발했는지 여부
    /// </summary>
    public bool isStart = false;
    /// <summary>
    /// 발사 레버 버튼 이미지
    /// </summary>
    public Image launchLever_img;
    /// <summary>
    /// 발사 레버 스프라이트 이미지 배열
    /// </summary>
    public Sprite[] launchLever_sprites;

    /// <summary>
    /// 첫 추가 가속 시간
    /// </summary>
    public float firstAccerlateTime;
    /// <summary>
    /// 첫 추가 가속 차감 시간
    /// </summary>
    public float decreasingTime;
    /// <summary>
    /// 실시간 로켓 출력량
    /// </summary>
    public float realtimePower;
    /// <summary>
    /// 첫 추가 가속 힘의 배율
    /// </summary>
    public float firstAclPower;
    /// <summary>
    /// 로켓의 기준 출력량
    /// </summary>
    public float stdPower;
    /// <summary>
    /// 출력량 조절 슬라이더
    /// </summary>
    public Slider power_slider;
    /// <summary>
    /// 부스터 파워 능력치
    /// </summary>
    public float boosterPowerAbility;

    /// <summary>
    /// 로켓 회전 힘
    /// </summary>
    public float rotateForce;
    /// <summary>
    /// 회전 중인지 여부
    /// </summary>
    public bool isRotating = false;
    /// <summary>
    /// 회전 가능 여부
    /// </summary>
    public bool allowRotate = true;
    /// <summary>
    /// 로켓 회전 코루틴
    /// </summary>
    private IEnumerator rotateRocket;

    /// <summary>
    /// 고도 단계별 중력량
    /// </summary>
    public float[] gravityPortion;
    /// <summary>
    /// 고도 기준치
    /// </summary>
    public float[] heightCriteria;

    /// <summary>
    /// 로켓 화염 효과음
    /// </summary>
    public AudioSource rocketFire_AudioSource;
    /// <summary>
    /// 로켓 화혐 효과음 볼륨 비율
    /// </summary>
    public float rocketFireAudioVolumeRatio = 1f;
    /// <summary>
    /// 로켓 화염 효과음 클립 배열
    /// </summary>
    public AudioClip[] rocketFires;
    /// <summary>
    /// 첫 발사 효과음인지 여부
    /// </summary>
    private bool isFireSoundPlay = false;

    /// <summary>
    /// 태블릿 버튼
    /// </summary>
    public GameObject tablet_button;
    /// <summary>
    /// 아이템 사용 버튼
    /// </summary>
    public Button item_button;
    /// <summary>
    /// 무기 발포 버튼
    /// </summary>
    public GameObject shot_button;
    /// <summary>
    /// 레버 다기는 효과음
    /// </summary>
    public AudioSource leverAudio;
    /// <summary>
    /// 연료 보충 효과음
    /// </summary>
    public AudioSource fuelingAudio;
    /// <summary>
    /// 폭발 효과음
    /// </summary>
    public AudioSource explosionAudio;

    private void Start()
    {
        if (instance == null)
            instance = this;
        // 기준, 실시간 출력량 스펙에 따라 초기화
        stdPower = realtimePower = RocketSet.instance.rocketData.MaxPower[0];
        // 로켓 물리 설정 초기화
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;
        if(RocketSet.instance.launcher_rigid != null)
            RocketSet.instance.launcher_rigid.bodyType = RigidbodyType2D.Kinematic;
        
        SetFuelStageText();
    }

    /// <summary>
    /// 가속 레버 버튼 리스너
    /// </summary>
    public void SetLaunchMode()
    {
        // 발사 준비가 완료 되었는지 확인
        if (launcherSettings.isSettingCompleted)
        {
            // 현재 레버 버튼 상태에 따라 올리거나 내리기
            if (!onAccButton)
            {
                launchLever_img.sprite = launchLever_sprites[1];
                AcceleratorDown();
            }
            else
            {
                launchLever_img.sprite = launchLever_sprites[0];
                AcceleratorUp();
            }
        }
    }

    /// <summary>
    /// 가속 레버 내리기 (가속 on)
    /// </summary>
    public void AcceleratorDown()
    {
        // 가속 버튼 상태 변경
        onAccButton = true;
        // 출발하지 않은 상태이면
        if(!isStart)
        {
            // 반복 재생 중인 효과음 중지
            launcherSettings.StopRepeatAudio();
            // 로켓 발사 효과음 재생
            PlayRocketFireSound();
            // 로켓 발사 효과 재생
            RocketSet.instance.FireEffectPlay();
            // 보조엔진 장착 시, 보조 엔진 사용(차감) 처리
            if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
                itemManager.UseItem(ItemManager.ItemType.VernierEngine);
            // 발사 스모크 효과 재생
            RocketSet.instance.launchSmokeEffect.Play();
            // 태블릿 버튼 비활성화
            tablet_button.SetActive(false);
            // 장착된 아이템이 있을 경우 (보조엔진 제외) 버튼 아이콘 설정 및 활성화
            if (ItemManager.equippedItem != null && ItemManager.equippedItem != ItemManager.ItemType.VernierEngine)
            {
                item_button.gameObject.SetActive(true);
                item_button.image.sprite = itemManager.items[(int)ItemManager.equippedItem].IconSprite;
            }
            // 장착된 무기가 있을 경우, 발사 버튼 활성화
            if(EquipmentManager.equippedWeaponIndex != -1)
                shot_button.SetActive(true);
            // 첫 발사 효과 제어 코루틴 시작
            StartCoroutine(FirstLaunchEffect(1.5f));
        }
        // 발사 이후에 가속을 다시 시작한 경우
        else
        {
            // 남은 연료량 확인 후, 가속 처리
            if (RocketSet.instance.FuelRemain > 0)
            {
                PlayRocketFireSound();
                RocketSet.instance.rocketMain_rigid.gravityScale = 0f;
                isAccerlate = true;
                RocketSet.instance.FireEffectPlay();
                StartCoroutine(Accerlating());
                leverAudio.Play();
            }
        }
    }

    /// <summary>
    /// 가속 레버 올리기 (가속 off)
    /// </summary>
    public void AcceleratorUp()
    {
        // 로켓 발사 효과음 중지
        StopRocketFireSound();
        // 레버 상태 변경
        onAccButton = false;
        // 중력 적용
        RocketSet.instance.rocketMain_rigid.gravityScale = 1f;
        // 가속 상태 비활성화
        isAccerlate = false;
        
        // (플레이어 제어)회전 불가 상태로 변경
        RotateRocketOff();
        RocketSet.instance.FireEffectStop();
        // 레버 효과음 재생
        if (PlayManager.isSurvive)
            leverAudio.Play();
    }

    /// <summary>
    /// 로켓 추진 효과음 재생
    /// </summary>
    public void PlayRocketFireSound()
    {
        if (!rocketFire_AudioSource.isPlaying)
        {
            rocketFire_AudioSource.loop = false;
            rocketFire_AudioSource.clip = rocketFires[0];
            rocketFire_AudioSource.Play();
            isFireSoundPlay = true;
            StartCoroutine(WaitForFirstClip());
        }
    }

    /// <summary>
    /// 2차 추진 클립 재생을 위한 코루틴
    /// </summary>
    IEnumerator WaitForFirstClip()
    {
        // 게임 일시정지거나 이미 재생 중일 땐 대기
        while (rocketFire_AudioSource.isPlaying || TabletManager.isPause)
            yield return new WaitForEndOfFrame();

        // 재생이 허가되었을 때 재생
        if (isFireSoundPlay)
        {
            rocketFire_AudioSource.clip = rocketFires[1];
            rocketFire_AudioSource.loop = true;
            rocketFire_AudioSource.Play();
        }
    }
    /// <summary>
    /// 추진 효과음 중단
    /// </summary>
    public void StopRocketFireSound()
    {
        isFireSoundPlay = false;
        rocketFire_AudioSource.Stop();
    }
    /// <summary>
    /// 로켓 화염 크기 동기화
    /// </summary>
    public void SyncRocketFire()
    {
        float ratio = power_slider.value / power_slider.maxValue;
        RocketSet.instance.rocketFire.FireSizeSync(ratio);
    }
    /// <summary>
    /// 가속 처리 코루틴
    /// </summary>
    IEnumerator Accerlating()
    {
        yield return new WaitForFixedUpdate();

        // 일시정지가 아닌 상태인지 확인
        if (!TabletManager.isPause)
        {
            // 남은 로켓 연료량 확인
            if (RocketSet.instance.FuelRemain > 0)
            {
                // 부스터 상태가 아니면 게이지에 따라 출력량 계산
                if (!itemManager.boosterOn)
                {
                    if (power_slider.interactable)
                        realtimePower = stdPower * power_slider.value;
                }
                // 부스터일 때 최대 출력 x 부스터 파워에 따라 계산
                else
                    realtimePower = stdPower * boosterPowerAbility;
                // 물리적 추진
                RocketSet.instance.rocketMain_rigid.AddForce(RocketSet.instance.rocket.transform.up * realtimePower);
                // 대기권 효과 재생 및 효과음 재생
                SetAtmosphereEffect();
                SetRocketFireVolume();
                // 지구를 벗어났을 때 출력량 슬라이더 조절 가능하도록 변경
                if(isOutsideEarth())
                    if (statManager.GetLevel(StatType.Accelerate).Equals(1))
                        power_slider.interactable = true;
                // 연료 소비
                ConsumeFuel(Time.deltaTime);
            }
        }

        if(isAccerlate && RocketSet.instance.FuelRemain > 0)
            StartCoroutine(Accerlating());
        // 연료를 모두 소진하면 가속 레버 올리고, 일시정지 버튼 비활성화
        if (RocketSet.instance.FuelRemain <= 0)
        {
            AcceleratorUp();
            pause_button.SetActive(false);
        }
    }

    /// <summary>
    /// 대기권 고도일 때 대기권 돌파 효과 재생
    /// </summary>
    private void SetAtmosphereEffect()
    {
        // 로켓에 대기권 효과 이펙트가 있을 때
        if(RocketSet.instance.atmosphereEffect != null)
        {
            // 배경이 따라오고 있을 때
            if (BackgroundControl.isBackgroundFollow)
            {
                // 로켓의 고도를 계산하여 특정 범위에 있을때 대기권 돌파 효과 재생
                float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
                if (heightCriteria[1] - 75 <= rocketY && rocketY <= heightCriteria[1] + 40)
                    RocketSet.instance.atmosphereEffect.Play();
                // 그 이상으로 올라가면 중단
                else
                {
                    if (RocketSet.instance.atmosphereEffect.isPlaying)
                        RocketSet.instance.atmosphereEffect.Stop();
                }
            }
        }
    }

    /// <summary>
    /// 출력량 하방 한계 처리
    /// </summary>
    public void PowerUnderLimit()
    {
        if (power_slider.value < 0.1f)
            power_slider.value = 0.1f;
    }
    /// <summary>
    /// 파워가 낮을 때 중력 계산 추가
    /// </summary>
    public void PowerUnderPenalty()
    {
        if (power_slider.value < 0.5f)
            RocketSet.instance.rocketMain_rigid.gravityScale = 0.5f;
        else
            RocketSet.instance.rocketMain_rigid.gravityScale = 0f;
    }
    
    /// <summary>
    /// 첫 발사 효과 처리 코루틴
    /// </summary>
    /// <param name="time">대기 시간</param>
    IEnumerator FirstLaunchEffect(float time)
    {
        // 처음에 카메라가 더 천천히 따라오도록 설정
        cameraFollow.followSpeed = 0.1f;
        cameraFollow.StartFollow();
        yield return new WaitForSeconds(time);

        // 로켓 물리 타입 변경 및 첫 가속 시작
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Dynamic;
        StartFirstAccerlate();
        // 발사 스모크 효과 중단
        RocketSet.instance.launchSmokeEffect.Stop();
        // 실시간 출력량 계산 및 카메라 팔로우 스피드 조정
        realtimePower = stdPower * power_slider.value;
        cameraFollow.followSpeed = 0.7f;
        pause_button.SetActive(true);
        // 출발 처리
        isStart = true;
        if (RocketSet.instance.launcher_rigid != null)
        {
            RocketSet.instance.launcher_rigid.bodyType = RigidbodyType2D.Dynamic;
            StartCoroutine(DisableLauncher(5f));
        }
        // 가속 레버 트리거
        if (onAccButton)
            AcceleratorDown();
        else
            AcceleratorUp();
    }
    /// <summary>
    /// 첫 가속 처리 시작
    /// </summary>
    private void StartFirstAccerlate()
    {
        // 초반 추가 가속력 계산(보정 값)
        realtimePower *= firstAclPower;
        StartCoroutine(FirstAccerlatingForce(decreasingTime, firstAccerlateTime, firstAclPower));
    }
    /// <summary>
    /// 초반 추가 가속력 차감 코루틴
    /// </summary>
    /// <param name="deltaTime">간격 시간</param>
    /// <param name="remainTime">남은 시간</param>
    /// <param name="power">추가된 파워 배율</param>
    IEnumerator FirstAccerlatingForce(float deltaTime, float remainTime, float power)
    {
        yield return new WaitForSeconds(deltaTime);

        // 남은 시간에 비례하여 추가 가속량 감소
        remainTime -= deltaTime;
        power += (1 - firstAclPower) / firstAccerlateTime * deltaTime;

        realtimePower = (stdPower * power_slider.value) * power;

        if (remainTime > 0)
            StartCoroutine(FirstAccerlatingForce(deltaTime, remainTime, power));
        else
        {
            realtimePower = stdPower * power_slider.value;
        }
    }

    /// <summary>
    /// 발사 후 발사대 이미지 비활성화 대기 코루틴
    /// </summary>
    /// <param name="time">대기 시간</param>
    IEnumerator DisableLauncher(float time)
    {
        yield return new WaitForSeconds(time);

        RocketSet.instance.launcher_rigid.gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어 개입으로 로켓 회전
    /// </summary>
    /// <param name="isLeft">왼쪽으로 회전할지 여부</param>
    public void RotateRocketOn(bool isLeft)
    {
        // 회전이 가능하며, 가속중일 때
        if (allowRotate && isAccerlate)
        {
            // 지구 밖으로 나갔을 때
            if (isOutsideEarth())
            {
                // 회전 시작
                rotateRocket = RotateRocket(isLeft);
                isRotating = true;
                StartCoroutine(rotateRocket);
            }
        }
    }

    /// <summary>
    /// 플레이어의 회전 개입 종료
    /// </summary>
    public void RotateRocketOff()
    {
        // 지구 밖일 때
        if (isOutsideEarth())
        {
            // 회전 중단
            isRotating = false;
            if(rotateRocket != null)
                StopCoroutine(rotateRocket);
            StartCoroutine(ResetFireRotate());
        }
    }

    /// <summary>
    /// 플레이어 개입의 로켓 회전 코루틴
    /// </summary>
    /// <param name="isLeft">회전 방향이 왼쪽인지 여부</param>
    /// <param name="rotateTime">회전 시간</param>
    IEnumerator RotateRocket(bool isLeft, float rotateTime = 0f)
    {
        yield return new WaitForEndOfFrame();

        rotateTime += Time.deltaTime;

        // 방향에 따라 로켓의 특정 위치를 기준으로 회전 힘 적용
        if (isLeft)
        {
            RocketSet.instance.rocketMain_rigid.AddForceAtPosition(Vector2.left * rotateForce * 0.01f, new Vector2(0, RocketSet.instance.rocket_rends[0].bounds.max.y), ForceMode2D.Impulse);
        }
        else
        {
            RocketSet.instance.rocketMain_rigid.AddForceAtPosition(Vector2.right * rotateForce * 0.01f, new Vector2(0, RocketSet.instance.rocket_rends[0].bounds.max.y), ForceMode2D.Impulse);
        }

        // 회전 시간이 0.25초 이하일 때 지속적 회전 적용
        if (rotateTime < 0.25f)
        {
            rotateRocket = RotateRocket(isLeft, rotateTime);
            StartCoroutine(rotateRocket);
        }
        // 0.25초가 지났을 때 회전 종료
        else
            RotateRocketOff();
    }
    /// <summary>
    /// 회전할 때 같이 회전한 화염 효과를 원래 방향으로 복원시키는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetFireRotate()
    {
        yield return new WaitForEndOfFrame();

        // 복원 속도
        float speed = 3f;
        float z = RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z;
        // 회전한 방향에 따라 회전 각도 계산 후 회전 처리
        if (z > 5 && z < rotateForce)
        {
           RocketSet.instance.fireEffect.transform.Rotate(0, 0, -rotateForce * Time.deltaTime * speed);
            StartCoroutine(ResetFireRotate());
        }
        else if (z < 360 && z > 360 - rotateForce)
        {
            RocketSet.instance.fireEffect.transform.Rotate(0, 0, rotateForce * Time.deltaTime * speed);
            StartCoroutine(ResetFireRotate());
        }
        // 일정 범위로 돌아왔을 때 각도를 0으로 처리
        else
        {
            RocketSet.instance.fireEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// 효율 곡선 대비 실시간 출력량으로 출력 효율 계산
    /// </summary>
    /// <returns>파워 효율 수치</returns>
    private float GetPowerEfficiencyByPower()
    {
        float powerPercentage = realtimePower / stdPower * 100f;
        float result = (RocketSet.instance.rocketData.A * powerPercentage * powerPercentage + RocketSet.instance.rocketData.B * powerPercentage + RocketSet.instance.rocketData.C) * 0.01f;
        return result * realtimePower;
    }

    /// <summary>
    /// 연료 소비
    /// </summary>
    /// <param name="deltaTime">흐른 시간</param>
    private void ConsumeFuel(float deltaTime)
    {
        // 초당 연료 소비량 계산 및 흐른 시간에 비례하여 연료 소비 처리
        int currentStage = RocketSet.instance.currentStage;
        float consumePsec = (RocketSet.instance.rocketMain_rigid.mass * GetGravityPortion() + GetPowerEfficiencyByPower() * powerEfficiency) * 0.02f / RocketSet.instance.rocketData.FuelEfficienty;
        
        RocketSet.instance.FuelRemain -= consumePsec * deltaTime;
        if (currentStage != RocketSet.instance.currentStage)
            SetFuelStageText();
        if(!isSupplying)
            SetFuelGage();
    }

    /// <summary>
    /// 연료 게이지 업데이트
    /// </summary>
    private void SetFuelGage()
    {
        float percentage = RocketSet.instance.FuelRemain / RocketSet.instance.fuelMax[RocketSet.instance.currentStage];
        float rotation = percentage * 110 - 55;
        rotation = Mathf.Clamp(rotation, -55f, 55f);
        fuelGage_axis.transform.rotation = Quaternion.Euler(0, 0, rotation);
        if (percentage < 0.01f)
            fuelMark.color = emptyColor;
        else if (percentage < 0.3f)
            fuelMark.color = lowColor;
        else
            fuelMark.color = Color.white;
    }

    /// <summary>
    /// 연료 보충
    /// </summary>
    /// <param name="value">보충량</param>
    public void SupplyFuel(float value)
    {
        isSupplying = true;
        fuelMark.color = supplyColor;
        fuelingAudio.Play();
        StartCoroutine(SupplyFuelEffect(value));
    }
    /// <summary>
    /// 연료 보충 효과 코루틴
    /// </summary>
    /// <param name="value">보충량</param>
    IEnumerator SupplyFuelEffect(float value)
    {
        yield return new WaitForEndOfFrame();

        // 연료 보충 속도에 따른 프레임별 보충량 계산
        value += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * supplyingSpeed * Time.deltaTime;
        // 보충 비율 계산 후 게이지 바늘 각도 계산 후 적용
        float percentage = value / RocketSet.instance.fuelMax[RocketSet.instance.currentStage];
        float rotation = percentage * 110 - 55;
        rotation = Mathf.Clamp(rotation, -55f, 55f);
        fuelGage_axis.transform.rotation = Quaternion.Euler(0, 0, rotation);

        // 연료 보충이 끝나면 보충 이펙트 종료
        if (value >= RocketSet.instance.FuelRemain)
        {
            isSupplying = false;
            fuelMark.color = Color.white;
            fuelingAudio.Stop();
        }
        else
            StartCoroutine(SupplyFuelEffect(value));
    }

    /// <summary>
    /// 로켓 고도에 따른 중력 비율 계산
    /// </summary>
    /// <returns>중력 비율</returns>
    private float GetGravityPortion()
    {
        float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
        if (rocketY < heightCriteria[0])
            return gravityPortion[0];
        else if (rocketY < heightCriteria[1])
            return gravityPortion[1];
        else
            return gravityPortion[2];
    }

    /// <summary>
    /// 로켓 화염 효과음 볼륨 설정
    /// </summary>
    private void SetRocketFireVolume()
    {
        float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
        if (rocketY < heightCriteria[0])
            rocketFireAudioVolumeRatio = 1f;
        else if (rocketY < heightCriteria[1])
            rocketFireAudioVolumeRatio = 0.6f;
        else
            rocketFireAudioVolumeRatio = 0.3f;
        rocketFire_AudioSource.volume = rocketFireAudioVolumeRatio * settings.GetEffectVolume();
    }

    /// <summary>
    /// 로켓이 지구를 벗어났는지 여부
    /// </summary>
    /// <returns>지구 탈출 여부</returns>
    private bool isOutsideEarth()
    {
        float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
        if (rocketY < heightCriteria[1])
            return true;//false
        else
            return true;
    }

    /// <summary>
    /// 현재 소모 중인 연료의 단 수를 업데이트
    /// </summary>
    public void SetFuelStageText()
    {
        fuelStage_text.text = GetStageAbbrev();
    }

    /// <summary>
    /// 현재의 단 수를 축약어 형태로 리턴
    /// </summary>
    /// <returns>현재 단 수(축약)</returns>
    private string GetStageAbbrev()
    {
        int currentStage = RocketSet.instance.currentStage;
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
        {
            if (currentStage.Equals(0))
                return "V";
            else
                return currentStage.ToString();
        }
        else
        {
            return (currentStage + 1).ToString();
        }
    }
}