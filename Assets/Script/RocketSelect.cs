using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����
/// </summary>
public enum RocketType { BottleRocket, Standard, Narrow, Powerful, Efficient, Ordinary }

/// <summary>
/// Inspector(OnClick)���� ������ ������ �� �ֵ��� �����ִ� Ŭ����
/// </summary>
public class RocketSelect : MonoBehaviour
{
    /// <summary>
    /// Inspector���� ���õ� ����
    /// </summary>
    public RocketType rocket;
}
