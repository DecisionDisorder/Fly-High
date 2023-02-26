using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rocket Upgrade Data", menuName = "Scriptable Object/Rocket Upgrade Data", order = int.MaxValue)]
public class RocketUpgradeData : ScriptableObject
{
    public UpgradeDataGroup[] upgradeItems = new UpgradeDataGroup[4];
}

[System.Serializable]
public class UpgradeDataGroup
{
    public string name;

    [SerializeField]
    private float[] effect;
    public float[] Effect { get { return effect; } }


    [SerializeField]
    private int[] cost;
    public int[] Cost { get { return cost; } }
}