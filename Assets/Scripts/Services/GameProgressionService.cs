using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Services
{
    
    [System.Serializable]
    public class GameProgressionService : IService
    {
        [SerializeField]
        private int _gold;
        public int Gold => _gold;

        public int Gems;
        public int BoosterAmount;
        public int CurrentLevel;

        public event Action OnInventoryChanged;

        private AnalyticsGameService _analytics;

        public void Initialize(GameConfigService gameConfig)
        {
            _analytics = ServiceLocator.GetService<AnalyticsGameService>();
            Load(gameConfig);
        }

        public void UpdateGold(int amount)
        {
            _gold += amount;
            OnInventoryChanged?.Invoke();
            Save();
            if (amount > 0)
                _analytics.SendEvent("receive_gold", new Dictionary<string, object>
                {
                    ["amount"] = amount
                });
        }

        public void UpdateGems(int amount)
        {
            Gems += amount;
            OnInventoryChanged?.Invoke();
            Save();
        }

        public void UpdateBooster(int amount)
        {
            BoosterAmount += amount;
            OnInventoryChanged?.Invoke();
            Save();
        }

//save and load
        private static string kSavePath = "/gameProgression.json";

        public void Save()
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + kSavePath, JsonUtility.ToJson(this));
        }

        private void Load(GameConfigService config)
        {
            if (System.IO.File.Exists(Application.persistentDataPath + kSavePath))
            {
                JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(Application.persistentDataPath + kSavePath),
                    this);
                return;
            }

            _gold = config.InitialGold;
            Gems = config.InitialGems;
            Save();
        }
//end of save and load

        public void Clear()
        {
        }
    }
}