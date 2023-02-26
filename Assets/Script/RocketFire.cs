using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ȭ�� ȿ���� �����ϴ� Ŭ����
/// </summary>
public class RocketFire : MonoBehaviour
{
    /// <summary>
    /// ������ ���� ȭ�� ��ƼŬ ����Ʈ
    /// </summary>
    public ParticleSystem fireEffect;
    /// <summary>
    /// ������ ����(child) ȭ�� ��ƼŬ ����Ʈ �迭
    /// </summary>
    public ParticleSystem[] otherFireEffects;
    /// <summary>
    /// ȭ���� ������ �ּ� �ӵ�
    /// </summary>
    public float minSpeed;
    /// <summary>
    /// ȭ���� ������ �ִ� �ӵ�
    /// </summary>
    public float maxSpeed;

    /// <summary>
    /// ����ڰ� ������ ��� ������ ���� ȭ���� ũ�⸦ �����Ѵ�
    /// </summary>
    /// <param name="ratio">��� ����</param>
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
    /// ������ ���� ȭ�� ����Ʈ�� ������ �ӵ� ���� ���Ѵ�.
    /// </summary>
    /// <param name="ratio">��� ����</param>
    /// <returns>ȭ���� �ӵ�</returns>
    private float GetSpeed(float ratio)
    {
        float speed = (maxSpeed - minSpeed) * ratio + minSpeed;

        if (speed < minSpeed)
            return minSpeed;
        else
            return speed;
    }
}
