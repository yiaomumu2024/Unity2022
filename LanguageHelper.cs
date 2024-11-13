using GameFramework;
using GameFramework.Localization;
using Product.Configs;
using Product.Script.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Product.Script.Tools
{
    /// <summary>
    /// 文本的多语言
    /// </summary>
    public class MultiLanText
    {

        public bool isTextComponent = false;
        public Text textComponent;
        public TextMeshProUGUI tmpComponent;
        public TMP_SubMeshUI subMeshUI;
        public bool isBasicMaterial = false;
        public string key;
        public object[] args;

        public void SetInfo(Text txt, string key, object[] args)
        {
            textComponent = txt;
            this.key = key;
            this.args = args;
            isTextComponent = true;
        }

        public void SetInfo(TextMeshProUGUI txt, string key, object[] args)
        {
            tmpComponent = txt;
            this.key = key;
            this.args = args;
            if (txt.fontMaterial.name == "BRLNSDB SDF Material (Instance)")
                isBasicMaterial = true;
            SetFont();
        }

        public void SetFont()
        {
            if (tmpComponent == null) return;
            if (subMeshUI == null && tmpComponent.transform.childCount > 0)
                subMeshUI = tmpComponent.transform.GetChild(0)?.GetComponent<TMP_SubMeshUI>();

            if (isBasicMaterial)
            {
                tmpComponent.font = LanguageHelper.tmpFontAsset;
                return;
            }
            if (subMeshUI != null)
                subMeshUI.fontAsset = LanguageHelper.tmpFontAsset;
            else
                if (TMP_Settings.defaultFontAsset != LanguageHelper.tmpFontAsset)
                Log.Warning($"LanguageHelper 设置 {tmpComponent.name} 多语言失败，没有找到FallBackAsset : {LanguageHelper.tmpFontAsset}");
        }
    }

    /// <summary>
    /// 多语言工具辅助类
    /// </summary>
    public static class LanguageHelper
    {
        // 缓存文本组件，用于热更新语言
        private static Dictionary<int, MultiLanText> textDic = new Dictionary<int, MultiLanText>();

        public static Language systemLanguage => GameFrameworkEntry.GetModule<ILocalizationManager>().SystemLanguage;

        public static Language CurLan;
        public static TMP_FontAsset tmpFontAsset;

        private static void SetLanguageFonts()
        {
            if (CurLan == Language.ChineseSimplified || CurLan == Language.ChineseTraditional || CurLan == Language.Japanese)
            {
                tmpFontAsset = TMP_Settings.fallbackFontAssets[0];
            }
            else if (CurLan == Language.Korean)
            {
                tmpFontAsset = TMP_Settings.fallbackFontAssets[1];
            }
            else
            {
                tmpFontAsset = TMP_Settings.defaultFontAsset;
            }
        }

        public static string GetString(string maybeKey)
        {
            if (string.IsNullOrEmpty(maybeKey)) { return string.Empty; }
            CurLan = GameEntry.Base.EditorLanguage;
            string val;
            if (CurLan == Language.Unspecified)
                val = GameEntry.Data.GetData<LanguageData>().GetString(maybeKey, systemLanguage);
            else
                val = GameEntry.Data.GetData<LanguageData>().GetString(maybeKey, CurLan);
            if (string.IsNullOrEmpty(val))
                return maybeKey;
            else
            {
                val = val.Replace("\\n", "\n");
                if (CurLan == Language.ChineseSimplified || CurLan == Language.ChineseTraditional)
                    val = val.Replace(" ", "\u00A0");
                return val;
            }

        }

        public static void SetText(Text textComponent, string maybeKey, params object[] args)
        {
            if (textComponent == null) { return; }
            string val = GetString(maybeKey);
            if (!val.Equals(maybeKey))
            {
                int uuid = textComponent.GetInstanceID();
                MultiLanText multiLanText;
                textDic.TryGetValue(uuid, out multiLanText);
                if (multiLanText == null)
                {
                    multiLanText = new MultiLanText();
                    textDic.Add(uuid, multiLanText);
                }
                multiLanText.SetInfo(textComponent, maybeKey, args);
            }
            textComponent.text = string.Format(val, args);
        }

        public static void SetText(TextMeshProUGUI textComponent, string maybeKey, params object[] args)
        {
            if (textComponent == null) { return; }
            string val = GetString(maybeKey);
            if (!val.Equals(maybeKey))
            {
                int uuid = textComponent.GetInstanceID();
                MultiLanText multiLanText;
                textDic.TryGetValue(uuid, out multiLanText);
                if (multiLanText == null)
                {
                    multiLanText = new MultiLanText();
                    textDic.Add(uuid, multiLanText);
                }
                multiLanText.SetInfo(textComponent, maybeKey, args);
            }
            try
            {
                textComponent.text = string.Format(GetString(maybeKey), args);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Multilanguage Error : exception" + exception.ToSafeString());
                textComponent.text = maybeKey;
            }
        }

        /// <summary>
        /// 语言环境更改时，动态更新缓存中的文本
        /// </summary>
        public static void UpdateTextView()
        {
            if (CurLan == GameEntry.Base.EditorLanguage) return;
            CurLan = GameEntry.Base.EditorLanguage;
            SetLanguageFonts();
            foreach (var item in textDic)
            {
                MultiLanText text = item.Value;
                if (text.isTextComponent)
                    SetText(text.textComponent, text.key, text.args);
                else
                {
                    text.SetFont();
                    SetText(text.tmpComponent, text.key, text.args);
                }
            }
        }
    }
}
