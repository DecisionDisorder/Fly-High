using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� ���� ������ �������� Ž�����ִ� Ŭ����
/// </summary>
public class DetectOutOfCamera : MonoBehaviour
{
    /// <summary>
    /// ��� ���� ī�޶�
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// ��ũ��Ʈ�� Ȱ��ȭ �Ǿ��� �� ī�޶� ���� �� Ž�� ����
    /// </summary>
    private void OnEnable()
    {
        mainCamera = Camera.main;
        StartCoroutine(CDetectOutOfCamera());
    }

    /// <summary>
    /// ī�޶� ���� Ž�� �ڷ�ƾ
    /// </summary>
    IEnumerator CDetectOutOfCamera()
    {
        // 0.2�� ���� �ݺ��Ͽ� �˻�
        yield return new WaitForSeconds(0.2f);

        // ���� ��ũ��Ʈ�� �ɾ��� ������Ʈ�� ��ġ�� ī�޶� ���� ��ǥ�� ��ȯ�Ͽ� Ư�� ������ ������� ����Ͽ� �� ������Ʈ ��Ȱ��ȭ
        Vector3 positonOnCamera = mainCamera.WorldToScreenPoint(transform.position);
        if (positonOnCamera.x < -100 || positonOnCamera.y < -50)
        {
            gameObject.SetActive(false);
        }

        // �� ������Ʈ�� Ȱ��ȭ �Ǿ��ִ� ���� �ݺ� �˻�
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CDetectOutOfCamera());
        }
    }
}
