using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로켓 업그레이드 선택 도우미 클래스(Inspector용)
/// </summary>
public class UpgradeItemSelect : MonoBehaviour
{
    /// <summary>
    /// 업그레이드 종류
    /// </summary>
    public RocketUpgradeItem upgradeList;

    public RocketUpgradeItem GetUpgradeType()
    {
        return upgradeList;
    }
}
