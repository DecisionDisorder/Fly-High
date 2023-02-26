using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로켓 종류
/// </summary>
public enum RocketType { BottleRocket, Standard, Narrow, Powerful, Efficient, Ordinary }

/// <summary>
/// Inspector(OnClick)에서 로켓을 선택할 수 있도록 도와주는 클래스
/// </summary>
public class RocketSelect : MonoBehaviour
{
    /// <summary>
    /// Inspector에서 선택된 로켓
    /// </summary>
    public RocketType rocket;
}
