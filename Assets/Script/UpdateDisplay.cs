using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 정보 자동 업데이트 도우미 클래스
/// </summary>
public class UpdateDisplay : MonoBehaviour
{
    public delegate void OnEnableUpdate();
    /// <summary>
    /// 오브젝트가 Enable 될 때 호출될 함수
    /// </summary>
    public OnEnableUpdate onEnable;

    /// <summary>
    /// 해당 오브젝트가 활성화 될 때 등록한 함수를 호출한다.
    /// </summary>
    private void OnEnable()
    {
        onEnable();
    }
}
