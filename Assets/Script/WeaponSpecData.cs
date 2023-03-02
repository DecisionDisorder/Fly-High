using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 스펙 데이터 관련 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Weapon Spec Data", menuName = "Scriptable Object/Weapon Spec Data", order = int.MaxValue)]
public class WeaponSpecData : ScriptableObject
{
    /// <summary>
    /// 무기 종류
    /// </summary>
    [SerializeField]
    public WeaponType weaponType;

    /// <summary>
    /// 무기의 아이템 데이터
    /// </summary>
    [SerializeField]
    public WeaponData weaponData;

    /// <summary>
    /// 무기의 스프라이트 이미지
    /// </summary>
    [SerializeField]
    public Sprite weaponSprite;

    /// <summary>
    /// 무기의 탄환 스프라이트 이미지
    /// </summary>
    [SerializeField]
    public Sprite bulletSprite;

    [SerializeField]
    private string bulletName;
    /// <summary>
    /// 탄환 이름
    /// </summary>
    public string BulletName { get { return bulletName; } }

    [SerializeField]
    private int bulletPrice;
    /// <summary>
    /// 탄환 가격
    /// </summary>
    public int BulletPrice { get { return bulletPrice; } }

    [SerializeField]
    private int bulletCapacity;
    /// <summary>
    /// 최대 소유 가능한 탄환 개수
    /// </summary>
    public int BulletCapacity { get { return bulletCapacity; } }

    [SerializeField]
    private int bulletForce;
    /// <summary>
    /// 탄환의 힘(속도)
    /// </summary>
    public int BulletForce { get { return bulletForce; } }

    [SerializeField]
    private int damage;
    /// <summary>
    /// 무기의 데미지
    /// </summary>
    public int Damage { get { return damage; } }
    
    [SerializeField]
    private int rpm;
    /// <summary>
    /// 무기의 연사 속도
    /// </summary>
    public int RPM { get { return rpm; } }

    /// <summary>
    /// 무기 발포 효과음 클립
    /// </summary>
    [SerializeField]
    public AudioClip weaponSound;
}
