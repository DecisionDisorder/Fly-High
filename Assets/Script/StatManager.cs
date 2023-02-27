using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ���� ������
/// </summary>
public enum StatType { Accelerate, Efficiency, StarSearch, CoinCollect, Reload, Booster }

/// <summary>
/// ���� �ý��� ���� Ŭ����
/// </summary>
public class StatManager : MonoBehaviour
{
    public LevelManager levelManager;
    public TabletManager tabletManager;
    public AndroidController androidController;
    public Launch launch;
    public WeaponManager weaponManager;

    /// <summary>
    /// �ڵ� UI ������Ʈ ����� ������Ʈ
    /// </summary>
    public UpdateDisplay updateDisplay;

    /// <summary>
    /// ����� ���� ������
    /// </summary>
    private StatData statData;
    /// <summary>
    /// ���� �ɷ�ġ �Ѱ� ������Ʈ
    /// </summary>
    public StatAbility[] stats;

    /// <summary>
    /// ������� ���� ���� ����Ʈ �ؽ�Ʈ
    /// </summary>
    public Text unusedStat_text;
    /// <summary>
    /// �׽�Ʈ ��� ����
    /// </summary>
    public bool isTestMode = false;

    private void Awake()
    {
        // �÷��� ���� �� ���� ����Ʈ�� �����Ѵ�.
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
            ApplyAllAbility();
        // �ɷ�ġ ���� UI�� ������Ʈ �Ѵ�
        updateDisplay.onEnable += InitializeAbilityText;
    }

    /// <summary>
    /// ���� ������
    /// </summary>
    /// <param name="statSelect">������ ���� ����(Inspector)</param>
    public void StatLevelUp(StatSelect statSelect)
    {
        // ������ ������ �ε��� ����
        int statIndex = (int)statSelect.statType;
        // �ִ� ���� �˻�
        if (statData.statLevels[statIndex] < stats[statIndex].statAbilityData.MaxLevel)
        {
            // ���� ����Ʈ ��� ó��
            if (levelManager.UseStatPoint())
            {
                // ���� ���� ��� �� ���� UI, ��ġ ������Ʈ
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
    /// ������ ���� ���� UI ������Ʈ
    /// </summary>
    public void SetUnusedStatText()
    {
        unusedStat_text.text = LocalizationManager.instance.GetLocalizedValue("unusedstat") + ": " + levelManager.GetUnusedStatPoint();
    }

    /// <summary>
    /// ��ü �ɷ�ġ �ؽ�Ʈ ���� ������Ʈ
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
    /// �ɷ�ġ �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    /// <param name="type">������Ʈ �� ����</param>
    public void SetAbilityText(StatType type)
    {
        int index = (int)type;
        switch(type)
        {
            // 1�ܰ� ���׷��̵常 ������ ���� ������ �н� ���� ������Ʈ
            case StatType.Accelerate:
                if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
                {
                    stats[index].statAbility_text.text = "X �� O";
                }
                else
                {
                    stats[index].statAbility_text.text = "O";
                }
                break;
            // �ۼ�Ʈ�� �ɷ�ġ ����ϴ� �η� ���� ������Ʈ
            default:
                if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
                {
                    stats[index].statAbility_text.text = stats[index].statAbilityData.Ability[statData.statLevels[index]]
                        + "% �� " + stats[index].statAbilityData.Ability[statData.statLevels[index] + 1] + "%";
                }
                else
                {
                    stats[index].statAbility_text.text = stats[index].statAbilityData.Ability[statData.statLevels[index]] + "%";
                }
                break;
        }

        // �ɷ�ġ ���� ���� ������Ʈ
        if (statData.statLevels[index] != stats[index].statAbilityData.MaxLevel)
            stats[index].statLevel_text.text = "Lv. " + statData.statLevels[index];
        else
            stats[index].statLevel_text.text = "Lv. <color=blue>MAX</color>";
    }

    /// <summary>
    /// ������ ������ ���� ������ ���´�.
    /// </summary>
    /// <param name="type">������ ����</param>
    /// <returns>���� ����</returns>
    public int GetLevel(StatType type)
    {
        return statData.statLevels[(int)type];
    }

    /// <summary>
    /// ������ ���� �ɷ�ġ ��ġ�� �����Ѵ�.
    /// </summary>
    /// <param name="type">������ ����</param>
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
    /// ��� �ɷ�ġ ��ġ�� �����Ѵ�.
    /// </summary>
    public void ApplyAllAbility()
    {
        for(int i = 0; i <= stats.Length; i++)
        {
            ApplyStatAbility((StatType)i);
        }
    }

    /// <summary>
    /// �ɷ�ġ ������ ����
    /// </summary>
    /// <param name="data">����� ������</param>
    public void SaveData(ref Data data)
    {
        data.statData = statData;
    }
    /// <summary>
    /// �ɷ�ġ ������ �ҷ�����
    /// </summary>
    /// <param name="data">�ҷ��� ������</param>
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
    /// �ɷ�ġ �׽�Ʈ ��� (��ü ����)
    /// </summary>
    public void StatTestMode()
    {
        for (int i = 0; i < statData.statLevels.Length; i++)
            statData.statLevels[i] = stats[i].statAbilityData.MaxLevel;
    }
}

/// <summary>
/// ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class StatData
{
    /// <summary>
    /// �� ���Ⱥ� ���� ����
    /// </summary>
    public int[] statLevels;
}
/// <summary>
/// ���� �ɷ�ġ ������ �� �ؽ�Ʈ ���� Ŭ����
/// </summary>
[System.Serializable]
public class StatAbility
{
    /// <summary>
    /// ���� �ɷ�ġ ������
    /// </summary>
    public StatAbilityData statAbilityData;
    /// <summary>
    /// ���� �ɷ�ġ ȿ�� ���� �ؽ�Ʈ
    /// </summary>
    public Text statAbility_text;
    /// <summary>
    /// ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text statLevel_text;
}
    