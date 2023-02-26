using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RocketController : MonoBehaviour
{
    private bool isLaunched = false;
    public bool isAccerlate = false;
    public float stdPower;
    public float realtimePower;
    public Slider power_slider;

    private IEnumerator rotateRocket;
    public float rotateForce;
    public bool isRotating = false;

    public Image launchLever_img;
    public Sprite[] launchLever_sprites;
    public GameObject pauseButton;

    public AudioSource explosionAudio;

    public CameraFollow cameraFollow;

    private void Start()
    {
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetAccelerate()
    {
        if (!isAccerlate)
        {
            launchLever_img.sprite = launchLever_sprites[1];
            AccelerateOn();
        }
        else
        {
            launchLever_img.sprite = launchLever_sprites[0];
            AccelerateOff();
        }
    }
    public void AccelerateOn()
    {
        isAccerlate = true;
        RocketSet.instance.FireEffectPlay();
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Dynamic;
        //RocketSet.instance.rocketMain_rigid.gravityScale = 0f;
        cameraFollow.StartFollow();

        if (!isLaunched)
        {
            isLaunched = true;
            RocketSet.instance.rocketMain_rigid.AddForce(RocketSet.instance.rocket.transform.up, ForceMode2D.Impulse);
            pauseButton.SetActive(true);
        }

        StartCoroutine(Accerlating());
    }

    public void AccelerateOff()
    {
        isAccerlate = false;
        //RocketSet.instance.rocketMain_rigid.gravityScale = 1f;
        RocketSet.instance.FireEffectStop();
    }

    IEnumerator Accerlating()
    {
        yield return new WaitForFixedUpdate();

        realtimePower = stdPower * power_slider.value;
        RocketSet.instance.rocketMain_rigid.AddForce(RocketSet.instance.rocket.transform.up * realtimePower);

        if (isAccerlate)
            StartCoroutine(Accerlating());
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
    public void SyncRocketFire()
    {
        float ratio = power_slider.value / power_slider.maxValue;
        RocketSet.instance.rocketFire.FireSizeSync(ratio);
    }


    public void RotateRocketOn(bool isLeft)
    {
        if (isAccerlate)
        {
            rotateRocket = RotateRocket(isLeft);
            isRotating = true;
            StartCoroutine(rotateRocket);
        }
    }

    public void RotateRocketOff()
    {
        isRotating = false;
        if (rotateRocket != null)
            StopCoroutine(rotateRocket);
        StartCoroutine(ResetFireRotate());
    }

    IEnumerator RotateRocket(bool isLeft)
    {
        yield return new WaitForEndOfFrame();

        float speed = 2f;
        if (isLeft)
        {
            RocketSet.instance.rocketMain_rigid.rotation += rotateForce * Time.deltaTime;

            //Debug.Log(string.Format("local Rotatation: {0}, rotateForce: {1}", RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z, rotateForce));
            if (RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z > 360 - rotateForce * 0.5f || RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z.Equals(0))
            {
                RocketSet.instance.fireEffect.transform.Rotate(0, 0, -rotateForce * speed * Time.deltaTime);
            }
        }
        else
        {
            RocketSet.instance.rocketMain_rigid.rotation -= rotateForce * Time.deltaTime;

            //Debug.Log(string.Format("local Rotatation: {0}, rotateForce: {1}", RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z, rotateForce));
            if (RocketSet.instance.fireEffect.transform.localRotation.eulerAngles.z < rotateForce * 0.5f)
            {
                RocketSet.instance.fireEffect.transform.Rotate(0, 0, rotateForce * speed * Time.deltaTime);
            }
        }

        rotateRocket = RotateRocket(isLeft);
        StartCoroutine(rotateRocket);
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
}
