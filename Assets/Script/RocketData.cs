using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로켓 스펙 데이터 관련 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName ="Rocket Data", menuName ="Scriptable Object/Rocket Data", order = int.MaxValue)]

public class RocketData: ScriptableObject
{
    /// <summary>
    /// 로켓 종류
    /// </summary>
    [SerializeField]
    public RocketType rocketType;

    /// <summary>
    /// 로켓의 아이템적 속성 데이터 
    /// </summary>
    [SerializeField]
    public Item itemData;

    /// <summary>
    /// 로켓 전체 프리셋 이미지
    /// </summary>
    [SerializeField]
    public RocketImagePreset imagePreset;

    [SerializeField]
    private int hp;
    /// <summary>
    /// 로켓 HP
    /// </summary>
    public int Hp { get { return hp; } }

    [SerializeField]
    private float[] maxPower;
    /// <summary>
    /// 로켓의 각 단마다의 최대 출력
    /// </summary>
    public float[] MaxPower { get { return maxPower; } }

    [SerializeField]
    private float[] weight;
    /// <summary>
    /// 로켓의 각 단마다의 중량
    /// </summary>
    public float[] Weight { get { return weight; } }

    [SerializeField]
    private List<float> fuel;
    /// <summary>
    /// 로켓의 각 단마다의 최대 연료량
    /// </summary>
    public List<float> Fuel { get { return fuel; } }

    /// <summary>
    /// 로켓 최대 연료량의 총합
    /// </summary>
    public float FuelTotal
    {
        get 
        { 
            float total = 0;
            for (int i = 0; i < fuel.Count; i++)
                total += fuel[i];
            return total;
        }
    }

    [SerializeField]
    private float fuelEfficienty;
    /// <summary>
    /// 로켓의 연료 효율 수치
    /// </summary>
    public float FuelEfficienty { get { return fuelEfficienty; } }

    /// <summary>
    /// 로켓의 무기 호환성
    /// </summary>
    public bool[] weaponCompatibility;

    /// <summary>
    /// 각 무기마다의 로켓 위에서의 장착 위치
    /// </summary>
    public WeaponPosition[] weaponPositions;

    /// <summary>
    /// 무기의 인덱스로 위치 오브젝트를 찾아주는 함수
    /// </summary>
    /// <param name="index">무기 인덱스</param>
    /// <returns>무기 위치 정보 오브젝트</returns>
    public WeaponPosition GetWeaponPositionByIndex(int index)
    {
        return GetWeaponPositionByType((WeaponType)index);
    }

    /// <summary>
    /// 무기 종류로 위치 오브젝트를 찾아주는 함수
    /// </summary>
    /// <param name="type">무기 종류</param>
    /// <returns>무기 위치 정보 오브젝트</returns>
    public WeaponPosition GetWeaponPositionByType(WeaponType type)
    {
        for(int i = 0; i < weaponPositions.Length; i++)
        {
            if(weaponPositions[i].weaponType == type)
            {
                return weaponPositions[i];
            }
        }

        return null;
    }

    [SerializeField]
    private float a;
    /// <summary>
    /// 로켓 파워-연료효율 계산 상수 A (2차항)
    /// </summary>
    public float A { get { return a; } }

    [SerializeField]
    private float b;
    /// <summary>
    /// 로켓 파워-연료효율 계산 상수 B (1차항)
    /// </summary>
    public float B { get { return b; } }

    [SerializeField]
    private float c;
    /// <summary>
    /// 로켓 파워-연료효율 계산 상수 C (상수항)
    /// </summary>
    public float C { get { return c; } }
}
