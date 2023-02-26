using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ֹ� ũ�� ������
/// </summary>
public enum ObstacleSize { Small, Middle, Large }

/// <summary>
/// ���� �ʵ忡 �����Ǵ� ��ֹ�
/// </summary>
[System.Serializable]
public class Obstacle : FlyingObject
{
    /// <summary>
    /// ���� ī�޶��� ī�޶� ���� ������Ʈ
    /// </summary>
    public CameraFollow cameraFollow;

    /// <summary>
    /// ��ֹ� ������
    /// </summary>
    public ObstacleData obstacleData;

    public ItemManager itemManager;

    /// <summary>
    /// ��ֹ� ȸ�� Ȯ��
    /// </summary>
    public static float evadeProbability = 0f;
    /// <summary>
    /// �ı� ��ƼŬ ȿ��
    /// </summary>
    public ParticleSystem destoryEffect;
    /// <summary>
    /// ��ֹ� �̹���
    /// </summary>
    public GameObject ObstacleImg;
    /// <summary>
    /// ��ֹ��� �������� �Ծ��� ���� �̹���
    /// </summary>
    public GameObject damagedImg;
    /// <summary>
    /// ��ֹ��� HP
    /// </summary>
    public int hp;

    /// <summary>
    /// ��ֹ� �ı� ȿ����
    /// </summary>
    public AudioSource crashAudio;
    /// <summary>
    /// ��ֹ� �ı� ȿ���� Ŭ�� �迭
    /// </summary>
    public AudioClip[] crashClips;

    /// <summary>
    /// ��ֹ��� �����Ǿ� Ȱ��ȭ �Ǿ��� �� ó��
    /// </summary>
    private void OnEnable()
    {
        // Ȱ��ȭ ó��
        Enable();
        // ���� ���� ����
        spawnedCount[(int)PropType.Obstacle]++;
        // HP �ʱ�ȭ
        hp = obstacleData.Hp;
        // ī�޶� ���� Ž�� ����
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// ��ֹ��� ȸ���Ǿ� ��Ȱ��ȭ �Ǿ��� ���� ó��
    /// </summary>
    private void OnDisable()
    {
        // ��Ȱ��ȭ ó��
        Disable();
        // �ǰ� �̹��� ��Ȱ��ȭ
        damagedImg.SetActive(false);
        // ���� ���� ����
        spawnedCount[(int)PropType.Obstacle]--;
    }

    /// <summary>
    /// �÷��̾� ���� Ȥ�� ��ȣ���� �浹 ���� ���� ó��
    /// </summary>
    /// <param name="collision">�浹ü</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��ȣ���� �ε����� ���� ó��
        if (collision.tag.Equals("Shield"))
        {
            // ��ֹ��� ��Ȱ��ȭ ���� �ʾ��� ��
            if (ObstacleImg.activeInHierarchy)
            {
                // ��ȣ�� �������� HP �谨 ó��
                itemManager.HitShield(obstacleData.Damage);
                Destroy();
            }
        }
        // �÷��̾� ���ϰ� �ε����� ���� ó��
        else if (collision.tag.Equals("Player"))
        {
            // ��ֹ��� ��Ȱ��ȭ ���� �ʾ��� ��
            if (ObstacleImg.activeInHierarchy)
            {
                // ������ ���� ���°� �ƴ� ��쿡
                if(!RocketSet.instance.isInvincible)
                {
                    // ȸ�� Ȯ���� ����Ͽ�
                    float r = Random.Range(1, 100f);
                    // ȸ�ǿ� �������� ��
                    if (r > evadeProbability)
                    {
                        // ������ HP, ���ᷮ ���� ó�� �� ���� �˵��� ������ �ִ� add force ó��
                        RocketSet.instance.Hp -= obstacleData.Damage;
                        RocketSet.instance.FuelRemain -= obstacleData.FuelDamage;
                        Vector3 dir = gameObject.transform.position - RocketSet.instance.rocket.transform.position;
                        RocketSet.instance.rocketMain_rigid.AddForce(-dir * obstacleData.DirectionDmg, ForceMode2D.Impulse);
                        // 0.5�ʰ� ����ȿ��
                        RocketSet.instance.SetInvincible(0.5f);
                        // �浹 ȿ�� �ʱ�ȭ
                        SpawnManager.instance.ResetCollideEffectStart();
                        // ī�޶�  �ѵ鸲 ȿ�� �� �浹 ȿ���� ���
                        cameraFollow.ShakeCamera(0.5f);
                        crashAudio.clip = crashClips[Random.Range(0, crashClips.Length)];
                        crashAudio.Play();
                    }
                    else
                    {
                        Debug.Log("ȸ��!");
                        // TODO : ȸ�� ȿ�� �ֱ�
                    }
                }
            }
        }
    }
    /// <summary>
    /// ��ֹ� �浹 ȿ��
    /// </summary>
    public void DamageEffect()
    {
        damagedImg.SetActive(true);
        StartCoroutine(DamagedEffectOff(0.2f));
    }

    /// <summary>
    /// ��ֹ� �浹 ȿ�� ��Ȱ��ȭ �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��Ȱ��ȭ ��� �ð�</param>
    IEnumerator DamagedEffectOff(float delay)
    {
        yield return new WaitForSeconds(delay);

        damagedImg.SetActive(false);
    }

    /// <summary>
    /// ��ֹ� �ı� ó��
    /// </summary>
    public void Destroy()
    {
        destoryEffect.Play();
        ObstacleImg.SetActive(false);
        StartCoroutine(DestoryWait(0.5f));
    }

    /// <summary>
    /// ��ֹ� �ı� ȿ�� ��� �� ȸ�� �ڷ�ƾ
    /// </summary>
    /// <param name="time">�ı� ��� �ð�</param>
    IEnumerator DestoryWait(float time)
    {
        yield return new WaitForSeconds(time);

        SpawnManager.instance.ReturnFlyingObject(this);
        ObstacleImg.SetActive(true);
    }
}


