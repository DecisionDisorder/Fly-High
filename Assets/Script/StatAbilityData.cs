using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �ɷ�ġ ������
/// </summary>
[CreateAssetMenu(fileName = "Stat Ability Data", menuName = "Scriptable Object/Stat Ability Data", order = int.MaxValue)]
public class StatAbilityData : ScriptableObject
{
    [SerializeField]
    private string abilityName;
    /// <summary>
    /// �ɷ�ġ �̸�
    /// </summary>
    public string AbilityName { get { return abilityName; } }

    [SerializeField]
    private int maxLevel;
    /// <summary>
    /// �ִ� ����
    /// </summary>
    public int MaxLevel { get { return maxLevel; } }

    [SerializeField]
    private float[] ability;
    /// <summary>
    /// �ɷ�ġ ���� ��ġ
    /// </summary>
    public float[] Ability { get { return ability; } }

}
