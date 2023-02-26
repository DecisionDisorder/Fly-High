using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UniverseObject
{
    public string name;
    public Image[] images;
    public Sprite[] sprites;
    public bool overlapAllowed = false;
    public int[] loadedSpriteList;
    private int loadedSpriteIndex = -2;
    public Queue<int> loadedImageIndex = new Queue<int>(); // -1: None(Default)
    private int lastImageIndex = -1;

    public void ReturnImage()
    {
        images[loadedImageIndex.Dequeue()].gameObject.SetActive(false);
    }

    public Image SetRandomImage()
    {
        if(loadedImageIndex.Count < images.Length)
        {
            loadedSpriteIndex++;
            if (loadedSpriteIndex >= sprites.Length || loadedSpriteIndex.Equals(-1))
            {
                SetRandomIndex();
                loadedSpriteIndex = 0;
            }

            int newImageIndex = (lastImageIndex + 1 < images.Length ? ++lastImageIndex : 0);
            loadedImageIndex.Enqueue(newImageIndex);
            images[newImageIndex].sprite = sprites[loadedSpriteList[loadedSpriteIndex]];
            float randomSize = Random.Range(0.5f, 1.2f);
            images[newImageIndex].transform.localScale = new Vector3(randomSize, randomSize, 1);
            images[newImageIndex].SetNativeSize();
            images[newImageIndex].transform.localPosition = GetRandomPosition(newImageIndex);
            images[newImageIndex].gameObject.SetActive(true);
            return images[newImageIndex];
        }

        return null;
    } 

    private Vector3 GetRandomPosition(int index)
    {
        int r = Random.Range(0, 2);
        float screenWidth = Screen.width;
        float screenheight = Screen.height;
        float x, y;
        float halfHeight = sprites[loadedSpriteList[loadedSpriteIndex]].rect.height * 0.5f * images[index].transform.localScale.y;
        float halfWidth = sprites[loadedSpriteList[loadedSpriteIndex]].rect.width * 0.5f * images[index].transform.localScale.x;
        if (r.Equals(0))
        {
            y = Random.Range(-halfHeight - halfHeight * 0.5f, halfHeight + screenheight);
            x = (screenWidth * 0.5f) + Random.Range(halfWidth, 2f * halfWidth);
            if (!RocketSet.instance.IsRocketRight())
                x = -x;
        }
        else
        {
            x = Random.Range(-screenWidth * 0.5f, screenWidth * 0.5f);
            y = (screenheight * 0.5f) + Random.Range(halfHeight, 2f * halfHeight);
        }
        return new Vector3(x, y, 0);
    }

    private void SetRandomIndex()
    {
        if(sprites.Length.Equals(1))
        {
            loadedSpriteList = new int[1];
            loadedSpriteList[0] = 0;
            return;
        }

        loadedSpriteList = new int[sprites.Length];
        for(int i = 0; i < loadedSpriteList.Length; i++)
            loadedSpriteList[i] = i;

        for (int i = 0; i < loadedSpriteList.Length; i++)
            Swap(ref loadedSpriteList[i], ref loadedSpriteList[Random.Range(0, loadedSpriteList.Length)]);
    }

    private void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}