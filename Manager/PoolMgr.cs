using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolMgr : SingletonBase<PoolMgr>
{

    private Dictionary<string, Stack<GameObject>> poolDic = new Dictionary<string, Stack<GameObject>>();

    private PoolMgr() { }

    public GameObject GetObj(string name)
    {
        GameObject obj;
        if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        {
            obj = poolDic[name].Pop();
            obj.SetActive(true);
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
        }
        return obj;
    }

    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        if (!poolDic.ContainsKey(obj.name))
            poolDic.Add(obj.name, new Stack<GameObject>());
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 切场景需要清楚对象引用，方便GC回收对象，防止内存泄漏
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
    }
}
