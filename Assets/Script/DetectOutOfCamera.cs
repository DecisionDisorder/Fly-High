using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 영역 밖으로 나갔는지 탐지해주는 클래스
/// </summary>
public class DetectOutOfCamera : MonoBehaviour
{
    /// <summary>
    /// 대상 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 스크립트가 활성화 되었을 때 카메라 설정 후 탐지 시작
    /// </summary>
    private void OnEnable()
    {
        mainCamera = Camera.main;
        StartCoroutine(CDetectOutOfCamera());
    }

    /// <summary>
    /// 카메라 범위 탐지 코루틴
    /// </summary>
    IEnumerator CDetectOutOfCamera()
    {
        // 0.2초 마다 반복하여 검사
        yield return new WaitForSeconds(0.2f);

        // 현재 스크립트가 심어진 오브젝트의 위치를 카메라 상의 좌표로 변환하여 특정 범위를 벗어났는지 계산하여 본 오브젝트 비활성화
        Vector3 positonOnCamera = mainCamera.WorldToScreenPoint(transform.position);
        if (positonOnCamera.x < -100 || positonOnCamera.y < -50)
        {
            gameObject.SetActive(false);
        }

        // 본 오브젝트가 활성화 되어있는 동안 반복 검사
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CDetectOutOfCamera());
        }
    }
}
