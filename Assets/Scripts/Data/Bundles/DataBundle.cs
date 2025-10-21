using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DataBundle : ScriptableObject {
    /// Returns all items of type T found in list of type TBase, where T is type of TBase and TBase is type of DataDisplayable.
    protected T[] GetItems<T, TBase>(List<TBase> items) where T : TBase where TBase : DataDisplayable {
        return (from i in items where i.GetType() == typeof(T) select i as T).ToArray();
    }
}

