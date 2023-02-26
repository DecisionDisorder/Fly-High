using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������Ʈ ���� ���� Ŭ����
/// </summary>
public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// ���� ���� Ŭ������ �̱��� �ν��Ͻ�
    /// </summary>
    public static SpawnManager instance;

    /// <summary>
    /// �ö��� ������Ʈ�� ������ �θ� Transform
    /// </summary>
    public Transform flyingObjectGroup;

    /// <summary>
    /// �� �ö��� ������Ʈ ������ ���� ��
    /// </summary>
    public FlyingGroup[] flyingGroups;
    /// <summary>
    /// ���� ���� �ð�
    /// </summary>
    public float spawnDelayTime;
    /// <summary>
    /// �� �ö��� ������Ʈ�� ���� Ȯ��
    /// </summary>
    public float[] spawnPossibility = new float[4];

    public Launch launch;
    public Camera mainCamera;

    void Awake()
    {
        instance = this;
        StartCoroutine(SpawnChecker());
        // �� �ö��� ������Ʈ�� ���� ��⿭ �ʱ�ȭ
        for (int i = 0; i <flyingGroups.Length; i++)
            flyingGroups[i].flightPropQueue = new Queue<FlyingObject>();
    }

    /// <summary>
    /// ���� �ֱ⸶�� ���ǿ� ���� ����
    /// </summary>
    private IEnumerator SpawnChecker()
    {
        yield return new WaitForSeconds(spawnDelayTime);
        // ������ y�� ��ġ
        float rocketY = RocketSet.instance.rocket.transform.position.y;
        
        // ���� ���ڸ� �����Ͽ� �� �ö��� ������Ʈ�� Ȯ���� ���� ������ ������Ʈ ����
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

        // ���� �˵� �� ��
        if (rocketY > launch.heightCriteria[1])
        {
            // ���õ� �ö��� ������Ʈ�� �� ũ���� Ȯ���� ���� ũ�� ����
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

            // ���õ� ũ��� ���õ� �ö��� ������Ʈ ����
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
        // ��� �˵��� ��
        else if(rocketY > launch.heightCriteria[0])
        {
            // ���� ũ���� �ö��� ������Ʈ ���� ���� (��ֹ��� �������� ����)
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

        // �÷��̾ ���� ���� ������ �ݺ� ����
        if(PlayManager.isSurvive)
            StartCoroutine(SpawnChecker());
    }

    /// <summary>
    /// ���ʽ� �ؽ�Ʈ�� Ȱ��ȭ�ϰ� ������Ʈ�Ѵ�.
    /// </summary>
    /// <param name="kind">(����)0: score, 1: coin</param>
    /// <param name="amount">����</param>
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
    /// ���ο� �ö��� ������Ʈ ����
    /// </summary>
    /// <param name="type">����</param>
    /// <param name="size">ũ��</param>
    /// <returns>������ �ö��� ������Ʈ</returns>
    private FlyingObject CreateNewProp(PropType type, int size)
    {
        // ������ prefab�� �����Ͽ� ���� �� ��⿭ ���
        FlyingObject newProp = Instantiate(flyingGroups[(int)type].flightPropPrefabs[size], flyingObjectGroup).GetComponent<FlyingObject>();
        flyingGroups[(int)type].flightPropQueue.Enqueue(newProp);
        newProp.gameObject.SetActive(false);

        return newProp;
    }

    /// <summary>
    /// �ö��� ������Ʈ ����
    /// </summary>
    /// <param name="type">����</param>
    /// <param name="size">ũ��</param>
    private void SpawnFlyingObject(PropType type, int size)
    {
        SpawnFlyingObject(type, size, GetRandomSpawnPos((int)type, size), 0);
    }

    /// <summary>
    /// �ö��� ������Ʈ ����
    /// </summary>
    /// <param name="type">����</param>
    /// <param name="size">ũ��</param>
    /// <param name="stdPoint">������</param>
    /// <param name="rad">���� ���� ������</param>
    private void SpawnFlyingObject(PropType type, int size, Vector3 stdPoint, float rad)
    {
        FlyingObject prop = null;
        // ������ �ö��� ������Ʈ�� ��⿭�� ������ ��ġ �������� Ȱ��ȭ�Ͽ� ����
        if (flyingGroups[(int)type].flightPropQueue.Count > 0)
        {
            prop = flyingGroups[(int)type].flightPropQueue.Dequeue();
            prop.gameObject.SetActive(true);
            prop.gameObject.transform.localPosition = stdPoint + new Vector3(Random.Range(-rad, rad), Random.Range(-rad, rad), 0);
        }
        // �ö��� ������Ʈ�� ��⿭�� ���µ�, ���� ���ѷ� �̸����� ���� �Ǿ� ������ �űԷ� ����
        else
        {
            if (flyingGroups[(int)type].flightPropQueue.Count + FlyingObject.spawnedCount[(int)type] < flyingGroups[(int)type].spawnLimit)
            {
                // �ű� ���� �� ���� ��⿭���� ����
                prop = CreateNewProp(type, size);
                flyingGroups[(int)type].flightPropQueue.Dequeue();
                // Ȱ��ȭ �� ��ġ ����
                prop.gameObject.SetActive(true);
                prop.gameObject.transform.localPosition = GetRandomSpawnPos((int)type, size);
            }
        }
    }

    /// <summary>
    /// ��ֹ� ��� ��ġ ����
    /// </summary>
    /// <param name="size">ũ��</param>
    private void SetObstacleGroupPosition(int size)
    {
        // ��ֹ��� ���� ������ ��� �� ������ ��ó�� �뷮 ����
        Vector3 stdPoint = GetRandomSpawnPos((int)PropType.Obstacle, size);
        int amount = Random.Range(8, 13);
        int rad = 300;
        for(int i = 0; i < amount; i++)
        {
            SpawnFlyingObject(PropType.Obstacle, size, stdPoint, rad);
        }
    }

    /// <summary>
    /// ���ο� ������Ʈ ����
    /// </summary>
    /// <param name="obj">������ ������Ʈ</param>
    /// <param name="parent">������ ������ �θ� Transform</param>
    /// <returns>������ ���� ������Ʈ</returns>
    private GameObject CreateNewObject(GameObject obj, Transform parent)
    {
        GameObject newObj = Instantiate(obj, parent);
        newObj.SetActive(false);
        return newObj;
    }

    /// <summary>
    /// ������Ʈ ����
    /// </summary>
    /// <param name="spawnable">���� ��� ������</param>
    /// <returns>������ ������Ʈ</returns>
    public GameObject SpawnObject(Spawnable spawnable)
    {
        // ���� ������ ��⿭�� ������Ʈ�� ������ Ȱ��ȭ �� ��ġ ���� �� ��ȯ
        if(spawnable.waitingQueue.Count > 0)
        {
            GameObject obj = spawnable.waitingQueue.Dequeue();
            spawnable.spawnedCount++;
            obj.transform.position = spawnable.spawnPos;
            obj.SetActive(true);
            return obj;
        }
        // ������, ���� ���� ���� Ȯ�� ��, �ű� �����Ͽ� ��ġ ���� �� ��ȯ
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
        // ��� ���ǿ� �ش���� ������ null ��ȯ
        return null;
    }

    /// <summary>
    /// ������Ʈ ȸ��
    /// </summary>
    /// <param name="obj">ȸ���� ������Ʈ</param>
    /// <param name="spawnable">�ش� ������Ʈ�� ������</param>
    public void ReturnObject(GameObject obj, Spawnable spawnable)
    {
        // ������Ʈ ��Ȱ��ȭ �� ���� ���� ����
        obj.SetActive(false);
        spawnable.spawnedCount--;
        // ���� ��⿭�� ���
        spawnable.waitingQueue.Enqueue(obj);
    }

    /// <summary>
    /// �ö��� ������Ʈ�� �������� ������ ��ġ ���
    /// </summary>
    /// <param name="type">�ö��� ������Ʈ ������ �ε���</param>
    /// <param name="size">ũ��</param>
    /// <returns>������ ��ġ</returns>
    private Vector3 GetRandomSpawnPos(int type, int size)
    {
        int r = Random.Range(0, 2);         // ���� ������ �������� ���Ѵ�
        float screenWidth = Screen.width;   // ȭ���� width
        float screenheight = Screen.height; // ȭ���� height
        float x, y;                         // ��� x, y ��ġ
        // �ö��� ������Ʈ�� half height ���
        float halfHeight = flyingGroups[type].flightPropPrefabs[size].rect.height * 0.5f * flyingGroups[type].flightPropPrefabs[size].transform.localScale.y;
        // �ö��� ������Ʈ�� half width ���
        float halfWidth = flyingGroups[type].flightPropPrefabs[size].rect.width * 0.5f * flyingGroups[type].flightPropPrefabs[size].transform.localScale.x;
        // ȭ���� ������ ����
        if (r.Equals(0))
        {
            y = Random.Range(-halfHeight - halfHeight * 0.5f, halfHeight + screenheight);
            x = (screenWidth * 0.5f) + Random.Range(halfWidth, 2f * halfWidth);
            if (!RocketSet.instance.IsRocketRight())
                x = -x;
        }
        // ȭ���� ��ܿ� ����
        else
        {
            x = Random.Range(-screenWidth * 0.5f, screenWidth * 0.5f);
            y = (screenheight * 0.5f) + Random.Range(halfHeight, 2f * halfHeight);
        }
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// �ö��� ������Ʈ ȸ�� ó��
    /// </summary>
    /// <param name="prop">ȸ���� ������Ʈ</param>
    public void ReturnFlyingObject(FlyingObject prop)
    {
        // ������Ʈ ��Ȱ��ȭ
        prop.gameObject.SetActive(false);

        // �� ������ ��⿭�� ����Ͽ� ó��
        if (prop is Obstacle)
            flyingGroups[(int)PropType.Obstacle].flightPropQueue.Enqueue(prop);
        else if (prop is CoinDrop)
            flyingGroups[(int)PropType.Coin].flightPropQueue.Enqueue((CoinDrop)prop);
    }

    /// <summary>
    /// �浹 ����Ʈ �ʱ�ȭ ��� ����
    /// </summary>
    public void ResetCollideEffectStart()
    {
        StartCoroutine(ResetCollideEffect());
    }

    /// <summary>
    /// �浹 ����Ʈ �ʱ�ȭ ����Ʈ ��� �ڷ�ƾ
    /// </summary>
    IEnumerator ResetCollideEffect()
    {
        yield return new WaitForSeconds(1.0f);
        // ȸ���� �ʱ�ȭ
        RocketSet.instance.rocketMain_rigid.angularVelocity = 0;
    }
}

/// <summary>
/// ���� ���� ������ ������ Ŭ����
/// </summary>
public class Spawnable
{
    /// <summary>
    /// ������ ���� ���� ������Ʈ
    /// </summary>
    public GameObject sampleObject;
    /// <summary>
    /// �ʵ忡 ������ ������Ʈ ��
    /// </summary>
    public int spawnedCount;
    /// <summary>
    /// ���� ��⿭
    /// </summary>
    public Queue<GameObject> waitingQueue;
    /// <summary>
    /// ���� �� Hierarchy �󿡼��� �θ�� ������ Transform
    /// </summary>
    public Transform parent;
    /// <summary>
    /// ���� ��ġ
    /// </summary>
    public Vector3 spawnPos;
    /// <summary>
    /// �ִ� ���� ���� ���ѷ�
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
/// �� �ö��� ������Ʈ�� ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class FlyingGroup
{
    /// <summary>
    /// �ö��� ������Ʈ�� �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// ���� ���ѷ�
    /// </summary>
    public int spawnLimit;
    /// <summary>
    /// ũ�⺰ �ö��� ������Ʈ RectTtransform
    /// </summary>
    public RectTransform[] flightPropPrefabs;
    /// <summary>
    /// ���� ��⿭
    /// </summary>
    public Queue<FlyingObject> flightPropQueue;
    /// <summary>
    /// ũ�⺰ ���� Ȯ��
    /// </summary>
    public float[] sizePossibility;
}