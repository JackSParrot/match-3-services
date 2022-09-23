using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalizedAsset : MonoBehaviour
{
    [SerializeField]
    private string _key;

    private void Awake()
    {
        LocalizationService localization = ServiceLocator.GetService<LocalizationService>();
        string address = localization.Localize(_key);
        Addressables.InstantiateAsync(address, transform);
    }
}