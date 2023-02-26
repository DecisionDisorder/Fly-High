using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Spec Data", menuName = "Scriptable Object/Weapon Spec Data", order = int.MaxValue)]
public class WeaponSpecData : ScriptableObject
{
    [SerializeField]
    public WeaponType weaponType;

    [SerializeField]
    public WeaponData weaponData;

    [SerializeField]
    public Sprite weaponSprite;

    [SerializeField]
    public Sprite bulletSprite;

    [SerializeField]
    private string bulletName;
    public string BulletName { get { return bulletName; } }

    [SerializeField]
    private int bulletPrice;
    public int BulletPrice { get { return bulletPrice; } }

    [SerializeField]
    private int bulletCapacity;
    public int BulletCapacity { get { return bulletCapacity; } }

    [SerializeField]
    private int bulletForce;
    public int BulletForce { get { return bulletForce; } }

    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    
    [SerializeField]
    private int rpm;
    public int RPM { get { return rpm; } }

    [SerializeField]
    public AudioClip weaponSound;
}
