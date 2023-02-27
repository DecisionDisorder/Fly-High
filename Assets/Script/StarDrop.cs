using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 필드에 소환되는 별 (보너스 점수)
/// </summary>
public class StarDrop : FlyingObject
{
    /// <summary>
    /// 보너스 점수 지급량 배율 (스탯 효과)
    /// </summary>
    public static float statEffect = 1;
    /// <summary>
    /// 보너스 점수 기본 지급량
    /// </summary>
    public int starPoint;
    /// <summary>
    /// 반짝이는 애니메이션 효과 
    /// </summary>
    public Animation twinklingAni;
    /// <summary>
    /// 별 획득 효과음
    /// </summary>
    public AudioSource starAudio;
    /// <summary>
    /// 별 이미지
    /// </summary>
    public Image image;

    /// <summary>
    /// 스폰 될 때의 이벤트
    /// </summary>
    private void OnEnable()
    {
        Enable();
        spawnedCount[(int)PropType.Fuel]++;
        SetStarColor();
        twinklingAni.Play();
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// 회수될 때의 이벤트
    /// </summary>
    private void OnDisable()
    {
        Disable();
        twinklingAni.Stop();
        spawnedCount[(int)PropType.Fuel]--;
    }

    /// <summary>
    /// 별 색상 랜덤 지정
    /// </summary>
    private void SetStarColor()
    {
        int r = Random.Range(0, 100);
        if(r < 10)
        {
            image.color = Color.white;
        }
        else
        {
            float[] colors = new float[3];
            int colorRange = 55;
            for(int i = 0; i < 3 && colorRange > 0; i++)
            {
                int c = Random.Range(0, colorRange + 1);
                colorRange -= c;
                colors[i] = 255 - c;
            }
            image.color = new Color(colors[0] / 255, colors[1] / 255, colors[2] / 255);
        }
    }

    /// <summary>
    /// 플레이어 로켓과 충돌 검사
    /// </summary>
    /// <param name="collision">충돌체</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            // 기본 지급량에 스탯 효과 곱연산 하여 점수 지급 후 회수 및 UI 업데이트
            PlayManager.instance.AddScore((int)(starPoint * statEffect));
            SpawnManager.instance.ReturnFlyingObject(this);
            SpawnManager.instance.SetBonusText(0, (int)(starPoint * statEffect));
            starAudio.Play();
        }
    }
}
