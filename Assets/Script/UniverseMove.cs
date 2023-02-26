using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniverseMove : MonoBehaviour
{
    public Universe universe;
    public Camera mainCamera;
    public enum UniverseType { Far, Random, Planet, Cloud }
    private UniverseObject universeObject;
    public UniverseType type;
    public float speed;
    public bool isRandomSpeed;
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

    private void OnEnable()
    {
        if (isRandomSpeed)
        {
            constSpeed = speed;
            speed = Random.Range(0.5f, 1.5f) * constSpeed;
        }
        StartMove();
    }

    public void StartMove()
    {
        StartCoroutine(Move());
        StartCoroutine(DetectOutOfCamera());
    }

    IEnumerator Move()
    {
        yield return new WaitForEndOfFrame();

        if(type.Equals(UniverseType.Cloud))
        {
            transform.localPosition -= Vector3.right * speed * Time.deltaTime;
        }
        else
        {
            Vector2 rocketV = RocketSet.instance.rocketMain_rigid.velocity;
            transform.localPosition += new Vector3(-rocketV.x, -rocketV.y) * speed * Time.deltaTime;
        }

        StartCoroutine(Move());
    }

    IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

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