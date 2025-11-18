using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DataBundle : ScriptableObject {
    protected static T[] GetItems<T, TBase>(List<TBase> items) where T : TBase where TBase : DataDisplayable {
        return items.OfType<T>().ToArray();
    }
    
    protected static T GetFirst<T, TBase>(List<TBase> items) where T : TBase where TBase : DataDisplayable {
        return items.OfType<T>().FirstOrDefault();
    }
}