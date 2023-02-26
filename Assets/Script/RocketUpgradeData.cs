using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로켓 업그레이드 데이터 관련 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Rocket Upgrade Data", menuName = "Scriptable Object/Rocket Upgrade Data", order = int.MaxValue)]
public class RocketUpgradeData : ScriptableObject
{
    /// <summary>
    /// 업그레이드 항목에 따른 배열
    /// </summary>
    public UpgradeDataGroup[] upgradeItems = new UpgradeDataGroup[4];
}

[System.Serializable]
public class UpgradeDataGroup
{
    /// <summary>
    /// 업그레이드 이름
    /// </summary>
    public string name;

    [SerializeField]
    private float[] effect;
    /// <summary>
    /// 업그레이드 보상 값
    /// </summary>
    public float[] Effect { get { return effect; } }


    [SerializeField]
    private int[] cost;
    /// <summary>
    /// 업그레이드 비용
    /// </summary>
    public int[] Cost { get { return cost; } }
}