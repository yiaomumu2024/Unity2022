using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

/// <summary>
/// 背包道具信息
/// </summary>
public class BagItemInfo
{
    public int id, num;
}

/// <summary>
/// 背包管理器
/// 主要管理背包的公共数据和公共方法
/// </summary>
public class BagMgr : SingletonBase<BagMgr>
{

    // 构造器私有化
    private BagMgr() { }

    public List<BagItemInfo> bagItemArr = new List<BagItemInfo>();

    public void InitItemInfo()
    {
        for (int i = 0; i < 1e6; i++)
        {
            BagItemInfo itemInfo = new BagItemInfo();
            itemInfo.id = i;
            itemInfo.num = i;
            bagItemArr.Add(itemInfo);
        }
    }
}
