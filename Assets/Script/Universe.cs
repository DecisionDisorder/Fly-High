using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 우주/하늘 배경 오브젝트를 총괄하는 클래스
/// </summary>
public class Universe : MonoBehaviour
{
    /// <summary>
    /// 우주 배경 총괄 클래스의 싱글톤 인스턴스
    /// </summary>
    public static Universe instance;
    /// <summary>
    /// 우주 배경 단계
    /// </summary>
    public enum UniverseUnit { Sky, Atmosphere, NearTheEarth, SolarSystem, OutsideSolarSystem, IntermediateOfMilkyWay, OutskirtOfMilkyWay }
    /// <summary>
    /// 현재 우주 배경 단계 (하늘로 시작)
    /// </summary>
    private UniverseUnit universeUnit = UniverseUnit.Sky;
    /// <summary>
    /// 배경 구분 기준 높이(고도)
    /// </summary>
    public int[] unitCriteria = new int[7];

    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// 먼 우주 배경의 우주 배경 오브젝트
    /// </summary>
    public UniverseObject farUniverse;
    /// <summary>
    /// 랜덤한 우주 배경의 우주 배경 오브젝트
    /// </summary>
    public UniverseObject randomUniverse;
    /// <summary>
    /// 행성 배경의 우주 배경 오브젝트
    /// </summary>
    public UniverseObject planet;

    /// <summary>
    /// 먼 우주 배경 생성 코루틴
    /// </summary>
    IEnumerator createFarUniverse;
    /// <summary>
    /// 랜덤 우주 배경 생성 코루틴
    /// </summary>
    IEnumerator createRandomUniverse;
    /// <summary>
    /// 행성 배경 생성 코루틴
    /// </summary>
    IEnumerator createPlanet;
    /// <summary>
    /// 구름 생성 코루틴
    /// </summary>
    IEnumerator createCloud;

    /// <summary>
    /// 구름 스프라이트 이미지
    /// </summary>
    public Sprite[] cloudSprites;
    /// <summary>
    /// 구름 이미지 렌더러
    /// </summary>
    public SpriteRenderer[] cloudImages;

