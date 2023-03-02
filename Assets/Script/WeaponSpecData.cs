using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ������ ���� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Weapon Spec Data", menuName = "Scriptable Object/Weapon Spec Data", order = int.MaxValue)]
public class WeaponSpecData : ScriptableObject
{
    /// <summary>
    /// ���� ����
    /// </summary>
    [SerializeField]
    public WeaponType weaponType;

    /// <summary>
    /// ������ ������ ������
    /// </summary>
    [SerializeField]
    public WeaponData weaponData;

    /// <summary>
    /// ������ ��������Ʈ �̹���
    /// </summary>
    [SerializeField]
    public Sprite weaponSprite;

    /// <summary>
    /// ������ źȯ ��������Ʈ �̹���
    /// </summary>
    [SerializeField]
    public Sprite bulletSprite;

    [SerializeField]
    private string bulletName;
    /// <summary>
    /// źȯ �̸�
    /// </summary>
    public string BulletName { get { return bulletName; } }

    [SerializeField]
    private int bulletPrice;
    /// <summary>
    /// źȯ ����
    /// </summary>
    public int BulletPrice { get { return bulletPrice; } }

    [SerializeField]
    private int bulletCapacity;
    /// <summary>
    /// �ִ� ���� ������ źȯ ����
    /// </summary>
    public int BulletCapacity { get { return bulletCapacity; } }

    [SerializeField]
    private int bulletForce;
    /// <summary>
    /// źȯ�� ��(�ӵ�)
    /// </summary>
    public int BulletForce { get { return bulletForce; } }

    [SerializeField]
    private int damage;
    /// <summary>
    /// ������ ������
    /// </summary>
    public int Damage { get { return damage; } }
    
    [SerializeField]
    private int rpm;
    /// <summary>
    /// ������ ���� �ӵ�
    /// </summary>
    public int RPM { get { return rpm; } }

    /// <summary>
    /// ���� ���� ȿ���� Ŭ��
    /// </summary>
    [SerializeField]
    public AudioClip weaponSound;
}
