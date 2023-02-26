using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RocketType { BottleRocket, Standard, Narrow, Powerful, Efficient, Ordinary }
public class RocketSelect : MonoBehaviour
{
    public RocketType rocket;

    public RocketType GetRocket()
    {
        return rocket;
    }
}
