/// <summary>
/// Custom Action Created by cnsoft 2013-11-20 
/// http://blog.donews.com/cnsoft or http://cnblogs.com/cnsoft 
/// This CusAction can be used to replace System.Action . because Actdion<int> will throw AOT only exception when Action += funcA .  
/// </summary>
//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CusAction<T>
{
    private List<System.Action<T>> m_Listeners = new List<System.Action<T>>();

    public static CusAction<T> operator +(CusAction<T> left, System.Action<T> right)
    {
        if (!left.m_Listeners.Contains(right))
            left.m_Listeners.Add(right);
        return left;
    }
    public static CusAction<T> operator -(CusAction<T> left, System.Action<T> right)
    {
        if (left.m_Listeners.Contains(right))
            left.m_Listeners.Remove(right);
        return left;
    }
    public void Invoke(T param)
    {
        foreach (var func in m_Listeners)
            func(param);
    }

}