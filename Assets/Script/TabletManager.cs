using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// 태블릿 앱 종류 열거형
/// </summary>
public enum TabletAppList { GameOver = 0, Settings = 1, Equipment = 2, Store = 3, Stat = 4 }

/// <summary>
/// 태블릿 시스템 관리 클래스
/// </summary>
public class TabletManager : MonoBehaviour
{
    /// <summary>
    /// 태블릿 전용 캔버스
    /// </summary>
    public Canvas tablet;
    /// <summary>
    /// 태블릿 on/off 애니메이션 효과
    /// </summary>
    public Animation tablet_ani;

    /// <summary>
    /// 태블릿 앱 그룹 오브젝트 배열
    /// </summary>
    public GameObject[] appArray;
    /// <summary>
    /// 태블릿 앱 그룹 RectTransform 배열
    /// </summary>
    public RectTransform[] appRectTransforms;
    /// <summary>
    /// 태블릿 앱 on/off 애니메이션 효과 배열
    /// </summary>
    public Animation[] appAniArray;
    /// <summary>
    /// 멀티태스킹 상의 앱 선택 버튼 배열
    /// </summary>
    public GameObject[] appMultiButtons;
    /// <summary>
    /// 앱이 밝은 배경인지 여부
    /// </summary>
    public bool[] hasBrightBackgound;
    /// <summary>
    /// 밝은 배경 색상에서의 네비게이션바 색상
    /// </summary>
    public Color colorOnBright;

    /// <summary>
    /// 네비게이션 버튼 이미지
    /// </summary>
    public Image[] underButtonImgs;

    /// <summary>
    /// 태블릿 시계 텍스트
    /// </summary>
    public Text time_text;
    
    /// <summary>
    /// 현재 화면이 홈 화면인지 여부
    /// </summary>
    private bool isHome = true;
    /// <summary>
    /// 최근에 열었던 앱 (Home/back 버튼에 사용되기 위함)
    /// </summary>
    private TabletAppList recentApp;
    /// <summary>
    /// 최근에 열었던 앱 리스트 (멀티태스킹 리스트)
    /// </summary>
    private List<TabletAppList> openedApps = new List<TabletAppList>();

    /// <summary>
    /// 앱의 hierarchy 상의 부모 Transform
    /// </summary>
    public Transform appParent;
    /// <summary>
    /// 앱의 기본 위치
    /// </summary>
    private Vector3 appBasicPos;
    /// <summary>
    /// 컨탠츠 기준 점
    /// </summary>
    private Vector3 contentStdPos;
    /// <summary>
    /// 멀티 태스킹 화면의 ScrollView 오브젝트
    /// </summary>
    public GameObject multiView;
    /// <summary>
    /// 멀티 태스킹 화면의 ScrollView-Content 오브젝트
    /// </summary>
    public GameObject multiContents;
    /// <summary>
    /// 멀티 태스킹 화면의 ScrollView-Content RectTransform
    /// </summary>
    private RectTransform multiContent_rect;
    /// <summary>
    /// 최근에 열었던 앱이 없었다는 메시지를 보여주는 텍스트
    /// </summary>
    public Text noApps_text;
    /// <summary>
    /// 멀티 태스킹 리스트업 애니메이션 효과
    /// </summary>
    public Animation multi_ani;

    /// <summary>
    /// 일시정지때 활성화 시킬 버튼 그룹 오브젝트
    /// </summary>
    public GameObject buttonsforPause;
    /// <summary>
    /// 재개 메시지 오브젝트
    /// </summary>
    public GameObject resumeMessage;
    /// <summary>
    /// 일시정지 버튼 오브젝트
    /// </summary>
    public GameObject pause_button;
    /// <summary>
    /// 게임 일시정지 여부
    /// </summary>
    public static bool isPause = false;
    /// <summary>
    /// 일시정지 직전의 로켓 RigidBody 타입
    /// </summary>
    private RigidbodyType2D priorType;
    /// <summary>
    /// 뒤로가기 버튼 클릭을 막는 RectTransform
    /// </summary>
    public RectTransform backBlock_RT;

