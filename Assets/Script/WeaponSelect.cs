using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 종류 열거형
/// </summary>
public enum WeaponType { Crossbow, BGG_0, RPG }
/// <summary>
/// 무기 선택 도우미 클래스(Inspector용)
/// </summary>
public class WeaponSelect : MonoBehaviour
{
    public WeaponType weapon;
}
