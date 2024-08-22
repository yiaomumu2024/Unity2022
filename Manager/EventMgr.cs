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

    private Dictionary<string, Action> eventDic = new Dictionary<string, Action>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void AddListener(string eventName, Action listener)
    {
        eventDic.Add(eventName, listener);
    }

    public void RemListener(string eventName)
    {
        if (instance == null) { return; }

        if (eventDic.TryGetValue(eventName, out Action thisEvent))
        {
            eventDic.Remove(eventName);
        }
    }

    public void TriggerEvent(string eventName)
    {
        if (eventDic.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}