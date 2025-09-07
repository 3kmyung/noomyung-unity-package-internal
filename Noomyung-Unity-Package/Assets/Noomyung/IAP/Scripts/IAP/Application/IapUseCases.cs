using System;
using System.Threading;
using System.Threading.Tasks;
using Noomyung.IAP.Domain;

namespace Noomyung.IAP.Application
{
    public sealed class IapUseCases : IDisposable
    {
        private readonly IIapService _iapService;

        public IapUseCases(IIapService iapService)
        {
            _iapService = iapService ?? throw new ArgumentNullException(nameof(iapService));
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            return _iapService.InitializeAsync(cancellationToken);
        }

        public Task<PurchaseResult> PurchaseAsync(string productId, CancellationToken cancellationToken = default)
        {
            return _iapService.PurchaseAsync(productId, cancellationToken);
        }

        public Task RestorePurchasesAsync(CancellationToken cancellationToken = default)
        {
            return _iapService.RestorePurchasesAsync(cancellationToken);
        }

        public Task<bool> ValidateReceiptAsync(string receipt, CancellationToken cancellationToken = default)
        {
            return _iapService.ValidateReceiptAsync(receipt, cancellationToken);
        }

        public void Dispose()
        {
            _iapService.Dispose();
        }
    }
}
