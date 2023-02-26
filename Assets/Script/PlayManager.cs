using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System;

/// <summary>
/// 게임 씬 분류
/// </summary>
public enum GameScene { MainScene, LaunchScene }

/// <summary>
/// 게임 플레이 관리 클래스
/// </summary>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// 게임 플레이 관리 클래스의 싱글톤 오브젝트
    /// </summary>
    public static PlayManager instance;

    /// <summary>
    /// 현재 플레이어 로켓이 생존 중인지 여부
    /// </summary>
    public static bool isSurvive = true;
    /// <summary>
    /// 현재 게임에서 획득하고 있는 점수
    /// </summary>
    public int score { get; private set; }
    /// <summary>
    /// '점수' 단어의 로컬라이징된 텍스트
    /// </summary>
    public string score_local;
    /// <summary>
    /// 점수 지급량 (이동 거리에 따라 계산)
    /// </summary>
    public int deltaScore;
    /// <summary>
    /// 점수 표기 텍스트
    /// </summary>
    public Text score_text;
    /// <summary>
    /// 최고 점수 표기 텍스트
    /// </summary>
    public Text bestScore_text;
    /// <summary>
    /// 플레이 데이터
    /// </summary>
    private PlayData playData = new PlayData();
    /// <summary>
    /// 최고 점수 달성 시의 폰트 크기
    /// </summary>
    public int fontSizeOnBestScore = 35;
    /// <summary>
    /// 최고 점수 달성 시의 텍스트 색상
    /// </summary>
    public Color colorOnBestScore;

    /// <summary>
    /// 점수 계산을 위한 직전 로켓 위치
    /// </summary>
    private Vector3 priorPos; 
    /// <summary>
    /// 점수 대비 코인 지급 비율
    /// </summary>
    public float coinPortion;
    /// <summary>
    /// 점수 대비 경험치 지급 비율
    /// </summary>
    public float expPortion;

    /// <summary>
    /// 로켓 높이
    /// </summary>
    private float rocketHeight;

    /// <summary>
    /// (게임오버 메뉴) 최종 점수 텍스트
    /// </summary>
    public Text gOver_score_text;
    /// <summary>
    /// (게임오버 메뉴) 코인 획득량 텍스트
    /// </summary>
    public Text gOver_coin_text;
    /// <summary>
    /// (게임오버 메뉴) 레벨 텍스트
    /// </summary>
    public Text gOver_level_text;
    /// <summary>
    /// (게임오버 메뉴) 직책 텍스트
    /// </summary>
    public Text gOver_position_text;
    /// <summary>
    /// (게임오버 메뉴) 획득한 경험치 텍스트
    /// </summary>
    public Text gOver_addExp_text;
    /// <summary>
    /// (게임오버 메뉴) 경험치 슬라이더
    /// </summary>
    public Slider gOver_exp_slider;
    /// <summary>
    /// (게임오버 메뉴) 현재 경험치 보유량 텍스트
    /// </summary>
    public Text gOver_curExp_text;

    public DataManager dataManager;
    public Launch launch;
    public LevelManager levelManager;
    public TabletManager tabletManager;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// 폭발 효과음
    /// </summary>
    public AudioSource explosionAudio;

    /// <summary>
    /// 보너스 애니메이션 효과
    /// </summary>
    public Animation bonusAnimation;
    /// <summary>
    /// 보너스 알림 텍스트
    /// </summary>
    public Text bonus_text;

    /// <summary>
    /// 애니메이션 종료 후 호출될 함수의 델리게이트
    /// </summary>
    public delegate void AfterAnimationPlay();
    /// <summary>
    /// 애니메이션 종료 후 호출될 함수의 델리게이트 (게임 오브젝트 매개변수 포함)
    /// </summary>
    /// <param name="obj">처리할 게임 오브젝트</param>
    public delegate void AfterAnimationPlay_obj(GameObject obj);
    /// <summary>
    /// 애니메이션 종료 후 호출될 함수의 델리게이트 (캔버스 매개변수 포함)
    /// </summary>
    /// <param name="cvs">처리할 캔버스</param>
    public delegate void AfterAnimationPlay_cvs(Canvas cvs);

    private void Awake()
    {
        instance = this;
        isSurvive = true;
    }

    private void Start()
    {
        // 발사 씬인 경우에 로켓 높이를 계산하여 게임 플레이 조건 검사 및 점수 획득 코루틴 시작
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            rocketHeight = RocketSet.instance.rocket.GetComponent<RectTransform>().rect.height * RocketSet.instance.rocket.transform.localScale.y * 0.5f;
            StartCoroutine(OnPlaying());
            StartCoroutine(Scoring());
        }
        // 입장 씬인 경우 최고 점수 업데이트
        else
            SetBestScoreText();
    }

    /// <summary>
    /// 게임 오버 여부 감지 코루틴
    /// </summary>
    IEnumerator OnPlaying()
    {
        yield return new WaitForSeconds(0.1f);

        if(!isSurvive)
        {
            GameOver();
        }
        else
        {
            // 특정 높이 이하로 떨어지면 게임 오버 처리
            Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(RocketSet.instance.rocket.transform.position + new Vector3(0, rocketHeight, 0));
            if (targetScreenPos.y < 0)
                isSurvive = false;

            StartCoroutine(OnPlaying());
        }
    }

    /// <summary>
    /// 주기적으로 이동 거리를 계산하여 점수를 추가하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Scoring()
    {
        yield return new WaitForSeconds(0.1f);

        // 생존 중에만 계산
        if (isSurvive)
        {
            // 발사가 시작되고 연료를 소진하지 않은 경우에만 계산
            if (launch.isStart && RocketSet.instance.FuelRemain > 0)
            {
                // 일시정지 상태가 아닌 경우에
                if (!TabletManager.isPause)
                {
                    // 가속하고 있는 상태에
                    if (launch.isAccerlate)
                    {
                        // 현재 로켓의 위치와 직전 위치의 거리를 계산하여 deltaScore 계산 후 점수 지급
                        Vector3 rocketPos = RocketSet.instance.rocket.transform.position;
                        deltaScore = (int)(Vector3.Distance(priorPos, rocketPos) * 4);
                        priorPos = rocketPos;
                        AddScore(deltaScore);
                    }
                }
            }
            else
                priorPos = RocketSet.instance.rocket.transform.position;
            StartCoroutine(Scoring());
        }
    }

    /// <summary>
    /// 점수 지급량 만큼 점수 추가
    /// </summary>
    /// <param name="add">점수 지급량</param>
    public void AddScore(int add)
    {
        score += add;
        ReloadScore();
    }
    /// <summary>
    /// 점수 텍스트 정보 업데이트
    /// </summary>
    public void ReloadScore()
    {
        // 0,000 형식을 유지하며 텍스트 정보 업데이트
        score_text.text = score_local + ": " + SetCommaOnDigit(score);
        // 최고 점수를 돌파했을 때 폰트 크기 및 색상 변경
        if(score > playData.bestScore && score_text.fontSize != fontSizeOnBestScore)
        {
            score_text.fontSize = fontSizeOnBestScore;
            score_text.color = colorOnBestScore;
        }
    }

    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    private void GameOver()
    {
        // 가속 취소 처리
        launch.AcceleratorUp();
        // 최종 점수 표기 및 경험치/코인 지급
        gOver_score_text.text = score_local + ": " + SetCommaOnDigit(score);
        int addCoin = (int)(score * coinPortion);
        int addExp = (int)(score * expPortion);
        levelManager.AddExp(addExp);
        EconomicMgr.instance.AddCoin(addCoin);
        // 최고점수 갱신 및 정보 업데이트
        CheckBestScore(score); // TODO: 베스트스코어 갱신시 갱신 축하 효과
        bestScore_text.text = LocalizationManager.instance.GetLocalizedValue("bestscore") + ": " + SetCommaOnDigit(playData.bestScore);
        gOver_coin_text.text = "+" + SetCommaOnDigit(addCoin + EconomicMgr.instance.bonusCoins);
        levelManager.SetLevelText(gOver_level_text, gOver_curExp_text);
        gOver_addExp_text.text = "Exp. +" + SetCommaOnDigit(addExp);
        // 직책 정보 업데이트
        gOver_position_text.text = LocalizationManager.instance.GetLocalizedValue("position_" + levelManager.GetPositionIndex());
        // GameOver 앱 활성화
        tabletManager.OpenApp(TabletAppList.GameOver);
        // 태블릿이 활성화 된 이후에 경험치 증감 효과 시작
        WaitAnimation(tabletManager.tablet_ani, levelManager.StartGageEffect);
        // 일시정지가 불가능하도록 설정
        tabletManager.pause_button.SetActive(false);
        // 추락중인 로켓의 상태 고정
        RocketSet.instance.rocketMain_rigid.velocity = Vector3.zero;
        if (RocketSet.instance.rocket.transform.position.y > launch.heightCriteria[0])
            RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;

        dataManager.SaveData();
    }

    /// <summary>
    /// 게임오버 버튼 리스너
    /// </summary>
    /// <param name="key">실행하고자 하는 기능</param>
    public void OnGameOverButton(int key)
    {
        switch (key)
        {
            case 0: // Game Reset
                ResetGame();
                break;
            case 1: // Go to Menu
                ResetGame();
                SceneManager.LoadScene((int)GameScene.MainScene);
                break;
            case 2: // Open Store
                Debug.Log("Store is not developed");
                break;
            case 3: // Open Equipment
                Debug.Log("Equipment is not developed");
                break;
            case 4: // Open Stat
                Debug.Log("Stat is not developed");
                break;
        }

    }

    /// <summary>
    /// 폭발 효과음 재생
    /// </summary>
    public void PlayExplosionSound()
    {
        explosionAudio.Play();
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    public void ResetGame()
    {
        // 게임 데이터 저장 이후 씬 리로드
        dataManager.SaveData();
        isSurvive = true;
        if (TabletManager.isPause)
            TabletManager.isPause = false;
        SceneManager.LoadScene((int)GameScene.LaunchScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// 최고 점수 갱신
    /// </summary>
    /// <param name="score">갱신할 점수</param>
    /// <returns>갱신 성공 여부</returns>
    private bool CheckBestScore(int score)
    {
        if (score > playData.bestScore)
        {
            playData.bestScore =  score;
            return true;
        }
        else
            return false;

    }

    /// <summary>
    /// 최고 점수 텍스트 정보 업데이트
    /// </summary>
    public void SetBestScoreText()
    {
        bestScore_text.text = "BestScore: " + SetCommaOnDigit(playData.bestScore);
    }

    /// <summary>
    /// 플레이 데이터 저장
    /// </summary>
    /// <param name="data">저장할 데이터</param>
    public void SaveData(ref Data data)
    {
        data.playData = playData;
    }
    /// <summary>
    /// 플레이 데이터 불러오기
    /// </summary>
    /// <param name="data">불러온 데이터</param>
    public void LoadData(Data data)
    {
        if (data.playData != null)
        { 
            playData = data.playData;
        }
        else
        {
            playData = new PlayData();
        }
    }

    /// <summary>
    /// 숫자를 #,### 양식의 문자열로 변환
    /// </summary>
    /// <param name="num">변환할 숫자</param>
    /// <returns>#,### 형태의 문자열</returns>
    public static string SetCommaOnDigit(int num)
    {
        StringBuilder result = new StringBuilder();
        string n = num.ToString();
        int len = n.Length;
        int front = len % 3;

        if (front != 0)
        {
            result.Append(n, 0, front);
            if (front < len)
                result.Append(',');
        }

        for (int i = front; i < len; i += 3)
        {
            result.Append(n, i, 3);
            if (i + 3 < len)
                result.Append(',');
        }
        return result.ToString();
    }
    /// <summary>
    /// 부동 소수점을 #,### 양식의 문자열로 변환
    /// </summary>
    /// <param name="num">변환할 부동 소수점</param>
    /// <returns>#,### 형태의 문자열</returns>
    public static string SetCommaOnDigit(float num)
    {
        if (num.Equals(0))
            return "0";

        return string.Format("{0:#,##0.00}", num);
    }

    /// <summary>
    /// 보너스 알림 정보 업데이트
    /// </summary>
    /// <param name="content">보너스 정보</param>
    /// <param name="textColor">텍스트 색상</param>
    public void SetBonusText(string content, Color textColor)
    {
        // 보너스 효과 재상 및 내용 설정
        bonusAnimation.gameObject.SetActive(true);
        bonusAnimation.clip = bonusAnimation["BonusOn"].clip;
        bonusAnimation.Play();
        bonus_text.text = content;
        bonus_text.color = textColor;

        // 대기 중인 보너스 텍스트 비활성화 코루틴 중지
        if (waitBonusText != null)
            StopCoroutine(waitBonusText);
        // 보너스 텍스트 비활성화 코루틴 시작
        waitBonusText = WaitBonusText(1.5f);
        StartCoroutine(waitBonusText);
    }

    /// <summary>
    /// 보너스 텍스트 비활성화 코루틴
    /// </summary>
    IEnumerator waitBonusText;
    /// <summary>
    /// 보너스 텍스트 비활성화 코루틴
    /// </summary>
    /// <param name="time">대기 시간</param>
    IEnumerator WaitBonusText(float time)
    {
        yield return new WaitForSeconds(time);

        bonusAnimation.clip = bonusAnimation["BonusOff"].clip;
        bonusAnimation.Play();
        StartCoroutine(WaitBonusOff());
    }

    /// <summary>
    /// 보너스 텍스트 비활성화 효과가 종료된 후 비활성화 대기 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitBonusOff()
    {
        while (bonusAnimation.isPlaying)
            yield return new WaitForEndOfFrame();

        bonusAnimation.gameObject.SetActive(false);
    }

    /// <summary>
    /// 애니메이션 종료 후 함수 콜백
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation));
    }
    /// <summary>
    /// 애니메이션 종료 후 함수 콜백
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param
    /// <param name="obj">게임 오브젝트 파라미터</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay_obj afterAnimation, GameObject obj)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation, obj));
    }
    /// <summary>
    /// 애니메이션 종료 후 함수 콜백
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param
    /// <param name="cvs">캔버스 파라미터</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay_cvs afterAnimation, Canvas cvs)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation, cvs));
    }
    /// <summary>
    /// 애니메이션 종료 후 함수 콜백 및 해당 코루틴 반환
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param>
    /// <param name="obj">게임 오브젝트 파라미터</param>
    public IEnumerator RWaitAnimation(Animation animation, AfterAnimationPlay_obj afterAnimation, GameObject obj)
    {
        IEnumerator coroutine = WaitforAnimation(animation, afterAnimation, obj);
        return coroutine;
    }

    /// <summary>
    /// 애니메이션 종료 후 함수 콜백 코루틴
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        while(animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation();
    }
    /// <summary>
    /// 애니메이션 종료 후 함수 콜백 코루틴
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param
    /// <param name="obj">게임 오브젝트 파라미터</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay_obj afterAnimation, GameObject obj)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(obj);
    }
    /// <summary>
    /// 애니메이션 종료 후 함수 콜백 코루틴
    /// </summary>
    /// <param name="animation">대상 애니메이션</param>
    /// <param name="afterAnimation">콜백할 함수</param
    /// <param name="cvs">캔버스 파라미터</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay_cvs afterAnimation, Canvas cvs)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(cvs);
    }
}

/// <summary>
/// 플레이 데이터
/// </summary>
[Serializable]
public class PlayData
{
    /// <summary>
    /// 최고 점수
    /// </summary>
    public int bestScore;
}
