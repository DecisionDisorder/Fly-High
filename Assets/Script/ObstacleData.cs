using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ֹ� ���� ������ ���� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Obstacle Data", menuName = "Scriptable Object/Obstacle Data", order = int.MaxValue)]
public class ObstacleData : ScriptableObject
{
    [SerializeField]
    private string obstacleName;
    /// <summary>
    /// ��ֹ��� �̸�
    /// </summary>
    public string ObstacleName { get { return obstacleName; } }

    [SerializeField]
    private Sprite img;
    /// <summary>
    /// ��ֹ��� ��������Ʈ �̹���
    /// </summary>
    public Sprite Img { get { return img; } }

    [SerializeField]
    private int damage;
    /// <summary>
    /// ��ֹ��� �÷��̾� ���Ͽ��� �ִ� ������
    /// </summary>
    public int Damage { get { return damage; } }

    [SerializeField]
    private float fuelDamage;
    /// <summary>
    /// ��ֹ��� �÷��̾� ������ ���ᷮ�� ���ҽ�Ű�� ��ġ
    /// </summary>
    public float FuelDamage { get { return fuelDamage; } }

    [SerializeField]
    private float directionDmg;
    /// <summary>
    /// ��ֹ��� �÷��̾� ������ ���⿡ ������ �ִ� ����
    /// </summary>
    public float DirectionDmg { get { return directionDmg; } }

    [SerializeField]
    private ObstacleSize obstacleSize;
    /// <summary>
    /// ��ֹ� ũ�� �з�
    /// </summary>
    public ObstacleSize ObstacleSize { get { return obstacleSize; } }

    [SerializeField]
    private int hp;
    /// <summary>
    /// ��ֹ��� �ִ� HP
    /// </summary>
    public int Hp { get { return hp; } }
}
