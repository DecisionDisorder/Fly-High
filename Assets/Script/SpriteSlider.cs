using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sprite ����� Ŀ���� Slider
/// </summary>
public class SpriteSlider : MonoBehaviour
{
    /// <summary>
    /// ��� ��������Ʈ
    /// </summary>
    public SpriteRenderer background;
    /// <summary>
    /// �������� ä�� ��������Ʈ
    /// </summary>
    public SpriteRenderer fill;
    /// <summary>
    /// Fill�� ������ ������ RectTransform
    /// </summary>
    public RectTransform fillAreaRect;
    /// <summary>
    /// Fill�� ������ RectTransform
    /// </summary>
    public RectTransform fillRect;
    /// <summary>
    /// �ּ� ��
    /// </summary>
    public float minValue;
    /// <summary>
    /// �ִ� ��
    /// </summary>
    public float maxValue;
    /// <summary>
    /// ���� ��
    /// </summary>
    private float value;

    /// <summary>
    /// �����̴� �� ����
    /// </summary>
    /// <param name="value">������ ��</param>
    public void SetValue(float value)
    {
        // �����̴��� ��
        float width = fillAreaRect.rect.width;
        // �ִ�, �ּ� ��ġ ����
        if (value > maxValue)
            value = maxValue;
        else if (value < minValue)
            value = minValue;
        this.value = value;

        // ���� ��� �� ���� ��ŭ ä���
        float per = (value - minValue) / (maxValue - minValue);

        fillRect.offsetMax = new Vector2(-(1 - per) * width, 0);
        fill.size = new Vector2(width * per, fill.size.y);
    }
}
