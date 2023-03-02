using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �߻� �ý��� ���� Ŭ����
/// </summary>
public class Launch : MonoBehaviour
{
    /// <summary>
    /// �߻� �ý����� �̱��� �ν��Ͻ�
    /// </summary>
    public static Launch instance;
    /// <summary>
    /// ����� ī�޶� ���� ������Ʈ
    /// </summary>
    public CameraFollow cameraFollow;

    public StatManager statManager;
    public LauncherSettings launcherSettings;
    public ItemManager itemManager;
    public Settings settings;
    public WeaponManager weaponManager;
    
    /// <summary>
    /// ���� ������ ȸ�� �߽� ��
    /// </summary>
    public GameObject fuelGage_axis;
    /// <summary>
    /// ���� ������ �̹���
    /// </summary>
    public Image fuelMark;
    /// <summary>
    /// ���� ���� ���� ����
    /// </summary>
    public Color lowColor;
    /// <summary>
    /// ���� ���� ���� ����
    /// </summary>
    public Color emptyColor;
    /// <summary>
    /// ���� ���� ���� ����
    /// </summary>
    public Color supplyColor;
    /// <summary>
    /// ���� ���� �ӵ�
    /// </summary>
    public float supplyingSpeed = 0.4f;
    /// <summary>
    /// ���� ���� ������ ����
    /// </summary>
    private bool isSupplying = false;
    /// <summary>
    /// ���� �ܰ� ���� �ؽ�Ʈ
    /// </summary>
    public Text fuelStage_text;
    /// <summary>
    /// ��� ȿ�� ���� ��ġ
    /// </summary>
    public float powerEfficiency = 1f; //������� ���� ������� ����

    /// <summary>
    /// �Ͻ����� ��ư
    /// </summary>
    public GameObject pause_button;
    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public Button acceleratorButton;
    /// <summary>
    /// ���� ������ �������ִ��� ����
    /// </summary>
    private bool onAccButton = false;
    /// <summary>
    /// ������ ���� �������� ����
    /// </summary>
    public bool isAccerlate = false;
    /// <summary>
    /// �߻�뿡�� ����ߴ��� ����
    /// </summary>
    public bool isStart = false;
    /// <summary>
    /// �߻� ���� ��ư �̹���
    /// </summary>
    public Image launchLever_img;
    /// <summary>
    /// �߻� ���� ��������Ʈ �̹��� �迭
    /// </summary>
    public Sprite[] launchLever_sprites;

    /// <summary>
    /// ù �߰� ���� �ð�
    /// </summary>
    public float firstAccerlateTime;
    /// <summary>
    /// ù �߰� ���� ���� �ð�
    /// </summary>
    public float decreasingTime;
    /// <summary>
    /// �ǽð� ���� ��·�
    /// </summary>
    public float realtimePower;
    /// <summary>
    /// ù �߰� ���� ���� ����
    /// </summary>
    public float firstAclPower;
    /// <summary>
    /// ������ ���� ��·�
    /// </summary>
    public float stdPower;
    /// <summary>
    /// ��·� ���� �����̴�
    /// </summary>
    public Slider power_slider;
    /// <summary>
    /// �ν��� �Ŀ� �ɷ�ġ
    /// </summary>
    public float boosterPowerAbility;

    /// <summary>
    /// ���� ȸ�� ��
    /// </summary>
    public float rotateForce;
    /// <summary>
    /// ȸ�� ������ ����
    /// </summary>
    public bool isRotating = false;
    /// <summary>
    /// ȸ�� ���� ����
    /// </summary>
    public bool allowRotate = true;
    /// <summary>
    /// ���� ȸ�� �ڷ�ƾ
    /// </summary>
    private IEnumerator rotateRocket;

    /// <summary>
    /// �� �ܰ躰 �߷·�
    /// </summary>
    public float[] gravityPortion;
    /// <summary>
    /// �� ����ġ
    /// </summary>
    public float[] heightCriteria;

    /// <summary>
    /// ���� ȭ�� ȿ����
    /// </summary>
    public AudioSource rocketFire_AudioSource;
    /// <summary>
    /// ���� ȭ�� ȿ���� ���� ����
    /// </summary>
    public float rocketFireAudioVolumeRatio = 1f;
    /// <summary>
    /// ���� ȭ�� ȿ���� Ŭ�� �迭
    /// </summary>
    public AudioClip[] rocketFires;
    /// <summary>
    /// ù �߻� ȿ�������� ����
    /// </summary>
    private bool isFireSoundPlay = false;

