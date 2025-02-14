using System;
using UnityEngine;

public class RandomScript : MonoBehaviour
{
    public StringIntDictionary stringIntDictionary;

    public void OnValidate()
    {
        Debug.Log("Chungus cat");
    }
}

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}
