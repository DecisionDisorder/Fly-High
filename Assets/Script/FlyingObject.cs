using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʵ� ��ȯ ������Ʈ�� ����
/// </summary>
public enum PropType { Obstacle, Coin, Fuel, Star }

/// <summary>
/// �ʵ� ��ȯ ������Ʈ
/// </summary>
public class FlyingObject : MonoBehaviour
{
    /// <summary>
    /// ������ FlyingObject ���� (4���� ������Ʈ�� ���� �迭)
    /// </summary>
    public static int[] spawnedCount = new int[4];
    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera mainCamera;
    /// <summary>
    /// ������Ʈ �̵� �ڷ�ƾ
    /// </summary>
    private IEnumerator moveObject;
    /// <summary>
    /// ������Ʈ �̵� �ӵ�
    /// </summary>
    public float speed;

    /// <summary>
    /// ������Ʈ �̵� Ȱ��ȭ �۾�
    /// </summary>
    protected void Enable()
    {
        moveObject = MoveObject();
        StartCoroutine(moveObject);
    }

    /// <summary>
    /// ������Ʈ �̵� ��� �۾�
    /// </summary>
    protected void Disable()
    {
        if (moveObject != null)
            StopCoroutine(moveObject);
    }

    /// <summary>
    /// ������ �ӵ��� �̵�
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveObject()
    {
        yield return new WaitForEndOfFrame();

        // ���� �Ͻ������� �����ϰ� �� ������ ���� ������ �ӵ��� ������ �ӵ��� ���Ͽ� �̵�
        if (!TabletManager.isPause)
        {
            Vector2 rocketV = RocketSet.instance.rocketMain_rigid.velocity.normalized;
            transform.localPosition += new Vector3(-rocketV.x, -rocketV.y) * speed * Time.deltaTime;
        }
        moveObject = MoveObject();
        StartCoroutine(moveObject);
    }

    /// <summary>
    /// ī�޶� ������ ������� Ž���ϴ� �ڷ�ƾ
    /// </summary>
    protected IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

        // ������ �̵� ���⿡ ���� Ư�� ���� ������ ������ ȸ�� ó��
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
