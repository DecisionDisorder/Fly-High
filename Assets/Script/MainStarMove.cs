using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 중앙 TV의 별 움직임 관리 클래스
/// </summary>
public class MainStarMove : MonoBehaviour
{
    /// <summary>
    /// 별 움직임 속도
    /// </summary>
    public float speed;
    /// <summary>
    /// 별 움직임 코루틴
    /// </summary>
    private IEnumerator starMove;

    private void Start()
    {
        StartMove();
    }

    /// <summary>
    /// 별 움직임 효과 시작
    /// </summary>
    public void StartMove()
    {
        starMove = StarMove();
        StartCoroutine(starMove);
    }

    /// <summary>
    /// 별 움직임 중단
    /// </summary>
    public void StopMove()
    {
        StopCoroutine(starMove);
    }

    /// <summary>
    /// 별 움직임 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator StarMove()
    {
        yield return new WaitForEndOfFrame();

        // 좌측으로 기준치만큼 이동 후 다시 원점으로 이동하는 것을 반복
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
