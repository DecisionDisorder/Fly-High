using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드 소환 오브젝트의 종류
/// </summary>
public enum PropType { Obstacle, Coin, Fuel, Star }

/// <summary>
/// 필드 소환 오브젝트
/// </summary>
public class FlyingObject : MonoBehaviour
{
    /// <summary>
    /// 스폰된 FlyingObject 수량 (4가지 오브젝트에 대한 배열)
    /// </summary>
    public static int[] spawnedCount = new int[4];
    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera mainCamera;
    /// <summary>
    /// 오브젝트 이동 코루틴
    /// </summary>
    private IEnumerator moveObject;
    /// <summary>
    /// 오브젝트 이동 속도
    /// </summary>
    public float speed;

    /// <summary>
    /// 오브젝트 이동 활성화 작업
    /// </summary>
    protected void Enable()
    {
        moveObject = MoveObject();
        StartCoroutine(moveObject);
    }

    /// <summary>
    /// 오브젝트 이동 취소 작업
    /// </summary>
    protected void Disable()
    {
        if (moveObject != null)
            StopCoroutine(moveObject);
    }

    /// <summary>
    /// 정해진 속도로 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveObject()
    {
        yield return new WaitForEndOfFrame();

        // 게임 일시정지를 제외하고 매 프레임 마다 로켓의 속도에 정해진 속도를 더하여 이동
        if (!TabletManager.isPause)
        {
            Vector2 rocketV = RocketSet.instance.rocketMain_rigid.velocity.normalized;
            transform.localPosition += new Vector3(-rocketV.x, -rocketV.y) * speed * Time.deltaTime;
        }
        moveObject = MoveObject();
        StartCoroutine(moveObject);
    }

    /// <summary>
    /// 카메라 밖으로 벗어나는지 탐지하는 코루틴
    /// </summary>
    protected IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

        // 로켓의 이동 방향에 따라서 특정 영역 밖으로 나가면 회수 처리
        Vector3 positonOnCamera = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        if(RocketSet.instance.IsRocketRight())
        {
            if(positonOnCamera.x < -Screen.width || positonOnCamera.y < -Screen.height * 0.05f)
                SpawnManager.instance.ReturnObstacle(this);
        }
        else
        {
            if(positonOnCamera.x > Screen.width || positonOnCamera.y < -Screen.height * 0.05f)
                SpawnManager.instance.ReturnObstacle(this);
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DetectOutOfCamera());
        }
    }
}
