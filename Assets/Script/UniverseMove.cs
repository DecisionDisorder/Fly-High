using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 우주 배경 오브젝트가 이동하는 것을 관리하는 클래스
/// </summary>
public class UniverseMove : MonoBehaviour
{
    /// <summary>
    /// 우주 배경 관리 오브젝트
    /// </summary>
    public Universe universe;
    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera mainCamera;
    /// <summary>
    /// 우주 배경 종류 열거형
    /// </summary>
    public enum UniverseType { Far, Random, Planet, Cloud }
    /// <summary>
    /// 우주 배경 오브젝트
    /// </summary>
    private UniverseObject universeObject;
    /// <summary>
    /// 해당 오브젝트의 우주 배경 종류
    /// </summary>
    public UniverseType type;
    /// <summary>
    /// 이동 속도
    /// </summary>
    public float speed;
    /// <summary>
    /// 이동 속도가 랜덤인지 여부
    /// </summary>
    public bool isRandomSpeed;
    /// <summary>
    /// 고정 속도
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
    /// 소환이 되었을 때 랜덤 속도 지정
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
    /// 움직이기 시작하여 카메라 범위 탐지도 함께 시작
    /// </summary>
    public void StartMove()
    {
        StartCoroutine(Move());
        StartCoroutine(DetectOutOfCamera());
    }

    /// <summary>
    /// 정해진 방향으로 이동하는 코루틴
    /// </summary>
    IEnumerator Move()
    {
        yield return new WaitForEndOfFrame();

        // 구름일 때 절대 속도로 이동한다.
        if(type.Equals(UniverseType.Cloud))
        {
            transform.localPosition -= Vector3.right * speed * Time.deltaTime;
        }
        // 우주 배경일 경우, 로켓 대비 상대 속도로 이동한다.
        else
        {
            Vector2 rocketV = RocketSet.instance.rocketMain_rigid.velocity;
            transform.localPosition += new Vector3(-rocketV.x, -rocketV.y) * speed * Time.deltaTime;
        }

        StartCoroutine(Move());
    }

    /// <summary>
    /// 카메라 이탈 범위 탐지
    /// </summary>
    IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

        // 로켓 진행 방향에 따라 마진을 다르게 두고 이탈 범위 탐지
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