using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DataBundle : ScriptableObject {
    protected static T[] GetItems<T, TBase>(List<TBase> items, bool sorted = false) where T : TBase where TBase : DataDisplayable {
        var values = items.OfType<T>().ToArray();
        if(sorted) Array.Sort(values);
        return values;
    }
    
    protected static T GetFirst<T, TBase>(List<TBase> items, bool sorted = false) where T : TBase where TBase : DataDisplayable {
        return GetItems<T, TBase>(items, sorted).FirstOrDefault();
    }
}