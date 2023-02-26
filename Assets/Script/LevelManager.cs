using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어 레벨 시스템 관리 클래스
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// 레벨 데이터 
    /// </summary>
    private LevelData levelData;
    /// <summary>
    /// 레벨 업 경험치 요구량
    /// </summary>
    public int expReq { get; private set; }
    /// <summary>
    /// 최대 레벨
    /// </summary>
    private int maxLevel;

    /// <summary>
    /// 직책 표기 텍스트
    /// </summary>
    public Text position_text;
    /// <summary>
    /// 레벨 표기 텍스트
    /// </summary>
    public Text level_text;
    /// <summary>
    /// 현재 경험치 보유량 텍스트
    /// </summary>
    public Text curExp_text;
    /// <summary>
    /// 현재 경험치 보유량 슬라이더(게이지)
    /// </summary>
    public Slider exp_slider;
    /// <summary>
    /// 경험치 증가 효과 대기 큐
    /// </summary>
    private Queue<IEnumerator> expSliderQueue;

    /// <summary>
    /// 게이지가 차오르는 속도
    /// </summary>
    public float gageSpeed;
    /// <summary>
    /// 직책 레벨 기준치
    /// </summary>
    public int[] positionCriteria;

    public StatManager statManager;
    public DataManager dataManager;

    private void Awake()
    {
        // 경험치 증가 효과 대기열 초기화
        expSliderQueue = new Queue<IEnumerator>();
        // 레벨, 경험치 관련 UI 업데이트
        SetLevelText();
        SetExpReq();
        UpdateExpSlider();
    }

    /// <summary>
    /// 레벨 업 경험치 요구량 업데이트
    /// </summary>
    private void SetExpReq()
    {
        // 최대 레벨인 경우, 경험치 요구량 0으로 설정
        if (levelData.level >= GetMaxLevel())
            expReq = 0;
        else
        {
            expReq = 1000;// TODO: 수식 작성
            exp_slider.maxValue = expReq;
        }
    }

    /// <summary>
    /// 경험치 지급
    /// </summary>
    /// <param name="add">지급할 경험치 양</param>
    public void AddExp(int add)
    {
        // 최대 레벨 이하인 경우에만 경험치 지급
        if (levelData.level < GetMaxLevel())
        {
            // 경험치 증가 효과 대기열 등록 및 경험치 지급
            IEnumerator expGageEffect = ExpGageEffect(levelData.currentExp, levelData.currentExp + add);
            expSliderQueue.Enqueue(expGageEffect);
            levelData.currentExp += add;
            // 레벨업 조건 검사
            if (levelData.currentExp >= expReq)
            {
                LevelUp();
            }
            else
            {
                dataManager.SaveData();
            }
            SetLevelText();
        }
    }

    /// <summary>
    /// 레벨업 처리
    /// </summary>
    private void LevelUp()
    {
        // 레벨 증가
        levelData.level++;
        // 최대 레벨 도달 여부 검사
        if (levelData.level <= GetMaxLevel())
        {
            // 스탯 포인트 지급 및 레벨 업 요구 경험치량 업데이트
            levelData.totalStatPoint++;
            levelData.unusedStatPoint++;
            //IEnumerator expGageEffect = ExpGageEffect(expReq, levelData.currentExp - expReq);
            levelData.currentExp -= expReq;
            if (levelData.level.Equals(GetMaxLevel()))
            {
                levelData.currentExp = 0;
            }
            else
                SetExpReq();
            // 경험치 증감 이펙트 대기열 등록 후 레벨 업 조건 재 검사
            IEnumerator expGageEffect = ExpGageEffect(0, levelData.currentExp);
            expSliderQueue.Enqueue(expGageEffect);
            if (levelData.currentExp >= expReq)
                LevelUp();
            else
                dataManager.SaveData();
        }
        else
        {
            levelData.level = GetMaxLevel();
            levelData.currentExp = 0;
        }
    }

    /// <summary>
    /// (테스트 용) 다량의 레벨 추가
    /// </summary>
    /// <param name="add">레벨 증가량</param>
    public void AddLevel(int add)
    {
        levelData.level += add;
        if (levelData.level > GetMaxLevel())
            levelData.level = GetMaxLevel();
        else
        {
            SetExpReq();
            levelData.totalStatPoint += add;
            levelData.unusedStatPoint += add;
        }
    }

    /// <summary>
    /// 레벨 정보 UI 업데이트
    /// </summary>
    public void SetLevelText()
    {
        if (level_text != null && LocalizationManager.instance != null)
        {
            // 스탯 UI 업데이트
            statManager.SetUnusedStatText();
            // 레벨 텍스트 업데이트
            SetLevelText(level_text, curExp_text);
            // 직책 정보 업데이트
            position_text.text = LocalizationManager.instance.GetLocalizedValue("position_" + GetPositionIndex());
        }
    }

    /// <summary>
    /// 레벨 텍스트 업데이트
    /// </summary>
    /// <param name="level_text">업데이트 대상 레벨 텍스트</param>
    /// <param name="curExp_text">경험치 보유량 텍스트</param>
    public void SetLevelText(Text level_text, Text curExp_text)
    {
        // 레벨 업 경험치 요구량 업데이트
        SetExpReq();
        // 최대 레벨일 때의 텍스트 업데이트 처리
        if (levelData.level.Equals(GetMaxLevel()))
        {
            level_text.text = "Level: " + GetMaxLevel() + " (MAX)";
            curExp_text.text = "0 / 0 (0%)";
        }
        // 최대 레벨 미만일 때의 텍스트 업데이트 처리
        else
        {
            level_text.text = "Level: " + levelData.level;
            curExp_text.text = PlayManager.SetCommaOnDigit(GetCurrentExp()) + " / " + PlayManager.SetCommaOnDigit(expReq) + " (" + PlayManager.SetCommaOnDigit((float)GetCurrentExp() / expReq * 100) + "%)";
        }
    }

    /// <summary>
    /// 플레이어에 레벨에 따른 직책 인덱스를 구한다.
    /// </summary>
    /// <returns>플레이어 직책 인덱스</returns>
    public int GetPositionIndex()
    {
        for (int i = 0; i < positionCriteria.Length; i++)
        {
            if (levelData.level < positionCriteria[i])
                return i;
        }
        return positionCriteria.Length;
    }

    /// <summary>
    /// 경험치 슬라이더 업데이트
    /// </summary>
    private void UpdateExpSlider()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(0))
            exp_slider.value = levelData.currentExp;
    }

    /// <summary>
    /// 경험치 증감 이펙트 시작
    /// </summary>
    public void StartGageEffect()
    {
        StartCoroutine(expSliderQueue.Dequeue());
    }

    /// <summary>
    /// 경험치 증감 이펙트
    /// </summary>
    /// <param name="from">시작 수치</param>
    /// <param name="to">종료 수치</param>
    IEnumerator ExpGageEffect(int from, int to)
    {
        yield return new WaitForEndOfFrame();

        float currentPercentage;
        float deltaSpeed;

        // 증가인 경우
        if (from < to)
        {
            // 현재 퍼센티지 계산
            currentPercentage = (exp_slider.value - from) / (to - from) * 100f;

            // 증가량의 90% 미만인 경우 일반 속도로 증가
            if (currentPercentage < 90)
                deltaSpeed = (to - from) * gageSpeed * Time.deltaTime;
            // 증가량이 90%를 넘은 경우 50%의 속도로 증가
            else
                deltaSpeed = (to - from) / 2f * gageSpeed * Time.deltaTime;

            // 슬라이더 값 증가
            exp_slider.value += deltaSpeed;
            if (exp_slider.value > to)
                exp_slider.value = to;

            // 증가 반복
            if (exp_slider.value != to && exp_slider.value < exp_slider.maxValue)
                StartCoroutine(ExpGageEffect(from, to));
            // 증가가 끝난 후에 다음 대기열의 증감 효과 시작
            else if (expSliderQueue.Count > 0)
            {
                exp_slider.value = 0f;
                StartCoroutine(expSliderQueue.Dequeue());
            }
        }
        // 감소인 경우
        else
        {
            // 현재 퍼센티지 계산
            currentPercentage = exp_slider.value / (from + to) * 100f;

            // 감소량의 10% 이상인 경우 일반 속도로 감소
            if (currentPercentage > 10)
                deltaSpeed = (from - to) * Time.deltaTime;
            // 감소량이 10% 미만으로 남은 경우 50%의 속도로 감소
            else
                deltaSpeed = (from - to) / 2f * Time.deltaTime;

            // 슬라이더 값 감소
            exp_slider.value -= deltaSpeed;
            if (exp_slider.value < to)
                exp_slider.value = to;

            // 감소 반복
            if (exp_slider.value != to && exp_slider.value > exp_slider.minValue)
                StartCoroutine(ExpGageEffect(from, to));
            // 감소가 끝난 후에 다음 대기열의 증감 효과 시작
            else if (expSliderQueue.Count > 0)
            {
                exp_slider.value = 0f;
                StartCoroutine(expSliderQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// 플레이어의 레벨을 얻는다.
    /// </summary>
    /// <returns>플레이어 레벨</returns>
    public int GetLevel()
    {
        return levelData.level;
    }
    /// <summary>
    /// 현재 보유중인 경험치를 얻는다.
    /// </summary>
    /// <returns>보유중인 경험치</returns>
    public int GetCurrentExp()
    {
        return levelData.currentExp;
    }
    /// <summary>
    /// 사용하지 않은 스탯 포인트를 얻는다.
    /// </summary>
    /// <returns>사용하지 않은 스탯 포인트</returns>
    public int GetUnusedStatPoint()
    {
        return levelData.unusedStatPoint;
    }
    /// <summary>
    /// 스탯 포인트 사용 처리
    /// </summary>
    /// <returns>처리 결과</returns>
    public bool UseStatPoint()
    {
        if(levelData.unusedStatPoint > 0)
        {
            levelData.unusedStatPoint--;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 최대레벨을 계산하여 반환한다.
    /// </summary>
    /// <returns>최대 레벨</returns>
    public int GetMaxLevel()
    {
        // maxLevel 값이 계산이 안되어있는 경우에만 계산하여 리턴
        if (maxLevel.Equals(0))
        {
            int _maxLevel = 1;
            for (int i = 0; i < statManager.stats.Length; i++)
                _maxLevel += statManager.stats[i].statAbilityData.MaxLevel;
            maxLevel = _maxLevel;
        }

        return maxLevel;
    }

    /// <summary>
    /// 레벨 데이터 저장
    /// </summary>
    /// <param name="data">저장될 데이터</param>
    public void SaveData(ref Data data)
    {
        data.levelData = levelData;
    }
    /// <summary>
    /// 레벨 데이터 불러오기
    /// </summary>
    /// <param name="data">불러온 데이터</param>
    public void LoadData(Data data)
    {
        if (data.levelData != null)
        {
            levelData = data.levelData;
        }
        else 
        {
            levelData = new LevelData();
        }
        SetLevelText();
    }
}
/// <summary>
/// 플레이어의 레벨 데이터
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>
    /// 플레이어 레벨
    /// </summary>
    public int level = 1;
    /// <summary>
    /// 플레이어가 보유 중인 경험치량
    /// </summary>
    public int currentExp;
    /// <summary>
    /// 누적 스탯 포인트
    /// </summary>
    public int totalStatPoint;
    /// <summary>
    /// 사용하지 않은 스탯 포인트
    /// </summary>
    public int unusedStatPoint;
}
