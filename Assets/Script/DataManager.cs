using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 게임 데이터 관리 클래스
/// </summary>
public class DataManager : MonoBehaviour
{
    /// <summary>
    /// 게임 저장 여부
    /// </summary>
    public static bool isSaveMode = false; // TODO: 출시때는 true

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
    /// 로컬 데이터 저장
    /// </summary>
    public void SaveData()
    {
        // 각 관리 클래스에 있는 데이터를 data에 복사
        Data data = new Data();
        playManager.SaveData(ref data);
        economicMgr.SaveData(ref data);
        levelManager.SaveData(ref data);
        statManager.SaveData(ref data);
        settings.SaveData(ref data);
        itemManager.SaveData(ref data);
        equipmentManager.SaveData(ref data);

        // 로컬 파일에 직렬화하여 저장
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/dataInfo.dat");
        formatter.Serialize(file, data);
        file.Close();
        
    }

    /// <summary>
    /// 로컬 데이터 로드
    /// </summary>
    public void LoadData()
    {
        // 지정된 경로에 파일이 있으면 데이터를 불러와서 역직렬화
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
            // 로드된 데이터가 없으면 데이터 초기화
            ResetData();
            return;
            //Debug.Log("Data file does not exist.");
        }

        // 불러온/초기화된 데이터 적용
        playManager.LoadData(data);
        economicMgr.LoadData(data);
        levelManager.LoadData(data);
        statManager.LoadData(data);
        settings.LoadData(data);
        equipmentManager.LoadData(data);
        itemManager.LoadData(data);
    }

    /// <summary>
    /// 데이터 초기화
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
/// 각 관리 클래스에 퍼져있는 데이터를 한 오브젝트로 모아주는 데이터 클래스
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