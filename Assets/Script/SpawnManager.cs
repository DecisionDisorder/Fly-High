using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public Transform flyingObjectGroup;

    /* Obstacles */
    public FlyingGroup[] flyingGroups;
    public float spawnDelayTime;
    public float[] spawnPossibility = new float[4];

    public Launch launch;
    public Camera mainCamera;

    void Awake()
    {
        instance = this;
        StartCoroutine(SpawnChecker());
        for (int i = 0; i <flyingGroups.Length; i++)
            flyingGroups[i].flightPropQueue = new Queue<FlyingObject>();
    }

    private IEnumerator SpawnChecker()
    {
        yield return new WaitForSeconds(spawnDelayTime);
        float rocketY = RocketSet.instance.rocket.transform.position.y;
        
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

        if (rocketY > launch.heightCriteria[1]) // universe
        {
            // middle / large / small
            float sizeR = Random.Range(1, 100f);
            int size = 0;
            accum = 0;
            for (int i = 0; i < flyingGroups[r].sizePossibility.Length; i++)
            {
                //Debug.Log(sizeR + ", " + r + ", " + i);
                if (sizeR <= flyingGroups[r].sizePossibility[i] + accum)
                {
                    size = i;
                    break;
                }
                accum += flyingGroups[r].sizePossibility[i];
            }

            switch(r)
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
        else if(rocketY > launch.heightCriteria[0]) // sky
        {
            // small
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

        if(PlayManager.isSurvive)
            StartCoroutine(SpawnChecker());
    }

    /// <summary>
    /// Active the Bonus text and set text.
    /// </summary>
    /// <param name="kind">0: score, 1: coin</param>
    /// <param name="amount">amount</param>
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

    private FlyingObject CreateNewProp(PropType type, int size)
    {
        FlyingObject newProp = Instantiate(flyingGroups[(int)type].flightPropPrefabs[size], flyingObjectGroup).GetComponent<FlyingObject>();
        flyingGroups[(int)type].flightPropQueue.Enqueue(newProp);
        newProp.gameObject.SetActive(false);

        return newProp;
    }

    private void SpawnFlyingObject(PropType type, int size)
    {
        SpawnFlyingObject(type, size, GetRandomSpawnPos((int)type, size), 0);
    }

    private void SpawnFlyingObject(PropType type, int size, Vector3 stdPoint, float rad)
    {
        FlyingObject prop = null;
        if (flyingGroups[(int)type].flightPropQueue.Count > 0)
        {
            prop = flyingGroups[(int)type].flightPropQueue.Dequeue();
            prop.gameObject.SetActive(true);
            prop.gameObject.transform.localPosition = stdPoint + new Vector3(Random.Range(-rad, rad), Random.Range(-rad, rad), 0);
        }
        else
        {
            if (flyingGroups[(int)type].flightPropQueue.Count + FlyingObject.spawnedCount[(int)type] < flyingGroups[(int)type].spawnLimit)
            {
                prop = CreateNewProp(type, size);
                flyingGroups[(int)type].flightPropQueue.Dequeue();
                prop.gameObject.SetActive(true);
                prop.gameObject.transform.localPosition = GetRandomSpawnPos((int)type, size);
            }
        }
    }

    private void SetObstacleGroupPosition(int size)
    {
        Vector3 stdPoint = GetRandomSpawnPos((int)PropType.Obstacle, size);
        int amount = Random.Range(8, 13);
        int rad = 300;
        for(int i = 0; i < amount; i++)
        {
            SpawnFlyingObject(PropType.Obstacle, size, stdPoint, rad);
        }
    }

    private GameObject CreateNewObject(GameObject obj, Transform parent)
    {
        GameObject newObj = Instantiate(obj, parent);
        newObj.SetActive(false);
        return newObj;
    }

    public GameObject SpawnObject(Spawnable spawnable)
    {
        if(spawnable.waitingQueue.Count > 0)
        {
            GameObject obj = spawnable.waitingQueue.Dequeue();
            spawnable.spawnedCount++;
            obj.transform.position = spawnable.spawnPos;
            obj.SetActive(true);
            return obj;
        }
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
        return null;
    }

    public void ReturnObject(GameObject obj, Spawnable spawnable)
    {
        obj.SetActive(false);
        spawnable.spawnedCount--;
        spawnable.waitingQueue.Enqueue(obj);
    }

    private Vector3 GetRandomSpawnPos(int type, int size)
    {
        int r = Random.Range(0, 2);
        float screenWidth = Screen.width;
        float screenheight = Screen.height;
        float x, y;
        float halfHeight = flyingGroups[type].flightPropPrefabs[size].rect.height * 0.5f * flyingGroups[type].flightPropPrefabs[size].transform.localScale.y;
        float halfWidth = flyingGroups[type].flightPropPrefabs[size].rect.width * 0.5f * flyingGroups[type].flightPropPrefabs[size].transform.localScale.x;
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

    public void ReturnObstacle(FlyingObject prop)
    {
        prop.gameObject.SetActive(false);

        if (prop is Obstacle)
            flyingGroups[(int)PropType.Obstacle].flightPropQueue.Enqueue(prop);
        else if (prop is CoinDrop)
            flyingGroups[(int)PropType.Coin].flightPropQueue.Enqueue((CoinDrop)prop);
    }

    public void ResetCollideEffectStart()
    {
        StartCoroutine(ResetCollideEffect());
    }

    IEnumerator ResetCollideEffect()
    {
        yield return new WaitForSeconds(1.0f);
        RocketSet.instance.rocketMain_rigid.angularVelocity = 0;
    }
}

public class Spawnable
{
    public GameObject sampleObject;
    public int spawnedCount;
    public Queue<GameObject> waitingQueue;
    public Transform parent;
    public Vector3 spawnPos;
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

[System.Serializable]
public class FlyingGroup
{
    public string name;
    public int spawnLimit;
    public RectTransform[] flightPropPrefabs;
    public Queue<FlyingObject> flightPropQueue;
    public float[] sizePossibility;
}