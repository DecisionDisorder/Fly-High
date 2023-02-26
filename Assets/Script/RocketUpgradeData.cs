using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���׷��̵� ������ ���� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Rocket Upgrade Data", menuName = "Scriptable Object/Rocket Upgrade Data", order = int.MaxValue)]
public class RocketUpgradeData : ScriptableObject
{
    /// <summary>
    /// ���׷��̵� �׸� ���� �迭
    /// </summary>
    public UpgradeDataGroup[] upgradeItems = new UpgradeDataGroup[4];
}

[System.Serializable]
public class UpgradeDataGroup
{
    /// <summary>
    /// ���׷��̵� �̸�
    /// </summary>
    public string name;

    [SerializeField]
    private float[] effect;
    /// <summary>
    /// ���׷��̵� ���� ��
    /// </summary>
    public float[] Effect { get { return effect; } }


    [SerializeField]
    private int[] cost;
    /// <summary>
    /// ���׷��̵� ���
    /// </summary>
    public int[] Cost { get { return cost; } }
}