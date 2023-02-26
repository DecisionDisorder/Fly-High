using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 맵 상에 드롭되는 연료 클래스
/// </summary>
public class FuelDrop : FlyingObject
{
    /// <summary>
    /// 보충되는 연료량
    /// </summary>
    public float fuelRecovery;

    /// <summary>
    /// 스폰 될 때의 이벤트
    /// </summary>
    private void OnEnable()
    {
        Enable();
        spawnedCount[(int)PropType.Fuel]++;
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// 회수 될 때의 이벤트
    /// </summary>
    private void OnDisable()
    {
        Disable();
        spawnedCount[(int)PropType.Fuel]--;
    }

    /// <summary>
    /// 플레이어 로켓과 충돌 했을 때의 처리
    /// </summary>
    /// <param name="collision">대상 충돌체</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            // 연료 보충 후 회수
            RocketSet.instance.FuelRemain += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * fuelRecovery;
            SpawnManager.instance.ReturnObstacle(this);
        }
    }
}
