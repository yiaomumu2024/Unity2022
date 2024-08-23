using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr : MonoBehaviour
{
    private static EventMgr instance;

    public static EventMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EvenManager");
                    instance = obj.AddComponent<EventMgr>();
                }
            }
            return instance;
        }
    }

    private Dictionary<string, Action<object[]>> eventDic = new Dictionary<string, Action<object[]>>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void AddListener(string eventName, Action<object[]> listener)
    {
        if (eventDic.ContainsKey(eventName))
        {
            RemListener(eventName);
        }
        eventDic.Add(eventName, listener);
    }

    public void RemListener(string eventName)
    {
        if (instance == null) { return; }
        eventDic.Remove(eventName);
    }

    public void TriggerEvent(string eventName, params object[] eventParams)
    {
        if (eventDic.TryGetValue(eventName, out Action<object[]> thisEvent))
        {
            thisEvent.Invoke(eventParams);
        }
    }
}