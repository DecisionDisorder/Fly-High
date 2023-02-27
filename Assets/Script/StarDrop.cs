using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �ʵ忡 ��ȯ�Ǵ� �� (���ʽ� ����)
/// </summary>
public class StarDrop : FlyingObject
{
    /// <summary>
    /// ���ʽ� ���� ���޷� ���� (���� ȿ��)
    /// </summary>
    public static float statEffect = 1;
    /// <summary>
    /// ���ʽ� ���� �⺻ ���޷�
    /// </summary>
    public int starPoint;
    /// <summary>
    /// ��¦�̴� �ִϸ��̼� ȿ�� 
    /// </summary>
    public Animation twinklingAni;
    /// <summary>
    /// �� ȹ�� ȿ����
    /// </summary>
    public AudioSource starAudio;
    /// <summary>
    /// �� �̹���
    /// </summary>
    public Image image;

    /// <summary>
    /// ���� �� ���� �̺�Ʈ
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
    /// ȸ���� ���� �̺�Ʈ
    /// </summary>
    private void OnDisable()
    {
        Disable();
        twinklingAni.Stop();
        spawnedCount[(int)PropType.Fuel]--;
    }

    /// <summary>
    /// �� ���� ���� ����
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
    /// �÷��̾� ���ϰ� �浹 �˻�
    /// </summary>
    /// <param name="collision">�浹ü</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            // �⺻ ���޷��� ���� ȿ�� ������ �Ͽ� ���� ���� �� ȸ�� �� UI ������Ʈ
            PlayManager.instance.AddScore((int)(starPoint * statEffect));
            SpawnManager.instance.ReturnFlyingObject(this);
            SpawnManager.instance.SetBonusText(0, (int)(starPoint * statEffect));
            starAudio.Play();
        }
    }
}
