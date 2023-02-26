using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    /* prototype variable */
    public Weapon weapon;
    public Text remainBullet_text;
    private bool shootMode = false;
    public float reloadAbility = 1.0f;
    public Image delayTimer_image;
    public AudioSource weaponAudio;
    public AudioSource reloadAudio;
    public AudioClip[] reloadClips;

    public Color[] bulletColors = new Color[3];

    private void Start()
    {
        if(remainBullet_text != null)
            UpdateRemainBullet();
    }

    public void ShotOnOff(bool on)
    {
        shootMode = on;
        if(shootMode)
        {
            float delay = 60f / (weapon.weaponSpecData.RPM * reloadAbility);
            StartCoroutine(RepeatShot(delay));
        }
    }

    public void UpdateRemainBullet()
    {
        if (remainBullet_text != null)
        {
            if (weapon != null)
            {
                int bulletLeft = weapon.weaponSpecData.weaponData.numOfBulletLeft;
                if (100 * bulletLeft / weapon.weaponSpecData.BulletCapacity > 30)
                    remainBullet_text.color = bulletColors[0];
                else if (bulletLeft > 1)
                    remainBullet_text.color = bulletColors[1];
                else
                    remainBullet_text.color = bulletColors[2];
                remainBullet_text.text = bulletLeft.ToString();
            }
            else
                remainBullet_text.text = "X";
        }
    }

    IEnumerator RepeatShot(float delay)
    {
        if (weapon.loadedBullet != null)
        {
            weapon.Shot();
            UpdateRemainBullet();
            weaponAudio.Play();
            StartCoroutine(ShotDelayTimer(delay));
            StartCoroutine(ReloadSound(0.2f, delay));

            yield return new WaitForSeconds(delay);

            weapon.ReloadBullet();
        }
        else
            yield return null;

        if (shootMode)
        {
            StartCoroutine(RepeatShot(delay));
        }
    }

    IEnumerator ReloadSound(float delay, float reloadTime)
    {
        yield return new WaitForSeconds(delay);

        if (reloadTime > 1)
            reloadAudio.clip = reloadClips[1];
        else
            reloadAudio.clip = reloadClips[0];

        reloadAudio.Play();
    }

    IEnumerator ShotDelayTimer(float fullTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        passedTime += Time.deltaTime;

        delayTimer_image.fillAmount = passedTime / fullTime;


        if (passedTime < fullTime)
        {
            StartCoroutine(ShotDelayTimer(fullTime, passedTime));
        }
        else
            reloadAudio.Stop();
    }
}

[System.Serializable]
public class WeaponPosition
{
    public WeaponType weaponType;
    public Vector2 position;

    public int GetWeaponType()
    {
        return (int)weaponType;
    }
}