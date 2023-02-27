using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// �º� �� ���� ������
/// </summary>
public enum TabletAppList { GameOver = 0, Settings = 1, Equipment = 2, Store = 3, Stat = 4 }

/// <summary>
/// �º� �ý��� ���� Ŭ����
/// </summary>
public class TabletManager : MonoBehaviour
{
    /// <summary>
    /// �º� ���� ĵ����
    /// </summary>
    public Canvas tablet;
    /// <summary>
    /// �º� on/off �ִϸ��̼� ȿ��
    /// </summary>
    public Animation tablet_ani;

    /// <summary>
    /// �º� �� �׷� ������Ʈ �迭
    /// </summary>
    public GameObject[] appArray;
    /// <summary>
    /// �º� �� �׷� RectTransform �迭
    /// </summary>
    public RectTransform[] appRectTransforms;
    /// <summary>
    /// �º� �� on/off �ִϸ��̼� ȿ�� �迭
    /// </summary>
    public Animation[] appAniArray;
    /// <summary>
    /// ��Ƽ�½�ŷ ���� �� ���� ��ư �迭
    /// </summary>
    public GameObject[] appMultiButtons;
    /// <summary>
    /// ���� ���� ������� ����
    /// </summary>
    public bool[] hasBrightBackgound;
    /// <summary>
    /// ���� ��� ���󿡼��� �׺���̼ǹ� ����
    /// </summary>
    public Color colorOnBright;

    /// <summary>
    /// �׺���̼� ��ư �̹���
    /// </summary>
    public Image[] underButtonImgs;

    /// <summary>
    /// �º� �ð� �ؽ�Ʈ
    /// </summary>
    public Text time_text;
    
    /// <summary>
    /// ���� ȭ���� Ȩ ȭ������ ����
    /// </summary>
    private bool isHome = true;
    /// <summary>
    /// �ֱٿ� ������ �� (Home/back ��ư�� ���Ǳ� ����)
    /// </summary>
    private TabletAppList recentApp;
    /// <summary>
    /// �ֱٿ� ������ �� ����Ʈ (��Ƽ�½�ŷ ����Ʈ)
    /// </summary>
    private List<TabletAppList> openedApps = new List<TabletAppList>();

    /// <summary>
    /// ���� hierarchy ���� �θ� Transform
    /// </summary>
    public Transform appParent;
    /// <summary>
    /// ���� �⺻ ��ġ
    /// </summary>
    private Vector3 appBasicPos;
    /// <summary>
    /// ������ ���� ��
    /// </summary>
    private Vector3 contentStdPos;
    /// <summary>
    /// ��Ƽ �½�ŷ ȭ���� ScrollView ������Ʈ
    /// </summary>
    public GameObject multiView;
    /// <summary>
    /// ��Ƽ �½�ŷ ȭ���� ScrollView-Content ������Ʈ
    /// </summary>
    public GameObject multiContents;
    /// <summary>
    /// ��Ƽ �½�ŷ ȭ���� ScrollView-Content RectTransform
    /// </summary>
    private RectTransform multiContent_rect;
    /// <summary>
    /// �ֱٿ� ������ ���� �����ٴ� �޽����� �����ִ� �ؽ�Ʈ
    /// </summary>
    public Text noApps_text;
    /// <summary>
    /// ��Ƽ �½�ŷ ����Ʈ�� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation multi_ani;

    /// <summary>
    /// �Ͻ������� Ȱ��ȭ ��ų ��ư �׷� ������Ʈ
    /// </summary>
    public GameObject buttonsforPause;
    /// <summary>
    /// �簳 �޽��� ������Ʈ
    /// </summary>
    public GameObject resumeMessage;
    /// <summary>
    /// �Ͻ����� ��ư ������Ʈ
    /// </summary>
    public GameObject pause_button;
    /// <summary>
    /// ���� �Ͻ����� ����
    /// </summary>
    public static bool isPause = false;
    /// <summary>
    /// �Ͻ����� ������ ���� RigidBody Ÿ��
    /// </summary>
    private RigidbodyType2D priorType;
    /// <summary>
    /// �ڷΰ��� ��ư Ŭ���� ���� RectTransform
    /// </summary>
    public RectTransform backBlock_RT;

    /// <summary>
    /// �߻� ���� ȿ���� �����
    /// </summary>
    public AudioSource beforeLaunchAudio;
    /// <summary>
    /// ���� ȭ�� ȿ����
    /// </summary>
    public AudioSource rocketFireAudio;
    /// <summary>
    /// �� ���ε� �ʿ� ����
    /// </summary>
    public static bool needReload = false;

