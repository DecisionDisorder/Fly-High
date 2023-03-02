using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �ý��� ���� Ŭ����
/// </summary>
public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public Weapon weapon;
    /// <summary>
    /// ���� źȯ ���� ǥ���ϴ� �ؽ�Ʈ
    /// </summary>
    public Text remainBullet_text;
    /// <summary>
    /// �߻� ���� ����
    /// </summary>
    private bool shootMode = false;
    /// <summary>
    /// ������ �ɷ�ġ
    /// </summary>
    public float reloadAbility = 1.0f;
    /// <summary>
    /// ���� �߻� ������ Ÿ�̸� �̹���
    /// </summary>
    public Image delayTimer_image;
    /// <summary>
    /// ���� ���� ȿ����
    /// </summary>
    public AudioSource weaponAudio;
    /// <summary>
    /// ������ ȿ����
    /// </summary>
    public AudioSource reloadAudio;
    /// <summary>
    /// ������ ȿ���� Ŭ�� �迭
    /// </summary>
    public AudioClip[] reloadClips;

    /// <summary>
    /// źȯ ǥ�� �ؽ�Ʈ ���� �迭
    /// </summary>
    public Color[] bulletColors = new Color[3];

    private void Start()
    {
        if(remainBullet_text != null)
            UpdateRemainBullet();
    }

    /// <summary>
    /// �߻� ������ ó��
    /// </summary>
    /// <param name="on">�߻� ���� ����</param>
    public void ShotOnOff(bool on)
    {
        shootMode = on;
        if(shootMode)
        {
            // ���� RPM ���忡 ���� ������ �ð� ��� �� ������ �ڷ�ƾ ����
            float delay = 60f / (weapon.weaponSpecData.RPM * reloadAbility);
            StartCoroutine(RepeatShot(delay));
        }
    }

    /// <summary>
    /// ���� źȯ ���� ���� ������Ʈ
    /// </summary>
    public void UpdateRemainBullet()
    {
        if (remainBullet_text != null)
        {
            if (weapon != null)
            {
                // ���� źȯ�� ������ ���� �ؽ�Ʈ ���� ����
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

    /// <summary>
    /// ���� ó�� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">�߻� ������</param>
    IEnumerator RepeatShot(float delay)
    {
        // ������ źȯ�� ���� ��
        if (weapon.loadedBullet != null)
        {
            // ���� ���� �� ȿ���� ���, ���� źȯ ���� ������Ʈ
            weapon.Shot();
            UpdateRemainBullet();
            weaponAudio.Play();
            // ���� ������ Ÿ�̸� �� ������ ���� ��� �ڷ�ƾ ����
            StartCoroutine(ShotDelayTimer(delay));
            StartCoroutine(ReloadSound(0.2f, delay));

            yield return new WaitForSeconds(delay);

            // ������
            weapon.ReloadBullet();
        }
        else
            yield return null;

        // �߻� ��ư�� ��� ������ ���� �� �ݺ�
        if (shootMode)
        {
            StartCoroutine(RepeatShot(delay));
        }
    }
    /// <summary>
    /// ������ ȿ���� ��� �� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��� ���ð�</param>
    /// <param name="reloadTime">������ �ҿ�ð�</param>
    IEnumerator ReloadSound(float delay, float reloadTime)
    {
        yield return new WaitForSeconds(delay);

        if (reloadTime > 1)
            reloadAudio.clip = reloadClips[1];
        else
            reloadAudio.clip = reloadClips[0];

        reloadAudio.Play();
    }

    /// <summary>
    /// ���� ������ Ÿ�̸�
    /// </summary>
    /// <param name="fullTime">�� ���ð�</param>
    /// <param name="passedTime">���� ���ð�</param>
    /// <returns></returns>
    IEnumerator ShotDelayTimer(float fullTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        passedTime += Time.deltaTime;

        // �߻� ������ Ÿ�̸� �̹��� ������Ʈ
        delayTimer_image.fillAmount = passedTime / fullTime;


        if (passedTime < fullTime)
        {
            StartCoroutine(ShotDelayTimer(fullTime, passedTime));
        }
        else
            reloadAudio.Stop();
    }
}

/// <summary>
/// ���� ��ġ ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class WeaponPosition
{
    /// <summary>
    /// ���� ����
    /// </summary>
    public WeaponType weaponType;
    /// <summary>
    /// ������ ����� ��ġ
    /// </summary>
    public Vector2 position;

    public int GetWeaponType()
    {
        return (int)weaponType;
    }
}