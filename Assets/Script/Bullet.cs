using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ѿ� ���� ������ ����ϴ� Ŭ����
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// �Ѿ��� �����Ǿ��ִ� ����
    /// </summary>
    public Weapon weapon;
    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    private Camera mainCamera;
    /// <summary>
    /// �Ѿ��� ���� ������ ������ rigidbody
    /// </summary>
    private Rigidbody2D rigid;

    /// <summary>
    /// �Ѿ� ���� ������ ������Ʈ
    /// </summary>
    private Spawnable spawnable;
    /// <summary>
    /// ī�޶� ���� Ž�� ���� ��� ����
    /// </summary>
    private float margin = 15f;
    /// <summary>
    /// �߻� �Ǿ����� ����
    /// </summary>
    private bool isShot = false;
    /// <summary>
    /// �Ѿ� �̹��� ��������Ʈ ������
    /// </summary>
    public SpriteRenderer bulletImg;
    /// <summary>
    /// ���� ����Ʈ ��ƼŬ
    /// </summary>
    public ParticleSystem explosionEffect;
    /// <summary>
    /// �߻� ���� ���� ����Ʈ ��ƼŬ
    /// </summary>
    public ParticleSystem rearSmokeEffect;
    
    /// <summary>
    /// �Ѿ� ��� ���� �ʱ�ȭ �۾�
    /// </summary>
    private void Initialize()
    {
        mainCamera = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        spawnable = weapon.bulletSpawnable;
    }

    /// <summary>
    /// �Ѿ� �ݹ� 
    /// </summary>
    public void Shot()
    {
        // ù �߿� �ʱ�ȭ �۾�
        if (rigid == null)
            Initialize();
        
        // ���� ���� ����
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.velocity = RocketSet.instance.rocketMain_rigid.velocity; // ���� ���� ����
        rigid.AddForce(spawnable.parent.up * weapon.weaponSpecData.BulletForce, ForceMode2D.Impulse);
        isShot = true;

        // �߻� ���� ���Ⱑ ������ ���
        if (rearSmokeEffect != null)
            rearSmokeEffect.Play();

        // ī�޶� ���� ����� �� �Ѿ��� ��Ȱ��ȭ �� �ڷ�ƾ
        StartCoroutine(DetectOutOfCamera());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �ݹ��� �� ���¿��� ��ֹ��� �ε����� �� ����
        if (collision.tag.Equals("Obstacle") && isShot)
        {
            // ��Ʈ ����
            Hit(collision);
            // �޴��� ����
            Vibration.Vibrate(100); 

            // ���� ����Ʈ�� ������ ���� ������Ʈ ȸ��
            if (explosionEffect == null)
            {
                //cameraFollow.ShakeCamera(0.3f, 0.01f);
                SpawnManager.instance.ReturnObject(gameObject, spawnable);
            }
            // ���� ����Ʈ�� ������ ���� ȿ���� �� ����Ʈ ��� �� ȸ�� ���
            else
            {
                //cameraFollow.ShakeCamera(0.3f, 0.02f);
                PlayManager.instance.PlayExplosionSound();
                rearSmokeEffect.Stop();
                explosionEffect.Play();
                bulletImg.color = new Color(0, 0, 0, 0);
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = 0;
                rigid.bodyType = RigidbodyType2D.Kinematic;
                StartCoroutine(WaitForExplosion(collision));
            }
        }
    }

    // ��Ʈ ����
    private void Hit(Collider2D collision)
    {
        // �浹 ��� ������Ʈ������ Obstacle ������Ʈ 
        Obstacle obs = collision.gameObject.GetComponent<Obstacle>();
        // �ش� ��ֹ��� ü��(������)�� �Ѿ��� ��������ŭ ����
        obs.hp -= weapon.weaponSpecData.Damage;
        // ��ֹ��� ������ ȿ�� ���
        obs.DamageEffect();
        // ��ֹ��� ü��(������)�� 0 ���ϰ� �Ǹ� �ı� ȿ�� ���
        if (obs.hp <= 0)
            obs.Destroy();
    }

    /// <summary>
    /// ȸ���� �Ѿ� �ʱ�ȭ �۾�
    /// </summary>
    private void ResetBullet()
    {
        isShot = false;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        bulletImg.color = Color.white;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        SpawnManager.instance.ReturnObject(gameObject, spawnable);
    }

    /// <summary>
    /// ���� ȿ�� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="collision">�浹 ���</param>
    IEnumerator WaitForExplosion(Collider2D collision)
    {
        while (explosionEffect.isPlaying)
            yield return new WaitForEndOfFrame();

        // ���� ȿ�� ��� �� �Ѿ� �ʱ�ȭ
        ResetBullet();
    }

    /// <summary>
    /// �Ѿ��� ī�޶� ���� ������ �������� Ȯ���ϴ� �ڷ�ƾ
    /// </summary>
    IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

        // �Ѿ��� �߷� ���� ������ �߷� ���� ����ȭ
        rigid.gravityScale = RocketSet.instance.rocketMain_rigid.gravityScale;

        // �Ѿ��� ��ġ�� ī�޶� ��ǥ��� ȯ���Ͽ� ������ ������� Ȯ�� �� �ʱ�ȭ
        Vector3 positonOnCamera = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        if (positonOnCamera.x < -margin || positonOnCamera.y < -margin || positonOnCamera.x > Screen.width + 3 * margin || positonOnCamera.y > Screen.height + 3 * margin)
        {
            ResetBullet();
        }

        // ���� ������Ʈ�� Ȱ��ȭ �Ǿ������� ������ Ž��
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DetectOutOfCamera());
        }
    }
}