    public DataManager dataManager;
    public AndroidController androidController;
    public StoreManager storeManager;
    public Settings settings;

    private void Start()
    {
        // �� �⺻ ��ġ �ʱ�ȭ
        appBasicPos = appArray[1].transform.localPosition;
        multiContent_rect = multiContents.GetComponent<RectTransform>();
        backBlock_RT.sizeDelta = new Vector2((float)Screen.width / Screen.height * 1080, 1080);
    }

    /// <summary>
    /// ������ �� Ȱ��ȭ
    /// </summary>
    /// <param name="app">������ ��</param>
    public void OpenApp(TabletAppList app)
    {
        // �º��� ��Ȱ��ȭ �Ǿ������� Ȱ��ȭ �۾�
        if(!tablet.enabled)
            OnOffTablet(true);
        // �� Ȱ��ȭ
        appArray[(int)app].SetActive(true);
        // ���ӿ��� ȭ���� �ƴϸ� �ִϸ��̼� ȿ�� ����
        if (app != TabletAppList.GameOver)
        {
            OpenAni(appAniArray[(int)app - 1], "AppOpening", appArray[(int)app]);
            AddToList(app);
        }
        // Ȩ ȭ���� �ƴ϶�� ǥ�� �� �ֱ� �� ���
        isHome = false;
        recentApp = app;
        // �׺���̼� �� ���� ����
        if(hasBrightBackgound[(int)app])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;
        // �ð� ���� ������Ʈ �ڷ�ƾ ����
        StartCoroutine(TimeSet());
    }
    /// <summary>
    /// ������ �� Ȱ��ȭ
    /// </summary>
    /// <param name="appSelect">������ ��(Inspector)</param>
    public void OpenApp(AppSelect appSelect)
    {
        OpenApp(appSelect.appList);
    }
    /// <summary>
    /// �ֱٿ� ����� �� ����Ʈ�� �߰�
    /// </summary>
    /// <param name="app">�߰��� ��</param>
    private void AddToList(TabletAppList app)
    {
        bool added = false;
        // �̹� ����Ʈ�� �ִ� ������ Ȯ��
        for(int i = 0; i < openedApps.Count - 1; i++)
        {
            if(app.Equals(openedApps[i]))
            {
                // ���� �� ������ ����
                SwapOpenedApps(openedApps.Count - 1, i);
                added = true;
                break;
            }
        }
        // ����Ʈ�� ��������� �߰�
        if(openedApps.Count.Equals(0))
            openedApps.Add(app);
        // ����Ʈ�� ���� ���̸� �߰�
        else if (!app.Equals(openedApps[openedApps.Count - 1]) && !added)
            openedApps.Add(app);
    }

    /// <summary>
    /// �ֱٿ� ����� �� ����Ʈ ����
    /// </summary>
    /// <param name="index1">�ε��� 1</param>
    /// <param name="index2">�ε��� 2</param>
    private void SwapOpenedApps(int index1, int index2)
    {
        TabletAppList temp = openedApps[index1];
        openedApps[index1] = openedApps[index2];
        openedApps[index2] = temp;
    }

    /// <summary>
    /// ���� �����ִ� �� ����
    /// </summary>
    public void CloseApp()
    {
        // �� ���� �ִϸ��̼� ����
        CloseAni(appAniArray[(int)recentApp - 1], "AppOpening", appArray[(int)recentApp]);
        // Ư�� �� ���� ���� ó��
        CloseApp(recentApp);
        // �ֱ� �� ����Ʈ���� ����
        openedApps.Remove(recentApp);
        if (openedApps.Count > 1)
            recentApp = openedApps[openedApps.Count - 1];
        // Ȩ ȭ�� ���� ����
        isHome = true;
        // �׺���̼ǹ� ���� ������Ʈ
        if(underButtonImgs[0].color != Color.white)
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = Color.white;

        dataManager.SaveData();
    }

    /// <summary>
    /// Ư�� �� ���� ���� �۾�
    /// </summary>
    /// <param name="tabletApp">������ ��</param>
    private void CloseApp(TabletAppList tabletApp)
    {
        switch(tabletApp)
        {
            case TabletAppList.Store:
                storeManager.Cancel();
                break;
        }
    }

