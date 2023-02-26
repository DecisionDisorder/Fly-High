using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum TabletAppList { GameOver = 0, Settings = 1, Equipment = 2, Store = 3, Stat = 4 }

public class TabletManager : MonoBehaviour
{
    public Canvas tablet;
    public Animation tablet_ani;

    public GameObject[] appArray;
    public RectTransform[] appRectTransforms;
    public Animation[] appAniArray;
    public GameObject[] appMultiButtons;
    public bool[] hasBrightBackgound;
    public Color colorOnBright;

    public Image[] underButtonImgs;

    public Text time_text;
    //private Animation toastMsg_ani;
    private bool isHome = true;
    private TabletAppList recentApp; // for home / back
    private List<TabletAppList> openedApps = new List<TabletAppList>(); // for multi

    public Transform appParent;
    private Vector3 appBasicPos;
    private Vector3 contentStdPos;
    public GameObject multiView;
    public GameObject multiContents;
    private RectTransform multiContent_rect;
    public Text noApps_text;
    public Animation multi_ani;

    public GameObject buttonsforPause;
    public GameObject resumeMessage;
    public GameObject pause_button;
    public static bool isPause = false;
    private RigidbodyType2D priorType;
    public RectTransform backBlock_RT;

    public AudioSource beforeLaunchAudio;
    public AudioSource rocketFireAudio;
    public static bool needReload = false;

    public DataManager dataManager;
    public AndroidController androidController;
    public StoreManager storeManager;
    public Settings settings;

    private void Start()
    {
        appBasicPos = appArray[1].transform.localPosition;
        multiContent_rect = multiContents.GetComponent<RectTransform>();
        backBlock_RT.sizeDelta = new Vector2((float)Screen.width / Screen.height * 1080, 1080);
    }

    public void OpenApp(TabletAppList app)
    {
        if(!tablet.enabled)
            OnOffTablet(true);
        appArray[(int)app].SetActive(true);
        if (app != TabletAppList.GameOver)
        {
            OpenAni(appAniArray[(int)app - 1], "AppOpening", appArray[(int)app]);
            AddToList(app);
        }
        isHome = false;
        recentApp = app;
        if(hasBrightBackgound[(int)app])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;
        StartCoroutine(TimeSet());
    }
    public void OpenApp(AppSelect appSelect)
    {
        OpenApp(appSelect.appList);
    }
    private void AddToList(TabletAppList app)
    {
        bool added = false;
        for(int i = 0; i < openedApps.Count - 1; i++)
        {
            if(app.Equals(openedApps[i]))
            {
                SwapOpenedApps(openedApps.Count - 1, i);
                added = true;
                break;
            }
        }
        if(openedApps.Count.Equals(0))
            openedApps.Add(app);
        else if (!app.Equals(openedApps[openedApps.Count - 1]) && !added)
            openedApps.Add(app);
    }

    private void SwapOpenedApps(int index1, int index2)
    {
        TabletAppList temp = openedApps[index1];
        openedApps[index1] = openedApps[index2];
        openedApps[index2] = temp;
    }

    public void CloseApp()
    {
        CloseAni(appAniArray[(int)recentApp - 1], "AppOpening", appArray[(int)recentApp]);
        CloseApp(recentApp);
        openedApps.Remove(recentApp);
        if (openedApps.Count > 1)
            recentApp = openedApps[openedApps.Count - 1];
        isHome = true;
        if(underButtonImgs[0].color != Color.white)
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = Color.white;

        dataManager.SaveData();
    }

    private void CloseApp(TabletAppList tabletApp)
    {
        switch(tabletApp)
        {
            case TabletAppList.Store:
                storeManager.Cancel();
                break;
        }
    }

    public void OnOffTablet(bool on)
    {
        if (on)
        {
            OpenAni(tablet_ani, "OpenTablet", tablet);
            beforeLaunchAudio.volume = settings.GetEffectVolume() * 0.2f;
        }
        else
        {
            CloseAni(tablet_ani, "OpenTablet", tablet);
            beforeLaunchAudio.volume = settings.GetEffectVolume();
            if (needReload)
            {
                needReload = false;
                PlayManager.instance.WaitAnimation(tablet_ani, Reload);
            }
        }
    }

