using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스탯 선택 도우미 클래스 (Inspector)
/// </summary>
public class StatSelect : MonoBehaviour
{
    /// <summary>
    /// 선택된 스탯 종류
    /// </summary>
    public StatType statType;

    public StatType GetStatType()
    {
        return statType;
    }
}
