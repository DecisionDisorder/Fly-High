using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Launch : MonoBehaviour
{
    public static Launch instance;
    public CameraFollow cameraFollow;
    public StatManager statManager;
    public LauncherSettings launcherSettings;
    public ItemManager itemManager;
    public Settings settings;
    public WeaponManager weaponManager;
    
    public GameObject fuelGage_axis;
    public Image fuelMark;
    public Color lowColor;
    public Color emptyColor;
    public Color supplyColor;
    public float supplyingSpeed = 0.4f;
    private bool isSupplying = false;
    public Text fuelStage_text;
    public float powerEfficiency = 1f; //연비향상 레벨 상승으로 조절

    public GameObject pause_button;
    public Button acceleratorButton;
    private bool onAccButton = false;
    public bool isAccerlate = false;
    public bool isStart = false;
    public Image launchLever_img;
    public Sprite[] launchLever_sprites;

    public float firstAccerlateTime;
    public float decreasingTime;
    public float realtimePower;
    public float firstAclPower;
    public float stdPower;
    public Slider power_slider;
    public float boosterPowerAbility;

    public float rotateForce;
    public bool isRotating = false;
    public bool allowRotate = true;
    private IEnumerator rotateRocket;

    public float[] gravityPortion;
    public float[] heightCriteria;


    public AudioSource rocketFire_AudioSource;
    public float rocketFireAudioVolumeRatio = 1f;
    public AudioClip[] rocketFires;
    private bool isFireSoundPlay = false;

    public GameObject tablet_button;
    public Button item_button;
    public GameObject shot_button;
    public AudioSource leverAudio;
    public AudioSource fuelingAudio;
    public AudioSource explosionAudio;

    private void Start()
    {
        if (instance == null)
            instance = this;
        stdPower = realtimePower = RocketSet.instance.rocketData.MaxPower[0];
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;
        if(RocketSet.instance.launcher_rigid != null)
            RocketSet.instance.launcher_rigid.bodyType = RigidbodyType2D.Kinematic;
        SetFuelStageText();
    }

    public void SetLaunchMode()
    {
        if (launcherSettings.isSettingCompleted)
        {
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

    public void AcceleratorDown()
    {
        onAccButton = true;
        if(!isStart)
        {
            launcherSettings.StopRepeatAudio();
            PlayRocketFireSound();
            //RocketSet.instance.fire.SetActive(true);
            RocketSet.instance.FireEffectPlay();
            if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
                itemManager.UseItem(ItemManager.ItemType.VernierEngine);
            RocketSet.instance.launchSmokeEffect.Play();
            tablet_button.SetActive(false);
            if (ItemManager.equippedItem != null && ItemManager.equippedItem != ItemManager.ItemType.VernierEngine)
            {
                item_button.gameObject.SetActive(true);
                item_button.image.sprite = itemManager.items[(int)ItemManager.equippedItem].IconSprite;
            }
            if(EquipmentManager.equippedWeaponIndex != -1)
                shot_button.SetActive(true);
            StartCoroutine(FirstLaunchEffect(1.5f));
        }
        else
        {
            if (RocketSet.instance.FuelRemain > 0)
            {
                PlayRocketFireSound();
                RocketSet.instance.rocketMain_rigid.gravityScale = 0f;
                isAccerlate = true;
                //RocketSet.instance.fire.SetActive(true);
                RocketSet.instance.FireEffectPlay();
                StartCoroutine(Accerlating());
                leverAudio.Play();
            }
        }
    }

    public void AcceleratorUp()
    {
        StopRocketFireSound();
        onAccButton = false;
        RocketSet.instance.rocketMain_rigid.gravityScale = 1f;
        isAccerlate = false;
        //RocketSet.instance.fire.SetActive(false); 
        RotateRocketOff();
        RocketSet.instance.FireEffectStop();
        if (PlayManager.isSurvive)
            leverAudio.Play();
    }

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
    IEnumerator WaitForFirstClip()
    {
        while (rocketFire_AudioSource.isPlaying || TabletManager.isPause)
            yield return new WaitForEndOfFrame();

        if (isFireSoundPlay)
        {
            rocketFire_AudioSource.clip = rocketFires[1];
            rocketFire_AudioSource.loop = true;
            rocketFire_AudioSource.Play();
        }
    }
    public void StopRocketFireSound()
    {
        isFireSoundPlay = false;
        rocketFire_AudioSource.Stop();
    }
    public void SyncRocketFire()
    {
        float ratio = power_slider.value / power_slider.maxValue;
        RocketSet.instance.rocketFire.FireSizeSync(ratio);
    }
    IEnumerator Accerlating()
    {
        yield return new WaitForFixedUpdate();

        if (!TabletManager.isPause)
        {
            if (RocketSet.instance.FuelRemain > 0)
            {
                if (!itemManager.boosterOn)
                {
                    if (power_slider.interactable)
                        realtimePower = stdPower * power_slider.value;
                }
                else
                    realtimePower = stdPower * boosterPowerAbility;
                RocketSet.instance.rocketMain_rigid.AddForce(RocketSet.instance.rocket.transform.up * realtimePower);
                SetAtmosphereEffect();
                SetRocketFireVolume();
                if(isOutsideEarth())
                    if (statManager.GetLevel(StatType.Accelerate).Equals(1))
                        power_slider.interactable = true;
                ConsumeFuel(Time.deltaTime);
            }
        }

        if(isAccerlate && RocketSet.instance.FuelRemain > 0)
            StartCoroutine(Accerlating());
        if (RocketSet.instance.FuelRemain <= 0)
        {
            AcceleratorUp();
            pause_button.SetActive(false);
        }
    }

    private void SetAtmosphereEffect()
    {
        if(RocketSet.instance.atmosphereEffect != null)
        {
            if (BackgroundControl.isBackgroundFollow)
            {
                float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
                if (heightCriteria[1] - 75 <= rocketY && rocketY <= heightCriteria[1] + 40)
                    RocketSet.instance.atmosphereEffect.Play();
                else
                {
                    if (RocketSet.instance.atmosphereEffect.isPlaying)
                        RocketSet.instance.atmosphereEffect.Stop();
                }
            }
        }
    }

    public void PowerUnderLimit()
    {
        if (power_slider.value < 0.1f)
            power_slider.value = 0.1f;
    }
    public void PowerUnderPenalty()
    {
        if (power_slider.value < 0.5f)
            RocketSet.instance.rocketMain_rigid.gravityScale = 0.5f;
        else
            RocketSet.instance.rocketMain_rigid.gravityScale = 0f;
    }
    

    IEnumerator FirstLaunchEffect(float time)
    {
        cameraFollow.followSpeed = 0.1f;
        cameraFollow.StartFollow();
        yield return new WaitForSeconds(time);

        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Dynamic;
        StartFirstAccerlate();
        RocketSet.instance.launchSmokeEffect.Stop();
        realtimePower = stdPower * power_slider.value;
        cameraFollow.followSpeed = 0.7f;
        pause_button.SetActive(true);
        isStart = true;
        if (RocketSet.instance.launcher_rigid != null)
        {
            RocketSet.instance.launcher_rigid.bodyType = RigidbodyType2D.Dynamic;
            StartCoroutine(DisableLauncher(5f));
        }
        if (onAccButton)
            AcceleratorDown();
        else
            AcceleratorUp();
    }

    private void StartFirstAccerlate()
    {
        realtimePower *= firstAclPower;
        StartCoroutine(FirstAccerlatingForce(decreasingTime, firstAccerlateTime, firstAclPower));
    }

    IEnumerator FirstAccerlatingForce(float deltaTime, float remainTime, float power)
    {
        yield return new WaitForSeconds(deltaTime);

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

    IEnumerator DisableLauncher(float time)
    {
        yield return new WaitForSeconds(time);

        RocketSet.instance.launcher_rigid.gameObject.SetActive(false);
    }

    public void RotateRocketOn(bool isLeft)
    {
        if (allowRotate && isAccerlate)
        {
            if (isOutsideEarth())
            {
                rotateRocket = RotateRocket(isLeft);
                isRotating = true;
                StartCoroutine(rotateRocket);
            }
        }
    }

    public void RotateRocketOff()
    {
        if (isOutsideEarth())
        {
            isRotating = false;
            if(rotateRocket != null)
                StopCoroutine(rotateRocket);
            StartCoroutine(ResetFireRotate());
        }
    }

    IEnumerator RotateRocket(bool isLeft, float rotateTime = 0f)
    {
        yield return new WaitForEndOfFrame();

        rotateTime += Time.deltaTime;

        if (isLeft)
        {
            RocketSet.instance.rocketMain_rigid.AddForceAtPosition(Vector2.left * rotateForce * 0.01f, new Vector2(0, RocketSet.instance.rocket_rends[0].bounds.max.y), ForceMode2D.Impulse);
        }
        else
        {
            RocketSet.instance.rocketMain_rigid.AddForceAtPosition(Vector2.right * rotateForce * 0.01f, new Vector2(0, RocketSet.instance.rocket_rends[0].bounds.max.y), ForceMode2D.Impulse);
        }

        if (rotateTime < 0.25f)
        {
            rotateRocket = RotateRocket(isLeft, rotateTime);
            StartCoroutine(rotateRocket);
        }
        else
            RotateRocketOff();
    }
    IEnumerator ResetFireRotate()
    {
        yield return new WaitForEndOfFrame();

        float speed = 3f;
        float z = RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z;
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
        else
        {
            RocketSet.instance.fireEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private float GetPowerEfficiencyByPower()
    {
        float powerPercentage = realtimePower / stdPower * 100f;
        float result = (RocketSet.instance.rocketData.A * powerPercentage * powerPercentage + RocketSet.instance.rocketData.B * powerPercentage + RocketSet.instance.rocketData.C) * 0.01f;
        return result * realtimePower;
    }

    private void ConsumeFuel(float deltaTime)
    {
        int currentStage = RocketSet.instance.currentStage;
        float consumePsec = (RocketSet.instance.rocketMain_rigid.mass * GetGravityPortion() + GetPowerEfficiencyByPower() * powerEfficiency) * 0.02f / RocketSet.instance.rocketData.FuelEfficienty;
        
        RocketSet.instance.FuelRemain -= consumePsec * deltaTime;
        if (currentStage != RocketSet.instance.currentStage)
            SetFuelStageText();
        if(!isSupplying)
            SetFuelGage();
    }

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

    public void SupplyFuel(float value)
    {
        isSupplying = true;
        fuelMark.color = supplyColor;
        fuelingAudio.Play();
        StartCoroutine(SupplyFuelEffect(value));
    }
    IEnumerator SupplyFuelEffect(float value)
    {
        yield return new WaitForEndOfFrame();

        value += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * supplyingSpeed * Time.deltaTime;
        float percentage = value / RocketSet.instance.fuelMax[RocketSet.instance.currentStage];
        float rotation = percentage * 110 - 55;
        rotation = Mathf.Clamp(rotation, -55f, 55f);
        fuelGage_axis.transform.rotation = Quaternion.Euler(0, 0, rotation);

        if (value >= RocketSet.instance.FuelRemain)
        {
            isSupplying = false;
            fuelMark.color = Color.white;
            fuelingAudio.Stop();
        }
        else
            StartCoroutine(SupplyFuelEffect(value));
    }

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

    private bool isOutsideEarth()
    {
        float rocketY = RocketSet.instance.rocket.transform.localPosition.y;
        if (rocketY < heightCriteria[1])
            return true;//false
        else
            return true;
    }

    public void SetFuelStageText()
    {
        fuelStage_text.text = GetStageAbbrev();
    }

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