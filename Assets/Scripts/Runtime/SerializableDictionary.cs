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
    
    public SerializableDictionary()
    {
        dictionary = new Dictionary<TKey, TValue>();
    }

    public TValue this[TKey key]
    {
        get
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in SerializableDictionary.");
        }
        set
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                int index = keys.IndexOf(key);
                if (index >= 0) values[index] = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }
    
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
        
        // Debug.Log($"Before Serialize: Keys={keys.Count}, Values={values.Count}, Dict={dictionary.Count}");
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();

        keys = keys ?? new List<TKey>();
        values = values ?? new List<TValue>();
        
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            if (!dictionary.ContainsKey(keys[i]))
            {
                dictionary[keys[i]] = values[i];
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            keys.Add(key);
            values.Add(value);
        }
    }

    public bool Remove(TKey key)
    {
        if (dictionary.Remove(key))
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
            return true;
        }
        return false;
    }

}