    /// <summary>
    /// 발사 직전 효과음 오디오
    /// </summary>
    public AudioSource beforeLaunchAudio;
    /// <summary>
    /// 로켓 화염 효과음
    /// </summary>
    public AudioSource rocketFireAudio;
    /// <summary>
    /// 씬 리로드 필요 여부
    /// </summary>
    public static bool needReload = false;

    public DataManager dataManager;
    public AndroidController androidController;
    public StoreManager storeManager;
    public Settings settings;

    private void Start()
    {
        // 앱 기본 위치 초기화
        appBasicPos = appArray[1].transform.localPosition;
        multiContent_rect = multiContents.GetComponent<RectTransform>();
        backBlock_RT.sizeDelta = new Vector2((float)Screen.width / Screen.height * 1080, 1080);
    }

    /// <summary>
    /// 선택한 앱 활성화
    /// </summary>
    /// <param name="app">선택한 앱</param>
    public void OpenApp(TabletAppList app)
    {
        // 태블릿이 비활성화 되어있으면 활성화 작업
        if(!tablet.enabled)
            OnOffTablet(true);
        // 앱 활성화
        appArray[(int)app].SetActive(true);
        // 게임오버 화면이 아니면 애니메이션 효과 시작
        if (app != TabletAppList.GameOver)
        {
            OpenAni(appAniArray[(int)app - 1], "AppOpening", appArray[(int)app]);
            AddToList(app);
        }
        // 홈 화면이 아니라는 표시 및 최근 앱 등록
        isHome = false;
        recentApp = app;
        // 네비게이션 바 색상 조정
        if(hasBrightBackgound[(int)app])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;
        // 시계 정보 업데이트 코루틴 시작
        StartCoroutine(TimeSet());
    }
    /// <summary>
    /// 선택한 앱 활성화
    /// </summary>
    /// <param name="appSelect">선택한 앱(Inspector)</param>
    public void OpenApp(AppSelect appSelect)
    {
        OpenApp(appSelect.appList);
    }
    /// <summary>
    /// 최근에 사용한 앱 리스트에 추가
    /// </summary>
    /// <param name="app">추가할 앱</param>
    private void AddToList(TabletAppList app)
    {
        bool added = false;
        // 이미 리스트에 있는 앱인지 확인
        for(int i = 0; i < openedApps.Count - 1; i++)
        {
            if(app.Equals(openedApps[i]))
            {
                // 가장 맨 앞으로 스왑
                SwapOpenedApps(openedApps.Count - 1, i);
                added = true;
                break;
            }
        }
        // 리스트가 비어있으면 추가
        if(openedApps.Count.Equals(0))
            openedApps.Add(app);
        // 리스트에 없는 앱이면 추가
        else if (!app.Equals(openedApps[openedApps.Count - 1]) && !added)
            openedApps.Add(app);
    }

    /// <summary>
    /// 최근에 사용한 앱 리스트 스왑
    /// </summary>
    /// <param name="index1">인덱스 1</param>
    /// <param name="index2">인덱스 2</param>
    private void SwapOpenedApps(int index1, int index2)
    {
        TabletAppList temp = openedApps[index1];
        openedApps[index1] = openedApps[index2];
        openedApps[index2] = temp;
    }

    /// <summary>
    /// 현재 켜져있는 앱 종료
    /// </summary>
    public void CloseApp()
    {
        // 앱 종료 애니메이션 시작
        CloseAni(appAniArray[(int)recentApp - 1], "AppOpening", appArray[(int)recentApp]);
        // 특정 앱 종료 보조 처리
        CloseApp(recentApp);
        // 최근 앱 리스트에서 제거
        openedApps.Remove(recentApp);
        if (openedApps.Count > 1)
            recentApp = openedApps[openedApps.Count - 1];
        // 홈 화면 여부 갱신
        isHome = true;
        // 네비게이션바 색상 업데이트
        if(underButtonImgs[0].color != Color.white)
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = Color.white;

        dataManager.SaveData();
    }

