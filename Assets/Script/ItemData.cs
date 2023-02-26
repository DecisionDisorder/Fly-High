using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 데이터 관련 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    /// <summary>
    /// 아이템 정보
    /// </summary>
    public Item item;
    /// <summary>
    /// 아이템의 아이콘
    /// </summary>
    public Sprite IconSprite;

    /// <summary>
    /// 아이템 종류
    /// </summary>
    [SerializeField]
    private ItemManager.ItemType itemType;
    public ItemManager.ItemType ItemType { get { return itemType; } }

    /// <summary>
    /// 아이템 관련 메인 스펙 수치
    /// </summary>
    [SerializeField]
    private float mainValue;
    public float MainValue { get { return mainValue; } }

    /// <summary>
    /// 아이템 관련 부차적 스펙 수치
    /// </summary>
    [SerializeField]
    private float[] subValues;
    public float[] SubValues { get { return subValues; } }

    /// <summary>
    /// 주요 스펙 값 불러오기
    /// </summary>
    /// <returns>주요 스펙 값</returns>
    public int GetMainValue()
    {
        return (int)mainValue;
    }

    /// <summary>
    /// 서브 스펙 값 불러오기
    /// </summary>
    /// <param name="index">서브 스펙 인덱스</param>
    /// <returns>서브 스펙 값</returns>
    public int GetSubValue(int index)
    {
        return (int)subValues[index];
    }
}
