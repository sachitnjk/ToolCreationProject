using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();
    
    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    
    public Dictionary<TKey, TValue> Dictionary => dictionary;
    public List<TKey> GetKeys() => keys;
    public List<TValue> GetValues() => values;
    
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary = new Dictionary<TKey, TValue>();

        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            if (!dictionary.ContainsKey(keys[i]))
            {
                dictionary.Add(keys[i], values[i]);
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
        }
    }

    public bool Remove(TKey key)
    {
        return dictionary.Remove(key);
    }

}
