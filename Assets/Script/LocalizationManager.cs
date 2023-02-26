using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 설정된 언어에 따라서 단어 및 문장을 찾아주는 관리 클래슽
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    /// <summary>
    /// 로컬라이징 관리 클래스의 싱글톤 오브젝트
    /// </summary>
    public static LocalizationManager instance;
    
    public PlayManager playManager;
    public Settings settings;
    public LevelManager levelManager;

    /// <summary>
    /// 설정된 언어에 따른 텍스트 데이터
    /// </summary>
    private Dictionary<string, string> localizedText;
    /// <summary>
    /// 로컬라이징 데이터 로드 실패 안내 메시지
    /// </summary>
    private string missingTextString = "Localized text not found";
    /// <summary>
    /// UI 캔버스
    /// </summary>
    public GameObject canvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 언어 설정
        settings.SetLanguage(settings.GetLanguage());
    }

    /// <summary>
    /// 로컬 데이터 파일에서 로컬라이징된 데이터를 불러온다.
    /// </summary>
    /// <param name="fileName">언어 파일 이름</param>
    public void LoadLocalizedText(string fileName)
    {
        // Dictionary 초기화
        localizedText = new Dictionary<string, string>();
        // DataFile에 있는 언어 파일 불러오기
        TextAsset mytxtData = Resources.Load<TextAsset>("DataFile/" + fileName);
        string txt = mytxtData.text;
        if(txt != "" && txt != null)
        {
            // JSON 으로 저장된 언어 파일을 불러와서 Dictionary에 각각 추가한다.
            string dataAsJson = txt;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for(int i = 0; i < loadedData.items.Length; i++)
            {
                if (!localizedText.ContainsKey(loadedData.items[i].value))
                    localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        // 언어 파일 불러오기 실패
        else
        {
            Debug.LogError("Cannot find file");
        }
    }
    /// <summary>
    /// 주어진 키 값에 맞는 로컬라이징된 텍스트를 가져온다.
    /// </summary>
    /// <param name="key">텍스트 키 값</param>
    /// <returns>설정된 언어에 맞는 텍스트</returns>
    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if(localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }
    /// <summary>
    /// 변경된 언어로 텍스트를 리로드 한다.
    /// </summary>
    /// <param name="fileName">언어 데이터 파일 이름</param>
    public void ReloadTexts(string fileName)
    {
        // 해당 언어 데이터 불러오기
        LoadLocalizedText(fileName);

        // 플레이 관리 클래스가 있는 경우에 점수 텍스트를 불러오고 점수 정보 리로드
        if (playManager != null) // Only for Launch Scene
        {
            playManager.score_local = GetLocalizedValue("score");
            playManager.ReloadScore();
        }
        // 레벨 데이터 초기화
        if (levelManager != null)
            levelManager.SetLevelText();

        // 캔버스 업데이트
        canvas.SetActive(false);
        canvas.SetActive(true);
    }
}

/// <summary>
/// Json 불러오기 위한 로컬라이징 데이터 클래스
/// </summary>
[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

/// <summary>
/// Json 불러오기 위한 개별 로컬라이징 데이터 클래스
/// </summary>
[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
    public LocalizationItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}