    /// <summary>
    /// 은하수 배경 색상
    /// </summary>
    public Color milkyWayBackgroundColor;
    /// <summary>
    /// 배경 제어 오브젝트
    /// </summary>
    public BackgroundControl backgroundControl;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(CreateTimer(1f));
    }

    /// <summary>
    /// 로켓의 고도에 따라서 현재 우주 배경 단계를 계산
    /// </summary>
    /// <returns>현재 우주 배경 단계</returns>
    public UniverseUnit GetUniverseUnit()
    {
        float rocketY = RocketSet.instance.rocket.transform.position.y;
        float distance = Vector3.Distance(Vector3.zero, RocketSet.instance.rocket.transform.position);

        for (int i = (int)universeUnit; i < unitCriteria.Length - 1; i++)
        {
            if(i < 3)
            {
                if (rocketY < unitCriteria[i])
                    return universeUnit = (UniverseUnit)i;
            }
            else
            {
                if (distance < unitCriteria[i])
                    return universeUnit = (UniverseUnit)i;
            }
        }

        return universeUnit = UniverseUnit.OutskirtOfMilkyWay;
    }

    /// <summary>
    /// 우주 배경 생성 시작
    /// </summary>
    /// <param name="createUniverse">배경 생성 코루틴</param>
    /// <param name="delay">생성 간격</param>
    /// <param name="universeObject">대상 우주 배경 오브젝트</param>
    private void StartCreateUniverse(IEnumerator createUniverse, float delay, UniverseObject universeObject)
    {
        if (createUniverse != null)
            StopCoroutine(createUniverse);

        createUniverse = CreateUniverse(delay, universeObject);
        StartCoroutine(createUniverse);
    }

    /// <summary>
    /// 생성 대기 코루틴
    /// </summary>
    /// <param name="delay">생성 간격</param>
    IEnumerator CreateTimer(float delay)
    {
        // 각 우주 배경 단계 조건에 따라서 생성 주기를 조절하여 각각의 코루틴 시작
        switch(GetUniverseUnit())
        {
            case UniverseUnit.Sky:
                if (createCloud == null)
                {
                    createCloud = CreateCloud(0.45f);
                    StartCoroutine(createCloud);
                }
                break;
            case UniverseUnit.Atmosphere:
                if (createCloud == null)
                {
                    StopCoroutine(createCloud);
                    createCloud = CreateCloud(0.1f);
                    StartCoroutine(createCloud);
                }
                break;
            case UniverseUnit.NearTheEarth:
                if(createRandomUniverse == null)
                    StartCreateUniverse(createRandomUniverse, 20f, randomUniverse);
                break;
            case UniverseUnit.SolarSystem:
                if (createPlanet == null)
                    StartCreateUniverse(createPlanet, 30f, planet);
                if(createFarUniverse == null)
                    StartCreateUniverse(createFarUniverse, 60f, farUniverse);
                break;
            case UniverseUnit.OutsideSolarSystem:
                StopCoroutine(createPlanet);
                break;
            case UniverseUnit.IntermediateOfMilkyWay:
                backgroundControl.ChangeBackgroundColor(milkyWayBackgroundColor, 3.0f);
                break;
            case UniverseUnit.OutskirtOfMilkyWay:
                break;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(CreateTimer(delay));
    }

    /// <summary>
    /// 우주 배경 생성 코루틴
    /// </summary>
    /// <param name="delay">생성 주기</param>
    /// <param name="universeObject">생성 대상 우주 배경 오브젝트</param>
    /// <param name="first">처음 생성인지 여부</param>
    IEnumerator CreateUniverse(float delay, UniverseObject universeObject, bool first = true)
    {
        if (first)
            yield return new WaitForSeconds(Random.Range(0, delay));
        else
            yield return new WaitForSeconds(delay);

        universeObject.SetRandomImage();

        StartCoroutine(CreateUniverse(delay, universeObject, false));
    }

    /// <summary>
    /// 구름 생성 코루틴
    /// </summary>
    /// <param name="delay">생성 주기</param>
    /// <param name="index">구름 인덱스</param>
    /// <param name="isFirst">처음 생성인지 여부</param>
    /// <returns></returns>
    IEnumerator CreateCloud(float delay, int index = 0, bool isFirst = true)
    {
        // 첫 생성이면 대기 없이 바로 생성
        if (isFirst)
            yield return null;
        else
            yield return new WaitForSeconds(delay);

        // 클라우드 이미지가 소환되지 않은 상태이면 활성화(소환) 처리
        if (!cloudImages[index].gameObject.activeInHierarchy)
        {
            // 구름 이미지 랜덤 선택
            cloudImages[index].sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            // 활성화
            cloudImages[index].gameObject.SetActive(true);
            // 우주 단계 계산 후 배경에 따라 크기 설정
            UniverseUnit unit = GetUniverseUnit();
            if (unit.Equals(UniverseUnit.Sky))
                cloudImages[index].size = new Vector2(Random.Range(5f, 10f), Random.Range(3.5f, 6f));
            else if(unit.Equals(UniverseUnit.Atmosphere))
                cloudImages[index].size = new Vector2(Random.Range(10f, 20f), Random.Range(8f, 25f));
            // 구름 위치 배정
            cloudImages[index].transform.position = GetRandomCloudPosition(cloudImages[index].size, isFirst);
            cloudImages[index].sortingOrder = Random.Range(2, 5);
            cloudImages[index].color = new Vector4(1, 1, 1, Random.Range(0.55f, 0.8f));
            index = (index + 1) % cloudImages.Length;
        }

        // 하늘/대기권 배경에서만 추가 생성 되도록 반복
        if (universeUnit.Equals(UniverseUnit.Sky) || universeUnit.Equals(UniverseUnit.Atmosphere))
            StartCoroutine(CreateCloud(delay, index, false));
    }

    /// <summary>
    /// 구름의 랜덤한 위치를 계산하는 함수
    /// </summary>
    /// <param name="size">구름의 크기</param>
    /// <param name="isFirst">첫 생성인지 여부</param>
    /// <returns></returns>
    private Vector3 GetRandomCloudPosition(Vector2 size, bool isFirst)
    {
        // 카메라 기준 위치 계산
        Vector3 cameraRightDown = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 0f, 0f));
        Vector3 cameraRightUp = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        // 처음 생성이면 특정 위치에 생성
        if (isFirst)
        {
            Vector3 onCamera = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.75f, 0f));
            return new Vector3(onCamera.x + Random.Range(0, cameraRightDown.x), Random.Range(onCamera.y, onCamera.y * 2f), 0);
        }

        // 카메라 기준 오른쪽 혹은 위에 생성
        int r = Random.Range(0, 2);
        if (r.Equals(0))
        {
            float x = cameraRightDown.x + Random.Range(size.x * 0.5f, size.x * 1.5f);
            if (!RocketSet.instance.IsRocketRight())
                x = -x;
            return new Vector3(x, cameraRightDown.y + Random.Range(size.y * 1.5f, cameraRightUp.y + size.y * 1.5f), 0);
        }
        else
        {
            float y = cameraRightUp.y + Random.Range(size.y * 3f, size.y * 5f);
            if (y < unitCriteria[1])
                return new Vector3(Random.Range(0, cameraRightDown.x), y, 0);
            else
                return new Vector3(Random.Range(0, cameraRightDown.x), unitCriteria[1] - Random.Range(size.y, size.y * 2f), 0);
        }
    }
}