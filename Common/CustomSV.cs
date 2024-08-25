using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据源接口
/// </summary>
/// <typeparam name="T">数据源类型</typeparam>
public interface ItemBase<T>
{
    public void SetInfo(T info) { }
}

/// <summary>
/// 自定义ScrollView类，用于节约内存与性能，通过缓存池复用对象支持无限个item显示
/// </summary>
/// <typeparam name="T">数据源</typeparam>
/// <typeparam name="K">格子类</typeparam>
public class CustomSV<T, K> where K : ItemBase<T>
{
    private RectTransform contentCanvas;
    private float viewportHeight;
    public int cols = 3;
    public int gridSize = 64;
    public int spacing = 10;
    private int totalSize = 74;
    private Dictionary<int, GameObject> itemDic = new Dictionary<int, GameObject>();
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;
    private string resPath;

    // 数据
    private List<T> itemInfoArr;

    /// <summary>
    /// 设置数据源并更新内容容器的高度
    /// </summary>
    /// <param name="itemInfos"></param>
    public void SetDataInfo(List<T> itemInfos)
    {
        itemInfoArr = itemInfos;
        contentCanvas.sizeDelta = new Vector2(0, Mathf.CeilToInt(itemInfos.Count / cols) * totalSize);
    }

    /// <summary>
    /// 初始化父对象及可视范围的高
    /// </summary>
    /// <param name="contentCanvas">内容容器</param>
    /// <param name="viewportHeight">可视范围</param>
    /// <param name="gridSize">格子规格</param>
    /// <param name="spacing">格子间间隔</param>
    /// <param name="cols">列数</param>
    public CustomSV(RectTransform contentCanvas, float viewportHeight, int gridSize, int spacing, int cols, string resPath)
    {
        totalSize = gridSize + spacing;
        this.contentCanvas = contentCanvas;
        this.viewportHeight = viewportHeight + totalSize;
        this.gridSize = gridSize;
        this.spacing = spacing;
        this.cols = cols;
        this.resPath = resPath;
    }

    /// <summary>
    /// 更新格子的显示
    /// </summary>
    public void CheckShowOrHide()
    {
        // 无数据判断
        if (itemInfoArr == null || itemInfoArr.Count == 0) { return; }
        // 边界判断
        if (contentCanvas.anchoredPosition.y < -1) { return; }

        // 检测哪些格子被显示出来
        int minIndex = (int)(contentCanvas.anchoredPosition.y / totalSize) * cols;
        // 没有变化不更新
        if (minIndex == oldMinIndex) { return; }
        int maxIndex = (int)((contentCanvas.anchoredPosition.y + viewportHeight) / totalSize) * cols + cols - 1;

        // 边界判断
        if (maxIndex >= itemInfoArr.Count)
        {
            maxIndex = itemInfoArr.Count - 1;
        }

        // 回收多余部分
        for (int i = oldMinIndex; i < minIndex; ++i)
        {
            if (itemDic.ContainsKey(i))
            {
                if (itemDic[i] != null)
                    PoolMgr.Instance.PushObj(itemDic[i]);
                itemDic.Remove(i);
            }
        }

        for (int i = maxIndex + 1; i <= oldMaxIndex; ++i)
        {
            if (itemDic.ContainsKey(i))
            {
                if (itemDic[i] != null)
                    PoolMgr.Instance.PushObj(itemDic[i]);
                itemDic.Remove(i);
            }
        }

        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        // 创建指定范围内的格子
        for (int i = minIndex; i <= maxIndex; ++i)
        {
            if (itemDic.ContainsKey(i))
            {
                continue;
            }
            // 创建
            GameObject item = PoolMgr.Instance.GetObj(resPath);
            // 设置父对象
            item.transform.SetParent(contentCanvas);
            // 重置缩放
            item.transform.localScale = Vector3.one;
            // 重置位置
            item.transform.localPosition = new Vector3(i % cols * totalSize, -(i / cols + 1) * totalSize, 0);
            // 更新信息
            item.GetComponent<K>().SetInfo(itemInfoArr[i]);
            itemDic.Add(i, item);
        }
    }
}
