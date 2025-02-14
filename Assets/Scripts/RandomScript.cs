using System;
using UnityEngine;

public class RandomScript : MonoBehaviour
{
    public StringIntDictionary stringIntDictionary;
    // public SerializableDictionary<float, bool> intIntDictionary;

    public void OnValidate()
    {
        Debug.Log("Chungus cat");
    }
}

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, string> {}

// [Serializable]
// public class IntIntDictionary : SerializableDictionary<int, int> {}