    /// <summary>
    /// �º� ��ư
    /// </summary>
    public GameObject tablet_button;
    /// <summary>
    /// ������ ��� ��ư
    /// </summary>
    public Button item_button;
    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public GameObject shot_button;
    /// <summary>
    /// ���� �ٱ�� ȿ����
    /// </summary>
    public AudioSource leverAudio;
    /// <summary>
    /// ���� ���� ȿ����
    /// </summary>
    public AudioSource fuelingAudio;
    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource explosionAudio;

    private void Start()
    {
        if (instance == null)
            instance = this;
        // ����, �ǽð� ��·� ���忡 ���� �ʱ�ȭ
        stdPower = realtimePower = RocketSet.instance.rocketData.MaxPower[0];
        // ���� ���� ���� �ʱ�ȭ
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;
        if(RocketSet.instance.launcher_rigid != null)
            RocketSet.instance.launcher_rigid.bodyType = RigidbodyType2D.Kinematic;
        
        SetFuelStageText();
    }

    /// <summary>
    /// ���� ���� ��ư ������
    /// </summary>
    public void SetLaunchMode()
    {
        // �߻� �غ� �Ϸ� �Ǿ����� Ȯ��
        if (launcherSettings.isSettingCompleted)
        {
            // ���� ���� ��ư ���¿� ���� �ø��ų� ������
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
    /// ���� ���� ������ (���� on)
    /// </summary>
    public void AcceleratorDown()
    {
        // ���� ��ư ���� ����
        onAccButton = true;
        // ������� ���� �����̸�
        if(!isStart)
        {
            // �ݺ� ��� ���� ȿ���� ����
            launcherSettings.StopRepeatAudio();
            // ���� �߻� ȿ���� ���
            PlayRocketFireSound();
            // ���� �߻� ȿ�� ���
            RocketSet.instance.FireEffectPlay();
            // �������� ���� ��, ���� ���� ���(����) ó��
            if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
                itemManager.UseItem(ItemManager.ItemType.VernierEngine);
            // �߻� ����ũ ȿ�� ���
            RocketSet.instance.launchSmokeEffect.Play();
            // �º� ��ư ��Ȱ��ȭ
            tablet_button.SetActive(false);
            // ������ �������� ���� ��� (�������� ����) ��ư ������ ���� �� Ȱ��ȭ
            if (ItemManager.equippedItem != null && ItemManager.equippedItem != ItemManager.ItemType.VernierEngine)
            {
                item_button.gameObject.SetActive(true);
                item_button.image.sprite = itemManager.items[(int)ItemManager.equippedItem].IconSprite;
            }
            // ������ ���Ⱑ ���� ���, �߻� ��ư Ȱ��ȭ
            if(EquipmentManager.equippedWeaponIndex != -1)
                shot_button.SetActive(true);
            // ù �߻� ȿ�� ���� �ڷ�ƾ ����
            StartCoroutine(FirstLaunchEffect(1.5f));
        }
        // �߻� ���Ŀ� ������ �ٽ� ������ ���
        else
        {
            // ���� ���ᷮ Ȯ�� ��, ���� ó��
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
    /// ���� ���� �ø��� (���� off)
    /// </summary>
    public void AcceleratorUp()
    {
        // ���� �߻� ȿ���� ����
        StopRocketFireSound();
        // ���� ���� ����
        onAccButton = false;
        // �߷� ����
        RocketSet.instance.rocketMain_rigid.gravityScale = 1f;
        // ���� ���� ��Ȱ��ȭ
        isAccerlate = false;
        
        // (�÷��̾� ����)ȸ�� �Ұ� ���·� ����
        RotateRocketOff();
        RocketSet.instance.FireEffectStop();
        // ���� ȿ���� ���
        if (PlayManager.isSurvive)
            leverAudio.Play();
    }

    /// <summary>
    /// ���� ���� ȿ���� ���
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
    /// 2�� ���� Ŭ�� ����� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator WaitForFirstClip()
    {
        // ���� �Ͻ������ų� �̹� ��� ���� �� ���
        while (rocketFire_AudioSource.isPlaying || TabletManager.isPause)
            yield return new WaitForEndOfFrame();

        // ����� �㰡�Ǿ��� �� ���
        if (isFireSoundPlay)
        {
            rocketFire_AudioSource.clip = rocketFires[1];
            rocketFire_AudioSource.loop = true;
            rocketFire_AudioSource.Play();
        }
    }
    /// <summary>
    /// ���� ȿ���� �ߴ�
    /// </summary>
    public void StopRocketFireSound()
    {
        isFireSoundPlay = false;
        rocketFire_AudioSource.Stop();
    }
    /// <summary>
    /// ���� ȭ�� ũ�� ����ȭ
    /// </summary>
    public void SyncRocketFire()
    {
        float ratio = power_slider.value / power_slider.maxValue;
        RocketSet.instance.rocketFire.FireSizeSync(ratio);
    }
    /// <summary>
    /// ���� ó�� �ڷ�ƾ
    /// </summary>
    IEnumerator Accerlating()
    {
        yield return new WaitForFixedUpdate();

        // �Ͻ������� �ƴ� �������� Ȯ��
        if (!TabletManager.isPause)
        {
            // ���� ���� ���ᷮ Ȯ��
            if (RocketSet.instance.FuelRemain > 0)
            {
                // �ν��� ���°� �ƴϸ� �������� ���� ��·� ���
                if (!itemManager.boosterOn)
                {
                    if (power_slider.interactable)
                        realtimePower = stdPower * power_slider.value;
                }
                // �ν����� �� �ִ� ��� x �ν��� �Ŀ��� ���� ���
                else
                    realtimePower = stdPower * boosterPowerAbility;
                // ������ ����
                RocketSet.instance.rocketMain_rigid.AddForce(RocketSet.instance.rocket.transform.up * realtimePower);
                // ���� ȿ�� ��� �� ȿ���� ���
                SetAtmosphereEffect();
                SetRocketFireVolume();
                // ������ ����� �� ��·� �����̴� ���� �����ϵ��� ����
                if(isOutsideEarth())
                    if (statManager.GetLevel(StatType.Accelerate).Equals(1))
                        power_slider.interactable = true;
                // ���� �Һ�
                ConsumeFuel(Time.deltaTime);
            }
        }

        if(isAccerlate && RocketSet.instance.FuelRemain > 0)
            StartCoroutine(Accerlating());
        // ���Ḧ ��� �����ϸ� ���� ���� �ø���, �Ͻ����� ��ư ��Ȱ��ȭ
        if (RocketSet.instance.FuelRemain <= 0)
        {
            AcceleratorUp();
            pause_button.SetActive(false);
        }
    }

    /// <summary>
    /// ���� ���� �� ���� ���� ȿ�� ���
    /// </summary>
    private void SetAtmosphereEffect()
    {
        // ���Ͽ� ���� ȿ�� ����Ʈ�� ���� ��
        if(RocketSet.instance.atmosphereEffect != null)
        {
            // ����� ������� ���� ��
            if (BackgroundControl.isBackgroundFollow)
            {
                // ������ ���� ����Ͽ� Ư�� ������ ������ ���� ���� ȿ�� ���
                float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
                if (heightCriteria[1] - 75 <= rocketY && rocketY <= heightCriteria[1] + 40)
                    RocketSet.instance.atmosphereEffect.Play();
                // �� �̻����� �ö󰡸� �ߴ�
                else
                {
                    if (RocketSet.instance.atmosphereEffect.isPlaying)
                        RocketSet.instance.atmosphereEffect.Stop();
                }
            }
        }
    }

    /// <summary>
    /// ��·� �Ϲ� �Ѱ� ó��
    /// </summary>
    public void PowerUnderLimit()
    {
        if (power_slider.value < 0.1f)
            power_slider.value = 0.1f;
    }
    /// <summary>
    /// �Ŀ��� ���� �� �߷� ��� �߰�
    /// </summary>
    public void PowerUnderPenalty()
    {
        if (power_slider.value < 0.5f)
            RocketSet.instance.rocketMain_rigid.gravityScale = 0.5f;
        else
            RocketSet.instance.rocketMain_rigid.gravityScale = 0f;
    }
    
    /// <summary>
    /// ù �߻� ȿ�� ó�� �ڷ�ƾ
    /// </summary>
    /// <param name="time">��� �ð�</param>
    IEnumerator FirstLaunchEffect(float time)
    {
        // ó���� ī�޶� �� õõ�� ��������� ����
        cameraFollow.followSpeed = 0.1f;
        cameraFollow.StartFollow();
        yield return new WaitForSeconds(time);

        // ���� ���� Ÿ�� ���� �� ù ���� ����
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Dynamic;
        StartFirstAccerlate();
        // �߻� ����ũ ȿ�� �ߴ�
        RocketSet.instance.launchSmokeEffect.Stop();
        // �ǽð� ��·� ��� �� ī�޶� �ȷο� ���ǵ� ����
        realtimePower = stdPower * power_slider.value;
        cameraFollow.followSpeed = 0.7f;
        pause_button.SetActive(true);
        // ��� ó��
        isStart = true;
        if (RocketSet.instance.launcher_rigid != null)
        {
            RocketSet.instance.launcher_rigid.bodyType = RigidbodyType2D.Dynamic;
            StartCoroutine(DisableLauncher(5f));
        }
        // ���� ���� Ʈ����
        if (onAccButton)
            AcceleratorDown();
        else
            AcceleratorUp();
    }
    /// <summary>
    /// ù ���� ó�� ����
    /// </summary>
    private void StartFirstAccerlate()
    {
        // �ʹ� �߰� ���ӷ� ���(���� ��)
        realtimePower *= firstAclPower;
        StartCoroutine(FirstAccerlatingForce(decreasingTime, firstAccerlateTime, firstAclPower));
    }
    /// <summary>
    /// �ʹ� �߰� ���ӷ� ���� �ڷ�ƾ
    /// </summary>
    /// <param name="deltaTime">���� �ð�</param>
    /// <param name="remainTime">���� �ð�</param>
    /// <param name="power">�߰��� �Ŀ� ����</param>
    IEnumerator FirstAccerlatingForce(float deltaTime, float remainTime, float power)
    {
        yield return new WaitForSeconds(deltaTime);

        // ���� �ð��� ����Ͽ� �߰� ���ӷ� ����
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
    /// �߻� �� �߻�� �̹��� ��Ȱ��ȭ ��� �ڷ�ƾ
    /// </summary>
    /// <param name="time">��� �ð�</param>
    IEnumerator DisableLauncher(float time)
    {
        yield return new WaitForSeconds(time);

        RocketSet.instance.launcher_rigid.gameObject.SetActive(false);
    }

    /// <summary>
    /// �÷��̾� �������� ���� ȸ��
    /// </summary>
    /// <param name="isLeft">�������� ȸ������ ����</param>
    public void RotateRocketOn(bool isLeft)
    {
        // ȸ���� �����ϸ�, �������� ��
        if (allowRotate && isAccerlate)
        {
            // ���� ������ ������ ��
            if (isOutsideEarth())
            {
                // ȸ�� ����
                rotateRocket = RotateRocket(isLeft);
                isRotating = true;
                StartCoroutine(rotateRocket);
            }
        }
    }

    /// <summary>
    /// �÷��̾��� ȸ�� ���� ����
    /// </summary>
    public void RotateRocketOff()
    {
        // ���� ���� ��
        if (isOutsideEarth())
        {
            // ȸ�� �ߴ�
            isRotating = false;
            if(rotateRocket != null)
                StopCoroutine(rotateRocket);
            StartCoroutine(ResetFireRotate());
        }
    }

    /// <summary>
    /// �÷��̾� ������ ���� ȸ�� �ڷ�ƾ
    /// </summary>
    /// <param name="isLeft">ȸ�� ������ �������� ����</param>
    /// <param name="rotateTime">ȸ�� �ð�</param>
    IEnumerator RotateRocket(bool isLeft, float rotateTime = 0f)
    {
        yield return new WaitForEndOfFrame();

        rotateTime += Time.deltaTime;

        // ���⿡ ���� ������ Ư�� ��ġ�� �������� ȸ�� �� ����
        if (isLeft)
        {
            RocketSet.instance.rocketMain_rigid.AddForceAtPosition(Vector2.left * rotateForce * 0.01f, new Vector2(0, RocketSet.instance.rocket_rends[0].bounds.max.y), ForceMode2D.Impulse);
        }
        else
        {
            RocketSet.instance.rocketMain_rigid.AddForceAtPosition(Vector2.right * rotateForce * 0.01f, new Vector2(0, RocketSet.instance.rocket_rends[0].bounds.max.y), ForceMode2D.Impulse);
        }

        // ȸ�� �ð��� 0.25�� ������ �� ������ ȸ�� ����
        if (rotateTime < 0.25f)
        {
            rotateRocket = RotateRocket(isLeft, rotateTime);
            StartCoroutine(rotateRocket);
        }
        // 0.25�ʰ� ������ �� ȸ�� ����
        else
            RotateRocketOff();
    }
    /// <summary>
    /// ȸ���� �� ���� ȸ���� ȭ�� ȿ���� ���� �������� ������Ű�� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetFireRotate()
    {
        yield return new WaitForEndOfFrame();

        // ���� �ӵ�
        float speed = 3f;
        float z = RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z;
        // ȸ���� ���⿡ ���� ȸ�� ���� ��� �� ȸ�� ó��
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
        // ���� ������ ���ƿ��� �� ������ 0���� ó��
        else
        {
            RocketSet.instance.fireEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// ȿ�� � ��� �ǽð� ��·����� ��� ȿ�� ���
    /// </summary>
    /// <returns>�Ŀ� ȿ�� ��ġ</returns>
    private float GetPowerEfficiencyByPower()
    {
        float powerPercentage = realtimePower / stdPower * 100f;
        float result = (RocketSet.instance.rocketData.A * powerPercentage * powerPercentage + RocketSet.instance.rocketData.B * powerPercentage + RocketSet.instance.rocketData.C) * 0.01f;
        return result * realtimePower;
    }

    /// <summary>
    /// ���� �Һ�
    /// </summary>
    /// <param name="deltaTime">�帥 �ð�</param>
    private void ConsumeFuel(float deltaTime)
    {
        // �ʴ� ���� �Һ� ��� �� �帥 �ð��� ����Ͽ� ���� �Һ� ó��
        int currentStage = RocketSet.instance.currentStage;
        float consumePsec = (RocketSet.instance.rocketMain_rigid.mass * GetGravityPortion() + GetPowerEfficiencyByPower() * powerEfficiency) * 0.02f / RocketSet.instance.rocketData.FuelEfficienty;
        
        RocketSet.instance.FuelRemain -= consumePsec * deltaTime;
        if (currentStage != RocketSet.instance.currentStage)
            SetFuelStageText();
        if(!isSupplying)
            SetFuelGage();
    }

    /// <summary>
    /// ���� ������ ������Ʈ
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
    /// ���� ����
    /// </summary>
    /// <param name="value">���淮</param>
    public void SupplyFuel(float value)
    {
        isSupplying = true;
        fuelMark.color = supplyColor;
        fuelingAudio.Play();
        StartCoroutine(SupplyFuelEffect(value));
    }
    /// <summary>
    /// ���� ���� ȿ�� �ڷ�ƾ
    /// </summary>
    /// <param name="value">���淮</param>
    IEnumerator SupplyFuelEffect(float value)
    {
        yield return new WaitForEndOfFrame();

        // ���� ���� �ӵ��� ���� �����Ӻ� ���淮 ���
        value += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * supplyingSpeed * Time.deltaTime;
        // ���� ���� ��� �� ������ �ٴ� ���� ��� �� ����
        float percentage = value / RocketSet.instance.fuelMax[RocketSet.instance.currentStage];
        float rotation = percentage * 110 - 55;
        rotation = Mathf.Clamp(rotation, -55f, 55f);
        fuelGage_axis.transform.rotation = Quaternion.Euler(0, 0, rotation);

        // ���� ������ ������ ���� ����Ʈ ����
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
    /// ���� ���� ���� �߷� ���� ���
    /// </summary>
    /// <returns>�߷� ����</returns>
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
    /// ���� ȭ�� ȿ���� ���� ����
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
    /// ������ ������ ������� ����
    /// </summary>
    /// <returns>���� Ż�� ����</returns>
    private bool isOutsideEarth()
    {
        float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
        if (rocketY < heightCriteria[1])
            return true;//false
        else
            return true;
    }

    /// <summary>
    /// ���� �Ҹ� ���� ������ �� ���� ������Ʈ
    /// </summary>
    public void SetFuelStageText()
    {
        fuelStage_text.text = GetStageAbbrev();
    }

    /// <summary>
    /// ������ �� ���� ���� ���·� ����
    /// </summary>
    /// <returns>���� �� ��(���)</returns>
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