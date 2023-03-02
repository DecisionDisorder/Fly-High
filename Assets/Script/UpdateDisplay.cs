using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �ڵ� ������Ʈ ����� Ŭ����
/// </summary>
public class UpdateDisplay : MonoBehaviour
{
    public delegate void OnEnableUpdate();
    /// <summary>
    /// ������Ʈ�� Enable �� �� ȣ��� �Լ�
    /// </summary>
    public OnEnableUpdate onEnable;

    /// <summary>
    /// �ش� ������Ʈ�� Ȱ��ȭ �� �� ����� �Լ��� ȣ���Ѵ�.
    /// </summary>
    private void OnEnable()
    {
        onEnable();
    }
}
