using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour
{
    private static UIMgr instance;

    public static UIMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIMgr");
                    instance = obj.AddComponent<UIMgr>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndInit();
    }

    private Canvas rootCanvas;
    private EventSystem eventSystem;
    private readonly string uiPrefabPath = "Prefabs/UI/";
    private readonly string canvasPrefabPath = "Prefabs/UI/RootCanvas";
    private readonly string eventSystemPath = "Prefabs/UI/EventSystem";
    private Dictionary<string, BaseUI> uiDic = new Dictionary<string, BaseUI>();

    private void CheckAndInit()
    {
        if (rootCanvas == null || eventSystem == null)
        {
            Canvas[] canvasArr = FindObjectsOfType<Canvas>();
            rootCanvas = canvasArr.FirstOrDefault(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
            if (!rootCanvas) { rootCanvas = Instantiate(Resources.Load<GameObject>(canvasPrefabPath)).GetComponent<Canvas>(); }
            rootCanvas.name = "RootCanvas";
            eventSystem = FindObjectOfType<EventSystem>();
            if (!eventSystem) { eventSystem = Instantiate(Resources.Load<GameObject>(eventSystemPath)).GetComponent<EventSystem>(); }
            eventSystem.name = "EventSystem";
        }
    }

    // 显示指定的面板
    public void ShowPanel(string panelName)
    {
        if (uiDic.TryGetValue(panelName, out BaseUI panel))
        {
            if (panel != null)
            {
                panel.Show();
            }
            else
            {
                RegisterPanel(panelName);
                ShowPanel(panelName);
            }
        }
        else
        {
            Debug.LogWarning($"Panel {panelName} not registered!");
        }
    }

    // 关闭指定的面板
    public void HidePanel(string panelName)
    {
        if (uiDic.TryGetValue(panelName, out BaseUI panel))
        {
            if (panel != null)
            {
                panel.Hide();
            }
            else
            {
                RegisterPanel(panelName);
                HidePanel(panelName);
            }
        }
        else
        {
            Debug.LogWarning($"Panel {panelName} not registered!");
        }
    }

    // 注册面板到管理器中
    public void RegisterPanel(string panelName)
    {
        if (uiDic.TryGetValue(panelName, out BaseUI panel))
        {
            if (panel == null)
            {
                panel = Instantiate(Resources.Load<GameObject>(uiPrefabPath + panelName)).GetComponent<BaseUI>();
                panel.name = panelName;
                panel.Hide();
                CheckAndInit();
                panel.transform.SetParent(rootCanvas.transform);
                uiDic[panelName] = panel;
            }
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(uiPrefabPath + panelName);
            if (prefab != null)
            {
                GameObject panelObject = Instantiate(prefab);
                panel = panelObject.GetComponent<BaseUI>();
                if (panel != null)
                {
                    panel.name = panelName;
                    panel.Hide();
                    CheckAndInit();
                    panel.transform.SetParent(rootCanvas.transform);
                    uiDic.Add(panelName, panel);
                }
                else
                {
                    Debug.LogError($"UI Prefab {panelName} does not contain a UIBase component!");
                }
            }
            else
            {
                Debug.LogError($"UI Prefab {panelName} not found in Resources at path: {uiPrefabPath + panelName}");
            }
        }
    }

    public static void AddCustomEventListener(GameObject widget, EventTriggerType triggerType, Action<PointerEventData> callback)
    {
        EventTrigger eventTrigger = widget.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = widget.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = triggerType
        };
        if (!eventTrigger.triggers.Contains(entry))
        {
            entry.callback.AddListener((data) => { callback((PointerEventData)data); });
            eventTrigger.triggers.Add(entry);
        }
    }
}