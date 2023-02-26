using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// ���� ������ ���� Ŭ����
/// </summary>
public class DataManager : MonoBehaviour
{
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public static bool isSaveMode = false; // TODO: ��ö��� true

    public LevelManager levelManager;
    public StatManager statManager;
    public PlayManager playManager;
    public EconomicMgr economicMgr;
    public Settings settings;
    public ItemManager itemManager;
    public EquipmentManager equipmentManager;
    public AndroidController androidController;

    private void Awake()
    {
        LoadData();
    }

    /// <summary>
    /// ���� ������ ����
    /// </summary>
    public void SaveData()
    {
        // �� ���� Ŭ������ �ִ� �����͸� data�� ����
        Data data = new Data();
        playManager.SaveData(ref data);
        economicMgr.SaveData(ref data);
        levelManager.SaveData(ref data);
        statManager.SaveData(ref data);
        settings.SaveData(ref data);
        itemManager.SaveData(ref data);
        equipmentManager.SaveData(ref data);

        // ���� ���Ͽ� ����ȭ�Ͽ� ����
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/dataInfo.dat");
        formatter.Serialize(file, data);
        file.Close();
        
    }

    /// <summary>
    /// ���� ������ �ε�
    /// </summary>
    public void LoadData()
    {
        // ������ ��ο� ������ ������ �����͸� �ҷ��ͼ� ������ȭ
        BinaryFormatter formatter = new BinaryFormatter(); 
        FileStream file = null; 
        Data data = null;
        try
        {
            file = File.Open(Application.persistentDataPath + "/dataInfo.dat", FileMode.Open);
            if (file != null && file.Length > 0)
            {
                data = (Data)formatter.Deserialize(file);

                file.Close();
            }
            else
            {
                //Debug.Log("Cannot load data file.");
                androidController.ShowMessage("[ERROR] Cannot load data file.", 5f);
            }

        }
        catch
        {
            // �ε�� �����Ͱ� ������ ������ �ʱ�ȭ
            ResetData();
            return;
            //Debug.Log("Data file does not exist.");
        }

        // �ҷ���/�ʱ�ȭ�� ������ ����
        playManager.LoadData(data);
        economicMgr.LoadData(data);
        levelManager.LoadData(data);
        statManager.LoadData(data);
        settings.LoadData(data);
        equipmentManager.LoadData(data);
        itemManager.LoadData(data);
    }

    /// <summary>
    /// ������ �ʱ�ȭ
    /// </summary>
    public void ResetData()
    {
        Data data = new Data();

        playManager.LoadData(data);
        economicMgr.LoadData(data);
        levelManager.LoadData(data);
        statManager.LoadData(data);
        settings.LoadData(data);
        equipmentManager.LoadData(data);
        itemManager.LoadData(data);
        itemManager.ResetItems();
    }
}

/// <summary>
/// �� ���� Ŭ������ �����ִ� �����͸� �� ������Ʈ�� ����ִ� ������ Ŭ����
/// </summary>
[System.Serializable]
public class Data
{
    public EconomicData economicData;
    public PlayData playData;
    public LevelData levelData;
    public StatData statData;
    public SettingData settingData;
    public List<Item> rocketItemData;
    public List<Item> itemsData;
    public EquipmentData equipmentData;
}