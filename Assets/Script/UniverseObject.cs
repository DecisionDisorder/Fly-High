using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 우주 배경 오브젝트 클래스
/// </summary>
[System.Serializable]
public class UniverseObject
{
    /// <summary>
    /// 우주 배경 오브젝트 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 화면에 표시할 이미지
    /// </summary>
    public Image[] images;
    /// <summary>
    /// 우주 배경 스프라이트 이미지 리소스
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// 현재 표시중인 스프라이트 이미지 리소스의 인덱스 리스트
    /// </summary>
    public int[] loadedSpriteList;
    /// <summary>
    /// 최근에 불러온 스프라이트 이미지 리소스들의 인덱스
    /// </summary>
    private int loadedSpriteIndex = -2;
    /// <summary>
    /// 최근에 표시한 이미지 인덱스의 큐
    /// </summary>
    public Queue<int> loadedImageIndex = new Queue<int>();
    /// <summary>
    /// 최근에 표시한 이미지 인덱스
    /// </summary>
    private int lastImageIndex = -1;

    /// <summary>
    /// 표시중인 배경 이미지 회수
    /// </summary>
    public void ReturnImage()
    {
        images[loadedImageIndex.Dequeue()].gameObject.SetActive(false);
    }

    /// <summary>
    /// 무작위로 스프라이트를 선정하여 우주 배경 이미지에 적용한다.
    /// </summary>
    /// <returns>적용된 이미지</returns>
    public Image SetRandomImage()
    {
        // 배경 이미지 생성 여유가 있는지 확인
        if(loadedImageIndex.Count < images.Length)
        {
            // 불러온 스프라이트 인덱스 기록에 1을 더한다.
            loadedSpriteIndex++;
            // 무작위로 설정된 순서를 모두 돌았거나, 처음으로 배경을 설정하는 경우, 스프라이트 이미지의 생성 순서를 다시 설정한다.
            if (loadedSpriteIndex >= sprites.Length || loadedSpriteIndex.Equals(-1))
            {
                SetRandomIndex();
                loadedSpriteIndex = 0;
            }

            // 배경 이미지 인덱스 + 1 후, 불러온 이미지 큐에 등록
            int newImageIndex = (lastImageIndex + 1 < images.Length ? ++lastImageIndex : 0);
            loadedImageIndex.Enqueue(newImageIndex);
            // 이미지 스프라이트 적용
            images[newImageIndex].sprite = sprites[loadedSpriteList[loadedSpriteIndex]];
            // 크기와 위치를 일정 범위 내의 무작위로 설정한 후에 활성화 한다.
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
    /// 랜덤으로 배경 이미지를 스폰할 위치 계산
    /// </summary>
    /// <param name="index">배경 이미지 인덱스</param>
    /// <returns>위치</returns>
    private Vector3 GetRandomPosition(int index)
    {
        int r = Random.Range(0, 2);
        float screenWidth = Screen.width;   // 화면 width
        float screenheight = Screen.height; // 화면 height
        float x, y;
        // 스프라이트 이미지의 half height
        float halfHeight = sprites[loadedSpriteList[loadedSpriteIndex]].rect.height * 0.5f * images[index].transform.localScale.y; 
        // 스프라이트 이미지의 half width
        float halfWidth = sprites[loadedSpriteList[loadedSpriteIndex]].rect.width * 0.5f * images[index].transform.localScale.x;

        // 반반의 확률로 화면의 위쪽 혹은 오른쪽(왼쪽)의 위치로 계산한다.
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
    /// 스프라이트 이미지의 생성 순서를 무작위로 설정
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
    /// 두 숫자를 서로 swap한다.
    /// </summary>
    /// <param name="a">숫자 1</param>
    /// <param name="b">숫자 2</param>
    private void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}