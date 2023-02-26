using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ������ ���� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName ="Rocket Data", menuName ="Scriptable Object/Rocket Data", order = int.MaxValue)]

public class RocketData: ScriptableObject
{
    /// <summary>
    /// ���� ����
    /// </summary>
    [SerializeField]
    public RocketType rocketType;

    /// <summary>
    /// ������ �������� �Ӽ� ������ 
    /// </summary>
    [SerializeField]
    public Item itemData;

    /// <summary>
    /// ���� ��ü ������ �̹���
    /// </summary>
    [SerializeField]
    public RocketImagePreset imagePreset;

    [SerializeField]
    private int hp;
    /// <summary>
    /// ���� HP
    /// </summary>
    public int Hp { get { return hp; } }

    [SerializeField]
    private float[] maxPower;
    /// <summary>
    /// ������ �� �ܸ����� �ִ� ���
    /// </summary>
    public float[] MaxPower { get { return maxPower; } }

    [SerializeField]
    private float[] weight;
    /// <summary>
    /// ������ �� �ܸ����� �߷�
    /// </summary>
    public float[] Weight { get { return weight; } }

    [SerializeField]
    private List<float> fuel;
    /// <summary>
    /// ������ �� �ܸ����� �ִ� ���ᷮ
    /// </summary>
    public List<float> Fuel { get { return fuel; } }

    /// <summary>
    /// ���� �ִ� ���ᷮ�� ����
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
    /// ������ ���� ȿ�� ��ġ
    /// </summary>
    public float FuelEfficienty { get { return fuelEfficienty; } }

    /// <summary>
    /// ������ ���� ȣȯ��
    /// </summary>
    public bool[] weaponCompatibility;

    /// <summary>
    /// �� ���⸶���� ���� �������� ���� ��ġ
    /// </summary>
    public WeaponPosition[] weaponPositions;

    /// <summary>
    /// ������ �ε����� ��ġ ������Ʈ�� ã���ִ� �Լ�
    /// </summary>
    /// <param name="index">���� �ε���</param>
    /// <returns>���� ��ġ ���� ������Ʈ</returns>
    public WeaponPosition GetWeaponPositionByIndex(int index)
    {
        return GetWeaponPositionByType((WeaponType)index);
    }

    /// <summary>
    /// ���� ������ ��ġ ������Ʈ�� ã���ִ� �Լ�
    /// </summary>
    /// <param name="type">���� ����</param>
    /// <returns>���� ��ġ ���� ������Ʈ</returns>
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
    /// ���� �Ŀ�-����ȿ�� ��� ��� A (2����)
    /// </summary>
    public float A { get { return a; } }

    [SerializeField]
    private float b;
    /// <summary>
    /// ���� �Ŀ�-����ȿ�� ��� ��� B (1����)
    /// </summary>
    public float B { get { return b; } }

    [SerializeField]
    private float c;
    /// <summary>
    /// ���� �Ŀ�-����ȿ�� ��� ��� C (�����)
    /// </summary>
    public float C { get { return c; } }
}
