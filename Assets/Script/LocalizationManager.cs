using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// ������ �� ���� �ܾ� �� ������ ã���ִ� ���� Ŭ����
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    /// <summary>
    /// ���ö���¡ ���� Ŭ������ �̱��� ������Ʈ
    /// </summary>
    public static LocalizationManager instance;
    
    public PlayManager playManager;
    public Settings settings;
    public LevelManager levelManager;

    /// <summary>
    /// ������ �� ���� �ؽ�Ʈ ������
    /// </summary>
    private Dictionary<string, string> localizedText;
    /// <summary>
    /// ���ö���¡ ������ �ε� ���� �ȳ� �޽���
    /// </summary>
    private string missingTextString = "Localized text not found";
    /// <summary>
    /// UI ĵ����
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
        // ��� ����
        settings.SetLanguage(settings.GetLanguage());
    }

    /// <summary>
    /// ���� ������ ���Ͽ��� ���ö���¡�� �����͸� �ҷ��´�.
    /// </summary>
    /// <param name="fileName">��� ���� �̸�</param>
    public void LoadLocalizedText(string fileName)
    {
        // Dictionary �ʱ�ȭ
        localizedText = new Dictionary<string, string>();
        // DataFile�� �ִ� ��� ���� �ҷ�����
        TextAsset mytxtData = Resources.Load<TextAsset>("DataFile/" + fileName);
        string txt = mytxtData.text;
        if(txt != "" && txt != null)
        {
            // JSON ���� ����� ��� ������ �ҷ��ͼ� Dictionary�� ���� �߰��Ѵ�.
            string dataAsJson = txt;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for(int i = 0; i < loadedData.items.Length; i++)
            {
                if (!localizedText.ContainsKey(loadedData.items[i].value))
                    localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        // ��� ���� �ҷ����� ����
        else
        {
            Debug.LogError("Cannot find file");
        }
    }
    /// <summary>
    /// �־��� Ű ���� �´� ���ö���¡�� �ؽ�Ʈ�� �����´�.
    /// </summary>
    /// <param name="key">�ؽ�Ʈ Ű ��</param>
    /// <returns>������ �� �´� �ؽ�Ʈ</returns>
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
    /// ����� ���� �ؽ�Ʈ�� ���ε� �Ѵ�.
    /// </summary>
    /// <param name="fileName">��� ������ ���� �̸�</param>
    public void ReloadTexts(string fileName)
    {
        // �ش� ��� ������ �ҷ�����
        LoadLocalizedText(fileName);

        // �÷��� ���� Ŭ������ �ִ� ��쿡 ���� �ؽ�Ʈ�� �ҷ����� ���� ���� ���ε�
        if (playManager != null) // Only for Launch Scene
        {
            playManager.score_local = GetLocalizedValue("score");
            playManager.ReloadScore();
        }
        // ���� ������ �ʱ�ȭ
        if (levelManager != null)
            levelManager.SetLevelText();

        // ĵ���� ������Ʈ
        canvas.SetActive(false);
        canvas.SetActive(true);
    }
}

/// <summary>
/// Json �ҷ����� ���� ���ö���¡ ������ Ŭ����
/// </summary>
[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

/// <summary>
/// Json �ҷ����� ���� ���� ���ö���¡ ������ Ŭ����
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