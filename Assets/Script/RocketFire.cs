using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로켓의 화염 효과를 관리하는 클래스
/// </summary>
public class RocketFire : MonoBehaviour
{
    /// <summary>
    /// 로켓의 메인 화염 파티클 이펙트
    /// </summary>
    public ParticleSystem fireEffect;
    /// <summary>
    /// 로켓의 서브(child) 화염 파티클 이펙트 배열
    /// </summary>
    public ParticleSystem[] otherFireEffects;
    /// <summary>
    /// 화염이 퍼지는 최소 속도
    /// </summary>
    public float minSpeed;
    /// <summary>
    /// 화염이 퍼지는 최대 속도
    /// </summary>
    public float maxSpeed;

    /// <summary>
    /// 사용자가 조절한 출력 비율에 따라서 화염의 크기를 조절한다
    /// </summary>
    /// <param name="ratio">출력 비율</param>
    public void FireSizeSync(float ratio)
    {
        ParticleSystem.MainModule main = fireEffect.main;
        float speed = GetSpeed(ratio);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed);
        if(otherFireEffects.Length >= 1)
        {
            for(int i = 0; i < otherFireEffects.Length; i++)
            {
                main = otherFireEffects[i].main;
                main.startSpeed = new ParticleSystem.MinMaxCurve(speed);
            }
        }
    }

    /// <summary>
    /// 비율에 따른 화염 이펙트가 퍼지는 속도 값을 구한다.
    /// </summary>
    /// <param name="ratio">출력 비율</param>
    /// <returns>화염의 속도</returns>
    private float GetSpeed(float ratio)
    {
        float speed = (maxSpeed - minSpeed) * ratio + minSpeed;

        if (speed < minSpeed)
            return minSpeed;
        else
            return speed;
    }
}
