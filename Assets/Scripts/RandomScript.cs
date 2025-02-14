using System;
using UnityEngine;

public class RandomScript : MonoBehaviour
{
    //Keys that work: float, int
    public SerializableDictionary<int, string> stringIntDictionary;

    public void OnValidate()
    {
        Debug.Log("Chungus cat");
    }
}

