using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� �÷��̾� ������ ��������� �ϴ� Ŭ����
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// ������������ ���� ���� ���� ��ǥ
    /// </summary>
    public float leftSideX = -10f;
    /// <summary>
    /// ����-ī�޶� ���� ������
    /// </summary>
    public Vector3 offset;
    /// <summary>
    /// ī�޶� ������ ���󰡴� �ӵ�
    /// </summary>
    public float followSpeed = 0.15f;
    /// <summary>
    /// ��ġ ���� ��
    /// </summary>
    public Vector2 gap;
    /// <summary>
    /// ī�޶� ȭ���� ��ǥ�� ���� ũ��
    /// </summary>
    private Vector2 cameraSize = new Vector2(17.8f, 10f);

    /// <summary>
    /// ������ �߶��� �� ���󰡴� ���� ��ǥ ������
    /// </summary>
    public float LowestY;
    /// <summary>
    /// ī�޶� �������� ������ ���󰡴� �Ѱ� �Ÿ�
    /// </summary>
    public float allowDrop;

    /// <summary>
    /// ī�޶� ��鸮�� �ִ��� ����
    /// </summary>
    private bool isShaking = false;
    /// <summary>
    /// ī�޶� ��鸮�� ����
    /// </summary>
    public float shakeAmount;
    /// <summary>
    /// ī�޶� ��鸮�� �ð�
    /// </summary>
    private float shakeTime;
    /// <summary>
    /// Ư�� ��Ȳ������ ������ ��ġ(ī�޶� ��鸲, ���� ���� ���� ��)
    /// </summary>
    private Vector3 initialPosition;

    private void Start()
    {
        InitializePosition();
    }

    /// <summary>
    /// ��ġ ���� �ʱ�ȭ
    /// </summary>
    private void InitializePosition()
    {
        initialPosition = Follow();
        transform.position = initialPosition;
    }

    /// <summary>
    /// ī�޶��� ���� ���� ����
    /// </summary>
    public void StartFollow()
    {
        StartCoroutine(FollowCamera());
    }

    /// <summary>
    /// ī�޶� ������ ��ġ�� ���󰡵��� �ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowCamera()
    {
        yield return new WaitForFixedUpdate();

        // ȭ�� ��鸲 ���ο� ���� ī�޶� �ٷ� �����ϰų� ���� ����Ʈ�� �����ϰų�
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
    /// ������ ��ġ�� �ε巴�� ���󰡵��� ��ġ�� ������ִ� �Լ�
    /// </summary>
    /// <returns>���� ����� �� ��ǥ</returns>
    private Vector3 Follow()
    {
        // ���� ��ġ���� �̵��ؾ��� ��ġ�� Lerp �Լ��� ���� ���� ������ �Ͽ� ���󰣴�.
        Vector3 camera_pos = RocketSet.instance.rocket.transform.position + offset + new Vector3(gap.x * cameraSize.x, gap.y * cameraSize.y);
        Vector3 lerp_pos = Vector3.Lerp(transform.position, camera_pos, followSpeed);
        Vector3 resultPos = lerp_pos;

        // ���� �߶� ���� �Ϲ� �Ѱ��� ���
        float y = resultPos.y;
        if (BackgroundControl.isBackgroundFollow)
            y = LowestLimit(y, LowestY);
        resultPos = new Vector3(resultPos.x, y, -10);

        // ������ ���ᰡ �����Ǿ��� ���� �Ϲ� �Ѱ��� ���
        if (RocketSet.instance.FuelRemain <= 0)
        {
            LowestY = (resultPos.y - allowDrop) < LowestY ? LowestY : (resultPos.y - allowDrop);
            y = LowestLimit(y, LowestY);
            resultPos = new Vector3(resultPos.x, y, -10);
        }
        return resultPos;
    }

    /// <summary>
    /// ī�޶� ��� ����Ʈ ����
    /// </summary>
    /// <param name="time">��� ���� �ð�</param>
    /// <param name="shakeAmount">����</param>
    public void ShakeCamera(float time, float shakeAmount = 0)
    {
        isShaking = true;
        initialPosition = transform.position;
        shakeTime = time;
        StartCoroutine(Shake(shakeAmount));
    }

    /// <summary>
    /// ī�޶� ���ӽð� ���� ���� ȿ�� ó�� �ڷ�ƾ
    /// </summary>
    /// <param name="_shakeAmount">ī�޶� ��� ����</param>
    /// <returns></returns>
    IEnumerator Shake(float _shakeAmount)
    {
        yield return new WaitForEndOfFrame();

        // ���ӽð� ����
        if(shakeTime > 0)
        {
            // ���� ���� ���� ���� ������ ������ŭ �������� ������ ����
            if(_shakeAmount.Equals(0))
                transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            else
                transform.position = Random.insideUnitSphere * _shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        // ���ӽð� ����� ���� �ʱ�ȭ
        else
        {
            shakeTime = 0f;
            isShaking = false;
        }

        if(isShaking)
            StartCoroutine(Shake(_shakeAmount));
    }

    /// <summary>
    /// �⺻������ ī�޶� ���󰡴� ���� ��ǥ ������
    /// </summary>
    private float LowestLimit(float target, float min)
    {
        if (target < min)
            return min;
        else
            return target;
    }
}
