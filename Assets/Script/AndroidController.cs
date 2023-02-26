using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ڷΰ���, �޽��� ���� �ȵ���̵� �Ϻ� ����� �����ϴ� ��Ʈ�ѷ� Ŭ����
/// </summary>
public class AndroidController : MonoBehaviour
{
    /// <summary>
    /// �޽��� ��� ������Ʈ
    /// </summary>
    public GameObject message_obj;
    /// <summary>
    /// �޽��� ���� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation messageEffect_ani;
    /// <summary>
    /// �޽��� �ؽ�Ʈ
    /// </summary>
    public Text message_text;
    /// <summary>
    /// �ڷΰ��� ��ư Ŭ�� Ƚ��
    /// </summary>
    private int count = 0;
    /// <summary>
    /// �ڷΰ��� ��ư Ŭ�� ������
    /// </summary>
    private bool backKeyDown = false;
    /// <summary>
    /// �ִϸ��̼� ȿ���� ������ ���� ����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator waitForEndOfAnimation;
    /// <summary>
    /// �޽��� ��Ȱ��ȭ ȿ�� ��� �ڷ�ƾ
    /// </summary>
    private IEnumerator messageFloating;

    private void Start()
    {
        StartCoroutine(Backkey(0));
    }

    /// <summary>
    /// �ڷΰ��� Ű ���� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">Backkey �ν� ������</param>
    IEnumerator Backkey(float delay)
    {
        // �����̰� ������ ������ �������� ���
        if (delay.Equals(0))
            yield return new WaitForEndOfFrame();
        // �����̰� ������ ���� �ð���ŭ(�����ϸ��� �ð� ������) ������
        else
            yield return new WaitForSecondsRealtime(delay);

        // �ڷΰ��� Ű�� �νĵǸ�
        if (Input.GetKey(KeyCode.Escape))
        {
            // backkey�� ���ȴٴ� ���� ǥ���ϰ�
            backKeyDown = true;
            // ������ ���� ��쿡 �� ����
            if (count > 0)
                Application.Quit();
            // �ƴϸ� �ڷΰ��� Ƚ���� �ø���, ī��Ʈ �ʱ�ȭ ��� �ڷ�ƾ ����
            else
            {
                count++;
                ExitMessage();
                StartCoroutine(InitializeDoubleBack());
            }
            // �ڷΰ��Ⱑ �ٷ� �ٽ� �νĵ��� �ʵ��� 0.1�� ������
            StartCoroutine(Backkey(0.1f));
        }

        // �Ϲ����� ���¿��� ������ ���� �ڷ�ƾ �ݺ�
        if (!backKeyDown)
            StartCoroutine(Backkey(0));
        else
            backKeyDown = false;
    }
    /// <summary>
    /// ���� ���� �ȳ� �޽����� ����ȭ(��/��)�� ���߾� �ҷ��ͼ� �����Ѵ�.
    /// </summary>
    private void ExitMessage()
    {
        ShowMessage(LocalizationManager.instance.GetLocalizedValue("exit_message"));
    }

    /// <summary>
    /// �佺Ʈ �޽���ó�� �޽����� �����Ѵ�.
    /// </summary>
    /// <param name="message">�޽��� ����</param>
    /// <param name="time">ǥ�� �ð�</param>
    public void ShowMessage(string message, float time = 1f)
    {
        // ���� �޽��� ��Ȱ��ȭ ��� �ڷ�ƾ ����
        if (waitForEndOfAnimation != null)
            StopCoroutine(waitForEndOfAnimation);
        if (messageFloating != null)
            StopCoroutine(messageFloating);

        // �޽��� ������Ʈ Ȱ��ȭ �� �ִϸ��̼� ȿ�� ���
        message_obj.SetActive(true);
        message_text.text = message;
        messageEffect_ani["MessageEffect"].time = 0;
        messageEffect_ani["MessageEffect"].speed = 1f;
        messageEffect_ani.Play();

        // �޽��� ��Ȱ��ȭ ��� �ڷ�ƾ ����
        messageFloating = MessageFloating(time);
        StartCoroutine(messageFloating);
    }

    /// <summary>
    /// �ڷΰ��� ī��Ʈ Ƚ�� �ʱ�ȭ ��� �ڷ�ƾ
    /// </summary>
    IEnumerator InitializeDoubleBack()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        count = 0;
    }

    /// <summary>
    /// �޽��� ��Ȱ��ȭ ��� �ڷ�ƾ
    /// </summary>
    /// <param name="time">�޽��� ��� �ð�</param>
    IEnumerator MessageFloating(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        // �޽��� �ִϸ��̼� ȿ�� ����� ���� �� ���
        messageEffect_ani["MessageEffect"].speed = -1;
        messageEffect_ani["MessageEffect"].time = messageEffect_ani["MessageEffect"].length;
        messageEffect_ani.Play();
        // �޽��� �ִϸ��̼� ���� �� �޽��� ������Ʈ ��Ȱ��ȭ ��� �ڷ�ƾ ���
        waitForEndOfAnimation = PlayManager.instance.RWaitAnimation(messageEffect_ani, CloseWindow, message_obj);
        StartCoroutine(waitForEndOfAnimation);
    }

    /// <summary>
    /// �޴�â ��Ȱ��ȭ(delegate ��Ͽ�)
    /// </summary>
    /// <param name="obj">��Ȱ��ȭ�� ���� ������Ʈ</param>
    private void CloseWindow(GameObject obj)
    {
        obj.SetActive(false);
    }
}
