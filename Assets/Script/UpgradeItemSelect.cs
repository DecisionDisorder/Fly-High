using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���׷��̵� ���� ����� Ŭ����(Inspector��)
/// </summary>
public class UpgradeItemSelect : MonoBehaviour
{
    /// <summary>
    /// ���׷��̵� ����
    /// </summary>
    public RocketUpgradeItem upgradeList;

    public RocketUpgradeItem GetUpgradeType()
    {
        return upgradeList;
    }
}
