using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ���� Ŭ����
/// </summary>
public class BackgroundControl : MonoBehaviour
{
    /// <summary>
    /// �����ư��� ǥ�õ� ��� ������Ʈ �迭
    /// </summary>
    public GameObject[] backgrounds = new GameObject[2];
    /// <summary>
    /// �����ư��� ǥ�õ� �� ������Ʈ �迭
    /// </summary>
    public GameObject[] grounds = new GameObject[2];
    /// <summary>
    /// ���� �� ������Ʈ
    /// </summary>
    public GameObject bottomBlock;
    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// ����� �÷��̾� ������ ����;� �ϴ��� ����
    /// </summary>
    public static bool isBackgroundFollow { private set; get; }

    /// <summary>
    /// ��� �� (��ǥ�� ����)
    /// </summary>
    private float distance = 80;
    
    /// <summary>
    /// ���� ������ �������� �ִ� ��� �ε��� ��
    /// </summary>
    private int onGround = 0;

    /// <summary>
    /// ��� �� ���� �ʱ� ��ǥ
    /// </summary>
    private Vector3[] resetPos;

    /// <summary>
    /// �ϴ� ��� ����
    /// </summary>
    public Color skyColor;
    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public Color atmosphereColor;
    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public Color universeColor;

    /// <summary>
    /// �׶��̼� ��� ������ ���� �ؽ�ó �迭
    /// </summary>
    private Texture2D[] backgroundTextures = new Texture2D[2];
    /// <summary>
    /// �ϴ� ��� ��������Ʈ ������
    /// </summary>
    public SpriteRenderer[] skyRenderers;
    /// <summary>
    /// ���� ��� ��������Ʈ ������
    /// </summary>
    public SpriteRenderer[] universeRenderers;

    private void Awake()
    {
        InitializeBackground();
        isBackgroundFollow = true;
    }

    private void Start()
    {
        resetPos = new Vector3[backgrounds.Length + grounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            resetPos[i] = backgrounds[i].transform.position;
            resetPos[i + backgrounds.Length] = grounds[i].transform.position;
        }

        StartCoroutine(BackgroundFollow());
    }

    /// <summary>
    /// ��� ���� �ʱ�ȭ �۾�
    /// </summary>
    private void InitializeBackground()
    {
        // �׶��̼��� �ʿ��� ��� �ؽ�ó �ʱ�ȭ �� �ϴ�-���� / ����-���� ��� �׶��̼� ���� ���
        for(int i = 0; i < 2; i++)
        {
            backgroundTextures[i] = new Texture2D(1, 2);
            backgroundTextures[i].wrapMode = TextureWrapMode.Clamp;
            backgroundTextures[i].filterMode = FilterMode.Bilinear;
        }
        for (int j = 0; j < skyRenderers.Length; j++)
            SetColor(skyColor, atmosphereColor, skyRenderers[j], 0);
        for (int j = 0; j < universeRenderers.Length; j++)
            SetColor(atmosphereColor, universeColor, universeRenderers[j], 1);
    }

