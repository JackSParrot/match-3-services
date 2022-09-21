using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Game.Services
{
    public class UnityIAPGameService : IIAPGameService, IStoreListener
    {
        private bool             _isInitialized        = false;
        private IStoreController _unityStoreController = null;
        private TaskStatus       _purchaseTaskStatus   = TaskStatus.Created;
        private TaskStatus       _initializeTaskStatus = TaskStatus.Created;


        public async Task Initialize(Dictionary<string, string> products)
        {
            _isInitialized = false;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (KeyValuePair<string, string> productEntry in products)
            {
                var ids = new IDs();
                ids.Add(productEntry.Value, new[] { GooglePlay.Name });
                builder.AddProduct(productEntry.Key, ProductType.Consumable, ids);
            }

            _initializeTaskStatus = TaskStatus.Running;
            UnityPurchasing.Initialize(this, builder);
            while (_initializeTaskStatus == TaskStatus.Running)
            {
                await Task.Delay(100);
            }
        }

        public bool IsReady() => _isInitialized;

        public async Task<bool> StartPurchase(string product)
        {
            if (!_isInitialized)
                return false;

            _purchaseTaskStatus = TaskStatus.Running;
            _unityStoreController.InitiatePurchase(product);

            while (_purchaseTaskStatus == TaskStatus.Running)
            {
                await Task.Delay(500);
            }

            return _purchaseTaskStatus == TaskStatus.RanToCompletion;
        }

        public string GetLocalizedPrice(string product)
        {
            if (!_isInitialized)
                return string.Empty;

            Product unityProduct = _unityStoreController.products.WithID(product);
            return unityProduct?.metadata?.localizedPriceString;
        }

        public void Clear()
        {
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _isInitialized = true;
            _unityStoreController = controller;
            _initializeTaskStatus = TaskStatus.RanToCompletion;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _isInitialized = false;
            _initializeTaskStatus = TaskStatus.RanToCompletion;
            Debug.LogError("Initialization failed with error: " + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            _purchaseTaskStatus = TaskStatus.RanToCompletion;
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError("Purchase failed with error: " + failureReason);
            _purchaseTaskStatus = TaskStatus.Faulted;
        }
    }
}