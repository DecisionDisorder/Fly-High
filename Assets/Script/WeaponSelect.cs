using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ������
/// </summary>
public enum WeaponType { Crossbow, BGG_0, RPG }
/// <summary>
/// ���� ���� ����� Ŭ����(Inspector��)
/// </summary>
public class WeaponSelect : MonoBehaviour
{
    public WeaponType weapon;
}