    /// <summary>
    /// color1�� color2�� �׶��̼� ������ ����Ͽ� ��� renderer�� ������ �׶��̼� ��������Ʈ �̹��� ����
    /// </summary>
    /// <param name="color1">�Ʒ��� ����</param>
    /// <param name="color2">���� ����</param>
    /// <param name="renderer">��� renderer</param>
    /// <param name="i">��� �ؽ�ó �ε���</param>
    private void SetColor(Color color1, Color color2, SpriteRenderer renderer, int i)
    {
        backgroundTextures[i].SetPixels(new Color[] { color1, color2 });
        backgroundTextures[i].Apply();
        Rect rect = new Rect(0, 0, backgroundTextures[i].width, backgroundTextures[i].height);
        renderer.sprite = Sprite.Create(backgroundTextures[i], rect, new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// ���� ȯ�濡���� �پ��� ��� ���� ����
    /// </summary>
    /// <param name="color">������ ����</param>
    /// <param name="delayTime">���� ������ �ð�(��)</param>
    public void ChangeBackgroundColor(Color color, float delayTime)
    {
        StartCoroutine(CChangeBackgroundColor(color, delayTime));
    }

    /// <summary>
    /// ī�޶��� ��� ������ ������ ��� �������� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="color">������ ����</param>
    /// <param name="delayTime">���� ������ �ð�(��)</param>
    /// <param name="passedTime">���� �ð�(��)</param>
    /// <returns></returns>
    IEnumerator CChangeBackgroundColor(Color color, float delayTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        // ���� �ð� ����Ͽ� ���� �ð� ��� ������ �ð� ���� ���
        passedTime += Time.deltaTime;
        float delta = passedTime / delayTime;

        // ���� ���󿡼� ����� �������� ��� ������ŭ ����
        mainCamera.backgroundColor += delta * (color - mainCamera.backgroundColor);

        if(passedTime < delayTime)
            StartCoroutine(CChangeBackgroundColor(color, delayTime, passedTime));
    }

    /// <summary>
    /// �÷��̾� ������ ��ġ�� ���� ����� �̵���Ű�� �ڷ�ƾ
    /// </summary>
    IEnumerator BackgroundFollow()
    {
        yield return new WaitForSeconds(0.1f);

        // ������ x ��ǥ�� �������� ���
        float rocketX = RocketSet.instance.rocket.transform.position.x;

        // ������ ���� ������ ȭ�� �� �������� ��
        if (RocketSet.instance.rocketMain_rigid.velocity.x >= 0)
        {
            // ������ x��ǥ ���� ���� ����� x��ǥ ���� ���������� �������� �� (ī�޶� ������� �ʰ� �ִ�) ���� ����� ���������� ��ġ
            if (backgrounds[onGround].transform.position.x < rocketX)
            {
                backgrounds[SwitchGroundNum(onGround - 1)].transform.position = backgrounds[onGround].transform.position + new Vector3(distance, 0, 0);
                grounds[SwitchGroundNum(onGround - 1)].transform.position = grounds[onGround].transform.position + new Vector3(distance, 0, 0);
                onGround = SwitchGroundNum(onGround + 1);
            }
        }
        // ������ ���� ������ ȭ�� �� ������ ��
        else
        {
            // ������ x��ǥ ���� ���� ����� x��ǥ ���� �������� �������� �� (ī�޶� ������� �ʰ� �ִ�) ������ ����� �������� ��ġ
            if (backgrounds[onGround].transform.position.x > rocketX)
            {
                backgrounds[SwitchGroundNum(onGround - 1)].transform.position = backgrounds[onGround].transform.position - new Vector3(distance, 0, 0);
                grounds[SwitchGroundNum(onGround - 1)].transform.position = grounds[onGround].transform.position - new Vector3(distance, 0, 0);
                onGround = SwitchGroundNum(onGround + 1);
            }
        }

        // ������ y��ǥ ���� ���� ���� ������ �� ����� ���󰣴�.
        if (RocketSet.instance.rocket.transform.position.y < distance * 4)
            StartCoroutine(BackgroundFollow());
        // ������ ���� ���� �̻����� �ö󰡸� ����� ������ ������ �ʰ� ��Ȱ��ȭ �ȴ�.
        else
        {
            isBackgroundFollow = false;
            for(int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].SetActive(false);
                grounds[i].SetActive(false);
            }
            bottomBlock.SetActive(false);
        }
    }

    /// <summary>
    /// ���� ����� ��� �ε����� ��ü�Ѵ�.
    /// </summary>
    /// <param name="n">����� ��(+/- ����� ����� ��)</param>
    /// <returns>Clamp�� 0 or 1</returns>
    private int SwitchGroundNum(int n)
    {
        if (n < 0)
            n = 1;
        if (n > 1)
            n = 0;
        return n;
    }

    /// <summary>
    /// ��� �ʱ�ȭ
    /// </summary>
    public void ResetBackground()
    {
        for(int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.position = resetPos[i];
            grounds[i].transform.position = resetPos[backgrounds.Length + i];
        }
        onGround = 0;
    }
}
