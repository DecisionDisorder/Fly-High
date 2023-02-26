using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateDisplay : MonoBehaviour
{
    public delegate void OnEnableUpdate();
    public OnEnableUpdate onEnable;

    private void OnEnable()
    {
        onEnable();
    }
}
