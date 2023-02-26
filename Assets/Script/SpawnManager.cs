using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 동적 오브젝트 생성 관리 클래스
/// </summary>
public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// 생성 관리 클래스의 싱글톤 인스턴스
    /// </summary>
    public static SpawnManager instance;

    /// <summary>
    /// 플라잉 오브젝트를 생성할 부모 Transform
    /// </summary>
    public Transform flyingObjectGroup;

    /// <summary>
    /// 각 플라잉 오브젝트 종류별 설정 값
    /// </summary>
    public FlyingGroup[] flyingGroups;
    /// <summary>
    /// 생성 지연 시간
    /// </summary>
    public float spawnDelayTime;
    /// <summary>
    /// 각 플라잉 오브젝트별 생성 확률
    /// </summary>
    public float[] spawnPossibility = new float[4];

    public Launch launch;
    public Camera mainCamera;

    void Awake()
    {
        instance = this;
        StartCoroutine(SpawnChecker());
        // 각 플라잉 오브젝트의 생성 대기열 초기화
        for (int i = 0; i <flyingGroups.Length; i++)
            flyingGroups[i].flightPropQueue = new Queue<FlyingObject>();
    }

    /// <summary>
    /// 생성 주기마다 조건에 따라 생성
    /// </summary>
    private IEnumerator SpawnChecker()
    {
        yield return new WaitForSeconds(spawnDelayTime);
        // 로켓의 y축 위치
        float rocketY = RocketSet.instance.rocket.transform.position.y;
        
        // 랜덤 숫자를 생성하여 각 플라잉 오브젝트의 확률에 따라 생성할 오브젝트 선택
        int r = 0;
        float val = Random.Range(1, 100f), accum = 0;
        for (int i = 0; i < spawnPossibility.Length; i++)
        {
            if (val <= spawnPossibility[i] + accum)
            {
                r = i;
                break;
            }
            else
                accum += spawnPossibility[i];
        }

        // 우주 궤도 일 때
        if (rocketY > launch.heightCriteria[1])
        {
            // 선택된 플라잉 오브젝트의 각 크기의 확률에 따라 크기 선택
            float sizeR = Random.Range(1, 100f);
            int size = 0;
            accum = 0;
            for (int i = 0; i < flyingGroups[r].sizePossibility.Length; i++)
            {
                if (sizeR <= flyingGroups[r].sizePossibility[i] + accum)
                {
                    size = i;
                    break;
                }
                accum += flyingGroups[r].sizePossibility[i];
            }

            // 선택된 크기로 선택된 플라잉 오브젝트 생성
            switch (r)
            {
                case 0:
                    int obsGroupPossibility = Random.Range(0, 100);
                    if (obsGroupPossibility < 85)
                        SpawnFlyingObject(PropType.Obstacle, size);
                    else
                        SetObstacleGroupPosition(size);
                    break;
                case 1:
                    SpawnFlyingObject(PropType.Coin, size);
                    break;
                case 2:
                    SpawnFlyingObject(PropType.Fuel, size);
                    break;
                case 3:
                    SpawnFlyingObject(PropType.Star, size);
                    break;
            }
        }
        // 대기 궤도일 때
        else if(rocketY > launch.heightCriteria[0])
        {
            // 작은 크기의 플라잉 오브젝트 고정 생성 (장애물은 생성되지 않음)
            switch (r)
            {
                case 0:
                    //SpawnFlyingObject(PropType.Obstacle, 0);
                    break;
                case 1:
                    SpawnFlyingObject(PropType.Coin, 0);
                    break;
                case 2:
                    SpawnFlyingObject(PropType.Fuel, 0);
                    break;
                case 3:
                    SpawnFlyingObject(PropType.Star, 0);
                    break;
            }
        }

        // 플레이어가 생존 중일 때에만 반복 생성
        if(PlayManager.isSurvive)
            StartCoroutine(SpawnChecker());
    }

    /// <summary>
    /// 보너스 텍스트를 활성화하고 업데이트한다.
    /// </summary>
    /// <param name="kind">(종류)0: score, 1: coin</param>
    /// <param name="amount">수량</param>
    public void SetBonusText(int kind, int amount)
    {
        if (kind.Equals(0)) // score
        {
            PlayManager.instance.SetBonusText(PlayManager.instance.score_local + " +" + amount, Color.white);
        }
        else if(kind.Equals(1)) // coin
        {
            PlayManager.instance.SetBonusText("coin +" + amount, Color.yellow);
        }
    }

    /// <summary>
    /// 새로운 플라잉 오브젝트 생성
    /// </summary>
    /// <param name="type">종류</param>
    /// <param name="size">크기</param>
    /// <returns>생성된 플라잉 오브젝트</returns>
    private FlyingObject CreateNewProp(PropType type, int size)
    {
        // 프리셋 prefab을 복사하여 생성 후 대기열 등록
        FlyingObject newProp = Instantiate(flyingGroups[(int)type].flightPropPrefabs[size], flyingObjectGroup).GetComponent<FlyingObject>();
        flyingGroups[(int)type].flightPropQueue.Enqueue(newProp);
        newProp.gameObject.SetActive(false);

        return newProp;
    }

    /// <summary>
    /// 플라잉 오브젝트 생성
    /// </summary>
    /// <param name="type">종류</param>
    /// <param name="size">크기</param>
    private void SpawnFlyingObject(PropType type, int size)
    {
        SpawnFlyingObject(type, size, GetRandomSpawnPos((int)type, size), 0);
    }

    /// <summary>
    /// 플라잉 오브젝트 생성
    /// </summary>
    /// <param name="type">종류</param>
    /// <param name="size">크기</param>
    /// <param name="stdPoint">기준점</param>
    /// <param name="rad">생성 범위 반지름</param>
    private void SpawnFlyingObject(PropType type, int size, Vector3 stdPoint, float rad)
    {
        FlyingObject prop = null;
        // 생성할 플라잉 오브젝트가 대기열에 있으면 위치 조정으로 활성화하여 생성
        if (flyingGroups[(int)type].flightPropQueue.Count > 0)
        {
            prop = flyingGroups[(int)type].flightPropQueue.Dequeue();
            prop.gameObject.SetActive(true);
            prop.gameObject.transform.localPosition = stdPoint + new Vector3(Random.Range(-rad, rad), Random.Range(-rad, rad), 0);
        }
        // 플라잉 오브젝트가 대기열에 없는데, 생성 제한량 미만으로 생성 되어 있으면 신규로 생성
        else
        {
            if (flyingGroups[(int)type].flightPropQueue.Count + FlyingObject.spawnedCount[(int)type] < flyingGroups[(int)type].spawnLimit)
            {
                // 신규 생성 및 생성 대기열에서 제거
                prop = CreateNewProp(type, size);
                flyingGroups[(int)type].flightPropQueue.Dequeue();
                // 활성화 후 위치 조정
                prop.gameObject.SetActive(true);
                prop.gameObject.transform.localPosition = GetRandomSpawnPos((int)type, size);
            }
        }
    }

    /// <summary>
    /// 장애물 덩어리 위치 조정
    /// </summary>
    /// <param name="size">크기</param>
    private void SetObstacleGroupPosition(int size)
    {
        // 장애물의 생성 기준점 계산 및 기준점 근처로 대량 생성
        Vector3 stdPoint = GetRandomSpawnPos((int)PropType.Obstacle, size);
        int amount = Random.Range(8, 13);
        int rad = 300;
        for(int i = 0; i < amount; i++)
        {
            SpawnFlyingObject(PropType.Obstacle, size, stdPoint, rad);
        }
    }

    /// <summary>
    /// 새로운 오브젝트 생성
    /// </summary>
    /// <param name="obj">복사할 오브젝트</param>
    /// <param name="parent">생성후 설정할 부모 Transform</param>
    /// <returns>생성한 게임 오브젝트</returns>
    private GameObject CreateNewObject(GameObject obj, Transform parent)
    {
        GameObject newObj = Instantiate(obj, parent);
        newObj.SetActive(false);
        return newObj;
    }

    /// <summary>
    /// 오브젝트 생성
    /// </summary>
    /// <param name="spawnable">생성 대상 프리셋</param>
    /// <returns>생성한 오브젝트</returns>
    public GameObject SpawnObject(Spawnable spawnable)
    {
        // 생성 프리셋 대기열에 오브젝트가 있으면 활성화 및 위치 조정 후 반환
        if(spawnable.waitingQueue.Count > 0)
        {
            GameObject obj = spawnable.waitingQueue.Dequeue();
            spawnable.spawnedCount++;
            obj.transform.position = spawnable.spawnPos;
            obj.SetActive(true);
            return obj;
        }
        // 없으면, 생성 제한 개수 확인 후, 신규 생성하여 위치 조정 후 반환
        else
        {
            if(spawnable.spawnedCount + spawnable.waitingQueue.Count < spawnable.maxSpawn)
            {
                GameObject obj = CreateNewObject(spawnable.sampleObject, spawnable.parent);
                spawnable.spawnedCount++;
                obj.transform.position = spawnable.spawnPos;
                obj.SetActive(true);
                return obj;
            }
        }
        // 모든 조건에 해당되지 않으면 null 반환
        return null;
    }

    /// <summary>
    /// 오브젝트 회수
    /// </summary>
    /// <param name="obj">회수할 오브젝트</param>
    /// <param name="spawnable">해당 오브젝트의 프리셋</param>
    public void ReturnObject(GameObject obj, Spawnable spawnable)
    {
        // 오브젝트 비활성화 및 생성 개수 차감
        obj.SetActive(false);
        spawnable.spawnedCount--;
        // 생성 대기열에 등록
        spawnable.waitingQueue.Enqueue(obj);
    }

    /// <summary>
    /// 플라잉 오브젝트를 랜덤으로 스폰할 위치 계산
    /// </summary>
    /// <param name="type">플라잉 오브젝트 종류의 인덱스</param>
    /// <param name="size">크기</param>
    /// <returns>랜덤한 위치</returns>
    private Vector3 GetRandomSpawnPos(int type, int size)
    {
        int r = Random.Range(0, 2);         // 생성 방향을 랜덤으로 정한다
        float screenWidth = Screen.width;   // 화면의 width
        float screenheight = Screen.height; // 화면의 height
        float x, y;                         // 결과 x, y 위치
        // 플라잉 오브젝트의 half height 계산
        float halfHeight = flyingGroups[type].flightPropPrefabs[size].rect.height * 0.5f * flyingGroups[type].flightPropPrefabs[size].transform.localScale.y;
        // 플라잉 오브젝트의 half width 계산
        float halfWidth = flyingGroups[type].flightPropPrefabs[size].rect.width * 0.5f * flyingGroups[type].flightPropPrefabs[size].transform.localScale.x;
        // 화면의 우측에 생성
        if (r.Equals(0))
        {
            y = Random.Range(-halfHeight - halfHeight * 0.5f, halfHeight + screenheight);
            x = (screenWidth * 0.5f) + Random.Range(halfWidth, 2f * halfWidth);
            if (!RocketSet.instance.IsRocketRight())
                x = -x;
        }
        // 화면의 상단에 생성
        else
        {
            x = Random.Range(-screenWidth * 0.5f, screenWidth * 0.5f);
            y = (screenheight * 0.5f) + Random.Range(halfHeight, 2f * halfHeight);
        }
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// 플라잉 오브젝트 회수 처리
    /// </summary>
    /// <param name="prop">회수할 오브젝트</param>
    public void ReturnFlyingObject(FlyingObject prop)
    {
        // 오브젝트 비활성화
        prop.gameObject.SetActive(false);

        // 각 종류별 대기열로 등록하여 처리
        if (prop is Obstacle)
            flyingGroups[(int)PropType.Obstacle].flightPropQueue.Enqueue(prop);
        else if (prop is CoinDrop)
            flyingGroups[(int)PropType.Coin].flightPropQueue.Enqueue((CoinDrop)prop);
    }

    /// <summary>
    /// 충돌 이펙트 초기화 대기 시작
    /// </summary>
    public void ResetCollideEffectStart()
    {
        StartCoroutine(ResetCollideEffect());
    }

    /// <summary>
    /// 충돌 이펙트 초기화 이펙트 대기 코루틴
    /// </summary>
    IEnumerator ResetCollideEffect()
    {
        yield return new WaitForSeconds(1.0f);
        // 회전값 초기화
        RocketSet.instance.rocketMain_rigid.angularVelocity = 0;
    }
}

