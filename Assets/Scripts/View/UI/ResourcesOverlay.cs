using Game.Services;
using TMPro;
using UnityEngine;

public class ResourcesOverlay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _goldText = null;
    [SerializeField]
    private TMP_Text _gemsText = null;

    private GameProgressionService _gameProgressionService = null;

    void Awake()
    {
        _gameProgressionService = ServiceLocator.GetService<GameProgressionService>();
        _goldText.text = _gameProgressionService.Gold.ToString();
        _gemsText.text = _gameProgressionService.Gems.ToString();
        _gameProgressionService.OnInventoryChanged += UpdateInventories;
        UpdateInventories();
    }

    private void OnDestroy()
    {
        _gameProgressionService.OnInventoryChanged -= UpdateInventories;
    }

    private void UpdateInventories()
    {
        _goldText.text = _gameProgressionService.Gold.ToString();
        _gemsText.text = _gameProgressionService.Gems.ToString();
    }
}