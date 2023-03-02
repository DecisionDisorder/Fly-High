using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� Ŭ���� 
/// </summary>
[System.Serializable]
public class Weapon: MonoBehaviour
{
    /// <summary>
    /// źȯ �߻� ���� ��ġ
    /// </summary>
    public GameObject firePos;
    /// <summary>
    /// źȯ �� ������
    /// </summary>
    public Bullet bulletPrefab;
    /// <summary>
    /// ������ źȯ
    /// </summary>
    public Bullet loadedBullet;
    /// <summary>
    /// źȯ ���� ���� ������
    /// </summary>
    public Spawnable bulletSpawnable;

    /// <summary>
    /// ���� ���� ������
    /// </summary>
    public WeaponSpecData weaponSpecData;
    /// <summary>
    /// ���� �� ��������Ʈ �̹��� ���ҽ�
    /// </summary>
    private Sprite beforeShot_sprite;
    /// <summary>
    /// ���� �� ��������Ʈ �̹��� ���ҽ�
    /// </summary>
    public Sprite afterShot_sprite;
    /// <summary>
    /// ���� ��������Ʈ ������
    /// </summary>
    public SpriteRenderer spriteRenderer;
    /// <summary>
    /// ���� �߻� ȿ���� Ŭ��
    /// </summary>
    public AudioClip weaponClip;
    /// <summary>
    /// ���� ���� ���� ������
    /// </summary>
    public LineRenderer shotLinerenderer;

    private void Start()
    {
        if (bulletSpawnable == null)
            Initialize();
        StartCoroutine(Draw());
    }

    /// <summary>
    /// źȯ �������� �� ���� transform �ʱ�ȭ
    /// </summary>
    public void Initialize()
    {
        bulletSpawnable = new Spawnable(bulletPrefab.gameObject, firePos.transform, firePos.transform.position);
        transform.SetParent(RocketSet.instance.weaponParent);
        transform.localPosition = RocketSet.instance.rocketData.GetWeaponPositionByIndex(EquipmentManager.equippedWeaponIndex).position;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// źȯ ������
    /// </summary>
    public void ReloadBullet()
    {
        // ���� źȯ�� ���� ��
        if (weaponSpecData.weaponData.numOfBulletLeft > 0)
        {
            // �߻� ��/�� �̹����� ���� �ִ� ��츸 ���� ��������Ʈ ����
            if (afterShot_sprite != null && beforeShot_sprite != null)
            {
                spriteRenderer.sprite = beforeShot_sprite;
            }

            // źȯ ��ġ ���� �� ��ȯ
            bulletSpawnable.spawnPos = firePos.transform.position + Vector3.forward;
            GameObject bulletObj = SpawnManager.instance.SpawnObject(bulletSpawnable);
            loadedBullet = bulletObj.GetComponent<Bullet>();
        }
    }
    
    /// <summary>
    /// ���� ����
    /// </summary>
    public void Shot()
    {
        // ������ �Ѿ��� ���� ��
        if(loadedBullet != null)
        {
            // ������ �Ѿ��� �߻��ϰ�, ���� źȯ ���� ����
            loadedBullet.Shot();
            weaponSpecData.weaponData.numOfBulletLeft--;
            // ������ �Ѿ� �������� ó��
            loadedBullet = null;

            //�߻� ��/�� �̹����� ���� �ִ� ��츸 �̹��� ��ü
            if(afterShot_sprite != null)
            {
                beforeShot_sprite = spriteRenderer.sprite;
                spriteRenderer.sprite = afterShot_sprite;
            }
        }
    }

    /// <summary>
    /// ���� ���� �� �׸��� �ڷ�ƾ
    /// </summary>
    IEnumerator Draw()
    {
        yield return new WaitForFixedUpdate();

        DrawShotLine();

        StartCoroutine(Draw());
    }

    /// <summary>
    /// ���� ���� �׸���
    /// </summary>
    public void DrawShotLine()
    {
        shotLinerenderer.SetPosition(0, shotLinerenderer.transform.position);
        shotLinerenderer.SetPosition(1, shotLinerenderer.transform.position + shotLinerenderer.transform.up * 20);
    }

    /// <summary>
    /// �� ���⸦ ������ �ִ��� ���θ� ��ȯ
    /// </summary>
    /// <returns>�� ���� ���� ����</returns>
    public bool GetHasItem()
    {
        return weaponSpecData.weaponData.hasItem;
    }   
}
/// <summary>
/// ���� ���� ������
/// </summary>
[System.Serializable]
public class WeaponData : Item
{
    /// <summary>
    /// ���� źȯ ��
    /// </summary>
    public int numOfBulletLeft;
}