/// <summary>
/// 스폰 정보 프리셋 데이터 클래스
/// </summary>
public class Spawnable
{
    /// <summary>
    /// 복사할 원본 샘플 오브젝트
    /// </summary>
    public GameObject sampleObject;
    /// <summary>
    /// 필드에 생성된 오브젝트 수
    /// </summary>
    public int spawnedCount;
    /// <summary>
    /// 생성 대기열
    /// </summary>
    public Queue<GameObject> waitingQueue;
    /// <summary>
    /// 생성 후 Hierarchy 상에서의 부모로 지정할 Transform
    /// </summary>
    public Transform parent;
    /// <summary>
    /// 생성 위치
    /// </summary>
    public Vector3 spawnPos;
    /// <summary>
    /// 최대 생성 개수 제한량
    /// </summary>
    public int maxSpawn;

    public Spawnable(GameObject sampleObject, Transform parent, Vector3 spawnPos, int maxSpawn = 10)
    {
        spawnedCount = 0;
        waitingQueue = new Queue<GameObject>();
        this.sampleObject = sampleObject;
        this.parent = parent;
        this.spawnPos = spawnPos;
        this.maxSpawn = maxSpawn;
    }
}

/// <summary>
/// 각 플라잉 오브젝트의 공통 데이터 클래스
/// </summary>
[System.Serializable]
public class FlyingGroup
{
    /// <summary>
    /// 플라잉 오브젝트의 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 생성 제한량
    /// </summary>
    public int spawnLimit;
    /// <summary>
    /// 크기별 플라잉 오브젝트 RectTtransform
    /// </summary>
    public RectTransform[] flightPropPrefabs;
    /// <summary>
    /// 생성 대기열
    /// </summary>
    public Queue<FlyingObject> flightPropQueue;
    /// <summary>
    /// 크기별 생성 확률
    /// </summary>
    public float[] sizePossibility;
}