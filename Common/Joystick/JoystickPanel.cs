using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EJoystickType
{

    // 固定
    Fix,
    // 跟随固定
    FixFollow,
    // 一直跟随
    Follow,
}

public class JoystickPanel : BaseUI
{

    public float maxL = 85;
    public float idleAlpha = 0.6f;
    public float toIdleTime = 0.5f;
    public EJoystickType stickType = EJoystickType.Follow;
    private Image touchRect, imgBg, imgControl;
    private Vector2 bgInitPos, controllerInitPos;
    private CanvasGroup opaGroup;
    private Coroutine myCoroutine;

    protected override void OnInit()
    {
        touchRect = GetComponentInChildren<Image>("touchRect");
        imgBg = GetComponentInChildren<Image>("imgBg");
        imgControl = GetComponentInChildren<Image>("imgControl");
        bgInitPos = imgBg.transform.position;
        controllerInitPos = imgControl.transform.position;
        opaGroup = GetComponentInChildren<CanvasGroup>();
        toIdleTime = (1 - idleAlpha) / toIdleTime;
        opaGroup.alpha = idleAlpha;

        UIMgr.AddCustomEventListener(touchRect.gameObject, EventTriggerType.PointerDown, OnPointerDown);
        UIMgr.AddCustomEventListener(touchRect.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        UIMgr.AddCustomEventListener(touchRect.gameObject, EventTriggerType.Drag, OnDrag);
    }

    private void OnPointerDown(PointerEventData data)
    {
        opaGroup.alpha = 1;
        if (myCoroutine != null) StopCoroutine(myCoroutine);
        if (stickType != EJoystickType.Fix)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(touchRect.rectTransform, data.position, data.pressEventCamera, out localPos);
            imgBg.transform.localPosition = localPos;
        }
        else
        {
            SetAndGetControllerPos(data);
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        myCoroutine = StartCoroutine(StartIdleAni());
        imgBg.transform.position = bgInitPos;
        imgControl.transform.position = controllerInitPos;
        EventMgr.Instance.TriggerEvent("Joystick", Vector2.zero);
    }

    private void OnDrag(PointerEventData data)
    {
        Vector2 localPos = SetAndGetControllerPos(data);
        if (stickType == EJoystickType.Follow && localPos.magnitude > maxL)
        {
            imgBg.transform.localPosition += (Vector3)(localPos.normalized * (localPos.magnitude - maxL));
        }

        EventMgr.Instance.TriggerEvent("Joystick", localPos.normalized);
    }

    private Vector2 SetAndGetControllerPos(PointerEventData data)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imgBg.rectTransform, data.position, data.pressEventCamera, out localPos);

        imgControl.transform.localPosition = localPos;


        if (localPos.magnitude > maxL)
        {
            imgControl.transform.localPosition = localPos.normalized * maxL;
        }
        return localPos;
    }

    private IEnumerator StartIdleAni()
    {
        while (opaGroup.alpha > idleAlpha)
        {
            opaGroup.alpha -= Time.deltaTime * toIdleTime;
            yield return null;
        }
    }
}
