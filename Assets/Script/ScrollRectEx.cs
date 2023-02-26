using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// ScrollRect가 겹쳤을 때의 문제를 해결하기 위한 커스텀 클래스
/// </summary>
public class ScrollRectEx : ScrollRect
{
    /// <summary>
    /// 부모 ScrollRect로 전달 여부
    /// </summary>
    private bool routeToParent = false;

    /// <summary>
    /// 모든 부모 계층으로 액션 전달
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
    /// 잠재적 드래그 이벤트를 부모에게 항상 라우팅
    /// </summary>
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    /// <summary>
    /// 드래그 이벤트 오버라이딩
    /// </summary>
    public override void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        else
            base.OnDrag(eventData);
    }

    /// <summary>
    /// 드래그 시작 이벤트 오버라이딩
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
    /// 드래그 종료 이벤트 오버라이딩
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
