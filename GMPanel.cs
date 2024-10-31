
using System;
using System.Collections.Generic;
using Product;
using Product.GDebug;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class GMDefine
{
    public static List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
    public static List<Func<string, string>> funcs = new List<Func<string, string>>();

    /// <summary>
    /// 添加指令 - 需要在GMPanel初始化时将指令执行完
    /// </summary>
    /// <param name="name"></param>
    /// <param name="func"></param>
    public static void AddCommand(string name, Func<string, string> func)
    {
        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
        optionData.text = name;
        optionDatas.Add(optionData);
        funcs.Add(func);
    }
}

public class GMPanel : UIBase
{
    private TMP_Dropdown dropdown;
    private TMP_InputField inputTxt;
    private Button triggerBtn;

    private void InitCommand()
    {
        GMDefine.AddCommand("加10个孔",
        (string param) =>
        {
            for (int i = 0; i < 11; i++)
                GameEntry.Event.Fire(this, ScrewHoleAddEventArgs.Create());
            return "";
        });

        GMDefine.AddCommand("跳关",
        (string param) =>
        {
            if (!int.TryParse(param, out int levelId)) levelId = 1;
            GameEntry.Event.Fire(this, DebugLoadLevelEventArgs.Create(levelId));
            return "";
        });
    }

    // Start is called before the first frame update
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        InitCommand();
        triggerBtn = transform.Find("Debug/triggerBtn").GetComponent<Button>();
        inputTxt = transform.Find("Debug/paramInput").GetComponent<TMP_InputField>();
        dropdown = transform.Find("Debug/dropDown").GetComponent<TMP_Dropdown>();
        triggerBtn.onClick.AddListener(() =>
        {
            if (dropdown.options.Count == 0) return;
            GMDefine.funcs[dropdown.value].Invoke(inputTxt.text);
        });
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        dropdown.options.Clear();
        dropdown.AddOptions(GMDefine.optionDatas);
    }
}
