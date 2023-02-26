using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장애물 크기 열거형
/// </summary>
public enum ObstacleSize { Small, Middle, Large }

/// <summary>
/// 게임 필드에 스폰되는 장애물
/// </summary>
[System.Serializable]
public class Obstacle : FlyingObject
{
    /// <summary>
    /// 메인 카메라의 카메라 추적 오브젝트
    /// </summary>
    public CameraFollow cameraFollow;

    /// <summary>
    /// 장애물 데이터
    /// </summary>
    public ObstacleData obstacleData;

    public ItemManager itemManager;

    /// <summary>
    /// 장애물 회피 확률
    /// </summary>
    public static float evadeProbability = 0f;
    /// <summary>
    /// 파괴 파티클 효과
    /// </summary>
    public ParticleSystem destoryEffect;
    /// <summary>
    /// 장애물 이미지
    /// </summary>
    public GameObject ObstacleImg;
    /// <summary>
    /// 장애물이 데미지를 입었을 때의 이미지
    /// </summary>
    public GameObject damagedImg;
    /// <summary>
    /// 장애물의 HP
    /// </summary>
    public int hp;

    /// <summary>
    /// 장애물 파괴 효과음
    /// </summary>
    public AudioSource crashAudio;
    /// <summary>
    /// 장애물 파괴 효과음 클립 배열
    /// </summary>
    public AudioClip[] crashClips;

    /// <summary>
    /// 장애물이 스폰되어 활성화 되었을 때 처리
    /// </summary>
    private void OnEnable()
    {
        // 활성화 처리
        Enable();
        // 스폰 개수 증가
        spawnedCount[(int)PropType.Obstacle]++;
        // HP 초기화
        hp = obstacleData.Hp;
        // 카메라 범위 탐지 시작
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// 장애물이 회수되어 비활성화 되었을 때의 처리
    /// </summary>
    private void OnDisable()
    {
        // 비활성화 처리
        Disable();
        // 피격 이미지 비활성화
        damagedImg.SetActive(false);
        // 스폰 개수 감소
        spawnedCount[(int)PropType.Obstacle]--;
    }

    /// <summary>
    /// 플레이어 로켓 혹은 보호막과 충돌 했을 때의 처리
    /// </summary>
    /// <param name="collision">충돌체</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 보호막과 부딪혔을 때의 처리
        if (collision.tag.Equals("Shield"))
        {
            // 장애물이 비활성화 되지 않았을 때
            if (ObstacleImg.activeInHierarchy)
            {
                // 보호막 아이템의 HP 삭감 처리
                itemManager.HitShield(obstacleData.Damage);
                Destroy();
            }
        }
        // 플레이어 로켓과 부딪혔을 때의 처리
        else if (collision.tag.Equals("Player"))
        {
            // 장애물이 비활성화 되지 않았을 때
            if (ObstacleImg.activeInHierarchy)
            {
                // 로켓이 무적 상태가 아닌 경우에
                if(!RocketSet.instance.isInvincible)
                {
                    // 회피 확률을 계산하여
                    float r = Random.Range(1, 100f);
                    // 회피에 실패했을 때
                    if (r > evadeProbability)
                    {
                        // 로켓의 HP, 연료량 감소 처리 및 로켓 궤도에 영향을 주는 add force 처리
                        RocketSet.instance.Hp -= obstacleData.Damage;
                        RocketSet.instance.FuelRemain -= obstacleData.FuelDamage;
                        Vector3 dir = gameObject.transform.position - RocketSet.instance.rocket.transform.position;
                        RocketSet.instance.rocketMain_rigid.AddForce(-dir * obstacleData.DirectionDmg, ForceMode2D.Impulse);
                        // 0.5초간 무적효과
                        RocketSet.instance.SetInvincible(0.5f);
                        // 충돌 효과 초기화
                        SpawnManager.instance.ResetCollideEffectStart();
                        // 카메라  한들림 효과 및 충돌 효과음 재생
                        cameraFollow.ShakeCamera(0.5f);
                        crashAudio.clip = crashClips[Random.Range(0, crashClips.Length)];
                        crashAudio.Play();
                    }
                    else
                    {
                        Debug.Log("회피!");
                        // TODO : 회피 효과 넣기
                    }
                }
            }
        }
    }
    /// <summary>
    /// 장애물 충돌 효과
    /// </summary>
    public void DamageEffect()
    {
        damagedImg.SetActive(true);
        StartCoroutine(DamagedEffectOff(0.2f));
    }

    /// <summary>
    /// 장애물 충돌 효과 비활성화 코루틴
    /// </summary>
    /// <param name="delay">비활성화 대기 시간</param>
    IEnumerator DamagedEffectOff(float delay)
    {
        yield return new WaitForSeconds(delay);

        damagedImg.SetActive(false);
    }

    /// <summary>
    /// 장애물 파괴 처리
    /// </summary>
    public void Destroy()
    {
        destoryEffect.Play();
        ObstacleImg.SetActive(false);
        StartCoroutine(DestoryWait(0.5f));
    }

    /// <summary>
    /// 장애물 파괴 효과 대기 후 회수 코루틴
    /// </summary>
    /// <param name="time">파괴 대기 시간</param>
    IEnumerator DestoryWait(float time)
    {
        yield return new WaitForSeconds(time);

        SpawnManager.instance.ReturnFlyingObject(this);
        ObstacleImg.SetActive(true);
    }
}


