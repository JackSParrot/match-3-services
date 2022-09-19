using System.Collections;
using Game.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreView : MonoBehaviour
{
    private GameConfigService      _gameConfig;
    private GameProgressionService _gameProgression;
    private AdsGameService         _adsService;
    private IIAPGameService        _iapService;

    [SerializeField]
    private Button _buyBoosterButton;
    [SerializeField]
    private Button _buyGoldButton;
    [SerializeField]
    private Button _buyIAPGemsButton;
    [SerializeField]
    private Button _buyAdGemsButton;
    [SerializeField]
    private TMP_Text _boosterCostText = null;
    [SerializeField]
    private TMP_Text _goldCostText = null;
    [SerializeField]
    private TMP_Text _goldAmountText = null;
    [SerializeField]
    private TMP_Text _adsGemsText = null;
    [SerializeField]
    private TMP_Text _iapGemsText = null;
    [SerializeField]
    private TMP_Text _iapGemsCostText = null;

    private void Awake()
    {
        _gameConfig = ServiceLocator.GetService<GameConfigService>();
        _gameProgression = ServiceLocator.GetService<GameProgressionService>();
        _adsService = ServiceLocator.GetService<AdsGameService>();
        _iapService = ServiceLocator.GetService<IIAPGameService>();
    }

    private void Start()
    {
        _boosterCostText.text = _gameConfig.GoldPerBooster.ToString();
        _goldCostText.text = _gameConfig.GoldPackCostInGems.ToString();
        _goldAmountText.text = _gameConfig.GoldInGoldPack.ToString();
        _adsGemsText.text = _gameConfig.GemsPerAd.ToString();
        _iapGemsText.text = _gameConfig.GemsPerIAP.ToString();
        _iapGemsCostText.text = "Loading...";
        _gameProgression.OnInventoryChanged += UpdateCards;

        UpdateCards();
    }

    private void UpdateCards()
    {
        _buyBoosterButton.interactable = _gameProgression.Gold >= _gameConfig.GoldPerBooster;
        _boosterCostText.color = _gameProgression.Gold >= _gameConfig.GoldPerBooster ? Color.white : Color.red;

        _buyGoldButton.interactable = _gameProgression.Gems >= _gameConfig.GoldPackCostInGems;
        _goldCostText.color = _gameProgression.Gems >= _gameConfig.GoldPackCostInGems ? Color.white : Color.red;

        _buyAdGemsButton.interactable = true;
        StopAllCoroutines();
        if (!_adsService.IsAdReady)
        {
            _buyAdGemsButton.interactable = false;
            StartCoroutine(WaitForAdReady());
        }

        _buyIAPGemsButton.interactable = true;
        if (!_iapService.IsReady())
        {
            _buyIAPGemsButton.interactable = false;
            StartCoroutine(WaitForIAPReady());
        }
    }

    IEnumerator WaitForAdReady()
    {
        while (!_adsService.IsAdReady)
        {
            yield return new WaitForSeconds(0.5f);
        }

        _buyAdGemsButton.interactable = true;
    }

    IEnumerator WaitForIAPReady()
    {
        while (!_iapService.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        _buyIAPGemsButton.interactable = true;
        _iapGemsCostText.text = _iapService.GetLocalizedPrice("test1");
    }

    public void PurchaseBooster()
    {
        _gameProgression.BoosterAmount++;
        _gameProgression.UpdateGold(-_gameConfig.GoldPerBooster);
        UpdateCards();
    }

    public void PurchaseGold()
    {
        _gameProgression.UpdateGold(_gameConfig.GoldInGoldPack);
        _gameProgression.UpdateGems(-_gameConfig.GoldPackCostInGems);
        UpdateCards();
    }

    public async void PurchaseIAPGems()
    {
        if (await _iapService.StartPurchase("test1"))
        {
            _gameProgression.UpdateGems(_gameConfig.GemsPerIAP);
            UpdateCards();
        }
        else
        {
            Debug.LogError("Purchase failed");
        }
    }

    public async void PurchaseAdGems()
    {
        if (await ServiceLocator.GetService<AdsGameService>().ShowAd())
        {
            _gameProgression.UpdateGems(_gameConfig.GemsPerAd);
            UpdateCards();
        }
        else
        {
            Debug.LogError("ad failed");
        }
    }
}