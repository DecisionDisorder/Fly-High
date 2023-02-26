using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배경 제어 클래스
/// </summary>
public class BackgroundControl : MonoBehaviour
{
    /// <summary>
    /// 번갈아가며 표시될 배경 오브젝트 배열
    /// </summary>
    public GameObject[] backgrounds = new GameObject[2];
    /// <summary>
    /// 번갈아가며 표시될 땅 오브젝트 배열
    /// </summary>
    public GameObject[] grounds = new GameObject[2];
    /// <summary>
    /// 시작 블럭 오브젝트
    /// </summary>
    public GameObject bottomBlock;
    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// 배경이 플레이어 로켓을 따라와야 하는지 여부
    /// </summary>
    public static bool isBackgroundFollow { private set; get; }

    /// <summary>
    /// 배경 폭 (좌표계 기준)
    /// </summary>
    private float distance = 80;
    
    /// <summary>
    /// 현재 로켓이 지나가고 있는 배경 인덱스 값
    /// </summary>
    private int onGround = 0;

    /// <summary>
    /// 배경 및 땅의 초기 좌표
    /// </summary>
    private Vector3[] resetPos;

    /// <summary>
    /// 하늘 배경 색상
    /// </summary>
    public Color skyColor;
    /// <summary>
    /// 대기권 배경 색상
    /// </summary>
    public Color atmosphereColor;
    /// <summary>
    /// 우주 배경 색상
    /// </summary>
    public Color universeColor;

    /// <summary>
    /// 그라데이션 배경 적용을 위한 텍스처 배열
    /// </summary>
    private Texture2D[] backgroundTextures = new Texture2D[2];
    /// <summary>
    /// 하늘 배경 스프라이트 렌더러
    /// </summary>
    public SpriteRenderer[] skyRenderers;
    /// <summary>
    /// 우주 배경 스프라이트 렌더러
    /// </summary>
    public SpriteRenderer[] universeRenderers;

    private void Awake()
    {
        InitializeBackground();
        isBackgroundFollow = true;
    }

    private void Start()
    {
        resetPos = new Vector3[backgrounds.Length + grounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            resetPos[i] = backgrounds[i].transform.position;
            resetPos[i + backgrounds.Length] = grounds[i].transform.position;
        }

        StartCoroutine(BackgroundFollow());
    }

    /// <summary>
    /// 배경 색상 초기화 작업
    /// </summary>
    private void InitializeBackground()
    {
        // 그라데이션이 필요한 배경 텍스처 초기화 및 하늘-대기권 / 대기권-우주 배경 그라데이션 색상 계산
        for(int i = 0; i < 2; i++)
        {
            backgroundTextures[i] = new Texture2D(1, 2);
            backgroundTextures[i].wrapMode = TextureWrapMode.Clamp;
            backgroundTextures[i].filterMode = FilterMode.Bilinear;
        }
        for (int j = 0; j < skyRenderers.Length; j++)
            SetColor(skyColor, atmosphereColor, skyRenderers[j], 0);
        for (int j = 0; j < universeRenderers.Length; j++)
            SetColor(atmosphereColor, universeColor, universeRenderers[j], 1);
    }

