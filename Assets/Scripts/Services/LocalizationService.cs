using Game.Services;
using UnityEngine;

public class LocalizationService : IService
{
    private LanguajeDictionary _currentLanguaje;

    public void Initialize(string defaultLanguajeFile, bool forceLanguaje = false)
    {
        if (forceLanguaje)
        {
            _currentLanguaje = Resources.Load<LanguajeDictionary>(defaultLanguajeFile);
            _currentLanguaje?.Initialize();
            return;
        }

        _currentLanguaje = Resources.Load<LanguajeDictionary>(Application.systemLanguage.ToString()) ??
                           Resources.Load<LanguajeDictionary>(defaultLanguajeFile);
        _currentLanguaje?.Initialize();
    }

    public string Localize(string key)
    {
        return _currentLanguaje?.Localize(key) ?? key;
    }

    public void Clear()
    {
    }
}