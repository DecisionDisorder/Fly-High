using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ ���� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    /// <summary>
    /// ������ ����
    /// </summary>
    public Item item;
    /// <summary>
    /// �������� ������
    /// </summary>
    public Sprite IconSprite;

    /// <summary>
    /// ������ ����
    /// </summary>
    [SerializeField]
    private ItemManager.ItemType itemType;
    public ItemManager.ItemType ItemType { get { return itemType; } }

    /// <summary>
    /// ������ ���� ���� ���� ��ġ
    /// </summary>
    [SerializeField]
    private float mainValue;
    public float MainValue { get { return mainValue; } }

    /// <summary>
    /// ������ ���� ������ ���� ��ġ
    /// </summary>
    [SerializeField]
    private float[] subValues;
    public float[] SubValues { get { return subValues; } }

    /// <summary>
    /// �ֿ� ���� �� �ҷ�����
    /// </summary>
    /// <returns>�ֿ� ���� ��</returns>
    public int GetMainValue()
    {
        return (int)mainValue;
    }

    /// <summary>
    /// ���� ���� �� �ҷ�����
    /// </summary>
    /// <param name="index">���� ���� �ε���</param>
    /// <returns>���� ���� ��</returns>
    public int GetSubValue(int index)
    {
        return (int)subValues[index];
    }
}
