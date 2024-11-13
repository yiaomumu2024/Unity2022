using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 全局按钮互斥
/// </summary>
public class GlobalBtnCoolDown : MonoBehaviour
{
    /// <summary>
    /// 延迟时间
    /// </summary>
    [SerializeField] float m_CoolDownDuration = 0.1f;
    private bool m_EnableSelectable = true;
    private float coolTime = 0;

    void Awake()
    {
        typeof(ExecuteEvents).GetField("s_PointerClickHandler", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, new ExecuteEvents.EventFunction<IPointerClickHandler>(OnPointerClick));
    }

    void OnPointerClick(IPointerClickHandler handler, BaseEventData eventData)
    {
        PointerEventData pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
        if (pointerEventData != null)
        {
            if (!m_EnableSelectable)
            {
                return;
            }
            handler.OnPointerClick(pointerEventData);
            m_EnableSelectable = false;
            coolTime = 0;
        }
    }

    void Update()
    {
        if (!m_EnableSelectable)
        {
            coolTime += Time.unscaledDeltaTime;
            if (coolTime >= m_CoolDownDuration)
                m_EnableSelectable = true;
        }
    }
}
