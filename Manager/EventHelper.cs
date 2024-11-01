using System;
using System.Collections.Concurrent;
using System.Threading;

public static class EventHelper
{

    // 使用 ConcurrentDictionary 和 ConcurrentBag 确保线程安全。
    private static readonly ConcurrentDictionary<string, ConcurrentBag<Action<object[]>>> _eventDictionary = new ConcurrentDictionary<string, ConcurrentBag<Action<object[]>>>();

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    public static void Subscribe(string eventName, Action<object[]> listener)
    {
        var actions = _eventDictionary.GetOrAdd(eventName, _ => new ConcurrentBag<Action<object[]>>());
        actions.Add(listener);
    }

    /// <summary>
    /// 取消订阅事件
    /// </summary>
    /// <param name="eventName"></param>
    public static void Unsubscribe(string eventName, Action<object[]> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out var actions))
        {
            var updatedActions = new ConcurrentBag<Action<object[]>>(actions);
            foreach (var action in actions)
            {
                if (action == listener)
                {
                    updatedActions.TryTake(out _);
                }
            }
            _eventDictionary[eventName] = updatedActions;
        }
    }

    /// <summary>
    /// 派发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="eventParams">事件参数</param>
    public static void Dispatch(string eventName, params object[] eventParams)
    {
        if (_eventDictionary.TryGetValue(eventName, out var actions))
        {
            foreach (var action in actions)
            {
                // 为了保证事件中的循环触发可以安全进行，这里使用一个局部变量存储事件，避免在迭代时修改集合
                Action<object[]> currentAction = action;
                // ThreadPool.QueueUserWorkItem 用于在后台线程中处理事件，以避免阻塞主线程
                ThreadPool.QueueUserWorkItem(_ => currentAction.Invoke(eventParams));
            }
        }
    }
}