    /// <summary>
    /// �º� on/off �ִϸ��̼� ���
    /// </summary>
    /// <param name="on">Ȱ��ȭ ����</param>
    public void OnOffTablet(bool on)
    {
        if (on)
        {
            OpenAni(tablet_ani, "OpenTablet", tablet);
            // �߻� ���� ȿ���� ���� ����
            beforeLaunchAudio.volume = settings.GetEffectVolume() * 0.2f;
        }
        else
        {
            CloseAni(tablet_ani, "OpenTablet", tablet);
            // �߻� ���� ȿ���� ���� ����
            beforeLaunchAudio.volume = settings.GetEffectVolume();
            // �� ���ε� �ʿ��, ���ε�
            if (needReload)
            {
                needReload = false;
                PlayManager.instance.WaitAnimation(tablet_ani, Reload);
            }
        }
    }

    /// <summary>
    /// �� ���ε�
    /// </summary>
    private void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// ĵ���� ��Ȱ��ȭ ó��
    /// </summary>
    /// <param name="cvs">��Ȱ��ȭ �� ĵ����</param>
    private void AfterCloseCanvas(Canvas cvs)
    {
        cvs.enabled = false;
    }

    /// <summary>
    /// ���� ������Ʈ ��Ȱ��ȭ ó��
    /// </summary>
    /// <param name="obj">��Ȱ��ȭ �� ���� ������Ʈ</param>
    private void AfterCloseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary>
    /// �׺���̼� �� ��ư Ŭ�� ������
    /// </summary>
    /// <param name="key">��ư Ű</param>
    public void OnHomeScreen(int key)
    {
        // �Ͻ����� ���� ó��
        if (isPause && key != 2)
            Resume();
        // �Ϲ� ��Ȳ������ ��ư �̺�Ʈ ó��
        switch (key)
        {
            case 0: // �ڷΰ��� ��ư
                // ��Ƽ�½�ŷ ���� ����� �� ����ó��
                if (multiView.activeInHierarchy)
                {
                    CancelMulti();
                }
                // �� ���� ó��
                else if (!isHome && openedApps.Count > 0)
                {
                    CloseApp();
                }
                // �º� ���� ó��
                else if (isHome && PlayManager.isSurvive)
                {
                    OnOffTablet(false);
                }
                break;
            case 1: // Ȩ ��ư
                // ��Ƽ�½�ŷ ��� ���� ó��
                if (multiView.activeInHierarchy)
                {
                    for (int i = 0; i < openedApps.Count; i++)
                    {
                        appArray[i].SetActive(false);
                    }
                    isHome = true;
                    CancelMulti();
                }
                // �º� ���� ó��
                else if (isHome)
                {
                    OnOffTablet(false);
                }
                // �Ϲ� �� ���� ó��
                else
                {
                    // ���� ������ �ƴ� ���� ���� �����ΰ� �ֱ� �� �� ����Ʈ�� �߰�
                    if (PlayManager.isSurvive)
                    {
                        CloseAni(appAniArray[(int)recentApp - 1], "AppOpening", appArray[(int)recentApp]);
                        for (int i = 0; i < openedApps.Count; i++)
                        {
                            if ((int)recentApp != i)
                                appArray[i].SetActive(false);
                        }
                        isHome = true;
                        if(underButtonImgs[0].color != Color.white)
                            for (int i = 0; i < 3; i++)
                                underButtonImgs[i].color = Color.white;

                        dataManager.SaveData();
                    }
                    else
                    {
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("tabletmsg00"));
                    }
                }
                break;
            case 2: // ��Ƽ�½�ŷ ���
                // �Ͻ������� �ƴ� ������
                if (!isPause)
                {
                    // ���ӿ����� �ƴ� ��
                    if (PlayManager.isSurvive)
                    {
                        // ��Ƽ�½�ŷ ��尡 �ƴ� �� ��Ƽ�½�ŷ Ȱ��ȭ
                        if (!multiView.activeInHierarchy)
                            MultitaskingMode();
                        else
                        {
                            // ����Ʈ�� ��������� ��Ƽ�½�ŷ ��� ����
                            if (openedApps.Count.Equals(0))
                                CancelMulti();
                            // �װ� �ƴϸ� �ֱٿ� ������ �۰� ���� �� ����
                            else if (openedApps.Count.Equals(1))
                                SelectMulti((int)openedApps[openedApps.Count - 1]);
                            else
                                SelectMulti((int)openedApps[openedApps.Count - 2]);
                        }
                    }
                    else
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("tabletmsg01"));
                }
                break;
        }
    }

    /// <summary>
    /// ��Ƽ �½�ŷ ��� Ȱ��ȭ
    /// </summary>
    private void MultitaskingMode()
    {
        multiView.SetActive(true);
        // Ȩ��ư�� ����� �ִϸ��̼� ȿ��
        if (isHome)
            OpenAni(multi_ani, "OpenMultitasking_home", multiView);
        // �׿��� ����� �ִϸ��̼� ȿ��
        else
            OpenAni(multi_ani, "OpenMultitasking", multiView, false);

        // �׺���̼� �� ��ư ���� ����
        if(underButtonImgs[0].color != Color.white)
        {
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = Color.white;
        }

        // �ֱ� ���� ���� ��쿡 ���� �޽��� ǥ��
        if (openedApps.Count.Equals(0))
        {
            noApps_text.gameObject.SetActive(true);
            noApps_text.text = LocalizationManager.instance.GetLocalizedValue("noappsmsg");
        }
        // ����Ʈ ������ ���� ��ġ ����
        else
        {
            noApps_text.gameObject.SetActive(false);
            contentStdPos = multiContent_rect.transform.localPosition;
            for (int i = openedApps.Count - 1; i >= 0; i--)
            {
                // ��Ƽ�½�ŷ �Ʒ��� �� �׷� �̵�
                int index = (int)openedApps[i];
                appArray[index].SetActive(true);
                appMultiButtons[index - 1].SetActive(true);
                appArray[index].transform.SetParent(multiContents.transform);
                if(appAniArray[index - 1].isPlaying)
                    appAniArray[index - 1].Stop();
                appArray[index].transform.localScale = new Vector3(0.35f, 0.35f, 1);
                appRectTransforms[index - 1].anchoredPosition = GetMultiPos(i);
            }
        }
    }

    /// <summary>
    /// ��Ƽ�½�ŷ ��忡���� �� ��ġ ���ϱ�
    /// </summary>
    /// <param name="index">����Ʈ ���� �ε���</param>
    /// <returns>��ġ</returns>
    private Vector3 GetMultiPos(int index)
    {
        float x = 360f, y = 200f;
        if (openedApps.Count.Equals(1))
        {
            return Vector3.zero;
        }
        else if (openedApps.Count.Equals(2))
        {
            if (index.Equals(0))
                return new Vector3(-x, 0, 0);
            else
                return new Vector3(x, 0, 0);
        }
        else
        {
            index = openedApps.Count - index - 1;
            switch (index)
            {
                case 0:
                    return new Vector3(x, y, 0);
                case 1:
                    return new Vector3(-x, y, 0);
                case 2: 
                    return new Vector3(x, -y, 0);
                case 3:
                    return new Vector3(-x, -y, 0);
            }

            return Vector3.zero;
        }
    }

    /// <summary>
    /// ��Ƽ�½�ŷ������ �� ����
    /// </summary>
    /// <param name="appCode">�� �ڵ�</param>
    public void SelectMulti(int appCode)
    {
        multiView.SetActive(false);
        multiContent_rect.transform.localPosition = contentStdPos;

        // �׺���̼� ��ư ���� ����
        if (hasBrightBackgound[appCode])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;

        // ���� hierarchy �� ��ġ ���󺹱�
        for (int i = openedApps.Count - 1; i >= 0; i--)
        {
            int index = (int)openedApps[i];
            appArray[index].transform.SetParent(appParent);
            appMultiButtons[index - 1].SetActive(false);
            appArray[index].transform.localPosition = appBasicPos;
            if (appCode != index)
            {
                appArray[index].transform.localScale = new Vector3(1, 1, 1);
                appArray[index].SetActive(false);
            }
        }
        // ���õ� ���� Ȱ��ȭ ó��
        OpenAni(appAniArray[appCode - 1], "AppOpening", appArray[appCode]);
        isHome = false;
        AddToList((TabletAppList)appCode);
        recentApp = (TabletAppList)appCode;
        appArray[appCode].SetActive(true);
    }

    /// <summary>
    /// ��Ƽ�½�ŷ ��� ��� ó��
    /// </summary>
    public void CancelMulti()
    {
        multiContent_rect.transform.localPosition = contentStdPos;

        // �׺���̼� ��ư ���� ����
        if (hasBrightBackgound[(int)recentApp])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;

        // ���� hierarchy �� ��ġ ���󺹱�
        for (int i = openedApps.Count - 1; i >= 0; i--)
        {
            int index = (int)openedApps[i];
            appArray[index].transform.SetParent(appParent);
            appMultiButtons[index - 1].SetActive(false);
            appArray[index].transform.localPosition = appBasicPos;
            appArray[index].transform.localScale = new Vector3(1, 1, 1);

            if (i != openedApps.Count - 1 || isHome)
            {
                appArray[index].SetActive(false);
            }
        }
        // ���ǿ� ���� �ִϸ��̼� ȿ�� ���
        if (isHome)
            CloseAni(multi_ani, "OpenMultitasking_home", multiView);
        else
        {
            multiView.SetActive(false);
            OpenAni(appAniArray[(int)openedApps[openedApps.Count - 1] - 1], "AppOpening", appArray[(int)openedApps[openedApps.Count - 1]]);
        }
    }

    /// <summary>
    /// �ð� ���� ������Ʈ �ڷ�ƾ
    /// </summary>
    IEnumerator TimeSet()
    {
        time_text.text = DateTime.Now.ToString("hh:mm");

        yield return new WaitForSeconds(10f);

        if (tablet.enabled)
            StartCoroutine(TimeSet());
    }
    
    /// <summary>
    /// �Ͻ�����/�簳 ó��
    /// </summary>
    public void Pause_button()
    {
        if (isPause)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// �Ͻ����� ó��
    /// </summary>
    public void Pause()
    {
        isPause = true;
        OnOffTablet(true);
        appArray[(int)TabletAppList.Settings].SetActive(true);
        buttonsforPause.SetActive(true);
        resumeMessage.SetActive(true);
        rocketFireAudio.Pause();
        priorType = RocketSet.instance.rocketMain_rigid.bodyType;
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Static;
    }
    /// <summary>
    /// ���� �簳 ó��
    /// </summary>
    public void Resume()
    {
        isPause = false;
        buttonsforPause.SetActive(false);
        resumeMessage.SetActive(false);
        rocketFireAudio.Play();
        appArray[(int)TabletAppList.Settings].SetActive(false);
        OnOffTablet(false);
        RocketSet.instance.rocketMain_rigid.bodyType = priorType;
    }
    
    /// <summary>
    /// ���� ȭ������ ���ư���
    /// </summary>
    public void GoToEntryScene()
    {
        isPause = false;
        dataManager.SaveData();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// on/off �ִϸ��̼ǿ��� Ȱ��ȭ�� �����Ѵ�.
    /// </summary>
    /// <param name="ani">Ȱ��ȭ�� �ִϸ��̼�</param>
    /// <param name="aniName">�ִϸ��̼� Ŭ�� �̸�</param>
    /// <param name="cvs">Ȱ��ȭ ��� ĵ����</param>
    /// <param name="aniIsMain">�ش� Ŭ���� �������� ����</param>
    private void OpenAni(Animation ani, string aniName, Canvas cvs, bool aniIsMain = true)
    {
        cvs.enabled = true;
        ani[aniName].speed = 1;
        if (aniIsMain)
            ani.Play();
        else
            ani.Play(aniName);
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
    /// on/off �ִϸ��̼ǿ��� ��Ȱ��ȭ��(�����) �����Ѵ�.
    /// </summary>
    /// <param name="ani">Ȱ��ȭ�� �ִϸ��̼�</param>
    /// <param name="aniName">�ִϸ��̼� Ŭ�� �̸�</param>
    /// <param name="cvs">Ȱ��ȭ ��� ĵ����</param>
    /// <param name="aniIsMain">�ش� Ŭ���� �������� ����</param>
    private void CloseAni(Animation ani, string aniName, Canvas cvs, bool aniIsMain = true)
    {
        ani[aniName].speed = -1;
        ani[aniName].time = ani[aniName].length;
        ani.Play();
        if (aniIsMain)
        {
            ani.Play();
            PlayManager.instance.WaitAnimation(ani, AfterCloseCanvas, cvs);
        }
        else
        {
            ani.Play(aniName);
            PlayManager.instance.WaitAnimation(ani, AfterCloseCanvas, cvs);
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
            PlayManager.instance.WaitAnimation(ani, AfterCloseObject, obj);
        }
        else
        {
            ani.Play(aniName);

            PlayManager.instance.WaitAnimation(ani, AfterCloseObject, obj);
        }
    }
}
