using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LanguajeDictionary : ScriptableObject
{
    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }

    [SerializeField]
    private List<LocalizationEntry> data = new List<LocalizationEntry>();

    [SerializeField]
    private TextAsset _textAsset;

    public void Initialize()
    {
        JsonUtility.FromJsonOverwrite(_textAsset.text, this);
    }

    public string Localize(string key)
    {
        return data.Find(x => x.key == key)?.value ?? key;
    }
}