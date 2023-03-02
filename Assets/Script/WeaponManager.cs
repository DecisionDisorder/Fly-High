using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기 시스템 관리 클래스
/// </summary>
public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// 장착 중인 무기
    /// </summary>
    public Weapon weapon;
    /// <summary>
    /// 남은 탄환 수를 표기하는 텍스트
    /// </summary>
    public Text remainBullet_text;
    /// <summary>
    /// 발사 가능 여부
    /// </summary>
    private bool shootMode = false;
    /// <summary>
    /// 재장전 능력치
    /// </summary>
    public float reloadAbility = 1.0f;
    /// <summary>
    /// 무기 발사 딜레이 타이머 이미지
    /// </summary>
    public Image delayTimer_image;
    /// <summary>
    /// 무기 발포 효과음
    /// </summary>
    public AudioSource weaponAudio;
    /// <summary>
    /// 재장전 효과음
    /// </summary>
    public AudioSource reloadAudio;
    /// <summary>
    /// 재장전 효과음 클립 배열
    /// </summary>
    public AudioClip[] reloadClips;

    /// <summary>
    /// 탄환 표시 텍스트 색상 배열
    /// </summary>
    public Color[] bulletColors = new Color[3];

    private void Start()
    {
        if(remainBullet_text != null)
            UpdateRemainBullet();
    }

    /// <summary>
    /// 발사 딜레이 처리
    /// </summary>
    /// <param name="on">발사 가능 여부</param>
    public void ShotOnOff(bool on)
    {
        shootMode = on;
        if(shootMode)
        {
            // 무기 RPM 스펙에 따른 딜레이 시간 계산 후 딜레이 코루틴 시작
            float delay = 60f / (weapon.weaponSpecData.RPM * reloadAbility);
            StartCoroutine(RepeatShot(delay));
        }
    }

    /// <summary>
    /// 남은 탄환 개수 정보 업데이트
    /// </summary>
    public void UpdateRemainBullet()
    {
        if (remainBullet_text != null)
        {
            if (weapon != null)
            {
                // 남은 탄환의 비율에 따라서 텍스트 색상 변경
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
    /// 연사 처리 코루틴
    /// </summary>
    /// <param name="delay">발사 딜레이</param>
    IEnumerator RepeatShot(float delay)
    {
        // 장전된 탄환이 있을 때
        if (weapon.loadedBullet != null)
        {
            // 무기 발포 및 효과음 재생, 남은 탄환 정보 업데이트
            weapon.Shot();
            UpdateRemainBullet();
            weaponAudio.Play();
            // 발포 딜레이 타이머 및 재장전 사운드 대기 코루틴 시작
            StartCoroutine(ShotDelayTimer(delay));
            StartCoroutine(ReloadSound(0.2f, delay));

            yield return new WaitForSeconds(delay);

            // 재장전
            weapon.ReloadBullet();
        }
        else
            yield return null;

        // 발사 버튼을 계속 누르고 있을 때 반복
        if (shootMode)
        {
            StartCoroutine(RepeatShot(delay));
        }
    }
    /// <summary>
    /// 재장전 효과음 대기 후 재생 코루틴
    /// </summary>
    /// <param name="delay">재생 대기시간</param>
    /// <param name="reloadTime">재장전 소요시간</param>
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
    /// 발포 딜레이 타이머
    /// </summary>
    /// <param name="fullTime">총 대기시간</param>
    /// <param name="passedTime">지난 대기시간</param>
    /// <returns></returns>
    IEnumerator ShotDelayTimer(float fullTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        passedTime += Time.deltaTime;

        // 발사 딜레이 타이머 이미지 업데이트
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
/// 무기 위치 정보 데이터 클래스
/// </summary>
[System.Serializable]
public class WeaponPosition
{
    /// <summary>
    /// 무기 종류
    /// </summary>
    public WeaponType weaponType;
    /// <summary>
    /// 무기의 상대적 위치
    /// </summary>
    public Vector2 position;

    public int GetWeaponType()
    {
        return (int)weaponType;
    }
}