    /// <summary>
    /// 특정 앱 종료 보조 작업
    /// </summary>
    /// <param name="tabletApp">종료할 앱</param>
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
    /// 태블릿 on/off 애니메이션 재생
    /// </summary>
    /// <param name="on">활성화 여부</param>
    public void OnOffTablet(bool on)
    {
        if (on)
        {
            OpenAni(tablet_ani, "OpenTablet", tablet);
            // 발사 직전 효과음 볼륨 감소
            beforeLaunchAudio.volume = settings.GetEffectVolume() * 0.2f;
        }
        else
        {
            CloseAni(tablet_ani, "OpenTablet", tablet);
            // 발사 직전 효과음 볼륨 복원
            beforeLaunchAudio.volume = settings.GetEffectVolume();
            // 씬 리로드 필요시, 리로드
            if (needReload)
            {
                needReload = false;
                PlayManager.instance.WaitAnimation(tablet_ani, Reload);
            }
        }
    }

    /// <summary>
    /// 씬 리로드
    /// </summary>
    private void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 캔버스 비활성화 처리
    /// </summary>
    /// <param name="cvs">비활성화 할 캔버스</param>
    private void AfterCloseCanvas(Canvas cvs)
    {
        cvs.enabled = false;
    }

    /// <summary>
    /// 게임 오브젝트 비활성화 처리
    /// </summary>
    /// <param name="obj">비활성화 할 게임 오브젝트</param>
    private void AfterCloseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary>
    /// 네비게이션 바 버튼 클릭 리스너
    /// </summary>
    /// <param name="key">버튼 키</param>
    public void OnHomeScreen(int key)
    {
        // 일시정지 해제 처리
        if (isPause && key != 2)
            Resume();
        // 일반 상황에서의 버튼 이벤트 처리
        switch (key)
        {
            case 0: // 뒤로가기 버튼
                // 멀티태스킹 선택 모드일 때 종료처리
                if (multiView.activeInHierarchy)
                {
                    CancelMulti();
                }
                // 앱 종료 처리
                else if (!isHome && openedApps.Count > 0)
                {
                    CloseApp();
                }
                // 태블릿 종료 처리
                else if (isHome && PlayManager.isSurvive)
                {
                    OnOffTablet(false);
                }
                break;
            case 1: // 홈 버튼
                // 멀티태스킹 모드 종료 처리
                if (multiView.activeInHierarchy)
                {
                    for (int i = 0; i < openedApps.Count; i++)
                    {
                        appArray[i].SetActive(false);
                    }
                    isHome = true;
                    CancelMulti();
                }
                // 태블릿 종료 처리
                else if (isHome)
                {
                    OnOffTablet(false);
                }
                // 일반 앱 종료 처리
                else
                {
                    // 게임 오버가 아닐 때에 앱을 내려두고 최근 연 앱 리스트에 추가
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
            case 2: // 멀티태스킹 모드
                // 일시정지가 아닐 때에만
                if (!isPause)
                {
                    // 게임오버가 아닐 때
                    if (PlayManager.isSurvive)
                    {
                        // 멀티태스킹 모드가 아닐 때 멀티태스킹 활성화
                        if (!multiView.activeInHierarchy)
                            MultitaskingMode();
                        else
                        {
                            // 리스트가 비어있으면 멀티태스킹 모드 종료
                            if (openedApps.Count.Equals(0))
                                CancelMulti();
                            // 그게 아니면 최근에 열었던 앱과 현재 앱 스왑
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
    /// 멀티 태스킹 모드 활성화
    /// </summary>
    private void MultitaskingMode()
    {
        multiView.SetActive(true);
        // 홈버튼인 경우의 애니메이션 효과
        if (isHome)
            OpenAni(multi_ani, "OpenMultitasking_home", multiView);
        // 그외의 경우의 애니메이션 효과
        else
            OpenAni(multi_ani, "OpenMultitasking", multiView, false);

        // 네비게이션 바 버튼 색상 변경
        if(underButtonImgs[0].color != Color.white)
        {
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = Color.white;
        }

        // 최근 앱이 없는 경우에 관련 메시지 표시
        if (openedApps.Count.Equals(0))
        {
            noApps_text.gameObject.SetActive(true);
            noApps_text.text = LocalizationManager.instance.GetLocalizedValue("noappsmsg");
        }
        // 리스트 순서에 따라 위치 지정
        else
        {
            noApps_text.gameObject.SetActive(false);
            contentStdPos = multiContent_rect.transform.localPosition;
            for (int i = openedApps.Count - 1; i >= 0; i--)
            {
                // 멀티태스킹 아래로 앱 그룹 이동
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
    /// 멀티태스킹 모드에서의 앱 위치 구하기
    /// </summary>
    /// <param name="index">리스트 상의 인덱스</param>
    /// <returns>위치</returns>
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
    /// 멀티태스킹에서의 앱 선택
    /// </summary>
    /// <param name="appCode">앱 코드</param>
    public void SelectMulti(int appCode)
    {
        multiView.SetActive(false);
        multiContent_rect.transform.localPosition = contentStdPos;

        // 네비게이션 버튼 색상 변경
        if (hasBrightBackgound[appCode])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;

        // 앱의 hierarchy 및 위치 원상복귀
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
        // 선택된 앱의 활성화 처리
        OpenAni(appAniArray[appCode - 1], "AppOpening", appArray[appCode]);
        isHome = false;
        AddToList((TabletAppList)appCode);
        recentApp = (TabletAppList)appCode;
        appArray[appCode].SetActive(true);
    }

    /// <summary>
    /// 멀티태스킹 모드 취소 처리
    /// </summary>
    public void CancelMulti()
    {
        multiContent_rect.transform.localPosition = contentStdPos;

        // 네비게이션 버튼 색상 변경
        if (hasBrightBackgound[(int)recentApp])
            for (int i = 0; i < 3; i++)
                underButtonImgs[i].color = colorOnBright;

        // 앱의 hierarchy 및 위치 원상복귀
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
        // 조건에 따라 애니메이션 효과 재생
        if (isHome)
            CloseAni(multi_ani, "OpenMultitasking_home", multiView);
        else
        {
            multiView.SetActive(false);
            OpenAni(appAniArray[(int)openedApps[openedApps.Count - 1] - 1], "AppOpening", appArray[(int)openedApps[openedApps.Count - 1]]);
        }
    }

    /// <summary>
    /// 시계 정보 업데이트 코루틴
    /// </summary>
    IEnumerator TimeSet()
    {
        time_text.text = DateTime.Now.ToString("hh:mm");

        yield return new WaitForSeconds(10f);

        if (tablet.enabled)
            StartCoroutine(TimeSet());
    }
    
    /// <summary>
    /// 일시정지/재개 처리
    /// </summary>
    public void Pause_button()
    {
        if (isPause)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// 일시정지 처리
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
    /// 게임 재개 처리
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
    /// 시작 화면으로 돌아가기
    /// </summary>
    public void GoToEntryScene()
    {
        isPause = false;
        dataManager.SaveData();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// on/off 애니메이션에서 활성화를 진행한다.
    /// </summary>
    /// <param name="ani">활성화할 애니메이션</param>
    /// <param name="aniName">애니메이션 클립 이름</param>
    /// <param name="cvs">활성화 대상 캔버스</param>
    /// <param name="aniIsMain">해당 클립이 메인인지 여부</param>
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
    /// on/off 애니메이션에서 비활성화를(역재생) 진행한다.
    /// </summary>
    /// <param name="ani">활성화할 애니메이션</param>
    /// <param name="aniName">애니메이션 클립 이름</param>
    /// <param name="cvs">활성화 대상 캔버스</param>
    /// <param name="aniIsMain">해당 클립이 메인인지 여부</param>
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
            PlayManager.instance.WaitAnimation(ani, AfterCloseObject, obj);
        }
        else
        {
            ani.Play(aniName);

            PlayManager.instance.WaitAnimation(ani, AfterCloseObject, obj);
        }
    }
}
