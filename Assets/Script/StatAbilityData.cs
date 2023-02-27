using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스탯 능력치 데이터
/// </summary>
[CreateAssetMenu(fileName = "Stat Ability Data", menuName = "Scriptable Object/Stat Ability Data", order = int.MaxValue)]
public class StatAbilityData : ScriptableObject
{
    [SerializeField]
    private string abilityName;
    /// <summary>
    /// 능력치 이름
    /// </summary>
    public string AbilityName { get { return abilityName; } }

    [SerializeField]
    private int maxLevel;
    /// <summary>
    /// 최대 레벨
    /// </summary>
    public int MaxLevel { get { return maxLevel; } }

    [SerializeField]
    private float[] ability;
    /// <summary>
    /// 능력치 보상 수치
    /// </summary>
    public float[] Ability { get { return ability; } }

}
