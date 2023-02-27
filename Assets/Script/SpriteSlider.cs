using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sprite 기반의 커스텀 Slider
/// </summary>
public class SpriteSlider : MonoBehaviour
{
    /// <summary>
    /// 배경 스프라이트
    /// </summary>
    public SpriteRenderer background;
    /// <summary>
    /// 게이지를 채울 스프라이트
    /// </summary>
    public SpriteRenderer fill;
    /// <summary>
    /// Fill의 범위를 제한할 RectTransform
    /// </summary>
    public RectTransform fillAreaRect;
    /// <summary>
    /// Fill을 움직일 RectTransform
    /// </summary>
    public RectTransform fillRect;
    /// <summary>
    /// 최소 값
    /// </summary>
    public float minValue;
    /// <summary>
    /// 최대 값
    /// </summary>
    public float maxValue;
    /// <summary>
    /// 현재 값
    /// </summary>
    private float value;

    /// <summary>
    /// 슬라이더 값 지정
    /// </summary>
    /// <param name="value">지정할 값</param>
    public void SetValue(float value)
    {
        // 슬라이더의 폭
        float width = fillAreaRect.rect.width;
        // 최대, 최소 수치 제한
        if (value > maxValue)
            value = maxValue;
        else if (value < minValue)
            value = minValue;
        this.value = value;

        // 비율 계산 후 비율 만큼 채우기
        float per = (value - minValue) / (maxValue - minValue);

        fillRect.offsetMax = new Vector2(-(1 - per) * width, 0);
        fill.size = new Vector2(width * per, fill.size.y);
    }
}
