using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSlider : MonoBehaviour
{
    public SpriteRenderer background;
    public SpriteRenderer fill;
    public RectTransform fillAreaRect;
    public RectTransform fillRect;
    public float minValue;
    public float maxValue;
    private float value;

    private void Start()
    {
        //SetValue(8);
    }

    public void SetValue(float value)
    {
        float width = fillAreaRect.rect.width;
        if (value > maxValue)
            value = maxValue;
        else if (value < minValue)
            value = minValue;
        this.value = value;

        float per = (value - minValue) / (maxValue - minValue);

        fillRect.offsetMax = new Vector2(-(1 - per) * width, 0);
        fill.size = new Vector2(width * per, fill.size.y);
    }
}
