using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public static Universe instance;
    public enum UniverseUnit { Sky, Atmosphere, NearTheEarth, SolarSystem, OutsideSolarSystem, IntermediateOfMilkyWay, OutskirtOfMilkyWay }
    private UniverseUnit universeUnit = UniverseUnit.Sky;
    public int[] unitCriteria = new int[7];

    public Camera mainCamera;

    public UniverseObject farUniverse;
    public UniverseObject randomUniverse;
    public UniverseObject planet;

    IEnumerator createFarUniverse;
    IEnumerator createRandomUniverse;
    IEnumerator createPlanet;
    IEnumerator createCloud;

    public Sprite[] cloudSprites;
    public SpriteRenderer[] cloudImages;

    public Color milkyWayBackgroundColor;
    public BackgroundControl backgroundControl;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(CreateTimer(1f));
    }

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

    private void StartCreateUniverse(IEnumerator createUniverse, float delay, UniverseObject universeObject)
    {
        if (createUniverse != null)
            StopCoroutine(createUniverse);

        createUniverse = CreateUniverse(delay, universeObject);
        StartCoroutine(createUniverse);
    }

    IEnumerator CreateTimer(float delay)
    {
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

    IEnumerator CreateUniverse(float delay, UniverseObject universeObject, bool first = true)
    {
        if (first)
            yield return new WaitForSeconds(Random.Range(0, delay));
        else
            yield return new WaitForSeconds(delay);

        universeObject.SetRandomImage();

        StartCoroutine(CreateUniverse(delay, universeObject, false));
    }

    IEnumerator CreateCloud(float delay, int index = 0, bool isFirst = true)
    {
        if (isFirst)
            yield return null;
        else
            yield return new WaitForSeconds(delay);

        if (!cloudImages[index].gameObject.activeInHierarchy)
        {
            cloudImages[index].sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            cloudImages[index].gameObject.SetActive(true);
            UniverseUnit unit = GetUniverseUnit();
            if (unit.Equals(UniverseUnit.Sky))
                cloudImages[index].size = new Vector2(Random.Range(5f, 10f), Random.Range(3.5f, 6f));
            else if(unit.Equals(UniverseUnit.Atmosphere))
                cloudImages[index].size = new Vector2(Random.Range(10f, 20f), Random.Range(8f, 25f));
            cloudImages[index].transform.position = GetRandomCloudPosition(cloudImages[index].size, isFirst);
            cloudImages[index].sortingOrder = Random.Range(2, 5);
            cloudImages[index].color = new Vector4(1, 1, 1, Random.Range(0.55f, 0.8f));
            index = (index + 1) % cloudImages.Length;
        }

        if (universeUnit.Equals(UniverseUnit.Sky) || universeUnit.Equals(UniverseUnit.Atmosphere))
            StartCoroutine(CreateCloud(delay, index, false));
    }

    private Vector3 GetRandomCloudPosition(Vector2 size, bool isFirst)
    {
        Vector3 cameraRightDown = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 0f, 0f));
        Vector3 cameraRightUp = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        if (isFirst)
        {
            Vector3 onCamera = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.75f, 0f));
            return new Vector3(onCamera.x + Random.Range(0, cameraRightDown.x), Random.Range(onCamera.y, onCamera.y * 2f), 0);
        }

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