    private void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void AfterCloseCanvas(Canvas cvs)
    {
        cvs.enabled = false;
    }
    private void AfterCloseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OnHomeScreen(int key)
    {
        if (isPause && key != 2)
            Resume();
        switch (key)
        {
            case 0: //back
                if (multiView.activeInHierarchy)
                {
                    CancelMulti();
                }
                else if (!isHome && openedApps.Count > 0)
                {
                    CloseApp();
                }
                else if (isHome && PlayManager.isSurvive)
                {
                    OnOffTablet(false);
                }
                break;
            case 1: //home
                if (multiView.activeInHierarchy)
                {
                    for (int i = 0; i < openedApps.Count; i++)
                    {
                        appArray[i].SetActive(false);
                    }
                    isHome = true;
                    CancelMulti();
                }
                else if (isHome)
                {
                    OnOffTablet(false);
                }
                else
                {
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
            case 2: //multi
                if (!isPause)
                {
                    if (PlayManager.isSurvive)
                    {
                        if (!multiView.activeInHierarchy)
                            MultitaskingMode();
                        else
                        {
                            if (openedApps.Count.Equals(0))
                                CancelMulti();
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

    private void MultitaskingMode()
    {
        multiView.SetActive(true);
        if (isHome)
            OpenAni(multi_ani, "OpenMultitasking_home", multiView);
        else
            OpenAni(multi_ani, "OpenMultitasking", multiView, false);

        if(underButtonImgs[0].color != Color.white)
        {
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = Color.white;
        }

        if (openedApps.Count.Equals(0))
        {
            noApps_text.gameObject.SetActive(true);
            noApps_text.text = LocalizationManager.instance.GetLocalizedValue("noappsmsg");
        }
        else
        {
            noApps_text.gameObject.SetActive(false);
            //multiContent_rect.offsetMin = new Vector2(-(openedApps.Count - 2 >= 0 ? openedApps.Count - 2 : 0) * 250f, multiContent_rect.offsetMin.y);
            contentStdPos = multiContent_rect.transform.localPosition;
            for (int i = openedApps.Count - 1; i >= 0; i--)
            {
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

    public void SelectMulti(int appCode)
    {
        multiView.SetActive(false);
        multiContent_rect.transform.localPosition = contentStdPos;

        if (hasBrightBackgound[appCode])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;

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
        OpenAni(appAniArray[appCode - 1], "AppOpening", appArray[appCode]);
        isHome = false;
        AddToList((TabletAppList)appCode);
        recentApp = (TabletAppList)appCode;
        appArray[appCode].SetActive(true);
    }

    public void CancelMulti()
    {
        multiContent_rect.transform.localPosition = contentStdPos;

        if (hasBrightBackgound[(int)recentApp])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;

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
        if (isHome)
            CloseAni(multi_ani, "OpenMultitasking_home", multiView);
        else
        {
            multiView.SetActive(false);
            OpenAni(appAniArray[(int)openedApps[openedApps.Count - 1] - 1], "AppOpening", appArray[(int)openedApps[openedApps.Count - 1]]);
        }
    }

    IEnumerator TimeSet()
    {
        time_text.text = DateTime.Now.ToString("hh:mm");

        yield return new WaitForSeconds(10f);

        if (tablet.enabled)
            StartCoroutine(TimeSet());
    }
    
    /*public void ToastMessage(string msg, float delay = 0.5f)
    {
        if(timeDelay != null)
            StopCoroutine(timeDelay);
        OpenAni(toastMsg_ani, "OpenToastMsg", toastMsg_obj);

        toastMsg_text.text = msg;

        timeDelay = TimeDelay(delay);
        StartCoroutine(timeDelay);
    }*/

    public void Pause_button()
    {
        if (isPause)
            Resume();
        else
            Pause();
    }


    public void Pause()
    {
        isPause = true;
        OnOffTablet(true);
        appArray[(int)TabletAppList.Settings].SetActive(true);
        buttonsforPause.SetActive(true);
        resumeMessage.SetActive(true);
        rocketFireAudio.Pause();
        /*tablet_ani.Stop();
        tablet_ani["OpenTablet"].time = tablet_ani["OpenTablet"].length;
        appAniArray[(int)TabletAppList.Settings - 1].Stop();
        appAniArray[(int)TabletAppList.Settings - 1]["AppOpening"].time = appAniArray[(int)TabletAppList.Settings - 1]["AppOpening"].length;*/
        priorType = RocketSet.instance.rocketMain_rigid.bodyType;
        RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Static;
    }
    public void Resume()
    {
        isPause = false;
        buttonsforPause.SetActive(false);
        resumeMessage.SetActive(false);
        rocketFireAudio.Play();
        appArray[(int)TabletAppList.Settings].SetActive(false);
        OnOffTablet(false);
        /*tablet_ani["OpenTablet"].time = 0f;
        tablet_ani.Stop();
        appAniArray[(int)TabletAppList.Settings - 1]["AppOpening"].time = 0f;
        appAniArray[(int)TabletAppList.Settings - 1].Stop();*/
        RocketSet.instance.rocketMain_rigid.bodyType = priorType;
    }

    public void GoToEntryScene()
    {
        isPause = false;
        dataManager.SaveData();
        SceneManager.LoadScene(0);
    }

    private void OpenAni(Animation ani, string aniName, Canvas cvs, bool aniIsMain = true)
    {
        cvs.enabled = true;
        ani[aniName].speed = 1;
        if (aniIsMain)
            ani.Play();
        else
            ani.Play(aniName);
    }

    private void OpenAni(Animation ani, string aniName, GameObject obj, bool aniIsMain = true)
    {
        obj.SetActive(true);
        ani[aniName].speed = 1;
        if (aniIsMain)
            ani.Play();
        else
            ani.Play(aniName);
    }

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
