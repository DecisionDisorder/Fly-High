using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 클래스 
/// </summary>
[System.Serializable]
public class Weapon: MonoBehaviour
{
    /// <summary>
    /// 탄환 발사 기준 위치
    /// </summary>
    public GameObject firePos;
    /// <summary>
    /// 탄환 모델 프리팹
    /// </summary>
    public Bullet bulletPrefab;
    /// <summary>
    /// 장전된 탄환
    /// </summary>
    public Bullet loadedBullet;
    /// <summary>
    /// 탄환 스폰 정보 프리셋
    /// </summary>
    public Spawnable bulletSpawnable;

    /// <summary>
    /// 무기 스펙 데이터
    /// </summary>
    public WeaponSpecData weaponSpecData;
    /// <summary>
    /// 발포 전 스프라이트 이미지 리소스
    /// </summary>
    private Sprite beforeShot_sprite;
    /// <summary>
    /// 발포 후 스프라이트 이미지 리소스
    /// </summary>
    public Sprite afterShot_sprite;
    /// <summary>
    /// 무기 스프라이트 렌더러
    /// </summary>
    public SpriteRenderer spriteRenderer;
    /// <summary>
    /// 무기 발사 효과음 클립
    /// </summary>
    public AudioClip weaponClip;
    /// <summary>
    /// 무기 궤적 라인 렌더러
    /// </summary>
    public LineRenderer shotLinerenderer;

    private void Start()
    {
        if (bulletSpawnable == null)
            Initialize();
        StartCoroutine(Draw());
    }

    /// <summary>
    /// 탄환 스폰정보 및 무기 transform 초기화
    /// </summary>
    public void Initialize()
    {
        bulletSpawnable = new Spawnable(bulletPrefab.gameObject, firePos.transform, firePos.transform.position);
        transform.SetParent(RocketSet.instance.weaponParent);
        transform.localPosition = RocketSet.instance.rocketData.GetWeaponPositionByIndex(EquipmentManager.equippedWeaponIndex).position;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// 탄환 재장전
    /// </summary>
    public void ReloadBullet()
    {
        // 남는 탄환이 있을 때
        if (weaponSpecData.weaponData.numOfBulletLeft > 0)
        {
            // 발사 전/후 이미지가 따로 있는 경우만 무기 스프라이트 변경
            if (afterShot_sprite != null && beforeShot_sprite != null)
            {
                spriteRenderer.sprite = beforeShot_sprite;
            }

            // 탄환 위치 조정 및 소환
            bulletSpawnable.spawnPos = firePos.transform.position + Vector3.forward;
            GameObject bulletObj = SpawnManager.instance.SpawnObject(bulletSpawnable);
            loadedBullet = bulletObj.GetComponent<Bullet>();
        }
    }
    
    /// <summary>
    /// 무기 발포
    /// </summary>
    public void Shot()
    {
        // 장전된 총알이 있을 때
        if(loadedBullet != null)
        {
            // 장전된 총알을 발사하고, 남은 탄환 개수 차감
            loadedBullet.Shot();
            weaponSpecData.weaponData.numOfBulletLeft--;
            // 장전된 총알 없음으로 처리
            loadedBullet = null;

            //발사 전/후 이미지가 따로 있는 경우만 이미지 교체
            if(afterShot_sprite != null)
            {
                beforeShot_sprite = spriteRenderer.sprite;
                spriteRenderer.sprite = afterShot_sprite;
            }
        }
    }

    /// <summary>
    /// 무기 궤적 선 그리는 코루틴
    /// </summary>
    IEnumerator Draw()
    {
        yield return new WaitForFixedUpdate();

        DrawShotLine();

        StartCoroutine(Draw());
    }

    /// <summary>
    /// 무기 궤적 그리기
    /// </summary>
    public void DrawShotLine()
    {
        shotLinerenderer.SetPosition(0, shotLinerenderer.transform.position);
        shotLinerenderer.SetPosition(1, shotLinerenderer.transform.position + shotLinerenderer.transform.up * 20);
    }

    /// <summary>
    /// 본 무기를 가지고 있는지 여부를 반환
    /// </summary>
    /// <returns>본 무기 소유 여부</returns>
    public bool GetHasItem()
    {
        return weaponSpecData.weaponData.hasItem;
    }   
}
/// <summary>
/// 무기 저장 데이터
/// </summary>
[System.Serializable]
public class WeaponData : Item
{
    /// <summary>
    /// 남은 탄환 수
    /// </summary>
    public int numOfBulletLeft;
}