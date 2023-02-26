using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFire : MonoBehaviour
{
    /*public RectTransform fire_rectT;
    public SpriteRenderer fire_spriteRenderer;
    private Vector2 originalSize;
    private float currentRatio;
    private float effectRatio = 0f;
    private float minEffect = 0.8f;
    private float fireEffectSpeed = 0.6f;
    IEnumerator fireEffectAnimation;*/

    public ParticleSystem fireEffect;
    public ParticleSystem[] otherFireEffects;
    public float minSpeed;
    public float maxSpeed;

    public void FireSizeSync(float ratio)
    {
        /*currentRatio = ratio;
        fire_rectT.offsetMin = new Vector2(0, originalSize.y - originalSize.y * ratio);
        fire_rectT.offsetMax = Vector2.zero;
        fire_spriteRenderer.size = fire_rectT.rect.size;*/
        ParticleSystem.MainModule main = fireEffect.main;
        float speed = GetSpeed(ratio);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed);
        if(otherFireEffects.Length >= 1)
        {
            for(int i = 0; i < otherFireEffects.Length; i++)
            {
                main = otherFireEffects[i].main;
                main.startSpeed = new ParticleSystem.MinMaxCurve(speed);
            }
        }
    }

    private float GetSpeed(float ratio)
    {
        float speed = (maxSpeed - minSpeed) * ratio + minSpeed;

        if (speed < minSpeed)
            return minSpeed;
        else
            return speed;
    }
    
    /*private void OnEnable()
    {
        fireEffectAnimation = FireEffectAnimation(true);
        StartCoroutine(fireEffectAnimation);
    }

    private void OnDisable()
    {
        effectRatio = 0f;
        StopCoroutine(fireEffectAnimation);
    }

    IEnumerator FireEffectAnimation(bool isIncrease)
    {
        yield return new WaitForEndOfFrame();
        
        float size = originalSize.y - originalSize.y * currentRatio * effectRatio;

        if (effectRatio < minEffect)
        {
            effectRatio += fireEffectSpeed * 3 * Time.deltaTime;
        }
        else
        {
            
            if (isIncrease)
            {
                effectRatio += fireEffectSpeed * Time.deltaTime;
                if (effectRatio > 1)
                {
                    effectRatio = 1;
                    isIncrease = false;
                }
            }
            else
            {
                effectRatio -= fireEffectSpeed * Time.deltaTime;
                if (effectRatio < minEffect)
                {
                    effectRatio = minEffect;
                    isIncrease = true;
                }
            }
        }
        fire_rectT.offsetMin = new Vector2(0, size);
        fire_rectT.offsetMax = Vector2.zero;
        fire_spriteRenderer.size = fire_rectT.rect.size;

        fireEffectAnimation = FireEffectAnimation(isIncrease);
        StartCoroutine(fireEffectAnimation);
    }*/
}
