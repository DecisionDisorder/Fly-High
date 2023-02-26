using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon: MonoBehaviour
{
    public GameObject firePos;
    public Bullet bulletPrefab;
    public Bullet loadedBullet;
    public Spawnable bulletSpawnable;
    public Vector3 position;
    public WeaponSpecData weaponSpecData;
    private Sprite beforeShot_sprite;
    public Sprite afterShot_sprite;
    public SpriteRenderer spriteRenderer;
    public AudioClip weaponClip;
    public LineRenderer shotLinerenderer;

    private void Start()
    {
        if (bulletSpawnable == null)
            Initialize();
        StartCoroutine(Draw());
    }

    public void Initialize()
    {
        bulletSpawnable = new Spawnable(bulletPrefab.gameObject, firePos.transform, firePos.transform.position);
        transform.SetParent(RocketSet.instance.weaponParent);
        transform.localPosition = RocketSet.instance.rocketData.GetWeaponPositionByIndex(EquipmentManager.equippedWeaponIndex).position;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void ReloadBullet()
    {
        if (weaponSpecData.weaponData.numOfBulletLeft > 0)
        {
            // 발사 후 이미지가 있는 경우만
            if (afterShot_sprite != null && beforeShot_sprite != null)
            {
                spriteRenderer.sprite = beforeShot_sprite;
            }

            bulletSpawnable.spawnPos = firePos.transform.position + Vector3.forward;
            GameObject bulletObj = SpawnManager.instance.SpawnObject(bulletSpawnable);
            loadedBullet = bulletObj.GetComponent<Bullet>();
        }
    }
    
    public void Shot()
    {
        if(loadedBullet != null)
        {
            loadedBullet.Shot();
            weaponSpecData.weaponData.numOfBulletLeft--;
            loadedBullet = null;

            //발사 후 이미지가 있는 경우만
            if(afterShot_sprite != null)
            {
                beforeShot_sprite = spriteRenderer.sprite;
                spriteRenderer.sprite = afterShot_sprite;
            }
        }
    }

    IEnumerator Draw()
    {
        yield return new WaitForFixedUpdate();

        DrawShotLine();

        StartCoroutine(Draw());
    }

    public void DrawShotLine()
    {
        shotLinerenderer.SetPosition(0, shotLinerenderer.transform.position);
        shotLinerenderer.SetPosition(1, shotLinerenderer.transform.position + shotLinerenderer.transform.up * 20);
    }

    public bool GetHasItem()
    {
        return weaponSpecData.weaponData.hasItem;
    }   
}

[System.Serializable]
public class WeaponData : Item
{
    public int numOfBulletLeft;
}