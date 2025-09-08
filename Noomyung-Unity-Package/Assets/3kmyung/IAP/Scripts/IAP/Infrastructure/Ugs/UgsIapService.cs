using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.IAP.Domain;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace Noomyung.IAP.Infrastructure
{
    public sealed class UgsIapService : IIapService
#if UNITY_PURCHASING
        , IStoreListener
#endif
    {
#if UNITY_PURCHASING
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        private bool _isInitialized;
        private TaskCompletionSource<bool> _initializationTcs;
        private TaskCompletionSource<PurchaseResult> _purchaseTcs;
#endif
        public UgsIapService()
        {
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_PURCHASING
            if (_isInitialized) return;

            try
            {
                _initializationTcs = new TaskCompletionSource<bool>();

                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                
                // 상품 ID들을 여기에 추가해야 합니다. 실제 구현에서는 설정 파일이나 상수에서 가져와야 합니다.
                // builder.AddProduct("product_id", ProductType.Consumable);

                UnityPurchasing.Initialize(this, builder);

                await _initializationTcs.Task;

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unity IAP Initialize failed. {ex.Message}");
                throw;
            }
#else
            Debug.LogError("UNITY_PURCHASING define is missing or Unity IAP package is not installed.");
            await Task.Yield();
#endif
        }

        public async Task<PurchaseResult> PurchaseAsync(string productId, CancellationToken cancellationToken = default)
        {
#if UNITY_PURCHASING
            try
            {
                if (!_isInitialized)
                {
                    return new PurchaseResult(productId, false, null, "IAP not initialized");
                }

                _purchaseTcs = new TaskCompletionSource<PurchaseResult>();

                _storeController.InitiatePurchase(productId);

                var result = await _purchaseTcs.Task;

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unity IAP Purchase failed. {ex.Message}");
                return new PurchaseResult(productId, false, null, ex.Message);
            }
#else
            Debug.LogError("UNITY_PURCHASING define is missing or Unity IAP package is not installed.");
            return new PurchaseResult(productId, false, null, "Unity IAP not available");
#endif
        }

        public Task RestorePurchasesAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_PURCHASING
            try
            {
                if (!_isInitialized)
                {
                    Debug.LogError("IAP not initialized");
                    return Task.CompletedTask;
                }

                // iOS에서만 복원이 필요합니다.
                if (Application.platform == RuntimePlatform.IPhonePlayer || 
                    Application.platform == RuntimePlatform.OSXPlayer)
                {
                    _extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Restore purchases failed. {ex.Message}");
            }
#else
            Debug.LogError("UNITY_PURCHASING define is missing or Unity IAP package is not installed.");
#endif
            return Task.CompletedTask;
        }

        public Task<bool> ValidateReceiptAsync(string receipt, CancellationToken cancellationToken = default)
        {
#if UNITY_PURCHASING
            try
            {
                if (string.IsNullOrEmpty(receipt))
                    return Task.FromResult(false);

                // Unity IAP의 CrossPlatformValidator를 사용한 영수증 검증
                var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
                var result = validator.Validate(receipt);
                
                return Task.FromResult(result != null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Receipt validation failed. {ex.Message}");
                return Task.FromResult(false);
            }
#else
            Debug.LogError("UNITY_PURCHASING define is missing or Unity IAP package is not installed.");
            return Task.FromResult(false);
#endif
        }

        public void Dispose()
        {
        }

#if UNITY_PURCHASING
        // IStoreListener 구현
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            _initializationTcs?.SetResult(true);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Unity IAP initialization failed: {error}");
            _initializationTcs?.SetException(new Exception($"IAP initialization failed: {error}"));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            try
            {
                var product = args.purchasedProduct;
                var receipt = product.receipt;
                
                var result = new PurchaseResult(product.definition.id, true, receipt, null);
                _purchaseTcs?.SetResult(result);
                
                return PurchaseProcessingResult.Complete;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Process purchase failed: {ex.Message}");
                var result = new PurchaseResult(args.purchasedProduct.definition.id, false, null, ex.Message);
                _purchaseTcs?.SetResult(result);
                
                return PurchaseProcessingResult.Complete;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"Purchase failed for product {product.definition.id}: {failureReason}");
            var result = new PurchaseResult(product.definition.id, false, null, failureReason.ToString());
            _purchaseTcs?.SetResult(result);
        }
#endif
    }
}
