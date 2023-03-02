using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ��� ������Ʈ Ŭ����
/// </summary>
[System.Serializable]
public class UniverseObject
{
    /// <summary>
    /// ���� ��� ������Ʈ �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// ȭ�鿡 ǥ���� �̹���
    /// </summary>
    public Image[] images;
    /// <summary>
    /// ���� ��� ��������Ʈ �̹��� ���ҽ�
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// ���� ǥ������ ��������Ʈ �̹��� ���ҽ��� �ε��� ����Ʈ
    /// </summary>
    public int[] loadedSpriteList;
    /// <summary>
    /// �ֱٿ� �ҷ��� ��������Ʈ �̹��� ���ҽ����� �ε���
    /// </summary>
    private int loadedSpriteIndex = -2;
    /// <summary>
    /// �ֱٿ� ǥ���� �̹��� �ε����� ť
    /// </summary>
    public Queue<int> loadedImageIndex = new Queue<int>();
    /// <summary>
    /// �ֱٿ� ǥ���� �̹��� �ε���
    /// </summary>
    private int lastImageIndex = -1;

    /// <summary>
    /// ǥ������ ��� �̹��� ȸ��
    /// </summary>
    public void ReturnImage()
    {
        images[loadedImageIndex.Dequeue()].gameObject.SetActive(false);
    }

    /// <summary>
    /// �������� ��������Ʈ�� �����Ͽ� ���� ��� �̹����� �����Ѵ�.
    /// </summary>
    /// <returns>����� �̹���</returns>
    public Image SetRandomImage()
    {
        // ��� �̹��� ���� ������ �ִ��� Ȯ��
        if(loadedImageIndex.Count < images.Length)
        {
            // �ҷ��� ��������Ʈ �ε��� ��Ͽ� 1�� ���Ѵ�.
            loadedSpriteIndex++;
            // �������� ������ ������ ��� ���Ұų�, ó������ ����� �����ϴ� ���, ��������Ʈ �̹����� ���� ������ �ٽ� �����Ѵ�.
            if (loadedSpriteIndex >= sprites.Length || loadedSpriteIndex.Equals(-1))
            {
                SetRandomIndex();
                loadedSpriteIndex = 0;
            }

            // ��� �̹��� �ε��� + 1 ��, �ҷ��� �̹��� ť�� ���
            int newImageIndex = (lastImageIndex + 1 < images.Length ? ++lastImageIndex : 0);
            loadedImageIndex.Enqueue(newImageIndex);
            // �̹��� ��������Ʈ ����
            images[newImageIndex].sprite = sprites[loadedSpriteList[loadedSpriteIndex]];
            // ũ��� ��ġ�� ���� ���� ���� �������� ������ �Ŀ� Ȱ��ȭ �Ѵ�.
            float randomSize = Random.Range(0.5f, 1.2f);
            images[newImageIndex].transform.localScale = new Vector3(randomSize, randomSize, 1);
            images[newImageIndex].SetNativeSize();
            images[newImageIndex].transform.localPosition = GetRandomPosition(newImageIndex);
            images[newImageIndex].gameObject.SetActive(true);
            return images[newImageIndex];
        }

        return null;
    } 
    
    /// <summary>
    /// �������� ��� �̹����� ������ ��ġ ���
    /// </summary>
    /// <param name="index">��� �̹��� �ε���</param>
    /// <returns>��ġ</returns>
    private Vector3 GetRandomPosition(int index)
    {
        int r = Random.Range(0, 2);
        float screenWidth = Screen.width;   // ȭ�� width
        float screenheight = Screen.height; // ȭ�� height
        float x, y;
        // ��������Ʈ �̹����� half height
        float halfHeight = sprites[loadedSpriteList[loadedSpriteIndex]].rect.height * 0.5f * images[index].transform.localScale.y; 
        // ��������Ʈ �̹����� half width
        float halfWidth = sprites[loadedSpriteList[loadedSpriteIndex]].rect.width * 0.5f * images[index].transform.localScale.x;

        // �ݹ��� Ȯ���� ȭ���� ���� Ȥ�� ������(����)�� ��ġ�� ����Ѵ�.
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

    /// <summary>
    /// ��������Ʈ �̹����� ���� ������ �������� ����
    /// </summary>
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

    /// <summary>
    /// �� ���ڸ� ���� swap�Ѵ�.
    /// </summary>
    /// <param name="a">���� 1</param>
    /// <param name="b">���� 2</param>
    private void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}