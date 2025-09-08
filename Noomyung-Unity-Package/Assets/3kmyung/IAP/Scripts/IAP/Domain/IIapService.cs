using System;
using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.IAP.Domain
{
    public sealed class PurchaseResult
    {
        public string ProductId { get; }

        public bool Success { get; }

        public string Receipt { get; }

        public string ErrorMessage { get; }

        public PurchaseResult(string productId, bool success, string receipt, string errorMessage)
        {
            ProductId = productId;
            Success = success;
            Receipt = receipt;
            ErrorMessage = errorMessage;
        }
    }

    public interface IIapService : IDisposable
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);

        Task<PurchaseResult> PurchaseAsync(string productId, CancellationToken cancellationToken = default);

        Task RestorePurchasesAsync(CancellationToken cancellationToken = default);

        Task<bool> ValidateReceiptAsync(string receipt, CancellationToken cancellationToken = default);
    }
}
