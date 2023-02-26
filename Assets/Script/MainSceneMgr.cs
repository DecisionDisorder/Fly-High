using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 입장 대기 화면 관리 클래스
/// </summary>
public class MainSceneMgr : MonoBehaviour
{
    /// <summary>
    /// 중앙 TV 게임 오브젝트
    /// </summary>
    public GameObject tv;
    /// <summary>
    /// 중앙 TV의 애니메이션 효과
    /// </summary>
    public Animation tv_ani;
    /// <summary>
    /// 앱 서랍 게임 오브젝트
    /// </summary>
    public GameObject appDrawer;
    /// <summary>
    /// 앱 서랍 on/off 애니메이션 효과
    /// </summary>
    public Animation appDrawer_ani;
    /// <summary>
    /// 앱 on/off 애니메이션 효과
    /// </summary>
    public Animation[] apps_ani;
    /// <summary>
    /// 앱 별 루트 게임 오브젝트 배열
    /// </summary>
    public GameObject[] apps_obj;
    /// <summary>
    /// 앱 종료 버튼 등장 애니메이션 효과
    /// </summary>
    public Animation closeButtonAppear_ani;
    /// <summary>
    /// 중앙 TV의 별 움직임 효과 관리 오브젝트
    /// </summary>
    public MainStarMove starMove;
    /// <summary>
    /// TV에 열려 있는 앱
    /// </summary>
    private TabletAppList openedApp;
    /// <summary>
    /// 시간 정보 텍스트
    /// </summary>
    public Text clock_text;

    /// <summary>
    /// 좌측 TV에 표기될 로켓 샘플 이미지 배열
    /// </summary>
    public GameObject[] rocketSamples;
    /// <summary>
    /// 현재 노출중인 로켓 샘플의 인덱스
    /// </summary>
    private int rocketSampleIndex = -1;

    public StoreManager storeManager;
    public DataManager dataManager;

    /// <summary>
    /// 중앙 TV가 확대되어 있는지 여부
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
    /// 발사 게임 씬으로 진입
    /// </summary>
    public void EnterLaunchScene()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 선택된 앱을 활성화
    /// </summary>
    /// <param name="appSelect">Inspector에서 선택한 앱</param>
    public void OpenApp(AppSelect appSelect)
    {
        // 활성화 앱 정보 업데이트
        openedApp = appSelect.appList;

        // TV가 확대되어있지 않으면 TV를 확대하고 앱 종료 버튼을 활성화한다.
        if (!isTvExpanded)
        {
            TvOnOff();
            PlayManager.AfterAnimationPlay afterAniPlay = OpenAppAni;
            afterAniPlay += CloseButtonAppearance;
            PlayManager.instance.WaitAnimation(tv_ani, afterAniPlay);
        }
        // TV가 확대되어 있으면 앱을 활성화한다.
        else
        {
            OpenAppAni();
        }
        
    }
    /// <summary>
    /// 선택된 앱을 TV에 활성화한다.
    /// </summary>
    private void OpenAppAni()
    {
        apps_obj[(int)openedApp - 1].SetActive(true);
        OpenAni(apps_ani[(int)openedApp - 1], "AppOpening", apps_obj[(int)openedApp - 1]);
    }
    /// <summary>
    /// TV 앱 종료 버튼을 활성화/비활성화 한다.
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
    /// on/off 애니메이션에서 활성화를 진행한다.
    /// </summary>
    /// <param name="ani">활성화할 애니메이션</param>
    /// <param name="aniName">애니메이션 클립 이름</param>
    /// <param name="obj">활성화 대상 오브젝트</param>
    /// <param name="aniIsMain">해당 클립이 메인인지 여부</param>
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
    /// 애플리케이션 종료 처리
    /// </summary>
    public void CloseApp()
    {
        // 앱 종료 버튼 상태 업데이트
        CloseButtonAppearance();
        // 앱 종료 후 TV 축소 애니메이션 효과 시작
        CloseApp(openedApp);
        CloseAni(apps_ani[(int)openedApp - 1], "AppOpening", apps_obj[(int)openedApp - 1]);
        PlayManager.AfterAnimationPlay afterAniPlay = TvOnOff;
        PlayManager.instance.WaitAnimation(apps_ani[(int)openedApp - 1], afterAniPlay);
        dataManager.SaveData();
    }

    /// <summary>
    /// 종료 버튼 활성화
    /// </summary>
    private void SetCloseButtonOn()
    {
        closeButtonAppear_ani["CloseButtonAppear"].time = closeButtonAppear_ani["CloseButtonAppear"].length;
    }

    /// <summary>
    /// 종료 버튼 비활성화
    /// </summary>
    private void SetCloseButtonOff()
    {
        closeButtonAppear_ani["CloseButtonAppear"].time = 0;
    }

    /// <summary>
    /// 선택한 앱을 종료 처리
    /// </summary>
    /// <param name="tabletApp">선택한 앱</param>
    private void CloseApp(TabletAppList tabletApp)
    {
        switch(tabletApp)
        {
            // 상점인 경우 구매 확인 창 종료
            case TabletAppList.Store:
                storeManager.Cancel();
                break;
        }
    }

    /// <summary>
    /// on/off 애니메이션에서 비활성화를(역재생) 진행한다.
    /// </summary>
    /// <param name="ani">활성화할 애니메이션</param>
    /// <param name="aniName">애니메이션 클립 이름</param>
    /// <param name="obj">활성화 대상 오브젝트</param>
    /// <param name="aniIsMain">해당 클립이 메인인지 여부</param>
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
    /// 선택한 오브젝트 비활성화
    /// </summary>
    /// <param name="obj">비활성화 할 오브젝트</param>
    private void CloseWindow(GameObject obj)
    {
        obj.SetActive(false);
    }
    /// <summary>
    /// TV를 확대/축소한다.
    /// </summary>
    private void TvOnOff()
    {
        // 확대되어 있으면 축소
        if(isTvExpanded)
        {
            tv_ani["OpenTv"].speed = -1;
            tv_ani["OpenTv"].time = tv_ani["OpenTv"].length;
            starMove.StartMove();
        }
        // 축소되어 있으면 확대
        else
        {
            tv_ani["OpenTv"].speed = 1;
            starMove.StopMove();
        }

        // 앱 서랍 활성화/비활성화
        AppDrawerOnOff();
        tv_ani.Play();
    }
    private void AppDrawerOnOff()
    {
        // TV가 확대되고 있으면 앱 활성화
        if(isTvExpanded)
        {
            OpenAni(appDrawer_ani, "AppDrawer", appDrawer);
        }
        // TV가 축소되고 있으면 앱 비활성화
        else
        {
            CloseAni(appDrawer_ani, "AppDrawer", appDrawer);
        }
    }

    /// <summary>
    /// 주기적으로 시간 정보 업데이트 해주는 코루틴
    /// </summary>
    /// <param name="tick">시간 사이의 콜론 색상</param>
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
    /// 로켓 샘플 이미지 업데이트
    /// </summary>
    /// <param name="rocketType">표시할 로켓의 인덱스</param>
    public void UpdateRocketSample(int rocketType)
    {
        // 인덱스가 -1이면 로켓 이미지 비활성화
        if (rocketSampleIndex != -1)
            rocketSamples[rocketSampleIndex].SetActive(false);

        rocketSampleIndex = rocketType;
        rocketSamples[rocketSampleIndex].SetActive(true);
    }
}
