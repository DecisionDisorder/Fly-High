using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Ability Data", menuName = "Scriptable Object/Stat Ability Data", order = int.MaxValue)]
public class StatAbilityData : ScriptableObject
{
    [SerializeField]
    private string abilityName;
    public string AbilityName { get { return abilityName; } }

    [SerializeField]
    private int maxLevel;
    public int MaxLevel { get { return maxLevel; } }

    [SerializeField]
    private float[] ability;
    public float[] Ability { get { return ability; } }

}
