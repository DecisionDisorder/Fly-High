using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarDrop : FlyingObject
{
    public static float statEffect = 1;
    public int starPoint;
    public Animation twinklingAni;
    public AudioSource starAudio;
    public Image image;

    private void OnEnable()
    {
        Enable();
        spawnedCount[(int)PropType.Fuel]++;
        SetStarColor();
        twinklingAni.Play();
        StartCoroutine(DetectOutOfCamera());
    }

    private void OnDisable()
    {
        Disable();
        twinklingAni.Stop();
        spawnedCount[(int)PropType.Fuel]--;
    }

    private void SetStarColor()
    {
        int r = Random.Range(0, 100);
        if(r < 10)
        {
            image.color = Color.white;
        }
        else
        {
            float[] colors = new float[3];
            int colorRange = 55;
            for(int i = 0; i < 3 && colorRange > 0; i++)
            {
                int c = Random.Range(0, colorRange + 1);
                colorRange -= c;
                colors[i] = 255 - c;
            }
            image.color = new Color(colors[0] / 255, colors[1] / 255, colors[2] / 255);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            PlayManager.instance.AddScore((int)(starPoint * statEffect));
            SpawnManager.instance.ReturnObstacle(this);
            SpawnManager.instance.SetBonusText(0, (int)(starPoint * statEffect));
            starAudio.Play();
        }
    }
}
