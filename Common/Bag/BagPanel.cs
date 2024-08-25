using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 背包面板，主要用来更新背包逻辑
/// </summary>
public class BagPanel : BaseUI
{
    // 装格子的容器
    private RectTransform contentCanvas;
    // 列数
    public int cols = 3;
    // 格子大小
    public int gridSize = 64;
    // 间隔
    public int spacing = 10;
    private CustomSV<BagItemInfo, BagItem> sv;

    protected override void OnInit()
    {
        contentCanvas = GetComponentInChildren<RectTransform>("Content");
        float viewportHeight = GetComponentInChildren<RectTransform>("Scroll View").rect.size.y;
        sv = new CustomSV<BagItemInfo, BagItem>(contentCanvas, viewportHeight, gridSize, spacing, cols, "Prefabs/UI/BagItem");
        sv.SetDataInfo(BagMgr.Instance.bagItemArr);
    }

    void Update()
    {
        sv.CheckShowOrHide();
    }
}