    /// <summary>
    /// color1과 color2의 그라데이션 색상을 계산하여 대상 renderer에 생성된 그라데이션 스프라이트 이미지 적용
    /// </summary>
    /// <param name="color1">아래쪽 색상</param>
    /// <param name="color2">위쪽 색상</param>
    /// <param name="renderer">대상 renderer</param>
    /// <param name="i">배경 텍스처 인덱스</param>
    private void SetColor(Color color1, Color color2, SpriteRenderer renderer, int i)
    {
        backgroundTextures[i].SetPixels(new Color[] { color1, color2 });
        backgroundTextures[i].Apply();
        Rect rect = new Rect(0, 0, backgroundTextures[i].width, backgroundTextures[i].height);
        renderer.sprite = Sprite.Create(backgroundTextures[i], rect, new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// 우주 환경에서의 다양한 배경 색상 변경
    /// </summary>
    /// <param name="color">적용할 색상</param>
    /// <param name="delayTime">적용 딜레이 시간(초)</param>
    public void ChangeBackgroundColor(Color color, float delayTime)
    {
        StartCoroutine(CChangeBackgroundColor(color, delayTime));
    }

    /// <summary>
    /// 카메라의 배경 색상을 서서히 대상 색상으로 변경하는 코루틴
    /// </summary>
    /// <param name="color">적용할 색상</param>
    /// <param name="delayTime">적용 딜레이 시간(초)</param>
    /// <param name="passedTime">지난 시간(초)</param>
    /// <returns></returns>
    IEnumerator CChangeBackgroundColor(Color color, float delayTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        // 지난 시간 계산하여 지난 시간 대비 딜레이 시간 비율 계산
        passedTime += Time.deltaTime;
        float delta = passedTime / delayTime;

        // 기존 색상에서 적용될 색상으로 상기 비율만큼 적용
        mainCamera.backgroundColor += delta * (color - mainCamera.backgroundColor);

        if(passedTime < delayTime)
            StartCoroutine(CChangeBackgroundColor(color, delayTime, passedTime));
    }

    /// <summary>
    /// 플레이어 로켓의 위치에 따라서 배경을 이동시키는 코루틴
    /// </summary>
    IEnumerator BackgroundFollow()
    {
        yield return new WaitForSeconds(0.1f);

        // 로켓의 x 좌표를 기준으로 계산
        float rocketX = RocketSet.instance.rocket.transform.position.x;

        // 로켓의 진행 방향이 화면 상 오른쪽일 때
        if (RocketSet.instance.rocketMain_rigid.velocity.x >= 0)
        {
            // 로켓의 x좌표 값이 현재 배경의 x좌표 값을 오른쪽으로 지나쳤을 때 (카메라에 노출되지 않고 있는) 왼쪽 배경을 오른쪽으로 배치
            if (backgrounds[onGround].transform.position.x < rocketX)
            {
                backgrounds[SwitchGroundNum(onGround - 1)].transform.position = backgrounds[onGround].transform.position + new Vector3(distance, 0, 0);
                grounds[SwitchGroundNum(onGround - 1)].transform.position = grounds[onGround].transform.position + new Vector3(distance, 0, 0);
                onGround = SwitchGroundNum(onGround + 1);
            }
        }
        // 로켓의 진행 방향이 화면 상 왼쪽일 때
        else
        {
            // 로켓의 x좌표 값이 현재 배경의 x좌표 값을 왼쪽으로 지나쳤을 때 (카메라에 노출되지 않고 있는) 오른쪽 배경을 왼쪽으로 배치
            if (backgrounds[onGround].transform.position.x > rocketX)
            {
                backgrounds[SwitchGroundNum(onGround - 1)].transform.position = backgrounds[onGround].transform.position - new Vector3(distance, 0, 0);
                grounds[SwitchGroundNum(onGround - 1)].transform.position = grounds[onGround].transform.position - new Vector3(distance, 0, 0);
                onGround = SwitchGroundNum(onGround + 1);
            }
        }

        // 로켓의 y좌표 값이 일정 수준 이하일 때 배경이 따라간다.
        if (RocketSet.instance.rocket.transform.position.y < distance * 4)
            StartCoroutine(BackgroundFollow());
        // 로켓이 일정 수준 이상으로 올라가면 배경이 로켓을 따라가지 않고 비활성화 된다.
        else
        {
            isBackgroundFollow = false;
            for(int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].SetActive(false);
                grounds[i].SetActive(false);
            }
            bottomBlock.SetActive(false);
        }
    }

    /// <summary>
    /// 조작 대상의 배경 인덱스를 교체한다.
    /// </summary>
    /// <param name="n">변경될 값(+/- 계산이 적용된 값)</param>
    /// <returns>Clamp된 0 or 1</returns>
    private int SwitchGroundNum(int n)
    {
        if (n < 0)
            n = 1;
        if (n > 1)
            n = 0;
        return n;
    }

    /// <summary>
    /// 배경 초기화
    /// </summary>
    public void ResetBackground()
    {
        for(int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.position = resetPos[i];
            grounds[i].transform.position = resetPos[backgrounds.Length + i];
        }
        onGround = 0;
    }
}
