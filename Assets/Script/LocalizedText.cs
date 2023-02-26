using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로컬라이징 된 텍스트 데이터 클래스
/// </summary>
public class LocalizedText : MonoBehaviour
{
    /// <summary>
    /// 해당 텍스트에 맞는 키 값
    /// </summary>
    public string key;
    /// <summary>
    /// 언어 설정에 따라 적용될 텍스트
    /// </summary>
    private Text text;

    /// <summary>
    /// 활성화 될 때(Canvas on/off) 텍스트를 업데이트한다.
    /// </summary>
    private void OnEnable()
    {
        if(text == null)
            text = GetComponent<Text>();
        ReloadText();
    }

    /// <summary>
    /// 설정된 언어에 따라 텍스트 정보 업데이트
    /// </summary>
    public void ReloadText()
    {
        if(text != null && LocalizationManager.instance != null)
        {
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
    }
}
