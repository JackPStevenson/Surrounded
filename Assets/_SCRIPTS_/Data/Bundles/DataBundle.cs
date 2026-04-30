using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DataBundle : ScriptableObject {
    protected static T[] GetItems<T, TBase>(List<TBase> items, bool sorted = false) where T : TBase where TBase : DataDisplayable {
        T[] values = items.OfType<T>().ToArray();
        if (sorted) Array.Sort(values);
        return  values;
    }

    protected static T GetFirst<T, TBase>(List<TBase> items, int itemLevel = 0) where T : TBase where TBase : DataDisplayable =>  GetFirst<T, TBase>(items, false, itemLevel);
    protected static T GetFirst<T, TBase>(List<TBase> items, bool sorted = false) where T : TBase where TBase : DataDisplayable =>  GetFirst<T, TBase>(items, sorted, 0);
    private static T GetFirst<T, TBase>(List<TBase> items, bool sorted, int itemLevel) where T : TBase where TBase : DataDisplayable {
        return (itemLevel < 1) ? GetItems<T, TBase>(items, sorted).FirstOrDefault() : GetItems<T, TBase>(items).FirstOrDefault(w => w.levelToUnlock == itemLevel);
    }
}