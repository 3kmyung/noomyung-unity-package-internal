using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.IAP.Domain;

#if PLAYFAB_PACKAGE
using PlayFab;
using PlayFab.ClientModels;
#endif

namespace Noomyung.IAP.Infrastructure
{
    public sealed class PlayFabIapService : IIapService
    {
        public PlayFabIapService()
        {
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            // Typically no special init required beyond PlayFab login.
            return Task.CompletedTask;
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return Task.CompletedTask;
#endif
        }

        public async Task<PurchaseResult> PurchaseAsync(string productId, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            // Client-side purchase flows are platform specific and often go through store SDKs.
            // Here we provide a minimal placeholder that "succeeds".
            await Task.Yield();

            return new PurchaseResult(productId, true, "RECEIPT_PLACEHOLDER", null);
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return new PurchaseResult(productId, false, null, "PlayFab not available");
#endif
        }

        public Task RestorePurchasesAsync(CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            // Platform-specific restore logic.
            return Task.CompletedTask;
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return Task.CompletedTask;
#endif
        }

        public Task<bool> ValidateReceiptAsync(string receipt, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            // Server-side validation recommended; placeholder returns true if non-empty.
            return Task.FromResult(!string.IsNullOrEmpty(receipt));
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return Task.FromResult(false);
#endif
        }

        public void Dispose()
        {
        }
    }
}
