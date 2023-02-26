using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �߾� TV�� �� ������ ���� Ŭ����
/// </summary>
public class MainStarMove : MonoBehaviour
{
    /// <summary>
    /// �� ������ �ӵ�
    /// </summary>
    public float speed;
    /// <summary>
    /// �� ������ �ڷ�ƾ
    /// </summary>
    private IEnumerator starMove;

    private void Start()
    {
        StartMove();
    }

    /// <summary>
    /// �� ������ ȿ�� ����
    /// </summary>
    public void StartMove()
    {
        starMove = StarMove();
        StartCoroutine(starMove);
    }

    /// <summary>
    /// �� ������ �ߴ�
    /// </summary>
    public void StopMove()
    {
        StopCoroutine(starMove);
    }

    /// <summary>
    /// �� ������ �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator StarMove()
    {
        yield return new WaitForEndOfFrame();

        // �������� ����ġ��ŭ �̵� �� �ٽ� �������� �̵��ϴ� ���� �ݺ�
        if (transform.localPosition.x > -2460f)
        {
            transform.localPosition += Vector3.left * speed * Time.deltaTime;
        }
        else
            transform.localPosition = Vector3.zero;

        starMove = StarMove();
        StartCoroutine(starMove);
    }
}
