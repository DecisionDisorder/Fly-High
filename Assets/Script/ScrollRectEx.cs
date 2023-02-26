using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// ScrollRect�� ������ ���� ������ �ذ��ϱ� ���� Ŀ���� Ŭ����
/// </summary>
public class ScrollRectEx : ScrollRect
{
    /// <summary>
    /// �θ� ScrollRect�� ���� ����
    /// </summary>
    private bool routeToParent = false;

    /// <summary>
    /// ��� �θ� �������� �׼� ����
    /// </summary>
    private void DoForParents<T>(Action<T> action) where T:IEventSystemHandler
    {
        Transform parent = transform.parent;
        while(parent != null)
        {
            foreach(var component in parent.GetComponents<Component>())
            {
                if (component is T)
                    action((T)(IEventSystemHandler)component);
            }
            parent = parent.parent;
        }
    }

    /// <summary>
    /// ������ �巡�� �̺�Ʈ�� �θ𿡰� �׻� �����
    /// </summary>
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    /// <summary>
    /// �巡�� �̺�Ʈ �������̵�
    /// </summary>
    public override void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        else
            base.OnDrag(eventData);
    }

    /// <summary>
    /// �巡�� ���� �̺�Ʈ �������̵�
    /// </summary>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
            routeToParent = true;
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
            routeToParent = true;
        else
            routeToParent = false;

        if (routeToParent)
            DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        else
            base.OnBeginDrag(eventData);
    }

    /// <summary>
    /// �巡�� ���� �̺�Ʈ �������̵�
    /// </summary>
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        else
            base.OnEndDrag(eventData);
        routeToParent = false;
    }
}
