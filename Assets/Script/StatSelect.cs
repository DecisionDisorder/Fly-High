using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ����� Ŭ���� (Inspector)
/// </summary>
public class StatSelect : MonoBehaviour
{
    /// <summary>
    /// ���õ� ���� ����
    /// </summary>
    public StatType statType;

    public StatType GetStatType()
    {
        return statType;
    }
}
