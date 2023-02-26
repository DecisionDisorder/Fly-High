using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �� �� ��ӵǴ� ���� Ŭ����
/// </summary>
public class CoinDrop : FlyingObject
{
    /// <summary>
    /// ���� ȹ�淮 ���� ȿ�� ��ġ
    /// </summary>
    public static float statEffect = 1;
    /// <summary>
    /// ���� ��ӷ� �ּ�ġ
    /// </summary>
    public int min;
    /// <summary>
    /// ���� ��ӷ� �ִ�ġ
    /// </summary>
    public int max;
    /// <summary>
    /// ���� ȹ�� ����� ȿ����
    /// </summary>
    public AudioSource coinAudio;

    /// <summary>
    /// ���� �� ���� �̺�Ʈ
    /// </summary>
    private void OnEnable()
    {
        // �θ� Ŭ������ Enable �Լ� �� ȭ�� ���� Ž�� ����
        Enable();
        StartCoroutine(DetectOutOfCamera());
        // ������ ����
        spawnedCount[(int)PropType.Coin]++;
    }

    /// <summary>
    /// ȸ�� �� ���� �̺�Ʈ
    /// </summary>
    private void OnDisable()
    {
        // �θ� Ŭ������ Disable �Լ� ȣ��
        Disable();
        // ������ ����
        spawnedCount[(int)PropType.Coin]--;
    }
    
    /// <summary>
    /// �÷��̾� ���ϰ� �浹 ���� ���� ó��
    /// </summary>
    /// <param name="collision">��� �浹ü</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� ���ϰ� �浹 ���� �� ���� ȹ�� �� ȸ��, UI ������Ʈ ó��
        if (collision.transform.tag.Equals("Player"))
        {
            int add = Random.Range(min, max);
            add = (int)((add - (add % 5)) * statEffect); // ���� ȹ�淮 ���� ���� ��� ����
            EconomicMgr.instance.BonusCoin(add);
            SpawnManager.instance.ReturnFlyingObject(this);
            SpawnManager.instance.SetBonusText(1, add);
            coinAudio.Play();
        }
    }
}
