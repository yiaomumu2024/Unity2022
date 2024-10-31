using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleEx : MonoBehaviour, IDragHandler
{

    private RectTransform _parent;

    private RectTransform parent
    {

        get
        {
            _parent ??= transform.parent.GetComponent<RectTransform>();
            return _parent;
        }
    }

    private RectTransform _rect;

    private RectTransform rect
    {

        get
        {
            _rect ??= transform.GetComponent<RectTransform>();
            return _rect;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rect.anchoredPosition = localPoint;
    }
}
