using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �� �� ��ӵǴ� ���� Ŭ����
/// </summary>
public class FuelDrop : FlyingObject
{
    /// <summary>
    /// ����Ǵ� ���ᷮ
    /// </summary>
    public float fuelRecovery;

    /// <summary>
    /// ���� �� ���� �̺�Ʈ
    /// </summary>
    private void OnEnable()
    {
        Enable();
        spawnedCount[(int)PropType.Fuel]++;
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// ȸ�� �� ���� �̺�Ʈ
    /// </summary>
    private void OnDisable()
    {
        Disable();
        spawnedCount[(int)PropType.Fuel]--;
    }

    /// <summary>
    /// �÷��̾� ���ϰ� �浹 ���� ���� ó��
    /// </summary>
    /// <param name="collision">��� �浹ü</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            // ���� ���� �� ȸ��
            RocketSet.instance.FuelRemain += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * fuelRecovery;
            SpawnManager.instance.ReturnObstacle(this);
        }
    }
}
