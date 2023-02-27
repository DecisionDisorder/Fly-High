using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 스탯 종류 열거형
/// </summary>
public enum StatType { Accelerate, Efficiency, StarSearch, CoinCollect, Reload, Booster }

/// <summary>
/// 스탯 시스템 관리 클래스
/// </summary>
public class StatManager : MonoBehaviour
{
    public LevelManager levelManager;
    public TabletManager tabletManager;
    public AndroidController androidController;
    public Launch launch;
    public WeaponManager weaponManager;

    /// <summary>
    /// 자동 UI 업데이트 도우미 오브젝트
    /// </summary>
    public UpdateDisplay updateDisplay;

    /// <summary>
    /// 저장될 스탯 데이터
    /// </summary>
    private StatData statData;
    /// <summary>
    /// 스탯 능력치 총괄 오브젝트
    /// </summary>
    public StatAbility[] stats;

    /// <summary>
    /// 사용하지 않은 스탯 포인트 텍스트
    /// </summary>
    public Text unusedStat_text;
    /// <summary>
    /// 테스트 모드 여부
    /// </summary>
    public bool isTestMode = false;

    private void Awake()
    {
        // 플레이 씬일 때 스탯 포인트를 적용한다.
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
            ApplyAllAbility();
        // 능력치 정보 UI를 업데이트 한다
        updateDisplay.onEnable += InitializeAbilityText;
    }

    /// <summary>
    /// 스탯 레벨업
    /// </summary>
    /// <param name="statSelect">선택한 스탯 종류(Inspector)</param>
    public void StatLevelUp(StatSelect statSelect)
    {
        // 선택한 스탯의 인덱스 추출
        int statIndex = (int)statSelect.statType;
        // 최대 레벨 검사
        if (statData.statLevels[statIndex] < stats[statIndex].statAbilityData.MaxLevel)
        {
            // 스탯 포인트 사용 처리
            if (levelManager.UseStatPoint())
            {
                // 스탯 레벨 상승 및 관련 UI, 수치 업데이트
                statData.statLevels[statIndex]++;
                SetUnusedStatText();
                SetAbilityText(statSelect.statType);
                if (!SceneManager.GetActiveScene().buildIndex.Equals(0))
                    ApplyStatAbility(statSelect.statType);
            }
            else
            {
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_stat"));
            }
        }
        else
            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("max_limit"));
    }

    /// <summary>
    /// 사용되지 않은 스탯 UI 업데이트
    /// </summary>
    public void SetUnusedStatText()
    {
        unusedStat_text.text = LocalizationManager.instance.GetLocalizedValue("unusedstat") + ": " + levelManager.GetUnusedStatPoint();
    }

    /// <summary>
    /// 전체 능력치 텍스트 정보 업데이트
    /// </summary>
    public void InitializeAbilityText()
    {
        SetUnusedStatText();
        for (int i = 0; i < stats.Length; i++)
        {
            SetAbilityText((StatType)i);
        }
    }

    /// <summary>
    /// 능력치 텍스트 정보 업데이트
    /// </summary>
    /// <param name="type">업데이트 할 스탯</param>
    public void SetAbilityText(StatType type)
    {
        int index = (int)type;
        switch(type)
        {
            // 1단계 업그레이드만 가능한 가속 조절법 학습 정보 업데이트
            case StatType.Accelerate:
                if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
                {
                    stats[index].statAbility_text.text = "X → O";
                }
                else
                {
                    stats[index].statAbility_text.text = "O";
                }
                break;
            // 퍼센트로 능력치 상승하는 부류 정보 업데이트
            default:
                if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
                {
                    stats[index].statAbility_text.text = stats[index].statAbilityData.Ability[statData.statLevels[index]]
                        + "% → " + stats[index].statAbilityData.Ability[statData.statLevels[index] + 1] + "%";
                }
                else
                {
                    stats[index].statAbility_text.text = stats[index].statAbilityData.Ability[statData.statLevels[index]] + "%";
                }
                break;
        }

        // 능력치 레벨 정보 업데이트
        if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
            stats[index].statLevel_text.text = "Lv. " + statData.statLevels[index];
        else
            stats[index].statLevel_text.text = "Lv. <color=blue>MAX</color>";
    }

    /// <summary>
    /// 선택한 스탯의 현재 레벨을 얻어온다.
    /// </summary>
    /// <param name="type">선택한 스탯</param>
    /// <returns>스탯 레벨</returns>
    public int GetLevel(StatType type)
    {
        return statData.statLevels[(int)type];
    }

    /// <summary>
    /// 선택한 스탯 능력치 수치를 적용한다.
    /// </summary>
    /// <param name="type">선택한 스탯</param>
    public void ApplyStatAbility(StatType type)
    {
        int index = (int)type;
        switch (type)
        {
            case StatType.Efficiency:
                launch.powerEfficiency = 1 - stats[index].statAbilityData.Ability[statData.statLevels[index]] * 0.01f;
                break;
            case StatType.StarSearch:
                StarDrop.statEffect = 1 + stats[index].statAbilityData.Ability[statData.statLevels[index]] * 0.01f;
                break;
            case StatType.CoinCollect:
                CoinDrop.statEffect = 1 + stats[index].statAbilityData.Ability[statData.statLevels[index]] * 0.01f;
                break;
            case StatType.Reload:
                weaponManager.reloadAbility = 1f + stats[index].statAbilityData.Ability[statData.statLevels[index]] * 0.01f;
                break;
            case StatType.Booster:
                launch.boosterPowerAbility = 1f + stats[index].statAbilityData.Ability[statData.statLevels[index]] * 0.01f;
                break;
        }
    }

    /// <summary>
    /// 모든 능력치 수치를 적용한다.
    /// </summary>
    public void ApplyAllAbility()
    {
        for(int i = 0; i <= stats.Length; i++)
        {
            ApplyStatAbility((StatType)i);
        }
    }

    /// <summary>
    /// 능력치 데이터 저장
    /// </summary>
    /// <param name="data">저장될 데이터</param>
    public void SaveData(ref Data data)
    {
        data.statData = statData;
    }
    /// <summary>
    /// 능력치 데이터 불러오기
    /// </summary>
    /// <param name="data">불러온 데이터</param>
    public void LoadData(Data data)
    {
        if (data.statData != null)
        {
            statData = data.statData;
        }
        else 
        { 
            statData = new StatData();
            statData.statLevels = new int[stats.Length];
        }

        if (isTestMode)
            StatTestMode();
    }
    /// <summary>
    /// 능력치 테스트 모드 (전체 만랩)
    /// </summary>
    public void StatTestMode()
    {
        for (int i = 0; i < statData.statLevels.Length; i++)
            statData.statLevels[i] = stats[i].statAbilityData.MaxLevel;
    }
}

/// <summary>
/// 스탯 데이터 클래스
/// </summary>
[System.Serializable]
public class StatData
{
    /// <summary>
    /// 각 스탯별 현재 레벨
    /// </summary>
    public int[] statLevels;
}
/// <summary>
/// 스탯 능력치 데이터 및 텍스트 보관 클래스
/// </summary>
[System.Serializable]
public class StatAbility
{
    /// <summary>
    /// 스탯 능력치 데이터
    /// </summary>
    public StatAbilityData statAbilityData;
    /// <summary>
    /// 스탯 능력치 효과 정보 텍스트
    /// </summary>
    public Text statAbility_text;
    /// <summary>
    /// 스탯 레벨 정보 텍스트
    /// </summary>
    public Text statLevel_text;
}
    