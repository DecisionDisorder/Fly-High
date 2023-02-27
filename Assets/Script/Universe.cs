using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����/�ϴ� ��� ������Ʈ�� �Ѱ��ϴ� Ŭ����
/// </summary>
public class Universe : MonoBehaviour
{
    /// <summary>
    /// ���� ��� �Ѱ� Ŭ������ �̱��� �ν��Ͻ�
    /// </summary>
    public static Universe instance;
    /// <summary>
    /// ���� ��� �ܰ�
    /// </summary>
    public enum UniverseUnit { Sky, Atmosphere, NearTheEarth, SolarSystem, OutsideSolarSystem, IntermediateOfMilkyWay, OutskirtOfMilkyWay }
    /// <summary>
    /// ���� ���� ��� �ܰ� (�ϴ÷� ����)
    /// </summary>
    private UniverseUnit universeUnit = UniverseUnit.Sky;
    /// <summary>
    /// ��� ���� ���� ����(��)
    /// </summary>
    public int[] unitCriteria = new int[7];

    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// �� ���� ����� ���� ��� ������Ʈ
    /// </summary>
    public UniverseObject farUniverse;
    /// <summary>
    /// ������ ���� ����� ���� ��� ������Ʈ
    /// </summary>
    public UniverseObject randomUniverse;
    /// <summary>
    /// �༺ ����� ���� ��� ������Ʈ
    /// </summary>
    public UniverseObject planet;

    /// <summary>
    /// �� ���� ��� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator createFarUniverse;
    /// <summary>
    /// ���� ���� ��� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator createRandomUniverse;
    /// <summary>
    /// �༺ ��� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator createPlanet;
    /// <summary>
    /// ���� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator createCloud;

    /// <summary>
    /// ���� ��������Ʈ �̹���
    /// </summary>
    public Sprite[] cloudSprites;
    /// <summary>
    /// ���� �̹��� ������
    /// </summary>
    public SpriteRenderer[] cloudImages;

    /// <summary>
    /// ���ϼ� ��� ����
    /// </summary>
    public Color milkyWayBackgroundColor;
    /// <summary>
    /// ��� ���� ������Ʈ
    /// </summary>
    public BackgroundControl backgroundControl;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(CreateTimer(1f));
    }

    /// <summary>
    /// ������ ���� ���� ���� ���� ��� �ܰ踦 ���
    /// </summary>
    /// <returns>���� ���� ��� �ܰ�</returns>
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
    /// ���� ��� ���� ����
    /// </summary>
    /// <param name="createUniverse">��� ���� �ڷ�ƾ</param>
    /// <param name="delay">���� ����</param>
    /// <param name="universeObject">��� ���� ��� ������Ʈ</param>
    private void StartCreateUniverse(IEnumerator createUniverse, float delay, UniverseObject universeObject)
    {
        if (createUniverse != null)
            StopCoroutine(createUniverse);

        createUniverse = CreateUniverse(delay, universeObject);
        StartCoroutine(createUniverse);
    }

    /// <summary>
    /// ���� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">���� ����</param>
    IEnumerator CreateTimer(float delay)
    {
        // �� ���� ��� �ܰ� ���ǿ� ���� ���� �ֱ⸦ �����Ͽ� ������ �ڷ�ƾ ����
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
    /// ���� ��� ���� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">���� �ֱ�</param>
    /// <param name="universeObject">���� ��� ���� ��� ������Ʈ</param>
    /// <param name="first">ó�� �������� ����</param>
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
    /// ���� ���� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">���� �ֱ�</param>
    /// <param name="index">���� �ε���</param>
    /// <param name="isFirst">ó�� �������� ����</param>
    /// <returns></returns>
    IEnumerator CreateCloud(float delay, int index = 0, bool isFirst = true)
    {
        // ù �����̸� ��� ���� �ٷ� ����
        if (isFirst)
            yield return null;
        else
            yield return new WaitForSeconds(delay);

        // Ŭ���� �̹����� ��ȯ���� ���� �����̸� Ȱ��ȭ(��ȯ) ó��
        if (!cloudImages[index].gameObject.activeInHierarchy)
        {
            // ���� �̹��� ���� ����
            cloudImages[index].sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            // Ȱ��ȭ
            cloudImages[index].gameObject.SetActive(true);
            // ���� �ܰ� ��� �� ��濡 ���� ũ�� ����
            UniverseUnit unit = GetUniverseUnit();
            if (unit.Equals(UniverseUnit.Sky))
                cloudImages[index].size = new Vector2(Random.Range(5f, 10f), Random.Range(3.5f, 6f));
            else if(unit.Equals(UniverseUnit.Atmosphere))
                cloudImages[index].size = new Vector2(Random.Range(10f, 20f), Random.Range(8f, 25f));
            // ���� ��ġ ����
            cloudImages[index].transform.position = GetRandomCloudPosition(cloudImages[index].size, isFirst);
            cloudImages[index].sortingOrder = Random.Range(2, 5);
            cloudImages[index].color = new Vector4(1, 1, 1, Random.Range(0.55f, 0.8f));
            index = (index + 1) % cloudImages.Length;
        }

        // �ϴ�/���� ��濡���� �߰� ���� �ǵ��� �ݺ�
        if (universeUnit.Equals(UniverseUnit.Sky) || universeUnit.Equals(UniverseUnit.Atmosphere))
            StartCoroutine(CreateCloud(delay, index, false));
    }

    /// <summary>
    /// ������ ������ ��ġ�� ����ϴ� �Լ�
    /// </summary>
    /// <param name="size">������ ũ��</param>
    /// <param name="isFirst">ù �������� ����</param>
    /// <returns></returns>
    private Vector3 GetRandomCloudPosition(Vector2 size, bool isFirst)
    {
        // ī�޶� ���� ��ġ ���
        Vector3 cameraRightDown = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 0f, 0f));
        Vector3 cameraRightUp = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        // ó�� �����̸� Ư�� ��ġ�� ����
        if (isFirst)
        {
            Vector3 onCamera = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.75f, 0f));
            return new Vector3(onCamera.x + Random.Range(0, cameraRightDown.x), Random.Range(onCamera.y, onCamera.y * 2f), 0);
        }

        // ī�޶� ���� ������ Ȥ�� ���� ����
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