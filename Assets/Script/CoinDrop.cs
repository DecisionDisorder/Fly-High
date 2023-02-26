using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 맵 상에 드롭되는 코인 클래스
/// </summary>
public class CoinDrop : FlyingObject
{
    /// <summary>
    /// 코인 획득량 증가 효과 수치
    /// </summary>
    public static float statEffect = 1;
    /// <summary>
    /// 코인 드롭량 최소치
    /// </summary>
    public int min;
    /// <summary>
    /// 코인 드롭량 최대치
    /// </summary>
    public int max;
    /// <summary>
    /// 코인 획득 오디오 효과음
    /// </summary>
    public AudioSource coinAudio;

    /// <summary>
    /// 스폰 될 때의 이벤트
    /// </summary>
    private void OnEnable()
    {
        // 부모 클래스의 Enable 함수 및 화면 범위 탐지 시작
        Enable();
        StartCoroutine(DetectOutOfCamera());
        // 스폰량 증가
        spawnedCount[(int)PropType.Coin]++;
    }

    /// <summary>
    /// 회수 될 때의 이벤트
    /// </summary>
    private void OnDisable()
    {
        // 부모 클래스의 Disable 함수 호출
        Disable();
        // 스폰량 감소
        spawnedCount[(int)PropType.Coin]--;
    }
    
    /// <summary>
    /// 플레이어 로켓과 충돌 했을 때의 처리
    /// </summary>
    /// <param name="collision">대상 충돌체</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 로켓과 충돌 했을 때 코인 획득 및 회수, UI 업데이트 처리
        if (collision.transform.tag.Equals("Player"))
        {
            int add = Random.Range(min, max);
            add = (int)((add - (add % 5)) * statEffect); // 코인 획득량 증가 스탯 계산 적용
            EconomicMgr.instance.BonusCoin(add);
            SpawnManager.instance.ReturnFlyingObject(this);
            SpawnManager.instance.SetBonusText(1, add);
            coinAudio.Play();
        }
    }
}
