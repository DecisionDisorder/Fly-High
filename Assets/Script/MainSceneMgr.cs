using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ��� ȭ�� ���� Ŭ����
/// </summary>
public class MainSceneMgr : MonoBehaviour
{
    /// <summary>
    /// �߾� TV ���� ������Ʈ
    /// </summary>
    public GameObject tv;
    /// <summary>
    /// �߾� TV�� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation tv_ani;
    /// <summary>
    /// �� ���� ���� ������Ʈ
    /// </summary>
    public GameObject appDrawer;
    /// <summary>
    /// �� ���� on/off �ִϸ��̼� ȿ��
    /// </summary>
    public Animation appDrawer_ani;
    /// <summary>
    /// �� on/off �ִϸ��̼� ȿ��
    /// </summary>
    public Animation[] apps_ani;
    /// <summary>
    /// �� �� ��Ʈ ���� ������Ʈ �迭
    /// </summary>
    public GameObject[] apps_obj;
    /// <summary>
    /// �� ���� ��ư ���� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation closeButtonAppear_ani;
    /// <summary>
    /// �߾� TV�� �� ������ ȿ�� ���� ������Ʈ
    /// </summary>
    public MainStarMove starMove;
    /// <summary>
    /// TV�� ���� �ִ� ��
    /// </summary>
    private TabletAppList openedApp;
    /// <summary>
    /// �ð� ���� �ؽ�Ʈ
    /// </summary>
    public Text clock_text;

    /// <summary>
    /// ���� TV�� ǥ��� ���� ���� �̹��� �迭
    /// </summary>
    public GameObject[] rocketSamples;
    /// <summary>
    /// ���� �������� ���� ������ �ε���
    /// </summary>
    private int rocketSampleIndex = -1;

    public StoreManager storeManager;
    public DataManager dataManager;

    /// <summary>
    /// �߾� TV�� Ȯ��Ǿ� �ִ��� ����
    /// </summary>
    private bool isTvExpanded
    {
        get
        {
            if (tv.transform.localScale.x < 1)
                return false;
            else
                return true;
        }
    }

    private void Start()
    {
        StartCoroutine(ClockTimer());
    }

    /// <summary>
    /// �߻� ���� ������ ����
    /// </summary>
    public void EnterLaunchScene()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// ���õ� ���� Ȱ��ȭ
    /// </summary>
    /// <param name="appSelect">Inspector���� ������ ��</param>
    public void OpenApp(AppSelect appSelect)
    {
        // Ȱ��ȭ �� ���� ������Ʈ
        openedApp = appSelect.appList;

        // TV�� Ȯ��Ǿ����� ������ TV�� Ȯ���ϰ� �� ���� ��ư�� Ȱ��ȭ�Ѵ�.
        if (!isTvExpanded)
        {
            TvOnOff();
            PlayManager.AfterAnimationPlay afterAniPlay = OpenAppAni;
            afterAniPlay += CloseButtonAppearance;
            PlayManager.instance.WaitAnimation(tv_ani, afterAniPlay);
        }
        // TV�� Ȯ��Ǿ� ������ ���� Ȱ��ȭ�Ѵ�.
        else
        {
            OpenAppAni();
        }
        
    }
    /// <summary>
    /// ���õ� ���� TV�� Ȱ��ȭ�Ѵ�.
    /// </summary>
    private void OpenAppAni()
    {
        apps_obj[(int)openedApp - 1].SetActive(true);
        OpenAni(apps_ani[(int)openedApp - 1], "AppOpening", apps_obj[(int)openedApp - 1]);
    }
    /// <summary>
    /// TV �� ���� ��ư�� Ȱ��ȭ/��Ȱ��ȭ �Ѵ�.
    /// </summary>
    private void CloseButtonAppearance()
    {
        if(!closeButtonAppear_ani.gameObject.activeInHierarchy)
        {
            closeButtonAppear_ani.gameObject.SetActive(true);
            OpenAni(closeButtonAppear_ani, "CloseButtonAppear", closeButtonAppear_ani.gameObject);
            PlayManager.instance.WaitAnimation(closeButtonAppear_ani, SetCloseButtonOn);
        }
        else
        {
            CloseAni(closeButtonAppear_ani, "CloseButtonAppear", closeButtonAppear_ani.gameObject);
            PlayManager.instance.WaitAnimation(closeButtonAppear_ani, SetCloseButtonOff);
        }
    }

    /// <summary>
    /// on/off �ִϸ��̼ǿ��� Ȱ��ȭ�� �����Ѵ�.
    /// </summary>
    /// <param name="ani">Ȱ��ȭ�� �ִϸ��̼�</param>
    /// <param name="aniName">�ִϸ��̼� Ŭ�� �̸�</param>
    /// <param name="obj">Ȱ��ȭ ��� ������Ʈ</param>
    /// <param name="aniIsMain">�ش� Ŭ���� �������� ����</param>
    private void OpenAni(Animation ani, string aniName, GameObject obj, bool aniIsMain = true)
    {
        obj.SetActive(true);
        ani[aniName].speed = 1;
        if (aniIsMain)
            ani.Play();
        else
            ani.Play(aniName);
    }

