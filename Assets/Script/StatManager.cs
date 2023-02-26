using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum StatType { Accelerate, Efficiency, StarSearch, CoinCollect, Reload, Booster }

public class StatManager : MonoBehaviour
{
    public LevelManager levelManager;
    public TabletManager tabletManager;
    public AndroidController androidController;
    public Launch launch;
    public WeaponManager weaponManager;

    public UpdateDisplay updateDisplay;

    private StatData statData;
    public StatAbility[] stats;

    public Text unusedStat_text;
    public bool isTestMode = false;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
            ApplyAllAbility();
        updateDisplay.onEnable += InitializeAbilityText;
    }

    public void StatLevelUp(StatSelect statSelect)
    {
        int statIndex = (int)statSelect.statType;
        if (statData.statLevels[statIndex] < stats[statIndex].statAbilityData.MaxLevel)
        {
            if (levelManager.UseStatPoint())
            {
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

    public void SetUnusedStatText()
    {
        unusedStat_text.text = LocalizationManager.instance.GetLocalizedValue("unusedstat") + ": " + levelManager.GetUnusedStatPoint();
    }

    public void InitializeAbilityText()
    {
        SetUnusedStatText();
        for (int i = 0; i < stats.Length; i++)
        {
            SetAbilityText((StatType)i);
        }
    }

    public void SetAbilityText(StatType type)
    {
        int index = (int)type;
        switch(type)
        {
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
            default: // 퍼센트류
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

        if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
            stats[index].statLevel_text.text = "Lv. " + statData.statLevels[index];
        else
            stats[index].statLevel_text.text = "Lv. <color=blue>MAX</color>";
    }

    public int GetLevel(StatType type)
    {
        return statData.statLevels[(int)type];
    }

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

    public void ApplyAllAbility()
    {
        for(int i = 0; i <= stats.Length; i++)
        {
            ApplyStatAbility((StatType)i);
        }
    }

    public void SaveData(ref Data data)
    {
        data.statData = statData;
    }
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
    public void StatTestMode()
    {
        for (int i = 0; i < statData.statLevels.Length; i++)
            statData.statLevels[i] = stats[i].statAbilityData.MaxLevel;
    }
}

[System.Serializable]
public class StatData
{
    public int[] statLevels;
}
[System.Serializable]
public class StatAbility
{
    public StatAbilityData statAbilityData;
    public Text statAbility_text;
    public Text statLevel_text;
}
    