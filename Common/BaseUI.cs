using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{

    // 用于存储当前UI的RectTransform
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        OnInit();
    }

    protected abstract void OnInit();

    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
    }

    protected virtual void OnShow() { }

    protected virtual void OnHide() { }

    protected T GetUIComponent<T>(string name) where T : Component
    {
        Transform target = rectTransform.Find(name);
        if (target != null)
        {
            return target.GetComponent<T>();
        }
        Debug.LogWarning($"UI Component with name {name} not found!");
        return null;
    }

    /// <summary>
    /// 从子节点中查找名为childName的子节点的某个组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="childName"></param>
    /// <returns></returns>
    protected T GetComponentInChildren<T>(string childName) where T : Component
    {
        Transform target = FindChildrenRecursive(transform, childName);
        if (target != null)
        {
            return target.GetComponent<T>();
        }
        Debug.LogWarning($"Child with name {childName} not found in children!");
        return null;
    }

    /// <summary>
    /// 递归查找子节点
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    private Transform FindChildrenRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            Transform found = FindChildrenRecursive(child, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
}