    /// <summary>
    /// ���ø����̼� ���� ó��
    /// </summary>
    public void CloseApp()
    {
        // �� ���� ��ư ���� ������Ʈ
        CloseButtonAppearance();
        // �� ���� �� TV ��� �ִϸ��̼� ȿ�� ����
        CloseApp(openedApp);
        CloseAni(apps_ani[(int)openedApp - 1], "AppOpening", apps_obj[(int)openedApp - 1]);
        PlayManager.AfterAnimationPlay afterAniPlay = TvOnOff;
        PlayManager.instance.WaitAnimation(apps_ani[(int)openedApp - 1], afterAniPlay);
        dataManager.SaveData();
    }

    /// <summary>
    /// ���� ��ư Ȱ��ȭ
    /// </summary>
    private void SetCloseButtonOn()
    {
        closeButtonAppear_ani["CloseButtonAppear"].time = closeButtonAppear_ani["CloseButtonAppear"].length;
    }

    /// <summary>
    /// ���� ��ư ��Ȱ��ȭ
    /// </summary>
    private void SetCloseButtonOff()
    {
        closeButtonAppear_ani["CloseButtonAppear"].time = 0;
    }

    /// <summary>
    /// ������ ���� ���� ó��
    /// </summary>
    /// <param name="tabletApp">������ ��</param>
    private void CloseApp(TabletAppList tabletApp)
    {
        switch(tabletApp)
        {
            // ������ ��� ���� Ȯ�� â ����
            case TabletAppList.Store:
                storeManager.Cancel();
                break;
        }
    }

    /// <summary>
    /// on/off �ִϸ��̼ǿ��� ��Ȱ��ȭ��(�����) �����Ѵ�.
    /// </summary>
    /// <param name="ani">Ȱ��ȭ�� �ִϸ��̼�</param>
    /// <param name="aniName">�ִϸ��̼� Ŭ�� �̸�</param>
    /// <param name="obj">Ȱ��ȭ ��� ������Ʈ</param>
    /// <param name="aniIsMain">�ش� Ŭ���� �������� ����</param>
    private void CloseAni(Animation ani, string aniName, GameObject obj, bool aniIsMain = true)
    {
        ani[aniName].speed = -1;
        ani[aniName].time = ani[aniName].length;
        ani.Play();
        
        if (aniIsMain)
        {
            ani.Play();
        }
        else
        {
            ani.Play(aniName);
        }
        PlayManager.instance.WaitAnimation(ani, CloseWindow, obj);
    }
    /// <summary>
    /// ������ ������Ʈ ��Ȱ��ȭ
    /// </summary>
    /// <param name="obj">��Ȱ��ȭ �� ������Ʈ</param>
    private void CloseWindow(GameObject obj)
    {
        obj.SetActive(false);
    }
    /// <summary>
    /// TV�� Ȯ��/����Ѵ�.
    /// </summary>
    private void TvOnOff()
    {
        // Ȯ��Ǿ� ������ ���
        if(isTvExpanded)
        {
            tv_ani["OpenTv"].speed = -1;
            tv_ani["OpenTv"].time = tv_ani["OpenTv"].length;
            starMove.StartMove();
        }
        // ��ҵǾ� ������ Ȯ��
        else
        {
            tv_ani["OpenTv"].speed = 1;
            starMove.StopMove();
        }

        // �� ���� Ȱ��ȭ/��Ȱ��ȭ
        AppDrawerOnOff();
        tv_ani.Play();
    }
    private void AppDrawerOnOff()
    {
        // TV�� Ȯ��ǰ� ������ �� Ȱ��ȭ
        if(isTvExpanded)
        {
            OpenAni(appDrawer_ani, "AppDrawer", appDrawer);
        }
        // TV�� ��ҵǰ� ������ �� ��Ȱ��ȭ
        else
        {
            CloseAni(appDrawer_ani, "AppDrawer", appDrawer);
        }
    }

    /// <summary>
    /// �ֱ������� �ð� ���� ������Ʈ ���ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="tick">�ð� ������ �ݷ� ����</param>
    /// <returns></returns>
    IEnumerator ClockTimer(bool tick = true)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if(tick)
            clock_text.text = string.Format("{0:hh}:{0:mm}:{0:ss}", DateTime.Now);
        else
            clock_text.text = string.Format("{0:hh}<color=black>:</color>{0:mm}<color=black>:</color>{0:ss}", DateTime.Now);

        StartCoroutine(ClockTimer(!tick));
    }

    /// <summary>
    /// ���� ���� �̹��� ������Ʈ
    /// </summary>
    /// <param name="rocketType">ǥ���� ������ �ε���</param>
    public void UpdateRocketSample(int rocketType)
    {
        // �ε����� -1�̸� ���� �̹��� ��Ȱ��ȭ
        if (rocketSampleIndex != -1)
            rocketSamples[rocketSampleIndex].SetActive(false);

        rocketSampleIndex = rocketType;
        rocketSamples[rocketSampleIndex].SetActive(true);
    }
}
