using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ö���¡ �� �ؽ�Ʈ ������ Ŭ����
/// </summary>
public class LocalizedText : MonoBehaviour
{
    /// <summary>
    /// �ش� �ؽ�Ʈ�� �´� Ű ��
    /// </summary>
    public string key;
    /// <summary>
    /// ��� ������ ���� ����� �ؽ�Ʈ
    /// </summary>
    private Text text;

    /// <summary>
    /// Ȱ��ȭ �� ��(Canvas on/off) �ؽ�Ʈ�� ������Ʈ�Ѵ�.
    /// </summary>
    private void OnEnable()
    {
        if(text == null)
            text = GetComponent<Text>();
        ReloadText();
    }

    /// <summary>
    /// ������ �� ���� �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    public void ReloadText()
    {
        if(text != null && LocalizationManager.instance != null)
        {
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
    }
}
