using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장애물 스펙 데이터 관련 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Obstacle Data", menuName = "Scriptable Object/Obstacle Data", order = int.MaxValue)]
public class ObstacleData : ScriptableObject
{
    [SerializeField]
    private string obstacleName;
    /// <summary>
    /// 장애물의 이름
    /// </summary>
    public string ObstacleName { get { return obstacleName; } }

    [SerializeField]
    private Sprite img;
    /// <summary>
    /// 장애물의 스프라이트 이미지
    /// </summary>
    public Sprite Img { get { return img; } }

    [SerializeField]
    private int damage;
    /// <summary>
    /// 장애물이 플레이어 로켓에게 주는 데미지
    /// </summary>
    public int Damage { get { return damage; } }

    [SerializeField]
    private float fuelDamage;
    /// <summary>
    /// 장애물이 플레이어 로켓의 연료량을 감소시키는 수치
    /// </summary>
    public float FuelDamage { get { return fuelDamage; } }

    [SerializeField]
    private float directionDmg;
    /// <summary>
    /// 장애물이 플레이어 로켓의 방향에 영향을 주는 강도
    /// </summary>
    public float DirectionDmg { get { return directionDmg; } }

    [SerializeField]
    private ObstacleSize obstacleSize;
    /// <summary>
    /// 장애물 크기 분류
    /// </summary>
    public ObstacleSize ObstacleSize { get { return obstacleSize; } }

    [SerializeField]
    private int hp;
    /// <summary>
    /// 장애물의 최대 HP
    /// </summary>
    public int Hp { get { return hp; } }
}
