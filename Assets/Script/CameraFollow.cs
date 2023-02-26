using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라가 플레이어 로켓을 따라오도록 하는 클래스
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// 시작지점에서 가장 왼쪽 끝의 좌표
    /// </summary>
    public float leftSideX = -10f;
    /// <summary>
    /// 로켓-카메라 간의 오프셋
    /// </summary>
    public Vector3 offset;
    /// <summary>
    /// 카메라가 로켓을 따라가는 속도
    /// </summary>
    public float followSpeed = 0.15f;
    /// <summary>
    /// 위치 보정 값
    /// </summary>
    public Vector2 gap;
    /// <summary>
    /// 카메라 화면의 좌표계 상의 크기
    /// </summary>
    private Vector2 cameraSize = new Vector2(17.8f, 10f);

    /// <summary>
    /// 로켓이 추락할 때 따라가는 최저 좌표 기준점
    /// </summary>
    public float LowestY;
    /// <summary>
    /// 카메라가 떨어지는 로켓을 따라가는 한계 거리
    /// </summary>
    public float allowDrop;

    /// <summary>
    /// 카메라가 흔들리고 있는지 여부
    /// </summary>
    private bool isShaking = false;
    /// <summary>
    /// 카메라가 흔들리는 강도
    /// </summary>
    public float shakeAmount;
    /// <summary>
    /// 카메라가 흔들리는 시간
    /// </summary>
    private float shakeTime;
    /// <summary>
    /// 특정 상황에서의 기준점 위치(카메라 흔들림, 로켓 방향 세팅 등)
    /// </summary>
    private Vector3 initialPosition;

    private void Start()
    {
        InitializePosition();
    }

    /// <summary>
    /// 위치 정보 초기화
    /// </summary>
    private void InitializePosition()
    {
        initialPosition = Follow();
        transform.position = initialPosition;
    }

    /// <summary>
    /// 카메라의 로켓 추적 시작
    /// </summary>
    public void StartFollow()
    {
        StartCoroutine(FollowCamera());
    }

    /// <summary>
    /// 카메라가 로켓의 위치를 따라가도록 하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowCamera()
    {
        yield return new WaitForFixedUpdate();

        // 화면 흔들림 여부에 따라서 카메라에 바로 적용하거나 기준 포인트에 설정하거나
        if (!isShaking)
        {
            transform.position = Follow();
        }
        else
        {
            initialPosition = Follow();
        }

        if(PlayManager.isSurvive)
            StartCoroutine(FollowCamera());
    }

    /// <summary>
    /// 로켓의 위치를 부드럽게 따라가도록 위치를 계산해주는 함수
    /// </summary>
    /// <returns>완충 계산이 된 좌표</returns>
    private Vector3 Follow()
    {
        // 현재 위치에서 이동해야할 위치를 Lerp 함수를 통해 선형 보간을 하여 따라간다.
        Vector3 camera_pos = RocketSet.instance.rocket.transform.position + offset + new Vector3(gap.x * cameraSize.x, gap.y * cameraSize.y);
        Vector3 lerp_pos = Vector3.Lerp(transform.position, camera_pos, followSpeed);
        Vector3 resultPos = lerp_pos;

        // 로켓 추락 때의 하방 한계점 계산
        float y = resultPos.y;
        if (BackgroundControl.isBackgroundFollow)
            y = LowestLimit(y, LowestY);
        resultPos = new Vector3(resultPos.x, y, -10);

        // 로켓의 연료가 소진되었을 때의 하방 한계점 계산
        if (RocketSet.instance.FuelRemain <= 0)
        {
            LowestY = (resultPos.y - allowDrop) < LowestY ? LowestY : (resultPos.y - allowDrop);
            y = LowestLimit(y, LowestY);
            resultPos = new Vector3(resultPos.x, y, -10);
        }
        return resultPos;
    }

    /// <summary>
    /// 카메라 충격 이펙트 시작
    /// </summary>
    /// <param name="time">충격 지속 시간</param>
    /// <param name="shakeAmount">강도</param>
    public void ShakeCamera(float time, float shakeAmount = 0)
    {
        isShaking = true;
        initialPosition = transform.position;
        shakeTime = time;
        StartCoroutine(Shake(shakeAmount));
    }

    /// <summary>
    /// 카메라를 지속시간 동안 흔드는 효과 처리 코루틴
    /// </summary>
    /// <param name="_shakeAmount">카메라 충격 강도</param>
    /// <returns></returns>
    IEnumerator Shake(float _shakeAmount)
    {
        yield return new WaitForEndOfFrame();

        // 지속시간 동안
        if(shakeTime > 0)
        {
            // 기준 점의 원형 범위 내에서 강도만큼 랜덤으로 포지션 변경
            if(_shakeAmount.Equals(0))
                transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            else
                transform.position = Random.insideUnitSphere * _shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        // 지속시간 종료시 상태 초기화
        else
        {
            shakeTime = 0f;
            isShaking = false;
        }

        if(isShaking)
            StartCoroutine(Shake(_shakeAmount));
    }

    /// <summary>
    /// 기본적으로 카메라가 따라가는 최저 좌표 기준점
    /// </summary>
    private float LowestLimit(float target, float min)
    {
        if (target < min)
            return min;
        else
            return target;
    }
}
