using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��� ������Ʈ�� �̵��ϴ� ���� �����ϴ� Ŭ����
/// </summary>
public class UniverseMove : MonoBehaviour
{
    /// <summary>
    /// ���� ��� ���� ������Ʈ
    /// </summary>
    public Universe universe;
    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera mainCamera;
    /// <summary>
    /// ���� ��� ���� ������
    /// </summary>
    public enum UniverseType { Far, Random, Planet, Cloud }
    /// <summary>
    /// ���� ��� ������Ʈ
    /// </summary>
    private UniverseObject universeObject;
    /// <summary>
    /// �ش� ������Ʈ�� ���� ��� ����
    /// </summary>
    public UniverseType type;
    /// <summary>
    /// �̵� �ӵ�
    /// </summary>
    public float speed;
    /// <summary>
    /// �̵� �ӵ��� �������� ����
    /// </summary>
    public bool isRandomSpeed;
    /// <summary>
    /// ���� �ӵ�
    /// </summary>
    private float constSpeed;

    private void Start()
    {
        switch (type)
        {
            case UniverseType.Far:
                universeObject = universe.farUniverse;
                break;
            case UniverseType.Random:
                universeObject = universe.randomUniverse;
                break;
            case UniverseType.Planet:
                universeObject = universe.planet;
                break;
        }
    }

    /// <summary>
    /// ��ȯ�� �Ǿ��� �� ���� �ӵ� ����
    /// </summary>
    private void OnEnable()
    {
        if (isRandomSpeed)
        {
            constSpeed = speed;
            speed = Random.Range(0.5f, 1.5f) * constSpeed;
        }
        StartMove();
    }

    /// <summary>
    /// �����̱� �����Ͽ� ī�޶� ���� Ž���� �Բ� ����
    /// </summary>
    public void StartMove()
    {
        StartCoroutine(Move());
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// ������ �������� �̵��ϴ� �ڷ�ƾ
    /// </summary>
    IEnumerator Move()
    {
        yield return new WaitForEndOfFrame();

        // ������ �� ���� �ӵ��� �̵��Ѵ�.
        if(type.Equals(UniverseType.Cloud))
        {
            transform.localPosition -= Vector3.right * speed * Time.deltaTime;
        }
        // ���� ����� ���, ���� ��� ��� �ӵ��� �̵��Ѵ�.
        else
        {
            Vector2 rocketV = RocketSet.instance.rocketMain_rigid.velocity;
            transform.localPosition += new Vector3(-rocketV.x, -rocketV.y) * speed * Time.deltaTime;
        }

        StartCoroutine(Move());
    }

    /// <summary>
    /// ī�޶� ��Ż ���� Ž��
    /// </summary>
    IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

        // ���� ���� ���⿡ ���� ������ �ٸ��� �ΰ� ��Ż ���� Ž��
        Vector3 positonOnCamera = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        if (RocketSet.instance.IsRocketRight())
        {
            if (positonOnCamera.x < -Screen.width || positonOnCamera.y < -Screen.height * 1.5f)
            {
                if (type.Equals(UniverseType.Cloud))
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    universeObject.ReturnImage();
                }
            }
        }
        else
        {
            if (positonOnCamera.x > Screen.width * 2 || positonOnCamera.y < -Screen.height * 1.5f)
            {
                if (type.Equals(UniverseType.Cloud))
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    universeObject.ReturnImage();
                }
            }
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DetectOutOfCamera());
        }
    }
}