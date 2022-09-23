using Game.Services;
using TMPro;
using UnityEngine;

public class LocalizeText : MonoBehaviour
{
    [SerializeField]
    private string _key;

    private void Awake()
    {
        if (TryGetComponent(out TMP_Text text))
        {
            LocalizationService localization = ServiceLocator.GetService<LocalizationService>();
            text.text = localization.Localize(string.IsNullOrEmpty(_key) ? text.text : _key);
        }
    }
}